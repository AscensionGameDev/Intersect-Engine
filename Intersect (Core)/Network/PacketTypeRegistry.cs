using Intersect.Collections;
using Intersect.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Intersect.Network
{
    public sealed class PacketTypeRegistry
    {
        private static readonly Assembly BuiltInAssembly = typeof(PacketTypeRegistry).Assembly;

        private Logger Logger { get; }

        private List<Type> BuiltInTypesInternal { get; }

        private List<Type> TypesInternal { get; }

        public IReadOnlyList<Type> BuiltInTypes { get; }

        public IReadOnlyList<Type> Types { get; }

        public PacketTypeRegistry(Logger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            BuiltInTypesInternal = new List<Type>();
            BuiltInTypes = BuiltInTypesInternal.WrapReadOnly();

            TypesInternal = new List<Type>();
            Types = TypesInternal.WrapReadOnly();
        }

        public bool IsRegistered(Type type) =>
            default == type ? throw new ArgumentNullException(nameof(type)) : TypesInternal.Contains(type);

        public bool TryRegisterBuiltIn()
        {
            var types = BuiltInAssembly.GetExportedTypes();
            var packetTypes = types.Where(type => type.GetInterfaces().Any(interfaceType => interfaceType == typeof(IPacket)));
            var definedPacketTypes = packetTypes.Where(type => !type.IsInterface && !type.IsValueType && !type.IsAbstract);
            BuiltInTypesInternal.AddRange(definedPacketTypes);
            return BuiltInTypesInternal.All(type => TryRegister(type)) && BuiltInTypesInternal.Count > 0;
        }

        public bool TryRegister<TPacket>() where TPacket : IPacket =>
            TryRegister(typeof(TPacket));

        public bool TryRegister(Type packetType)
        {
            try
            {
                Register(packetType);
                return true;
            } catch (Exception exception)
            {
                switch (exception)
                {
                    // Handle all expected exceptions
                    case ArgumentNullException _:
                    case ArgumentException _:
                    case InvalidOperationException _:
                        Logger.Error(exception);
                        return false;

                    default:
                        // Something has gone horribly wrong if we get here
                        throw;
                }
            }
        }

        internal void Register(Type packetType)
        {
            if (default == packetType)
            {
                throw new ArgumentNullException(nameof(packetType));
            }

            if (!packetType.GetInterfaces().Any(baseInterface => typeof(IPacket) == baseInterface))
            {
                throw new ArgumentException(
                    $"Packet type '{packetType.FullName}' does not inherit from '{typeof(IPacket).FullName}'.",
                    nameof(packetType)
                );
            }

            if (packetType.IsAbstract)
            {
                throw new ArgumentException($"Abstract packet types are not allowed: {packetType.FullName}", nameof(packetType));
            }

            if (packetType.IsInterface)
            {
                throw new ArgumentException($"Interfaces cannot be packets: {packetType.FullName}", nameof(packetType));
            }

            if (packetType.IsValueType)
            {
                throw new ArgumentException($"Value types (structs) cannot be packets: {packetType.FullName}", nameof(packetType));
            }

            if (TypesInternal.Contains(packetType))
            {
                throw new InvalidOperationException($"The packet registry already contains '{packetType.FullName}'.");
            }

            TypesInternal.Add(packetType);
        }
    }
}
