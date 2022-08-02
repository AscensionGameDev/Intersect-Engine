using System.Diagnostics;


namespace Intersect.Platform;

public static partial class WebBrowser
{
    public static void OpenInDefaultBrowser(string uri)
    {
        var processStartInfo = new ProcessStartInfo(uri)
        {
            UseShellExecute = true,
        };
        _ = Process.Start(processStartInfo);
    }
}
