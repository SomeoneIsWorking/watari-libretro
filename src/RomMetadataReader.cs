using System.Text;

namespace watari_libretro;

public record GameMetadata(string Name, string? Id);

public class RomMetadataReader
{
    public GameMetadata? ReadMetadata(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        using var stream = File.OpenRead(filePath);
        using var reader = new BinaryReader(stream);

        return extension switch
        {
            ".gba" => ReadGbaMetadata(reader),
            ".nes" => ReadNesMetadata(reader, filePath),
            ".snes" or ".smc" => ReadSnesMetadata(reader, filePath),
            _ => null
        };
    }

    private GameMetadata? ReadGbaMetadata(BinaryReader reader)
    {
        if (reader.BaseStream.Length < 0xC0) return null;

        reader.BaseStream.Seek(0xA0, SeekOrigin.Begin);
        var titleBytes = reader.ReadBytes(12);
        var title = Encoding.ASCII.GetString(titleBytes).TrimEnd('\0');

        reader.BaseStream.Seek(0xAC, SeekOrigin.Begin);
        var codeBytes = reader.ReadBytes(4);
        var code = Encoding.ASCII.GetString(codeBytes).TrimEnd('\0');

        return new GameMetadata(title, code);
    }

    private GameMetadata? ReadNesMetadata(BinaryReader reader, string filePath)
    {
        // NES doesn't have internal name, use filename
        var filename = Path.GetFileNameWithoutExtension(filePath);
        return new GameMetadata(filename, null);
    }

    private GameMetadata? ReadSnesMetadata(BinaryReader reader, string filePath)
    {
        // SNES header at 0x7FC0 or 0xFFC0 for LoROM/HiROM
        // Try LoROM first
        reader.BaseStream.Seek(0x7FC0, SeekOrigin.Begin);
        if (reader.BaseStream.Position + 21 <= reader.BaseStream.Length)
        {
            var titleBytes = reader.ReadBytes(21);
            var title = Encoding.ASCII.GetString(titleBytes).TrimEnd('\0');
            if (!string.IsNullOrWhiteSpace(title) && title.Length > 3)
            {
                return new GameMetadata(title, null);
            }
        }

        // Try HiROM
        reader.BaseStream.Seek(0xFFC0, SeekOrigin.Begin);
        if (reader.BaseStream.Position + 21 <= reader.BaseStream.Length)
        {
            var titleBytes = reader.ReadBytes(21);
            var title = Encoding.ASCII.GetString(titleBytes).TrimEnd('\0');
            if (!string.IsNullOrWhiteSpace(title))
            {
                return new GameMetadata(title, null);
            }
        }

        var filename = Path.GetFileNameWithoutExtension(filePath);
        return new GameMetadata(filename, null);
    }
}