using Intersect.Core;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Crypto;
using Intersect.Crypto.Formats;
using Intersect.Server.Core.Services;
using Intersect.Server.Database;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Networking.Helpers;
using Intersect.Server.Networking.Lidgren;
using Intersect.Server.Web.RestApi;

using Open.Nat;

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Intersect.Factories;
using Intersect.Plugins;
using Intersect.Server.Plugins;
using Intersect.Server.General;
using Intersect.Logging.Output;
using System.Collections.Immutable;
using System.Collections.Generic;
using Intersect.Plugins.Interfaces;

#if WEBSOCKETS
using Intersect.Server.Networking.Websockets;
#endif

namespace Intersect.Server.Core
{
    /// <summary>
    /// Implements <see cref="IServerContext"/>.
    /// </summary>
    internal sealed class ServerContext : ApplicationContext<ServerContext, ServerCommandLineOptions>, IServerContext
    {
        internal ServerContext(ServerCommandLineOptions startupOptions, Logger logger, INetworkHelper networkHelper) : base(
            startupOptions, logger, networkHelper
        )
        {
            // Register the factory for creating service plugin contexts
            FactoryRegistry<IPluginContext>.RegisterFactory(new ServerPluginContext.Factory());

            if (startupOptions.Port > 0)
            {
                Options.ServerPort = startupOptions.Port;
            }

            RestApi = new RestApi(startupOptions.ApiPort);

            Network = CreateNetwork(networkHelper);
        }

        public IConsoleService ConsoleService => GetExpectedService<IConsoleService>();

        public ILogicService LogicService => GetExpectedService<ILogicService>();

        public ServerNetwork Network { get; }

        public RestApi RestApi { get; }

        #region Startup

        protected override void InternalStart()
        {
            try
            {
                InternalStartNetworking();
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
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (disposing)
            {
                #region CLEAN THIS UP

                // TODO: This may actually be fine here? Might want to move it into Bootstrapper though as "PrintShutdown()"
                Console.WriteLine();
                Console.WriteLine(Strings.Commands.exiting);
                Console.WriteLine();

#if WEBSOCKETS
                Log.Info("Shutting down websockets..." + $" ({stopwatch.ElapsedMilliseconds}ms)");
                WebSocketNetwork.Stop();
#endif

                // Except this line, this line is fine.
                Log.Info("Disposing network..." + $" ({stopwatch.ElapsedMilliseconds}ms)");
                Network.Dispose();

                Log.Info("Saving updated server variable values");
                DbInterface.SaveUpdatedServerVariables();

                // TODO: This probably also needs to not be a global, but will require more work to clean up.
                Log.Info("Saving online users/players..." + $" ({stopwatch.ElapsedMilliseconds}ms)");

                var savingTasks = new List<Task>();
                foreach (var user in Database.PlayerData.User.OnlineList.ToArray())
                {
                    savingTasks.Add(Task.Run(() => user.Save()));
                }

                Task.WaitAll(savingTasks.ToArray());

                // TODO: This probably also needs to not be a global, but will require more work to clean up.
                Log.Info("Online users/players saved." + $" ({stopwatch.ElapsedMilliseconds}ms)");


                //Disconnect All Clients
                //Will kill their packet handling threads so we have a clean shutdown
                lock (Globals.ClientLock)
                {
                    var clients = Globals.Clients.ToArray();
                    foreach (var client in clients)
                    {
                        client.Disconnect("Server Shutdown", true);
                    }
                }


                // TODO: This needs to not be a global. I'm also in the middle of rewriting the API anyway.
                Log.Info("Shutting down the API..." + $" ({stopwatch.ElapsedMilliseconds}ms)");
                RestApi.Dispose();

                #endregion

                if (ThreadConsole?.IsAlive ?? false)
                {
                    Log.Info("Shutting down the console thread..." + $" ({stopwatch.ElapsedMilliseconds}ms)");
                    if (!ThreadConsole.Join(1000))
                    {
                        try
                        {
                            ThreadConsole.Abort();
                        }
                        catch (ThreadAbortException threadAbortException)
                        {
                            Log.Error(threadAbortException, $"{nameof(ThreadConsole)} aborted.");
                        }
                    }
                }

                if (ThreadLogic?.IsAlive ?? false)
                {
                    Log.Info("Shutting down the logic thread..." + $" ({stopwatch.ElapsedMilliseconds}ms)");
                    if (!ThreadLogic.Join(10000))
                    {
                        try
                        {
                            ThreadLogic.Abort();
                        }
                        catch (ThreadAbortException threadAbortException)
                        {
                            Log.Error(threadAbortException, $"{nameof(ThreadLogic)} aborted.");
                        }
                    }
                }

                NetworkHelper.HandlerRegistry.Dispose();
            }

            Log.Info("Base dispose." + $" ({stopwatch.ElapsedMilliseconds}ms)");
            base.Dispose(disposing);
            Log.Info("Finished disposing server context." + $" ({stopwatch.ElapsedMilliseconds}ms)");
            Console.WriteLine(Strings.Commands.exited);
            System.Environment.Exit(-1);
        }

        #endregion

        #region Threads

        private Thread ThreadConsole => ConsoleService.Thread;

        private Thread ThreadLogic => LogicService.Thread;

        #endregion

        #region Network

        private ServerNetwork CreateNetwork(INetworkHelper networkHelper)
        {
            ServerNetwork network;

            #region Apply CLI Options

            //Options.ServerPort = StartupOptions.ValidPort(Options.ServerPort);

            #endregion

            #region Create Network

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Intersect.Server.network.handshake.bkey"))
            {
                var rsaKey = EncryptionKey.FromStream<RsaKey>(stream ?? throw new InvalidOperationException());
                Debug.Assert(rsaKey != null, "rsaKey != null");
                network = new ServerNetwork(this, networkHelper, new NetworkConfiguration(Options.ServerPort), rsaKey.Parameters);
            }

            #endregion

            #region Configure Packet Handlers

            var packetHandler = new PacketHandler(this, networkHelper.HandlerRegistry);
            network.Handler = packetHandler.HandlePacket;
            network.PreProcessHandler = packetHandler.PreProcessPacket;

            #endregion

            return network;
        }

        #region Listen

        private void InternalStartNetworking()
        {
            Console.WriteLine();

            if (!Network.Listen())
            {
                Log.Error("An error occurred while attempting to connect.");
            }
            else
            {
                Console.WriteLine(Strings.Intro.started.ToString(Options.ServerPort));
            }

#if WEBSOCKETS
            WebSocketNetwork.Init(Options.ServerPort);
            Log.Pretty.Info(Strings.Intro.websocketstarted.ToString(Options.ServerPort));
            Console.WriteLine();
#endif

            RestApi.Start();

            if (!Options.UPnP || Instance.StartupOptions.NoNatPunchthrough)
            {
                return;
            }

            Console.WriteLine();

            UpnP.ConnectNatDevice().Wait(5000);
#if WEBSOCKETS
            UpnP.OpenServerPort(Options.ServerPort, Protocol.Tcp).Wait(5000);
#endif
            UpnP.OpenServerPort(Options.ServerPort, Protocol.Udp).Wait(5000);

            if (RestApi.IsStarted)
            {
                RestApi.Configuration.Ports.ToList()
                    .ForEach(port => UpnP.OpenServerPort(port, Protocol.Tcp).Wait(5000));
            }

            Console.WriteLine();

            Bootstrapper.CheckNetwork();

            Console.WriteLine();
        }

        #endregion

        #endregion

        #region Exception Handling

        protected override void NotifyNonTerminatingExceptionOccurred() =>
            Console.WriteLine(Strings.Errors.errorlogged);

        internal static void DispatchUnhandledException(Exception exception, bool isTerminating = true)
        {
            var sender = Thread.CurrentThread;
            Task.Factory.StartNew(
                () => HandleUnhandledException(sender, new UnhandledExceptionEventArgs(exception, isTerminating))
            );
        }

        #endregion Exception Handling
    }
}
