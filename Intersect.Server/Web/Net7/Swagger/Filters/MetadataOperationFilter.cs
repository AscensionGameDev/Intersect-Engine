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