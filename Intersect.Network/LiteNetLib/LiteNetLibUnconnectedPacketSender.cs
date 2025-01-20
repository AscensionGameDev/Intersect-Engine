using System.Net;
using Intersect.Core;
using Microsoft.Extensions.Logging;


namespace Intersect.Network.LiteNetLib;

public sealed class LiteNetLibUnconnectedPacketSender : IPacketSender
{
    private readonly IPEndPoint _endPoint;
    private readonly LiteNetLibInterface _networkLayerInterface;

    public LiteNetLibUnconnectedPacketSender(
        LiteNetLibInterface networkLayerInterface,
        IPEndPoint endPoint,
        INetwork network
    )
    {
        _networkLayerInterface = networkLayerInterface;
        _endPoint = endPoint;
        ApplicationContext = network.ApplicationContext;
        Network = network;
    }

    public IApplicationContext ApplicationContext { get; }

    public INetwork Network { get; }

    public bool Send(IPacket packet)
    {
        if (packet is UnconnectedPacket unconnectedPacket)
        {
            return _networkLayerInterface.SendUnconnectedPacket(_endPoint, unconnectedPacket);
        }

        ApplicationContext.Logger.LogError($"Expected a {nameof(UnconnectedPacket)} but tried to send a {packet.GetType().FullName}");
        return false;

    }
}