using System.Security.Cryptography;
using System.Text;
using Intersect.Framework;

namespace Intersect.Models;

public sealed class ObjectCacheData<TObject>
{
    private string? _checksum;

    public Id<TObject> Id { get; init; }

    public string Checksum
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_checksum))
            {
                return _checksum;
            }

            _checksum = ObjectCacheKey<TObject>.ComputeChecksum(Data);
            return _checksum;
        }
    }

    public byte[] Data { get; init; }

    public string Version { get; init; }

    public static string? GetCacheFileName(Id<TObject> id)
    {
        var hashData = new byte[32];
        var hashInput = id.Guid.ToByteArray()
            .Concat(Encoding.UTF8.GetBytes((typeof(TObject).FullName ?? typeof(TObject).Name)))
            .ToArray();
        if (!SHA256.TryHashData(hashInput, hashData, out var bytesWritten))
        {
            return default;
        }

        if (bytesWritten != hashData.Length)
        {
            return default;
        }

        var hash = Convert.ToHexString(hashData);
        var fileName = $"{id.Guid}-{hash}";
        return fileName;
    }
}