using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Intersect.Server.Web.Swagger.Filters;

public class PolymorphicSchemaFilter<TBase> : ISchemaFilter where TBase : class
{
    private readonly string _discriminatorPropertyName;
    private readonly HashSet<Type> _subtypes = [];

    public PolymorphicSchemaFilter(string discriminatorPropertyName, IEnumerable<Type> subtypes)
    {
        _discriminatorPropertyName = discriminatorPropertyName;
        foreach (var subtype in subtypes)
        {
            if (subtype.IsAbstract || subtype.IsGenericTypeDefinition || !subtype.IsAssignableTo(typeof(TBase)))
            {
                throw new ArgumentException(
                    $"Tried to create schema filter for {typeof(TBase).FullName} with {subtype.FullName ?? subtype.Name} which is not a valid subtype."
                );
            }

            _subtypes.Add(subtype);
        }
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!_subtypes.Contains(context.Type))
        {
            return;
        }

        if (schema.AllOf.Count > 0)
        {
            var foundDiscriminator = schema.AllOf.Any(
                aos => aos.Reference == default && aos.Properties.Remove(_discriminatorPropertyName)
            );

            if (foundDiscriminator)
            {
                return;
            }

            throw new InvalidOperationException("No discriminator found?");
        }

        var clonedSchema = new OpenApiSchema
        {
            Properties = schema.Properties.Where(kvp => kvp.Key != _discriminatorPropertyName).ToDictionary(),
            Required = schema.Required,
            Type = schema.Type,
        };

        if (context.SchemaRepository.TryLookupByType(typeof(TBase), out var baseSchema))
        {
            schema.AllOf = new List<OpenApiSchema>
            {
                new()
                {
                    Reference = new OpenApiReference(baseSchema.Reference),
                },
                clonedSchema,
            };
        }

        schema.Properties = new Dictionary<string, OpenApiSchema>();
    }
}