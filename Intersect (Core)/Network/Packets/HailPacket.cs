using System.Diagnostics;
using System.Security.Cryptography;

using Intersect.Memory;

#if INTERSECT_DIAGNOSTIC
using Intersect.Logging;
#endif

namespace Intersect.Network.Packets
{

    public class HailPacket : ConnectionPacket
    {

        private byte[] mEncryptedHail;

        private RSAParameters mRsaParameters;

        private byte[] mVersionData;

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

            using (var hailBuffer = new MemoryBuffer())
            {
                hailBuffer.Write(VersionData);
                hailBuffer.Write(HandshakeSecret, SIZE_HANDSHAKE_SECRET);

#if INTERSECT_DIAGNOSTIC
                Log.Debug($"VersionData: {BitConverter.ToString(VersionData)}");
                Log.Debug($"Handshake secret: {BitConverter.ToString(HandshakeSecret)}.");
#endif

                Debug.Assert(RsaParameters.Modulus != null, "RsaParameters.Modulus != null");
                var bits = (ushort) (RsaParameters.Modulus.Length << 3);
                hailBuffer.Write(bits);
                hailBuffer.Write(RsaParameters.Exponent, 3);
                hailBuffer.Write(RsaParameters.Modulus, bits >> 3);

#if INTERSECT_DIAGNOSTIC
                DumpKey(RsaParameters, true);
#endif

                Debug.Assert(mRsa != null, "mRsa != null");
                mEncryptedHail = mRsa.Encrypt(hailBuffer.ToArray(), true);
            }
        }

        public byte[] VersionData
        {
            get => mVersionData;
            set => mVersionData = value;
        }

        public RSAParameters RsaParameters
        {
            get => mRsaParameters;
            set => mRsaParameters = value;
        }

    }

}
