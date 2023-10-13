using Intersect.Core;
using Intersect.Server.Core.Services;

namespace Intersect.Server.Core
{

    internal sealed partial class ConsoleService : ApplicationService<IServerContext, IConsoleService, ConsoleService>, IConsoleService
    {

        private readonly ConsoleThread _consoleThread = new();

        public Thread Thread { get; private set; }

        /// <inheritdoc />
        public override bool IsEnabled => true;

        /// <inheritdoc />
        protected override void TaskStart(IServerContext applicationContext) {
            if (!applicationContext.StartupOptions.DoNotShowConsole)
            {
                Thread = _consoleThread.Start(applicationContext);
            }
        }

        /// <inheritdoc />
        protected override void TaskStop(IServerContext applicationContext)
        {
            // Nothing to do, the thread already has a stopping condition
        }

        public void Wait(bool doNotContinue = false) => _consoleThread.Wait(doNotContinue);

    }

}
