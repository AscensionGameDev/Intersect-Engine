using System.Diagnostics;

namespace Intersect.Utilities;

public static class BrowserUtils
{
    public static void Open(string url)
    {
        Process.Start(
            new ProcessStartInfo(url)
            {
                UseShellExecute = true,
            }
        );
    }

    public static void Open(Uri uri)
    {
        Open(uri.AbsoluteUri);
    }
}