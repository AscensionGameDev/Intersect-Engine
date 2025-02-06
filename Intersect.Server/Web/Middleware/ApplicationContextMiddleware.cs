using Intersect.Core;

namespace Intersect.Server.Web.Middleware;

public sealed class ApplicationContextMiddleware(RequestDelegate next, IApplicationContext applicationContext)
{
    public async Task Invoke(HttpContext context)
    {
        ApplicationContext.Context.Value ??= applicationContext;
        await next(context);
    }
}