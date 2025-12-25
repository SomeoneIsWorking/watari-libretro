namespace watari_libretro.Types;

public class FrameData
{
    public required string Pixels { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string PixelFormat { get; set; } = "RGBA8888";
}