using Microsoft.OpenApi;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Intersect.Server.Web.Swagger.Extensions;

public sealed class OpenApiGeneratorTagsExtension : IOpenApiExtension
{
    public const string Name = "x-generator-tags";

    public required List<string> Tags { get; init; }

    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));

        writer.WriteStartArray();
        foreach (var tag in Tags)
        {
            writer.WriteValue(tag);
        }
        writer.WriteEndArray();
    }

    public static bool Add(IDictionary<string, IOpenApiExtension> extensions, List<string> tags) =>
        extensions.TryAdd(
            Name,
            new OpenApiGeneratorTagsExtension
            {
                Tags = tags,
            }
        );
}