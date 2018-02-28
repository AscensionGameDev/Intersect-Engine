using Nancy.Responses;

namespace Intersect.Server.WebApi.Modules
{
    public class RootModule : ServerModule
    {
        public RootModule() : base("/")
        {
            Get("/", args => new HtmlResponse(Nancy.HttpStatusCode.NotFound));
        }
    }
}
