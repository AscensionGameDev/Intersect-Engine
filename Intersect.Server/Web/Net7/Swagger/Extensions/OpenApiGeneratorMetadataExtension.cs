using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Intersect.Server.Web.Swagger.Extensions;

public sealed class OpenApiGeneratorMetadataExtension : IOpenApiExtension
{
    public const string Name = "x-generator-metadata";

    public required IOpenApiAny Metadata { get; init; }

    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));

        writer.WriteAny(Metadata);
    }

    public static bool Add(IDictionary<string, IOpenApiExtension> extensions, IOpenApiAny metadata) =>
        extensions.TryAdd(
            Name,
            new OpenApiGeneratorMetadataExtension
            {
                Metadata = metadata,
            }
        );
}