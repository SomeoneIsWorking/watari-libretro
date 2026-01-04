using MessagePack;

namespace Watari.Libretro.Types;

[MessagePackObject]
public class FrameData
{
    [Key(0)]
    public required byte[] Pixels { get; set; }
    [Key(1)]
    public int Width { get; set; }
    [Key(2)]
    public int Height { get; set; }
    [Key(3)]
    public string PixelFormat { get; set; } = "RGBA8888";
}