namespace Intersect.Network
{
    public interface IExtendedPacket<out TType> : IPacket
    {
        TType Type { get; }
    }
}