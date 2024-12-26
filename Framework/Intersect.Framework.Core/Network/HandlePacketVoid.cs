namespace Intersect.Network;

public delegate void HandlePacketVoid<TPacketSender, TPacket>(TPacketSender packetSender, TPacket packet)
    where TPacketSender : IPacketSender where TPacket : IPacket;