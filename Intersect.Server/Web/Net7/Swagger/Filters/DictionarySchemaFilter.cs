using Intersect.Reflection;
using Intersect.Server.Web.Swagger.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Intersect.Server.Web.Swagger.Filters;

public sealed class DictionarySchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!typeof(IReadOnlyDictionary<,>).ExtendedBy(context.Type))
        {
            return;
        }

        var generator = context.SchemaGenerator;
        var repository = context.SchemaRepository;

        var keyType = context.Type.GetGenericArguments().First();
        if (!keyType.IsEnum)
        {
            return;
        }

        if (!repository.TryLookupByType(keyType, out var keyTypeSchema))
        {
            keyTypeSchema = generator.GenerateSchema(keyType, repository);
        }

        _ = OpenApiKeyTypeExtension.Add(schema.Extensions, keyTypeSchema);
    }
}