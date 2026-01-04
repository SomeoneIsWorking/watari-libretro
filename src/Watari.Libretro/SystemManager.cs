using System.Linq;

namespace Watari.Libretro;

public class SystemManager(CoreManager coreManager)
{
    private readonly CoreManager coreManager = coreManager;
    private List<SystemInfo>? _cachedSystems;

    public async Task<List<SystemInfo>> GetSystems()
    {
        if (_cachedSystems != null) return _cachedSystems;

        var cores = await coreManager.GetCores();
        var systems = new Dictionary<string, SystemInfo>();
        foreach (var core in cores)
        {
            foreach (var db in core.Database)
            {
                if (!systems.TryGetValue(db, out var existing))
                {
                    systems[db] = new SystemInfo(db, core.SupportedExtensions);
                }
                else
                {
                    var newExt = existing.Extensions.Concat(core.SupportedExtensions).Distinct().ToList();
                    systems[db] = existing with { Extensions = newExt };
                }
            }
        }
        _cachedSystems = systems.Values.ToList();
        return _cachedSystems;
    }

    public void InvalidateCache() => _cachedSystems = null;
}