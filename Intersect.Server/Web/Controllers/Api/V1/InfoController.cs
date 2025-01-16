using System.Net;
using Intersect.Enums;
using Intersect.Server.General;
using Intersect.Server.Metrics;
using Intersect.Server.Web.Http;
using Intersect.Server.Web.Types;
using Intersect.Server.Web.Types.Info;
using Intersect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.Controllers.Api.V1
{
    [Route("api/v1/info")]
    [Authorize]
    public sealed partial class InfoController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(InfoResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult Default() => Ok(new InfoResponseBody(Options.Instance.GameName, Options.ServerPort));

        [HttpGet("authorized")]
        [Authorize]
        [ProducesResponseType(typeof(AuthorizedResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult Authorized() => Ok(new AuthorizedResponseBody());

        [HttpGet("config")]
        [ProducesResponseType(typeof(Options), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult Config() => Ok(Options.Instance);

        [HttpGet("config/stats")]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult CombatStats() => Ok(Enum.GetNames<Stat>());

        [HttpGet("stats")]
        [ProducesResponseType(typeof(InfoStatsResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult Stats() => Ok(new InfoStatsResponseBody(Timing.Global.Milliseconds, Globals.Cps, Globals.Clients?.Count, Globals.OnlineList?.Count));

        [HttpGet("metrics")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(ContentResult), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult StatsMetrics()
        {
            if (MetricsRoot.Instance == default)
            {
                return NotFound("Metrics not found or they're disabled.");
            }

            return Ok(Content(MetricsRoot.Instance?.Metrics, ContentTypes.Json));
        }
    }
}
