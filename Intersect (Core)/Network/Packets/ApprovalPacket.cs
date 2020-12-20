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

    public class ApprovalPacket : ConnectionPacket
    {

        private const int SIZE_AES_KEY = 32;

        private const int SIZE_GUID = 16;

        private byte[] mAesKey;

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
        }

        [Exclude]
        public Guid Guid
        {
            get => mGuid;
            set => mGuid = value;
        }

        [Exclude]
        public byte[] AesKey
        {
            get => mAesKey;
            set => mAesKey = value;
        }

        public override bool Encrypt()
        {
            using (var buffer = new MemoryBuffer())
            {
#if INTERSECT_DIAGNOSTIC
                Log.Debug($"Handshake secret: {BitConverter.ToString(HandshakeSecret)}.");
                Log.Debug($"Specified AES Key: {BitConverter.ToString(AesKey)}");
                Log.Debug($"Assigning UUID: {Guid}).");
#endif
                buffer.Write(HandshakeSecret, SIZE_HANDSHAKE_SECRET);
                buffer.Write(AesKey, SIZE_AES_KEY);
                buffer.Write(Guid.ToByteArray(), SIZE_GUID);
                buffer.Write(Adjusted);
#if DEBUG
                buffer.Write(UTC);
                buffer.Write(Offset);
#endif

                Debug.Assert(mRsa != null, "mRsa != null");
                EncryptedData = mRsa.Encrypt(buffer.ToArray(), true) ??
                                throw new InvalidOperationException("Failed to encrypt the buffer.");

                return true;
            }
        }

        public override bool Decrypt(RSACryptoServiceProvider rsa)
        {
            mRsa = rsa;

            if (mRsa == null)
            {
                throw new ArgumentNullException(nameof(rsa));
            }

            var decryptedApproval = mRsa.Decrypt(EncryptedData, true);
            using (var buffer = new MemoryBuffer(decryptedApproval))
            {
                if (!buffer.Read(out mHandshakeSecret, SIZE_HANDSHAKE_SECRET))
                {
                    return false;
                }

                if (!buffer.Read(out mAesKey, SIZE_AES_KEY))
                {
                    return false;
                }

                if (!buffer.Read(out var guidData, SIZE_GUID) || guidData == null)
                {
                    return false;
                }

                Guid = new Guid(guidData);

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
                Log.Debug($"Handshake secret: {BitConverter.ToString(HandshakeSecret)}.");	
                Log.Debug($"Assigned AES Key: {BitConverter.ToString(AesKey)}");	
                Log.Debug($"Assigned UUID: {Guid}).");
#endif

                return true;
            }
        }

    }

}
