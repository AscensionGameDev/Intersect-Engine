using System;
using System.Net.Http;
using System.Web.Http;

using Intersect.Server.General;
using Intersect.Server.Metrics;
using Intersect.Server.Web.RestApi.Attributes;

namespace Intersect.Server.Web.RestApi.Routes.V1
{

    [RoutePrefix("info")]
    [ConfigurableAuthorize]
    public sealed class InfoController : ApiController
    {

        [Route("authorized")]
        [HttpGet]
        [Authorize]
        public object Authorized()
        {
            return new
            {
                authorized = true
            };
        }

        [Route]
        [HttpGet]
        public object Default()
        {
            return new
            {
                name = Options.Instance.GameName,
                port = Options.ServerPort,
            };
        }

        [Route("config")]
        [HttpGet]
        public object Config()
        {
            return Options.Instance;
        }

        [Route("config/stats")]
        [HttpGet]
        public object CombatStats()
        {
            return Enum.GetNames(typeof(Enums.Stats));
        }

        [Route("stats")]
        [HttpGet]
        public object Stats()
        {
            return new
            {
                uptime = Globals.Timing.Milliseconds,
                cps = Globals.Cps,
                connectedClients = Globals.Clients?.Count,
                onlineCount = Globals.OnlineList?.Count
            };
        }

        [Route("metrics")]
        [HttpGet]
        public object StatsMetrics()
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MetricsRoot.Instance.Metrics, System.Text.Encoding.UTF8, "application/json")
            };
        }
    }

}
