using System.IO.Compression;
using System.Runtime.InteropServices;
using watari_libretro.Types;
using Watari;

namespace watari_libretro;

public class LibretroApplication(WatariContext context)
{
    private RetroRunner? runner;
    private readonly string coresDir = context.PathCombine("cores");
    private readonly Dictionary<uint, bool> buttonStates = [];
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

        await Stop();

        var retro = new RetroWrapper();
        retro.OnFrame = (data, w, h, pitch) =>
        {
            var pixelBytes = PixelConverter.ConvertFrame(data, w, h, pitch, retro.PixelFormat);
            var base64 = Convert.ToBase64String(pixelBytes);
            OnFrameReceived(new FrameData
            {
                Pixels = base64,
                Width = (int)w,
                Height = (int)h,
                PixelFormat = "RGBA8888"
            });
        };
        retro.OnSample = (left, right) => { /* TODO: audio */ };
        retro.OnCheckInput = (port, device, index, id) =>
        {
            if (device == (uint)retro_device_type.RETRO_DEVICE_JOYPAD && port == 0 && index == 0)
            {
                return buttonStates.TryGetValue(id, out var pressed) && pressed ? (short)1 : (short)0;
            }
            return 0;
        };
        retro.LoadCore(dylibPath);
        runner = new RetroRunner(retro);
    }

    public void LoadGame(string gamePath)
    {
        if (runner == null) throw new Exception("Load core first");
        var gameInfo = new retro_game_info
        {
            path = Marshal.StringToHGlobalAnsi(gamePath),
            data = IntPtr.Zero,
            size = 0,
            meta = IntPtr.Zero
        };
        runner.LoadGame(gameInfo);
    }

    public void Run()
    {
        if (runner == null)
        {
            throw new Exception("Load core and game first");
        }
        runner.Start();
    }

    public async Task Stop()
    {
        if (runner != null)
        {
            await runner.Stop();
            runner = null;
        }
    }

    public IEnumerable<string> ListDownloadedCores()
    {
        Directory.CreateDirectory(coresDir);
        var files = Directory.GetFiles(coresDir, "*_libretro.dylib");
        return files.Select(f => Path.GetFileNameWithoutExtension(f).Replace("_libretro", ""));
    }

    public void SendKeyDown(string key)
    {
        Console.WriteLine($"Key Down: {key}");
        try
        {
            var id = (uint)(retro_device_id_joypad)Enum.Parse(typeof(retro_device_id_joypad), key);
            buttonStates[id] = true;
        }
        catch
        {
            // Invalid key, ignore
        }
    }

    public void SendKeyUp(string key)
    {
        Console.WriteLine($"Key Up: {key}");
        try
        {
            var id = (uint)(retro_device_id_joypad)Enum.Parse(typeof(retro_device_id_joypad), key);
            buttonStates[id] = false;
        }
        catch
        {
            // Invalid key, ignore
        }
    }
}
