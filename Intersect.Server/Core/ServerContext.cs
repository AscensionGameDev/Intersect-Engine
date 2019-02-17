using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Intersect.Logging;
using JetBrains.Annotations;

namespace Intersect.Server.Core
{
    internal sealed class ServerContext : ApplicationContext<ServerContext>
    {
        public static bool IsRunningSafe
        {
            get
            {
                try
                {
                    return Instance.IsRunning && !Instance.IsShutdownRequested;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        #region Threads

        private Thread ThreadConsole { get; set; }
        private Thread ThreadLogic { get; set; }

        #endregion

        [NotNull]
        public CommandLineOptions StartupOptions { get; }

        [NotNull]
        public ServerConsole ServerConsole { get; }

        [NotNull]
        public ServerLogic ServerLogic { get; }

        public ServerContext([NotNull] CommandLineOptions startupOptions)
        {
            StartupOptions = startupOptions;

            ServerConsole = new ServerConsole();
            ServerLogic = new ServerLogic();
        }

        #region Threads

        protected override void InternalStart()
        {
            try
            {
                if (!StartupOptions.DoNotShowConsole)
                {
                    ThreadConsole = ServerConsole.Start();
                }

                ThreadLogic = ServerLogic.Start();
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                Dispose();
                throw;
            }
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerStatic.Shutdown();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}