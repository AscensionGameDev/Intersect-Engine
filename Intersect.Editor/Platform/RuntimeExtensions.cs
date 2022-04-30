namespace Intersect.Editor.Platform;

public static class RuntimeExtensions
{
    public static string GetIdentifier(this Runtime runtime)
    {
        switch (runtime)
        {
            case Runtime.Android:
                return RuntimeIdentifiers.Android;

            case Runtime.Browser:
                return RuntimeIdentifiers.Browser;

            case Runtime.BrowserWasm:
                return RuntimeIdentifiers.BrowserWasm;

            case Runtime.Ios:
                return RuntimeIdentifiers.Ios;

            case Runtime.Linux64:
                return RuntimeIdentifiers.Linux64;

            case Runtime.Osx64:
                return RuntimeIdentifiers.Osx64;

            case Runtime.OsxArm64:
                return RuntimeIdentifiers.OsxArm64;

            case Runtime.Windows64:
                return RuntimeIdentifiers.Windows64;

            case Runtime.Windows86:
                return RuntimeIdentifiers.Windows86;

            case Runtime.Unknown:
                throw new NotSupportedException(runtime.ToString());

            default:
                throw new ArgumentOutOfRangeException(nameof(runtime));
        }
    }
}
