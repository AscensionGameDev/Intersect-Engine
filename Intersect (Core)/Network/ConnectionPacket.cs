using System.Security.Cryptography;
using Intersect.Network.Packets;
using Intersect.Utilities;
using MessagePack;

#if INTERSECT_DIAGNOSTIC

#endif

namespace Intersect.Network;

[MessagePackObject]
[Union(0, typeof(ApprovalPacket))]
[Union(1, typeof(HailPacket))]
public abstract partial class ConnectionPacket : IntersectPacket
{
    protected const int SIZE_HANDSHAKE_SECRET = 32;

    [IgnoreMember]
    protected RSA mRsa;

    [IgnoreMember]
    protected byte[] mHandshakeSecret;

    [IgnoreMember]
    protected long mAdjusted;

    [IgnoreMember]
    protected long mUTC;

    [IgnoreMember]
    protected long mOffset;

    protected ConnectionPacket()
    {
    }

    protected ConnectionPacket(RSA rsa, byte[] handshakeSecret)
    {
        mRsa = rsa ?? throw new ArgumentNullException(nameof(rsa));

        mHandshakeSecret = handshakeSecret;

        Adjusted = Timing.Global.Ticks;
        Offset = Timing.Global.TicksOffset;
        UTC = Timing.Global.TicksUtc;
    }

    [IgnoreMember]
    public byte[] HandshakeSecret
    {
        get => mHandshakeSecret;
        set => mHandshakeSecret = value;
    }

    [IgnoreMember]
    public long Adjusted
    {
        get => mAdjusted;
        set => mAdjusted = value;
    }

    [IgnoreMember]
    public long UTC
    {
        get => mUTC;
        set => mUTC = value;
    }

    [IgnoreMember]
    public long Offset
    {
        get => mOffset;
        set => mOffset = value;
    }

    [Key(0)]
    public byte[] EncryptedData { get; set; }

    public abstract bool Encrypt();

    public abstract bool Decrypt(RSA rsa);

    protected static void DumpKey(RSAParameters parameters, bool isPublic)
    {
#if INTERSECT_DIAGNOSTIC
        ApplicationContext.Context.Value?.Logger.LogTrace($"Exponent: {BitConverter.ToString(parameters.Exponent)}");
        ApplicationContext.Context.Value?.Logger.LogTrace($"Modulus: {BitConverter.ToString(parameters.Modulus)}");

        if (isPublic) return;
        ApplicationContext.Context.Value?.Logger.LogTrace($"D: {BitConverter.ToString(parameters.D)}");
        ApplicationContext.Context.Value?.Logger.LogTrace($"DP: {BitConverter.ToString(parameters.DP)}");
        ApplicationContext.Context.Value?.Logger.LogTrace($"DQ: {BitConverter.ToString(parameters.DQ)}");
        ApplicationContext.Context.Value?.Logger.LogTrace($"InverseQ: {BitConverter.ToString(parameters.InverseQ)}");
        ApplicationContext.Context.Value?.Logger.LogTrace($"P: {BitConverter.ToString(parameters.P)}");
        ApplicationContext.Context.Value?.Logger.LogTrace($"Q: {BitConverter.ToString(parameters.Q)}");
#endif
    }
}
