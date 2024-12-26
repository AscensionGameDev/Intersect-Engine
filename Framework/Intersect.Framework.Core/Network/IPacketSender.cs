using Intersect.Core;

namespace Intersect.Network;

public interface IPacketSender
{
    IApplicationContext ApplicationContext { get; }

    INetwork Network { get; }

    bool Send(IPacket packet);
}
