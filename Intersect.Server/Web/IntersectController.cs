using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Intersect.Security.Claims;
using Intersect.Server.Web.Types;
using Microsoft.AspNetCore.Mvc;
using MyCSharp.HttpUserAgentParser.AspNetCore;
using IntersectUser = Intersect.Server.Database.PlayerData.User;

namespace Intersect.Server.Web;

public abstract class IntersectController : Controller
{
    private string? _userAgent;
    private string? _userAgentProductName;
    private string? _userAgentProductVersion;
    private string? _userAgentComment;

    private bool TryGetUserAgent(out string? userAgent)
    {
        userAgent = _userAgent;
        if (!string.IsNullOrWhiteSpace(userAgent))
        {
            return true;
        }

        // ReSharper disable once InvertIf
        if (_userAgent == null)
        {
            var accessor = HttpContext.RequestServices.GetService<IHttpUserAgentParserAccessor>();
            var userAgentString = accessor.GetHttpContextUserAgent(HttpContext);
            _userAgent = userAgentString;

            var parts = (_userAgent ?? string.Empty).Split(' ').Select(ProductInfoHeaderValue.Parse).ToArray();
            foreach (var part in parts)
            {
                if (part.Product != null)
                {
                    _userAgentProductName = part.Product.Name;
                    _userAgentProductVersion = part.Product.Version;
                    continue;
                }

                var userAgentComment = part.Comment;
                if (!string.IsNullOrWhiteSpace(userAgentComment))
                {
                    if (userAgentComment.StartsWith('('))
                    {
                        userAgentComment = userAgentComment[1..];
                    }

                    if (userAgentComment.EndsWith(')'))
                    {
                        userAgentComment = userAgentComment[..^1];
                    }

                    _userAgentComment = userAgentComment;
                }
            }
        }

        userAgent = _userAgent;
        return !string.IsNullOrWhiteSpace(userAgent);
    }

    protected string? UserAgentProductName => TryGetUserAgent(out _) ? _userAgentProductName : null;

    protected string? UserAgentProductVersion => TryGetUserAgent(out _) ? _userAgentProductVersion : null;

    protected string? UserAgentComment => TryGetUserAgent(out _) ? _userAgentComment : null;

    protected string? UserAgent => TryGetUserAgent(out var userAgent) ? userAgent : null;

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
    protected virtual IActionResult Ok(string message) => StatusCodeMessage(HttpStatusCode.OK, message);

    [NonAction]
    protected virtual IActionResult NotFound(string message) => StatusCodeMessage(HttpStatusCode.NotFound, message);

    [NonAction]
    protected virtual IActionResult Gone(string message) => StatusCodeMessage(HttpStatusCode.Gone, message);

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