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
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking.Helpers;
using Intersect.Server.WebApi;
using Open.Nat;
#if WEBSOCKETS
using Intersect.Server.Networking.Websockets;
#endif

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

        private void InternalStartNetworking()
        {
            if (!Network.Listen())
            {
                Log.Error("An error occurred while attempting to connect.");
            }

#if WEBSOCKETS
            WebSocketNetwork.Init(Options.ServerPort);
            Console.WriteLine(Strings.Intro.websocketstarted.ToString(Options.ServerPort));
#endif

            Console.WriteLine();

            if (!Options.UPnP || Options.NoPunchthrough)
            {
                return;
            }

            UpnP.ConnectNatDevice().Wait(5000);
#if WEBSOCKETS
            UpnP.OpenServerPort(Options.ServerPort, Protocol.Tcp).Wait(5000);
#endif
            UpnP.OpenServerPort(Options.ServerPort, Protocol.Udp).Wait(5000);

            if (Options.ApiEnabled)
            {
                UpnP.OpenServerPort(Options.ApiPort, Protocol.Tcp).Wait(5000);
            }

            Console.WriteLine();

            if (Options.ApiEnabled)
            {
                // TODO: API needs to be owned by the context, and rewritten to WebAPI2
                Console.WriteLine(Strings.Intro.api.ToString(Options.ApiPort));
                Globals.Api = new ServerApi(Options.ApiPort);
                Globals.Api.Start();
                Console.WriteLine();
                Bootstrapper.DeleteIfExists("Nancy.dll");
            }

            Bootstrapper.CheckNetwork();
        }

        #endregion

        #endregion

        #region Startup

        protected override void InternalStart()
        {
            try
            {
                InternalStartNetworking();

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
                #region CLEAN THIS UP
                // TODO: This may actually be fine here? Might want to move it into Bootstrapper though as "PrintShutdown()"
                Console.WriteLine();
                Console.WriteLine(Strings.Commands.exiting);
                Console.WriteLine();

                // Except this line, this line is fine.
                Network.Dispose();

                // TODO: This probably also needs to not be a global, but will require more work to clean up.
                LegacyDatabase.SavePlayerDatabase();
                LegacyDatabase.SaveGameDatabase();

                // TODO: This needs to not be a global. I'm also in the middle of rewriting the API anyway.
                Globals.Api?.Stop();

                #endregion

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
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}