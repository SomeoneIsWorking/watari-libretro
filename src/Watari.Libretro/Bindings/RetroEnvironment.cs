namespace Watari.Libretro.Bindings;

using System.Reflection;

public static class RetroEnvironment
{
    /// <summary>
    /// This bit indicates that the associated environment call is experimental,
    /// and may be changed or removed in the future.
    /// Frontends should mask out this bit before handling the environment call.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_EXPERIMENTAL = 0x10000;

    /// <summary>
    /// Requests the frontend to set the screen rotation.
    /// Valid values are 0, 1, 2, and 3.
    /// These numbers respectively set the screen rotation to 0, 90, 180, and 270 degrees counter-clockwise.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_ROTATION = 1;

    /// <summary>
    /// Queries whether the core should use overscan or not.
    /// Set to true if the core should use overscan, false if it should be cropped away.
    /// Deprecated in favor of core options.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_OVERSCAN = 2;

    /// <summary>
    /// Queries whether the frontend supports frame duping.
    /// Set to true if the frontend supports frame duping.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_CAN_DUPE = 3;

    /// <summary>
    /// Displays a user-facing message for a short time.
    /// Deprecated, prefer RETRO_ENVIRONMENT_SET_MESSAGE_EXT.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_MESSAGE = 6;

    /// <summary>
    /// Requests the frontend to shutdown the core.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SHUTDOWN = 7;

    /// <summary>
    /// Gives a hint to the frontend of how demanding this core is on the system.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_PERFORMANCE_LEVEL = 8;

    /// <summary>
    /// Returns the path to the frontend's system directory.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY = 9;

    /// <summary>
    /// Sets the internal pixel format used by the frontend for rendering.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_PIXEL_FORMAT = 10;

    /// <summary>
    /// Sets an array of input descriptors for the frontend.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_INPUT_DESCRIPTORS = 11;

    /// <summary>
    /// Sets a callback function used to notify the core about keyboard events.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_KEYBOARD_CALLBACK = 12;

    /// <summary>
    /// Sets an interface that the frontend can use to insert and remove disks.
    /// Deprecated, prefer RETRO_ENVIRONMENT_SET_DISK_CONTROL_EXT_INTERFACE.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_DISK_CONTROL_INTERFACE = 13;

    /// <summary>
    /// Requests that a frontend enable a particular hardware rendering API.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_HW_RENDER = 14;

    /// <summary>
    /// Retrieves a core option's value from the frontend.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_VARIABLE = 15;

    /// <summary>
    /// Notifies the frontend of the core's available options.
    /// Deprecated, prefer RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_VARIABLES = 16;

    /// <summary>
    /// Queries whether at least one core option was updated by the frontend.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_VARIABLE_UPDATE = 17;

    /// <summary>
    /// Notifies the frontend that this core can run without loading any content.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_SUPPORT_NO_GAME = 18;

    /// <summary>
    /// Retrieves the absolute path from which this core was loaded.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_LIBRETRO_PATH = 19;

    /// <summary>
    /// Sets a callback that notifies the core of how much time has passed.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_FRAME_TIME_CALLBACK = 21;

    /// <summary>
    /// Registers a set of functions that the frontend can use for audio output.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_AUDIO_CALLBACK = 22;

    /// <summary>
    /// Gets an interface that a core can use to access a controller's rumble motors.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_RUMBLE_INTERFACE = 23;

    /// <summary>
    /// Returns the frontend's supported input device types.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_INPUT_DEVICE_CAPABILITIES = 24;

    /// <summary>
    /// Gets an interface that the core can use to access device sensors.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_SENSOR_INTERFACE = 25 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Gets an interface that the core can use to access the device's video camera.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_CAMERA_INTERFACE = 26 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Gets an interface that the frontend can use for cross-platform logging.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_LOG_INTERFACE = 27;

    /// <summary>
    /// Gets an interface that the core can use for profiling code.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_PERF_INTERFACE = 28;

    /// <summary>
    /// Gets an interface that the core can use to retrieve the device's location.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_LOCATION_INTERFACE = 29;

    /// <summary>
    /// Returns the frontend's "core assets" directory.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_CORE_ASSETS_DIRECTORY = 30;

    /// <summary>
    /// Returns the frontend's save data directory.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY = 31;

    /// <summary>
    /// Sets new video and audio parameters for the core.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_SYSTEM_AV_INFO = 32;

    /// <summary>
    /// Provides an interface that the frontend can use to get function pointers from the core.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_PROC_ADDRESS_CALLBACK = 33;

    /// <summary>
    /// Registers a core's ability to handle "subsystems".
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_SUBSYSTEM_INFO = 34;

    /// <summary>
    /// Declares one or more types of controllers supported by this core.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_CONTROLLER_INFO = 35;

    /// <summary>
    /// Notifies the frontend of the address spaces used by the core's emulated hardware.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_MEMORY_MAPS = 36 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Resizes the viewport without reinitializing the video driver.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_GEOMETRY = 37;

    /// <summary>
    /// Returns the name of the user, if possible.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_USERNAME = 38;

    /// <summary>
    /// Returns the frontend's configured language.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_LANGUAGE = 39;

    /// <summary>
    /// Returns a frontend-managed framebuffer that the core may render directly into.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_CURRENT_SOFTWARE_FRAMEBUFFER = 40 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Returns an interface for accessing the data of specific rendering APIs.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_HW_RENDER_INTERFACE = 41 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Explicitly notifies the frontend of whether this core supports achievements.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_SUPPORT_ACHIEVEMENTS = 42 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Defines an interface that the frontend can use to ask the core for parameters it needs for a hardware rendering context.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE = 43 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Notifies the frontend about quirks associated with serialization.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_SERIALIZATION_QUIRKS = 44;

    /// <summary>
    /// The frontend will try to use a "shared" context when setting up a hardware context.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_HW_SHARED_CONTEXT = 45 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Returns an interface that the core can use to access the file system.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_VFS_INTERFACE = 46 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Returns an interface that the core can use to set the state of any accessible device LEDs.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_LED_INTERFACE = 47 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Returns hints about certain steps that the core may skip for this frame.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_AUDIO_VIDEO_ENABLE = 48 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Returns an interface that the core can use for raw MIDI I/O.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_MIDI_INTERFACE = 49 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Asks the frontend if it's currently in fast-forward mode.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_FASTFORWARDING = 50 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Returns the refresh rate the frontend is targeting, in Hz.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_TARGET_REFRESH_RATE = 51 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Returns whether the frontend can return the state of all buttons at once as a bitmask.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_INPUT_BITMASKS = 52 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Returns the version of the core options API supported by the frontend.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION = 52;

    /// <summary>
    /// Defines an interface that the frontend can use to set the state of core options.
    /// Deprecated, prefer RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_CORE_OPTIONS = 53;

    /// <summary>
    /// A variant of RETRO_ENVIRONMENT_SET_CORE_OPTIONS that supports internationalization.
    /// Deprecated, prefer RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2_INTL.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_CORE_OPTIONS_INTL = 54;

    /// <summary>
    /// Notifies the frontend that it should show or hide the named core option.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_CORE_OPTIONS_DISPLAY = 55;

    /// <summary>
    /// Registers a callback that the frontend can use to notify the core of audio buffer occupancy.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_AUDIO_BUFFER_STATUS_CALLBACK = 62;

    /// <summary>
    /// Requests a minimum frontend audio latency in milliseconds.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_MINIMUM_AUDIO_LATENCY = 63;

    /// <summary>
    /// Allows the core to tell the frontend when it should enable fast-forwarding.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_FASTFORWARDING_OVERRIDE = 64 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Allows an implementation to override 'global' content info parameters.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_CONTENT_INFO_OVERRIDE = 65;

    /// <summary>
    /// Allows an implementation to fetch extended game information.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_GAME_INFO_EXT = 66;

    /// <summary>
    /// Defines a set of core options that can be shown and configured by the frontend.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2 = 67;

    /// <summary>
    /// A variant of RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2 that supports internationalization.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2_INTL = 68;

    /// <summary>
    /// Registers a callback for the frontend to use when setting the visibility of core options.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_CORE_OPTIONS_UPDATE_DISPLAY_CALLBACK = 69;

    /// <summary>
    /// Forcibly sets a core option's value.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_VARIABLE = 70;

    /// <summary>
    /// Allows an implementation to get details on the actual rate the frontend is attempting to call retro_run().
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_THROTTLE_STATE = 71 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Allows an implementation to get details on the savestate context.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_SAVESTATE_CONTEXT = 72 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Queries which interface is supported for hardware render context negotiation.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE_SUPPORT = 73 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Asks the frontend whether JIT compilation can be used.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_JIT_CAPABLE = 74;

    /// <summary>
    /// Returns an interface that the core can use to receive microphone input.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_MICROPHONE_INTERFACE = 75 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Returns the device's current power state as reported by the frontend.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_DEVICE_POWER = 77 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Returns the "playlist" directory of the frontend.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_PLAYLIST_DIRECTORY = 79;

    /// <summary>
    /// Returns the "file browser" start directory of the frontend.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_FILE_BROWSER_START_DIRECTORY = 80;

    /// <summary>
    /// Returns the audio sample rate the frontend is targeting, in Hz.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_TARGET_SAMPLE_RATE = 81 | RETRO_ENVIRONMENT_EXPERIMENTAL;

    /// <summary>
    /// Returns the version of the message interface supported by the frontend.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_MESSAGE_INTERFACE_VERSION = 59;

    /// <summary>
    /// Displays a user-facing message for a short time.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_MESSAGE_EXT = 60;

    /// <summary>
    /// Returns the number of active input devices currently provided by the frontend.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_GET_INPUT_MAX_USERS = 61;

    /// <summary>
    /// Returns an interface for accessing network packets.
    /// </summary>
    public const uint RETRO_ENVIRONMENT_SET_NETPACKET_INTERFACE = 78;

    public static string GetEnvironmentCommandName(uint cmd)
    {
        var fields = typeof(RetroEnvironment).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            if (field.FieldType != typeof(uint))
            {
                continue;
            }
            uint fieldValue = (uint)field.GetValue(null)!;
            uint maskedValue = fieldValue & ~RETRO_ENVIRONMENT_EXPERIMENTAL;
            if (maskedValue == cmd)
            {
                string name = field.Name;
                if ((fieldValue & RETRO_ENVIRONMENT_EXPERIMENTAL) != 0)
                {
                    name += " (experimental)";
                }
                return name;
            }
        }
        return $"Unknown ({cmd})";
    }
}