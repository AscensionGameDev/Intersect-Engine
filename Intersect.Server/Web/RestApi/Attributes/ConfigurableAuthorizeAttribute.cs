﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

using Intersect.Security.Claims;
using Intersect.Server.Web.RestApi.Services;

using JetBrains.Annotations;

namespace Intersect.Server.Web.RestApi.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    internal class ConfigurableAuthorizeAttribute : AuthorizeAttribute
    {
        [NotNull]
        protected IEnumerable<string> InternalRoles =>
            Roles?.Split(',').Where(role => !string.IsNullOrWhiteSpace(role)).Select(role => role.Trim()) ??
            new string[0];

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var authorized = base.IsAuthorized(actionContext);
            var whitelistedRoles = InternalRoles?.ToList();

            if (whitelistedRoles?.Count > 0)
            {
                return actionContext?.RequestContext?.Principal is ClaimsPrincipal claimsPrincipal &&
                       InternalRoles.Any(
                           role => !string.IsNullOrWhiteSpace(role) &&
                                   claimsPrincipal.HasClaim(
                                       claim => IntersectClaimTypes.Role.Equals(
                                                    claim?.Type, StringComparison.OrdinalIgnoreCase
                                                ) &&
                                                role.Equals(claim?.Value, StringComparison.OrdinalIgnoreCase)
                                   )
                       );
            }

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
