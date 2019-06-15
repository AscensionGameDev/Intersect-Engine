using Intersect.Server.Database.GameData;
using Intersect.Server.Web.RestApi.Attributes;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Intersect.Server.Web.RestApi.Routes.V1
{

    [RoutePrefix("variables")]
    [ConfigurableAuthorize]
    public sealed class VariableController : ApiController
    {

        [Route("global")]
        [HttpGet]
        public object GlobalVariablesGet(int page = 0, int count = 10)
        {
            page = Math.Max(page, 0);
            count = Math.Max(Math.Min(count, 100), 5);

            using (var context = GameContext.Temporary)
            {
                var entries = GameContext.Queries.ServerVariables(context, page, count)?.ToList();
                return new
                {
                    total = context.ServerVariables?.Count() ?? 0,
                    page,
                    count = entries?.Count ?? 0,
                    entries
                };
            }
        }

        [Route("global/{guid:guid}")]
        [HttpGet]
        public object GlobalVariableGet(Guid guid)
        {
            if (Guid.Empty == guid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid global variable id.");
            }

            using (var context = GameContext.Temporary)
            {
                var variable = GameContext.Queries.ServerVariableById(context, guid);
                
                if (variable == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No global variable with id '{guid}'.");
                }

                return variable;
            }
        }

        [Route("global/{guid:guid}/value")]
        [HttpGet]
        public object GlobalVariableGetValue(Guid guid)
        {
            if (Guid.Empty == guid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid global variable id.");
            }

            using (var context = GameContext.Temporary)
            {
                var variable = GameContext.Queries.ServerVariableById(context, guid);

                return variable?.Value.Value ?? Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No global variable with id '{guid}'.");
            }
        }

        [Route("global/{guid:guid}")]
        [HttpPost]
        public object GlobalVariableSet(Guid guid, [FromBody] dynamic value)
        {
            if (Guid.Empty == guid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid global variable id.");
            }

            using (var context = GameContext.Temporary)
            {
                var variable = GameContext.Queries.ServerVariableById(context, guid);

                if (variable == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No global variable with id '{guid}'.");
                }

                variable.Value.Value = value;

                return variable;
            }
        }

    }

}
