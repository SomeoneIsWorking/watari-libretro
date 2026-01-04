using MessagePack;

namespace Watari.Libretro.Types;

[MessagePackObject]
public class AudioData
{
    [Key(0)]
    public required short[] Samples { get; set; } // 16-bit signed PCM, interleaved LR LR...
}