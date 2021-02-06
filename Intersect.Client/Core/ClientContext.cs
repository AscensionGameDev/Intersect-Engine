using Intersect.Client.Networking;
using Intersect.Client.Plugins.Contexts;
using Intersect.Core;
using Intersect.Factories;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Plugins;
using Intersect.Plugins.Interfaces;
using Intersect.Reflection;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Intersect.Client.Core
{
    /// <summary>
    /// Implements <see cref="IClientContext"/>.
    /// </summary>
    internal sealed class ClientContext : ApplicationContext<ClientContext, ClientCommandLineOptions>, IClientContext
    {
        private IPlatformRunner mPlatformRunner;

        internal ClientContext(ClientCommandLineOptions startupOptions, Logger logger, INetworkHelper networkHelper) : base(
            startupOptions, logger, networkHelper
        )
        {
            FactoryRegistry<IPluginContext>.RegisterFactory(new ClientPluginContext.Factory());
        }

        protected override bool UsesMainThread => true;

        public IPlatformRunner PlatformRunner
        {
            get => mPlatformRunner ?? throw new ArgumentNullException(nameof(PlatformRunner));
            private set => mPlatformRunner = value;
        }

        /// <inheritdoc />
        protected override void InternalStart()
        {
            Networking.Network.PacketHandler = new PacketHandler(this, NetworkHelper.HandlerRegistry);
            PlatformRunner = typeof(ClientContext).Assembly.CreateInstanceOf<IPlatformRunner>();
            PlatformRunner.Start(this, PostStartup);
        }

        #region Exception Handling

        internal static void DispatchUnhandledException(
            Exception exception,
            bool isTerminating = true,
            bool wait = false
        )
        {
            var sender = Thread.CurrentThread;
            var task = Task.Factory.StartNew(
                () => HandleUnhandledException(sender, new UnhandledExceptionEventArgs(exception, isTerminating))
            );

            if (wait)
            {
                task.Wait();
            }
        }

        #endregion Exception Handling

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                NetworkHelper.HandlerRegistry.Dispose();
            }
        }
    }
}
