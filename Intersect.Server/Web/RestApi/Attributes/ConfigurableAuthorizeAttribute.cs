using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

using Intersect.Server.Web.RestApi.Services;

namespace Intersect.Server.Web.RestApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    internal class ConfigurableAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var authorized = base.IsAuthorized(actionContext);

            if (authorized)
            {
                return true;
            }

            var route = actionContext?.ControllerContext?.RouteData?.Route?.RouteTemplate ?? "";
            var method = actionContext?.Request?.Method?.Method ?? "GET";
            var service = actionContext?.ControllerContext?.Configuration?.DependencyResolver?.GetAuthorizedRoutes();

            return !service?.RequiresAuthorization(route, method) ?? false;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
        }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
        }
    }
}
