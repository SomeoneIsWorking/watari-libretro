using Libretro.NET;
using SkiaSharp;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Libretro.NET.Bindings;
using watari_libretro.Types;
using Watari;

namespace watari_libretro;

public class LibretroApplication(WatariContext context)
{
    private RetroWrapper? retro;
    private readonly string coresDir = context.PathCombine("cores");
    private readonly Dictionary<retro_key, bool> keyStates = [];
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


    public void LoadCore(string name)
    {
        var dylibPath = Path.Combine(coresDir, $"{name}_libretro.dylib");
        if (!File.Exists(dylibPath))
            throw new Exception("Download the core first");
        retro = new RetroWrapper();
        retro.LoadCore(dylibPath);
        retro.OnFrame = (frame, w, h) =>
        {
            // Handle different pixel formats
            SKColorType colorType;
            int bytesPerPixel;
            switch (retro.PixelFormat)
            {
                case retro_pixel_format.RETRO_PIXEL_FORMAT_0RGB1555: // RETRO_PIXEL_FORMAT_0RGB1555
                    colorType = SKColorType.Rgba8888; // need conversion?
                    bytesPerPixel = 2;
                    break;
                case retro_pixel_format.RETRO_PIXEL_FORMAT_XRGB8888: // RETRO_PIXEL_FORMAT_XRGB8888
                    colorType = SKColorType.Rgba8888;
                    bytesPerPixel = 4;
                    break;
                case retro_pixel_format.RETRO_PIXEL_FORMAT_RGB565: // RETRO_PIXEL_FORMAT_RGB565
                    colorType = SKColorType.Rgb565;
                    bytesPerPixel = 2;
                    break;
                default:
                    colorType = SKColorType.Rgba8888;
                    bytesPerPixel = 4;
                    break;
            }
            using var bitmap = new SKBitmap((int)w, (int)h, colorType, SKAlphaType.Opaque);
            unsafe
            {
                var pixels = bitmap.GetPixels();
                Marshal.Copy(frame, 0, pixels, (int)(w * h * bytesPerPixel));
            }
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 80);
            var pngBytes = data.ToArray();
            var base64 = Convert.ToBase64String(pngBytes);
            OnFrameReceived(new FrameData
            {
                Image = $"data:image/png;base64,{base64}",
                Width = (int)w,
                Height = (int)h
            });
        };
        retro.OnSample = (sample) => { /* TODO: audio */ };
        retro.OnCheckInput = (port, device, index, id) =>
        {
            if (keyStates.TryGetValue((retro_key)id, out var pressed))
                return pressed;
            return false;
        };
    }

    public void LoadGame(string gamePath)
    {
        if (retro == null) throw new Exception("Load core first");
        retro.LoadGame(gamePath);
    }

    public void Run()
    {
        if (retro == null) throw new Exception("Load core and game first");
        Task.Run(() =>
        {
            while (true)
            {
                retro.Run();
                Thread.Sleep(16);
            }
        });
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
        if (Enum.TryParse<retro_key>(key, true, out var k))
            keyStates[k] = true;
    }

    public void SendKeyUp(string key)
    {
        Console.WriteLine($"Key Up: {key}");
        if (Enum.TryParse<retro_key>(key, true, out var k))
            keyStates[k] = false;
    }
}
