using Intersect.Memory;

namespace Intersect.Network.Packets.Reflectable
{
    public class BinaryPacket : CerasPacket
    {
        private ByteBuffer mBuffer;

        public byte[] Data
        {
            get
            {
                if (mBuffer != null) return mBuffer.ToArray();
                return new byte[0];
            }
            set
            {
                mBuffer = new ByteBuffer();
                mBuffer.WriteBytes(value);
            }
        }

        public BinaryPacket()
        {

        }

        public BinaryPacket(IConnection connection, ByteBuffer buffer)
        {
            mBuffer = buffer;
        }

        public ByteBuffer GetBuffer()
        {
            return mBuffer;
        }

    }
}