using System.Net;
using System.Security.Claims;
using Intersect.Security.Claims;
using Intersect.Server.Web.RestApi.Types;
using Microsoft.AspNetCore.Mvc;
using IntersectUser = Intersect.Server.Database.PlayerData.User;

namespace Intersect.Server.Web;

public abstract class IntersectController : Controller
{
    private IntersectUser? _intersectUser;

    protected IntersectUser? IntersectUser
    {
        get
        {
            if (_intersectUser != default)
            {
                return _intersectUser;
            }

            var identity = User.Identity as ClaimsIdentity;
            var idString = identity?.FindFirst(IntersectClaimTypes.UserId)?.Value;
            if (string.IsNullOrWhiteSpace(idString) || !Guid.TryParse(idString, out var id))
            {
                return default;
            }

            _intersectUser = IntersectUser.FindOnline(id) ?? IntersectUser.FindById(id);

            return _intersectUser;
        }
    }

    [NonAction]
    protected virtual IActionResult StatusCodeMessage(HttpStatusCode statusCode, string message) => StatusCode(
        (int)statusCode,
        new StatusMessageResponseBody(message)
    );

    [NonAction]
    protected virtual IActionResult Forbidden(string message) => StatusCodeMessage(HttpStatusCode.Forbidden, message);

    [NonAction]
    protected virtual IActionResult InternalServerError(string message) =>
        StatusCodeMessage(HttpStatusCode.InternalServerError, message);

    [NonAction]
    protected virtual IActionResult NotImplemented(string message) =>
        StatusCodeMessage(HttpStatusCode.NotImplemented, message);

    [NonAction]
    protected virtual IActionResult BadRequest(string message) => StatusCodeMessage(HttpStatusCode.BadRequest, message);

    [NonAction]
    protected virtual IActionResult NotFound(string message) => StatusCodeMessage(HttpStatusCode.NotFound, message);

    [NonAction]
    protected virtual ObjectResult StatusCode(HttpStatusCode statusCode, object? data = default) =>
        StatusCode((int)statusCode, data);

    [NonAction]
    protected virtual ObjectResult Forbidden(object? data = default) => StatusCode(HttpStatusCode.Forbidden, data);

    [NonAction]
    protected virtual ObjectResult Gone() => StatusCode(HttpStatusCode.Gone);

    [NonAction]
    protected virtual ObjectResult InternalServerError(object? data = default) =>
        StatusCode(HttpStatusCode.InternalServerError, data);

    [NonAction]
    protected virtual ObjectResult NotImplemented(object? data = default) =>
        StatusCode(HttpStatusCode.NotImplemented, data);
}