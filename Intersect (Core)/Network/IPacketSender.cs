namespace Intersect.Network
{
    public interface IPacketSender
    {
        bool Send(IPacket packet);
    }
}
