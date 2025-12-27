using System.IO.Compression;
using System.Text.Json;
using watari_libretro.Types;
using Watari;
using SeparateProcess;
using Microsoft.Extensions.Logging;

namespace watari_libretro;

public class LibretroApplication
{
    private readonly ILogger logger;
    private readonly WatariContext context;
    private readonly Dictionary<uint, bool> buttonStates = [];
    private readonly CoreManager coreManager;
    private readonly GameManager gameManager;
    private readonly ManifestManager manifestManager;
    private LibretroService? runner;
    public event Action<FrameData> OnFrameReceived = delegate { };
    public event Action<DownloadProgress> OnDownloadProgress = delegate { };
    public event Action<string> OnDownloadComplete = delegate { };

    public LibretroApplication(WatariContext context, ILogger<LibretroApplication> logger)
    {
        this.context = context;
        this.logger = logger;
        coreManager = new CoreManager(context);
        gameManager = new GameManager(context);
        manifestManager = new ManifestManager(context);
        coreManager.OnDownloadProgress += (p) => OnDownloadProgress?.Invoke(p);
        coreManager.OnDownloadComplete += (n) => OnDownloadComplete?.Invoke(n);
    }

    public async Task DownloadCore(string name) => await coreManager.DownloadCore(name);

    public async Task<CoreInfo[]> ListCoreInfos() => await coreManager.ListCoreInfos();

    public IEnumerable<string> ListDownloadedCores() => coreManager.ListDownloadedCores();

    public void SaveLibrary(List<GameInfo> games) => gameManager.SaveLibrary(games);

    public List<GameInfo> LoadLibrary() => gameManager.LoadLibrary();

    public void AddGame(GameInfo game) 
    {
        var systems = GetSystems();
        if (!systems.Any(s => s.Name == game.SystemId))
        {
            throw new Exception($"Unknown system: {game.SystemId}");
        }
        gameManager.AddGame(game);
    }

    public void RemoveGame(string path) => gameManager.RemoveGame(path);

    public List<SystemInfo> GetSystems() => manifestManager.GetSystems();

    public GameMetadata? GetGameMetadata(string gamePath)
    {
        // Can be called without loading core
        var reader = new RomMetadataReader();
        return reader.ReadMetadata(gamePath);
    }

    public async Task LoadCore(string name)
    {
        var dylibPath = Path.Combine(coreManager.GetCoresDir(), $"{name}_libretro.dylib");
        if (!File.Exists(dylibPath))
            throw new Exception("Download the core first");

        logger.LogInformation("Loading core: {DylibPath}", dylibPath);
        if (runner != null)
        {
            await runner.StopAsync();
            runner = null;
        }

        runner = await Spawner.Spawn<LibretroService>(logger);

        // Register event handlers
        runner.OnFrame += OnFrameReceived;
        runner.OnAudio += OnAudioReceived;

        await runner.LoadCore(dylibPath);
    }

    private void OnAudioReceived(AudioData audioData)
    {
        // Samples is now byte[]
        var bytes = audioData!.Samples;
        var samples = new short[bytes.Length / 2];
        Buffer.BlockCopy(bytes, 0, samples, 0, bytes.Length);
        // Stream audio to application
        context.Application.PlayAudio(samples);
    }

    public void LoadGame(string gamePath)
    {
        if (runner == null) throw new Exception("Load core first");
        runner.LoadGame(gamePath);

        // Initialize audio with correct sample rate
        var sampleRate = runner.GetSampleRate();
        logger.LogInformation("Initializing audio with sample rate: {SampleRate}", sampleRate);
        context.Application.InitAudio(sampleRate);
    }

    public async Task Run()
    {
        if (runner == null)
        {
            throw new Exception("Load core and game first");
        }
        await runner.StartAsync();
    }

    public async Task SendKeyDown(string key)
    {
        logger.LogDebug("Key Down: {Key}", key);
        try
        {
            var id = (uint)(retro_device_id_joypad)Enum.Parse(typeof(retro_device_id_joypad), key);
            buttonStates[id] = true;
            if (runner != null)
            {
                await runner.SetInput(key, true);
            }
        }
        catch
        {
            // Invalid key, ignore
        }
    }

    public async Task SendKeyUp(string key)
    {
        logger.LogDebug("Key Up: {Key}", key);
        try
        {
            var id = (uint)(retro_device_id_joypad)Enum.Parse(typeof(retro_device_id_joypad), key);
            buttonStates[id] = false;
            if (runner != null)
            {
                await runner.SetInput(key, false);
            }
        }
        catch
        {
            // Invalid key, ignore
        }
    }

    public async Task Stop()
    {
        if (runner != null)
        {
            await runner.StopAsync();
            runner = null;
        }
    }
}