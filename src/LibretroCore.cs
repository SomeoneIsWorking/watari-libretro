using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Reflection;
using Microsoft.Extensions.Logging;
using watari_libretro.libretro;

namespace watari_libretro;

public unsafe class LibretroCore : IDisposable
{
    private IntPtr handle;
    private readonly LibretroFunctionPointers functions;

    private readonly retro_video_refresh_callback? _videoRefreshCallback;
    private readonly retro_audio_sample_callback? _audioSampleCallback;
    private readonly retro_audio_sample_batch_callback? _audioSampleBatchCallback;
    private readonly retro_input_poll_callback? _inputPollCallback;
    private readonly retro_input_state_callback? _inputStateCallback;
    private readonly retro_environment_callback? _environmentCallback;
    private retro_log_printf_callback? _logCallback;

    public event retro_video_refresh_callback? VideoRefresh;
    public event retro_audio_sample_callback? AudioSample;
    public event retro_audio_sample_batch_callback? AudioSampleBatch;
    public event retro_input_poll_callback? InputPoll;
    public event retro_input_state_callback? InputState;
    public event retro_environment_callback? Environment;
    public event Action<retro_log_level, string>? Log;

    private readonly ILogger _logger;

    public retro_pixel_format PixelFormat { get; private set; }
    public double SampleRate { get; private set; }
    public Dictionary<string, string> Variables { get; } = [];
    public Dictionary<string, string> VariableDefinitions { get; } = [];
    public static List<uint> EnvironmentCommands { get; } = [];
    public static List<string> QueriedVariables { get; } = [];
    private static readonly HashSet<uint> loggedUnhandledCommands = [];

    public LibretroCore(string path, ILogger logger)
    {
        _logger = logger;
        if (handle != IntPtr.Zero) throw new InvalidOperationException("Already loaded");
        handle = NativeLibrary.Load(path);
        functions = new LibretroFunctionPointers(handle);

        // Automatically register callbacks to fire events
        _videoRefreshCallback = new retro_video_refresh_callback(InstanceVideoRefresh);
        functions.retro_set_video_refresh(Marshal.GetFunctionPointerForDelegate(_videoRefreshCallback));
        _audioSampleCallback = new retro_audio_sample_callback(InstanceAudioSample);
        functions.retro_set_audio_sample(Marshal.GetFunctionPointerForDelegate(_audioSampleCallback));
        _audioSampleBatchCallback = new retro_audio_sample_batch_callback(InstanceAudioSampleBatch);
        functions.retro_set_audio_sample_batch(Marshal.GetFunctionPointerForDelegate(_audioSampleBatchCallback));
        _inputPollCallback = new retro_input_poll_callback(InstanceInputPoll);
        functions.retro_set_input_poll(Marshal.GetFunctionPointerForDelegate(_inputPollCallback));
        _inputStateCallback = new retro_input_state_callback(InstanceInputState);
        functions.retro_set_input_state(Marshal.GetFunctionPointerForDelegate(_inputStateCallback));
        _environmentCallback = new retro_environment_callback(InstanceEnvironment);
        functions.retro_set_environment(Marshal.GetFunctionPointerForDelegate(_environmentCallback));
    }

    public void Init()
    {
        functions.retro_init();
    }

    public void Deinit()
    {
        functions.retro_deinit();
    }

    public uint ApiVersion() => functions.retro_api_version();

    public retro_system_info GetSystemInfo()
    {
        retro_system_info info = new();
        functions.retro_get_system_info((IntPtr)Unsafe.AsPointer(ref info));
        return info;
    }

    public retro_system_av_info GetSystemAvInfo()
    {
        retro_system_av_info info = new();
        functions.retro_get_system_av_info((IntPtr)Unsafe.AsPointer(ref info));
        return info;
    }

    public bool LoadGame(retro_game_info gameInfo)
    {
        _logger.LogInformation("Calling retro_load_game");
        bool result = functions.retro_load_game((IntPtr)Unsafe.AsPointer(ref gameInfo));
        _logger.LogInformation("retro_load_game returned {Result}", result);
        return result;
    }

    public void UnloadGame()
    {
        functions.retro_unload_game();
    }

    public void Run()
    {
        functions.retro_run();
    }

    public void Reset()
    {
        functions.retro_reset();
    }

    public nuint SerializeSize() => functions.retro_serialize_size();

    public bool Serialize(IntPtr data, nuint size) => functions.retro_serialize(data, size);

    public bool Unserialize(IntPtr data, nuint size) => functions.retro_unserialize(data, size);

    public void CheatReset() => functions.retro_cheat_reset();

    public void CheatSet(uint index, bool enabled, string code) => functions.retro_cheat_set(index, enabled, Marshal.StringToHGlobalAnsi(code));

    public uint GetRegion() => functions.retro_get_region();

    public IntPtr GetMemoryData(uint id) => functions.retro_get_memory_data(id);

    public nuint GetMemorySize(uint id) => functions.retro_get_memory_size(id);

    public void Dispose()
    {
        if (handle != IntPtr.Zero)
        {
            NativeLibrary.Free(handle);
            handle = IntPtr.Zero;
        }
        GC.SuppressFinalize(this);
    }

    private bool HandleGetSystemDirectory(IntPtr data)
    {
        if (data != IntPtr.Zero)
        {
            // Return a dummy system directory
            Marshal.WriteIntPtr(data, Marshal.StringToHGlobalAnsi("/tmp"));
        }
        return true;
    }

    private bool HandleGetSaveDirectory(IntPtr data)
    {
        if (data != IntPtr.Zero)
        {
            // Return a dummy save directory
            Marshal.WriteIntPtr(data, Marshal.StringToHGlobalAnsi("/tmp"));
        }
        return true;
    }

    private bool HandleSetPixelFormat(IntPtr data)
    {
        PixelFormat = (retro_pixel_format)Marshal.ReadInt32(data);
        _logger.LogInformation("SET_PIXEL_FORMAT: {PixelFormat}", PixelFormat);
        return true;
    }

    private bool HandleSetSystemAvInfo(IntPtr data)
    {
        if (data != IntPtr.Zero)
        {
            retro_system_av_info avInfo = Marshal.PtrToStructure<retro_system_av_info>(data);
            SampleRate = avInfo.timing.sample_rate;
        }
        return true;
    }

    private bool HandleGetLogInterface(IntPtr data)
    {
        _logCallback = new retro_log_printf_callback(InstanceLog);
        retro_log_callback logCallback = new() { log = Marshal.GetFunctionPointerForDelegate(_logCallback) };
        Marshal.StructureToPtr(logCallback, data, false);
        return true;
    }

    private bool HandleGetVariable(IntPtr data)
    {
        if (data == IntPtr.Zero)
        {
            return false;
        }
        var variable = Marshal.PtrToStructure<retro_variable>(data);
        string key = Marshal.PtrToStringAnsi(variable.key) ?? "";
        QueriedVariables.Add(key);
        _logger.LogDebug("GET_VARIABLE: {Key}", key);
        if (Variables.TryGetValue(key, out string? value))
        {
            variable.value = Marshal.StringToHGlobalAnsi(value);
            Marshal.StructureToPtr(variable, data, false);
            _logger.LogDebug("SET_VARIABLE: {Key} = {Value}", key, value);
            return true;
        }
        return false;
    }

    private bool HandleSetVariables(IntPtr data)
    {
        if (data != IntPtr.Zero)
        {
            IntPtr ptr = data;
            while (true)
            {
                retro_variable var = Marshal.PtrToStructure<retro_variable>(ptr);
                if (var.key == IntPtr.Zero) break;
                string key = Marshal.PtrToStringAnsi(var.key) ?? "";
                string value = Marshal.PtrToStringAnsi(var.value) ?? "";
                VariableDefinitions[key] = value;
                _logger.LogInformation("SET_VARIABLES: {Key} = {Value}", key, value);
                ptr += Marshal.SizeOf<retro_variable>();
            }
        }
        return true;
    }

    private void InstanceVideoRefresh(IntPtr data, uint width, uint height, nuint pitch)
    {
        VideoRefresh?.Invoke(data, width, height, pitch);
    }

    private void InstanceAudioSample(short left, short right)
    {
        AudioSample?.Invoke(left, right);
    }

    private nuint InstanceAudioSampleBatch(IntPtr data, nuint frames)
    {
        return AudioSampleBatch?.Invoke(data, frames) ?? 0;
    }

    private void InstanceInputPoll()
    {
        InputPoll?.Invoke();
    }

    private short InstanceInputState(uint port, uint device, uint index, uint id)
    {
        return InputState?.Invoke(port, device, index, id) ?? 0;
    }

    private bool InstanceEnvironment(uint cmd, IntPtr data)
    {
        cmd &= ~RetroEnvironment.RETRO_ENVIRONMENT_EXPERIMENTAL;
        EnvironmentCommands.Add(cmd);
        if (cmd == RetroEnvironment.RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY)
            return HandleGetSystemDirectory(data);
        if (cmd == RetroEnvironment.RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY)
            return HandleGetSaveDirectory(data);
        if (cmd == RetroEnvironment.RETRO_ENVIRONMENT_SET_PIXEL_FORMAT)
            return HandleSetPixelFormat(data);
        if (cmd == RetroEnvironment.RETRO_ENVIRONMENT_SET_SYSTEM_AV_INFO)
            return HandleSetSystemAvInfo(data);
        if (cmd == RetroEnvironment.RETRO_ENVIRONMENT_GET_LOG_INTERFACE)
            return HandleGetLogInterface(data);
        if (cmd == RetroEnvironment.RETRO_ENVIRONMENT_GET_VARIABLE)
            return HandleGetVariable(data);
        if (cmd == RetroEnvironment.RETRO_ENVIRONMENT_SET_VARIABLES)
            return HandleSetVariables(data);

        if (!loggedUnhandledCommands.Contains(cmd))
        {
            _logger.LogWarning("Unhandled environment cmd: {CommandName}", RetroEnvironment.GetEnvironmentCommandName(cmd));
            loggedUnhandledCommands.Add(cmd);
        }
        return Environment?.Invoke(cmd, data) ?? false;
    }

    private void InstanceLog(retro_log_level level, IntPtr fmt)
    {
        string? message = Marshal.PtrToStringAnsi(fmt);
        Log?.Invoke(level, message ?? "");
    }
}