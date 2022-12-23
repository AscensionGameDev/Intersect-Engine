using System;

namespace Intersect.Client.MonoGame.NativeInterop
{
    public sealed class FunctionLoader
    {
        private readonly IntPtr _libraryHandle;

        private FunctionLoader(IntPtr libraryHandle)
        {
            _libraryHandle = libraryHandle;
        }

        public FunctionLoader(string name) : this(DllLoader.PlatformLoader.LoadLibrary(name))
        {
        }

        public FunctionLoader(KnownLibrary knownLibrary) : this(ResolveKnownLibrary(knownLibrary))
        {
        }

        public TDelegate LoadFunction<TDelegate>(string functionName)
        {
            return DllLoader.PlatformLoader.LoadFunction<TDelegate>(_libraryHandle, functionName);
        }

        private static string ResolveKnownLibrary(KnownLibrary knownLibrary)
        {
            switch (knownLibrary)
            {
                case KnownLibrary.OpenAL:
                    switch (PlatformHelper.CurrentPlatform)
                    {
                        /* Fall-back to Linux if the platform is unknown */
                        case Platform.Unknown:
                        case Platform.Linux:
                            return "libopenal.so.1";

                        case Platform.MacOS: return "libopenal.1.dylib";
                        case Platform.Windows: return "soft_oal.dll";
                        default: throw new IndexOutOfRangeException();
                    }

                case KnownLibrary.SDL2:
                    switch (PlatformHelper.CurrentPlatform)
                    {
                        /* Fall-back to Linux if the platform is unknown */
                        case Platform.Unknown:
                        case Platform.Linux:
                            return "libSDL2-2.0.so.0";

                        case Platform.MacOS: return "libSDL2-2.0.0.dylib";
                        case Platform.Windows: return "SDL2.dll";
                        default: throw new IndexOutOfRangeException();
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(knownLibrary), knownLibrary, null);
            }
        }
    }
}
