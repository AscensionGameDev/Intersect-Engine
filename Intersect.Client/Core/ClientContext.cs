using Intersect.Client.Plugins.Contexts;
using Intersect.Core;
using Intersect.Factories;
using Intersect.Logging;
using Intersect.Plugins;
using Intersect.Reflection;

using JetBrains.Annotations;

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

        internal ClientContext(ClientCommandLineOptions startupOptions, [NotNull] Logger logger) : base(
            startupOptions, logger
        )
        {
            FactoryRegistry<IPluginContext>.RegisterFactory(new ClientPluginContext.Factory());
        }

        protected override bool UsesMainThread => true;

        [NotNull]
        public IPlatformRunner PlatformRunner
        {
            get => mPlatformRunner ?? throw new ArgumentNullException(nameof(PlatformRunner));
            private set => mPlatformRunner = value;
        }

        /// <inheritdoc />
        protected override void InternalStart()
        {
            PlatformRunner = typeof(ClientContext).Assembly.CreateInstanceOf<IPlatformRunner>();
            PlatformRunner.Start(this, StartServices);
        }

        #region Exception Handling

        [UsedImplicitly]
        internal static void DispatchUnhandledException(
            [NotNull] Exception exception,
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
    }
}
