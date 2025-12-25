using System.IO.Compression;
using watari_libretro.Types;
using Watari;

namespace watari_libretro;

public class LibretroApplication(WatariContext context)
{
    private readonly string coresDir = context.PathCombine("cores");
    private readonly Dictionary<uint, bool> buttonStates = [];
    private RunnerManager<LibretroHandler>? runnerManager;
    public event Action<FrameData> OnFrameReceived = delegate { };
    public event Action<DownloadProgress> OnDownloadProgress = delegate { };
    public event Action<string> OnDownloadComplete = delegate { };

    public async Task DownloadCore(string name)
    {
        var zipPath = Path.Combine(coresDir, $"{name}.zip");
        Directory.CreateDirectory(coresDir);
        await DoDownloadCore(name, zipPath);

        ZipFile.ExtractToDirectory(zipPath, coresDir, true);
        var dylibPath = Path.Combine(coresDir, $"{name}_libretro.dylib");
        File.Delete(zipPath);
        OnDownloadComplete(name);
    }

    private async Task DoDownloadCore(string name, string zipPath)
    {
        using var client = new HttpClient();
        var url = $"https://buildbot.libretro.com/nightly/apple/osx/arm64/latest/{name}_libretro.dylib.zip";
        var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var total = response.Content.Headers.ContentLength;
        using var stream = await response.Content.ReadAsStreamAsync();
        using var fileStream = File.Create(zipPath);
        var buffer = new byte[8192];
        int bytesRead;
        long totalRead = 0;
        while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            totalRead += bytesRead;
            if (total.HasValue)
            {
                var progress = (double)totalRead / total.Value * 100;
                OnDownloadProgress(new DownloadProgress { Name = name, Progress = progress });
            }
        }
    }

    public string[] ListAvailableCores() => new[]
    {
        "snes9x", "gambatte", "genesis_plus_gx", "nestopia", "fceumm", "mednafen_psx", "mgba", "bsnes"
    };


    public async Task LoadCore(string name)
    {
        var dylibPath = Path.Combine(coresDir, $"{name}_libretro.dylib");
        if (!File.Exists(dylibPath))
            throw new Exception("Download the core first");

        Console.WriteLine($"[Main] Loading core: {dylibPath}");
        if (runnerManager != null)
        {
            await runnerManager.Stop();
            runnerManager = null;
        }

        runnerManager = new RunnerManager<LibretroHandler>();
        await runnerManager.StartRunner();

        // Register event handlers
        runnerManager.On(x => x.OnFrame, OnFrameReceived);
        runnerManager.On(x => x.OnAudio, OnAudioReceived);

        await runnerManager.Call(x => x.LoadCore(dylibPath));
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

    public async Task LoadGame(string gamePath)
    {
        if (runnerManager == null) throw new Exception("Load core first");
        await runnerManager.Call(x => x.LoadGame(gamePath));

        // Initialize audio with correct sample rate
        var sampleRate = await runnerManager.Call(x => x.GetSampleRate());
        context.Application.InitAudio(sampleRate);
    }

    public async Task Run()
    {
        if (runnerManager == null)
        {
            throw new Exception("Load core and game first");
        }
        await runnerManager.Call(x => x.Run());
    }

    public IEnumerable<string> ListDownloadedCores()
    {
        Directory.CreateDirectory(coresDir);
        var files = Directory.GetFiles(coresDir, "*_libretro.dylib");
        return files.Select(f => Path.GetFileNameWithoutExtension(f).Replace("_libretro", ""));
    }

    public async Task SendKeyDown(string key)
    {
        Console.WriteLine($"Key Down: {key}");
        try
        {
            var id = (uint)(retro_device_id_joypad)Enum.Parse(typeof(retro_device_id_joypad), key);
            buttonStates[id] = true;
            if (runnerManager != null)
            {
                await runnerManager.Call(x => x.SetInput(key, true));
            }
        }
        catch
        {
            // Invalid key, ignore
        }
    }

    public async Task SendKeyUp(string key)
    {
        Console.WriteLine($"Key Up: {key}");
        try
        {
            var id = (uint)(retro_device_id_joypad)Enum.Parse(typeof(retro_device_id_joypad), key);
            buttonStates[id] = false;
            if (runnerManager != null)
            {
                await runnerManager.Call(x => x.SetInput(key, false));
            }
        }
        catch
        {
            // Invalid key, ignore
        }
    }
}
