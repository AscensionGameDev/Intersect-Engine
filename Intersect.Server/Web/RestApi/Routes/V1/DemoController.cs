using System.Net;
using Intersect.Server.Web.Http;
using Intersect.Server.Web.RestApi.Types.Demo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [Authorize]
    [Route("api/v1/demo")]
    public sealed partial class DemoController : IntersectController
    {
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(DemoResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult Default() => Ok(new DemoResponseBody("GET:demo"));

        [HttpGet("authorize")]
        [Authorize]
        [ProducesResponseType(typeof(DemoResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult Authorize() => Ok(new DemoResponseBody("GET:demo/authorize"));
    }
}
