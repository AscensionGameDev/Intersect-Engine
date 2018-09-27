using System.Security.Claims;

namespace Intersect.Server.WebApi.Authentication
{
    public interface IAuthorizationProvider
    {
        ClaimsPrincipal FindUserFrom(JwtToken token);

        bool Expire(JwtToken token);

        JwtToken Authorize(string username, string password);

        JwtToken Decode(string authorizationHeader);

        string Encode(JwtToken token);
    }
}
