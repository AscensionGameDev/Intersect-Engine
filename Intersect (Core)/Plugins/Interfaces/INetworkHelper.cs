using Intersect.Network;

using System;
using System.Collections.Generic;

namespace Intersect.Plugins.Interfaces
{
    public interface INetworkHelper
    {
        PacketTypeRegistry TypeRegistry { get; }

        PacketHandlerRegistry HandlerRegistry { get; }

        IReadOnlyList<Type> AvailablePacketTypes { get; }

        IReadOnlyList<Type> BuiltInPacketTypes { get; }

        IReadOnlyList<Type> AllPluginPacketTypes { get; }

        IReadOnlyList<Type> CurrentPluginPacketTypes { get; }

        bool TryRegisterPacketType<TPacket>() where TPacket : IPacket;

        bool TryRegisterPacketPreprocessor<THandler, TPacket>(out THandler handler)
            where TPacket : IPacket where THandler : IPacketHandler<TPacket>;

        bool TryRegisterPacketHandler<THandler, TPacket>(out THandler handler)
            where TPacket : IPacket where THandler : IPacketHandler<TPacket>;

        bool TryRegisterPacketPreHook<THandler, TPacket>(out THandler handler)
            where TPacket : IPacket where THandler : IPacketHandler<TPacket>;

        bool TryRegisterPacketPostHook<THandler, TPacket>(out THandler handler)
            where TPacket : IPacket where THandler : IPacketHandler<TPacket>;
    }
}
