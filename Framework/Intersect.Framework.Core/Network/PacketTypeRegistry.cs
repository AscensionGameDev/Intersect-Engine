using System.Globalization;
using Intersect.Collections;
using System.Reflection;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;

namespace Intersect.Network;

public sealed partial class PacketTypeRegistry
{
    private Assembly _builtinAssembly;

    private ILogger Logger { get; }

    private List<Type> BuiltInTypesInternal { get; }

    private List<Type> TypesInternal { get; }

    public IReadOnlyList<Type> BuiltInTypes { get; }

    public IReadOnlyList<Type> Types { get; }

    public PacketTypeRegistry(ILogger logger, Assembly builtinAssembly)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _builtinAssembly = builtinAssembly;

        BuiltInTypesInternal = [];
        BuiltInTypes = BuiltInTypesInternal.WrapReadOnly();

        TypesInternal = [];
        Types = TypesInternal.WrapReadOnly();
    }

    public bool IsRegistered(Type type) =>
        default == type ? throw new ArgumentNullException(nameof(type)) : TypesInternal.Contains(type);

    public bool TryRegisterBuiltIn()
    {
        var types = _builtinAssembly.GetExportedTypes();
        var packetTypes = types.Where(type => type.GetInterfaces().Any(interfaceType => interfaceType == typeof(IPacket)));
        var definedPacketTypes = packetTypes
            .Where(type => !type.IsInterface && !type.IsValueType && !type.IsAbstract)
            .SelectMany(
                type =>
                {
                    if (!type.IsGenericType)
                    {
                        return Enumerable.Empty<Type>().Append(type);
                    }

                    var genericPacketTypeArgumentsAttributes =
                        type.GetCustomAttributes<GenericPacketTypeArgumentsAttribute>();
                    return genericPacketTypeArgumentsAttributes.Select(
                        attribute => type.MakeGenericType(attribute.TypeArguments)
                    );
                }
            )
            .OrderBy(
                type => type.GetName(qualified: true),
                CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.Ordinal)
            );
        BuiltInTypesInternal.AddRange(definedPacketTypes);
        return BuiltInTypesInternal.All(TryRegister) && BuiltInTypesInternal.Count > 0;
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
                    Logger.LogError(
                        exception,
                        "Failed to register packet type {Type}",
                        packetType.GetName(qualified: true)
                    );
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
