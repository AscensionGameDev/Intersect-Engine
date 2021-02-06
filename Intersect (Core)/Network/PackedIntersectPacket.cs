using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network
{
    [MessagePackObject]
    public class PackedIntersectPacket
    {
        private static readonly string[] BuiltInPacketNamespaces = new[]
        {
            "Intersect.Network.Packets",
            "Intersect.Network.Packets.Client",
            "Intersect.Network.Packets.Editor",
            "Intersect.Network.Packets.Server",
            "Intersect.Admin.Actions"
        };

        public static Dictionary<Type, short> KnownTypes { get; set; } = new Dictionary<Type, short>();

        public static Dictionary<short, Type> KnownKeys { get; set; } = new Dictionary<short, Type>();

        private static IEnumerable<Type> FindTypes(IEnumerable<string> nameSpaces) => nameSpaces.SelectMany(FindTypes);

        private static IEnumerable<Type> FindTypes(string nameSpace) =>
            typeof(Ceras).Assembly.GetTypes().Where(type => type.Namespace == nameSpace);

        public static void AddKnownTypes(IReadOnlyList<Type> types)
        {
            short i = (short)KnownKeys.Count;
            foreach (var type in types.Where(type => !KnownTypes.ContainsKey(type))) {
                KnownKeys.Add(i, type);
                KnownTypes.Add(type, i++);
            }
        }

        static PackedIntersectPacket()
        {
            AddKnownTypes(FindTypes(BuiltInPacketNamespaces).ToList());
        }

        [Key(0)]
        public short PacketKey { get; set; } = -1;

        [Key(1)]
        public byte[] PacketData { get; set; }

        [IgnoreMember]
        public Type PacketType => KnownKeys[PacketKey];

        public PackedIntersectPacket()
        {

        }

        public PackedIntersectPacket(IntersectPacket pkt)
        {
            PacketKey = KnownTypes[pkt.GetType()];
        }
    }
}
