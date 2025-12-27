using Watari;
using System.IO;

namespace watari_libretro;

public class SystemManager(CoreManager coreManager)
{
    private readonly CoreManager coreManager = coreManager;

    public async Task<List<SystemInfo>> GetSystems()
    {
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
        return systems.Values.ToList();
    }
}