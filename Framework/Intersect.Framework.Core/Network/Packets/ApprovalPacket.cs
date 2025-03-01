using System.Diagnostics;
using System.Security.Cryptography;
using Intersect.Core;
using Intersect.Memory;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace Intersect.Network.Packets;

[MessagePackObject]
public class ApprovalPacket : ConnectionPacket
{
    [IgnoreMember] private const int SIZE_AES_KEY = 32;

    [IgnoreMember] private const int SIZE_GUID = 16;

    [IgnoreMember] private byte[] mAesKey;

    public ApprovalPacket()
    {
    }

    public ApprovalPacket(RSA rsa, byte[] handshakeSecret, byte[] aesKey, Guid guid) : base(rsa, handshakeSecret)
    {
        AesKey = aesKey;
        Guid = guid;
    }

    public ApprovalPacket(RSAParameters rsaParameters, byte[] handshakeSecret, byte[] aesKey, Guid guid) : base(
        RSA.Create(rsaParameters),
        handshakeSecret
    )
    {
        AesKey = aesKey;
        Guid = guid;
    }

    [IgnoreMember] public Guid Guid { get; set; }

    [IgnoreMember]
    public byte[] AesKey
    {
        get => mAesKey;
        set => mAesKey = value;
    }

    public override bool Encrypt()
    {
        using (var buffer = new MemoryBuffer())
        {
#if INTERSECT_DIAGNOSTIC
                ApplicationContext.Context.Value?.Logger.LogDebug($"Handshake secret: {BitConverter.ToString(HandshakeSecret)}.");
                ApplicationContext.Context.Value?.Logger.LogDebug($"Specified AES Key: {BitConverter.ToString(AesKey)}");
                ApplicationContext.Context.Value?.Logger.LogDebug($"Assigning UUID: {Guid}).");
#endif
            buffer.Write(HandshakeSecret, SIZE_HANDSHAKE_SECRET);
            buffer.Write(AesKey, SIZE_AES_KEY);
            buffer.Write(Guid.ToByteArray(), SIZE_GUID);
            buffer.Write(Adjusted);
#if DEBUG
            buffer.Write(UTC);
            buffer.Write(Offset);
#endif

            Debug.Assert(mRsa != null, "mRsa != null");
            EncryptedData = mRsa.Encrypt(buffer.ToArray(), RSAEncryptionPadding.OaepSHA256) ??
                            throw new InvalidOperationException("Failed to encrypt the buffer.");
#if DIAGNOSTIC
            ApplicationContext.Context.Value?.Logger.LogDebug($"ApprovalPacket.Encrypt() [{mRsa.KeySize}] EncryptedData({EncryptedData.Length})={Convert.ToHexString(EncryptedData)}");
#endif
            return true;
        }
    }

    public override bool Decrypt(RSA rsa)
    {
        mRsa = rsa;

        if (mRsa == null)
        {
            throw new ArgumentNullException(nameof(rsa));
        }

        try
        {
#if DIAGNOSTIC
            ApplicationContext.Context.Value?.Logger.LogDebug(
                $"ApprovalPacket.Decrypt() [{rsa.KeySize}] EncryptedData({EncryptedData.Length})={Convert.ToHexString(EncryptedData)}"
            );
#endif
            var decryptedApproval = mRsa.Decrypt(EncryptedData, RSAEncryptionPadding.OaepSHA256);
            using (var buffer = new MemoryBuffer(decryptedApproval))
            {
                if (!buffer.Read(out mHandshakeSecret, SIZE_HANDSHAKE_SECRET))
                {
                    return false;
                }

                if (!buffer.Read(out mAesKey, SIZE_AES_KEY))
                {
                    return false;
                }

                if (!buffer.Read(out var guidData, SIZE_GUID) || guidData == null)
                {
                    return false;
                }

                Guid = new Guid(guidData);

                if (!buffer.Read(out mAdjusted))
                {
                    return false;
                }

#if DEBUG
                if (!buffer.Read(out mUTC))
                {
                    return false;
                }

                if (!buffer.Read(out mOffset))
                {
                    return false;
                }
#endif

#if INTERSECT_DIAGNOSTIC
                ApplicationContext.Context.Value?.Logger.LogDebug($"Handshake secret: {BitConverter.ToString(HandshakeSecret)}.");
                ApplicationContext.Context.Value?.Logger.LogDebug($"Assigned AES Key: {BitConverter.ToString(AesKey)}");
                ApplicationContext.Context.Value?.Logger.LogDebug($"Assigned UUID: {Guid}).");
#endif

                return true;
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(exception, "Error decrypting approval packet");
            return false;
        }
    }
}