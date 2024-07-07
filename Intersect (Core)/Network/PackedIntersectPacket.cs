﻿using MessagePack;

namespace Intersect.Network;

[MessagePackObject]
public partial class PackedIntersectPacket
{
    private static readonly string[] BuiltInPacketNamespaces = new[]
    {
        "Intersect.Network.Packets",
        "Intersect.Network.Packets.Client",
        "Intersect.Network.Packets.Editor",
        "Intersect.Network.Packets.Server",
        "Intersect.Admin.Actions"
    };

    public static readonly Dictionary<Type, short> KnownTypes = new();

    public static readonly Dictionary<short, Type> KnownKeys = new();

    private static IEnumerable<Type> FindTypes(IEnumerable<string> nameSpaces) => nameSpaces.SelectMany(FindTypes);

    private static IEnumerable<Type> FindTypes(string nameSpace) =>
        typeof(Ceras).Assembly.GetTypes().Where(type => type.Namespace == nameSpace);

    public static void AddKnownTypes(IReadOnlyList<Type> types)
    {
        var key = (short)KnownKeys.Count;
        foreach (var type in types) {
            if (KnownTypes.ContainsKey(type))
            {
                continue;
            }
            
            KnownKeys.Add(key, type);
            KnownTypes.Add(type, key);

            key++;
        }
    }

    [Key(0)]
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public short Key { get; set; } = -1;

    [Key(1)]
    public byte[] Data { get; set; }

    [IgnoreMember]
    public Type PacketType => KnownKeys[Key];

    public PackedIntersectPacket()
    {

    }

    public PackedIntersectPacket(IntersectPacket packet)
    {
        if (!KnownTypes.TryGetValue(packet.GetType(), out var key))
        {
            throw new ArgumentException($"Type not a known packet type: {packet.GetType().FullName}");
        }

        Key = key;
    }
}
