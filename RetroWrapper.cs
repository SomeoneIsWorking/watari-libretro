using System.Runtime.InteropServices;
using watari_libretro.Types;

namespace watari_libretro;

public class RetroWrapper : IDisposable
{
    private readonly LibretroCore core;
    public retro_pixel_format PixelFormat => core.PixelFormat;

    public RetroWrapper()
    {
        core = new LibretroCore();
    }

    public void LoadCore(string path)
    {
        core.Load(path);
        core.SetEnvironment(OnEnvironment ?? Environment);
        core.SetVideoRefresh(OnFrame ?? VideoRefresh);
        core.SetAudioSample(OnSample ?? AudioSample);
        core.SetAudioSampleBatch(OnSampleBatch ?? AudioSampleBatch);
        core.SetInputPoll(OnInputPoll ?? InputPoll);
        core.SetInputState(OnCheckInput ?? InputState);
        core.Init();
    }

    public void Init()
    {
        core.Init();
    }

    public void Deinit()
    {
        core.Deinit();
    }

    public retro_system_info GetSystemInfo()
    {
        return core.GetSystemInfo();
    }

    public retro_system_av_info GetSystemAvInfo()
    {
        return core.GetSystemAvInfo();
    }

    public void SetEnvironment(retro_environment_callback callback)
    {
        core.SetEnvironment(callback);
    }

    public void SetVideoRefresh(retro_video_refresh_callback callback)
    {
        core.SetVideoRefresh(callback);
    }

    public void SetAudioSample(retro_audio_sample_callback callback)
    {
        core.SetAudioSample(callback);
    }

    public void SetAudioSampleBatch(retro_audio_sample_batch_callback callback)
    {
        core.SetAudioSampleBatch(callback);
    }

    public void SetInputPoll(retro_input_poll_callback callback)
    {
        core.SetInputPoll(callback);
    }

    public void SetInputState(retro_input_state_callback callback)
    {
        core.SetInputState(callback);
    }

    public bool LoadGame(retro_game_info gameInfo)
    {
        return core.LoadGame(gameInfo);
    }

    public void UnloadGame()
    {
        core.UnloadGame();
    }

    public void Run()
    {
        core.Run();
    }

    public void Reset()
    {
        core.Reset();
    }

    public void Dispose()
    {
        core.Dispose();
    }

    // Events
    public retro_video_refresh_callback? OnFrame;
    public retro_audio_sample_callback? OnSample;
    public retro_audio_sample_batch_callback? OnSampleBatch;
    public retro_input_poll_callback? OnInputPoll;
    public retro_input_state_callback? OnCheckInput;
    public retro_environment_callback? OnEnvironment;

    private void VideoRefresh(IntPtr data, uint width, uint height, nuint pitch)
    {
        OnFrame?.Invoke(data, width, height, pitch);
    }

    private void AudioSample(short left, short right)
    {
        OnSample?.Invoke(left, right);
    }

    private nuint AudioSampleBatch(IntPtr data, nuint frames)
    {
        OnSampleBatch?.Invoke(data, frames);
        return frames;
    }

    private void InputPoll()
    {
        // Poll input
    }

    private short InputState(uint port, uint device, uint index, uint id)
    {
        return OnCheckInput?.Invoke(port, device, index, id) ?? 0;
    }

    private bool Environment(uint cmd, IntPtr data)
    {
        // Handle environment
        return false;
    }
}