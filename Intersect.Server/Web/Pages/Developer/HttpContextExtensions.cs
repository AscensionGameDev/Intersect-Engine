using Intersect.Server.Web.Controllers.AssetManagement;
using Microsoft.Extensions.Options;

namespace Intersect.Server.Web.Pages.Developer;

public static class HttpContextExtensions
{
    public static UpdateServerOptions GetUpdateServerOptions(this HttpContext httpContext)
    {
        return httpContext.RequestServices.GetService<IOptionsSnapshot<UpdateServerOptions>>()?.Value ??
               new UpdateServerOptions
               {
                   Enabled = false,
               };
    }

    public static bool IsUpdateServerEnabled(this HttpContext httpContext)
    {
        return httpContext.RequestServices.GetService<IOptionsSnapshot<UpdateServerOptions>>()?.Value.Enabled ?? false;
    }
}