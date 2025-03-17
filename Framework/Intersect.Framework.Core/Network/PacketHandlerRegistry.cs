using Intersect.Collections;
using System.Reflection;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;

namespace Intersect.Network;

public partial class PacketHandlerRegistry : IDisposable
{
    private static Type HandlePacketDelegateType { get; } = typeof(HandlePacket<,>);

    private static Type HandlePacketVoidDelegateType { get; } = typeof(HandlePacketVoid<,>);

    private MethodInfo CreateWeaklyTypedDelegateForMethodInfoInfo { get; }

    private readonly MethodInfo _createHandlerDelegateForTypeInfo;

    private Dictionary<Type, HandlePacketGeneric> Handlers { get; }

    private Dictionary<Type, List<IPacketHandler>> Preprocessors { get; }

    private Dictionary<Type, List<IPacketHandler>> PreHooks { get; }

    private Dictionary<Type, List<IPacketHandler>> PostHooks { get; }

    private Stack<DisposableHandler> DisposableHandlers { get; }

    protected PacketTypeRegistry PacketTypeRegistry { get; }

    protected ILogger Logger { get; }

    public PacketHandlerRegistry(PacketTypeRegistry packetTypeRegistry, ILogger logger)
    {
        PacketTypeRegistry = packetTypeRegistry ?? throw new ArgumentNullException(nameof(packetTypeRegistry));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;

        CreateWeaklyTypedDelegateForMethodInfoInfo = GetType()
            .GetMethod(
                nameof(CreateWeaklyTypedDelegateForMethodInfo), BindingFlags.Instance | BindingFlags.NonPublic
            );

        if (default == CreateWeaklyTypedDelegateForMethodInfoInfo)
        {
            throw new NullReferenceException(
                $"Failed to reflect {nameof(CreateWeaklyTypedDelegateForMethodInfo)}."
            );
        }

        _createHandlerDelegateForTypeInfo = GetType()
            .GetMethod(nameof(CreateHandlerDelegateForType), BindingFlags.Instance | BindingFlags.NonPublic);

        if (default == _createHandlerDelegateForTypeInfo)
        {
            throw new NullReferenceException($"Failed to reflect {nameof(CreateHandlerDelegateForType)}.");
        }

        Handlers = new Dictionary<Type, HandlePacketGeneric>();
        DisposableHandlers = new Stack<DisposableHandler>();

        Preprocessors = new Dictionary<Type, List<IPacketHandler>>();
        PreHooks = new Dictionary<Type, List<IPacketHandler>>();
        PostHooks = new Dictionary<Type, List<IPacketHandler>>();

        TryRegisterAvailableHandlers(typeof(PacketHandlerRegistry).Assembly, requireAttribute: true);
    }

    public int Count => Handlers.Count;

    public bool IsEmpty => Count < 1;

    protected virtual HandlePacketGeneric CreateWeaklyTypedDelegateForMethodInfo<TPacketSender, TPacket>(
        MethodInfo methodInfo,
        object target = null
    ) where TPacketSender : IPacketSender where TPacket : IPacket
    {
        if (methodInfo == null)
        {
            throw new ArgumentNullException(nameof(methodInfo));
        }

        if (typeof(bool) == methodInfo.ReturnType)
        {
            var stronglyTyped =
                Delegate.CreateDelegate(typeof(HandlePacket<TPacketSender, TPacket>), target, methodInfo) as
                    HandlePacket<TPacketSender, TPacket>;

            return (IPacketSender packetSender, IPacket packet) => stronglyTyped(
                (TPacketSender)packetSender, (TPacket)packet
            );
        }

        if (typeof(void) == methodInfo.ReturnType)
        {
            var stronglyTyped = Delegate.CreateDelegate(
                typeof(HandlePacketVoid<TPacketSender, TPacket>), target, methodInfo
            ) as HandlePacketVoid<TPacketSender, TPacket>;

            return (IPacketSender packetSender, IPacket packet) =>
            {
                stronglyTyped((TPacketSender)packetSender, (TPacket)packet);

                return true;
            };
        }

        throw new ArgumentException($"Unsupported packet handler return type '{methodInfo.ReturnType.FullName}'.");
    }

    protected virtual HandlePacketGeneric CreateHandlerDelegateForInstance<TPacket>(
        IPacketHandler packetHandlerGeneric
    ) where TPacket : class, IPacket
    {
        var stronglyTypedPacketHandler = packetHandlerGeneric as IPacketHandler<TPacket>;

        return (IPacketSender packetSender, IPacket packet) =>
            stronglyTypedPacketHandler.Handle(packetSender, (TPacket)packet);
    }

    protected virtual HandlePacketGeneric CreateHandlerDelegateForType<TPacket>(Type handlerType)
        where TPacket : class, IPacket
    {
        var packetHandler = Activator.CreateInstance(handlerType) as IPacketHandler;

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (packetHandler is IDisposable disposable)
        {
            DisposableHandlers.Push(new DisposableHandler(typeof(TPacket), disposable));
        }

        return CreateHandlerDelegateForInstance<TPacket>(packetHandler);
    }

    protected virtual HandlePacketGeneric CreateInstance(
        Type packetSenderType,
        Type packetType,
        MethodInfo methodInfo,
        object target = null
    )
    {
        var typedDelegateFactory =
            CreateWeaklyTypedDelegateForMethodInfoInfo.MakeGenericMethod(packetSenderType, packetType);

        var weakDelegate =
            typedDelegateFactory.Invoke(this, new object[] { methodInfo, target }) as HandlePacketGeneric;

        return weakDelegate;
    }

    protected virtual HandlePacketGeneric CreateInstance(Type packetType, Type type)
    {
        var typedDelegateFactory = _createHandlerDelegateForTypeInfo.MakeGenericMethod(packetType);
        var weakDelegate = typedDelegateFactory.Invoke(this, [type]) as HandlePacketGeneric;

        return weakDelegate;
    }

    public bool HasHandler<TPacket>() where TPacket : IPacket =>TryGetHandler(typeof(TPacket), out _);

    public bool HasHandler(Type packetType) => TryGetHandler(packetType, out _);

    public bool TryGetHandler(Type packetType, out HandlePacketGeneric handlerInstance) =>
        Handlers.TryGetValue(packetType ?? throw new ArgumentNullException(nameof(packetType)), out handlerInstance);

    public bool TryGetHandler<TPacket>(out HandlePacketGeneric handlerInstance) =>
        TryGetHandler(typeof(TPacket), out handlerInstance);

    public bool TryGetHandler(IPacket packet, out HandlePacketGeneric handlerInstance) =>
        TryGetHandler(packet?.GetType(), out handlerInstance);

    public bool TryGetPreprocessors(Type packetType, out IReadOnlyList<IPacketHandler> preprocessors)
    {
        if (!Preprocessors.TryGetValue(packetType, out var handlers))
        {
            preprocessors = default;
            return false;
        }

        preprocessors = handlers?.WrapReadOnly();
        return preprocessors != default;
    }

    public bool TryGetPreprocessors<TPacket>(out IReadOnlyList<IPacketHandler> preprocessors)
        where TPacket : IPacket =>
        TryGetPreprocessors(typeof(TPacket), out preprocessors);

    public bool TryGetPreprocessors(IPacket packet, out IReadOnlyList<IPacketHandler> preprocessors) =>
        TryGetPreprocessors(packet?.GetType(), out preprocessors);

    public bool TryGetPreHooks(Type packetType, out IReadOnlyList<IPacketHandler> preHooks)
    {
        if (!PreHooks.TryGetValue(packetType, out var handlers))
        {
            preHooks = default;
            return false;
        }

        preHooks = handlers?.WrapReadOnly();
        return preHooks != default;
    }

    public bool TryGetPreHooks<TPacket>(out IReadOnlyList<IPacketHandler> preHooks)
        where TPacket : IPacket =>
        TryGetPreHooks(typeof(TPacket), out preHooks);

    public bool TryGetPreHooks(IPacket packet, out IReadOnlyList<IPacketHandler> preHooks) =>
        TryGetPreHooks(packet?.GetType(), out preHooks);

    public bool TryGetPostHooks(Type packetType, out IReadOnlyList<IPacketHandler> postHooks)
    {
        if (!PostHooks.TryGetValue(packetType, out var handlers))
        {
            postHooks = default;
            return false;
        }

        postHooks = handlers?.WrapReadOnly();
        return postHooks != default;
    }

    public bool TryGetPostHooks<TPacket>(out IReadOnlyList<IPacketHandler> postHooks)
        where TPacket : IPacket =>
        TryGetPostHooks(typeof(TPacket), out postHooks);

    public bool TryGetPostHooks(IPacket packet, out IReadOnlyList<IPacketHandler> postHooks) =>
        TryGetPostHooks(packet?.GetType(), out postHooks);

    protected static Type GetPacketSenderType(MethodInfo methodInfo)
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
        if (!PacketHandlerAttribute.TypeIPacketSender.IsAssignableFrom(packetSenderType))
        {
            throw new ArgumentException(
                $"Invalid sender parameter type ({packetSenderType.FullName}) in handler '{methodInfo.GetFullSignature()}'.",
                nameof(methodInfo)
            );
        }

        return packetSenderType;
    }

    protected virtual bool TryRegister(IEnumerable<MethodInfo> methodInfos, object target = null)
    {
        if (default == methodInfos)
        {
            throw new ArgumentNullException(nameof(methodInfos));
        }

        foreach (var methodInfo in methodInfos)
        {
            try
            {
                var packetType = PacketHandlerAttribute.GetPacketType(methodInfo);
                if (!PacketTypeRegistry.IsRegistered(packetType))
                {
                    Logger.LogError($"The packet type '{packetType.FullName}' has not been registered, a handler cannot be added.");
                    return false;
                }

                if (HasHandler(packetType))
                {
                    Logger.LogError($"There is already a packet handler for '{packetType.FullName}'.");
                    return false;
                }

                var instance = CreateInstance(GetPacketSenderType(methodInfo), packetType, methodInfo, target);
                Handlers.Add(packetType, instance);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Logger.LogError(
                    exception,
                    "Error registering handler for {Method} on {Type}",
                    methodInfo.Name,
                    methodInfo.DeclaringType?.GetName(qualified: true)
                );

                return false;
            }
        }

        return true;
    }

    protected virtual bool TryRegister(IEnumerable<Type> handlerTypes)
    {
        if (default == handlerTypes)
        {
            throw new ArgumentNullException(nameof(handlerTypes));
        }

        foreach (var handlerType in handlerTypes)
        {
            var packetTypes = PacketHandlerAttribute.GetPacketTypes(handlerType);
            foreach (var packetType in packetTypes)
            {
                if (!PacketTypeRegistry.IsRegistered(packetType))
                {
                    Logger.LogError(
                        "The packet type {PacketType} has not been registered, a the handler type {HandlerType} cannot be added.",
                        packetType.GetName(qualified: true),
                        handlerType.GetName(qualified: true)
                    );
                    return false;
                }

                if (TryGetHandler(packetType, out var existingHandler))
                {
                    Logger.LogError(
                        "There is already a packet handler ({ExistingHandlerType}) for '{PacketType}'",
                        existingHandler.GetType().GetName(qualified: true),
                        packetType.GetName(qualified: true)
                    );
                    return false;
                }

                var instance = CreateInstance(packetType, handlerType);
                Handlers.Add(packetType, instance);
            }
        }

        return true;
    }

    protected virtual bool TryRegister(Dictionary<Type, List<IPacketHandler>> collection, Type handlerType, Type packetType, out IPacketHandler handler)
    {
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));
        ArgumentNullException.ThrowIfNull(handlerType, nameof(handlerType));
        ArgumentNullException.ThrowIfNull(packetType, nameof(packetType));

        handler = Activator.CreateInstance(handlerType) as IPacketHandler;
        return TryRegister(collection, handlerType, packetType, handler);
    }

    protected virtual bool TryRegister(Dictionary<Type, List<IPacketHandler>> collection, Type handlerType, Type packetType, IPacketHandler handler)
    {
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));
        ArgumentNullException.ThrowIfNull(handlerType, nameof(handlerType));
        ArgumentNullException.ThrowIfNull(packetType, nameof(packetType));
        ArgumentNullException.ThrowIfNull(handler, nameof(handler));

        var expectedPacketTypes = PacketHandlerAttribute.GetPacketTypes(handlerType);
        if (!expectedPacketTypes.Contains(packetType))
        {
            Logger.LogError(
                "Handler {HandlerTypeName} is incompatible with {ActualPacketTypeName}, expected one of {ExpectedPacketTypeNames}.",
                handlerType.GetName(qualified: true),
                packetType.GetName(qualified: true),
                string.Join(", ", expectedPacketTypes.Select(type => type.GetName(qualified: true)))
            );
            return false;
        }

        if (!PacketTypeRegistry.IsRegistered(packetType))
        {
            Logger.LogError(
                "The packet type '{PacketTypeName}' has not been registered, a handler cannot be added.",
                packetType.GetName(qualified: true)
            );
            return false;
        }

        if (!collection.TryGetValue(packetType, out var handlerList))
        {
            handlerList = [];
            collection.Add(packetType, handlerList);
        }

        handlerList.Add(handler);
        return true;
    }

    protected virtual bool TryRegister<THandler, TPacket>(Dictionary<Type, List<IPacketHandler>> collection, out THandler handler)
        where TPacket : class, IPacket
        where THandler : IPacketHandler<TPacket>
    {
        if (TryRegister(collection, typeof(THandler), typeof(TPacket), out var genericHandler))
        {
            handler = (THandler)genericHandler;
            return true;
        }

        handler = default;
        return false;
    }

    protected virtual bool TryRegister<THandler, TPacket>(Dictionary<Type, List<IPacketHandler>> collection, THandler handler)
        where TPacket : class, IPacket
        where THandler : IPacketHandler<TPacket>
    {
        return TryRegister(collection, typeof(THandler), typeof(TPacket), handler);
    }

    public virtual bool TryRegisterPreprocessor(Type handlerType, Type packetType, out IPacketHandler handler) =>
        TryRegister(Preprocessors, handlerType, packetType, out handler);

    public bool TryRegisterPreprocessor<THandler, TPacket>(out THandler handler)
        where TPacket : class, IPacket
        where THandler : IPacketHandler<TPacket>
        => TryRegister<THandler, TPacket>(Preprocessors, out handler);

    public virtual bool TryRegisterPreHook(Type handlerType, Type packetType, out IPacketHandler handler) =>
        TryRegister(PreHooks, handlerType, packetType, out handler);

    public bool TryRegisterPreHook<THandler, TPacket>(out THandler handler)
        where TPacket : class, IPacket
        where THandler : IPacketHandler<TPacket>
        => TryRegister<THandler, TPacket>(PreHooks, out handler);

    public bool TryRegisterPreHook<THandler, TPacket>(THandler handler)
        where TPacket : class, IPacket
        where THandler : IPacketHandler<TPacket>
    {
        return TryRegister<THandler, TPacket>(PreHooks, handler);
    }

    public virtual bool TryRegisterPostHook(Type handlerType, Type packetType, out IPacketHandler handler) =>
        TryRegister(PostHooks, handlerType, packetType, out handler);

    public bool TryRegisterPostHook<THandler, TPacket>(out THandler handler)
        where TPacket : class, IPacket
        where THandler : IPacketHandler<TPacket>
        => TryRegister<THandler, TPacket>(PostHooks, out handler);

    public bool TryRegisterPostHook<THandler, TPacket>(THandler handler)
        where TPacket : class, IPacket
        where THandler : IPacketHandler<TPacket>
    {
        return TryRegister<THandler, TPacket>(PostHooks, handler);
    }

    public virtual bool TryRegisterHandler(Type handlerType, out IPacketHandler handler)
    {
        handler = default;

        var packetTypes = PacketHandlerAttribute.GetPacketTypes(handlerType);
        foreach (var packetType in packetTypes)
        {
            if (!PacketTypeRegistry.IsRegistered(packetType))
            {
                Logger.LogError("The packet type '{PacketType}' has not been registered, a {Ha cannot be added.");
                return false;
            }

            if (HasHandler(packetType))
            {
                Logger.LogError("There is already a packet handler for '{packetType.FullName}'.");
                return false;
            }

            var instance = CreateInstance(packetType, handlerType);
            Handlers.Add(packetType, instance);
        }
        return true;
    }

    public bool TryRegisterHandler<THandler>(out THandler handler) where THandler : IPacketHandler
    {
        handler = default;
        if (TryRegisterHandler(typeof(THandler), out var genericHandler))
        {
            handler = (THandler)genericHandler;
            return true;
        }

        return false;
    }

    // TODO: Expose this
    bool TryRegisterAvailableHandlers(Assembly assembly, bool requireAttribute = true) =>
        TryRegisterAvailableMethodHandlers(assembly, requireAttribute) &&
        TryRegisterAvailableTypeHandlers(assembly, requireAttribute);

    // TODO: Expose this
    bool TryRegisterAvailableMethodHandlers(Assembly assembly, bool requireAttribute = true) =>
        TryRegister(DiscoverMethods(assembly, requireAttribute));

    public bool TryRegisterAvailableMethodHandlers(Type type, object target = null, bool requireAttribute = true) =>
        TryRegister(
            DiscoverMethods(type, target == null ? BindingFlags.Static : BindingFlags.Instance, requireAttribute),
            target
        );

    public bool TryRegisterAvailableTypeHandlers(Assembly assembly, bool requireAttribute = true) =>
        TryRegister(DiscoverTypes(assembly, requireAttribute));

    /// <summary>
    /// Discovers all <see langword="static"/> methods in a given assembly that are valid packet handlers, by default requiring a <see cref="PacketHandlerAttribute"/>.
    ///
    /// Note: Changing <paramref name="requireAttribute"/> to <see langword="false"/> is very computationally expensive.
    /// </summary>
    /// <param name="assembly">the <see cref="Assembly"/> to scan</param>
    /// <param name="requireAttribute">if <see cref="PacketHandlerAttribute"/> needs to be specified (default <see langword="true"/>, <see langword="false"/> is slow)</param>
    /// <returns>an <see cref="IEnumerable{T}"/> of valid <see cref="MethodInfo"/>s</returns>
    public static IEnumerable<MethodInfo> DiscoverMethods(Assembly assembly, bool requireAttribute = true)
    {
        if (default == assembly)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        var assemblyTypes = assembly.GetTypes();
        var methodInfos = assemblyTypes.SelectMany(
            assemblyType => DiscoverMethods(assemblyType, BindingFlags.Static, requireAttribute)
        );

        return methodInfos;
    }

    /// <summary>
    /// Discovers all <see langword="static"/> methods in a given type that are valid packet handlers, by default requiring a <see cref="PacketHandlerAttribute"/>.
    ///
    /// Note: Changing <paramref name="requireAttribute"/> to <see langword="false"/> is very computationally expensive.
    /// </summary>
    /// <param name="type">the <see cref="Type"/> to scan</param>
    /// <param name="extraBindingFlags">extra <see cref="BindingFlags"/> (besides <see cref="BindingFlags.Public"/>) to filter the methods by</param>
    /// <param name="requireAttribute">if <see cref="PacketHandlerAttribute"/> needs to be specified (default <see langword="true"/>, <see langword="false"/> is slow)</param>
    /// <returns>an <see cref="IEnumerable{T}"/> of valid <see cref="MethodInfo"/>s</returns>
    public static IEnumerable<MethodInfo> DiscoverMethods(
        Type type,
        BindingFlags extraBindingFlags,
        bool requireAttribute = true
    )
    {
        if (default == type)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var methodInfos = type.GetMethods(BindingFlags.Public | extraBindingFlags)
            .Where(
                methodInfo =>
                    (!requireAttribute || Attribute.IsDefined(methodInfo, typeof(PacketHandlerAttribute))) &&
                    PacketHandlerAttribute.IsValidHandler(methodInfo)
            );

        return methodInfos;
    }

    /// <summary>
    /// Discovers all <see cref="Type"/>s in a given assembly that are valid packet handlers, by default requiring a <see cref="PacketHandlerAttribute"/>.
    ///
    /// Note: Changing <paramref name="requireAttribute"/> to <see langword="false"/> is very computationally expensive.
    /// </summary>
    /// <param name="assembly">the <see cref="Assembly"/> to scan</param>
    /// <param name="requireAttribute">if <see cref="PacketHandlerAttribute"/> needs to be specified (default <see langword="true"/>, <see langword="false"/> is slow)</param>
    /// <returns>an <see cref="IEnumerable{T}"/> of valid <see cref="Type"/>s</returns>
    public static IEnumerable<Type> DiscoverTypes(Assembly assembly, bool requireAttribute = true)
    {
        if (default == assembly)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        var assemblyTypes = assembly.GetTypes();
        var packetHandlerTypes = assemblyTypes.Where(
            type => (!requireAttribute || Attribute.IsDefined(type, typeof(PacketHandlerAttribute))) &&
                    PacketHandlerAttribute.IsValidHandler(type)
        );

        return packetHandlerTypes;
    }

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            while (DisposableHandlers.Count > 0)
            {
                var disposableHandler = DisposableHandlers.Pop();
                Handlers.Remove(disposableHandler.PacketType);
                disposableHandler.Dispose();
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
