using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Network
{
    public sealed class PacketType
    {
        private static readonly ICollection<PacketType> PacketTypes = new HashSet<PacketType>();
        private static readonly IDictionary<Type, PacketType> TypeLookup = new Dictionary<Type, PacketType>();

        private static readonly IDictionary<PacketCode, PacketType> CodeLookup =
            new Dictionary<PacketCode, PacketType>();

        private PacketType(Type type, ConstructorInfo constructor, PacketCode code)
        {
            Type = type;
            Constructor = constructor;
            Code = code;
        }

        public Type Type { get; }
        public ConstructorInfo Constructor { get; }
        public PacketCode Code { get; }
        public IPacket Instance => CreateInstance();

        public IPacket CreateInstance(IConnection connection = null)
            => Constructor.Invoke(new object[] {connection}) as IPacket;

        public static PacketType Of(Type type)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(TypeLookup != null, "sTypeLookup != null");
            return TypeLookup.TryGetValue(type, out PacketType packetType) ? packetType : Of(type, null, null);
        }

        public static PacketType Of(IPacket packet)
        {
            Debug.Assert(CodeLookup != null, "sCodeLookup != null");
            return CodeLookup.TryGetValue(packet?.Code ?? PacketCode.Unknown, out PacketType packetType)
                ? packetType
                : Of(packet?.GetType(), packet, null);
        }

        private static PacketType Of(Type type, IPacket instance, ConstructorInfo constructor)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type), @"Type cannot be null.");

            if (type.IsAbstract || type.IsInterface)
                throw new InvalidOperationException("Can't have abstract or interface packets...");

            if (!typeof(IPacket).IsAssignableFrom(type))
                throw new InvalidOperationException("Must inherit from IPacket...");

            if (constructor == null)
            {
                constructor = type.GetConstructor(new[] {typeof(IConnection)});
            }

            if (instance == null)
            {
                instance = constructor?.Invoke(new object[] {null}) as IPacket;
            }

            Debug.Assert(instance != null, "instance != null");
            Debug.Assert(PacketTypes != null, "sPacketTypes != null");
            Debug.Assert(CodeLookup != null, "sCodeLookup != null");
            Debug.Assert(TypeLookup != null, "sTypeLookup != null");

            var packetType = new PacketType(type, constructor, instance.Code);
            PacketTypes.Add(packetType);
            CodeLookup.Add(instance.Code, packetType);
            TypeLookup.Add(type, packetType);
            return packetType;
        }
    }
}