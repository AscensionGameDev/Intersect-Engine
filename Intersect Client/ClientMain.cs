using System;
using System.Globalization;
using Intersect.Localization;
using Intersect_Client_MonoGame;

namespace Intersect_MonoGameDx
{
#if WINDOWS || LINUX
    /// <summary>
    ///     The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            using (var game = new IntersectGame())
            {
                try
                {
                    game.Run();
                }
                catch (PlatformNotSupportedException)
                {
                    System.Windows.Forms.MessageBox.Show(Strings.Get("errors", "openglerror"),
                        Strings.Get("errors", "notsupported"));
                }
            }
        }
    }
#endif
}