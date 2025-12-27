using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace watari_libretro;

public record GameInfo(string Name, string Path, string SystemName)
{
    private static string ComputeMd5(string input)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = md5.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    public string CoverName => ComputeMd5(Path);
}
