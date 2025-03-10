using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using Htmx;
using Intersect.Server.Localization;
using Intersect.Server.Web.Authentication;
using Intersect.Server.Web.Pages.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebSocketSharp.Net;

namespace Intersect.Server.Web.Pages.Account;

public partial class LoginModel : PageModel
{
    private readonly IntersectAuthenticationManager _authenticationManager;

    public LoginModel(IntersectAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    [BindProperty]
    [Required]
    [Display(Name = "Password")]
    public string? Password { get; init; }

    [BindProperty]
    [Required]
    [Display(Name = "Username")]
    public string? Username { get; init; }

    [BindProperty]
    public string? RedirectUrl { get; init; }

    [BindProperty]
    public string? ReturnUrl
    {
        get => RedirectUrl;
        init
        {
            if (string.IsNullOrWhiteSpace(RedirectUrl))
            {
                RedirectUrl = value;
            }
        }
    }

    [BindProperty]
    [Display(Name = "Stay Logged In")]
    public bool StayLoggedIn { get; init; }

    public async Task<IActionResult> OnPost()
    {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!Database.PlayerData.User.TryAuthenticate(Username, Password, out var user))
        {
            return StatusCode((int)HttpStatusCode.BadRequest, Strings.Account.BadLogin);
        }

        var authenticationResult = await _authenticationManager.TryAuthenticate(user);
        var claimsIdentity = authenticationResult.Identity;
        var refreshToken = authenticationResult.RefreshToken;
        var statusCode = authenticationResult.Type switch
        {
            AuthenticationResultType.Unknown => HttpStatusCode.InternalServerError,
            AuthenticationResultType.Success => HttpStatusCode.OK,
            AuthenticationResultType.ErrorOccurred => HttpStatusCode.InternalServerError,
            AuthenticationResultType.Expired => HttpStatusCode.Forbidden,
            AuthenticationResultType.Unauthorized => HttpStatusCode.Unauthorized,
            _ => throw new UnreachableException(),
        };
        if (authenticationResult.Type != AuthenticationResultType.Success ||
            claimsIdentity == default ||
            refreshToken == default)
        {
            if (statusCode == HttpStatusCode.OK)
            {
                statusCode = HttpStatusCode.InternalServerError;
            }

            return StatusCode((int)statusCode, Strings.General.UnknownErrorPleaseTryAgain);
        }

        var claimsPrincipal = authenticationResult.Principal ?? new ClaimsPrincipal(claimsIdentity);
        var redirectUrl = string.IsNullOrWhiteSpace(RedirectUrl) ? "/" : RedirectUrl;

        AuthenticationProperties authenticationProperties = new()
        {
            AllowRefresh = true,
            ExpiresUtc = authenticationResult.ExpiresAt,
            IsPersistent = StayLoggedIn,
            IssuedUtc = authenticationResult.IssuedAt,
            RedirectUri = redirectUrl,
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            authenticationProperties
        );

        if (Request.IsHtmx())
        {
            Response.Headers["HX-Redirect"] = redirectUrl;
        }

        var avatar = user.TryLoadAvatarName(out _, out var avatarName, out _)
            ? avatarName
            : default;
        var userProfile = new UserProfile(user.Name, avatar);
        return new OkObjectResult(userProfile);
    }
}