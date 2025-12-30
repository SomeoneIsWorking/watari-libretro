using System.Text.Json;
using watari_libretro.Types;
using Watari;
using SeparateProcess;
using Microsoft.Extensions.Logging;
using watari_libretro.libretro;

namespace watari_libretro;

public class LibretroApplication
{
    private readonly ILogger logger;
    private readonly WatariContext context;
    private readonly Dictionary<uint, bool> buttonStates = [];
    private readonly CoreManager coreManager;
    private readonly GameManager gameManager;
    private readonly SystemManager systemManager;
    private SteamGridDBApi? _steamGridDBApi;
    private LibretroService? runner;
    public event Action<FrameData> OnFrameReceived = delegate { };
    public event Action<DownloadProgress> OnDownloadProgress = delegate { };
    public event Action<string> OnDownloadComplete = delegate { };

    public LibretroApplication(WatariContext context, ILogger<LibretroApplication> logger)
    {
        this.context = context;
        this.logger = logger;
        coreManager = new CoreManager(context, logger);
        gameManager = new GameManager(context);
        systemManager = new SystemManager(coreManager);
        coreManager.OnDownloadProgress += (p) => OnDownloadProgress?.Invoke(p);
        coreManager.OnDownloadComplete += (n) => OnDownloadComplete?.Invoke(n);
        var settings = GetSettings();
        _steamGridDBApi = !string.IsNullOrEmpty(settings.SteamGridDBApiKey) ? new SteamGridDBApi(settings.SteamGridDBApiKey) : null;
    }

    public async Task DownloadCore(string name) => await coreManager.DownloadCore(name);

    public void RemoveCore(string name) => coreManager.RemoveCore(name);

    public async Task<List<CoreInfo>> ListCoreInfos() => await coreManager.GetCores();

    public void SaveLibrary(List<GameInfo> games) => gameManager.SaveLibrary(games);

    public List<GameInfo> LoadLibrary() => gameManager.LoadLibrary();

    public async Task AddGame(GameInfo game)
    {
        var systems = await GetSystems();
        if (!systems.Any(s => s.Name == game.SystemName))
        {
            throw new Exception($"Unknown system: {game.SystemName}");
        }
        gameManager.AddGame(game);
    }

    public void RemoveGame(string path) => gameManager.RemoveGame(path);

    public void RenameGame(string path, string newName) => gameManager.RenameGame(path, newName);

    public async Task<List<SystemInfo>> GetSystems() => await systemManager.GetSystems();

    public string? GetCover(string identifier)
    {
        var fullPath = context.PathCombine($"config/covers/{identifier}.png");
        if (!File.Exists(fullPath)) return null;
        var bytes = File.ReadAllBytes(fullPath);
        return Convert.ToBase64String(bytes);
    }

    public AppSettings GetSettings()
    {
        var settingsPath = context.PathCombine("config", "settings.json");
        if (File.Exists(settingsPath))
        {
            var json = File.ReadAllText(settingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        return new AppSettings();
    }

    public void SaveSettings(AppSettings settings)
    {
        var settingsPath = context.PathCombine("config", "settings.json");
        Directory.CreateDirectory(Path.GetDirectoryName(settingsPath)!);
        var json = JsonSerializer.Serialize(settings);
        File.WriteAllText(settingsPath, json);
        _steamGridDBApi = !string.IsNullOrEmpty(settings.SteamGridDBApiKey) ? new SteamGridDBApi(settings.SteamGridDBApiKey) : null;
    }

    public async Task<List<CoverOption>> SearchCovers(string name)
    {
        if (_steamGridDBApi == null) throw new Exception("SteamGridDB API not configured. Set STEAMGRIDDB_API_KEY environment variable.");
        var options = await _steamGridDBApi.Search(name);
        return options;
    }

    public async Task DownloadSystemCover(string systemName, string fullUrl)
    {
        var systems = await GetSystems();
        var system = systems.Find(s => s.Name == systemName)
            ?? throw new Exception($"System not found: {systemName}");
        var coverPath = Path.Combine(coreManager.GetCoversDir(), $"{system.CoverName}.png");
        Directory.CreateDirectory(Path.GetDirectoryName(coverPath)!);
        using var client = new HttpClient();
        var response = await client.GetAsync(fullUrl);
        if (response.IsSuccessStatusCode)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var fileStream = File.Create(coverPath);
            await stream.CopyToAsync(fileStream);
        }
    }

    public async Task DownloadGameCover(string gameId, string fullUrl)
    {
        var games = LoadLibrary();
        var game = games.Find(g => g.Path == gameId)
            ?? throw new Exception($"Game not found: {gameId}");
        var coverPath = Path.Combine(coreManager.GetCoversDir(), $"{game.CoverName}.png");
        Directory.CreateDirectory(Path.GetDirectoryName(coverPath)!);
        using var client = new HttpClient();
        var response = await client.GetAsync(fullUrl);
        if (response.IsSuccessStatusCode)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var fileStream = File.Create(coverPath);
            await stream.CopyToAsync(fileStream);
        }
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

        // Load and set saved core options
        var savedOptions = coreManager.LoadCoreOptionValues(name);
        await runner.SetCoreOptions(savedOptions);
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
        if (runner == null)
        {
            throw new Exception("Load core first");
        }
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

    public Dictionary<string, string> GetCoreOptions(string coreId) => coreManager.GetCoreOptions(coreId);

    public Dictionary<string, string> LoadCoreOptionValues(string coreId) => coreManager.LoadCoreOptionValues(coreId);

    public void SaveCoreOptionValues(string coreId, Dictionary<string, string> values) => coreManager.SaveCoreOptionValues(coreId, values);
}