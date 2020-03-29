namespace Intersect.Network
{

    public delegate bool HandlePacket(IConnection connection, IPacket packet);

    public delegate bool ShouldProcessPacket(IConnection connection, long pSize);

}
