using System.Reflection;

using Intersect.Collections;
using Intersect.Logging;

namespace Intersect.Network
{
    public sealed partial class PacketTypeRegistry
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

        public bool TryRegisterBuiltIn() =>
            TryRegisterFromAssembly(BuiltInAssembly, builtIn: true);

        public bool TryRegisterFromAssembly(Assembly assembly) =>
            TryRegisterFromAssembly(assembly, builtIn: false);

        private bool TryRegisterFromAssembly(Assembly assembly, bool builtIn)
        {
            if (assembly.IsDynamic)
            {
                return false;
            }

            IEnumerable<Type> exportedTypes;
            try
            {
                exportedTypes = assembly.ExportedTypes;
            }
            catch
            {
                exportedTypes = Array.Empty<Type>();
            }

            var definedPacketTypes = exportedTypes.Where(
                type =>
                    !type.IsInterface
                    && !type.IsValueType
                    && !type.IsAbstract
                    && type.GetInterfaces().Any(interfaceType => interfaceType == typeof(IPacket))
            );

            if (builtIn)
            {
                BuiltInTypesInternal.AddRange(definedPacketTypes);
                return BuiltInTypesInternal.All(TryRegister) && BuiltInTypesInternal.Count > 0;
            }

            return definedPacketTypes.All(TryRegister);
        }

        public bool TryRegisterLoadedAssemblies()
        {
            var currentDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            return currentDomainAssemblies
                .Where(assembly => !assembly.IsDynamic)
                .All(TryRegisterFromAssembly);
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
                return;
            }

            TypesInternal.Add(packetType);
        }
    }
}
