using System.IO;
using System.Runtime.InteropServices;
using Xunit;
using watari_libretro;

namespace watari_libretro.Tests;

[Collection("Libretro")]
public class LibretroCoreTests : IDisposable
{
    private LibretroCore? _core;

    public void Dispose()
    {
        _core?.Dispose();
    }

    [Fact]
    public void LoadCore_ShouldLoadSuccessfully()
    {
        // Arrange
        _core = new LibretroCore();
        string corePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "src", "cores", "mgba_libretro.dylib");

        // Assert file exists
        Assert.True(File.Exists(corePath), $"Core file not found at {corePath}");

        // Act
        _core.Load(corePath);

        // Assert
        Assert.NotNull(_core);
    }

    [Fact]
    public void ApiVersion_ShouldReturnValidVersion()
    {
        // Arrange
        _core = new LibretroCore();
        string corePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "src", "cores", "mgba_libretro.dylib");
        _core.Load(corePath);
        _core.SetEnvironment((cmd, data) => true);
        _core.Init();

        // Act
        uint version = _core.ApiVersion();

        // Assert
        Assert.True(version > 0);
    }

    [Fact]
    public void GetSystemInfo_ShouldReturnValidInfo()
    {
        // Arrange
        _core = new LibretroCore();
        string corePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "src", "cores", "mgba_libretro.dylib");
        _core.Load(corePath);
        _core.SetEnvironment((cmd, data) => true);
        _core.Init();

        // Act
        var info = _core.GetSystemInfo();

        // Assert
        Assert.NotEqual(IntPtr.Zero, info.library_name);
        Assert.NotEqual(IntPtr.Zero, info.library_version);
    }

    [Fact]
    public void SetEnvironment_ShouldSetCallback()
    {
        // Arrange
        _core = new LibretroCore();
        string corePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "src", "cores", "mgba_libretro.dylib");
        _core.Load(corePath);

        retro_environment_callback callback = (cmd, data) => true;

        // Act
        bool result = _core.SetEnvironment(callback);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GetSystemAvInfo_ShouldReturnValidInfo()
    {
        // Arrange
        _core = new LibretroCore();
        string corePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "src", "cores", "mgba_libretro.dylib");
        _core.Load(corePath);
        _core.SetEnvironment((cmd, data) => true);
        _core.Init();

        string romPath = "/Users/barishamil/Pets/kirby.gba";
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

        // Act
        var avInfo = _core.GetSystemAvInfo();

        // Assert
        Assert.True(avInfo.geometry.base_width > 0);
        Assert.True(avInfo.geometry.base_height > 0);
        Assert.True(avInfo.timing.fps > 0);
        Assert.True(avInfo.timing.sample_rate > 0);

        // Cleanup
        Marshal.FreeHGlobal(romPtr);
        Marshal.FreeHGlobal(gameInfo.path);
    }

    // }

    [Fact]
    public void GetRegion_ShouldReturnValidRegion()
    {
        // Arrange
        _core = new LibretroCore();
        string corePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "src", "cores", "mgba_libretro.dylib");
        _core.Load(corePath);
        _core.SetEnvironment((cmd, data) => true);
        _core.Init();

        // Act
        uint region = _core.GetRegion();

        // Assert
        Assert.True(region == (uint)retro_region.RETRO_REGION_NTSC || region == (uint)retro_region.RETRO_REGION_PAL);
    }

    [Fact]
    public void UnloadGame_ShouldUnloadSuccessfully()
    {
        // Arrange
        _core = new LibretroCore();
        string corePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "src", "cores", "mgba_libretro.dylib");
        _core.Load(corePath);
        _core.SetEnvironment((cmd, data) => true);
        _core.Init();

        string romPath = "/Users/barishamil/Pets/kirby.gba";
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

        // Act & Assert
        var exception = Record.Exception(() => _core.UnloadGame());
        Assert.Null(exception);

        // Cleanup
        Marshal.FreeHGlobal(romPtr);
        Marshal.FreeHGlobal(gameInfo.path);
    }

    [Fact]
    public void Reset_ShouldResetSuccessfully()
    {
        // Arrange
        _core = new LibretroCore();
        string corePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "src", "cores", "mgba_libretro.dylib");
        _core.Load(corePath);
        _core.SetEnvironment((cmd, data) => true);
        _core.Init();

        string romPath = "/Users/barishamil/Pets/kirby.gba";
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

        // Act & Assert
        var exception = Record.Exception(() => _core.Reset());
        Assert.Null(exception);

        // Cleanup
        Marshal.FreeHGlobal(romPtr);
        Marshal.FreeHGlobal(gameInfo.path);
    }

    [Fact]
    public void SerializeSize_ShouldReturnValidSize()
    {
        // Arrange
        _core = new LibretroCore();
        string corePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "src", "cores", "mgba_libretro.dylib");
        _core.Load(corePath);
        _core.SetEnvironment((cmd, data) => true);
        _core.Init();

        string romPath = "/Users/barishamil/Pets/kirby.gba";
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

        // Act
        nuint size = _core.SerializeSize();

        // Assert
        Assert.True(size > 0);

        // Cleanup
        Marshal.FreeHGlobal(romPtr);
        Marshal.FreeHGlobal(gameInfo.path);
    }

    [Fact]
    public void GetMemorySize_ShouldReturnValidSize()
    {
        // Arrange
        _core = new LibretroCore();
        string corePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "src", "cores", "mgba_libretro.dylib");
        _core.Load(corePath);
        _core.SetEnvironment((cmd, data) => true);
        _core.Init();

        string romPath = "/Users/barishamil/Pets/kirby.gba";
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

        // Act
        nuint size = _core.GetMemorySize((uint)retro_memory_type.RETRO_MEMORY_SYSTEM_RAM);

        // Assert
        Assert.True(size > 0);

        // Cleanup
        Marshal.FreeHGlobal(romPtr);
        Marshal.FreeHGlobal(gameInfo.path);
    }
}

[CollectionDefinition("Libretro")]
public class LibretroCollection : ICollectionFixture<LibretroCoreTests>
{
}