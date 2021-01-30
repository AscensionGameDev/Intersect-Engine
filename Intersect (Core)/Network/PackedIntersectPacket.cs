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

        public static Dictionary<Type, Int32> KnownTypes { get; set; } = new Dictionary<Type, Int32>();

        public static Dictionary<Int32, Type> KnownKeys { get; set; } = new Dictionary<Int32, Type>();

        private static IEnumerable<Type> FindTypes(IEnumerable<string> nameSpaces) => nameSpaces.SelectMany(FindTypes);

        private static IEnumerable<Type> FindTypes(string nameSpace) =>
            typeof(Ceras).Assembly.GetTypes().Where(type => type.Namespace == nameSpace);

        static PackedIntersectPacket()
        {
            var i = 0;
            foreach (var type in FindTypes(BuiltInPacketNamespaces))
            {
                KnownKeys.Add(i, type);
                KnownTypes.Add(type, i++);
            }
        }

        [Key(0)]
        public Int32 PacketKey { get; set; } = -1;

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
