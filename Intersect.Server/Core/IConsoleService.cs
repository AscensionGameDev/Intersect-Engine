using Intersect.Core;

namespace Intersect.Server.Core.Services
{
    internal interface IConsoleService : IThreadableApplicationService
    {

        void Wait(bool doNotContinue = false);

    }
}
