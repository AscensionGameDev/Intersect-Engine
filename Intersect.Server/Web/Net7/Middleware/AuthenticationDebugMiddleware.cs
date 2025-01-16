using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Intersect.Server.Web.Middleware;

public class AuthenticationDebugMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of <see cref="AuthenticationMiddleware"/>.
    /// </summary>
    /// <param name="next">The next item in the middleware pipeline.</param>
    /// <param name="schemes">The <see cref="IAuthenticationSchemeProvider"/>.</param>
    public AuthenticationDebugMiddleware(RequestDelegate next, IAuthenticationSchemeProvider schemes)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        Schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
    }

    /// <summary>
    /// Gets or sets the <see cref="IAuthenticationSchemeProvider"/>.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public IAuthenticationSchemeProvider Schemes { get; }

    public async Task Invoke(HttpContext context)
    {
        var handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
        var schemes = (await Schemes.GetRequestHandlerSchemesAsync()).ToArray();
        foreach (var scheme in schemes)
        {
            if (await handlers.GetHandlerAsync(context, scheme.Name) is IAuthenticationRequestHandler handler &&
                await handler.HandleRequestAsync())
            {
                return;
            }
        }

        await _next(context);
    }
}