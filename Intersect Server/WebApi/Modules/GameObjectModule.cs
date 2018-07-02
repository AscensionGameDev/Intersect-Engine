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
using Nancy.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ServerOptions = Intersect.Options;

namespace Intersect.Server.WebApi.Modules
{
    using Sync = Func<dynamic, Response>;

    public class GameObjectModule : ServerModule
    {
        public GameObjectModule() : base("/objects")
        {
            Get("/{type}/{guid:guid}", (Sync)Get_Type_Guid);
            Patch("/{type}/{guid:guid}", (Sync)Patch_Type_Guid);
            Get("/{type}/stats", (Sync)Get_Type_Stats);
        }

        private Response Get_Type_Stats(dynamic parameters)
        {
            try
            {
                GameObjectType type = GameObjectTypeUtils.TypeFromName(parameters.type);
                var lookup = type.GetLookup();
                return Response.AsJson(new
                {
                    type = type.ToString(),
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
                    var json = JsonConvert.SerializeObject(gameObject);
                    var jsonBytes = Encoding.UTF8.GetBytes(json);
                    return new Response
                    {
                        StatusCode = HttpStatusCode.OK,
                        ContentType = "application/json",
                        Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
                    };
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

        private Response Patch_Type_Guid(dynamic parameters)
        {
            try
            {
                GameObjectType type = GameObjectTypeUtils.TypeFromName(parameters.type);
                if (Guid.TryParse(parameters.guid, out Guid guid))
                {
                    var gameObject = type.GetLookup().Get(guid);

                    var body = Request.Body.AsString();
                    JsonConvert.PopulateObject(body, gameObject);

                    var json = JsonConvert.SerializeObject(gameObject);
                    var jsonBytes = Encoding.UTF8.GetBytes(json);
                    return new Response
                    {
                        StatusCode = HttpStatusCode.OK,
                        ContentType = "application/json",
                        Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
                    };
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
