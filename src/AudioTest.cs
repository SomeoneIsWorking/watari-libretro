using System.IO;
using System.Runtime.InteropServices;
using watari_libretro;
using Watari.Controls.Platform;

class AudioTest
{
    static LibretroCore? _core;
    static Application? _application;
    static bool _audioReceived = false;

    public static void Main()
    {
        Console.WriteLine("Starting audio test...");

        // Arrange
        _core = new LibretroCore();
        _application = new Application();
        _application.InitAudio();
        string corePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "cores", "mgba_libretro.dylib");

        // Assert file exists
        if (!File.Exists(corePath))
        {
            Console.WriteLine($"Core file not found at {corePath}");
            return;
        }

        // Act: Load core
        _core.Load(corePath);
        _core.SetEnvironment((cmd, data) => true);
        _core.Init();

        // Set callbacks
        _core.SetVideoRefresh((data, width, height, pitch) => { });
        _core.SetAudioSampleBatch(AudioCallback);
        _core.SetInputPoll(() => { });
        _core.SetInputState((port, device, index, id) => 0);

        // Load game
        string romPath = "/Users/barishamil/Pets/kirby.gba";
        if (!File.Exists(romPath))
        {
            Console.WriteLine($"ROM file not found at {romPath}");
            return;
        }
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

        Console.WriteLine("Running game for 5 seconds...");

        // Run for a few frames
        for (int i = 0; i < 300; i++) // 5 seconds at 60fps
        {
            _core.Run();
            if (i % 60 == 0) Console.Write(".");
        }

        Console.WriteLine();

        // Assert
        if (_audioReceived)
        {
            Console.WriteLine("Audio was received!");
        }
        else
        {
            Console.WriteLine("Audio was not received.");
        }

        // Cleanup
        Marshal.FreeHGlobal(romPtr);
        Marshal.FreeHGlobal(gameInfo.path);
        _core?.Dispose();
    }

    static nuint AudioCallback(IntPtr data, nuint frames)
    {
        _audioReceived = true;
        // Assuming stereo, 2 channels
        int sampleCount = (int)frames * 2;
        short[] samples = new short[sampleCount];
        Marshal.Copy(data, samples, 0, sampleCount);
        _application?.PlayAudio(samples);
        return frames;
    }
}