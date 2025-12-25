using System.Runtime.InteropServices;
using watari_libretro.Types;
using Watari;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace watari_libretro;

public class LibretroHandler(ILogger logger) : IRunnerHandler
{
    private readonly ILogger logger = logger;
    private RetroWrapper? retro;
    private RetroRunner? runner;
    private readonly Dictionary<uint, bool> buttonStates = [];
    private double sampleRate = 44100;
    public event Action<FrameData>? OnFrame;
    public event Action<AudioData>? OnAudio;

    public Task LoadCore(string corePath)
    {
        retro = new RetroWrapper();
        retro.OnFrame = (data, w, h, pitch) =>
        {
            try
            {
                if (data != IntPtr.Zero)
                {
                    var pixelBytes = PixelConverter.ConvertFrame(data, w, h, pitch, retro.PixelFormat);
                    var base64 = Convert.ToBase64String(pixelBytes);
                    var frameData = new FrameData
                    {
                        Pixels = base64,
                        Width = (int)w,
                        Height = (int)h,
                        PixelFormat = "RGBA8888"
                    };
                    OnFrame?.Invoke(frameData);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in OnFrame: {ex}");
            }
        };

        retro.OnSample = (left, right) =>
        {
            try
            {
                var audioData = new AudioData
                {
                    Samples = Convert.ToBase64String(BitConverter.GetBytes(left).Concat(BitConverter.GetBytes(right)).ToArray()),
                    SampleRate = 44100
                };
                OnAudio?.Invoke(audioData);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in OnSample: {ex}");
            }
        };

        retro.OnSampleBatch = (data, frames) =>
        {
            try
            {
                var sampleCount = (int)frames * 2; // stereo
                var samples = new short[sampleCount];
                Marshal.Copy(data, samples, 0, sampleCount);
                var audioData = new AudioData
                {
                    Samples = Convert.ToBase64String(MemoryMarshal.AsBytes<short>(samples)),
                    SampleRate = (int)sampleRate
                };
                OnAudio?.Invoke(audioData);
                return frames;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in OnSampleBatch: {ex}");
                return 0;
            }
        };

        retro.OnCheckInput = (port, device, index, inputId) =>
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
                Console.Error.WriteLine($"Error in OnCheckInput: {ex}");
                return 0;
            }
        };

        retro.LoadCore(corePath);
        LogMessage("Info", "Core loaded");
        return Task.CompletedTask;
    }

    public Task LoadGame(string gamePath)
    {
        if (retro != null && runner == null)
        {
            runner = new RetroRunner(retro);
        }
        if (runner != null)
        {
            var gameInfo = new retro_game_info
            {
                path = Marshal.StringToHGlobalAnsi(gamePath),
                data = IntPtr.Zero,
                size = 0,
                meta = IntPtr.Zero
            };
            runner.LoadGame(gameInfo);
            var avInfo = retro!.GetSystemAvInfo();
            sampleRate = avInfo.timing.sample_rate;
            LogMessage("Info", $"Sample rate: {sampleRate}");
            LogMessage("Info", "Game loaded");
        }
        return Task.CompletedTask;
    }

    public Task Run()
    {
        if (runner != null)
        {
            runner.Start();
            LogMessage("Info", "Runner started");
        }
        return Task.CompletedTask;
    }

    public async Task Stop()
    {
        if (runner != null)
        {
            await runner.Stop();
            LogMessage("Info", "Runner stopped");
        }
    }

    public Task SetInput(string key, bool down)
    {
        try
        {
            var id = (uint)(retro_device_id_joypad)Enum.Parse(typeof(retro_device_id_joypad), key);
            buttonStates[id] = down;
            LogMessage("Info", $"Input set: {key} = {down}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Invalid key: {key}", ex);
        }
        return Task.CompletedTask;
    }

    private void LogMessage(string level, string message)
    {
        if (Enum.TryParse<LogLevel>(level, true, out var logLevel))
        {
            logger.Log(logLevel, message);
        }
        else
        {
            logger.Log(LogLevel.Information, $"{level}: {message}");
        }
    }

    public double GetSampleRate() => sampleRate;

    public async Task<object?> HandleMessage(string type, JsonElement data)
    {
        switch (type)
        {
            case "LoadCore":
                var corePath = data.GetProperty("corePath").GetString()!;
                await LoadCore(corePath);
                return null;
            case "LoadGame":
                var gamePath = data.GetProperty("gamePath").GetString()!;
                await LoadGame(gamePath);
                return null;
            case "Run":
                await Run();
                return null;
            case "Stop":
                await Stop();
                return null;
            case "SetInput":
                var key = data.GetProperty("key").GetString()!;
                var down = data.GetProperty("down").GetBoolean();
                await SetInput(key, down);
                return null;
            case "GetSampleRate":
                return sampleRate;
            default:
                throw new NotImplementedException($"Unknown message type: {type}");
        }
    }
}