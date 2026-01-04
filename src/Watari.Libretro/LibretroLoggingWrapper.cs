using System.Runtime.InteropServices;
using System.IO;
using Watari.Libretro.Bindings;

namespace Watari.Libretro;

public class LibretroLoggingWrapper : IDisposable
{
    private IntPtr loggingHandle;
    private IntPtr logFunc;
    private log_callback_t callbackDelegate;

    private delegate void log_callback_t(int level, string message);
    private delegate void set_log_callback_delegate(log_callback_t callback);

    public IntPtr LogFunctionPointer => logFunc;
    public event Action<retro_log_level, string>? Log;

    public LibretroLoggingWrapper()
    {
        string libPath = Path.Combine(Directory.GetCurrentDirectory(), "NativeLogging", "libLoggingWrapper.dylib");
        loggingHandle = NativeLibrary.Load(libPath);
        logFunc = NativeLibrary.GetExport(loggingHandle, "retro_log_printf");
        IntPtr setCallbackFunc = NativeLibrary.GetExport(loggingHandle, "set_log_callback");
        var setCallback = Marshal.GetDelegateForFunctionPointer<set_log_callback_delegate>(setCallbackFunc);
        callbackDelegate = InstanceLogFormatted;
        setCallback(callbackDelegate);
    }

    private void InstanceLogFormatted(int level, string message)
    {
        Log?.Invoke((retro_log_level)level, message);
    }

    public void Dispose()
    {
        if (loggingHandle != IntPtr.Zero)
        {
            NativeLibrary.Free(loggingHandle);
            loggingHandle = IntPtr.Zero;
        }
    }
}