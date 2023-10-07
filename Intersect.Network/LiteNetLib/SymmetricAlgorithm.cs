using System.Security.Cryptography;

namespace Intersect.Network.LiteNetLib;

public abstract class SymmetricAlgorithm : ISymmetricAlgorithm
{
    protected static bool IsValidSize(KeySizes sizes, int size)
    {
        if (size == sizes.MinSize || size == sizes.MaxSize)
        {
            return true;
        }

        if (size < sizes.MinSize || size > sizes.MaxSize)
        {
            return false;
        }

        return (size - sizes.MinSize) % sizes.SkipSize == 0;
    }

    protected static bool TryPickValidSize(KeySizes sizes, out byte size)
    {
        if (sizes.MinSize > byte.MaxValue)
        {
            size = default;
            return false;
        }

        var currentSize = sizes.MaxSize;

        while (currentSize > byte.MaxValue)
        {
            currentSize -= sizes.SkipSize;
        }

        size = (byte)currentSize;
        return true;
    }

    public static ISymmetricAlgorithm PickForVersion(byte version, ReadOnlySpan<byte> key = default)
    {
        if (version == 0)
        {
            return new AesAlgorithm(key);
        }

        if (!AesGcm.IsSupported)
        {
            throw new PlatformNotSupportedException(
                $"Received an AES-GCM stream but this platform is reporting that it is not supported."
            );
        }

        return new AesGcmAlgorithm(key);
    }

    public static ISymmetricAlgorithm PickForPlatform(ReadOnlySpan<byte> key = default) =>
        AesGcm.IsSupported ? new AesGcmAlgorithm(key) : new AesAlgorithm(key);

    public abstract void Dispose();
    public abstract bool SetKey(ReadOnlySpan<byte> key);
    public abstract EncryptionResult TryDecrypt(ReadOnlySpan<byte> cipherdata, out ReadOnlySpan<byte> plaindata);
    public abstract EncryptionResult TryDecrypt(ReadOnlySpan<byte> cipherdata, int offset, int length, out ReadOnlySpan<byte> plaindata);
    public abstract EncryptionResult TryEncrypt(ReadOnlySpan<byte> plaindata, out ReadOnlySpan<byte> cipherdata);
    public abstract EncryptionResult TryEncrypt(ReadOnlySpan<byte> plaindata, int offset, int length, out ReadOnlySpan<byte> cipherdata);
}