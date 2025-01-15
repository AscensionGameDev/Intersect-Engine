using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Intersect.Framework.Resources;
using Intersect.Server.Collections.Indexing;
using Intersect.Server.Web.RestApi.Types;
using Intersect.Server.Web.Swagger.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Intersect.Server.Web.Swagger.Filters;

public sealed class MetadataOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo is { } methodInfo)
        {
            var description = operation.Description;
            if (string.IsNullOrWhiteSpace(description))
            {
                var descriptionAttribute = methodInfo.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
                var endpointDescriptionAttribute = methodInfo.GetCustomAttributes<EndpointDescriptionAttribute>().FirstOrDefault();
                description = endpointDescriptionAttribute?.Description ?? descriptionAttribute?.Description;
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var resourceString = OpenAPIResources.ResourceManager.GetStringWithFallback(description, CultureInfo.CurrentCulture);
                if (!string.IsNullOrWhiteSpace(resourceString))
                {
                    description = resourceString;
                }
            }

            operation.Description = description;

            var summary = operation.Summary;
            if (string.IsNullOrWhiteSpace(summary))
            {
                var endpointSummaryAttribute = methodInfo.GetCustomAttributes<EndpointSummaryAttribute>().FirstOrDefault();
                summary = endpointSummaryAttribute?.Summary;
            }

            if (!string.IsNullOrWhiteSpace(summary))
            {
                var resourceString = OpenAPIResources.ResourceManager.GetStringWithFallback(summary, CultureInfo.CurrentCulture);
                if (!string.IsNullOrWhiteSpace(resourceString))
                {
                    summary = resourceString;
                }
            }

            operation.Summary = summary;
        }

        var api = context.ApiDescription;
        List<string> tags = [];

        if (!string.IsNullOrWhiteSpace(api.HttpMethod))
        {
            tags.Add($"method:{api.HttpMethod}");
        }

        if (!string.IsNullOrWhiteSpace(api.GroupName))
        {
            tags.Add($"group:{api.GroupName}");
        }

        if (api.ActionDescriptor is ControllerActionDescriptor controllerDescriptor)
        {
            tags.Add($"action:{controllerDescriptor.ActionName}");
            tags.Add($"controller:{controllerDescriptor.ControllerName}");
        }

        foreach (var parameterDescriptor in api.ActionDescriptor.Parameters)
        {
            if (parameterDescriptor.ParameterType.IsEnum)
            {
                tags.Add($"enum:{parameterDescriptor.ParameterType.Name}");
            }

            if (parameterDescriptor.ParameterType == typeof(PagingInfo))
            {
                tags.Add("option:paged");
            }

            if (parameterDescriptor.ParameterType == typeof(LookupKey))
            {
                tags.Add("option:lookup");
            }
        }

        if (!OpenApiGeneratorTagsExtension.Add(operation.Extensions, tags))
        {
            throw new InvalidOperationException(
                $"Failed to add generator tags to {api.HttpMethod ?? "N/A"} {api.RelativePath}"
            );
        }
    }
}