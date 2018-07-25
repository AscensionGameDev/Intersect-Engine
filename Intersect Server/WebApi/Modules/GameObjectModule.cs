using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Models;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Networking;
using Intersect.Utilities;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Intersect.Server.WebApi.Modules
{
    using Sync = Func<dynamic, Response>;

    public class GameObjectModule : ServerModule
    {
#if DEBUG
        protected override bool Secured => false;
#endif

        public GameObjectModule() : base("/objects")
        {
            Get("/{type}", (Sync)Get_Type_List);

            Get("/{type}/{guid:guid}", (Sync)Get_Type_Guid);
            Delete("/{type}/{guid:guid}", (Sync)Delete_Type_Guid);
            Patch("/{type}/{guid:guid}", (Sync)Patch_Type_Guid);
            Post("/{type}", (Sync)Post_Type);

#if DEBUG
            Get("/{type}/{guid:guid}/delete", (Sync)Debug_Type_Guid_Delete);
            Get("/{type}/create", (Sync)Debug_Type_Create);
#endif

            Get("/{type}/stats", (Sync)Get_Type_Stats);
        }

#if DEBUG
        private Response Debug_Type_Guid_Delete(dynamic parameters)
        {
            if (Request.Query.all != true)
            {
                return Delete_Type_Guid(parameters);
            }

            try
            {
                GameObjectType type = GameObjectTypeUtils.TypeFromName(parameters.type);
                var lookup = type.GetLookup();

                lookup.ValueList.ForEach(gameObject => DeleteObject(type, gameObject));

                return Response.AsJson(new
                {
                    success = true,
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

        private Response Debug_Type_Create(dynamic parameters)
        {

            try
            {
                GameObjectType type = GameObjectTypeUtils.TypeFromName(parameters.type);

                var count = Request.Query.count.HasValue ? MathHelper.Clamp(Request.Query.count, 0, 100) : 1;
                var objects = new IDatabaseObject[count];
                for (var i = 0; i < count; ++i)
                {
                    objects[i] = LegacyDatabase.AddGameObject(type);
                }

                return Response.AsJson(new
                {
                    success = true,
                    count,
                    objects = objects
                });
            }
            catch (GameObjectTypeException exception)
            {
                var response = Response.AsJson(new { message = exception.Message });
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
        }
#endif

        private Response Get_Type_List(dynamic parameters)
        {
            try
            {
                GameObjectType type = GameObjectTypeUtils.TypeFromName(parameters.type);
                var lookup = type.GetLookup();
                if (lookup.IsEmpty)
                {
                    return Response.AsJson(new
                    {
                        totalCount = 0,
                        objects = new object[0]
                    });
                }

                var pageSize = (int)MathHelper.Clamp(Request.Query.pageSize, 10, 100);
                var pageCount = (int)Math.Ceiling(lookup.Count / (double)pageSize);
                var page = (int)MathHelper.Clamp(Request.Query.page, 0, pageCount - 1);

                var count = Math.Min(pageSize, lookup.Count - page * pageSize);

                var json = JsonConvert.SerializeObject(new
                {
                    totalCount = lookup.Count,
                    count,
                    page,
                    pageSize,
                    objects = lookup.ValueList.GetRange(page * pageSize, count)
                });
                var jsonBytes = Encoding.UTF8.GetBytes(json);
                return new Response
                {
                    StatusCode = HttpStatusCode.OK,
                    ContentType = "application/json",
                    Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
                };
            }
            catch (GameObjectTypeException exception)
            {
                var response = Response.AsJson(new { message = exception.Message });
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
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

        private static bool DeleteObject(GameObjectType type, IDatabaseObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }

            switch (type)
            {
                //if Item or Resource, kill all global entities of that kind
                case GameObjectType.Item:
                    Globals.KillItemsOf((ItemBase)gameObject);
                    break;
                case GameObjectType.Resource:
                    Globals.KillResourcesOf((ResourceBase)gameObject);
                    break;
                case GameObjectType.Npc:
                    Globals.KillNpcsOf((NpcBase)gameObject);
                    break;
            }

            LegacyDatabase.DeleteGameObject(gameObject);
            LegacyDatabase.SaveGameDatabase();
            PacketSender.SendGameObjectToAll(gameObject, true);

            return true;
        }

        private Response Delete_Type_Guid(dynamic parameters)
        {
            try
            {
                GameObjectType type = GameObjectTypeUtils.TypeFromName(parameters.type);
                if (Guid.TryParse(parameters.guid, out Guid guid))
                {
                    var gameObject = type.GetLookup().Get(guid);
                    if (DeleteObject(type, gameObject))
                    {
                        return Response.AsJson(new { success = true });
                    }
                }

                var response = Response.AsJson(new { success = false, message = $"Invalid guid '{guid}'." });
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
                    try
                    {
                        JsonConvert.PopulateObject(body, gameObject);
                    }
                    catch (Exception exception)
                    {
                        return Response.AsJson(new
                        {
                            message = exception.Message
                        }, HttpStatusCode.BadRequest);
                    }

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

        private Response Post_Type(dynamic parameters)
        {
            try
            {
                GameObjectType type = GameObjectTypeUtils.TypeFromName(parameters.type);

                var body = Request.Body.AsString();
                if (string.IsNullOrWhiteSpace(body))
                {
                    body = "{}";
                }

                try
                {
                    var gameObject = type.CreateNew();

                    JsonConvert.PopulateObject(body, gameObject);
                    LegacyDatabase.AddGameObject(type, gameObject);
                    PacketSender.SendGameObjectToAll(gameObject);

                    var json = JsonConvert.SerializeObject(gameObject);
                    var jsonBytes = Encoding.UTF8.GetBytes(json);
                    return new Response
                    {
                        StatusCode = HttpStatusCode.OK,
                        ContentType = "application/json",
                        Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
                    };
                }
                catch (Exception exception)
                {
                    return Response.AsJson(new
                    {
                        message = exception.Message
                    }, HttpStatusCode.BadRequest);
                }
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
