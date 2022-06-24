using Intersect.Collections;
using Intersect.Network;
using Intersect.Plugins.Interfaces;

using System;
using System.Collections.Generic;

namespace Intersect.Plugins.Helpers
{
    /// <summary>
    /// Implementation for <see cref="IPacketHelper"/>.
    /// </summary>
    public sealed partial class PacketHelper : IPacketHelper
    {
        private PacketHelper Parent { get; }

        private List<Type> PluginPacketTypes { get; }

        public PacketHelper(PacketTypeRegistry packetTypeRegistry, PacketHandlerRegistry packetHandlerRegistry)
        {
            Parent = null;

            HandlerRegistry = packetHandlerRegistry ?? throw new ArgumentNullException(nameof(packetHandlerRegistry));
            TypeRegistry = packetTypeRegistry ?? throw new ArgumentNullException(nameof(packetTypeRegistry));

            PluginPacketTypes = new List<Type>();
        }

        public PacketHelper(IPacketHelper parentPacketHelper)
        {
            if (default == parentPacketHelper)
            {
                throw new ArgumentNullException(nameof(parentPacketHelper));
            }

            if (!(parentPacketHelper is PacketHelper parentNetworkHelper))
            {
                throw new ArgumentException(
                    $"This constructor can only be used if {nameof(parentPacketHelper)} is a {typeof(PacketHelper).FullName}.",
                    nameof(parentPacketHelper)
                );
            }

            Parent = parentNetworkHelper;

            HandlerRegistry = Parent.HandlerRegistry;
            TypeRegistry = Parent.TypeRegistry;

            PluginPacketTypes = new List<Type>();
        }

        public PacketHandlerRegistry HandlerRegistry { get; }

        public PacketTypeRegistry TypeRegistry { get; }

        public IReadOnlyList<Type> AvailablePacketTypes => TypeRegistry.Types;

        public IReadOnlyList<Type> BuiltInPacketTypes => TypeRegistry.BuiltInTypes;

        public IReadOnlyList<Type> AllPluginPacketTypes => (Parent?.PluginPacketTypes ?? PluginPacketTypes).WrapReadOnly();

        public IReadOnlyList<Type> CurrentPluginPacketTypes => PluginPacketTypes.WrapReadOnly();

        public bool TryRegisterPacketHandler<THandler, TPacket>(out THandler handler)
            where TPacket : IPacket
            where THandler : IPacketHandler<TPacket>
            => HandlerRegistry.TryRegisterHandler(out handler);

        public bool TryRegisterPacketPostHook<THandler, TPacket>(out THandler handler)
            where TPacket : IPacket
            where THandler : IPacketHandler<TPacket>
            => HandlerRegistry.TryRegisterPostHook<THandler, TPacket>(out handler);

        public bool TryRegisterPacketPreHook<THandler, TPacket>(out THandler handler)
            where TPacket : IPacket
            where THandler : IPacketHandler<TPacket>
            => HandlerRegistry.TryRegisterPreHook<THandler, TPacket>(out handler);

        public bool TryRegisterPacketPreprocessor<THandler, TPacket>(out THandler handler)
            where TPacket : IPacket
            where THandler : IPacketHandler<TPacket>
            => HandlerRegistry.TryRegisterPreHook<THandler, TPacket>(out handler);

        public bool TryRegisterPacketType<TPacket>() where TPacket : IPacket
        {
            if (TypeRegistry.TryRegister<TPacket>())
            {
                PluginPacketTypes.Add(typeof(TPacket));
                Parent?.PluginPacketTypes?.Add(typeof(TPacket));
                return true;
            }

            return false;
        }
    }
}
