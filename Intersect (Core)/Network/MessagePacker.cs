using Intersect.Logging;
using MessagePack;
using Intersect.Memory;

namespace Intersect.Network
{
    public partial class MessagePacker
    {
        public static readonly MessagePacker Instance = new();

        private readonly MessagePackSerializerOptions mOptions  = MessagePackSerializerOptions.Standard.
            WithResolver(MessagePack.Resolvers.CompositeResolver.Create(
                    MessagePack.Resolvers.NativeGuidResolver.Instance,
                MessagePack.Resolvers.NativeDateTimeResolver.Instance,
                MessagePack.Resolvers.NativeDecimalResolver.Instance,
                MessagePack.Resolvers.StandardResolver.Instance
                )
            );

        private readonly MessagePackSerializerOptions mCompressedOptions = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);

        public byte[] Serialize(IntersectPacket packet)
        {
            var packedPacket = new PackedIntersectPacket(packet)
            {
                Data = MessagePackSerializer.Serialize((object)packet, mOptions)
            };

            return MessagePackSerializer.Serialize((object)packedPacket, mCompressedOptions);
        }

        public object? Deserialize(byte[] packetData, bool silent = false)
        {
            try
            {
                var packedPacket = MessagePackSerializer.Deserialize<PackedIntersectPacket>(packetData, mCompressedOptions);
                var intersectPacket = MessagePackSerializer.Deserialize(packedPacket.PacketType, packedPacket.Data, mOptions);
                return intersectPacket;
            }
            catch (Exception exception)
            {
#if DIAGNOSTIC
                Log.Debug($"Invalid packet: {Convert.ToHexString(data)}");
#endif

                if (!silent)
                {
                    Log.Error(exception);
                }

                return null;
            }
        }

        public TPacket? Deserialize<TPacket>(byte[] packetData, bool silent = false) where TPacket : IntersectPacket
        {
            return Deserialize(packetData, silent) as TPacket;
        }

        public object Deserialize(IBuffer buffer) => Deserialize(buffer.ReadBytes(buffer.Remaining));

    }
}
