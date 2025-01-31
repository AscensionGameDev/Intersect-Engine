using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Intersect.Framework.Resources;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Intersect.Server.Web.Swagger.Filters;

public sealed class MetadataSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var description = schema.Description;
        if (string.IsNullOrWhiteSpace(description))
        {
            var descriptionAttribute = context.Type.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
            description = descriptionAttribute?.Description;
        }

        // ReSharper disable once InvertIf
        if (!string.IsNullOrWhiteSpace(description))
        {
            var resourceString = OpenAPIResources.ResourceManager.GetStringWithFallback(description, CultureInfo.CurrentUICulture);
            if (!string.IsNullOrWhiteSpace(resourceString))
            {
                description = resourceString;
            }
        }

        schema.Description = description;

        var title = schema.Title;
        if (string.IsNullOrWhiteSpace(title))
        {
            var typeName = context.Type.Name;
            if (context.Type.IsGenericTypeDefinition)
            {
                title = $"{typeName[..typeName.IndexOf('`')]}<{string.Join(", ", context.Type.GetGenericArguments().Select(argType => argType.Name))}>";
            }
        }

        schema.Title = title;
    }
}