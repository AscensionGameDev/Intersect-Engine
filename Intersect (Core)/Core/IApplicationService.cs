
using System;

namespace Intersect.Core
{
    /// <summary>
    /// Declares the API surface for application services.
    /// </summary>
    public interface IApplicationService : IDisposable
    {
        #region Service Properties

        /// <summary>
        /// The actual type of the service, should be an interface type.
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        /// The name of the service, should be the name of the implementation type.
        /// </summary>
        string Name { get; }

        #endregion Service Properties

        #region Lifecycle Properties

        /// <summary>
        /// If the service is currently enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// If the service is currently running.
        /// </summary>
        bool IsRunning { get; }

        #endregion Lifecycle Properties

        #region Lifecycle Methods

        /// <summary>
        /// Bootstrapping lifecycle method for application services.
        /// </summary>
        /// <param name="applicationContext">the current application context</param>
        /// <returns>if bootstrapping was successful</returns>
        bool Bootstrap(IApplicationContext applicationContext);

        /// <summary>
        /// Startup lifecycle method for application services.
        /// </summary>
        /// <param name="applicationContext">the current application context</param>
        /// <returns>if startup was successful</returns>
        bool Start(IApplicationContext applicationContext);

        /// <summary>
        /// Shutdown lifecycle method for application services.
        /// </summary>
        /// <param name="applicationContext">the current application context</param>
        /// <returns>if shutdown was successful</returns>
        bool Stop(IApplicationContext applicationContext);

        #endregion Lifecycle Methods
    }
}
