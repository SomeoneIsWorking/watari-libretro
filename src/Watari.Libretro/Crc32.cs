namespace Watari.Libretro;

public static class Crc32
{
    private static readonly uint[] Table;

    static Crc32()
    {
        const uint polynomial = 0xedb88320;
        Table = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            var entry = i;
            for (var j = 0; j < 8; j++)
            {
                if ((entry & 1) == 1)
                    entry = (entry >> 1) ^ polynomial;
                else
                    entry >>= 1;
            }
            Table[i] = entry;
        }
    }

    public static uint Calculate(byte[] bytes)
    {
        var crc = 0xffffffff;
        foreach (var b in bytes)
        {
            var index = (byte)((crc & 0xff) ^ b);
            crc = (crc >> 8) ^ Table[index];
        }
        return ~crc;
    }

    public static string CalculateString(byte[] bytes)
    {
        return Calculate(bytes).ToString("X8");
    }

    public static async Task<string> CalculateFile(string path)
    {
        using var stream = File.OpenRead(path);
        var crc = 0xffffffff;
        var buffer = new byte[8192];
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
        {
            for (var i = 0; i < bytesRead; i++)
            {
                var index = (byte)((crc & 0xff) ^ buffer[i]);
                crc = (crc >> 8) ^ Table[index];
            }
        }
        return (~crc).ToString("X8");
    }
}
