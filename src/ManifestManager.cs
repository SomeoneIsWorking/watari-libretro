using Watari;

namespace watari_libretro;

public class ManifestManager(WatariContext context)
{
    private readonly string manifestsDir = context.PathCombine("config", "manifests");

    public List<SystemInfo> GetSystems()
    {
        var systems = new Dictionary<string, SystemInfo>();
        if (!Directory.Exists(manifestsDir)) return [];
        foreach (var file in Directory.GetFiles(manifestsDir, "*.info"))
        {
            var lines = File.ReadAllLines(file);
            var dict = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (line.Contains('='))
                {
                    var parts = line.Split('=', 2);
                    dict[parts[0].Trim()] = parts[1].Trim();
                }
            }
            if (dict.TryGetValue("systemid", out var systemid) && dict.TryGetValue("supported_extensions", out var exts) && dict.TryGetValue("corename", out var core))
            {
                var extensions = exts.Split('|').ToList();
                if (!systems.ContainsKey(systemid))
                {
                    systems[systemid] = new SystemInfo(systemid, core, extensions);
                }
                else
                {
                    // merge extensions
                    var existing = systems[systemid];
                    var newExtensions = existing.Extensions.Concat(extensions).Distinct().ToList();
                    systems[systemid] = existing with { Extensions = newExtensions };
                }
            }
        }
        return systems.Values.ToList();
    }
}