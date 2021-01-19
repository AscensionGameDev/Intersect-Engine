using Intersect.Logging;
using Intersect.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Intersect.Network
{
    public partial class PacketHandlerRegistry : IDisposable
    {
        private static Type HandlePacketDelegateType { get; } = typeof(HandlePacket<,>);

        private static Type HandlePacketVoidDelegateType { get; } = typeof(HandlePacketVoid<,>);

        private MethodInfo CreateWeaklyTypedDelegateForMethodInfoInfo { get; }

        private MethodInfo CreateHandlerDelegateForTypeInfo { get; }

        private Dictionary<Type, HandlePacketGeneric> Handlers { get; }

        private Stack<DisposableHandler> DisposableHandlers { get; }

        public Logger Logger { get; }

        public PacketHandlerRegistry(Logger logger)
        {
            if (default == logger)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;

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

            CreateHandlerDelegateForTypeInfo = GetType()
                .GetMethod(nameof(CreateHandlerDelegateForType), BindingFlags.Instance | BindingFlags.NonPublic);

            if (default == CreateHandlerDelegateForTypeInfo)
            {
                throw new NullReferenceException($"Failed to reflect {nameof(CreateHandlerDelegateForType)}.");
            }

            Handlers = new Dictionary<Type, HandlePacketGeneric>();
            DisposableHandlers = new Stack<DisposableHandler>();
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
                    (TPacketSender) packetSender, (TPacket) packet
                );
            }

            if (typeof(void) == methodInfo.ReturnType)
            {
                var stronglyTyped = Delegate.CreateDelegate(
                    typeof(HandlePacketVoid<TPacketSender, TPacket>), target, methodInfo
                ) as HandlePacketVoid<TPacketSender, TPacket>;

                return (IPacketSender packetSender, IPacket packet) =>
                {
                    stronglyTyped((TPacketSender) packetSender, (TPacket) packet);

                    return true;
                };
            }

            throw new ArgumentException($"Unsupported packet handler return type '{methodInfo.ReturnType.FullName}'.");
        }

        protected virtual HandlePacketGeneric CreateHandlerDelegateForInstance<TPacket>(
            IPacketHandler packetHandlerGeneric
        ) where TPacket : IPacket
        {
            var stronglyTypedPacketHandler = packetHandlerGeneric as IPacketHandler<TPacket>;

            return (IPacketSender packetSender, IPacket packet) =>
                stronglyTypedPacketHandler.Handle(packetSender, (TPacket) packet);
        }

        protected virtual HandlePacketGeneric CreateHandlerDelegateForType<TPacket>(Type handlerType)
            where TPacket : IPacket
        {
            var packetHandler = Activator.CreateInstance(handlerType) as IPacketHandler;

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
                typedDelegateFactory.Invoke(this, new object[] {methodInfo, target}) as HandlePacketGeneric;

            return weakDelegate;
        }

        protected virtual HandlePacketGeneric CreateInstance(Type packetType, Type type)
        {
            var typedDelegateFactory = CreateHandlerDelegateForTypeInfo.MakeGenericMethod(packetType);
            var weakDelegate = typedDelegateFactory.Invoke(this, new object[] {type}) as HandlePacketGeneric;

            return weakDelegate;
        }

        public bool HasHandler<TPacket>() where TPacket : IPacket => HasHandler(typeof(TPacket));

        public bool HasHandler(Type packetType) => Handlers.ContainsKey(packetType);

        public bool TryGetHandler(IPacket packet, out HandlePacketGeneric handlerInstance) =>
            Handlers.TryGetValue(packet?.GetType(), out handlerInstance);

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
                    if (HasHandler(packetType))
                    {
                        Logger.Error($"There is already a packet handler for {packetType.FullName}.");

                        return false;
                    }

                    var instance = CreateInstance(GetPacketSenderType(methodInfo), packetType, methodInfo, target);
                    Handlers.Add(packetType, instance);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    Logger.Error(exception);

                    return false;
                }
            }

            return true;
        }

        protected virtual bool TryRegister(IEnumerable<Type> types)
        {
            if (default == types)
            {
                throw new ArgumentNullException(nameof(types));
            }

            foreach (var type in types)
            {
                var packetType = PacketHandlerAttribute.GetPacketType(type);
                if (HasHandler(packetType))
                {
                    Logger.Error($"There is already a packet handler for {packetType.FullName}.");

                    return false;
                }

                var instance = CreateInstance(packetType, type);
                Handlers.Add(packetType, instance);
            }

            return true;
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

        // TODO: Expose this
        bool TryRegisterAvailableTypeHandlers(Assembly assembly, bool requireAttribute = true) =>
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
}
