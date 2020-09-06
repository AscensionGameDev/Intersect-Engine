using Intersect.Core;

using JetBrains.Annotations;

namespace Intersect.Server.Core.Services
{
    internal interface ILogicService : IThreadableApplicationService
    {
        [NotNull] object LogicLock { get; }
    }
}
