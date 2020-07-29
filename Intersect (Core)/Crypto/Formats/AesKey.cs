using System;

using Intersect.Memory;

namespace Intersect.Crypto.Formats
{
    public class AesKey : EncryptionKey
    {
        private byte[] mData;

        public AesKey() : this(null)
        {
        }

        public AesKey(byte[] data) : base(KeyFormat.Aes)
        {
            Data = data;
        }

        public byte[] Data
        {
            get => mData;
            set => mData = value;
        }

        protected override bool InternalRead(IBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            return buffer.Read(out mData);
        }

        protected override bool InternalWrite(IBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            buffer.Write(mData);

            return true;
        }
    }
}
