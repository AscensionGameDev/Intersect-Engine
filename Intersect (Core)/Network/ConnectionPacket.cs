using System;
using System.Security.Cryptography;

using Ceras;

using JetBrains.Annotations;

#if INTERSECT_DIAGNOSTIC
using Intersect.Logging;
#endif

namespace Intersect.Network
{

    public abstract class ConnectionPacket : CerasPacket
    {

        protected const int SIZE_HANDSHAKE_SECRET = 32;

        protected RSACryptoServiceProvider mRsa;

        private byte[] mEncryptedData;

        protected byte[] mHandshakeSecret;

        protected ConnectionPacket()
        {
        }

        protected ConnectionPacket(RSACryptoServiceProvider rsa, byte[] handshakeSecret)
        {
            mRsa = rsa ?? throw new ArgumentNullException();

            mHandshakeSecret = handshakeSecret;
        }

        [Exclude]
        public byte[] HandshakeSecret
        {
            get => mHandshakeSecret;
            set => mHandshakeSecret = value;
        }

        [Include, NotNull]
        protected byte[] EncryptedData
        {
            get => mEncryptedData;

            set => mEncryptedData = value;
        }

        public abstract bool Encrypt();

        public abstract bool Decrypt([NotNull] RSACryptoServiceProvider rsa);

        protected static void DumpKey(RSAParameters parameters, bool isPublic)
        {
#if INTERSECT_DIAGNOSTIC
            Log.Diagnostic($"Exponent: {BitConverter.ToString(parameters.Exponent)}");
            Log.Diagnostic($"Modulus: {BitConverter.ToString(parameters.Modulus)}");

            if (isPublic) return;
            Log.Diagnostic($"D: {BitConverter.ToString(parameters.D)}");
            Log.Diagnostic($"DP: {BitConverter.ToString(parameters.DP)}");
            Log.Diagnostic($"DQ: {BitConverter.ToString(parameters.DQ)}");
            Log.Diagnostic($"InverseQ: {BitConverter.ToString(parameters.InverseQ)}");
            Log.Diagnostic($"P: {BitConverter.ToString(parameters.P)}");
            Log.Diagnostic($"Q: {BitConverter.ToString(parameters.Q)}");
#endif
        }

    }

}
