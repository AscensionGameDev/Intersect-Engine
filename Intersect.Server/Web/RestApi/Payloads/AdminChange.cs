using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace Intersect.Server.Web.RestApi.Payloads
{

    public struct AdminChange
    {

        [JsonIgnore, NotMapped]
        public bool IsValid => !string.IsNullOrWhiteSpace(New);

        public string New { get; set; }

    }

}
