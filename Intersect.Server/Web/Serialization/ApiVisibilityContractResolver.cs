using System.Reflection;
using System.Security.Claims;
using Intersect.Reflection;
using Intersect.Security;
using Intersect.Security.Claims;
using Newtonsoft.Json.Serialization;

namespace Intersect.Server.Web.Serialization;

public class ApiVisibilityContractResolver : DefaultContractResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiVisibilityContractResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override List<MemberInfo> GetSerializableMembers(Type objectType)
    {
        var typeApiVisibility = objectType?.GetCustomAttribute<ApiVisibilityAttribute>();
        var httpContext = _httpContextAccessor.HttpContext;

        var claimsPrincipal = httpContext?.User;
        var readClaims = claimsPrincipal?.FindAll(IntersectClaimTypes.AccessRead).ToArray() ?? Array.Empty<Claim>();

        var hasAccessToType = typeApiVisibility?.Visibility != ApiVisibility.Hidden;
        if (hasAccessToType && (typeApiVisibility?.Visibility.HasFlag(ApiVisibility.Restricted) ?? false))
        {
            hasAccessToType = readClaims.Any(
                claim => string.Equals(objectType.FullName, claim?.Value, StringComparison.Ordinal)
            );
        }

        if (!hasAccessToType)
        {
            // return new List<MemberInfo>();
        }

        var serializableMembers = base.GetSerializableMembers(objectType);

        return serializableMembers?.Where(
                memberInfo =>
                {
                    var apiVisibility = memberInfo?.GetCustomAttribute<ApiVisibilityAttribute>();
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
                        return readClaims.Any(
                            claim => string.Equals(memberInfo?.GetFullName(), claim?.Value, StringComparison.Ordinal)
                        );
                    }

                    return false;
                }
            )
            .ToList();
    }
}