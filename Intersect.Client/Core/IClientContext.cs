using Intersect.Core;

namespace Intersect.Client.Core
{
    /// <summary>
    /// Declares the API surface of client contexts.
    /// </summary>
    internal interface IClientContext : IApplicationContext<ClientCommandLineOptions>
    {
        /// <summary>
        /// The platform-specific runner that initializes the actual user-visible client.
        /// </summary>
        IPlatformRunner PlatformRunner { get; }
    }
}
