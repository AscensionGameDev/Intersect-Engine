namespace Intersect.Server.Web.Constraints;

public class NonNullConstraint : IRouteConstraint
{
    public bool Match(
        HttpContext httpContext,
        IRouter route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection
    )
    {
        return values.TryGetValue(routeKey, out var value) && value != null;
    }
}