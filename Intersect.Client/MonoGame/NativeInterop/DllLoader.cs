using System;
using System.Runtime.InteropServices;

namespace Intersect.Client.MonoGame.NativeInterop
{
    public abstract class DllLoader
    {
        public static readonly DllLoader PlatformLoader;

        static DllLoader()
        {
            switch (PlatformHelper.CurrentPlatform)
            {
                case Platform.Unknown:
                    PlatformLoader = new UnixDllLoader();
                    break;

                case Platform.Linux:
                    PlatformLoader = new LinuxDllLoader();
                    break;

                case Platform.MacOS:
                    PlatformLoader = new MacDllLoader();
                    break;

                case Platform.Windows:
                    PlatformLoader = new WindowsDllLoader();
                    break;

                default: throw new IndexOutOfRangeException();
            }
        }

        public abstract IntPtr LoadLibrary(string libraryName);

        protected abstract IntPtr LoadFunctionPointer(IntPtr libraryHandle, string functionName);

        public virtual TDelegate LoadFunction<TDelegate>(IntPtr libraryHandle, string functionName)
        {
            var functionPointer = LoadFunctionPointer(libraryHandle, functionName);
            return functionPointer == default
                ? default
                : Marshal.GetDelegateForFunctionPointer<TDelegate>(functionPointer);
        }

        private sealed class WindowsDllLoader : DllLoader
        {
            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
            private static extern IntPtr LoadLibraryW(string lpszLib);

            public override IntPtr LoadLibrary(string libraryName)
            {
                return LoadLibraryW(libraryName);
            }

            protected override IntPtr LoadFunctionPointer(IntPtr libraryHandle, string functionName)
            {
                return GetProcAddress(libraryHandle, functionName);
            }
        }

        private sealed class MacDllLoader : UnixDllLoader
        {
            public MacDllLoader() : base(dlopen, dlsym) { }

            [DllImport("/usr/lib/libSystem.dylib")]
            private static extern IntPtr dlopen(string path, int flags);

            [DllImport("/usr/lib/libSystem.dylib")]
            private static extern IntPtr dlsym(IntPtr handle, string symbol);
        }

        private sealed class LinuxDllLoader : UnixDllLoader
        {
            public LinuxDllLoader() : base(dlopen, dlsym) { }

            [DllImport("libdl.so.2")]
            private static extern IntPtr dlopen(string path, int flags);

            [DllImport("libdl.so.2")]
            private static extern IntPtr dlsym(IntPtr handle, string symbol);
        }

        private class UnixDllLoader : DllLoader
        {
            protected const int RTLD_LAZY = 0x0001;

            private readonly dlopen_d _dlopen;
            private readonly dlsym_d _dlsym;

            protected UnixDllLoader(dlopen_d dlopen, dlsym_d dlsym)
            {
                _dlopen = dlopen;
                _dlsym = dlsym;
            }

            public UnixDllLoader() : this(dlopen, dlsym) { }

            [DllImport("libdl")]
            private static extern IntPtr dlopen(string path, int flags);

            [DllImport("libdl")]
            private static extern IntPtr dlsym(IntPtr handle, string symbol);

            public override IntPtr LoadLibrary(string libraryName)
            {
                return _dlopen(libraryName, RTLD_LAZY);
            }

            protected override IntPtr LoadFunctionPointer(IntPtr libraryHandle, string functionName)
            {
                return _dlsym(libraryHandle, functionName);
            }

            protected delegate IntPtr dlopen_d(string path, int flags);

            protected delegate IntPtr dlsym_d(IntPtr handle, string symbol);
        }
    }
}
