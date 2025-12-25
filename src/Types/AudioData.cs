using MessagePack;

namespace watari_libretro.Types;

[MessagePackObject]
public class AudioData
{
    [Key(0)]
    public required byte[] Samples { get; set; } // 16-bit signed PCM, interleaved LR LR...
    [Key(1)]
    public int SampleRate { get; set; } = 44100; // Default, but should be set from AV info
}