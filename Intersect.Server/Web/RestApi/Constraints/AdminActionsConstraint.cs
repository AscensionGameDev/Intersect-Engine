using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;

using Intersect.Enums;

namespace Intersect.Server.Web.RestApi.Constraints
{

    internal sealed class AdminActionsConstraint : IHttpRouteConstraint
    {

        /// <inheritdoc />
        public bool Match(
            HttpRequestMessage request,
            IHttpRoute route,
            string parameterName,
            IDictionary<string, object> values,
            HttpRouteDirection routeDirection
        )
        {
            if (!values.TryGetValue(parameterName, out var value) || value == null)
            {
                return false;
            }

            var stringValue = value as string ?? Convert.ToString(value);

            return Enum.TryParse<AdminActions>(stringValue, out _);
        }

    }

}
