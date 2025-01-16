using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Intersect.Server.Web.Swagger.Filters;

public sealed class PolymorphicDocumentFilter<TBase> : PolymorphicDocumentFilter<TBase, string> where TBase : class
{
    public PolymorphicDocumentFilter(string discriminatorPropertyName) : base(discriminatorPropertyName)
    {
    }
}

public class PolymorphicDocumentFilter<TBase, TDiscriminator> : IDocumentFilter where TBase : class
{
    private readonly string _discriminatorPropertyName;
    private readonly Dictionary<string, Type> _subtypes = [];

    public PolymorphicDocumentFilter(string discriminatorPropertyName)
    {
        _discriminatorPropertyName = discriminatorPropertyName;
    }

    public PolymorphicDocumentFilter<TBase, TDiscriminator> WithSubtype<TSubtype>(TDiscriminator discriminatorValue)
        where TSubtype : TBase
    {
        if (typeof(TSubtype).IsAbstract || typeof(TSubtype).IsGenericTypeDefinition)
        {
            throw new ArgumentException(
                $"Invalid subtype '{typeof(TSubtype).FullName ?? typeof(TSubtype).Name}'",
                nameof(TSubtype)
            );
        }

        var discriminatorValueString = discriminatorValue?.ToString();
        if (discriminatorValueString == null)
        {
            throw new ArgumentException("Discriminator value resolved to null string", nameof(discriminatorValue));
        }

        _subtypes.Add(discriminatorValueString, typeof(TSubtype));
        return this;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var schemaGenerator = context.SchemaGenerator;
        var schemaRepository = context.SchemaRepository;

        var baseType = typeof(TBase);
        if (!schemaRepository.TryLookupByType(baseType, out var schema))
        {
            schema = schemaGenerator.GenerateSchema(baseType, schemaRepository);
        }

        schema.Discriminator ??= new OpenApiDiscriminator
        {
            PropertyName = _discriminatorPropertyName,
        };

        schema.Required.Add(schema.Discriminator.PropertyName);
        if (!schema.Properties.ContainsKey(_discriminatorPropertyName))
        {
            schema.Properties.Add(
                _discriminatorPropertyName,
                new OpenApiSchema
                {
                    Type = "string",
                }
            );
        }

        foreach (var (subtypeDiscriminatorValue, subtype) in _subtypes)
        {
            if (!schemaRepository.TryLookupByType(subtype, out var subtypeSchema))
            {
                subtypeSchema = schemaGenerator.GenerateSchema(subtype, schemaRepository);
            }

            if (!schema.Discriminator.Mapping.TryAdd(subtypeDiscriminatorValue, subtypeSchema.Reference.ReferenceV3))
            {
                throw new InvalidOperationException(
                    $"Duplicate subtype discriminator value: '{subtypeDiscriminatorValue}'"
                );
            }
        }
    }

    public PolymorphicSchemaFilter<TBase> CreateSchemaFilter() => new(_discriminatorPropertyName, _subtypes.Values);
}