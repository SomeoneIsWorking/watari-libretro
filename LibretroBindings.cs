using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace watari_libretro;

public enum retro_pixel_format : uint
{
    RETRO_PIXEL_FORMAT_0RGB1555 = 0,
    RETRO_PIXEL_FORMAT_XRGB8888 = 1,
    RETRO_PIXEL_FORMAT_RGB565 = 2,
    RETRO_PIXEL_FORMAT_UNKNOWN = 0xFFFFFFFF
}

public enum retro_region : uint
{
    RETRO_REGION_NTSC = 0,
    RETRO_REGION_PAL = 1
}

public enum retro_language : uint
{
    RETRO_LANGUAGE_ENGLISH = 0,
    RETRO_LANGUAGE_JAPANESE = 1,
    RETRO_LANGUAGE_FRENCH = 2,
    RETRO_LANGUAGE_SPANISH = 3,
    RETRO_LANGUAGE_GERMAN = 4,
    RETRO_LANGUAGE_ITALIAN = 5,
    RETRO_LANGUAGE_DUTCH = 6,
    RETRO_LANGUAGE_PORTUGUESE_BRAZIL = 7,
    RETRO_LANGUAGE_PORTUGUESE_PORTUGAL = 8,
    RETRO_LANGUAGE_RUSSIAN = 9,
    RETRO_LANGUAGE_KOREAN = 10,
    RETRO_LANGUAGE_CHINESE_TRADITIONAL = 11,
    RETRO_LANGUAGE_CHINESE_SIMPLIFIED = 12,
    RETRO_LANGUAGE_ESPERANTO = 13,
    RETRO_LANGUAGE_POLISH = 14,
    RETRO_LANGUAGE_VIETNAMESE = 15,
    RETRO_LANGUAGE_ARABIC = 16,
    RETRO_LANGUAGE_GREEK = 17,
    RETRO_LANGUAGE_TURKISH = 18,
    RETRO_LANGUAGE_SLOVAK = 19,
    RETRO_LANGUAGE_PERSIAN = 20,
    RETRO_LANGUAGE_HEBREW = 21,
    RETRO_LANGUAGE_ASTURIAN = 22,
    RETRO_LANGUAGE_FINNISH = 23,
    RETRO_LANGUAGE_LAST
}

public enum retro_key : uint
{
    RETROK_UNKNOWN = 0,
    RETROK_FIRST = 0,
    RETROK_BACKSPACE = 8,
    RETROK_TAB = 9,
    RETROK_CLEAR = 12,
    RETROK_RETURN = 13,
    RETROK_PAUSE = 19,
    RETROK_ESCAPE = 27,
    RETROK_SPACE = 32,
    RETROK_EXCLAIM = 33,
    RETROK_QUOTEDBL = 34,
    RETROK_HASH = 35,
    RETROK_DOLLAR = 36,
    RETROK_AMPERSAND = 38,
    RETROK_QUOTE = 39,
    RETROK_LEFTPAREN = 40,
    RETROK_RIGHTPAREN = 41,
    RETROK_ASTERISK = 42,
    RETROK_PLUS = 43,
    RETROK_COMMA = 44,
    RETROK_MINUS = 45,
    RETROK_PERIOD = 46,
    RETROK_SLASH = 47,
    RETROK_0 = 48,
    RETROK_1 = 49,
    RETROK_2 = 50,
    RETROK_3 = 51,
    RETROK_4 = 52,
    RETROK_5 = 53,
    RETROK_6 = 54,
    RETROK_7 = 55,
    RETROK_8 = 56,
    RETROK_9 = 57,
    RETROK_COLON = 58,
    RETROK_SEMICOLON = 59,
    RETROK_LESS = 60,
    RETROK_EQUALS = 61,
    RETROK_GREATER = 62,
    RETROK_QUESTION = 63,
    RETROK_AT = 64,
    RETROK_LEFTBRACKET = 91,
    RETROK_BACKSLASH = 92,
    RETROK_RIGHTBRACKET = 93,
    RETROK_CARET = 94,
    RETROK_UNDERSCORE = 95,
    RETROK_BACKQUOTE = 96,
    RETROK_a = 97,
    RETROK_b = 98,
    RETROK_c = 99,
    RETROK_d = 100,
    RETROK_e = 101,
    RETROK_f = 102,
    RETROK_g = 103,
    RETROK_h = 104,
    RETROK_i = 105,
    RETROK_j = 106,
    RETROK_k = 107,
    RETROK_l = 108,
    RETROK_m = 109,
    RETROK_n = 110,
    RETROK_o = 111,
    RETROK_p = 112,
    RETROK_q = 113,
    RETROK_r = 114,
    RETROK_s = 115,
    RETROK_t = 116,
    RETROK_u = 117,
    RETROK_v = 118,
    RETROK_w = 119,
    RETROK_x = 120,
    RETROK_y = 121,
    RETROK_z = 122,
    RETROK_LEFTBRACE = 123,
    RETROK_BAR = 124,
    RETROK_RIGHTBRACE = 125,
    RETROK_TILDE = 126,
    RETROK_DELETE = 127,
    RETROK_KP0 = 256,
    RETROK_KP1 = 257,
    RETROK_KP2 = 258,
    RETROK_KP3 = 259,
    RETROK_KP4 = 260,
    RETROK_KP5 = 261,
    RETROK_KP6 = 262,
    RETROK_KP7 = 263,
    RETROK_KP8 = 264,
    RETROK_KP9 = 265,
    RETROK_KP_PERIOD = 266,
    RETROK_KP_DIVIDE = 267,
    RETROK_KP_MULTIPLY = 268,
    RETROK_KP_MINUS = 269,
    RETROK_KP_PLUS = 270,
    RETROK_KP_ENTER = 271,
    RETROK_KP_EQUALS = 272,
    RETROK_UP = 273,
    RETROK_DOWN = 274,
    RETROK_RIGHT = 275,
    RETROK_LEFT = 276,
    RETROK_INSERT = 277,
    RETROK_HOME = 278,
    RETROK_END = 279,
    RETROK_PAGEUP = 280,
    RETROK_PAGEDOWN = 281,
    RETROK_F1 = 282,
    RETROK_F2 = 283,
    RETROK_F3 = 284,
    RETROK_F4 = 285,
    RETROK_F5 = 286,
    RETROK_F6 = 287,
    RETROK_F7 = 288,
    RETROK_F8 = 289,
    RETROK_F9 = 290,
    RETROK_F10 = 291,
    RETROK_F11 = 292,
    RETROK_F12 = 293,
    RETROK_F13 = 294,
    RETROK_F14 = 295,
    RETROK_F15 = 296,
    RETROK_NUMLOCK = 300,
    RETROK_CAPSLOCK = 301,
    RETROK_SCROLLOCK = 302,
    RETROK_RSHIFT = 303,
    RETROK_LSHIFT = 304,
    RETROK_RCTRL = 305,
    RETROK_LCTRL = 306,
    RETROK_RALT = 307,
    RETROK_LALT = 308,
    RETROK_RMETA = 309,
    RETROK_LMETA = 310,
    RETROK_LSUPER = 311,
    RETROK_RSUPER = 312,
    RETROK_MODE = 313,
    RETROK_COMPOSE = 314,
    RETROK_HELP = 315,
    RETROK_PRINT = 316,
    RETROK_SYSREQ = 317,
    RETROK_BREAK = 318,
    RETROK_MENU = 319,
    RETROK_POWER = 320,
    RETROK_EURO = 321,
    RETROK_UNDO = 322,
    RETROK_OEM_102 = 323,
    RETROK_LAST
}

public enum retro_mod : uint
{
    RETROKMOD_NONE = 0x0000,
    RETROKMOD_SHIFT = 0x01,
    RETROKMOD_CTRL = 0x02,
    RETROKMOD_ALT = 0x04,
    RETROKMOD_META = 0x08,
    RETROKMOD_NUMLOCK = 0x10,
    RETROKMOD_CAPSLOCK = 0x20,
    RETROKMOD_SCROLLOCK = 0x40
}

public enum retro_environment : uint
{
    RETRO_ENVIRONMENT_SET_ROTATION = 1,
    RETRO_ENVIRONMENT_GET_OVERSCAN = 2,
    RETRO_ENVIRONMENT_GET_CAN_DUPE = 3,
    RETRO_ENVIRONMENT_SET_MESSAGE = 6,
    RETRO_ENVIRONMENT_SHUTDOWN = 7,
    RETRO_ENVIRONMENT_SET_PERFORMANCE_LEVEL = 8,
    RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY = 9,
    RETRO_ENVIRONMENT_SET_PIXEL_FORMAT = 10,
    RETRO_ENVIRONMENT_SET_INPUT_DESCRIPTORS = 11,
    RETRO_ENVIRONMENT_SET_KEYBOARD_CALLBACK = 12,
    RETRO_ENVIRONMENT_SET_DISK_CONTROL_INTERFACE = 13,
    RETRO_ENVIRONMENT_SET_HW_RENDER = 14,
    RETRO_ENVIRONMENT_GET_VARIABLE = 15,
    RETRO_ENVIRONMENT_SET_VARIABLES = 16,
    RETRO_ENVIRONMENT_GET_VARIABLE_UPDATE = 17,
    RETRO_ENVIRONMENT_SET_SUPPORT_NO_GAME = 18,
    RETRO_ENVIRONMENT_GET_LIBRETRO_PATH = 19,
    RETRO_ENVIRONMENT_SET_FRAME_TIME_CALLBACK = 21,
    RETRO_ENVIRONMENT_SET_AUDIO_CALLBACK = 22,
    RETRO_ENVIRONMENT_GET_RUMBLE_INTERFACE = 23,
    RETRO_ENVIRONMENT_GET_INPUT_DEVICE_CAPABILITIES = 24,
    RETRO_ENVIRONMENT_GET_SENSOR_INTERFACE = 25,
    RETRO_ENVIRONMENT_GET_CAMERA_INTERFACE = 26,
    RETRO_ENVIRONMENT_GET_LOG_INTERFACE = 27,
    RETRO_ENVIRONMENT_GET_PERF_INTERFACE = 28,
    RETRO_ENVIRONMENT_GET_LOCATION_INTERFACE = 29,
    RETRO_ENVIRONMENT_GET_CONTENT_DIRECTORY = 30,
    RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY = 31,
    RETRO_ENVIRONMENT_SET_SYSTEM_AV_INFO = 32,
    RETRO_ENVIRONMENT_SET_PROC_ADDRESS_CALLBACK = 33,
    RETRO_ENVIRONMENT_SET_SUBSYSTEM_INFO = 34,
    RETRO_ENVIRONMENT_SET_CONTROLLER_INFO = 35,
    RETRO_ENVIRONMENT_SET_MEMORY_MAPS = 36,
    RETRO_ENVIRONMENT_SET_GEOMETRY = 37,
    RETRO_ENVIRONMENT_GET_USERNAME = 38,
    RETRO_ENVIRONMENT_GET_LANGUAGE = 39,
    RETRO_ENVIRONMENT_GET_CURRENT_SOFTWARE_FRAMEBUFFER = 40,
    RETRO_ENVIRONMENT_GET_HW_RENDER_INTERFACE = 41,
    RETRO_ENVIRONMENT_SET_SUPPORT_ACHIEVEMENTS = 42,
    RETRO_ENVIRONMENT_SET_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE = 43,
    RETRO_ENVIRONMENT_SET_SERIALIZATION_QUIRKS = 44,
    RETRO_ENVIRONMENT_SET_HW_SHARED_CONTEXT = 45,
    RETRO_ENVIRONMENT_GET_VFS_INTERFACE = 46,
    RETRO_ENVIRONMENT_GET_LED_INTERFACE = 47,
    RETRO_ENVIRONMENT_GET_AUDIO_VIDEO_ENABLE = 48,
    RETRO_ENVIRONMENT_GET_MIDI_INTERFACE = 49,
    RETRO_ENVIRONMENT_GET_FASTFORWARDING = 50,
    RETRO_ENVIRONMENT_GET_TARGET_REFRESH_RATE = 51,
    RETRO_ENVIRONMENT_GET_INPUT_BITMASKS = 52,
    RETRO_ENVIRONMENT_SET_FRAME_TIME_CALLBACK2 = 53,
    RETRO_ENVIRONMENT_GET_THROTTLE_STATE = 54,
    RETRO_ENVIRONMENT_GET_SLOWMOTION_RATIO = 55,
    RETRO_ENVIRONMENT_SET_AUDIO_BUFFER_STATUS_CALLBACK = 56,
    RETRO_ENVIRONMENT_SET_MINIMUM_FRAME_TIME_DELTA = 57
}

public delegate void retro_video_refresh_callback(IntPtr data, uint width, uint height, nuint pitch);
public delegate void retro_audio_sample_callback(short left, short right);
public delegate nuint retro_audio_sample_batch_callback(IntPtr data, nuint frames);
public delegate void retro_input_poll_callback();
public delegate short retro_input_state_callback(uint port, uint device, uint index, uint id);
public delegate bool retro_environment_callback(uint cmd, IntPtr data);

public enum retro_hw_render_interface_type : uint
{
    RETRO_HW_RENDER_INTERFACE_VULKAN = 0,
    RETRO_HW_RENDER_INTERFACE_D3D9_CG = 1,
    RETRO_HW_RENDER_INTERFACE_D3D9_HLSL = 2,
    RETRO_HW_RENDER_INTERFACE_D3D10 = 3,
    RETRO_HW_RENDER_INTERFACE_D3D11 = 4,
    RETRO_HW_RENDER_INTERFACE_D3D12 = 5,
    RETRO_HW_RENDER_INTERFACE_GLES2 = 6,
    RETRO_HW_RENDER_INTERFACE_GLES3 = 7,
    RETRO_HW_RENDER_INTERFACE_GL_CORE = 8,
    RETRO_HW_RENDER_INTERFACE_GL = 9,
    RETRO_HW_RENDER_INTERFACE_VULKAN_VERSION = 10
}

public enum retro_rumble_effect : uint
{
    RETRO_RUMBLE_STRONG = 0,
    RETRO_RUMBLE_WEAK = 1
}

public enum retro_sensor_action : uint
{
    RETRO_SENSOR_ACCELEROMETER_ENABLE = 0,
    RETRO_SENSOR_ACCELEROMETER_DISABLE = 1,
    RETRO_SENSOR_GYROSCOPE_ENABLE = 2,
    RETRO_SENSOR_GYROSCOPE_DISABLE = 3,
    RETRO_SENSOR_ILLUMINANCE_ENABLE = 4,
    RETRO_SENSOR_ILLUMINANCE_DISABLE = 5
}

public enum retro_camera_buffer : uint
{
    RETRO_CAMERA_BUFFER_OPENGL_TEXTURE = 0,
    RETRO_CAMERA_BUFFER_RAW_FRAMEBUFFER = 1
}

public enum retro_log_level : uint
{
    RETRO_LOG_DEBUG = 0,
    RETRO_LOG_INFO = 1,
    RETRO_LOG_WARN = 2,
    RETRO_LOG_ERROR = 3
}

public enum retro_hw_render_context_negotiation_interface_type : uint
{
    RETRO_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE_VULKAN = 0,
    RETRO_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE_D3D9 = 1,
    RETRO_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE_D3D10 = 2,
    RETRO_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE_D3D11 = 3,
    RETRO_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE_D3D12 = 4,
    RETRO_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE_GLES2 = 5,
    RETRO_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE_GLES3 = 6,
    RETRO_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE_GL_CORE = 7,
    RETRO_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE_GL = 8
}

public enum retro_memory_type : uint
{
    RETRO_MEMORY_MASK = 0xff,
    RETRO_MEMORY_SAVE_RAM = 0,
    RETRO_MEMORY_RTC = 1,
    RETRO_MEMORY_SYSTEM_RAM = 2,
    RETRO_MEMORY_VIDEO_RAM = 3
}

public enum retro_keybind_set_id : uint
{
    RETRO_KEYBIND_SET_ID_CORE = 0,
    RETRO_KEYBIND_SET_ID_PORT_1 = 1,
    RETRO_KEYBIND_SET_ID_PORT_2 = 2,
    RETRO_KEYBIND_SET_ID_PORT_3 = 3,
    RETRO_KEYBIND_SET_ID_PORT_4 = 4,
    RETRO_KEYBIND_SET_ID_LAST = 5
}

public enum retro_device_type : uint
{
    RETRO_DEVICE_NONE = 0,
    RETRO_DEVICE_JOYPAD = 1,
    RETRO_DEVICE_MOUSE = 2,
    RETRO_DEVICE_KEYBOARD = 3,
    RETRO_DEVICE_LIGHTGUN = 4,
    RETRO_DEVICE_ANALOG = 5,
    RETRO_DEVICE_POINTER = 6
}

public enum retro_device_id_joypad : uint
{
    RETRO_DEVICE_ID_JOYPAD_B = 0,
    RETRO_DEVICE_ID_JOYPAD_Y = 1,
    RETRO_DEVICE_ID_JOYPAD_SELECT = 2,
    RETRO_DEVICE_ID_JOYPAD_START = 3,
    RETRO_DEVICE_ID_JOYPAD_UP = 4,
    RETRO_DEVICE_ID_JOYPAD_DOWN = 5,
    RETRO_DEVICE_ID_JOYPAD_LEFT = 6,
    RETRO_DEVICE_ID_JOYPAD_RIGHT = 7,
    RETRO_DEVICE_ID_JOYPAD_A = 8,
    RETRO_DEVICE_ID_JOYPAD_X = 9,
    RETRO_DEVICE_ID_JOYPAD_L = 10,
    RETRO_DEVICE_ID_JOYPAD_R = 11,
    RETRO_DEVICE_ID_JOYPAD_L2 = 12,
    RETRO_DEVICE_ID_JOYPAD_R2 = 13,
    RETRO_DEVICE_ID_JOYPAD_L3 = 14,
    RETRO_DEVICE_ID_JOYPAD_R3 = 15
}

public enum retro_device_id_mouse : uint
{
    RETRO_DEVICE_ID_MOUSE_X = 0,
    RETRO_DEVICE_ID_MOUSE_Y = 1,
    RETRO_DEVICE_ID_MOUSE_LEFT = 2,
    RETRO_DEVICE_ID_MOUSE_RIGHT = 3,
    RETRO_DEVICE_ID_MOUSE_WHEELUP = 4,
    RETRO_DEVICE_ID_MOUSE_WHEELDOWN = 5,
    RETRO_DEVICE_ID_MOUSE_MIDDLE = 6,
    RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELUP = 7,
    RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELDOWN = 8,
    RETRO_DEVICE_ID_MOUSE_BUTTON_4 = 9,
    RETRO_DEVICE_ID_MOUSE_BUTTON_5 = 10
}

public enum retro_device_id_lightgun : uint
{
    RETRO_DEVICE_ID_LIGHTGUN_SCREEN_X = 13,
    RETRO_DEVICE_ID_LIGHTGUN_SCREEN_Y = 14,
    RETRO_DEVICE_ID_LIGHTGUN_IS_OFFSCREEN = 15,
    RETRO_DEVICE_ID_LIGHTGUN_TRIGGER = 2,
    RETRO_DEVICE_ID_LIGHTGUN_RELOAD = 16,
    RETRO_DEVICE_ID_LIGHTGUN_AUX_A = 3,
    RETRO_DEVICE_ID_LIGHTGUN_AUX_B = 4,
    RETRO_DEVICE_ID_LIGHTGUN_START = 6,
    RETRO_DEVICE_ID_LIGHTGUN_SELECT = 7,
    RETRO_DEVICE_ID_LIGHTGUN_AUX_C = 8,
    RETRO_DEVICE_ID_LIGHTGUN_DPAD_UP = 9,
    RETRO_DEVICE_ID_LIGHTGUN_DPAD_DOWN = 10,
    RETRO_DEVICE_ID_LIGHTGUN_DPAD_LEFT = 11,
    RETRO_DEVICE_ID_LIGHTGUN_DPAD_RIGHT = 12,
    RETRO_DEVICE_ID_LIGHTGUN_X = 0,
    RETRO_DEVICE_ID_LIGHTGUN_Y = 1,
    RETRO_DEVICE_ID_LIGHTGUN_CURSOR = 3,
    RETRO_DEVICE_ID_LIGHTGUN_TURBO = 4,
    RETRO_DEVICE_ID_LIGHTGUN_PAUSE = 5
}

public enum retro_device_id_analog : uint
{
    RETRO_DEVICE_ID_ANALOG_X = 0,
    RETRO_DEVICE_ID_ANALOG_Y = 1
}

public enum retro_device_index_analog : uint
{
    RETRO_DEVICE_INDEX_ANALOG_LEFT = 0,
    RETRO_DEVICE_INDEX_ANALOG_RIGHT = 1
}

public enum retro_device_id_pointer : uint
{
    RETRO_DEVICE_ID_POINTER_X = 0,
    RETRO_DEVICE_ID_POINTER_Y = 1,
    RETRO_DEVICE_ID_POINTER_PRESSED = 2,
    RETRO_DEVICE_ID_POINTER_COUNT = 3
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_system_info
{
    public IntPtr library_name;
    public IntPtr library_version;
    public IntPtr valid_extensions;
    public bool need_fullpath;
    public bool block_extract;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_game_geometry
{
    public uint base_width;
    public uint base_height;
    public uint max_width;
    public uint max_height;
    public float aspect_ratio;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_system_timing
{
    public double fps;
    public double sample_rate;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_system_av_info
{
    public retro_game_geometry geometry;
    public retro_system_timing timing;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_variable
{
    public IntPtr key;
    public IntPtr value;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_game_info
{
    public IntPtr path;
    public IntPtr data;
    public nuint size;
    public IntPtr meta;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_input_descriptor
{
    public uint port;
    public uint device;
    public uint index;
    public uint id;
    public IntPtr description;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_keyboard_callback
{
    public IntPtr callback;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_hw_render_callback
{
    public retro_hw_render_interface_type context_type;
    public retro_hw_render_context_negotiation_interface_type context_reset;
    public IntPtr get_current_framebuffer;
    public IntPtr get_proc_address;
    public bool depth;
    public bool stencil;
    public bool bottom_left_origin;
    public bool version_major;
    public bool version_minor;
    public bool cache_context;
    public IntPtr context_destroy;
    public IntPtr hw_render_interface;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_framebuffer
{
    public IntPtr data;
    public uint width;
    public uint height;
    public nuint pitch;
    public retro_pixel_format format;
    public uint access_flags;
    public uint memory_flags;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_message
{
    public IntPtr msg;
    public uint frames;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_message_ext
{
    public IntPtr msg;
    public uint duration;
    public uint priority;
    public retro_log_level level;
    public IntPtr target;
    public uint verbosity;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_fastforwarding_override
{
    public bool fastforward;
    public float percentage;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_microphone_interface
{
    public IntPtr interface_version;
    public IntPtr open_mic;
    public IntPtr close_mic;
    public IntPtr get_params;
    public IntPtr set_state;
    public IntPtr read;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_camera_callback
{
    public IntPtr caps;
    public IntPtr start;
    public IntPtr stop;
    public IntPtr frame_raw_framebuffer;
    public IntPtr frame_opengl_texture;
    public uint initialized;
    public retro_camera_buffer frame_raw_framebuffer_pixels;
    public uint frame_opengl_texture_id;
    public uint frame_opengl_texture_target;
    public uint frame_opengl_texture_format;
    public IntPtr cleanup;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_location_callback
{
    public IntPtr start;
    public IntPtr stop;
    public IntPtr get_position;
    public IntPtr set_interval;
    public IntPtr initialized;
    public IntPtr deinitialized;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_rumble_interface
{
    public IntPtr set_rumble_state;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_sensor_interface
{
    public IntPtr get_sensor_input;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_log_callback
{
    public IntPtr log;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_perf_callback
{
    public IntPtr perf_register;
    public IntPtr perf_start;
    public IntPtr perf_stop;
    public IntPtr perf_log;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_get_proc_address_interface
{
    public IntPtr get_proc_address;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_subsystem_memory_info
{
    public IntPtr extension;
    public uint type;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_subsystem_info
{
    public IntPtr desc;
    public IntPtr ident;
    public retro_subsystem_memory_info[]? infos;
    public uint num_memory_infos;
    public uint id;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_controller_info
{
    public IntPtr types;
    public uint num_types;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_controller_description
{
    public IntPtr desc;
    public uint id;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_memory_map
{
    public IntPtr descriptors;
    public uint num_descriptors;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_memory_descriptor
{
    public ulong flags;
    public IntPtr ptr;
    public nuint offset;
    public nuint start;
    public nuint select;
    public nuint disconnect;
    public nuint len;
    public IntPtr addrspace;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_input_gamepad_info
{
    public IntPtr desc;
    public uint id;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_input_analog_info
{
    public IntPtr desc;
    public uint id;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_input_keyboard_info
{
    public IntPtr desc;
    public uint id;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_input_mouse_info
{
    public IntPtr desc;
    public uint id;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_input_lightgun_info
{
    public IntPtr desc;
    public uint id;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_input_pointer_info
{
    public IntPtr desc;
    public uint id;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_input_info
{
    public retro_input_gamepad_info gamepad;
    public retro_input_analog_info analog;
    public retro_input_keyboard_info keyboard;
    public retro_input_mouse_info mouse;
    public retro_input_lightgun_info lightgun;
    public retro_input_pointer_info pointer;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_disk_control_callback
{
    public IntPtr set_eject_state;
    public IntPtr get_eject_state;
    public IntPtr get_image_index;
    public IntPtr set_image_index;
    public IntPtr get_num_images;
    public IntPtr replace_image_index;
    public IntPtr add_image_index;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_disk_control_ext_callback
{
    public IntPtr set_eject_state;
    public IntPtr get_eject_state;
    public IntPtr get_image_index;
    public IntPtr set_image_index;
    public IntPtr get_num_images;
    public IntPtr replace_image_index;
    public IntPtr add_image_index;
    public IntPtr set_initial_image;
    public IntPtr get_image_path;
    public IntPtr get_image_label;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_netpacket_send_t
{
    public IntPtr send_packet;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_netpacket_receive_t
{
    public IntPtr receive_packet;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_netpacket_callback
{
    public retro_netpacket_send_t send_packet;
    public retro_netpacket_receive_t receive_packet;
    public IntPtr poll_receive_packet;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_audio_buffer_status_callback
{
    public IntPtr status;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_audio_callback
{
    public IntPtr callback;
    public IntPtr set_state;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_midi_interface
{
    public IntPtr input_enabled;
    public IntPtr output_enabled;
    public IntPtr read;
    public IntPtr write;
    public IntPtr flush;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_led_interface
{
    public IntPtr set_led_state;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_video_refresh_t
{
    public IntPtr callback;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_audio_sample_t
{
    public IntPtr callback;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_audio_sample_batch_t
{
    public IntPtr callback;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_input_poll_t
{
    public IntPtr callback;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_input_state_t
{
    public IntPtr callback;
}

[StructLayout(LayoutKind.Sequential)]
public struct retro_environment_t
{
    public IntPtr callback;
}

public delegate void retro_init_t();
public delegate void retro_deinit_t();
public delegate uint retro_api_version_t();
public delegate void retro_get_system_info_t(IntPtr info);
public delegate void retro_get_system_av_info_t(IntPtr info);
public delegate bool retro_set_environment_t(IntPtr callback);
public delegate void retro_set_video_refresh_t(IntPtr callback);
public delegate void retro_set_audio_sample_t(IntPtr callback);
public delegate void retro_set_audio_sample_batch_t(IntPtr callback);
public delegate void retro_set_input_poll_t(IntPtr callback);
public delegate void retro_set_input_state_t(IntPtr callback);
public delegate bool retro_load_game_t(IntPtr gameInfo);
public delegate void retro_unload_game_t();
public delegate void retro_run_t();
public delegate void retro_reset_t();
public delegate nuint retro_serialize_size_t();
public delegate bool retro_serialize_t(IntPtr data, nuint size);
public delegate bool retro_unserialize_t(IntPtr data, nuint size);
public delegate void retro_cheat_reset_t();
public delegate void retro_cheat_set_t(uint index, bool enabled, IntPtr code);
public delegate uint retro_get_region_t();
public delegate IntPtr retro_get_memory_data_t(uint id);
public delegate nuint retro_get_memory_size_t(uint id);

public unsafe class LibretroCore
{
    private static LibretroCore? currentInstance;
    private IntPtr handle;
    private retro_init_t? retro_init;
    private retro_deinit_t? retro_deinit;
    private retro_api_version_t? retro_api_version;
    private retro_get_system_info_t? retro_get_system_info;
    private retro_get_system_av_info_t? retro_get_system_av_info;
    private retro_set_environment_t? retro_set_environment;
    private retro_set_video_refresh_t? retro_set_video_refresh;
    private retro_set_audio_sample_t? retro_set_audio_sample;
    private retro_set_audio_sample_batch_t? retro_set_audio_sample_batch;
    private retro_set_input_poll_t? retro_set_input_poll;
    private retro_set_input_state_t? retro_set_input_state;
    private retro_load_game_t? retro_load_game;
    private retro_unload_game_t? retro_unload_game;
    private retro_run_t? retro_run;
    private retro_reset_t? retro_reset;
    private retro_serialize_size_t? retro_serialize_size;
    private retro_serialize_t? retro_serialize;
    private retro_unserialize_t? retro_unserialize;
    private retro_cheat_reset_t? retro_cheat_reset;
    private retro_cheat_set_t? retro_cheat_set;
    private retro_get_region_t? retro_get_region;
    private retro_get_memory_data_t? retro_get_memory_data;
    private retro_get_memory_size_t? retro_get_memory_size;

    public retro_video_refresh_callback? OnVideoRefresh;
    public retro_audio_sample_callback? OnAudioSample;
    public retro_audio_sample_batch_callback? OnAudioSampleBatch;
    public retro_input_poll_callback? OnInputPoll;
    public retro_input_state_callback? OnInputState;
    public retro_environment_callback? OnEnvironment;

    public retro_pixel_format PixelFormat { get; private set; }

    public LibretroCore()
    {
        // Empty constructor, load later
    }

    public void Load(string path)
    {
        if (handle != IntPtr.Zero) throw new InvalidOperationException("Already loaded");
        handle = NativeLibrary.Load(path);
        // Load all function pointers
        retro_init = Marshal.GetDelegateForFunctionPointer<retro_init_t>(GetProcAddress("retro_init"));
        retro_deinit = Marshal.GetDelegateForFunctionPointer<retro_deinit_t>(GetProcAddress("retro_deinit"));
        retro_api_version = Marshal.GetDelegateForFunctionPointer<retro_api_version_t>(GetProcAddress("retro_api_version"));
        retro_get_system_info = Marshal.GetDelegateForFunctionPointer<retro_get_system_info_t>(GetProcAddress("retro_get_system_info"));
        retro_get_system_av_info = Marshal.GetDelegateForFunctionPointer<retro_get_system_av_info_t>(GetProcAddress("retro_get_system_av_info"));
        retro_set_environment = Marshal.GetDelegateForFunctionPointer<retro_set_environment_t>(GetProcAddress("retro_set_environment"));
        retro_set_video_refresh = Marshal.GetDelegateForFunctionPointer<retro_set_video_refresh_t>(GetProcAddress("retro_set_video_refresh"));
        retro_set_audio_sample = Marshal.GetDelegateForFunctionPointer<retro_set_audio_sample_t>(GetProcAddress("retro_set_audio_sample"));
        retro_set_audio_sample_batch = Marshal.GetDelegateForFunctionPointer<retro_set_audio_sample_batch_t>(GetProcAddress("retro_set_audio_sample_batch"));
        retro_set_input_poll = Marshal.GetDelegateForFunctionPointer<retro_set_input_poll_t>(GetProcAddress("retro_set_input_poll"));
        retro_set_input_state = Marshal.GetDelegateForFunctionPointer<retro_set_input_state_t>(GetProcAddress("retro_set_input_state"));
        retro_load_game = Marshal.GetDelegateForFunctionPointer<retro_load_game_t>(GetProcAddress("retro_load_game"));
        retro_unload_game = Marshal.GetDelegateForFunctionPointer<retro_unload_game_t>(GetProcAddress("retro_unload_game"));
        retro_run = Marshal.GetDelegateForFunctionPointer<retro_run_t>(GetProcAddress("retro_run"));
        retro_reset = Marshal.GetDelegateForFunctionPointer<retro_reset_t>(GetProcAddress("retro_reset"));
        retro_serialize_size = Marshal.GetDelegateForFunctionPointer<retro_serialize_size_t>(GetProcAddress("retro_serialize_size"));
        retro_serialize = Marshal.GetDelegateForFunctionPointer<retro_serialize_t>(GetProcAddress("retro_serialize"));
        retro_unserialize = Marshal.GetDelegateForFunctionPointer<retro_unserialize_t>(GetProcAddress("retro_unserialize"));
        retro_cheat_reset = Marshal.GetDelegateForFunctionPointer<retro_cheat_reset_t>(GetProcAddress("retro_cheat_reset"));
        retro_cheat_set = Marshal.GetDelegateForFunctionPointer<retro_cheat_set_t>(GetProcAddress("retro_cheat_set"));
        retro_get_region = Marshal.GetDelegateForFunctionPointer<retro_get_region_t>(GetProcAddress("retro_get_region"));
        retro_get_memory_data = Marshal.GetDelegateForFunctionPointer<retro_get_memory_data_t>(GetProcAddress("retro_get_memory_data"));
        retro_get_memory_size = Marshal.GetDelegateForFunctionPointer<retro_get_memory_size_t>(GetProcAddress("retro_get_memory_size"));
        currentInstance = this;
    }

    private IntPtr GetProcAddress(string name)
    {
        if (!NativeLibrary.TryGetExport(handle, name, out IntPtr addr))
            throw new Exception($"Function {name} not found");
        return addr;
    }

    public void Init()
    {
        retro_init!();
    }

    public void Deinit()
    {
        retro_deinit!();
    }

    public uint ApiVersion() => retro_api_version!();

    public retro_system_info GetSystemInfo()
    {
        retro_system_info info = new();
        retro_get_system_info!((IntPtr)Unsafe.AsPointer(ref info));
        return info;
    }

    public retro_system_av_info GetSystemAvInfo()
    {
        retro_system_av_info info = new();
        retro_get_system_av_info!((IntPtr)Unsafe.AsPointer(ref info));
        return info;
    }

    public bool SetEnvironment(retro_environment_callback callback)
    {
        OnEnvironment = callback;
        return retro_set_environment!(Marshal.GetFunctionPointerForDelegate(EnvironmentCallback));
    }

    public void SetVideoRefresh(retro_video_refresh_callback callback)
    {
        OnVideoRefresh = callback;
        retro_set_video_refresh!(Marshal.GetFunctionPointerForDelegate(VideoRefreshCallback));
    }

    public void SetAudioSample(retro_audio_sample_callback callback)
    {
        OnAudioSample = callback;
        retro_set_audio_sample!(Marshal.GetFunctionPointerForDelegate(AudioSampleCallback));
    }

    public void SetAudioSampleBatch(retro_audio_sample_batch_callback callback)
    {
        OnAudioSampleBatch = callback;
        retro_set_audio_sample_batch!(Marshal.GetFunctionPointerForDelegate(AudioSampleBatchCallback));
    }

    public void SetInputPoll(retro_input_poll_callback callback)
    {
        OnInputPoll = callback;
        retro_set_input_poll!(Marshal.GetFunctionPointerForDelegate(InputPollCallback));
    }

    public void SetInputState(retro_input_state_callback callback)
    {
        OnInputState = callback;
        retro_set_input_state!(Marshal.GetFunctionPointerForDelegate(InputStateCallback));
    }

    public bool LoadGame(retro_game_info gameInfo)
    {
        return retro_load_game!((IntPtr)Unsafe.AsPointer(ref gameInfo));
    }

    public void UnloadGame()
    {
        retro_unload_game!();
    }

    public void Run()
    {
        retro_run!();
    }

    public void Reset()
    {
        retro_reset!();
    }

    public nuint SerializeSize() => retro_serialize_size!();

    public bool Serialize(IntPtr data, nuint size) => retro_serialize!(data, size);

    public bool Unserialize(IntPtr data, nuint size) => retro_unserialize!(data, size);

    public void CheatReset() => retro_cheat_reset!();

    public void CheatSet(uint index, bool enabled, string code) => retro_cheat_set!(index, enabled, Marshal.StringToHGlobalAnsi(code));

    public uint GetRegion() => retro_get_region!();

    public IntPtr GetMemoryData(uint id) => retro_get_memory_data!(id);

    public nuint GetMemorySize(uint id) => retro_get_memory_size!(id);

    public void Dispose()
    {
        if (handle != IntPtr.Zero)
        {
            NativeLibrary.Free(handle);
            handle = IntPtr.Zero;
        }
    }

    private static readonly retro_video_refresh_callback VideoRefreshCallback = VideoRefresh;
    private static readonly retro_audio_sample_callback AudioSampleCallback = AudioSample;
    private static readonly retro_audio_sample_batch_callback AudioSampleBatchCallback = AudioSampleBatch;
    private static readonly retro_input_poll_callback InputPollCallback = InputPoll;
    private static readonly retro_input_state_callback InputStateCallback = InputState;
    private static readonly retro_environment_callback EnvironmentCallback = Environment;

    private static void VideoRefresh(IntPtr data, uint width, uint height, nuint pitch)
    {
        currentInstance?.OnVideoRefresh?.Invoke(data, width, height, pitch);
    }

    private static void AudioSample(short left, short right)
    {
        currentInstance?.OnAudioSample?.Invoke(left, right);
    }

    private static nuint AudioSampleBatch(IntPtr data, nuint frames)
    {
        return currentInstance?.OnAudioSampleBatch?.Invoke(data, frames) ?? 0;
    }

    private static void InputPoll()
    {
        currentInstance?.OnInputPoll?.Invoke();
    }

    private static short InputState(uint port, uint device, uint index, uint id)
    {
        return currentInstance?.OnInputState?.Invoke(port, device, index, id) ?? 0;
    }

    private static bool Environment(uint cmd, IntPtr data)
    {
        if (cmd == (uint)retro_environment.RETRO_ENVIRONMENT_SET_PIXEL_FORMAT)
        {
            if (data != IntPtr.Zero && currentInstance != null)
            {
                currentInstance.PixelFormat = (retro_pixel_format)Marshal.ReadInt32(data);
            }
            return true;
        }
        // Handle other commands as needed
        return false;
    }
}