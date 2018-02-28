using Nancy.Responses;

namespace Intersect.Server.WebApi.Modules
{
    public class RootModule : ServerModule
    {
        public RootModule() : base("/")
        {
            Get("/", args =>
            {
                var identity = Context.CurrentUser;
                return new HtmlResponse(Nancy.HttpStatusCode.NotFound);
            });
            //Options("/", args => )
        }
    }
}
