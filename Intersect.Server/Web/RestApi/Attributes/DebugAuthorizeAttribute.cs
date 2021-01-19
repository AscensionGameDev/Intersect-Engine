using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Intersect.Server.Web.RestApi.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    internal class DebugAuthorizeAttribute : AuthorizeAttribute
    {

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
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
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
        }

    }

}
