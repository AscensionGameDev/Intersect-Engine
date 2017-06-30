using Intersect.Memory;

namespace Intersect.Network.Packets
{
    public class BinaryPacket : AbstractPacket
    {
        public ByteBuffer Buffer;

        public override int EstimatedSize => Buffer?.Length() + sizeof(int) ?? sizeof(int);

        public BinaryPacket(IConnection connection)
            : base(connection, PacketCodes.BinaryPacket)
        {
        }

        public override bool Read(ref IBuffer buffer)
        {
            if (!base.Read(ref buffer)) return false;

            if (!buffer.Read(out byte[] bytes)) return false;

            Buffer = new ByteBuffer();
            Buffer.WriteBytes(bytes);

            return true;
        }

        public override bool Write(ref IBuffer buffer)
        {
            if (!base.Write(ref buffer)) return false;

            if (Buffer == null)
            {
                buffer.Write(0);
            }
            else
            {
                buffer.Write(Buffer.ToArray());
            }

            return true;
        }
    }
}