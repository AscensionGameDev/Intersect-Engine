using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Intersect.Security.Claims;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Api;
using Intersect.Server.Web.Configuration;
using Intersect.Server.Web.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Intersect.Server.Web.Authentication;

public sealed class IntersectAuthenticationManager(
    ILogger<IntersectAuthenticationManager> logger,
    IOptions<TokenGenerationOptions> tokenGenerationOptions
)
{
    public async Task<AuthenticationResult> TryAuthenticate(User? user)
    {
        try
        {
            if (user == default)
            {
                return new AuthenticationResult(AuthenticationResultType.Unauthorized);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var ticketId = Guid.NewGuid();
            var clientId = Guid.Empty;
            var claims = user.Claims.ToList();
            claims.Add(new Claim(IntersectClaimTypes.ClientId, clientId.ToString()));
            claims.Add(new Claim(IntersectClaimTypes.TicketId, ticketId.ToString()));
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = tokenGenerationOptions.Value.Audience,
                Issuer = tokenGenerationOptions.Value.Issuer,
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddMinutes(tokenGenerationOptions.Value.AccessTokenLifetime),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenGenerationOptions.Value.SecretData),
                    SecurityAlgorithms.HmacSha512Signature
                ),
            };

            var accessToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializedAccessToken = tokenHandler.WriteToken(accessToken);
            var issued = DateTime.UtcNow;
            var expires = issued.AddMinutes(tokenGenerationOptions.Value.RefreshTokenLifetime);
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                ClientId = default,
                Subject = user.Name,
                Issued = issued,
                Expires = expires,
                TicketId = ticketId,
                Ticket = serializedAccessToken,
            };

            if (!await RefreshToken.TryAddAsync(refreshToken))
            {
                return new AuthenticationResult(AuthenticationResultType.ErrorOccurred);
            }

            ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new AuthenticationResult(AuthenticationResultType.Success, Identity: claimsIdentity)
            {
                ExpiresAt = expires,
                IssuedAt = tokenDescriptor.IssuedAt ?? DateTime.UtcNow,
                RefreshToken = refreshToken,
            };
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Authentication failed for {UserId}", user?.Id);
            return new AuthenticationResult(AuthenticationResultType.ErrorOccurred);
        }
    }

    public async Task<AuthenticationResult> UpdatePrincipal(
        ClaimsPrincipal? currentPrincipal = default,
        bool force = false,
        User? user = default,
        bool ignoreMidpoint = false
    )
    {
        string? currentTicketIdRaw = currentPrincipal?.FindFirstValue(IntersectClaimTypes.TicketId);

        if (string.IsNullOrWhiteSpace(currentTicketIdRaw))
        {
            return new AuthenticationResult(AuthenticationResultType.Unauthorized);
        }

        if (!Guid.TryParse(currentTicketIdRaw, out var currentTicketId))
        {
            return new AuthenticationResult(AuthenticationResultType.Unauthorized);
        }

        if (!RefreshToken.TryFindForTicket(currentTicketId, out var currentRefreshToken, includeUser: user == default))
        {
            return new AuthenticationResult(AuthenticationResultType.Expired);
        }

        user ??= currentRefreshToken.User;
        if (user == default)
        {
            return new AuthenticationResult(AuthenticationResultType.Unauthorized);
        }

        var expirationMidpoint = (currentRefreshToken.Issued.Ticks + currentRefreshToken.Expires.Ticks) / 2;
        if (!force && (ignoreMidpoint || DateTime.UtcNow.Ticks <= expirationMidpoint))
        {
            var claimsIdentity = currentPrincipal.Identities.FirstOrDefault();

            if (claimsIdentity != default && currentPrincipal.HasSameRoles(user.Roles))
            {
                return new AuthenticationResult(
                    AuthenticationResultType.Success,
                    Identity: claimsIdentity,
                    Principal: currentPrincipal
                );
            }

            logger.LogInformation("Updating token due to role change for {Username}", user.Name);
        }
        else
        {
            logger.LogInformation("Updating token due to expiration for {Username}", user.Name);
        }

        var authenticationResult = await TryAuthenticate(user);
        if (authenticationResult.Identity == default || authenticationResult.Type != AuthenticationResultType.Success)
        {
            return authenticationResult;
        }

        ClaimsPrincipal claimsPrincipal = new(authenticationResult.Identity);
        return new AuthenticationResult(
            authenticationResult.Type,
            Identity: authenticationResult.Identity,
            Principal: claimsPrincipal
        )
        {
            ExpiresAt = authenticationResult.ExpiresAt,
            IssuedAt = authenticationResult.IssuedAt,
        };
    }
}