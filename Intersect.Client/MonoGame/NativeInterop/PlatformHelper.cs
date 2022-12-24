using System.Runtime.InteropServices;

namespace Intersect.Client.MonoGame.NativeInterop
{
    public static class PlatformHelper
    {
        public static readonly Platform CurrentPlatform;

        static PlatformHelper()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                CurrentPlatform = Platform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                CurrentPlatform = Platform.MacOS;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                CurrentPlatform = Platform.Linux;
            }
            else
            {
                CurrentPlatform = Platform.Unknown;
            }
        }
    }
}
