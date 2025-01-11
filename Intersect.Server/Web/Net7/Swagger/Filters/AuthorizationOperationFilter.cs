using System.Reflection;
using Intersect.Server.Web.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Intersect.Server.Web.Swagger.Filters;

public sealed class AuthorizationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        AuthorizeAttribute[] authorizationAttributes = [
            ..context.MethodInfo.GetCustomAttributes<AuthorizeAttribute>(true),
            ..(context.MethodInfo.DeclaringType?.GetCustomAttributes<AuthorizeAttribute>(true) ?? [])
        ];

        if (authorizationAttributes.Length < 1)
        {
            return;
        }

        operation.Security =
        [
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = SecuritySchemes.Bearer, Type = ReferenceType.SecurityScheme,
                        },
                    },
                    authorizationAttributes.SelectMany(
                        authorizationAttribute => authorizationAttribute.Roles?.Split(
                            ',',
                            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                        ) ?? []
                    ).ToList()
                },
            },
        ];
    }
}