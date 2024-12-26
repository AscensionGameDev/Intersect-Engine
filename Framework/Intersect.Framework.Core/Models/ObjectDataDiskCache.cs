using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Intersect.Configuration;
using Intersect.Framework;
using Intersect.Logging;

namespace Intersect.Models;

public static class ObjectDataDiskCache<TObject>
{ private static string RootCacheDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        ".intersect",
        (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName().Name ??
        throw new InvalidOperationException(),
        ConfigurationHelper.CacheName
    );

    private static SymmetricAlgorithm CreateCryptoServiceProvider(string identifier, out string nonce)
    {
        var nonceData = RandomNumberGenerator.GetBytes(16);
        nonce = Convert.ToBase64String(nonceData);
        return CreateCryptoServiceProvider(identifier, nonce);
    }

    private static SymmetricAlgorithm CreateCryptoServiceProvider(string identifier, string nonce)
    {
        var instanceNameData = Encoding.UTF8.GetBytes(ConfigurationHelper.CacheName);
        var identifierData = Encoding.UTF8.GetBytes(identifier);
#if DIAGNOSTIC
        var start = Stopwatch.GetTimestamp();
#endif
        var keyData = Rfc2898DeriveBytes.Pbkdf2(instanceNameData, identifierData, 1_000, HashAlgorithmName.SHA256, 16);
#if DIAGNOSTIC
        var elapsed = Stopwatch.GetElapsedTime(start);
        Console.WriteLine($"Took {elapsed.TotalMicroseconds}us to derive the encryption key for {identifier}");
#endif
        var nonceData = Convert.FromBase64String(nonce);
        var csp = Aes.Create();
        csp.Key = keyData;
        csp.IV = nonceData;
        return csp;
    }

    public static bool TryLoad(Guid id, [NotNullWhen(true)] out ObjectCacheData<TObject>? data)
    {
        try
        {
#if DEBUG
            var start = Stopwatch.GetTimestamp();
#endif

            var cacheDirectory = RootCacheDirectory;
            if (!Directory.Exists(cacheDirectory))
            {
                data = default;
                return false;
            }

            Id<TObject> strongId = new(id);
            var fileName = ObjectCacheData<TObject>.GetCacheFileName(strongId);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                data = default;
                return false;
            }

            var qualifiedFileName = Path.Combine(cacheDirectory, fileName);
            if (!File.Exists(qualifiedFileName))
            {
                data = default;
                return false;
            }

            using var fileStream = File.OpenRead(qualifiedFileName);
            var nonceLength = fileStream.ReadByte();
            Span<byte> nonceBytes = stackalloc byte[nonceLength];
            var read = fileStream.Read(nonceBytes);
            if (read != nonceLength)
            {
                data = default;
                return false;
            }

            var nonce = Encoding.UTF8.GetString(nonceBytes);
            using var symmetricAlgorithm = CreateCryptoServiceProvider(fileName, nonce);
            using CryptoStream cryptoStream = new(
                fileStream,
                symmetricAlgorithm.CreateDecryptor(),
                CryptoStreamMode.Read
            );
            using GZipStream compressedStream = new(cryptoStream, CompressionMode.Decompress);
            using BinaryReader reader = new(compressedStream);
            Guid rawId = new(reader.ReadBytes(16));
            if (rawId != id)
            {
                data = default;
                return false;
            }

            var versionLength = reader.ReadInt32();
            var version = versionLength > 0 ? Encoding.UTF8.GetString(reader.ReadBytes(versionLength)) : default;
            var dataLength = reader.ReadInt32();
            if (dataLength < 0)
            {
                data = default;
                return false;
            }

            var rawData = reader.ReadBytes(dataLength);
            data = new ObjectCacheData<TObject>
            {
                Id = strongId,
                Data = rawData,
                Version = version,
            };

#if DEBUG
            var elapsed = Stopwatch.GetElapsedTime(start);
            Console.WriteLine($"Took {elapsed.TotalMicroseconds}us to load {fileName}");
#endif

            return true;
        }
        catch (Exception exception)
        {
            LegacyLogging.Logger?.Error(exception);
            data = default;
            return false;
        }
    }

    public static bool TrySave(ObjectCacheData<TObject> objectCacheData)
    {
        try
        {
#if DEBUG
            var start = Stopwatch.GetTimestamp();
#endif

            var data = objectCacheData.Data;
            if (data == default)
            {
                return false;
            }

            var cacheDirectory = RootCacheDirectory;
            if (!Directory.Exists(cacheDirectory))
            {
                Directory.CreateDirectory(cacheDirectory);
            }

            var fileName = ObjectCacheData<TObject>.GetCacheFileName(objectCacheData.Id);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            var qualifiedFileName = Path.Combine(cacheDirectory, fileName);

            using var symmetricAlgorithm = CreateCryptoServiceProvider(fileName, out string nonce);
            using var fileStream = File.OpenWrite(qualifiedFileName);
            var nonceBytes = Encoding.UTF8.GetBytes(nonce);
            fileStream.WriteByte((byte)nonceBytes.Length);
            fileStream.Write(nonceBytes);
            using CryptoStream cryptoStream = new(fileStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
            using GZipStream compressedStream = new(cryptoStream, CompressionLevel.Optimal);

            using BinaryWriter writer = new(compressedStream);
            writer.Write(objectCacheData.Id.Guid.ToByteArray());

            var version = objectCacheData.Version;
            if (string.IsNullOrWhiteSpace(version))
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(version.Length);
                writer.Write(Encoding.UTF8.GetBytes(version));
            }

            writer.Write(data.Length);
            writer.Write(data);

#if DEBUG
            var elapsed = Stopwatch.GetElapsedTime(start);
            Console.WriteLine($"Took {elapsed.TotalMicroseconds}us to save {fileName}");
#endif

            return true;
        }
        catch (Exception exception)
        {
            LegacyLogging.Logger?.Error(exception);
            return false;
        }
    }
}