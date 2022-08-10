using Intersect.Framework.Reflection;

using MessagePack;
using MessagePack.Formatters;

namespace Intersect.Network;

public sealed class IntersectPacketFormatter : IMessagePackFormatter<IntersectPacket>
{
    private static readonly Dictionary<Type, int> _packetTypeIndex;
    private static readonly Dictionary<int, Type> _packetTypeReverseIndex;

    static IntersectPacketFormatter()
    {
        _packetTypeIndex = new();
        _packetTypeReverseIndex = new();

        var currentDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        var packetTypes = currentDomainAssemblies
            .Where(assembly => !assembly.IsDynamic)
            .SelectMany(assembly =>
            {
                try
                {
                    return assembly.ExportedTypes;
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            });

        AddTypes(packetTypes);
    }

    public static void AddTypes(params Type[] types) =>
        AddTypes(types as IEnumerable<Type>);

    public static void AddTypes(IEnumerable<Type> types)
    {
        var packetTypes = types
            .Where(
                type => !type.IsAbstract
                    && !type.IsGenericTypeDefinition
                    && type.Extends(typeof(IntersectPacket))
            )
            .Distinct()
            .OrderBy(type => type.AssemblyQualifiedName);

        foreach (var packetType in packetTypes)
        {
            var packetTypeCode = _packetTypeIndex.Count;

            if (_packetTypeIndex.ContainsKey(packetType))
            {
                continue;
            }

            _packetTypeIndex[packetType] = packetTypeCode;
            _packetTypeReverseIndex[packetTypeCode] = packetType;
        }
    }

    public IntersectPacket Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return default;
        }

        var packetTypeCode = _packetTypeIndex.Count switch
        {
            <= byte.MaxValue => reader.ReadByte(),
            <= ushort.MaxValue => reader.ReadUInt16(),
            _ => reader.ReadInt32(),
        };

        var packetType = _packetTypeReverseIndex[packetTypeCode];
        return MessagePackSerializer.Deserialize(packetType, ref reader, options) as IntersectPacket;
    }

    public void Serialize(ref MessagePackWriter writer, IntersectPacket value, MessagePackSerializerOptions options)
    {
        if (value == default)
        {
            writer.WriteNil();
        }

        var packetType = value.GetType();
        var packetTypeCode = _packetTypeIndex[packetType];
        switch (_packetTypeIndex.Count)
        {
            case <= byte.MaxValue: writer.WriteUInt8((byte)packetTypeCode); break;
            case <= ushort.MaxValue: writer.WriteUInt16((ushort)packetTypeCode); break;
            default: writer.WriteInt32(packetTypeCode); break;
        }

        MessagePackSerializer.Serialize(packetType, ref writer, value, options);
    }
}
