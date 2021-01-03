using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace Intersect.Server.Web.RestApi.Payloads
{

    public struct AuthorizedChange
    {

        [JsonIgnore, NotMapped]
        public bool IsValid => !string.IsNullOrWhiteSpace(Authorization) && !string.IsNullOrWhiteSpace(New);

        public string Authorization { get; set; }

        public string New { get; set; }

    }

}
