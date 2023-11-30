using Intersect.Enums;
using Intersect.Server.General;
using Intersect.Server.Metrics;
using Intersect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.RestApi.Routes.V1
{

    [Route("api/v1/info")]
    [Authorize]
    public sealed partial class InfoController : Controller
    {

        [HttpGet("authorized")]
        [Authorize]
        public object Authorized()
        {
            return new
            {
                authorized = true,
            };
        }

        [HttpGet]
        [AllowAnonymous]
        public object Default()
        {
            return new
            {
                name = Options.Instance.GameName,
                port = Options.ServerPort,
            };
        }

        [HttpGet("config")]
        public object Config()
        {
            return Options.Instance;
        }

        [HttpGet("config/stats")]
        public object CombatStats() => Enum.GetNames<Stat>();

        [HttpGet("stats")]
        public object Stats()
        {
            return new
            {
                uptime = Timing.Global.Milliseconds,
                cps = Globals.Cps,
                connectedClients = Globals.Clients?.Count,
                onlineCount = Globals.OnlineList?.Count
            };
        }

        [HttpGet("metrics")]
        [Produces("application/json")]
        public object StatsMetrics()
        {
            return Ok(MetricsRoot.Instance.Metrics );
        }
    }
}
