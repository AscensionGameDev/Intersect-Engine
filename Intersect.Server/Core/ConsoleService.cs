using Intersect.Core;
using Intersect.Server.Core.Services;

using System.Threading;

namespace Intersect.Server.Core
{

    internal sealed partial class ConsoleService : ApplicationService<IServerContext, IConsoleService, ConsoleService>, IConsoleService
    {

        private readonly ConsoleThread mConsoleThread;

        public Thread Thread { get; private set; }

        public ConsoleService()
        {
            mConsoleThread = new ConsoleThread();
        }

        /// <inheritdoc />
        public override bool IsEnabled => true;

        /// <inheritdoc />
        protected override void TaskStart(IServerContext applicationContext) {
            if (!applicationContext.StartupOptions.DoNotShowConsole)
            {
                Thread = mConsoleThread.Start(applicationContext);
            }
        }

        /// <inheritdoc />
        protected override void TaskStop(IServerContext applicationContext)
        {
            // Nothing to do, the thread already has a stopping condition
        }

        public void Wait(bool doNotContinue = false) => mConsoleThread.Wait(doNotContinue);

    }

}
