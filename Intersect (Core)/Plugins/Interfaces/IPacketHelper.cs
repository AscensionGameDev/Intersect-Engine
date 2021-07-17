using Intersect.Network;

using System;
using System.Collections.Generic;

namespace Intersect.Plugins.Interfaces
{
    /// <summary>
    /// Defines the API for accessing packet information.
    /// </summary>
    public interface IPacketHelper
    {
        /// <summary>
        /// Registry of packet types.
        /// </summary>
        PacketTypeRegistry TypeRegistry { get; }

        /// <summary>
        /// Registry of packet handlers.
        /// </summary>
        PacketHandlerRegistry HandlerRegistry { get; }

        /// <summary>
        /// All packet types available to the networking layer.
        /// </summary>
        IReadOnlyList<Type> AvailablePacketTypes { get; }

        /// <summary>
        /// Native packet types that are not added by plugins.
        /// </summary>
        IReadOnlyList<Type> BuiltInPacketTypes { get; }

        /// <summary>
        /// Packet types that are added by all plugins.
        /// </summary>
        IReadOnlyList<Type> AllPluginPacketTypes { get; }

        /// <summary>
        /// Packet types that are added by this plugin.
        /// </summary>
        IReadOnlyList<Type> CurrentPluginPacketTypes { get; }

        /// <summary>
        /// Try to add a packet type to the registry.
        /// </summary>
        /// <typeparam name="TPacket"></typeparam>
        /// <returns></returns>
        bool TryRegisterPacketType<TPacket>() where TPacket : IPacket;

        /// <summary>
        /// Try to add a packet preprocessor (multiple allowed) to the packet
        /// handler registry for the specified packet type.
        ///
        /// Preprocessors are meant as boolean filters for whether a packet
        /// should or should be handled (pre-hook, handler, post-hook), and
        /// should also be where any packet mutation occurs (if a packet
        /// needs to be mutated for some reason).
        ///
        /// Preprocessors run before all handlers and pre-hooks.
        /// </summary>
        /// <param name="handler"></param>
        /// <typeparam name="THandler"></typeparam>
        /// <typeparam name="TPacket"></typeparam>
        /// <returns></returns>
        bool TryRegisterPacketPreprocessor<THandler, TPacket>(out THandler handler)
            where TPacket : IPacket where THandler : IPacketHandler<TPacket>;

        /// <summary>
        /// Try to add a packet handler (can only be one per packet) to the
        /// packet handler registry for the specified packet type.
        /// </summary>
        /// <param name="handler"></param>
        /// <typeparam name="THandler"></typeparam>
        /// <typeparam name="TPacket"></typeparam>
        /// <returns></returns>
        bool TryRegisterPacketHandler<THandler, TPacket>(out THandler handler)
            where TPacket : IPacket where THandler : IPacketHandler<TPacket>;

        /// <summary>
        /// Try to add a packet pre-hook (multiple allowed) to the packet
        /// handler registry for the specified packet type.
        ///
        /// Pre-hooks run immediately before handlers.
        /// </summary>
        /// <param name="handler"></param>
        /// <typeparam name="THandler"></typeparam>
        /// <typeparam name="TPacket"></typeparam>
        /// <returns></returns>
        bool TryRegisterPacketPreHook<THandler, TPacket>(out THandler handler)
            where TPacket : IPacket where THandler : IPacketHandler<TPacket>;

        /// <summary>
        /// Try to add a packet post-hook (multiple allowed) to the packet
        /// handler registry for the specified packet type.
        ///
        /// Post-hooks run immediately after handlers.
        /// </summary>
        /// <param name="handler"></param>
        /// <typeparam name="THandler"></typeparam>
        /// <typeparam name="TPacket"></typeparam>
        /// <returns></returns>
        bool TryRegisterPacketPostHook<THandler, TPacket>(out THandler handler)
            where TPacket : IPacket where THandler : IPacketHandler<TPacket>;
    }
}
