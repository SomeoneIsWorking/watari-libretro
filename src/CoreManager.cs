using System.IO.Compression;
using Watari;
using watari_libretro.Types;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace watari_libretro;

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
    private readonly string coresDir;
    private readonly string manifestsDir;
    private readonly string coversDir;
    private readonly string infoZipCachePath;
    private readonly string coreOptionsDir;


    public CoreManager(WatariContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
        coresDir = _context.PathCombine("config", "cores");
        manifestsDir = _context.PathCombine("config", "manifests");
        coversDir = _context.PathCombine("config", "covers");
        infoZipCachePath = _context.PathCombine("config", "info.zip");
        coreOptionsDir = _context.PathCombine("config", "core-options");
    }

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

    public void RemoveCore(string name)
    {
        var dylibPath = Path.Combine(coresDir, $"{name}_libretro.dylib");
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

    public Dictionary<string, string> GetCoreOptions(string coreId)
    {
        var dylibPath = Path.Combine(coresDir, $"{coreId}_libretro.dylib");
        if (!File.Exists(dylibPath))
            throw new Exception("Core not downloaded");

        using var retro = new LibretroCore(dylibPath);
        retro.Init();
        var options = retro.VariableDefinitions;
        return options;
    }

    public Dictionary<string, string> LoadCoreOptionValues(string coreId)
    {
        var optionsPath = Path.Combine(coreOptionsDir, $"{coreId}.json");
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
        Directory.CreateDirectory(coreOptionsDir);
        var optionsPath = Path.Combine(coreOptionsDir, $"{coreId}.json");
        var json = JsonSerializer.Serialize(nonDefaults);
        File.WriteAllText(optionsPath, json);
    }

    public string GetCoversDir() => coversDir;
}