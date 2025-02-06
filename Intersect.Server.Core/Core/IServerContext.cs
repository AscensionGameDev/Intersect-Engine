using Intersect.Core;
using Intersect.Network;
using Intersect.Server.Core.Services;

namespace Intersect.Server.Core;

/// <summary>
/// Declares the API surface of server contexts.
/// </summary>
internal interface IServerContext : IApplicationContext<ServerCommandLineOptions>
{
    #region Services

    /// <summary>
    /// The server's core logic service.
    /// </summary>
    ILogicService LogicService { get; }

    /// <summary>
    /// The server's network processing service.
    /// </summary>
    INetwork Network { get; }

    #endregion Services

    void WaitForConsole();

    void RequestShutdown(bool join = false);
}