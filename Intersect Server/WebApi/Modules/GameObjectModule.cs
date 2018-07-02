using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.General;
using Intersect.Utilities;
using Nancy;
using Newtonsoft.Json.Linq;

using ServerOptions = Intersect.Options;

namespace Intersect.Server.WebApi.Modules
{
    using Sync = Func<dynamic, Response>;

    public class GameObjectModule : ServerModule
    {
        public GameObjectModule() : base("/objects")
        {
            Get("/{type}/stats", (Sync)Get_Type_Guid);
            Get("/{type}/{guid}", (Sync)Get_Type_Guid);
        }

        private Response Get_Type_Stats(dynamic parameters)
        {
            try
            {
                GameObjectType type = GameObjectTypeUtils.TypeFromName(parameters.type);
                var lookup = type.GetLookup();
                return Response.AsJson(new
                {
                    count = lookup.Count
                });
            }
            catch (GameObjectTypeException exception)
            {
                var response = Response.AsJson(new { message = exception.Message });
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
        }

        private Response Get_Type_Guid(dynamic parameters)
        {
            try
            {
                GameObjectType type = GameObjectTypeUtils.TypeFromName(parameters.type);
                if (Guid.TryParse(parameters.guid, out Guid guid))
                {
                    var gameObject = type.GetLookup().Get(guid);
                }

                var response = Response.AsJson(new { message = $"Invalid guid '{guid}'." });
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
            catch (GameObjectTypeException exception)
            {
                var response = Response.AsJson(new { message = exception.Message });
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
        }
    }
}
