namespace watari_libretro.Types;

public class AudioData
{
    public required string Samples { get; set; } // Base64 encoded 16-bit signed PCM, interleaved LR LR...
    public int SampleRate { get; set; } = 44100; // Default, but should be set from AV info
}