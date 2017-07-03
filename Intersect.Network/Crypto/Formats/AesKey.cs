using Intersect.Memory;
using System;

namespace Intersect.Network.Crypto.Formats
{
    public class AesKey : EncryptionKey
    {
        private byte[] mData;
        public byte[] Data
        {
            get => mData;
            set => mData = value;
        }

        public AesKey()
            : this(null)
        {
        }

        public AesKey(byte[] data)
            : base(KeyFormat.Aes)
        {
            Data = data;
        }

        protected override bool InternalRead(IBuffer buffer)
        {
            if (buffer == null) throw new ArgumentNullException();

            return buffer.Read(out mData);
        }

        protected override bool InternalWrite(IBuffer buffer)
        {
            if (buffer == null) throw new ArgumentNullException();

            buffer.Write(mData);

            return true;
        }
    }
}