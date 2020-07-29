using Intersect.Core;
using Intersect.Server.Core.Services;

using JetBrains.Annotations;

using System.Threading;

namespace Intersect.Server.Core
{
    internal sealed partial class LogicService : ApplicationService<IServerContext, ILogicService, LogicService>, ILogicService
    {

        [NotNull] private readonly LogicThread mLogicThread;

        public LogicService()
        {
            mLogicThread = new LogicThread();
        }

        public Thread Thread { get; private set; }

        public object LogicLock => mLogicThread.LogicLock;

        /// <inheritdoc />
        public override bool IsEnabled => true;

        /// <inheritdoc />
        protected override void TaskStart(IServerContext applicationContext) =>
            Thread = mLogicThread.Start(applicationContext);

        /// <inheritdoc />
        protected override void TaskStop(IServerContext applicationContext)
        {
            // Nothing to do, the thread already has a stopping condition
        }

    }
}
