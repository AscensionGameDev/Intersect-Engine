using Intersect.Collections;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Web.RestApi.Attributes;
using Intersect.Server.Web.RestApi.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

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
            if (!Enum.TryParse<GameObjectType>(objType, true, out gameObjectType) || gameObjectType == GameObjectType.Time)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid object type.");
            }

            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.Count = Math.Max(Math.Min(pageInfo.Count, 100), 5);

            var lookup = GameObjectTypeExtensions.GetLookup(gameObjectType);

            if (lookup != null)
            {
                return lookup.OrderBy(user => user.Value.TimeCreated).Skip(pageInfo.Page * pageInfo.Count).Take(pageInfo.Count);
            }

            return null;
        }

        [Route("{objType}/{objId:guid}")]
        [HttpGet]
        public object GameObjectById(string objType, Guid objId)
        {
            if (objId == Guid.Empty)
            {
                return null;
            }

            GameObjectType gameObjectType;
            if (!Enum.TryParse<GameObjectType>(objType, true, out gameObjectType) || gameObjectType == GameObjectType.Time)
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
