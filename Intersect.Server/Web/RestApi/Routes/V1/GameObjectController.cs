using System.Net;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Models;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.RestApi.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [Route("api/v1/gameobjects")]
    [Authorize]
    public sealed partial class GameObjectController : IntersectController
    {
        [HttpGet("{gameObjectType}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest, "application/json")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound, "application/json")]
        [ProducesResponseType(typeof(DataPage<IDatabaseObject>), (int)HttpStatusCode.OK, "application/json")]
        public IActionResult List(GameObjectType gameObjectType, [FromQuery] PagingInfo pageInfo)
        {
            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.Count = Math.Max(Math.Min(pageInfo.Count, 100), 5);

            if (!gameObjectType.TryGetLookup(out var lookup))
            {
                return BadRequest($"Invalid {nameof(GameObjectType)} '{gameObjectType}'");
            }

            var baseEnumerable = gameObjectType == GameObjectType.Event
                ? lookup.Where(obj => ((EventBase)obj.Value).CommonEvent).Select(pair => pair.Value)
                : lookup.Values;

            var entries = baseEnumerable.OrderBy(obj => obj.Name)
                .Skip(pageInfo.Page * pageInfo.Count)
                .Take(pageInfo.Count)
                .ToArray();

            return Ok(
                new DataPage<IDatabaseObject>
                {
                    Total = gameObjectType == GameObjectType.Event
                        ? lookup.Count(obj => ((EventBase)obj.Value).CommonEvent)
                        : lookup.Count,
                    Page = pageInfo.Page,
                    Count = entries.Length,
                    Values = entries,
                }
            );
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
