using Intersect.Server.Web.RestApi.Payloads;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Intersect.Server.Web.Swagger.Filters;

public sealed class LookupKeyDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (!context.SchemaRepository.TryLookupByType(typeof(LookupKey), out var schema))
        {
            schema = context.SchemaGenerator.GenerateSchema(typeof(LookupKey), context.SchemaRepository);
        }

        schema.OneOf =
        [
            new OpenApiSchema
            {
                Type = "string", Format = "uuid",
            },
            new OpenApiSchema
            {
                Type = "string", Format = "username",
            },
        ];

        // schema.Type = default;
    }
}