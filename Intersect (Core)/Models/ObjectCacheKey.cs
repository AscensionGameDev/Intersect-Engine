using Intersect.Framework;
using MessagePack;

namespace Intersect.Models;

[MessagePackObject(true)]
public record struct ObjectCacheKey<TObject>(
    [property: Key(0)] Id<TObject> Id,
    [property: Key(1)] string? Checksum = default,
    [property: Key(2)] string? Version = default
)
{
    [SerializationConstructor]
    public ObjectCacheKey() : this(default)
    {
    }
}