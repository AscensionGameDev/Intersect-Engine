using System.IO.Hashing;

namespace Intersect.Framework.Core.AssetManagement;

public sealed record UpdateManifestFile(string Path, string Checksum, long Size)
    : IComparable<UpdateManifestFile>
{
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public int Segments => Path.Count(c => c == '/');

    public int CompareTo(UpdateManifestFile other)
    {
        var segmentDifference = Segments - other.Segments;
        return segmentDifference != 0 ? segmentDifference : string.CompareOrdinal(Path, other.Path);
    }

    public static UpdateManifestFile From(FileInfo fileInfo, string rootPath)
    {
        var relativeFilePath = System.IO.Path.GetRelativePath(rootPath, fileInfo.FullName);
        var checksum = ComputeChecksum(fileInfo);
        return new UpdateManifestFile(relativeFilePath, checksum, fileInfo.Length);
    }

    public static string ComputeChecksum(FileInfo fileInfo, NonCryptographicHashAlgorithm algorithm = default)
    {
        if (fileInfo == default)
        {
            throw new ArgumentNullException(nameof(fileInfo));
        }

        algorithm ??= new Crc64();

        using var fileStream = fileInfo.OpenRead();
        algorithm.Append(fileStream);

        var data = algorithm.GetHashAndReset();
        return Convert.ToBase64String(data);
    }
}