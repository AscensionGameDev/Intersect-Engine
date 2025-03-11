using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Intersect.Server.Web.Extensions;

public static class HttpContextExtensions
{
    public static ILogger GetAPILogger(this HttpContext httpContext)
    {
        return httpContext.RequestServices.GetRequiredService<ILogger<ApiService>>();
    }

    public static bool IsController(this HttpContext httpContext)
    {
        if (httpContext.GetEndpoint() is not { } endpoint)
        {
            return false;
        }

        return endpoint.Metadata.GetMetadata<ControllerActionDescriptor>() is not null;
    }

    public static bool IsPage(this HttpContext httpContext)
    {
        if (httpContext.GetEndpoint() is not { } endpoint)
        {
            return false;
        }

        return endpoint.Metadata.GetMetadata<PageRouteMetadata>() is not null;
    }
}