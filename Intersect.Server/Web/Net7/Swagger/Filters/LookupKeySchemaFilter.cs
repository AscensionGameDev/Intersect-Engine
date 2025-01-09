using Intersect.Server.Localization;
using Intersect.Server.Web.RestApi.Payloads;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Intersect.Server.Web.Swagger.Filters;

public sealed class LookupKeySchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(LookupKey))
        {
            return;
        }

        schema.Type = "string";
        schema.Description = "An id or a name";

        // TODO: Multiple examples? It doesn't look like the C# API exposes this but it exists according to the Swagger documentation
        // Example = new OpenApiString($"test or {new Guid("01234567-89ab-cdef-0123-456789abcdef")}"),
        schema.Example = new OpenApiString("test");

        schema.OneOf =
        [
            new OpenApiSchema
            {
                Type = "string", Format = "uuid",
            },
            new OpenApiSchema
            {
                Type = "string", Format = "username", Pattern = Strings.Regex.Username,
            },
        ];

        schema.Properties = new Dictionary<string, OpenApiSchema>();

        // var foundId = false;
        // var foundName = false;
        // foreach (var oneOfSchema in schema.OneOf)
        // {
        //     if (oneOfSchema.Type != "string")
        //     {
        //         continue;
        //     }
        //
        //     if (oneOfSchema.Format == "uuid")
        //     {
        //         foundId = true;
        //     }
        //     else if (string.IsNullOrWhiteSpace(oneOfSchema.Format))
        //     {
        //         foundName = true;
        //     }
        // }
        //
        // if (!foundId)
        // {
        //     schema.OneOf.Add(new OpenApiSchema { Type = "string", Format = "uuid" });
        // }
        //
        // if (!foundName)
        // {
        //     schema.OneOf.Add(new OpenApiSchema { Type = "string", Format = "username"});
        // }
    }
}