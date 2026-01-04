using System.IO.Compression;
using Watari.Libretro.Types;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Watari.Libretro;

public record CoreInfo(
    string Id,
    string Name,
    List<string> SupportedExtensions,
    List<string> Database,
    bool IsDownloaded
);

public class CoreManager
{
    private readonly WatariContext _context;
    private readonly ILogger _logger;


    public CoreManager(WatariContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    private async Task EnsureManifestsExtracted()
    {
        if (Directory.Exists(Directories.ManifestsDir) && Directory.GetFiles(Directories.ManifestsDir, "*.info").Length > 0)
        {
            // Already extracted
            return;
        }

        // Download the ZIP
        using var client = new HttpClient();
        var url = "https://buildbot.libretro.com/assets/frontend/info.zip";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var tempZipPath = Path.GetTempFileName();
        using (var stream = await response.Content.ReadAsStreamAsync())
        using (var fileStream = File.Create(tempZipPath))
            await stream.CopyToAsync(fileStream);

        // Extract
        ZipFile.ExtractToDirectory(tempZipPath, Directories.ManifestsDir, true);
        File.Delete(tempZipPath);
    }

    public event Action<DownloadProgress>? OnDownloadProgress;
    public event Action<string>? OnDownloadComplete;

    public async Task DownloadCore(string name)
    {
        var zipPath = Path.Combine(Directories.CoresDir, $"{name}.zip");
        await DoDownloadCore(name, zipPath);

        ZipFile.ExtractToDirectory(zipPath, Directories.CoresDir, true);
        var dylibPath = Path.Combine(Directories.CoresDir, $"{name}_libretro.dylib");
        File.Delete(zipPath);
        OnDownloadComplete?.Invoke(name);
    }

    public void RemoveCore(string name)
    {
        var dylibPath = Path.Combine(Directories.CoresDir, $"{name}_libretro.dylib");
        if (File.Exists(dylibPath))
        {
            File.Delete(dylibPath);
        }
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
        var path = Path.Combine(Directories.ManifestsDir, $"{core}_libretro.info");
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

    public string GetCoresDir() => Directories.CoresDir;

    public string GetManifestsDir() => Directories.ManifestsDir;

    public async Task<List<CoreInfo>> GetCores()
    {
        await EnsureManifestsExtracted();

        var infoFiles = Directory.GetFiles(Directories.ManifestsDir, "*_libretro.info");
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
            var isDownloaded = File.Exists(Path.Combine(Directories.CoresDir, $"{id}_libretro.dylib"));
            cores.Add(new CoreInfo(id, corename, supportedExtensions, database, isDownloaded));
        }
        return cores;
    }

    public Dictionary<string, string> GetCoreOptions(string coreId)
    {
        var dylibPath = Path.Combine(Directories.CoresDir, $"{coreId}_libretro.dylib");
        if (!File.Exists(dylibPath))
            throw new Exception("Core not downloaded");

        using var retro = new LibretroCore(dylibPath, _logger);
        retro.Init();
        var options = retro.VariableDefinitions;
        return options;
    }

    public Dictionary<string, string> LoadCoreOptionValues(string coreId)
    {
        var optionsPath = Path.Combine(Directories.CoreOptionsDir, $"{coreId}.json");
        if (File.Exists(optionsPath))
        {
            var json = File.ReadAllText(optionsPath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }
        return [];
    }

    public void SaveCoreOptionValues(string coreId, Dictionary<string, string> values)
    {
        var defaults = GetCoreOptions(coreId);
        var nonDefaults = values
            .Where(kv => !defaults.TryGetValue(kv.Key, out var def) || kv.Value != def)
            .ToDictionary(kv => kv.Key, kv => kv.Value);
        var optionsPath = Path.Combine(Directories.CoreOptionsDir, $"{coreId}.json");
        var json = JsonSerializer.Serialize(nonDefaults);
        File.WriteAllText(optionsPath, json);
    }

    public string GetCoversDir() => Directories.CoversDir;
}