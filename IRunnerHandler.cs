using watari_libretro.Types;
using System.Text.Json;

namespace watari_libretro;

public interface IRunnerHandler
{
    Task LoadCore(string corePath);
    Task LoadGame(string gamePath);
    Task Run();
    Task Stop();
    Task SetInput(string key, bool down);
    event Action<FrameData> OnFrame;
    event Action<AudioData> OnAudio;
    Task<object?> HandleMessage(string type, JsonElement data);
}