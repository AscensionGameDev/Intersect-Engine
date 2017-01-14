using System;
using System.Media;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client_MonoGame;
using Intersect_Library.Localization;

namespace Intersect_MonoGameDx
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Intersect())
            {
                try
                {
                    game.Run();
                }
                catch (System.PlatformNotSupportedException)
                {
                    System.Windows.Forms.MessageBox.Show(Strings.Get("errors","openglerror"), Strings.Get("errors", "notsupported"));
                }
            }
                
        }
    }
#endif
}
