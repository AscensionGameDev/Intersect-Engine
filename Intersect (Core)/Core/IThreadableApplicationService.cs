
using System.Threading;

namespace Intersect.Core
{
    /// <summary>
    /// Declares the API surface for application services that have their own thread.
    /// </summary>
    public interface IThreadableApplicationService : IApplicationService
    {
        /// <summary>
        /// The thread for this service.
        /// </summary>
        Thread Thread { get; }
    }
}
