using System.ComponentModel.DataAnnotations.Schema;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Server.Web.RestApi.Payloads
{

    public struct AuthorizedChange
    {

        [JsonIgnore, NotMapped]
        public bool IsValid => !string.IsNullOrWhiteSpace(Authorization) && !string.IsNullOrWhiteSpace(New);

        [NotNull]
        public string Authorization { get; set; }

        [NotNull]
        public string New { get; set; }

    }

}
