using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Intersect.Server.Web.RestApi.Types.User;

public partial struct AdminChangeRequestBody
{
    [JsonIgnore, NotMapped]
    public bool IsValid => !string.IsNullOrWhiteSpace(New);

    public string New { get; set; }
}
