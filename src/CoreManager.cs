using System.IO.Compression;
using Watari;
using watari_libretro.Types;

namespace watari_libretro;

public record CoreInfo(
    string Id,
    string Name,
    List<string> SupportedExtensions,
    List<string> Database,
    bool IsDownloaded
);

public class CoreManager(WatariContext context)
{
    private readonly string coresDir = context.PathCombine("config", "cores");
    private readonly string manifestsDir = context.PathCombine("config", "manifests");
    private readonly string coversDir = context.PathCombine("config", "covers");
    private readonly string infoZipCachePath = context.PathCombine("config", "info.zip");

    private async Task EnsureManifestsExtracted()
    {
        if (Directory.Exists(manifestsDir) && Directory.GetFiles(manifestsDir, "*.info").Length > 0)
        {
            // Already extracted
            return;
        }

        // Download the ZIP
        Directory.CreateDirectory(Path.GetDirectoryName(infoZipCachePath)!);
        using var client = new HttpClient();
        var url = "https://buildbot.libretro.com/assets/frontend/info.zip";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        using (var stream = await response.Content.ReadAsStreamAsync())
        using (var fileStream = File.Create(infoZipCachePath))
            await stream.CopyToAsync(fileStream);

        // Extract
        Directory.CreateDirectory(manifestsDir);
        ZipFile.ExtractToDirectory(infoZipCachePath, manifestsDir, true);
    }

    public event Action<DownloadProgress>? OnDownloadProgress;
    public event Action<string>? OnDownloadComplete;

    public async Task DownloadCore(string name)
    {
        var zipPath = Path.Combine(coresDir, $"{name}.zip");
        Directory.CreateDirectory(coresDir);
        await DoDownloadCore(name, zipPath);

        ZipFile.ExtractToDirectory(zipPath, coresDir, true);
        var dylibPath = Path.Combine(coresDir, $"{name}_libretro.dylib");
        File.Delete(zipPath);
        OnDownloadComplete?.Invoke(name);
    }

    public async Task DownloadCover(string systemName)
    {
        var coverPath = Path.Combine(coversDir, $"{systemName}.png");
        Directory.CreateDirectory(coversDir);
        using var client = new HttpClient();
        var url = $"https://thumbnails.libretro.com/Named_Boxarts/{Uri.EscapeDataString(systemName)}.png";
        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var fileStream = File.Create(coverPath);
            await stream.CopyToAsync(fileStream);
        }
        // Optionally handle failure
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
                OnDownloadProgress?.Invoke(new DownloadProgress { Name = name, Progress = progress });
            }
        }
    }

    public Dictionary<string, string> GetManifest(string core)
    {
        var path = Path.Combine(manifestsDir, $"{core}_libretro.info");
        if (!File.Exists(path))
        {
            throw new Exception($"Manifest not found for core: {core}");
        }
        var lines = File.ReadAllLines(path);
        var dict = new Dictionary<string, string>();
        foreach (var line in lines)
        {
            if (line.Contains('='))
            {
                var parts = line.Split('=', 2);
                var key = parts[0].Trim();
                var value = parts[1].Trim();
                if (value.StartsWith('"') && value.EndsWith('"'))
                {
                    value = value[1..^1];
                }
                dict[key] = value;
            }
        }
        return dict;
    }

    public string GetCoresDir() => coresDir;

    public string GetManifestsDir() => manifestsDir;

    public string GetCoversDir() => coversDir;

    public async Task<List<CoreInfo>> GetCores()
    {
        await EnsureManifestsExtracted();

        var infoFiles = Directory.GetFiles(manifestsDir, "*_libretro.info");
        var cores = new List<CoreInfo>();
        foreach (var file in infoFiles)
        {
            var id = Path.GetFileNameWithoutExtension(file).Replace("_libretro", "");
            var manifest = GetManifest(id);
            if (
                !manifest.TryGetValue("corename", out var corename) ||
                !manifest.TryGetValue("supported_extensions", out var extStr) ||
                !manifest.TryGetValue("database", out var dbStr)
            )
            {
                continue;
            }
            var database = dbStr.Split('|').ToList();
            var supportedExtensions = extStr.Split('|').ToList();
            var isDownloaded = File.Exists(Path.Combine(coresDir, $"{id}_libretro.dylib"));
            cores.Add(new CoreInfo(id, corename, supportedExtensions, database, isDownloaded));
        }
        return cores;
    }
}