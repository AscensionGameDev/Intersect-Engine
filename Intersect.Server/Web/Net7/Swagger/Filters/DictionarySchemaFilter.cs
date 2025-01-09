using Intersect.Reflection;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Intersect.Server.Web.Swagger.Filters;

public sealed class OpenApiKeyTypeExtension : IOpenApiExtension
{
    public const string Name = "x-key-type";

    public required OpenApiSchema KeyType { get; init; }

    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));

        KeyType.Reference.Serialize(writer, specVersion);
    }

    public static bool Add(IDictionary<string, IOpenApiExtension> extensions, OpenApiSchema keyType) =>
        extensions.TryAdd(
            Name,
            new OpenApiKeyTypeExtension
            {
                KeyType = keyType,
            }
        );
}

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