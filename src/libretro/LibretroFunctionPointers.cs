using System.Runtime.InteropServices;

namespace watari_libretro.libretro;

public class LibretroFunctionPointers(IntPtr handle)
{
    public retro_init_t retro_init = Marshal.GetDelegateForFunctionPointer<retro_init_t>(GetProcAddress(handle, "retro_init"));
    public retro_deinit_t retro_deinit = Marshal.GetDelegateForFunctionPointer<retro_deinit_t>(GetProcAddress(handle, "retro_deinit"));
    public retro_api_version_t retro_api_version = Marshal.GetDelegateForFunctionPointer<retro_api_version_t>(GetProcAddress(handle, "retro_api_version"));
    public retro_get_system_info_t retro_get_system_info = Marshal.GetDelegateForFunctionPointer<retro_get_system_info_t>(GetProcAddress(handle, "retro_get_system_info"));
    public retro_get_system_av_info_t retro_get_system_av_info = Marshal.GetDelegateForFunctionPointer<retro_get_system_av_info_t>(GetProcAddress(handle, "retro_get_system_av_info"));
    public retro_set_environment_t retro_set_environment = Marshal.GetDelegateForFunctionPointer<retro_set_environment_t>(GetProcAddress(handle, "retro_set_environment"));
    public retro_set_video_refresh_t retro_set_video_refresh = Marshal.GetDelegateForFunctionPointer<retro_set_video_refresh_t>(GetProcAddress(handle, "retro_set_video_refresh"));
    public retro_set_audio_sample_t retro_set_audio_sample = Marshal.GetDelegateForFunctionPointer<retro_set_audio_sample_t>(GetProcAddress(handle, "retro_set_audio_sample"));
    public retro_set_audio_sample_batch_t retro_set_audio_sample_batch = Marshal.GetDelegateForFunctionPointer<retro_set_audio_sample_batch_t>(GetProcAddress(handle, "retro_set_audio_sample_batch"));
    public retro_set_input_poll_t retro_set_input_poll = Marshal.GetDelegateForFunctionPointer<retro_set_input_poll_t>(GetProcAddress(handle, "retro_set_input_poll"));
    public retro_set_input_state_t retro_set_input_state = Marshal.GetDelegateForFunctionPointer<retro_set_input_state_t>(GetProcAddress(handle, "retro_set_input_state"));
    public retro_load_game_t retro_load_game = Marshal.GetDelegateForFunctionPointer<retro_load_game_t>(GetProcAddress(handle, "retro_load_game"));
    public retro_unload_game_t retro_unload_game = Marshal.GetDelegateForFunctionPointer<retro_unload_game_t>(GetProcAddress(handle, "retro_unload_game"));
    public retro_run_t retro_run = Marshal.GetDelegateForFunctionPointer<retro_run_t>(GetProcAddress(handle, "retro_run"));
    public retro_reset_t retro_reset = Marshal.GetDelegateForFunctionPointer<retro_reset_t>(GetProcAddress(handle, "retro_reset"));
    public retro_serialize_size_t retro_serialize_size = Marshal.GetDelegateForFunctionPointer<retro_serialize_size_t>(GetProcAddress(handle, "retro_serialize_size"));
    public retro_serialize_t retro_serialize = Marshal.GetDelegateForFunctionPointer<retro_serialize_t>(GetProcAddress(handle, "retro_serialize"));
    public retro_unserialize_t retro_unserialize = Marshal.GetDelegateForFunctionPointer<retro_unserialize_t>(GetProcAddress(handle, "retro_unserialize"));
    public retro_cheat_reset_t retro_cheat_reset = Marshal.GetDelegateForFunctionPointer<retro_cheat_reset_t>(GetProcAddress(handle, "retro_cheat_reset"));
    public retro_cheat_set_t retro_cheat_set = Marshal.GetDelegateForFunctionPointer<retro_cheat_set_t>(GetProcAddress(handle, "retro_cheat_set"));
    public retro_get_region_t retro_get_region = Marshal.GetDelegateForFunctionPointer<retro_get_region_t>(GetProcAddress(handle, "retro_get_region"));
    public retro_get_memory_data_t retro_get_memory_data = Marshal.GetDelegateForFunctionPointer<retro_get_memory_data_t>(GetProcAddress(handle, "retro_get_memory_data"));
    public retro_get_memory_size_t retro_get_memory_size = Marshal.GetDelegateForFunctionPointer<retro_get_memory_size_t>(GetProcAddress(handle, "retro_get_memory_size"));

    private static IntPtr GetProcAddress(IntPtr handle, string name)
    {
        IntPtr proc = NativeLibrary.GetExport(handle, name);
        if (proc == IntPtr.Zero)
        {
            throw new MissingMethodException($"Function {name} not found in the native library.");
        }
        return proc;
    }
}