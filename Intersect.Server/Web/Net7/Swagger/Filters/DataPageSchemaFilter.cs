using Intersect.Framework.Reflection;
using Intersect.Server.Web.RestApi.Types;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Intersect.Server.Web.Swagger.Filters;

public sealed class DataPageSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var contextType = context.Type;
        if (!typeof(DataPage<>).ExtendedBy(contextType))
        {
            return;
        }

        var generator = context.SchemaGenerator;
        var repository = context.SchemaRepository;

        if (!repository.TryLookupByType(typeof(IDataPage<>), out var genericTypeDefinitionSchema))
        {
            genericTypeDefinitionSchema = generator.GenerateSchema(typeof(IDataPage<>), repository);
        }

        if (contextType.IsGenericType)
        {
            schema.Properties = new Dictionary<string, OpenApiSchema>();
            schema.AllOf =
            [
                new OpenApiSchema
                {
                    Reference = genericTypeDefinitionSchema.Reference ??
                                throw new InvalidOperationException(
                                    $"Missing schema reference for {typeof(DataPage<>).Name}"
                                ),
                },
            ];
            return;
        }

        contextType.ToString();
    }
}