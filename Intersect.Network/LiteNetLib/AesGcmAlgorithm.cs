using System.Buffers;
using System.Buffers.Binary;
using System.Security;
using System.Security.Cryptography;
using Intersect.Core;
using Microsoft.Extensions.Logging;

namespace Intersect.Network.LiteNetLib;

public sealed class AesGcmAlgorithm : SymmetricAlgorithm
{
    // Encode the version (1 byte), plaindata length (1 int), the nonce length (1 byte), and the tag length (1 byte)
    private const int HeaderSize = sizeof(int) + sizeof(byte) * 3;

    private readonly HashSet<AesGcm> _algorithmInstances = [];
    private readonly HashSet<AesGcm> _algorithmInstancesToDispose = [];
    private readonly Stack<AesGcm> _algorithmPool = [];
    private readonly Memory<byte> _key;
    private readonly byte _nonceSize;
    private readonly byte _tagSize;

    private MemoryHandle _keyHandle;

    public AesGcmAlgorithm(ReadOnlySpan<byte> key = default)
    {
        var buffer = RandomNumberGenerator.GetBytes(32);
        if (key != default)
        {
            var keyLength = key.Length;
            if (keyLength != 32)
            {
                throw new ArgumentOutOfRangeException(nameof(key), keyLength, "Key size must be exactly 32 bytes.");
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

        if (!TryPickValidSize(AesGcm.NonceByteSizes, out _nonceSize))
        {
            throw new SecurityException("Failed to initialize network encryption, no valid nonce sizes");
        }

        if (!TryPickValidSize(AesGcm.TagByteSizes, out _tagSize))
        {
            throw new SecurityException("Failed to initialize network encryption, no valid tag sizes");
        }
    }

    private AesGcm AcquireInstance()
    {
        lock (_algorithmPool)
        {
            if (_algorithmPool.TryPop(out var algorithmInstance))
            {
                return algorithmInstance;
            }

            algorithmInstance = new AesGcm(key: _key.Span, tagSizeInBytes: _tagSize);
            _algorithmInstances.Add(algorithmInstance);
            return algorithmInstance;
        }
    }

    private void ReleaseInstance(AesGcm algorithmInstance)
    {
        lock (_algorithmPool)
        {
            if (_algorithmInstancesToDispose.Remove(algorithmInstance))
            {
                algorithmInstance.Dispose();
                return;
            }

            _algorithmPool.Push(algorithmInstance);
        }
    }

    public override void Dispose()
    {
        _keyHandle.Dispose();
        ClearPool();
    }

    private void ClearPool()
    {
        lock (_algorithmPool)
        {
            while (_algorithmPool.TryPop(out var algorithmInstance))
            {
                _algorithmInstances.Remove(algorithmInstance);
                algorithmInstance.Dispose();
            }

            foreach (var algorithmInstance in _algorithmInstances)
            {
                _algorithmInstancesToDispose.Add(algorithmInstance);
            }

            _algorithmInstances.Clear();
        }
    }

    public override bool SetKey(ReadOnlySpan<byte> key)
    {
        if (!key.TryCopyTo(_key.Span))
        {
            return false;
        }

        ClearPool();
        return true;
    }

    public override EncryptionResult TryDecrypt(ReadOnlySpan<byte> cipherdata, out ReadOnlySpan<byte> plaindata)
    {
        if (cipherdata.Length < HeaderSize)
        {
            plaindata = default;
            return EncryptionResult.NoHeader;
        }

        var cipherdataView = cipherdata;

        var version = cipherdataView[0];
        cipherdataView = cipherdataView[1..];

        if (version != 1)
        {
            plaindata = default;
            return EncryptionResult.InvalidVersion;
        }

        var plaindataLength = BinaryPrimitives.ReadInt32LittleEndian(cipherdataView);
        if (plaindataLength < 1)
        {
            plaindata = default;
            return EncryptionResult.EmptyInput;
        }

        cipherdataView = cipherdataView[sizeof(int)..];

        var nonceLength = cipherdataView[0];
        if (!IsValidSize(AesGcm.NonceByteSizes, nonceLength))
        {
            plaindata = default;
            return EncryptionResult.InvalidNonce;
        }

        cipherdataView = cipherdataView[1..];

        var tagLength = cipherdataView[0];
        if (!IsValidSize(AesGcm.TagByteSizes, tagLength))
        {
            plaindata = default;
            return EncryptionResult.InvalidTag;
        }

        cipherdataView = cipherdataView[1..];

        var nonce = cipherdataView[..nonceLength];
        cipherdataView = cipherdataView[nonceLength..];

        var tag = cipherdataView[..tagLength];
        cipherdataView = cipherdataView[tagLength..];

        if (cipherdataView.Length != plaindataLength)
        {
            plaindata = default;
            return EncryptionResult.SizeMismatch;
        }

        var buffer = new byte[plaindataLength];
        Memory<byte> memory = new(buffer);

        try
        {
            var aesGcm = AcquireInstance();
            aesGcm.Decrypt(nonce, cipherdataView, tag, memory.Span);
            ReleaseInstance(aesGcm);

            plaindata = memory.Span;
            return EncryptionResult.Success;
        }
        catch (Exception exception)
        {
#if DIAGNOSTIC
            ApplicationContext.Context.Value?.Logger.LogDebug($"cipherdata({cipherdata.Length})=[nonce={Convert.ToHexString(nonce)}, tag={Convert.ToHexString(tag)}]");
            ApplicationContext.Context.Value?.Logger.LogDebug($"cipherdataView({cipherdataView.Length})={Convert.ToHexString(cipherdataView)}");
            ApplicationContext.Context.Value?.Logger.LogDebug($"key={Convert.ToHexString(_key.Span)}");
#endif
            var configuredLogLevel = Options.Instance?.Logging.Level ?? LogLevel.Trace;
            if (configuredLogLevel >= LogLevel.Debug)
            {
                ApplicationContext.Context.Value?.Logger.LogError(exception, "Error decrypting cipherdata");
            }
            plaindata = default;
            return EncryptionResult.Error;
        }
    }

    public override EncryptionResult TryDecrypt(ReadOnlySpan<byte> cipherdata, int offset, int length, out ReadOnlySpan<byte> plaindata) =>
        TryDecrypt(cipherdata[offset..(offset + length)], out plaindata);

    private EncryptionResult TryEncryptInternal(ReadOnlySpan<byte> plaindata, ReadOnlySpan<byte> nonce, out ReadOnlySpan<byte> cipherdata)
    {
        if (plaindata.Length < 1)
        {
            cipherdata = default;
            return EncryptionResult.EmptyInput;
        }

        if (nonce.Length != _nonceSize)
        {
            cipherdata = default;
            return EncryptionResult.InvalidNonce;
        }

        var nonceLength = (byte)nonce.Length;

        var cipherdataLength = HeaderSize + nonceLength + _tagSize + plaindata.Length;

        var buffer = new byte[cipherdataLength];
        Memory<byte> memory = new(buffer);

        var cipherdataView = memory.Span;

        cipherdataView[0] = 1; /* Version */
        cipherdataView = cipherdataView[1..];

        BinaryPrimitives.WriteInt32LittleEndian(cipherdataView, plaindata.Length);
        cipherdataView = cipherdataView[sizeof(int)..];

        cipherdataView[0] = nonceLength;
        cipherdataView = cipherdataView[1..];

        cipherdataView[0] = _tagSize;
        cipherdataView = cipherdataView[1..];

        nonce.CopyTo(cipherdataView[..nonceLength]);
        cipherdataView = cipherdataView[nonceLength..];

        var tag = cipherdataView[.._tagSize];
        cipherdataView = cipherdataView[_tagSize..];

        try
        {
            var aesGcm = AcquireInstance();
            aesGcm.Encrypt(nonce, plaindata, cipherdataView, tag);
            ReleaseInstance(aesGcm);

            cipherdata = memory.Span;

#if DIAGNOSTIC
            if (plaindata.Length > 1000)
            {
                ApplicationContext.Context.Value?.Logger.LogDebug($"cipherdata({cipherdata.Length})=[nonce={Convert.ToHexString(nonce)}, tag={Convert.ToHexString(tag)}]");
                ApplicationContext.Context.Value?.Logger.LogDebug($"cipherdataView({cipherdataView.Length})={Convert.ToHexString(cipherdataView)}");
                ApplicationContext.Context.Value?.Logger.LogDebug($"key={Convert.ToHexString(_key.Span)}");
            }
#endif

            return EncryptionResult.Success;
        }
        catch (Exception exception)
        {
            var configuredLogLevel = Options.Instance?.Logging.Level ?? LogLevel.Trace;
            if (configuredLogLevel >= LogLevel.Debug)
            {
                ApplicationContext.Context.Value?.Logger.LogError(exception, "Error encrypting plaindata");
            }
            cipherdata = default;
            return EncryptionResult.Error;
        }
    }

    public override EncryptionResult TryEncrypt(ReadOnlySpan<byte> plaindata, out ReadOnlySpan<byte> cipherdata)
    {
        var nonce = new byte[_nonceSize];
        RandomNumberGenerator.Fill(nonce);

        return TryEncryptInternal(plaindata, nonce, out cipherdata);
    }

    public override EncryptionResult TryEncrypt(ReadOnlySpan<byte> plaindata, ReadOnlySpan<byte> nonce, out ReadOnlySpan<byte> cipherdata)
    {
        return TryEncryptInternal(plaindata, nonce, out cipherdata);
    }

    public override EncryptionResult TryEncrypt(ReadOnlySpan<byte> plaindata, int offset, int length, out ReadOnlySpan<byte> cipherdata) =>
        TryEncrypt(plaindata[offset..(offset + length)], out cipherdata);
}