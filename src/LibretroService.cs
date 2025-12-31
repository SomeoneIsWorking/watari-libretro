using System.Runtime.InteropServices;
using watari_libretro.Types;
using Microsoft.Extensions.Logging;
using SeparateProcess;
using watari_libretro.libretro;

namespace watari_libretro;

#pragma warning disable CA1070 // Do not declare event fields as virtual
public class LibretroService(ILogger<LibretroService> logger) : ISeparateProcess
{
    private LibretroCore? retro;
    private RetroRunner? runner;
    private readonly Dictionary<uint, bool> buttonStates = [];
    public virtual event Action<FrameData> FrameReceived = delegate { };
    public virtual event Action<AudioData> AudioReceived = delegate { };
    public virtual event Action<double> SampleRateChanged = delegate { };

    public virtual Task StartAsync()
    {
        if (runner == null)
        {
            throw new Exception("Load core and game first");
        }
        runner.Start();
        logger.LogInformation("LibretroService started");
        return Task.CompletedTask;
    }

    public virtual Task LoadCore(string corePath)
    {
        retro = new LibretroCore(corePath, logger);
        retro.SystemAvInfoReceived += OnSystemAvInfoReceived;
        retro.Log += LogMessage;
        retro.VideoRefresh += OnVideoRefresh;
        retro.AudioSample += OnAudioSample;
        retro.AudioSampleBatch += OnAudioSampleBatch;
        retro.InputState += OnInputState;
        retro.Init();
        logger.LogInformation("Core loaded");
        return Task.CompletedTask;
    }

    private void OnSystemAvInfoReceived(retro_system_av_info info)
    {
        if (info.timing.sample_rate > 0)
            SampleRateChanged(info.timing.sample_rate);
    }

    private void OnAudioSample(short left, short right)
    {
        try
        {
            var audioData = new AudioData
            {
                Samples = [left, right],
            };
            AudioReceived(audioData);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in OnSample");
        }
    }

    private void OnVideoRefresh(nint data, uint w, uint h, nuint pitch)
    {
        if (data == IntPtr.Zero)
        {
            return;
        }
        var pixelBytes = PixelConverter.ConvertFrame(data, w, h, pitch, retro!.PixelFormat);
        var frameData = new FrameData
        {
            Pixels = pixelBytes,
            Width = (int)w,
            Height = (int)h,
            PixelFormat = "RGBA8888"
        };
        FrameReceived(frameData);
    }

    private short OnInputState(uint port, uint device, uint index, uint inputId)
    {
        if (device == (uint)retro_device_type.RETRO_DEVICE_JOYPAD && port == 0 && index == 0)
        {
            return buttonStates.TryGetValue(inputId, out var pressed) && pressed ? (short)1 : (short)0;
        }
        return 0;
    }

    private nuint OnAudioSampleBatch(nint data, nuint frames)
    {
        var sampleCount = (int)frames * 2;
        var samples = new short[sampleCount];
        Marshal.Copy(data, samples, 0, sampleCount);
        var audioData = new AudioData
        {
            Samples = samples,
        };
        AudioReceived(audioData);
        return frames;
    }

    public virtual void LoadGame(string gamePath)
    {
        if (retro == null)
        {
            throw new Exception("Load core first");
        }

        logger.LogInformation($"Loading game: {gamePath}");
        runner ??= new RetroRunner(retro);

        // Load ROM data
        byte[] romData = File.ReadAllBytes(gamePath);
        GCHandle handle = GCHandle.Alloc(romData, GCHandleType.Pinned);

        var gameInfo = new retro_game_info
        {
            path = Marshal.StringToHGlobalAnsi(gamePath),
            data = handle.AddrOfPinnedObject(),
            size = (nuint)romData.Length,
            meta = IntPtr.Zero
        };
        bool success = runner.LoadGame(gameInfo);

        if (!success)
        {
            logger.LogError("Failed to load game");
            throw new Exception("Failed to load game");
        }

        logger.LogInformation("Game loaded");

        var avInfo = retro.GetSystemAvInfo();
        if (avInfo.timing.sample_rate > 0)
        {
            SampleRateChanged(avInfo.timing.sample_rate);
        }
    }

    public virtual async Task StopAsync()
    {
        if (runner != null)
        {
            await runner.Stop();
            logger.LogInformation("Runner stopped");
        }
    }

    public virtual Task SetInput(string key, bool down)
    {
        try
        {
            var id = (uint)(retro_device_id_joypad)Enum.Parse(typeof(retro_device_id_joypad), key);
            buttonStates[id] = down;
        }
        catch (Exception ex)
        {
            throw new Exception($"Invalid key: {key}", ex);
        }
        return Task.CompletedTask;
    }

    public virtual Task SetCoreOptions(Dictionary<string, string> options)
    {
        if (retro != null)
        {
            foreach (var kvp in options)
            {
                retro.Variables[kvp.Key] = kvp.Value;
            }
        }
        return Task.CompletedTask;
    }

    private void LogMessage(retro_log_level level, string msg)
    {
        var logLevel = level switch
        {
            retro_log_level.RETRO_LOG_DEBUG => LogLevel.Debug,
            retro_log_level.RETRO_LOG_INFO => LogLevel.Information,
            retro_log_level.RETRO_LOG_WARN => LogLevel.Warning,
            retro_log_level.RETRO_LOG_ERROR => LogLevel.Error,
            _ => LogLevel.Information
        };
        logger.Log(logLevel, $"[Libretro] {msg}");
    }
}