using System;
using System.Diagnostics;
using System.Security.Cryptography;

using Ceras;

using Intersect.Logging;
using Intersect.Memory;

using JetBrains.Annotations;

#if INTERSECT_DIAGNOSTIC
using Intersect.Logging;
#endif

namespace Intersect.Network.Packets
{

    public class HailPacket : ConnectionPacket
    {
        private byte[] mVersionData;

        private RSAParameters mRsaParameters;

        public HailPacket()
        {
        }

        public HailPacket(RSACryptoServiceProvider rsa) : base(rsa, null)
        {
        }

        public HailPacket(
            RSACryptoServiceProvider rsa,
            byte[] handshakeSecret,
            byte[] versionData,
            RSAParameters rsaParameters
        ) : base(rsa, handshakeSecret)
        {
            VersionData = versionData;
            RsaParameters = rsaParameters;
        }

        [Exclude]
        public byte[] VersionData
        {
            get => mVersionData;
            set => mVersionData = value;
        }

        [Exclude]
        public RSAParameters RsaParameters
        {
            get => mRsaParameters;
            set => mRsaParameters = value;
        }

        public override bool Encrypt()
        {
            using (var hailBuffer = new MemoryBuffer())
            {
                hailBuffer.Write(VersionData);
                hailBuffer.Write(HandshakeSecret, SIZE_HANDSHAKE_SECRET);

#if INTERSECT_DIAGNOSTIC
                Log.Debug($"VersionData: {BitConverter.ToString(VersionData)}");
                Log.Debug($"Handshake secret: {BitConverter.ToString(HandshakeSecret)}.");
#endif

                Debug.Assert(RsaParameters.Modulus != null, "RsaParameters.Modulus != null");
                var bits = (ushort)(RsaParameters.Modulus.Length << 3);
                hailBuffer.Write(bits);
                hailBuffer.Write(RsaParameters.Exponent, 3);
                hailBuffer.Write(RsaParameters.Modulus, bits >> 3);

#if INTERSECT_DIAGNOSTIC
                DumpKey(RsaParameters, true);
#endif

                Debug.Assert(mRsa != null, "mRsa != null");

                EncryptedData = mRsa.Encrypt(hailBuffer.ToArray(), true) ??
                                throw new InvalidOperationException("Failed to encrypt the buffer.");

                return true;
            }
        }

        public override bool Decrypt(RSACryptoServiceProvider rsa)
        {
            try
            {
                mRsa = rsa;

                if (mRsa == null)
                {
                    throw new ArgumentNullException(nameof(rsa));
                }

                var decryptedHail = mRsa.Decrypt(EncryptedData, true);
                using (var hailBuffer = new MemoryBuffer(decryptedHail))
                {
                    if (!hailBuffer.Read(out mVersionData))
                    {
                        return false;
                    }

                    if (!hailBuffer.Read(out mHandshakeSecret, SIZE_HANDSHAKE_SECRET))
                    {
                        return false;
                    }

#if INTERSECT_DIAGNOSTIC
                    Log.Debug($"VersionData: {BitConverter.ToString(VersionData)}");
                    Log.Debug($"Handshake secret: {BitConverter.ToString(HandshakeSecret)}.");
#endif

                    if (!hailBuffer.Read(out ushort bits))
                    {
                        return false;
                    }

                    RsaParameters = new RSAParameters();
                    if (!hailBuffer.Read(out mRsaParameters.Exponent, 3))
                    {
                        return false;
                    }

                    if (!hailBuffer.Read(out mRsaParameters.Modulus, bits >> 3))
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

}
