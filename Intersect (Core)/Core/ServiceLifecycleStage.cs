using System;

namespace Intersect.Core
{
    /// <summary>
    /// Enumeration of the different lifecycle stages for services.
    /// </summary>
    public enum ServiceLifecycleStage
    {
        /// <summary>
        /// The stage that runs after basic initialization of the application.
        /// </summary>
        Bootstrap,

        /// <summary>
        /// The application initialization stage.
        /// </summary>
        Startup,

        /// <summary>
        /// The application finalization stage.
        /// </summary>
        Shutdown,

        /// <summary>
        /// If the lifecycle stage is unknown.
        /// </summary>
        [Obsolete("This should only be used by ServiceLifecycleFailureException.")]
        Unknown
    }
}
