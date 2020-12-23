namespace Intersect.Network
{
    public delegate bool HandlePacket<TPacketSender, TPacket>(TPacketSender packetSender, TPacket packet)
        where TPacketSender : IPacketSender where TPacket : IPacket;

    public delegate void HandlePacketVoid<TPacketSender, TPacket>(TPacketSender packetSender, TPacket packet)
        where TPacketSender : IPacketSender where TPacket : IPacket;

    public delegate bool HandlePacketGeneric(IPacketSender packetSender, IPacket packet);

    public delegate bool HandlePacket(IConnection connection, IPacket packet);

    public delegate bool ShouldProcessPacket(IConnection connection, long pSize);
}
