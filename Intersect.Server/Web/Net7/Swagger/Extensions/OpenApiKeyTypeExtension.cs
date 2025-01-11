using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Intersect.Server.Web.Swagger.Extensions;

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