using System;

namespace Intersect.Client.Core
{
    /// <summary>
    /// Declares the API surface to launch instances of platform-specific (e.g. MonoGame, Unity) runners.
    /// </summary>
    internal interface IPlatformRunner
    {
        /// <summary>
        /// Starts the platform-specific runner for the provided context and post-startup action.
        /// </summary>
        /// <param name="context">the <see cref="IClientContext"/> to run for</param>
        /// <param name="postStartupAction">the <see cref="Action"/> to do after startup</param>
        void Start(IClientContext context, Action postStartupAction);
    }
}
