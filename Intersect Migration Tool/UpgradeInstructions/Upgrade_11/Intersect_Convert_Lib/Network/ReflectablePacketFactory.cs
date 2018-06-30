using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Logging;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Network
{
    [SuppressMessage("ReSharper", "ConvertIfStatementToNullCoalescingExpression")]
    [SuppressMessage("ReSharper", "UseNullPropagation")]
    [SuppressMessage("ReSharper", "MergeConditionalExpression")]
    public sealed class ReflectablePacketFactory : AbstractPacketFactory
    {
        private static readonly object FactoryInstanceLock;
        private static ReflectablePacketFactory sFactoryInstance;

        static ReflectablePacketFactory()
        {
            FactoryInstanceLock = new object();
        }

        private ReflectablePacketFactory()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly == null) continue;
                foreach (var type in assembly.GetTypes())
                {
                    if (type == null) continue;
                    if (!"Intersect.Network.Packets.Reflectable".Equals(type.Namespace)) continue;
                    var packetType = PacketType.Of(type);
                    if (packetType == null) continue;
                    if (AddPacketTypes(packetType)) continue;
                    Log.Debug($"Error trying to add packet type {packetType} to the ReflectablePacketFactory.");
                }
            }
        }

        public static ReflectablePacketFactory Instance
        {
            get
            {
                Debug.Assert(FactoryInstanceLock != null, "sFactoryInstanceLock != null");
                lock (FactoryInstanceLock)
                {
                    if (sFactoryInstance == null)
                    {
                        sFactoryInstance = new ReflectablePacketFactory();
                    }

                    return sFactoryInstance;
                }
            }
        }

        public override bool CanCreatePacketType(PacketCode packetCode)
        {
            if (packetCode == PacketCode.Unknown) return false;
            Debug.Assert(PacketTypeLookup != null, "PacketTypeLookup != null");
            return PacketTypeLookup.ContainsKey(packetCode);
        }

        public override IPacket Create(PacketCode packetCode, IConnection connection)
        {
            return Create(packetCode, connection, null);
        }

        public override IPacket Create(PacketCode packetCode, IConnection connection, params object[] args)
        {
            Debug.Assert(PacketTypeLookup != null, "PacketTypeLookup != null");
            var packetType = PacketTypeLookup[packetCode];
            return packetType == null ? null : packetType.CreateInstance(connection);
        }

        public override TPacketType Create<TPacketType>(PacketCode packetCode, IConnection connection)
        {
            return Create<TPacketType>(packetCode, connection, null);
        }

        public override TPacketType Create<TPacketType>(PacketCode packetCode, IConnection connection,
            params object[] args)
        {
            return Create(packetCode, connection, args) as TPacketType;
        }
    }
}