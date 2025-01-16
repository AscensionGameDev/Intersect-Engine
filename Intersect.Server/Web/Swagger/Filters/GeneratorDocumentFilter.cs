using Intersect.Server.Web.Swagger.Extensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Intersect.Server.Web.Swagger.Filters;

public sealed class GeneratorDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        OpenApiGeneratorMetadataExtension.Add(
            swaggerDoc.Extensions,
            new OpenApiObject
            {
                {
                    "route_priority", new OpenApiArray
                    {
                        new OpenApiObject
                        {
                            {
                                "group", new OpenApiString("controller")
                            },
                            {
                                "key", new OpenApiString("OAuth")
                            },
                            {
                                "titleKey", new OpenApiString("authentication")
                            },
                        },
                        new OpenApiObject
                        {
                            {
                                "group", new OpenApiString("controller")
                            },
                            {
                                "key", new OpenApiString("Info")
                            },
                            {
                                "titleKey", new OpenApiString("server_info")
                            },
                        },
                        new OpenApiObject
                        {
                            {
                                "group", new OpenApiString("controller")
                            },
                            {
                                "key", new OpenApiString("User")
                            },
                            {
                                "titleKey", new OpenApiString("users")
                            },
                        },
                        new OpenApiObject
                        {
                            {
                                "group", new OpenApiString("controller")
                            },
                            {
                                "key", new OpenApiString("Player")
                            },
                            {
                                "titleKey", new OpenApiString("players")
                            },
                        },
                        new OpenApiObject
                        {
                            {
                                "group", new OpenApiString("controller")
                            },
                            {
                                "key", new OpenApiString("Guild")
                            },
                            {
                                "titleKey", new OpenApiString("guilds")
                            },
                        },
                        new OpenApiObject
                        {
                            {
                                "group", new OpenApiString("controller")
                            },
                            {
                                "key", new OpenApiString("Variables")
                            },
                            {
                                "titleKey", new OpenApiString("server_variables")
                            },
                        },
                        new OpenApiObject
                        {
                            {
                                "group", new OpenApiString("controller")
                            },
                            {
                                "key", new OpenApiString("Chat")
                            },
                            {
                                "titleKey", new OpenApiString("chat")
                            },
                        },
                        new OpenApiObject
                        {
                            {
                                "group", new OpenApiString("controller")
                            },
                            {
                                "key", new OpenApiString("Logs")
                            },
                            {
                                "titleKey", new OpenApiString("logging")
                            },
                        },
                        new OpenApiObject
                        {
                            {
                                "group", new OpenApiString("enum")
                            },
                            {
                                "key", new OpenApiString("AdminAction")
                            },
                            {
                                "titleKey", new OpenApiString("admin_actions")
                            },
                        },
                        new OpenApiObject
                        {
                            {
                                "group", new OpenApiString("controller")
                            },
                            {
                                "key", new OpenApiString("GameObject")
                            },
                            {
                                "titleKey", new OpenApiString("game_objects")
                            },
                        },
                        new OpenApiObject
                        {
                            {
                                "group", new OpenApiString("controller")
                            },
                            {
                                "key", new OpenApiString("RootInfo")
                            },
                            {
                                "ignore", new OpenApiBoolean(true)
                            },
                        },
                        new OpenApiObject
                        {
                            {
                                "group", new OpenApiString("controller")
                            },
                            {
                                "key", new OpenApiString("Demo")
                            },
                            {
                                "ignore", new OpenApiBoolean(true)
                            },
                        },
                        new OpenApiObject
                        {
                            {
                                "group", new OpenApiString("controller")
                            },
                        },
                    }
                },
            }
        );
    }
}