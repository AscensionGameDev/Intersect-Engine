using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Server.Web.RestApi.Payloads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [Route("api/v1/gameobjects")]
    [Authorize]
    public sealed partial class GameObjectController : IntersectController
    {
        [HttpGet("{gameObjectType}")]
        public object List(GameObjectType gameObjectType, [FromQuery] PagingInfo pageInfo)
        {
            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.Count = Math.Max(Math.Min(pageInfo.Count, 100), 5);

            var lookup = GameObjectTypeExtensions.GetLookup(gameObjectType);

            if (lookup != null)
            {
                var entries = gameObjectType == GameObjectType.Event
                    ? lookup.Where(obj => ((EventBase) obj.Value).CommonEvent)
                        .OrderBy(obj => obj.Value.Name)
                        .Skip(pageInfo.Page * pageInfo.Count)
                        .Take(pageInfo.Count)
                    : lookup.OrderBy(obj => obj.Value.Name)
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

            return BadRequest();
        }

        [HttpGet("{gameObjectType}/names")]
        public object Names(GameObjectType gameObjectType)
        {
            var lookup = GameObjectTypeExtensions.GetLookup(gameObjectType);

            if (lookup != null)
            {
                var entries = gameObjectType == GameObjectType.Event
                    ? lookup.Where(obj => ((EventBase)obj.Value).CommonEvent).Select(t => new { t.Key, t.Value.Name }).ToDictionary(t => t.Key, t => t.Name)
                    : lookup.Select(t => new { t.Key, t.Value.Name }).ToDictionary(t => t.Key, t => t.Name);

                return entries;
            }

            return BadRequest();
        }

        [HttpGet("{gameObjectType}/{objId:guid}")]
        public object GameObjectById(GameObjectType gameObjectType, Guid objId)
        {
            if (objId == Guid.Empty)
            {
                return BadRequest(@"Object not found!");
            }

            var obj = GameObjectTypeExtensions.GetLookup(gameObjectType)?.Get(objId);

            if (obj != null)
            {
                return obj;
            }

            return NotFound(@"Object not found!");
        }

        [HttpGet("time")]
        public object Time()
        {
            return TimeBase.GetTimeBase();
        }
    }
}
