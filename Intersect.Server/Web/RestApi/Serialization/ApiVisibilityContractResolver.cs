using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web.Http.Controllers;

using Intersect.Reflection;
using Intersect.Security;
using Intersect.Security.Claims;

using JetBrains.Annotations;

using Newtonsoft.Json.Serialization;

namespace Intersect.Server.Web.RestApi.Serialization
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class ApiVisibilityContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 
        /// </summary>
        [NotNull] public HttpRequestContext RequestContext { get; }

        public ApiVisibilityContractResolver([NotNull] HttpRequestContext requestContext)
        {
            RequestContext = requestContext;
        }

        /// <inheritdoc />
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var serializableMembers = base.GetSerializableMembers(objectType);
            var typeApiVisibility = objectType?.GetCustomAttribute<ApiVisibilityAttribute>();
            return serializableMembers?.Where(
                memberInfo =>
                {
                    var apiVisibility = memberInfo?.GetCustomAttribute<ApiVisibilityAttribute>() ?? typeApiVisibility;
                    if (apiVisibility == null || apiVisibility.Visibility == ApiVisibility.Public)
                    {
                        return true;
                    }

                    if (apiVisibility.Visibility.HasFlag(ApiVisibility.Private))
                    {
                        // TODO: Not sure how to implement ownership visibility yet
                    }

                    // ReSharper disable once InvertIf
                    if (apiVisibility.Visibility.HasFlag(ApiVisibility.Restricted))
                    {
                        var claimsPrincipal = RequestContext.Principal as ClaimsPrincipal;
                        var readClaims = claimsPrincipal?.FindAll(IntersectClaimTypes.AccessRead) ?? new Claim[0];

                        return readClaims.Any(
                            claim => string.Equals(memberInfo?.GetFullName(), claim?.Value, StringComparison.Ordinal)
                        );
                    }

                    return false;
                }).ToList();
        }
    }
}
