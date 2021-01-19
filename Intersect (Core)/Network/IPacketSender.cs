using Intersect.Core;

namespace Intersect.Network
{
    public interface IPacketSender
    {
        IApplicationContext ApplicationContext { get; }

        bool Send(IPacket packet);
    }
}
