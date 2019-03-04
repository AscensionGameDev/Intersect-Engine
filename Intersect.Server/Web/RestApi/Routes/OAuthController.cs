using Intersect.Server.Classes.Database.PlayerData.Api;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Intersect.Server.Web.RestApi.Routes
{
    [RoutePrefix("oauth")]
    public sealed class OAuthController : ApiController
    {
        [Authorize]
        [HttpDelete]
        [Route("token/{tokenId:guid}")]
        public IHttpActionResult DeleteToken(Guid tokenId)
        {
            if (User?.Identity == null)
            {
                return Unauthorized();
            }

            if (RefreshToken.Remove(tokenId, true).GetAwaiter().GetResult())
            {
                return Ok(new
                {
                    revoked_refresh_token = tokenId
                });
            }

            return StatusCode(HttpStatusCode.Gone);
        }

        [Route("authorize")]
        [HttpGet]
        public IHttpActionResult Authorize()
        {
            
            return Ok(new
            {
                authorize = "yes"
            });
            //var authentication = HttpContext.GetOwinContext().Authentication;
            //var ticket = authentication.AuthenticateAsync("Application").Result;
            //var identity = ticket != null ? ticket.Identity : null;
            //if (identity == null)
            //{
            //    authentication.Challenge("Application");
            //    return new HttpUnauthorizedResult();
            //}

            //var scopes = (Request.QueryString.Get("scope") ?? "").Split(' ');

            //if (Request.HttpMethod == "POST")
            //{
            //    if (!string.IsNullOrEmpty(Request.Form.Get("submit.Grant")))
            //    {
            //        identity = new ClaimsIdentity(identity.Claims, "Bearer", identity.NameClaimType, identity.RoleClaimType);
            //        foreach (var scope in scopes)
            //        {
            //            identity.AddClaim(new Claim("urn:oauth:scope", scope));
            //        }
            //        authentication.SignIn(identity);
            //    }
            //    if (!string.IsNullOrEmpty(Request.Form.Get("submit.Login")))
            //    {
            //        authentication.SignOut("Application");
            //        authentication.Challenge("Application");
            //        return new HttpUnauthorizedResult();
            //    }
            //}
        }
    }
}
