using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Intersect.Core;
using Intersect.Framework.Reflection;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace Intersect.Network;

[MessagePackObject]
public sealed class AsymmetricEncryptedPacket : IntersectPacket
{
    private byte[]? _encryptedData;
    private IntersectPacket? _innerPacket;

    [SerializationConstructor]
    public AsymmetricEncryptedPacket() { }

    public AsymmetricEncryptedPacket(IntersectPacket innerPacket, RSAParameters parameters)
    {
        InnerPacket = innerPacket;
        Parameters = parameters;
    }

    [Key(0)]
    public byte[]? EncryptedData
    {
        get => _encryptedData ??= TryEncrypt(out var encryptedData) ? encryptedData : default;
        set
        {
            _encryptedData = value;
            _innerPacket = null;
        }
    }

    [IgnoreMember]
    public IntersectPacket? InnerPacket
    {
        get => _innerPacket ??= TryDecrypt(out var innerPacket) ? innerPacket : default;
        set
        {
            _encryptedData = null;
            _innerPacket = value;
        }
    }

    [IgnoreMember]
    public RSAParameters? Parameters { get; set; }

    private bool TryDecrypt([NotNullWhen(true)] out IntersectPacket? innerPacket)
    {
        innerPacket = default;

        var parameters = Parameters;
        var encryptedData = _encryptedData;
        if (parameters == null || encryptedData == null)
        {
            return false;
        }

        try
        {
            using var rsa = RSA.Create(parameters.Value);
#if DIAGNOSTIC
            ApplicationContext.Context.Value?.Logger.LogDebug($"{nameof(TryDecrypt)} {nameof(encryptedData)}({encryptedData.Length})={Convert.ToHexString(encryptedData)}");
#endif
            var innerPacketData = rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);
#if DIAGNOSTIC
            ApplicationContext.Context.Value?.Logger.LogDebug($"innerPacketData({innerPacketData.Length})={Convert.ToHexString(innerPacketData)}");
#endif
            var deserializedObject = MessagePacker.Instance.Deserialize(innerPacketData);
            innerPacket = deserializedObject as IntersectPacket;

            if (innerPacket != default)
            {
                return true;
            }

            if (deserializedObject == default)
            {
                ApplicationContext.Context.Value?.Logger.LogDebug("Null packet");
                return false;
            }

            ApplicationContext.Context.Value?.Logger.LogDebug($"Expected {nameof(InnerPacket)}, received {deserializedObject.GetFullishName()}");
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(exception, "Error decrypting packet");

// #if DIAGNOSTIC
            ApplicationContext.Context.Value?.Logger.LogDebug($"{nameof(TryDecrypt)} {nameof(encryptedData)}({encryptedData.Length})={Convert.ToHexString(encryptedData)}");
// #endif
        }

        return false;
    }

    private bool TryEncrypt([NotNullWhen(true)] out byte[]? encryptedData)
    {
        encryptedData = default;

        var parameters = Parameters;
        var innerPacket = _innerPacket;
        if (parameters == null || innerPacket == null)
        {
            return false;
        }

        try
        {
            using var rsa = RSA.Create(parameters.Value);
            var innerPacketData = innerPacket.Data;
#if DIAGNOSTIC
            ApplicationContext.Context.Value?.Logger.LogDebug($"innerPacketData({innerPacketData.Length})={Convert.ToHexString(innerPacketData)}");
#endif
            encryptedData = rsa.Encrypt(innerPacketData, RSAEncryptionPadding.OaepSHA256);
#if DIAGNOSTIC
            ApplicationContext.Context.Value?.Logger.LogDebug($"encryptedData({encryptedData.Length})={Convert.ToHexString(encryptedData)}");
#endif
            return true;
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(exception, "Error encrypting packet");
            return false;
        }
    }
}