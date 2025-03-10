using System.Runtime.InteropServices;

namespace Intersect.Framework.SystemInformation;

public static class PlatformInformation
{
    private static string? _runtimeIdentifier;

    public static string RuntimeIdentifier
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_runtimeIdentifier))
            {
                return _runtimeIdentifier;
            }

            var processArchitecture = RuntimeInformation.ProcessArchitecture;
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            var suffix = processArchitecture switch
            {
                Architecture.X86 => "x86",
                Architecture.X64 => "x64",
                Architecture.Arm => "arm",
                Architecture.Arm64 => "arm64",
                Architecture.Wasm => "wasm",
                _ => throw new PlatformNotSupportedException(processArchitecture.ToString()),
            };

            if (OperatingSystem.IsLinux())
            {
                _runtimeIdentifier = $"linux-{suffix}";
            }
            else if (OperatingSystem.IsWindows())
            {
                _runtimeIdentifier = $"win-{suffix}";
            }
            else if (OperatingSystem.IsMacOS())
            {
                _runtimeIdentifier = $"osx-{suffix}";
            }
            else if (OperatingSystem.IsAndroid())
            {
                _runtimeIdentifier = $"android-{suffix}";
            }
            else if (OperatingSystem.IsIOS())
            {
                _runtimeIdentifier = $"ios-{suffix}";
            }
            else if (OperatingSystem.IsBrowser())
            {
                _runtimeIdentifier = $"browser-{suffix}";
            }
            else
            {
                throw new PlatformNotSupportedException(RuntimeInformation.RuntimeIdentifier);
            }

            return _runtimeIdentifier;
        }
    }
}