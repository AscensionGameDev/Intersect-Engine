namespace Intersect.Network;

public delegate bool HandlePacket<TPacketSender, TPacket>(TPacketSender packetSender, TPacket packet)
    where TPacketSender : IPacketSender where TPacket : IPacket;