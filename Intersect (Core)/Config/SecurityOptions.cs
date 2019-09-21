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
        
        public List<string> IpBlacklist = new List<string>();

        public bool CheckIp (string ip)
        {
            ip = ip.Trim();
            var parts = ip.Split('.');
            if (parts.Length != 4) return false; //Bad IP

            //Check if all 4 parts match any of the ips on our blacklist
            foreach (var checkIp in IpBlacklist)
            {
                var chkIp = checkIp.Trim();
                var chkParts = chkIp.Split('.');
                if (chkParts.Length == 4) //Valid IP
                {
                    var match = true;
                    for (int i = 0; i < 4; i++)
                    {
                        if (chkParts[i] != "*" && chkParts[i] != parts[i])
                        {
                            match = false;
                        }
                    }
                    if (match) return false; //Bad Ip
                }
            }

            return true; //Good Ip
        }
    }
}
