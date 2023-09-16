using System.Net;
using System.Security.Claims;
using Intersect.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using IntersectUser = Intersect.Server.Database.PlayerData.User;

namespace Intersect.Server.Web;

public abstract class IntersectController : Controller
{
    public const int PAGE_SIZE_MAX = 100;

    public const int PAGE_SIZE_MIN = 5;

    private IntersectUser? _intersectUser;

    public IntersectUser? IntersectUser
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
    public virtual ObjectResult StatusCode(HttpStatusCode statusCode, object? data = default)
    {
        return StatusCode((int)statusCode, data);
    }

    [NonAction]
    public virtual ObjectResult Forbidden(object? data = default)
    {
        return StatusCode(HttpStatusCode.Forbidden, data);
    }

    [NonAction]
    public virtual ObjectResult Gone()
    {
        return StatusCode(HttpStatusCode.Gone);
    }

    [NonAction]
    public virtual ObjectResult InternalServerError(object? data = default)
    {
        return StatusCode(HttpStatusCode.InternalServerError, data);
    }

    [NonAction]
    public virtual ObjectResult NotImplemented(object? data = default)
    {
        return StatusCode(HttpStatusCode.NotImplemented, data);
    }
}