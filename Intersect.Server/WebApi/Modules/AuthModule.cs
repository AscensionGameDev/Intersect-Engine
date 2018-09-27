using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.WebApi.Authentication;
using JetBrains.Annotations;
using Jose;
using Nancy;
using Newtonsoft.Json;

namespace Intersect.Server.WebApi.Modules
{
    using MethodPath = ValueTuple<string, string>;
    using MethodPathCode = ValueTuple<string, string, HttpStatusCode>;

    public class AuthModule : ServerModule
    {
        [NotNull]
        private IAuthorizationProvider AuthorizationProvider => ServerApi.Instance.AuthorizationProvider;

        protected override bool Secured => false;

        protected override IDictionary<MethodPath, bool> RouteSecurity => new Dictionary<MethodPath, bool>
        {
            [("GET", "/access")] = true
        };

        protected override IDictionary<MethodPathCode, object> DefaultResponse => new Dictionary<MethodPathCode, object>
        {
            [("GET", "/access", HttpStatusCode.Unauthorized)] = new string[0]
        };

        public AuthModule() : base("/auth")
        {
            Post("/", args => RequestAuthorization(args));
            Delete("/", args => RevokeAuthorization(args));

#if DEBUG
            Get("/request", args => RequestAuthorization(args));
            Get("/revoke", args => RevokeAuthorization(args));
#endif

            Get("/access", args =>
            {
                var userIdentity = Context?.CurrentUser?.Identity as UserIdentity;
                var access = userIdentity?.User.Power.EnumeratePermissions() ?? new List<string>();
                //return Response.AsJson(JsonConvert.SerializeObject(access));
                return Response.AsJson(access);
            });
        }

        private Response RequestAuthorization(dynamic args)
        {
            string username = Request?.Form?.username;
            string password = Request?.Form?.password;

#if DEBUG
            username = username ?? Request?.Query?.username;
            password = password ?? Request?.Query?.password;
#endif

            JwtToken token;
            try
            {
                token = AuthorizationProvider.Authorize(username, password);
            }
            catch (Exception exception)
            {
                return new Response
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    ReasonPhrase = exception.Message ?? "Invalid credentials."
                };
            }

            //return Response.AsJson(new {Authorization = $"Bearer {AuthorizationProvider.Encode(token)}"});

            return new Response
            {
                StatusCode = HttpStatusCode.OK,
                Headers =
                {
                    {"Authorization", $"Bearer {AuthorizationProvider.Encode(token)}"},

#if DEBUG
                    {"Set-Cookie", $"__isid={AuthorizationProvider.Encode(token)}; Path=/"}
#endif
                }
            };
        }

        private Response RevokeAuthorization(dynamic args)
        {
            var authorization = Request?.Headers?.Authorization;

#if DEBUG
            authorization = authorization ?? Request?.Query?.authorization;
#endif

            var token = AuthorizationProvider.Decode(authorization);
            var statusCode = HttpStatusCode.InternalServerError;

            if (token == null)
            {
                statusCode = HttpStatusCode.Unauthorized;
            } else if (AuthorizationProvider.Expire(token))
            {
                statusCode = HttpStatusCode.OK;
            }

            return new Response { StatusCode = statusCode};
        }
    }
}
