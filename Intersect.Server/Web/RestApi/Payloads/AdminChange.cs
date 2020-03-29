using System.ComponentModel.DataAnnotations.Schema;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Server.Web.RestApi.Payloads
{

    public struct AdminChange
    {

        [JsonIgnore, NotMapped]
        public bool IsValid => !string.IsNullOrWhiteSpace(New);

        [NotNull]
        public string New { get; set; }

    }

}
