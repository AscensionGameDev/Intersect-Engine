using Intersect.Server.Core.Services;
using Intersect.Server.Web;

namespace Intersect.Server.Core;

internal interface IFullServerContext : IServerContext
{
    /// <summary>
    /// The server's REST API provider service.
    /// </summary>
    IApiService ApiService { get; }

    /// <summary>
    /// The server's console processing service.
    /// </summary>
    IConsoleService ConsoleService { get; }
}