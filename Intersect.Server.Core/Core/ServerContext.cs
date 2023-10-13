using Intersect.Core;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Server.Core.Services;
using Intersect.Server.Database;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using System.Diagnostics;
using System.Resources;
using System.Security.Cryptography;
using Intersect.Factories;
using Intersect.Plugins;
using Intersect.Server.Plugins;
using Intersect.Server.General;
using Intersect.Plugins.Interfaces;
using Intersect.Rsa;
using Intersect.Server.Database.PlayerData.Players;


namespace Intersect.Server.Core
{
    /// <summary>
    /// Implements <see cref="IServerContext"/>.
    /// </summary>
    internal partial class ServerContext : ApplicationContext<ServerContext, ServerCommandLineOptions>, IServerContext
    {
        public static ServerContextFactory ServerContextFactory { get; set; } = (options, logger, packetHelper) =>
            new ServerContext(options, logger, packetHelper);

        protected ServerContext(
            ServerCommandLineOptions startupOptions,
            Logger logger,
            IPacketHelper packetHelper
        ) : base(
            startupOptions, logger, packetHelper
        )
        {
            // Register the factory for creating service plugin contexts
            FactoryRegistry<IPluginContext>.RegisterFactory(new ServerPluginContext.Factory());

            if (startupOptions.Port > 0)
            {
                Options.ServerPort = startupOptions.Port;
            }

            Network = CreateNetwork();
        }

        public ILogicService LogicService => GetExpectedService<ILogicService>();

        public INetwork Network { get; }

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

        internal int ExitCode { get; set; }

        internal bool DisposeWithoutExiting { get; set; }

        protected virtual void JoinOrKillConsoleThread(Stopwatch stopwatch) { }

        protected virtual void OnDisposing(Stopwatch stopwatch) { }

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

                OnDisposing(stopwatch);

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

                if (!DisposeWithoutExiting)
                {
                    JoinOrKillConsoleThread(stopwatch);
                }

                if (ThreadLogic?.IsAlive ?? false)
                {
                    Log.Info("Shutting down the logic thread..." + $" ({stopwatch.ElapsedMilliseconds}ms)");
                    if (!ThreadLogic.Join(10000))
                    {
                        try
                        {
                            ThreadLogic.Interrupt();
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

            if (DisposeWithoutExiting)
            {
                return;
            }

            Console.WriteLine(Strings.Commands.exited);
            Exit();
        }

        internal void Exit(int? exitCode = null)
        {
            Exit(exitCode ?? ExitCode);
        }

        internal static void Exit(int exitCode)
        {
            Environment.Exit(exitCode);
        }

        #endregion

        #region Threads

        public virtual void WaitForConsole() { }

        private Thread ThreadLogic => LogicService.Thread;

        #endregion

        #region Network


        internal static NetworkFactory? NetworkFactory { get; set; }

        protected virtual RsaKey AsymmetricKey => new(default);

        private INetwork CreateNetwork()
        {
            #region Apply CLI Options

            //Options.ServerPort = StartupOptions.ValidPort(Options.ServerPort);

            #endregion

            var packetHandler = new PacketHandler(this, PacketHelper.HandlerRegistry);

            var networkFactory = NetworkFactory;
            if (networkFactory == default)
            {
                throw new InvalidOperationException($"{nameof(NetworkFactory)} was not set.");
            }

            var network = networkFactory.Invoke(
                this,
                AsymmetricKey.Parameters,
                packetHandler.HandlePacket,
                packetHandler.PreProcessPacket
            );

            return network;
        }

        #region Listen

        protected virtual void InternalStartNetworking()
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
