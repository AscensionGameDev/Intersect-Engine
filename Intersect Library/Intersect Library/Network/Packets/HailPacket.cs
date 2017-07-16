using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Intersect.Logging;
using Intersect.Memory;

namespace Intersect.Network.Packets
{
    public class HailPacket : ConnectionPacket
    {
        private byte[] mEncryptedHail;

        private byte[] mVersionData;
        public byte[] VersionData
        {
            get => mVersionData;
            set => mVersionData = value;
        }

        private RSAParameters mRsaParameters;
        public RSAParameters RsaParameters
        {
            get => mRsaParameters;
            set => mRsaParameters = value;
        }

        public override int EstimatedSize => mEncryptedHail?.Length + sizeof(int) ?? -1;

        public HailPacket(RSACryptoServiceProvider rsa)
            : base(rsa, null)
        {
        }

        public HailPacket(RSACryptoServiceProvider rsa, byte[] handshakeSecret, byte[] versionData, RSAParameters rsaParameters)
            : base(rsa, handshakeSecret)
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
                var bits = (ushort)(RsaParameters.Modulus.Length << 3);
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

        public override bool Read(ref IBuffer buffer)
        {
            if (!base.Read(ref buffer)) return false;
            if (!buffer.Read(out mEncryptedHail)) return false;

            if (mRsa == null) throw new ArgumentNullException(nameof(mRsa));
            var decryptedHail = mRsa.Decrypt(mEncryptedHail, true);
            using (var hailBuffer = new MemoryBuffer(decryptedHail))
            {
                if (!hailBuffer.Read(out mVersionData)) return false;
                if (!hailBuffer.Read(out mHandshakeSecret, SIZE_HANDSHAKE_SECRET)) return false;

#if INTERSECT_DIAGNOSTIC
                Log.Debug($"VersionData: {BitConverter.ToString(VersionData)}");
                Log.Debug($"Handshake secret: {BitConverter.ToString(HandshakeSecret)}.");
#endif

                if (!hailBuffer.Read(out ushort bits)) return false;
                RsaParameters = new RSAParameters();
                if (!hailBuffer.Read(out mRsaParameters.Exponent, 3)) return false;
                if (!hailBuffer.Read(out mRsaParameters.Modulus, bits >> 3)) return false;

#if INTERSECT_DIAGNOSTIC
                DumpKey(RsaParameters, true);
#endif

                return true;
            }
        }

        public override bool Write(ref IBuffer buffer)
        {
            buffer.Write(mEncryptedHail);

            return true;
        }
    }
}