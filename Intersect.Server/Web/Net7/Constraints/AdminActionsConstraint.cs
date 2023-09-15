using Intersect.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Intersect.Server.Web.Constraints;

internal sealed partial class AdminActionsConstraint : IRouteConstraint
{
    /// <inheritdoc />
    public bool Match(
        HttpContext httpContext,
        IRouter route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection
    )
    {
        if (!values.TryGetValue(routeKey, out var value) || value == null)
        {
            return false;
        }

        var stringValue = value as string ?? Convert.ToString(value);

        return Enum.TryParse<AdminAction>(stringValue, out _);
    }
}