using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.Web.RestApi.Payloads
{
    public class IpAddress
    {
        public string Ip { get; set; }

        public DateTime LastUsed { get; set; }

        public Dictionary<Guid, string> OtherUsers { get; set; } = new Dictionary<Guid, string>();
    }
}
