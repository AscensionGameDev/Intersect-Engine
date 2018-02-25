using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Server.Classes.General;
using Nancy;
using Newtonsoft.Json.Linq;

namespace Intersect.Server.WebApi.Modules
{
    public class StatsModule : ServerModule
    {
        public StatsModule()
        {
            Get["/stats", true] = async (parameters, ct) =>
            {
                var stats = new JObject
                {
                    {"online", Globals.Clients?.Count},
                    {"cps", Globals.Cps}
                };

                return stats.ToString();
            };
        }
    }
}
