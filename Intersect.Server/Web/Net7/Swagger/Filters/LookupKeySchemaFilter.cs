using Intersect.Server.Web.RestApi.Payloads;
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

        var foundId = false;
        var foundName = false;
        foreach (var oneOfSchema in schema.OneOf)
        {
            if (oneOfSchema.Type != "string")
            {
                continue;
            }

            if (oneOfSchema.Format == "uuid")
            {
                foundId = true;
            }
            else if (string.IsNullOrWhiteSpace(oneOfSchema.Format))
            {
                foundName = true;
            }
        }

        if (!foundId)
        {
            schema.OneOf.Add(new OpenApiSchema { Type = "string", Format = "uuid" });
        }

        if (!foundName)
        {
            schema.OneOf.Add(new OpenApiSchema { Type = "string", Format = "username"});
        }
    }
}