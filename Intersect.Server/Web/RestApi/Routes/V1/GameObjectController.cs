using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Server.Web.RestApi.Attributes;
using Intersect.Server.Web.RestApi.Payloads;

namespace Intersect.Server.Web.RestApi.Routes.V1
{

    [RoutePrefix("gameobjects")]
    [ConfigurableAuthorize]
    public sealed class GameObjectController : ApiController
    {

        [Route("{objType}")]
        [HttpPost]
        public object List(string objType, [FromBody] PagingInfo pageInfo)
        {
            GameObjectType gameObjectType;
            if (!Enum.TryParse<GameObjectType>(objType, true, out gameObjectType) ||
                gameObjectType == GameObjectType.Time)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid object type.");
            }

            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.Count = Math.Max(Math.Min(pageInfo.Count, 100), 5);

            var lookup = GameObjectTypeExtensions.GetLookup(gameObjectType);

            if (lookup != null)
            {
                var entries = gameObjectType == GameObjectType.Event
                    ? lookup.Where(obj => ((EventBase) obj.Value).CommonEvent)
                        .OrderBy(obj => obj.Value.TimeCreated)
                        .Skip(pageInfo.Page * pageInfo.Count)
                        .Take(pageInfo.Count)
                    : lookup.OrderBy(obj => obj.Value.TimeCreated)
                        .Skip(pageInfo.Page * pageInfo.Count)
                        .Take(pageInfo.Count);

                return new
                {
                    total = gameObjectType == GameObjectType.Event
                        ? lookup.Where(obj => ((EventBase) obj.Value).CommonEvent).Count()
                        : lookup.Count(),
                    pageInfo.Page,
                    count = entries.Count(),
                    entries
                };
            }

            return null;
        }

        [Route("{objType}/names")]
        [HttpPost]
        public object Names(string objType)
        {
            GameObjectType gameObjectType;
            if (!Enum.TryParse<GameObjectType>(objType, true, out gameObjectType) ||
                gameObjectType == GameObjectType.Time)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid object type.");
            }

            var lookup = GameObjectTypeExtensions.GetLookup(gameObjectType);

            if (lookup != null)
            {
                
                var entries = gameObjectType == GameObjectType.Event
                    ? lookup.Where(obj => ((EventBase)obj.Value).CommonEvent).Select(t => new { t.Key, t.Value.Name }).ToDictionary(t => t.Key, t => t.Name)
                    : lookup.Select(t => new { t.Key, t.Value.Name }).ToDictionary(t => t.Key, t => t.Name);

                return entries;
            }

            return null;
        }

        [Route("{objType}/{objId:guid}")]
        [HttpGet]
        public object GameObjectById(string objType, Guid objId)
        {
            if (objId == Guid.Empty)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Object not found!");
                return null;
            }

            GameObjectType gameObjectType;
            if (!Enum.TryParse<GameObjectType>(objType, true, out gameObjectType) ||
                gameObjectType == GameObjectType.Time)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid object type.");
            }

            var obj = GameObjectTypeExtensions.GetLookup(gameObjectType)?.Get(objId);

            if (obj != null)
            {
                return obj;
            }

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Object not found!");
        }

        [Route("time")]
        [HttpGet]
        public object Time()
        {
            return TimeBase.GetTimeBase();
        }

    }

}
