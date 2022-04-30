using System.Runtime.InteropServices;

namespace Intersect.Editor.Platform;

public class RuntimeHelper
{
    public static Runtime Runtime { get; }

    private static RuntimeLoader? Loader { get; }

    public static IntPtr LoadNativeLibrary(string libraryName)
    {
        if (Loader == default)
        {
            throw new InvalidOperationException();
        }

        var runtimeIdentifier = Runtime.GetIdentifier();
        var runtimeDirectory = Path.Combine("runtimes", runtimeIdentifier);
        var nativeDirectory = Path.Combine(runtimeDirectory, "native");
        var libraryPath = Path.Combine(nativeDirectory, libraryName);
        return Loader.LoadLibrary(libraryPath);
    }

    private const int RTLD_LAZY = 0x0001;

    private interface RuntimeLoader
    {
        IntPtr LoadLibrary(string libraryName);
    }

    private class Windows : RuntimeLoader
    {
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibraryW(string lpszLib);

        public IntPtr LoadLibrary(string libraryName) => LoadLibraryW(libraryName);
    }

    private class Linux : RuntimeLoader
    {
        [DllImport("libdl.so.2")]
        public static extern IntPtr dlopen(string path, int flags);

        [DllImport("libdl.so.2")]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport("libc")]
        public static extern int uname(IntPtr buf);

        public IntPtr LoadLibrary(string libraryName) => dlopen(libraryName, RTLD_LAZY);
    }

    private class Osx : RuntimeLoader
    {
        [DllImport("/usr/lib/libSystem.dylib")]
        public static extern IntPtr dlopen(string path, int flags);

        [DllImport("/usr/lib/libSystem.dylib")]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

        public IntPtr LoadLibrary(string libraryName) => dlopen(libraryName, RTLD_LAZY);
    }

    static RuntimeHelper()
    {
        var platformId = Environment.OSVersion.Platform;
        switch (platformId)
        {
            case PlatformID.Win32NT:
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.WinCE:
                Runtime = Environment.Is64BitOperatingSystem ? Runtime.Windows64 : Runtime.Windows86;
                Loader = new Windows();
                break;

            case PlatformID.MacOSX:
                Runtime = Runtime.Osx64;
                Loader = new Osx();
                break;

            case PlatformID.Unix:
                IntPtr buffer = default;

                try
                {
                    buffer = Marshal.AllocHGlobal(8192);
                    if (Linux.uname(buffer) == 0 && Marshal.PtrToStringAnsi(buffer) == "Linux")
                    {
                        Runtime = Runtime.Linux64;
                        Loader = new Linux();
                        break;
                    }
                }
                catch
                {
                }
                finally
                {
                    if (buffer != default)
                    {
                        Marshal.FreeHGlobal(buffer);
                    }
                }

                Runtime = Runtime.Unknown;
                break;

            default:
                Runtime = Runtime.Unknown;
                break;
        }
    }
}
