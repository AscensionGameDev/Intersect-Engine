using System.Reflection;
using Intersect.Framework.Reflection;

namespace Intersect.Network;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Delegate | AttributeTargets.Struct,
    AllowMultiple = false
)]
public partial class PacketHandlerAttribute : Attribute
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
            (packetTypeFromMethod.IsGenericType && !packetTypeFromMethod.IsConstructedGenericType) ||
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

    public static Type[] GetHandlerInterfaces(Type implementationType)
    {
        ArgumentNullException.ThrowIfNull(implementationType, nameof(implementationType));

        var interfaces = implementationType.GetInterfaces()
            .Where(
                interfaceType => interfaceType.IsGenericType &&
                                 interfaceType.GetGenericTypeDefinition() == TypeIPacketHandlerInterface_1
            )
            .ToArray();

        if (interfaces.Length < 1)
        {
            throw new ArgumentException(
                $"{implementationType.GetName(qualified: true)} does not directly implement {TypeIPacketHandlerInterface_1.GetName(qualified: true)}",
                nameof(implementationType)
            );
        }

        return interfaces;
    }

    public static Type[] GetPacketTypes<THandler>() where THandler : IPacketHandler =>
        GetPacketTypes(typeof(THandler));

    public static Type[] GetPacketTypes(Type packetHandlerType)
    {
        ArgumentNullException.ThrowIfNull(packetHandlerType, nameof(packetHandlerType));

        var interfaceTypes = GetHandlerInterfaces(packetHandlerType);
        if (interfaceTypes.Length < 1)
        {
            throw new ArgumentException(
                $"{packetHandlerType.GetName(qualified: true)} does not implement {TypeIPacketHandlerInterface_1.GetName(qualified: true)}.",
                nameof(packetHandlerType)
            );
        }

        var genericTypeArguments = interfaceTypes.SelectMany(interfaceType => interfaceType.GenericTypeArguments).Distinct().ToArray();
        var packetTypesFromType = genericTypeArguments.Where(
                packetTypeFromType =>
                    packetTypeFromType is { IsInterface: false, IsAbstract: false, IsGenericType: false } &&
                    packetTypeFromType.Extends(TypeIPacket)
            )
            .ToArray();

        if (packetTypesFromType.Length < 1)
        {
            throw new ArgumentException(
                $"{packetHandlerType.GetName(qualified: true)} handles no packet types",
                nameof(packetHandlerType)
            );
        }

        var packetHandlerAttribute =
            GetCustomAttribute(packetHandlerType, typeof(PacketHandlerAttribute)) as PacketHandlerAttribute;

        var packetTypeFromAttribute = packetHandlerAttribute?.PacketType;
        var expectedPacketTypes = packetTypeFromAttribute == null ? packetTypesFromType : [packetTypeFromAttribute];

        if (!packetTypesFromType.SequenceEqual(expectedPacketTypes))
        {
            var packetHandlerTypeName = packetHandlerType.GetName(qualified: true);
            var packetTypeNames = string.Join(", ", packetTypesFromType.Select(type => type.GetName(qualified: true)));
            var expectedPacketTypeNames = string.Join(", ", expectedPacketTypes.Select(type => type.GetName(qualified: true)));
            throw new ArgumentException(
                $"{packetHandlerTypeName} is a packet handler for {packetTypeNames} but is marked with an attribute for {expectedPacketTypeNames}.",
                nameof(packetHandlerType)
            );
        }

        return expectedPacketTypes;
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
            return GetPacketTypes(type).Length > 0;
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
