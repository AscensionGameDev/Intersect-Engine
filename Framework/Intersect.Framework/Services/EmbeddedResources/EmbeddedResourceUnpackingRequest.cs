using System.Reflection;

namespace Intersect.Framework.Services.EmbeddedResources;

public sealed record EmbeddedResourceUnpackingRequest(
    Assembly Assembly,
    string ResourceName,
    bool Overwrite = false,
    string? UnpackedName = default
)
{
    public Assembly Assembly { get; } = Assembly;

    public string ResourceName { get; } = ResourceName;
}