using System.Reflection;
using Intersect.Enums;
using Intersect.Framework.Reflection;
using Intersect.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Intersect.Server.Web.Swagger.Filters;

public sealed class GameObjectTypeSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var generator = context.SchemaGenerator;
        var repository = context.SchemaRepository;

        var contextType = context.Type;
        if (contextType == typeof(IDatabaseObject))
        {
            schema.Discriminator = new OpenApiDiscriminator
            {
                PropertyName = nameof(IDatabaseObject.Type),
                Mapping = Enum.GetValues<GameObjectType>().ToDictionary(
                    gameObjectType => gameObjectType.ToString(),
                    gameObjectType => gameObjectType.GetObjectType().Name
                ),
            };

            foreach (var gameObjectType in Enum.GetValues<GameObjectType>())
            {
                var subtype = gameObjectType.GetObjectType();
                schema.Discriminator.Mapping[gameObjectType.ToString()] = subtype.Name;
                if (!repository.TryLookupByType(subtype, out _))
                {
                    _ = generator.GenerateSchema(subtype, repository);
                }
            }
        }

        if (!contextType.Extends<IDatabaseObject>() && !contextType.ExtendedBy<IDatabaseObject>() &&
            contextType != typeof(IDatabaseObject) && contextType != typeof(IFolderable))
        {

            return;
        }

        List<OpenApiReference> references = [];
        var baseType = contextType.BaseType;
        if (baseType != default)
        {
            if (contextType.IsGenericType)
            {
                var genericTypeDefinition = contextType.GetGenericTypeDefinition();
                if (genericTypeDefinition != contextType)
                {
                    baseType = genericTypeDefinition;
                }
            }

            if (baseType is { IsGenericType: true, IsGenericTypeDefinition: false })
            {
                baseType = baseType.GetGenericTypeDefinition();
            }

            if (baseType != typeof(object))
            {
                if (!repository.TryLookupByType(baseType, out var baseTypeSchema))
                {
                    baseTypeSchema = generator.GenerateSchema(baseType, repository);
                }

                references.Add(baseTypeSchema.Reference);
            }
        }

        foreach (var interfaceType in contextType.GetUniqueInterfaces())
        {
            if (!repository.TryLookupByType(interfaceType, out _))
            {
                _ = generator.GenerateSchema(interfaceType, repository);
            }

            references.Add(new OpenApiReference
            {
                Id = interfaceType.Name,
                Type = ReferenceType.Schema,
            });
        }

        var nonInheritedProperties = schema.Properties.Where(
            kvp =>
            {
                var (propertyName, _) = kvp;
                if (!contextType.TryFindProperty(propertyName, out var propertyInfo))
                {
                    throw new InvalidOperationException(
                        $"Missing {nameof(PropertyInfo)} for {contextType.GetName(qualified: true)}.{propertyName}"
                    );
                }
                return contextType.IsOwnProperty(propertyInfo);
            }
        ).ToDictionary();

        if (references.Count <= 0)
        {
            return;
        }

        schema.AllOf =
        [
            ..references.Select(reference => new OpenApiSchema { Reference = reference, }),
        ];

        if (nonInheritedProperties.Count > 0)
        {
            schema.AllOf.Add(
                new OpenApiSchema
                {
                    Type = "object", Properties = nonInheritedProperties,
                }
            );
        }

        schema.Properties = new Dictionary<string, OpenApiSchema>();
    }
}