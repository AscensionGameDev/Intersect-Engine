using System;
using System.Security.Cryptography;

using Ceras;

using Intersect.Utilities;

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
        protected byte[] mHandshakeSecret;

        protected long mAdjusted;
        protected long mUTC;
        protected long mOffset;

        protected ConnectionPacket()
        {
        }

        protected ConnectionPacket(RSACryptoServiceProvider rsa, byte[] handshakeSecret)
        {
            mRsa = rsa ?? throw new ArgumentNullException();

            mHandshakeSecret = handshakeSecret;

            Adjusted = Timing.Global.Ticks;
            Offset = Timing.Global.TicksOffset;
            UTC = Timing.Global.TicksUTC;
        }

        [Exclude]
        public byte[] HandshakeSecret
        {
            get => mHandshakeSecret;
            set => mHandshakeSecret = value;
        }

        [Exclude]
        public long Adjusted
        {
            get => mAdjusted;
            set => mAdjusted = value;
        }

        [Exclude]
        public long UTC
        {
            get => mUTC;
            set => mUTC = value;
        }

        [Exclude]
        public long Offset
        {
            get => mOffset;
            set => mOffset = value;
        }

        [Include, NotNull]
        protected byte[] EncryptedData { get; set; }

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
