using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.General;
using Nancy;
using Newtonsoft.Json.Linq;

namespace Intersect.Server.WebApi.Modules
{
    public class StatsModule : ServerModule
    {
        public StatsModule() : base("/stats")
        {
            Get("/", parameters =>
            {
                var stats = new JObject {
                    {"uptime", Globals.System?.GetTimeMs() ?? -1},
                    {"cps", Globals.Cps},
                    {"connectedClients", Globals.Clients?.Count},
                    {"onlineCount", Globals.OnlineList?.Count}
                };

                return stats.ToString();
            });
        }
    }
}
