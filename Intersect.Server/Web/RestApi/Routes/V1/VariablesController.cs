using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Intersect.Server.Database.GameData;
using Intersect.Server.Web.RestApi.Attributes;
using Intersect.Server.Web.RestApi.Payloads;

namespace Intersect.Server.Web.RestApi.Routes.V1
{

    [RoutePrefix("variables")]
    [ConfigurableAuthorize]
    public sealed class VariablesController : ApiController
    {

        [Route("global")]
        [HttpPost]
        public object GlobalVariablesGet([FromBody] PagingInfo pageInfo)
        {
            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.Count = Math.Max(Math.Min(pageInfo.Count, 100), 5);

            var context = GameContext.Current;
            var entries = GameContext.Queries.ServerVariables(context, pageInfo.Page, pageInfo.Count)?.ToList();

            return new
            {
                total = context.ServerVariables?.Count() ?? 0,
                pageInfo.Page,
                count = entries?.Count ?? 0,
                entries
            };
        }

        [Route("global/{guid:guid}")]
        [HttpGet]
        public object GlobalVariableGet(Guid guid)
        {
            if (Guid.Empty == guid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid global variable id.");
            }

            var context = GameContext.Current;
            var variable = GameContext.Queries.ServerVariableById(context, guid);

            if (variable == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No global variable with id '{guid}'.");
            }

            return variable;
        }

        [Route("global/{guid:guid}/value")]
        [HttpGet]
        public object GlobalVariableGetValue(Guid guid)
        {
            if (Guid.Empty == guid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid global variable id.");
            }

            var context = GameContext.Current;
            var variable = GameContext.Queries.ServerVariableById(context, guid);

            if (variable == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No global variable with id '{guid}'.");
            }

            return new
            {
                value = variable?.Value.Value,
            };
        }

        [Route("global/{guid:guid}")]
        [HttpPost]
        public object GlobalVariableSet(Guid guid, [FromBody] VariableValue value)
        {
            if (Guid.Empty == guid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid global variable id.");
            }

            var context = GameContext.Current;
            var variable = GameContext.Queries.ServerVariableById(context, guid);

            if (variable == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No global variable with id '{guid}'.");
            }

            variable.Value.Value = value.Value;

            return variable;
        }

    }

}
