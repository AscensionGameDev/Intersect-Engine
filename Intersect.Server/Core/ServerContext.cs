﻿using Intersect.Core;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Server.Core.Services;
using Intersect.Server.Database;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Networking.Lidgren;

using System.Diagnostics;
using System.Reflection;

using Intersect.Factories;
using Intersect.Plugins;
using Intersect.Server.Plugins;
using Intersect.Server.General;
using Intersect.Plugins.Interfaces;
using Intersect.Rsa;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Web;

#if WEBSOCKETS
using Intersect.Server.Networking.Websockets;
#endif

namespace Intersect.Server.Core
{
    /// <summary>
    /// Implements <see cref="IServerContext"/>.
    /// </summary>
    internal sealed partial class ServerContext : ApplicationContext<ServerContext, ServerCommandLineOptions>, IServerContext
    {
        internal ServerContext(ServerCommandLineOptions startupOptions, Logger logger, IPacketHelper packetHelper) : base(
            startupOptions, logger, packetHelper
        )
        {
            // Register the factory for creating service plugin contexts
            FactoryRegistry<IPluginContext>.RegisterFactory(new ServerPluginContext.Factory());

            if (startupOptions.Port > 0)
            {
                Options.ServerPort = startupOptions.Port;
            }

            Network = CreateNetwork(packetHelper);
        }

        public IApiService ApiService => GetExpectedService<IApiService>();

        public IConsoleService ConsoleService => GetExpectedService<IConsoleService>();

        public ILogicService LogicService => GetExpectedService<ILogicService>();

        public ServerNetwork Network { get; }

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


                Log.Info("Saving loaded guilds....");

                savingTasks.Clear();
                //Should we send out guild updates?
                foreach (var guild in Guild.Guilds)
                {
                    savingTasks.Add(Task.Run(() => guild.Value.Save()));
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

                PacketHelper.HandlerRegistry.Dispose();
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

        private ServerNetwork CreateNetwork(IPacketHelper packetHelper)
        {
            ServerNetwork network;

            #region Apply CLI Options

            //Options.ServerPort = StartupOptions.ValidPort(Options.ServerPort);

            #endregion

            #region Create Network

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Intersect.Server.network.handshake.bkey"))
            {
                var rsaKey = new RsaKey(stream ?? throw new InvalidOperationException());
                Debug.Assert(rsaKey != null, "rsaKey != null");
                network = new ServerNetwork(this, packetHelper, new NetworkConfiguration(Options.ServerPort), rsaKey.Parameters);
            }

            #endregion

            #region Configure Packet Handlers

            var packetHandler = new PacketHandler(this, packetHelper.HandlerRegistry);
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

            if (!Options.UPnP || Instance.StartupOptions.NoNatPunchthrough)
            {
                return;
            }

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
