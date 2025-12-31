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
    private double sampleRate = 44100;
    public virtual event Action<FrameData> OnFrame = delegate { };
    public virtual event Action<AudioData> OnAudio = delegate { };

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
        retro.Log += LogMessage;
        retro.VideoRefresh += (data, w, h, pitch) =>
        {
            try
            {
                if (data != IntPtr.Zero)
                {
                    var pixelBytes = PixelConverter.ConvertFrame(data, w, h, pitch, retro.PixelFormat);
                    var frameData = new FrameData
                    {
                        Pixels = pixelBytes,
                        Width = (int)w,
                        Height = (int)h,
                        PixelFormat = "RGBA8888"
                    };
                    OnFrame(frameData);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in OnFrame");
            }
        };

        retro.AudioSample += (left, right) =>
        {
            try
            {
                var audioData = new AudioData
                {
                    Samples = [.. BitConverter.GetBytes(left), .. BitConverter.GetBytes(right)],
                    SampleRate = 44100
                };
                OnAudio(audioData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in OnSample");
            }
        };

        retro.AudioSampleBatch += (data, frames) =>
        {
            try
            {
                var sampleCount = (int)frames * 2; // stereo
                var samples = new short[sampleCount];
                Marshal.Copy(data, samples, 0, sampleCount);
                var audioData = new AudioData
                {
                    Samples = MemoryMarshal.AsBytes<short>(samples).ToArray(),
                    SampleRate = (int)sampleRate
                };
                OnAudio(audioData);
                return frames;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in OnSampleBatch");
                return 0;
            }
        };

        retro.InputState += (port, device, index, inputId) =>
        {
            try
            {
                if (device == (uint)retro_device_type.RETRO_DEVICE_JOYPAD && port == 0 && index == 0)
                {
                    return buttonStates.TryGetValue(inputId, out var pressed) && pressed ? (short)1 : (short)0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in OnCheckInput");
                return 0;
            }
        };

        retro.Init();
        logger.LogInformation("Core loaded");
        return Task.CompletedTask;
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
        var avInfo = retro.GetSystemAvInfo();
        sampleRate = avInfo.timing.sample_rate;

        if (!success)
        {
            logger.LogError("Failed to load game");
            throw new Exception("Failed to load game");
        }
        logger.LogInformation("Game loaded");
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

    public virtual double GetSampleRate() => sampleRate;

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
        logger.Log(logLevel, msg);
    }
}