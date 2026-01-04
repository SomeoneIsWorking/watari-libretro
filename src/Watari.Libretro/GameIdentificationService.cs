using Microsoft.Extensions.Logging;

namespace Watari.Libretro;

public class GameIdentificationService(SystemManager systemManager, DatabaseManager databaseManager, ILogger logger)
{
    private readonly SystemManager _systemManager = systemManager;
    private readonly DatabaseManager _databaseManager = databaseManager;
    private readonly ILogger _logger = logger;

    public async Task<GameInfo> IdentifyGame(string path)
    {
        var fileName = Path.GetFileName(path);
        var fileNameNoExt = Path.GetFileNameWithoutExtension(path);
        var extension = Path.GetExtension(path).TrimStart('.').ToLower();

        _logger.LogInformation("Identifying game: {Path} (ext: {Extension})", path, extension);

        // 1. Identify System
        var systems = await _systemManager.GetSystems();
        var applicableSystems = systems.Where(s => s.Extensions.Any(e => e.Equals(extension, StringComparison.OrdinalIgnoreCase))).ToList();

        _logger.LogInformation("Found {Count} applicable systems", applicableSystems.Count);

        // 2. Try to identify using libretro-database across all applicable systems
        if (applicableSystems.Count > 0)
        {
            string? crc32 = null;
            try
            {
                crc32 = await Crc32.CalculateFile(path);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to calculate CRC32 for {Path}", path);
            }

            foreach (var system in applicableSystems)
            {
                // Try by CRC32 first
                if (crc32 != null)
                {
                    var dbEntry = await _databaseManager.FindGameByHash(system.Name, crc32);
                    if (dbEntry != null)
                    {
                        _logger.LogInformation("Found game in libretro-database by CRC: {Name} (System: {System})", dbEntry.Name, system.Name);
                        return new GameInfo(dbEntry.Name, path, system.Name);
                    }
                }

                // Try by filename in database
                var dbEntryByName = await _databaseManager.FindGameByName(system.Name, fileName);
                if (dbEntryByName != null)
                {
                    _logger.LogInformation("Found game in libretro-database by filename: {Name} (System: {System})", dbEntryByName.Name, system.Name);
                    return new GameInfo(dbEntryByName.Name, path, system.Name);
                }
            }
        }

        // 3. Fallback to heuristics
        string systemName = "Unknown";
        if (applicableSystems.Count == 1)
        {
            systemName = applicableSystems[0].Name;
        }
        else if (applicableSystems.Count > 1)
        {
            systemName = GuessSystem(applicableSystems, path) ?? applicableSystems[0].Name;
        }

        return new GameInfo(fileNameNoExt, path, systemName);
    }

    private string? GuessSystem(List<SystemInfo> systems, string path)
    {
        var fileName = Path.GetFileName(path).ToLower();
        
        // Some common heuristics
        if (fileName.Contains("megadrive") || fileName.Contains("genesis"))
        {
            return systems.FirstOrDefault(s => s.Name.Contains("Mega Drive") || s.Name.Contains("Genesis"))?.Name;
        }
        
        if (fileName.Contains("super nintendo") || fileName.Contains("snes"))
        {
            return systems.FirstOrDefault(s => s.Name.Contains("Super Nintendo") || s.Name.Contains("SNES"))?.Name;
        }

        return null;
    }
}
