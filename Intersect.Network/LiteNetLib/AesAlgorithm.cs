using System.Buffers;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using Intersect.Logging;

namespace Intersect.Network.LiteNetLib;

public sealed class AesAlgorithm : SymmetricAlgorithm
{
    // Encode the plaindata length (1 int), the nonce length (1 byte), and the tag length (1 byte)
    private const int HeaderSize = sizeof(int) + sizeof(byte) * 2;

    private static KeySizes[] LegalBlockSizes;
    private static KeySizes[] LegalKeySizes;

    static AesAlgorithm()
    {
        using var aes = Aes.Create();
        LegalBlockSizes = aes.LegalBlockSizes;
        LegalKeySizes = aes.LegalKeySizes;
    }

    private readonly Memory<byte> _key;

    private MemoryHandle _keyHandle;

    private static new bool IsValidSize(KeySizes sizes, int size)
    {
        return SymmetricAlgorithm.IsValidSize(
            new KeySizes(sizes.MinSize / 8, sizes.MaxSize / 8, sizes.SkipSize / 8),
            size
        );
    }

    public AesAlgorithm(ReadOnlySpan<byte> key = default)
    {
        var buffer = RandomNumberGenerator.GetBytes(32);
        if (key != default)
        {
            var keyLength = key.Length;
            if (!LegalKeySizes.Any(keySizes => IsValidSize(keySizes, keyLength)))
            {
                throw new ArgumentException(
                    $"{key.Length} is not a supported key length in bytes.",
                    nameof(key)
                );
            }

            if (!key.TryCopyTo(buffer))
            {
                throw new ArgumentException(
                    "The provided key could not be copied to the internal buffer.",
                    nameof(key)
                );
            }
        }

        _key = new Memory<byte>(buffer);
        _keyHandle = _key.Pin();
    }

    public override void Dispose()
    {
        _keyHandle.Dispose();
    }

    public override bool SetKey(ReadOnlySpan<byte> key) => key.TryCopyTo(_key.Span);

    public override EncryptionResult TryDecrypt(ReadOnlySpan<byte> cipherdata, out ReadOnlySpan<byte> plaindata)
    {
        plaindata = default;

        try
        {
            var version = cipherdata[0];
            cipherdata = cipherdata[1..];

            if (version != 0)
            {
                return EncryptionResult.InvalidVersion;
            }

            using var aes = Aes.Create();
            aes.Key = _key.ToArray();

            var plaindataLength = BinaryPrimitives.ReadInt32LittleEndian(cipherdata);
            cipherdata = cipherdata[sizeof(int)..];

            var nonceLength = cipherdata[0];
            cipherdata = cipherdata[1..];

            var nonce = cipherdata[..nonceLength];
            cipherdata = cipherdata[nonceLength..];

            aes.IV = nonce.ToArray();

            var cryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new(cipherdata.ToArray());
            using CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Read);
            var plaindataBuffer = new byte[plaindataLength];
            cryptoStream.ReadExactly(plaindataBuffer);
            plaindata = plaindataBuffer;

#if DIAGNOSTIC
            Log.Debug($"Decrypting {plaindata.Length} bytes:\nKey={Convert.ToHexString(aes.Key)} Nonce={Convert.ToHexString(aes.IV)}\nplaindata={Convert.ToHexString(plaindata)}\ncipherdata={Convert.ToHexString(cipherdata)}");
#endif

            return EncryptionResult.Success;
        }
        catch (Exception exception)
        {
            Log.Debug(exception);
            return EncryptionResult.Error;
        }
    }

    public override EncryptionResult TryDecrypt(ReadOnlySpan<byte> cipherdata, int offset, int length, out ReadOnlySpan<byte> plaindata) =>
        TryDecrypt(cipherdata[offset..(offset + length)], out plaindata);

    public override EncryptionResult TryEncrypt(ReadOnlySpan<byte> plaindata, out ReadOnlySpan<byte> cipherdata)
    {
        cipherdata = default;

        try
        {
            using var aes = Aes.Create();
            aes.Key = _key.ToArray();

            var nonce = RandomNumberGenerator.GetBytes(aes.BlockSize / 8);
            aes.IV = nonce;

            using MemoryStream memoryStream = new();

            // Version
            memoryStream.WriteByte(0);

            var plaindataLengthBytes = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32LittleEndian(plaindataLengthBytes, plaindata.Length);
            memoryStream.Write(plaindataLengthBytes);

            memoryStream.WriteByte((byte)nonce.Length);
            memoryStream.Write(nonce);

            var cryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);
            using CryptoStream cryptoStream = new(memoryStream, cryptoTransform, CryptoStreamMode.Write, true);
            cryptoStream.Write(plaindata);

            cryptoStream.Close();
            memoryStream.Close();

            cipherdata = memoryStream.ToArray();

#if DIAGNOSTIC
            Log.Debug($"Encrypting {plaindata.Length} bytes:\nKey={Convert.ToHexString(aes.Key)} Nonce={Convert.ToHexString(aes.IV)}\nplaindata={Convert.ToHexString(plaindata)}\ncipherdata={Convert.ToHexString(cipherdata[^plaindata.Length..])}");
#endif

            return EncryptionResult.Success;
        }
        catch (Exception exception)
        {
            Log.Debug(exception);
            return EncryptionResult.Error;
        }
    }

    public override EncryptionResult TryEncrypt(ReadOnlySpan<byte> plaindata, int offset, int length, out ReadOnlySpan<byte> cipherdata) =>
        TryEncrypt(plaindata[offset..(offset + length)], out cipherdata);
}