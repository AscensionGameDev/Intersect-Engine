using System.Web.Http;
using Intersect.Server.General;
using Intersect.Server.Web.RestApi.Attributes;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [RoutePrefix("demo")]
    public sealed class DemoController : ApiController
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
        [ConfigurableAuthorize]
        public object Default()
        {
            return new
            {
                name = Options.GameName,
                port = Options.ServerPort,
            };
        }

        [Route("config")]
        [HttpGet]
        [ConfigurableAuthorize]
        public object Config()
        {
            return new
            {
                name = Options.GameName,
                port = Options.ServerPort,
                upnp = Options.UPnP,
                openPortChecker = Options.OpenPortChecker
            };
        }

        [Route("config/{param}")]
        [HttpGet]
        [ConfigurableAuthorize]
        public object Param(string param)
        {
            return new
            {
                name = Options.GameName,
                port = Options.ServerPort,
                upnp = Options.UPnP,
                openPortChecker = Options.OpenPortChecker
            };
        }

        [Route("stats")]
        [HttpGet]
        public object Stats()
        {
            return new
            {
                uptime = Globals.Timing.TimeMs,
                cps = Globals.Cps,
                connectedClients = Globals.Clients?.Count,
                onlineCount = Globals.OnlineList?.Count
            };
        }
    }
}