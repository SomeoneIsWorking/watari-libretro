using MessagePack;

namespace watari_libretro.Types;

[MessagePackObject]
public class AudioData
{
    [Key(0)]
    public required short[] Samples { get; set; } // 16-bit signed PCM, interleaved LR LR...
}