using System.Runtime.InteropServices;
using Xunit;
using Watari.Controls.Platform;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace watari_libretro.Tests;

[Collection("Libretro")]
public class LibretroMacOSTests : IDisposable
{
    private readonly ILogger logger = NullLogger<LibretroMacOSTests>.Instance;
    private LibretroCore? _core;
    private bool _audioReceived = false;
    private Application? _app;

    public void Dispose()
    {
        _core?.Dispose();
    }

    [Fact]
    public void CreateCore_LoadCore_LoadGame_Run_ShouldPlayAudio()
    {
        // Skip if not on macOS
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return;
        }

        // Arrange
        _core = new LibretroCore();
        _app = new Application();
        string corePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "src", "cores", "mgba_libretro.dylib");

        // Assert file exists
        Assert.True(File.Exists(corePath), $"Core file not found at {corePath}");

        // Act: Load core
        _core.Load(corePath);
        _core.SetEnvironment((cmd, data) => true);
        _core.Init();
        logger.LogDebug("SampleRate after Init: {SampleRate}", _core.SampleRate);

        // Set callbacks
        _core.SetVideoRefresh((data, width, height, pitch) => { });
        _core.SetAudioSampleBatch(AudioCallback);
        _core.SetInputPoll(() => { });
        _core.SetInputState((port, device, index, id) => 0);

        // Load game
        string romPath = "/Users/barishamil/Pets/kirby.gba";
        Assert.True(File.Exists(romPath), $"ROM file not found at {romPath}");
        byte[] romData = File.ReadAllBytes(romPath);
        IntPtr romPtr = Marshal.AllocHGlobal(romData.Length);
        Marshal.Copy(romData, 0, romPtr, romData.Length);

        retro_game_info gameInfo = new()
        {
            path = Marshal.StringToHGlobalAnsi(romPath),
            data = romPtr,
            size = (nuint)romData.Length,
            meta = IntPtr.Zero
        };
        _core.LoadGame(gameInfo);
        var avInfo = _core.GetSystemAvInfo();
        logger.LogDebug("AV Info sample rate: {SampleRate}, fps: {Fps}", avInfo.timing.sample_rate, avInfo.timing.fps);

        // Set sample rate
        double sampleRate = avInfo.timing.sample_rate;
        _app.InitAudio(sampleRate);
        logger.LogDebug("Using sample rate: {SampleRate}", sampleRate);

        // Run for a few frames
        for (int i = 0; i < 120; i++) // 1 second at 60fps
        {
            _core.Run();
            Thread.Sleep(16);
        }

        // Wait for async audio to play
        Thread.Sleep(2000);

        // Assert
        Assert.True(_audioReceived, "Audio should have been received during gameplay");

        // Cleanup
        Marshal.FreeHGlobal(romPtr);
        Marshal.FreeHGlobal(gameInfo.path);
    }

    private nuint AudioCallback(IntPtr data, nuint frames)
    {
        _audioReceived = true;
        if (_app != null && data != IntPtr.Zero && frames > 0)
        {
            int sampleCount = (int)frames * 2; // stereo
            short[] samples = new short[sampleCount];
            Marshal.Copy(data, samples, 0, sampleCount);
            _app.PlayAudio(samples);
        }
        return frames;
    }

    [Fact]
    public void TestApplicationAudio()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return;
        }

        var app = new Application();
        app.InitAudio();
        short[] samples = new short[8820]; // 0.1 second at 44100 * 2 channels
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = (short)(i % 1000);
        }
        app.PlayAudio(samples);
        Thread.Sleep(1000); // Wait for audio to play
    }
}