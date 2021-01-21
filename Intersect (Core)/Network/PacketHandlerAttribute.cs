using Intersect.Reflection;

using System;
using System.Linq;
using System.Reflection;

namespace Intersect.Network
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Delegate | AttributeTargets.Struct,
        AllowMultiple = false
    )]
    public class PacketHandlerAttribute : Attribute
    {
        internal static Type TypeIPacketHandlerInterface_1 = typeof(IPacketHandler<>);

        internal static Type TypeIPacket = typeof(IPacket);

        internal static Type TypeIPacketSender = typeof(IPacketSender);

        public Type PacketType { get; }

        public PacketHandlerAttribute(Type packetType)
        {
            if (packetType == default)
            {
                throw new ArgumentNullException(nameof(packetType));
            }

            if (!typeof(IPacket).IsAssignableFrom(packetType))
            {
                throw new ArgumentException(
                    $"{packetType.FullName} does not implement {typeof(IPacket).FullName}.", nameof(packetType)
                );
            }

            PacketType = packetType;
        }

        public static Type GetPacketType(MethodInfo methodInfo)
        {
            if (default == methodInfo)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var parameterTypes = methodInfo.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
            if (parameterTypes.Length != 2)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(methodInfo), $"Invalid packet handler '{methodInfo.GetFullSignature()}'."
                );
            }

            var packetSenderType = parameterTypes[0];
            if (!TypeIPacketSender.IsAssignableFrom(packetSenderType))
            {
                throw new ArgumentException(
                    $"Invalid sender parameter type ({packetSenderType.FullName}) in handler '{methodInfo.GetFullSignature()}'.",
                    nameof(methodInfo)
                );
            }

            var packetTypeFromMethod = parameterTypes[1];
            if (packetTypeFromMethod.IsInterface ||
                packetTypeFromMethod.IsAbstract ||
                packetTypeFromMethod.IsGenericType ||
                !TypeIPacket.IsAssignableFrom(packetTypeFromMethod))
            {
                throw new ArgumentException(
                    $"Invalid packet parameter type ({packetTypeFromMethod.FullName}) in handler '{methodInfo.GetFullSignature()}'.",
                    nameof(methodInfo)
                );
            }

            var packetHandlerAttribute =
                GetCustomAttribute(methodInfo, typeof(PacketHandlerAttribute)) as PacketHandlerAttribute;

            var packetTypeFromAttribute = packetHandlerAttribute?.PacketType;
            var expectedPacketType = packetTypeFromAttribute ?? packetTypeFromMethod;

            if (packetTypeFromMethod != expectedPacketType)
            {
                throw new ArgumentException(
                    $"{methodInfo.Name} is a packet handler for {packetTypeFromMethod.FullName} but is marked with an attribute for {packetTypeFromAttribute.FullName}.",
                    nameof(methodInfo)
                );
            }

            return expectedPacketType;
        }

        public static Type GetHandlerInterface(Type implementationType)
        {
            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            return implementationType.GetInterfaces()
                .FirstOrDefault(
                    interfaceType => interfaceType.IsGenericType &&
                                     interfaceType.GetGenericTypeDefinition() == TypeIPacketHandlerInterface_1
                );
        }

        public static Type GetPacketType<THandler>() where THandler : IPacketHandler =>
            GetPacketType(typeof(THandler));

        public static Type GetPacketType(Type packetHandlerType)
        {
            if (packetHandlerType == default)
            {
                throw new ArgumentNullException(nameof(packetHandlerType));
            }

            var interfaceType = GetHandlerInterface(packetHandlerType);
            if (interfaceType == null)
            {
                throw new ArgumentException(
                    $"{packetHandlerType.FullName} does not implement {TypeIPacketHandlerInterface_1.FullName}.",
                    nameof(packetHandlerType)
                );
            }

            var packetTypeFromType = interfaceType.GenericTypeArguments.FirstOrDefault();
            if (packetTypeFromType.IsInterface ||
                packetTypeFromType.IsAbstract ||
                packetTypeFromType.IsGenericType ||
                !TypeIPacket.IsAssignableFrom(packetTypeFromType))
            {
                throw new ArgumentException(
                    $"Invalid packet generic type ({packetTypeFromType.FullName}) in handler declaration '{packetHandlerType.FullName}'.",
                    nameof(packetHandlerType)
                );
            }

            var packetHandlerAttribute =
                GetCustomAttribute(packetHandlerType, typeof(PacketHandlerAttribute)) as PacketHandlerAttribute;

            var packetTypeFromAttribute = packetHandlerAttribute?.PacketType;
            var expectedPacketType = packetTypeFromAttribute ?? packetTypeFromType;

            if (packetTypeFromType != expectedPacketType)
            {
                throw new ArgumentException(
                    $"{packetHandlerType.FullName} is a packet handler for {packetTypeFromType.FullName} but is marked with an attribute for {packetTypeFromAttribute.FullName}.",
                    nameof(packetHandlerType)
                );
            }

            return expectedPacketType;
        }

        public static bool IsValidHandler(MethodInfo methodInfo)
        {
            if (methodInfo == default)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            try
            {
                return GetPacketType(methodInfo) != default;
            }
            catch (ArgumentNullException)
            {
                throw;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return false;
            }
        }

        public static bool IsValidHandler(Type type)
        {
            if (type == default)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsInterface || type.IsAbstract || type.IsGenericType)
            {
                return false;
            }

            try
            {
                return GetPacketType(type) != default;
            }
            catch (ArgumentNullException)
            {
                throw;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return false;
            }
        }
    }
}
