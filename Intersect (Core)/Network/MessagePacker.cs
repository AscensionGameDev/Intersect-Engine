using Intersect.Logging;
using Intersect.Network.Packets.Client;
using Intersect.Network.Packets.Server;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network
{
    public class MessagePacker
    {
        public static readonly MessagePacker Instance = new MessagePacker();

        private readonly MessagePackSerializerOptions mOptions  = MessagePackSerializerOptions.Standard.
            WithResolver(MessagePack.Resolvers.CompositeResolver.Create(
                new IFormatterResolver[] { 
                    MessagePack.Resolvers.NativeGuidResolver.Instance, 
                    MessagePack.Resolvers.NativeDateTimeResolver.Instance, 
                    MessagePack.Resolvers.NativeDecimalResolver.Instance, 
                    MessagePack.Resolvers.StandardResolver.Instance }
                )
            );

        private readonly MessagePackSerializerOptions mCompressedOptions = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);

        public byte[] Serialize(IntersectPacket pkt)
        {
            var packedPacket = new PackedIntersectPacket(pkt)
            {
                Data = MessagePackSerializer.Serialize((object)pkt, mOptions)
            };

            return MessagePackSerializer.Serialize((object)packedPacket, mCompressedOptions);
        }

        public object Deserialize(byte[] data)
        {
            try
            {
                var packedPacket = MessagePackSerializer.Deserialize<PackedIntersectPacket>(data, mCompressedOptions);
                var intersectPacket = MessagePackSerializer.Deserialize(packedPacket.PacketType, packedPacket.Data, mOptions);
                return intersectPacket;
            }
            catch (Exception exception)
            {
                Log.Error(exception);

                return null;
            }
        }

    }
}
