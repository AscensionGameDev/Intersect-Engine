using System.Diagnostics;
using System.Security.Cryptography;

using Intersect.Logging;
using Intersect.Memory;
using MessagePack;

#if INTERSECT_DIAGNOSTIC
using Intersect.Logging;
#endif

namespace Intersect.Network.Packets;

[MessagePackObject]
public partial class HailPacket : ConnectionPacket
{
    [IgnoreMember]
    private byte[] mVersionData;

    [IgnoreMember]
    private RSAParameters mRsaParameters;

    private byte _symmetricVersion = (byte)(AesGcm.IsSupported ? 1 : 0);

    public HailPacket()
    {
    }

    public HailPacket(RSACryptoServiceProvider rsa) : base(rsa, null)
    {
    }

    public HailPacket(
        RSA rsa,
        byte[] handshakeSecret,
        byte[] versionData,
        RSAParameters rsaParameters
    ) : base(rsa, handshakeSecret)
    {
        VersionData = versionData;
        RsaParameters = rsaParameters;
    }

    [IgnoreMember]
    public byte[] VersionData
    {
        get => mVersionData;
        set => mVersionData = value;
    }

    [IgnoreMember]
    public RSAParameters RsaParameters
    {
        get => mRsaParameters;
        set => mRsaParameters = value;
    }

    [IgnoreMember]
    public byte SymmetricVersion
    {
        get => _symmetricVersion;
        set => _symmetricVersion = value;
    }

    public override bool Encrypt()
    {
        using (var buffer = new MemoryBuffer())
        {
            buffer.Write(VersionData);
            buffer.Write(HandshakeSecret, SIZE_HANDSHAKE_SECRET);
            buffer.Write(Adjusted);
#if DEBUG
            buffer.Write(UTC);
            buffer.Write(Offset);
#endif

#if INTERSECT_DIAGNOSTIC
            Log.Debug($"VersionData: {BitConverter.ToString(VersionData)}");
            Log.Debug($"Handshake secret: {BitConverter.ToString(HandshakeSecret)}.");
#endif

            Debug.Assert(RsaParameters.Modulus != null, "RsaParameters.Modulus != null");
            var bits = (ushort)(RsaParameters.Modulus.Length << 3);
            buffer.Write(bits);
            buffer.Write(RsaParameters.Exponent, 3);
            buffer.Write(RsaParameters.Modulus, bits >> 3);
            buffer.Write(SymmetricVersion);

#if INTERSECT_DIAGNOSTIC
            DumpKey(RsaParameters, true);
#endif

            Debug.Assert(mRsa != null, "mRsa != null");

            EncryptedData = mRsa.Encrypt(buffer.ToArray(), RSAEncryptionPadding.OaepSHA256) ??
                            throw new InvalidOperationException("Failed to encrypt the buffer.");

            return true;
        }
    }

    public override bool Decrypt(RSA rsa)
    {
        try
        {
            mRsa = rsa;

            if (mRsa == null)
            {
                throw new ArgumentNullException(nameof(rsa));
            }

            var decryptedHail = mRsa.Decrypt(EncryptedData, RSAEncryptionPadding.OaepSHA256);
            using (var buffer = new MemoryBuffer(decryptedHail))
            {
                if (!buffer.Read(out mVersionData))
                {
                    return false;
                }

                if (!buffer.Read(out mHandshakeSecret, SIZE_HANDSHAKE_SECRET))
                {
                    return false;
                }

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
                Log.Debug($"VersionData: {BitConverter.ToString(VersionData)}");
                Log.Debug($"Handshake secret: {BitConverter.ToString(HandshakeSecret)}.");
#endif

                if (!buffer.Read(out ushort bits))
                {
                    return false;
                }

                RsaParameters = new RSAParameters();
                if (!buffer.Read(out mRsaParameters.Exponent, 3))
                {
                    return false;
                }

                if (!buffer.Read(out mRsaParameters.Modulus, bits >> 3))
                {
                    return false;
                }

                if (!buffer.Read(out _symmetricVersion))
                {
                    return false;
                }

#if INTERSECT_DIAGNOSTIC
                DumpKey(RsaParameters, true);
#endif

                return true;
            }
        }
        catch (Exception exception)
        {
            Log.Warn(exception);
            return false;
        }
    }

}
