using System;
using System.Media;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client_MonoGame;

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
                    System.Windows.Forms.MessageBox.Show(
                        "This platform is not supported. Intersect requires OpenGL 3.0 compatible drivers, or either ARB_framebuffer_object or EXT_framebuffer_object extensions. Upgrading your graphic drivers may resolve this problem.","Not Supported");
                }
            }
                
        }
    }
#endif
}
