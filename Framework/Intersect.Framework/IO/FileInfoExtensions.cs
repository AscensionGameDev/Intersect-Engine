using System.Diagnostics.CodeAnalysis;
using System.IO.Hashing;

namespace Intersect.Framework.IO;

public static class FileInfoExtensions
{
    public static bool TryComputeChecksum(this FileInfo fileInfo, [NotNullWhen(true)] out string? checksum)
    {
        if (!fileInfo.Exists)
        {
            checksum = null;
            return false;
        }

        Crc64 algorithm = new();

        using var fileStream = fileInfo.OpenRead();
        algorithm.Append(fileStream);

        var data = algorithm.GetHashAndReset();
        if (data.Length < 1)
        {
            checksum = null;
            return false;
        }

        checksum = Convert.ToBase64String(data);
        if (!string.IsNullOrWhiteSpace(checksum))
        {
            return true;
        }

        checksum = null;
        return false;

    }
}