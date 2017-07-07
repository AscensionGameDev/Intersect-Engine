using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Policy;
using Intersect.Collections;

namespace Intersect.Network
{
    [SuppressMessage("ReSharper", "ArrangeAccessorOwnerBody")]
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public abstract class AbstractPacketFactory : IPacketFactory
    {
        private readonly IDictionary<PacketCode, PacketType> mPacketTypeLookup;

        protected IDictionary<PacketCode, PacketType> PacketTypeLookup { get; }

        protected AbstractPacketFactory()
        {
            mPacketTypeLookup = new Dictionary<PacketCode, PacketType>();
            PacketTypeLookup = new ReadOnlyDictionary<PacketCode, PacketType>(mPacketTypeLookup);
        }

        protected bool AddPacketTypes(params PacketType[] packetTypes)
        {
            return AddPacketTypes(packetTypes as IEnumerable<PacketType>);
        }

        protected bool AddPacketTypes(IEnumerable<PacketType> packetTypes)
        {
            if (packetTypes == null) throw new ArgumentNullException();

            var success = true;
            foreach (var packetType in packetTypes)
            {
                if (packetType == null) continue;
                if (mPacketTypeLookup?.ContainsKey(packetType.Code) ?? false) continue;
                mPacketTypeLookup?.Add(packetType.Code, packetType);
                success &= mPacketTypeLookup?.ContainsKey(packetType.Code) ?? false;
            }
            return success;
        }

        protected bool RemovePacketTypes(params PacketType[] packetTypes)
        {
            return RemovePacketTypes(packetTypes as IEnumerable<PacketType>);
        }

        protected bool RemovePacketTypes(IEnumerable<PacketType> packetTypes)
        {
            if (packetTypes == null) throw new ArgumentNullException();

            var success = true;
            foreach (var packetType in packetTypes)
            {
                if (packetType == null) continue;
                if (mPacketTypeLookup?.ContainsKey(packetType.Code) ?? true) continue;
                success &= (bool) mPacketTypeLookup?.Remove(packetType.Code);
            }
            return success;
        }

        public abstract bool CanCreatePacketType(PacketCode packetCode);

        public abstract IPacket Create(PacketCode packetCode, IConnection connection);
        public abstract IPacket Create(PacketCode packetCode, IConnection connection, params object[] args);

        public abstract TPacketType Create<TPacketType>(PacketCode packetCode, IConnection connection) where TPacketType : class, IPacket;
        public abstract TPacketType Create<TPacketType>(PacketCode packetCode, IConnection connection, params object[] args) where TPacketType : class, IPacket;
    }
}