using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Intersect.Security.Claims;
using Newtonsoft.Json;

namespace Intersect.Server.Database.PlayerData;

public partial class User
{
    private List<Claim>? _claims;

    [JsonIgnore]
    [NotMapped]
    public List<Claim> Claims
    {
        get => _claims ??= InitializeClaims();
        set => _claims = value;
    }

    private List<Claim> InitializeClaims()
    {
        var userIdString = Id.ToString();
        return new List<Claim>
        {
            new(ClaimTypes.Name, Name),
            new(ClaimTypes.NameIdentifier, userIdString),
            new(IntersectClaimTypes.UserId, userIdString),
            new(IntersectClaimTypes.UserName, Name),
            new(IntersectClaimTypes.Email, Email),
        };
    }
}