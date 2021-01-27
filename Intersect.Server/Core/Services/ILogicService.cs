using Amib.Threading;
using Intersect.Core;

namespace Intersect.Server.Core.Services
{
    internal interface ILogicService : IThreadableApplicationService
    {
        object LogicLock { get; }

        SmartThreadPool LogicPool { get; }
    }
}
