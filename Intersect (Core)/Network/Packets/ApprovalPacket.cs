using System;
using System.Diagnostics;
using System.Security.Cryptography;

using Intersect.Memory;

#if INTERSECT_DIAGNOSTIC
using Intersect.Logging;
#endif

namespace Intersect.Network.Packets
{

    public class ApprovalPacket : ConnectionPacket
    {

        private const int SIZE_AES_KEY = 32;

        private const int SIZE_GUID = 16;

        private byte[] mAesKey;

        private byte[] mEncryptedApproval;

        private Guid mGuid;

        public ApprovalPacket()
        {
        }

        public ApprovalPacket(RSACryptoServiceProvider rsa, byte[] handshakeSecret, byte[] aesKey, Guid guid) : base(
            rsa, handshakeSecret
        )
        {
            AesKey = aesKey;
            Guid = guid;

            using (var approvalBuffer = new MemoryBuffer())
            {
#if INTERSECT_DIAGNOSTIC
                Log.Debug($"Handshake secret: {BitConverter.ToString(HandshakeSecret)}.");
                Log.Debug($"Specified AES Key: {BitConverter.ToString(AesKey)}");
                Log.Debug($"Assigning UUID: {Guid}).");
#endif
                approvalBuffer.Write(HandshakeSecret, SIZE_HANDSHAKE_SECRET);
                approvalBuffer.Write(AesKey, SIZE_AES_KEY);
                approvalBuffer.Write(Guid.ToByteArray(), SIZE_GUID);

                Debug.Assert(mRsa != null, "mRsa != null");
                mEncryptedApproval = mRsa.Encrypt(approvalBuffer.ToArray(), true);
            }
        }

        public Guid Guid
        {
            get => mGuid;
            set => mGuid = value;
        }

        public byte[] AesKey
        {
            get => mAesKey;
            set => mAesKey = value;
        }

    }

}
