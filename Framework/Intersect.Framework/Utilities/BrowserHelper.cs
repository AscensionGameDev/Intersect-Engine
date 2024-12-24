using System.Diagnostics;
// ReSharper disable MemberCanBePrivate.Global

namespace Intersect.Framework.Utilities;

public static class BrowserHelper
{
    public static void Open(string url)
    {
        Process.Start(new ProcessStartInfo(url)
        {
            UseShellExecute = true,
        });
    }

    public static void Open(Uri uri) => Open(uri.AbsoluteUri);
}
