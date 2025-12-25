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
    public required Action<FrameData> OnFrame;
    public required Action<AudioData> OnAudio;

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
                Console.Error.WriteLine($"Error in OnFrame: {ex}");
            }
        };

        retro.OnSample = (left, right) =>
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
                    Samples = MemoryMarshal.AsBytes<short>(samples).ToArray(),
                    SampleRate = (int)sampleRate
                };
                OnAudio(audioData);
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
        logger.LogInformation("Core loaded");
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
            logger.LogInformation($"Sample rate: {sampleRate}");
            logger.LogInformation("Game loaded");
        }
        return Task.CompletedTask;
    }

    public Task Run()
    {
        if (runner != null)
        {
            runner.Start();
            logger.LogInformation("Runner started");
        }
        return Task.CompletedTask;
    }

    public async Task Stop()
    {
        if (runner != null)
        {
            await runner.Stop();
            logger.LogInformation("Runner stopped");
        }
    }

    public Task SetInput(string key, bool down)
    {
        try
        {
            var id = (uint)(retro_device_id_joypad)Enum.Parse(typeof(retro_device_id_joypad), key);
            buttonStates[id] = down;
            logger.LogInformation($"Input set: {key} = {down}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Invalid key: {key}", ex);
        }
        return Task.CompletedTask;
    }

    public double GetSampleRate() => sampleRate;

}