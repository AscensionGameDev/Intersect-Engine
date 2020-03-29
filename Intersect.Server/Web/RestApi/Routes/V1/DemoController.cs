using System.Web.Http;

using Intersect.Server.Web.RestApi.Attributes;

namespace Intersect.Server.Web.RestApi.Routes.V1
{

    [ConfigurableAuthorize]
    [RoutePrefix("demo")]
    public sealed class DemoController : ApiController
    {

        [Route]
        [HttpGet]
        [AllowAnonymous]
        public string Default()
        {
            return "GET:demo";
        }

        [Route("authorize")]
        [HttpGet]
        [Authorize]
        public object Authorize()
        {
            return "GET:demo/authorize";
        }

        [Route("configurable_authorize")]
        [HttpGet]
        [ConfigurableAuthorize]
        public string ConfigurableAuthorize()
        {
            return "GET:demo/configurable_authorize";
        }

        [Route("configurable_authorize/{param}")]
        [HttpGet]
        [ConfigurableAuthorize]
        public string ConfigurableAuthorize(string param)
        {
            return "GET:demo/configurable_authorize:" + param;
        }

        [Route("configurable_authorize/{*param}")]
        [HttpGet]
        [ConfigurableAuthorize]
        public string ConfigurableAuthorizeParams(string param)
        {
            return "GET:demo/configurable_authorize:" + param;
        }

    }

}
