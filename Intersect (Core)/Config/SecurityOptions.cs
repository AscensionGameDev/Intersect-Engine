using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Config
{
    public class SecurityOptions
    {
        [JsonProperty("PacketFlooding")]
        public PacketOptions PacketOpts = new PacketOptions();
    }
}
