using Intersect.Core;
using Intersect.Server.Core.Services;
using Intersect.Server.Networking.Lidgren;
using Intersect.Server.Web.RestApi;

namespace Intersect.Server.Core
{
    /// <summary>
    /// Declares the API surface of server contexts.
    /// </summary>
    internal interface IServerContext : IApplicationContext<ServerCommandLineOptions>
    {
        #region Services

        /// <summary>
        /// The server's console processing service.
        /// </summary>
        IConsoleService ConsoleService { get; }

        /// <summary>
        /// The server's core logic service.
        /// </summary>
        ILogicService LogicService { get; }

        /// <summary>
        /// The server's network processing service.
        /// </summary>
        ServerNetwork Network { get; }

        /// <summary>
        /// The server's REST API provider service.
        /// </summary>
        RestApi RestApi { get; }

        #endregion Services
    }
}
