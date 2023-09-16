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
        public string Default()
        {
            return "GET:demo";
        }

        [HttpGet("authorize")]
        [Authorize]
        public object Authorize()
        {
            return "GET:demo/authorize";
        }
    }
}
