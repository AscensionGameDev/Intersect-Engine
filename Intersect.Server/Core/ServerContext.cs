using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Crypto;
using Intersect.Network.Crypto.Formats;
using Intersect.Server.Networking;
using Intersect.Server.Networking.Lidgren;
using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

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

        [NotNull]
        public ServerNetwork Network { get; }

        public ServerContext([NotNull] CommandLineOptions startupOptions)
        {
            StartupOptions = startupOptions;

            ServerConsole = new ServerConsole();
            ServerLogic = new ServerLogic();

            Network = CreateNetwork();
        }

        #region Network

        [NotNull]
        private ServerNetwork CreateNetwork()
        {
            ServerNetwork network;

            #region Apply CLI Options

            Options.ServerPort = StartupOptions.ValidPort(Options.ServerPort);

            #endregion

            #region Create Network

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Intersect.Server.private-intersect.bek"))
            {
                var rsaKey = EncryptionKey.FromStream<RsaKey>(stream);
                Debug.Assert(rsaKey != null, "rsaKey != null");
                network = new ServerNetwork(new NetworkConfiguration(Options.ServerPort), rsaKey.Parameters);
            }

            #endregion

            #region Configure Packet Handlers

            var packetHandler = new PacketHandler();
            network.Handlers[PacketCode.BinaryPacket] = packetHandler.HandlePacket;

            #endregion

            return network;
        }

        #region Listen

        internal void NetworkListen()
        {
            // TODO: Move this into InternalStart()
            if (!Network.Listen())
            {
                Log.Error("An error occurred while attempting to connect.");
            }
        }

        #endregion

        #endregion

        #region Startup

        protected override void InternalStart()
        {
            try
            {
                if (!StartupOptions.DoNotShowConsole)
                {
                    ThreadConsole = ServerConsole.Start();
                }

                ThreadLogic = ServerLogic.Start();

                // TODO: Move Network.Listen() to here from CreateNetwork()
                //if (!Network.Listen())
                //{
                //    Log.Error("An error occurred while attempting to connect.");
                //}
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

                if (ThreadConsole?.IsAlive ?? false)
                {
                    if (!ThreadConsole.Join(1000))
                    {
                        ThreadConsole.Abort();
                    }
                }

                if (ThreadLogic?.IsAlive ?? false)
                {
                    if (!ThreadLogic.Join(10000))
                    {
                        ThreadLogic.Abort();
                    }
                }

                Network.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}