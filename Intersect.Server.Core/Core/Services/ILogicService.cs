using Amib.Threading;
using Intersect.Core;

namespace Intersect.Server.Core.Services;

internal interface ILogicService : IThreadableApplicationService
{
    long CyclesPerSecond { get; }

    object LogicLock { get; }

    SmartThreadPool LogicPool { get; }
}
