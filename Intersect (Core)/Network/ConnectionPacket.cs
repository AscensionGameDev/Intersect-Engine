using System;
using System.Security.Cryptography;
using Intersect.Network.Packets;
using Intersect.Utilities;

using MessagePack;

#if INTERSECT_DIAGNOSTIC
using Intersect.Logging;
#endif

namespace Intersect.Network
{
    [MessagePackObject]
    [Union(0, typeof(ApprovalPacket))]
    [Union(1, typeof(HailPacket))]
    public abstract class ConnectionPacket : IntersectPacket
    {
        protected const int SIZE_HANDSHAKE_SECRET = 32;

        [IgnoreMember]
        protected RSACryptoServiceProvider mRsa;

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

        protected ConnectionPacket(RSACryptoServiceProvider rsa, byte[] handshakeSecret)
        {
            mRsa = rsa ?? throw new ArgumentNullException();

            mHandshakeSecret = handshakeSecret;

            Adjusted = Timing.Global.Ticks;
            Offset = Timing.Global.TicksOffset;
            UTC = Timing.Global.TicksUTC;
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

        public abstract bool Decrypt(RSACryptoServiceProvider rsa);

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
