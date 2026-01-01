using System.Xml.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace watari_libretro;

public record DatabaseEntry(string Name, string RomName, string Crc, string Md5, string Sha1);

public class DatabaseManager(ILogger logger)
{
    private readonly ILogger _logger = logger;
    private readonly string _databaseDir = Directories.DatabaseDir;

    public async Task<DatabaseEntry?> FindGameByHash(string systemName, string crc32)
    {
        var datPath = await EnsureDatabase(systemName);
        if (datPath == null) return null;

        _logger.LogInformation("Searching for CRC {Crc} in {SystemName} database", crc32, systemName);

        try
        {
            var content = await File.ReadAllTextAsync(datPath);
            if (content.TrimStart().StartsWith("<?xml") || content.TrimStart().StartsWith("<datafile"))
            {
                return FindInXml(content, crc32, null);
            }
            else
            {
                return FindInClrMamePro(content, crc32, null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing database for {SystemName}", systemName);
        }

        return null;
    }

    public async Task<DatabaseEntry?> FindGameByName(string systemName, string fileName)
    {
        var datPath = await EnsureDatabase(systemName);
        if (datPath == null) return null;

        try
        {
            var content = await File.ReadAllTextAsync(datPath);
            if (content.TrimStart().StartsWith("<?xml") || content.TrimStart().StartsWith("<datafile"))
            {
                return FindInXml(content, null, fileName);
            }
            else
            {
                return FindInClrMamePro(content, null, fileName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing database for {SystemName}", systemName);
        }

        return null;
    }

    private DatabaseEntry? FindInXml(string content, string? crc32, string? fileName)
    {
        var doc = XDocument.Parse(content);
        XElement? game = null;

        if (crc32 != null)
        {
            game = doc.Descendants("game")
                .FirstOrDefault(g => g.Element("rom")?.Attribute("crc")?.Value.Equals(crc32, StringComparison.OrdinalIgnoreCase) == true);
        }
        else if (fileName != null)
        {
            game = doc.Descendants("game")
                .FirstOrDefault(g => g.Element("rom")?.Attribute("name")?.Value.Equals(fileName, StringComparison.OrdinalIgnoreCase) == true);
            
            if (game == null)
            {
                game = doc.Descendants("game")
                    .FirstOrDefault(g => g.Attribute("name")?.Value.Equals(Path.GetFileNameWithoutExtension(fileName), StringComparison.OrdinalIgnoreCase) == true);
            }
        }

        if (game != null)
        {
            var rom = game.Element("rom");
            return new DatabaseEntry(
                game.Attribute("name")?.Value ?? "Unknown",
                rom?.Attribute("name")?.Value ?? "Unknown",
                rom?.Attribute("crc")?.Value ?? "",
                rom?.Attribute("md5")?.Value ?? "",
                rom?.Attribute("sha1")?.Value ?? ""
            );
        }
        return null;
    }

    private DatabaseEntry? FindInClrMamePro(string content, string? crc32, string? fileName)
    {
        // Very basic clrmamepro parser
        var games = content.Split("game (", StringSplitOptions.RemoveEmptyEntries);
        foreach (var gameBlock in games)
        {
            if (gameBlock.TrimStart().StartsWith("clrmamepro")) continue;

            var nameMatch = Regex.Match(gameBlock, "name \"([^\"]+)\"");
            if (!nameMatch.Success) continue;
            var gameName = nameMatch.Groups[1].Value;

            var romMatches = Regex.Matches(gameBlock, "rom \\( ([^)]+) \\)");
            foreach (Match romMatch in romMatches)
            {
                var romBlock = romMatch.Groups[1].Value;
                var romNameMatch = Regex.Match(romBlock, "name \"([^\"]+)\"");
                var romCrcMatch = Regex.Match(romBlock, "crc ([0-9A-Fa-f]+)");

                if (romNameMatch.Success)
                {
                    var romName = romNameMatch.Groups[1].Value;
                    var romCrc = romCrcMatch.Success ? romCrcMatch.Groups[1].Value : "";

                    bool match = false;
                    if (crc32 != null && romCrc.Equals(crc32, StringComparison.OrdinalIgnoreCase))
                    {
                        match = true;
                    }
                    else if (fileName != null && (romName.Equals(fileName, StringComparison.OrdinalIgnoreCase) || gameName.Equals(Path.GetFileNameWithoutExtension(fileName), StringComparison.OrdinalIgnoreCase)))
                    {
                        match = true;
                    }

                    if (match)
                    {
                        var md5Match = Regex.Match(romBlock, "md5 ([0-9A-Fa-f]+)");
                        var sha1Match = Regex.Match(romBlock, "sha1 ([0-9A-Fa-f]+)");

                        return new DatabaseEntry(
                            gameName,
                            romName,
                            romCrc,
                            md5Match.Success ? md5Match.Groups[1].Value : "",
                            sha1Match.Success ? sha1Match.Groups[1].Value : ""
                        );
                    }
                }
            }
        }
        return null;
    }

    private async Task<string?> EnsureDatabase(string systemName)
    {
        var fileName = $"{systemName}.dat";
        var localPath = Path.Combine(_databaseDir, fileName);

        if (File.Exists(localPath))
        {
            return localPath;
        }

        _logger.LogInformation("Downloading database for {SystemName}...", systemName);
        
        var escapedName = Uri.EscapeDataString(systemName);
        var baseUrls = new[]
        {
            "https://raw.githubusercontent.com/libretro/libretro-database/master/metadat/no-intro/",
            "https://raw.githubusercontent.com/libretro/libretro-database/master/metadat/redump/",
            "https://raw.githubusercontent.com/libretro/libretro-database/master/metadat/libretro-dats/",
            "https://raw.githubusercontent.com/libretro/libretro-database/master/dat/"
        };
        
        using var client = new HttpClient();
        foreach (var baseUrl in baseUrls)
        {
            var url = $"{baseUrl}{escapedName}.dat";
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(localPath, content);
                    _logger.LogInformation("Successfully downloaded database from {Url}", url);
                    return localPath;
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to download from {Url}", url);
            }
        }

        _logger.LogWarning("Could not find database for {SystemName} in any known location", systemName);
        return null;
    }
}
