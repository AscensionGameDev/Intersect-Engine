using System.Net;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Models;
using Intersect.Server.Web.Http;
using Intersect.Server.Web.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.Controllers.Api.V1
{
    [Route("api/v1/gameobjects")]
    [Authorize]
    public sealed partial class GameObjectController : IntersectController
    {
        [HttpGet("{gameObjectType}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(DataPage<IDatabaseObject>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult List(GameObjectType gameObjectType, [FromQuery] PagingInfo pageInfo)
        {
            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.PageSize = Math.Max(Math.Min(pageInfo.PageSize, 100), 5);

            if (!gameObjectType.TryGetLookup(out var lookup))
            {
                return BadRequest($"Invalid {nameof(GameObjectType)} '{gameObjectType}'");
            }

            var baseEnumerable = gameObjectType == GameObjectType.Event
                ? lookup.Where(obj => ((EventBase)obj.Value).CommonEvent).Select(pair => pair.Value)
                : lookup.Values;

            var entries = baseEnumerable.OrderBy(obj => obj.Name)
                .Skip(pageInfo.Page * pageInfo.PageSize)
                .Take(pageInfo.PageSize)
                .ToArray();

            var total = gameObjectType == GameObjectType.Event
                ? lookup.Count(obj => ((EventBase)obj.Value).CommonEvent)
                : lookup.Count;

            return Ok(
                new DataPage<IDatabaseObject>(
                    Total: total,
                    Page: pageInfo.Page,
                    PageSize: pageInfo.PageSize,
                    Count: entries.Length,
                    Values: entries
                )
            );
        }

        [HttpGet("{gameObjectType}/names")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(DataPage<string>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult Names(GameObjectType gameObjectType)
        {
            if (!gameObjectType.TryGetLookup(out var lookup))
            {
                return BadRequest($"Invalid {nameof(GameObjectType)} '{gameObjectType}'");
            }

            var descriptors = lookup.Values.AsEnumerable();
            if (gameObjectType == GameObjectType.Event)
            {
                descriptors = descriptors.OfType<EventBase>().Where(descriptor => descriptor.CommonEvent);
            }

            var descriptorNames = descriptors.Select(descriptor => descriptor.Name).ToArray();

            return Ok(
                new DataPage<string>(
                    Total: descriptorNames.Length,
                    Page: 0,
                    PageSize: descriptorNames.Length,
                    Count: descriptorNames.Length,
                    descriptorNames
                )
            );
        }

        [HttpGet("{gameObjectType}/{objectId:guid}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(IDatabaseObject), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult GameObjectById(GameObjectType gameObjectType, Guid objectId)
        {
            if (objectId == default)
            {
                return BadRequest($@"Invalid id '{objectId}'");
            }

            if (!gameObjectType.TryGetLookup(out var lookup))
            {
                return BadRequest($"Invalid {nameof(GameObjectType)} '{gameObjectType}'");
            }

            return !lookup.TryGetValue(objectId, out var gameObject)
                ? NotFound($"No {gameObjectType} with id '{objectId}'")
                : Ok(gameObject);
        }

        [HttpGet("time")]
        [ProducesResponseType(typeof(TimeBase), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult Time() => Ok(TimeBase.GetTimeBase());
    }
}
