using System.IO.Compression;
using System.Text.Json;
using Watari;
using watari_libretro.Types;

namespace watari_libretro;

public record CoreInfo(string Name, string Status, Dictionary<string, string> Manifest);

public class CoreManager(WatariContext context)
{
    private readonly string coresDir = context.PathCombine("config", "cores");
    private readonly string manifestsDir = context.PathCombine("config", "manifests");
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

    public async Task<CoreInfo[]> ListCoreInfos()
    {
        var cores = await GetAvailableCores();
        var downloaded = ListDownloadedCores().ToHashSet();
        var result = new List<CoreInfo>();
        foreach (var core in cores)
        {
            var status = downloaded.Contains(core) ? "downloaded" : "available";
            var manifest = GetManifest(core);
            result.Add(new CoreInfo(core, status, manifest));
        }
        
        return result.ToArray();
    }

    private async Task<string[]> GetAvailableCores()
    {
        await EnsureManifestsExtracted();

        var infoFiles = Directory.GetFiles(manifestsDir, "*.info");
        var cores = infoFiles.Select(f => Path.GetFileNameWithoutExtension(f).Replace("_libretro", "")).ToArray();
        return cores;
    }

    private Dictionary<string, string> GetManifest(string core)
    {
        var path = Path.Combine(manifestsDir, $"{core}_libretro.info");
        if (File.Exists(path))
        {
            var lines = File.ReadAllLines(path);
            var dict = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (line.Contains('='))
                {
                    var parts = line.Split('=', 2);
                    dict[parts[0]] = parts[1];
                }
            }
            return dict;
        }
        return new Dictionary<string, string>();
    }

    public string GetCoresDir() => coresDir;

    public IEnumerable<string> ListDownloadedCores()
    {
        Directory.CreateDirectory(coresDir);
        var files = Directory.GetFiles(coresDir, "*_libretro.dylib");
        return files.Select(f => Path.GetFileNameWithoutExtension(f).Replace("_libretro", ""));
    }
}