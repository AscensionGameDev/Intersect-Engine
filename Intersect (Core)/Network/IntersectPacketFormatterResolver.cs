
using MessagePack;
using MessagePack.Formatters;

namespace Intersect.Network;

public sealed class IntersectPacketFormatterResolver : IFormatterResolver
{
    private readonly IntersectPacketFormatter _intersectPacketFormatter = new();

    public static readonly IntersectPacketFormatterResolver Instance = new();

    public IMessagePackFormatter<T> GetFormatter<T>()
    {
        if (typeof(T) == typeof(IntersectPacket))
        {
            return _intersectPacketFormatter as IMessagePackFormatter<T>;
        }

        return default;
    }
}
