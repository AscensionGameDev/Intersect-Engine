using System.IO.Hashing;
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

    public static string? ComputeChecksum(ReadOnlySpan<byte> data)
    {
        if (data.Length < 1)
        {
            return default;
        }

        var checksumData = new byte[sizeof(ulong)];
        if (!Crc64.TryHash(data, checksumData, out var bytesWritten) || bytesWritten != sizeof(ulong))
        {
            return default;
        }

        var checksum = Convert.ToBase64String(checksumData);
        return checksum;
    }
}