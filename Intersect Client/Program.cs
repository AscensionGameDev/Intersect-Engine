using System;
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
                game.Run();
        }
    }
#endif
}
