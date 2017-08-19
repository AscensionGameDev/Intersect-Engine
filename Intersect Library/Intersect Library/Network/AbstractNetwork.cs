using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intersect.Logging;
using Intersect.Memory;

namespace Intersect.Network
{
    public abstract class AbstractNetwork : INetwork
    {
        protected readonly IDictionary<Guid, IConnection> mConnections;

        private readonly IDictionary<PacketCode, HandlePacket> mHandlers;

        private readonly List<INetworkLayerInterface> mNetworkLayerInterfaces;
        private bool mDisposed;

        protected AbstractNetwork(NetworkConfiguration configuration)
        {
            mDisposed = false;

            mNetworkLayerInterfaces = new List<INetworkLayerInterface>();

            mConnections = CreateDictionaryLegacy<Guid, IConnection>();

            Configuration = configuration;

            mHandlers = new SortedDictionary<PacketCode, HandlePacket>();
            PacketFactories = new HashSet<IPacketFactory> {ReflectablePacketFactory.Instance};
        }

        public ICollection<IConnection> Connections => mConnections?.Values;
        public IDictionary<Guid, IConnection> ConnectionLookup => mConnections;
        public IDictionary<PacketCode, HandlePacket> Handlers => mHandlers;
        protected ICollection<IPacketFactory> PacketFactories { get; }
        public int ConnectionCount => Connections?.Count ?? 0;

        public NetworkConfiguration Configuration { get; }
        public abstract Guid Guid { get; }

        public bool AddConnection(IConnection connection)
        {
            if (connection == null) throw new ArgumentNullException();
            mConnections?.Add(connection.Guid, connection);
            return mConnections?.ContainsKey(connection.Guid) ?? false;
        }

        public bool RemoveConnection(IConnection connection)
        {
            if (connection == null) throw new ArgumentNullException();
            if (connection.IsConnected) return false;
            return mConnections?.Remove(connection.Guid) ?? false;
        }

        public void Dispose()
        {
            lock (this)
            {
                if (mDisposed) return;
                mDisposed = true;
            }

            if (!Disconnect("disposing"))
                Log.Error("Error disconnecting while disposing.");

            mConnections?.Clear();
        }

        public bool Disconnect(string message = "") => Disconnect(Connections, message);

        public bool Disconnect(Guid guid, string message = "")
            => Disconnect(FindConnection(guid), message);

        public bool Disconnect(IConnection connection, string message = "")
            => Disconnect(new[] {connection}, message);

        public bool Disconnect(ICollection<Guid> guids, string message = "")
            => Disconnect(FindConnections(guids), message);

        public bool Disconnect(ICollection<IConnection> connections, string message = "")
        {
            mNetworkLayerInterfaces?.ForEach(
                networkLayerInterface => networkLayerInterface?.Disconnect(connections, message));
            return true;
        }

        public abstract bool Send(IPacket packet);

        public bool Send(Guid guid, IPacket packet)
        {
            var connection = FindConnection(guid);
            return connection != null && Send(connection, packet);
        }

        public abstract bool Send(IConnection connection, IPacket packet);

        public bool Send(ICollection<Guid> guids, IPacket packet)
            => Send(FindConnections(guids), packet);

        public abstract bool Send(ICollection<IConnection> connections, IPacket packet);

        public IConnection FindConnection(Guid guid)
        {
            Debug.Assert(ConnectionLookup != null, "ConnectionLookup != null");
            if (ConnectionLookup.TryGetValue(guid, out IConnection connection)) return connection;

            Log.Diagnostic($"Could not find connection for guid {guid}.");
            return null;
        }

        public TConnection FindConnection<TConnection>(Guid guid) where TConnection : class, IConnection
        {
            return FindConnection(guid) as TConnection;
        }

        public TConnection FindConnection<TConnection>(Func<TConnection, bool> selector)
            where TConnection : class, IConnection
        {
            Debug.Assert(selector != null, "selector != null");
            var connections = FindConnections<TConnection>();
            Debug.Assert(connections != null, "connections != null");
            return connections.FirstOrDefault(selector);
        }

        public ICollection<IConnection> FindConnections(ICollection<Guid> guids)
        {
            var connections = new List<IConnection>(guids?.Count ?? 0);
            connections.AddRange((guids ?? new Guid[0]).Select(FindConnection).Where(connection => connection != null));
            return connections;
        }

        public ICollection<TConnection> FindConnections<TConnection>() where TConnection : class, IConnection
        {
            return Connections?.OfType<TConnection>().ToList();
        }

        protected void AddNetworkLayerInterface(INetworkLayerInterface networkLayerInterface)
        {
            if (mNetworkLayerInterfaces == null) throw new ArgumentNullException(nameof(mNetworkLayerInterfaces));
            if (networkLayerInterface == null) throw new ArgumentNullException(nameof(networkLayerInterface));
            networkLayerInterface.OnPacketAvailable += HandleInboundMessageAvailable;
            mNetworkLayerInterfaces.Add(networkLayerInterface);
        }

        private void HandleInboundMessageAvailable(INetworkLayerInterface sender)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (!sender.TryGetInboundBuffer(out IBuffer buffer, out IConnection connection))
            {
                Log.Error($"Failed to obtain packet when told a packet was available.");
                return;
            }

            HandleInboundData(buffer, connection);

            sender.ReleaseInboundBuffer(buffer);
        }

        protected void AddPacketHandler(PacketCode packetCode, HandlePacket handler)
        {
            Debug.Assert(Handlers != null, "Handlers != null");
            Handlers[packetCode] += handler;
        }

        protected void RemovePacketHandler(PacketCode packetCode, HandlePacket handler)
        {
            Debug.Assert(Handlers != null, "Handlers != null");
            if (!Handlers.ContainsKey(packetCode)) return;
            // ReSharper disable once DelegateSubtraction
            Handlers[packetCode] -= handler;
        }

        protected virtual void HandleConnectionApproved(IConnection connection)
        {
        }

        protected virtual void HandleConnected(IConnection connection)
        {
            connection?.HandleConnected();
        }

        protected virtual void HandleDisconnected(IConnection connection)
        {
            connection?.HandleDisconnected();
        }

        private void HandleInboundData(IBuffer buffer, IConnection connection)
        {
            Debug.Assert(PacketFactories != null, "PacketFactories != null");

            if (buffer == default(IBuffer)) return;
            if (buffer.Length < 1) return;

            //if (!buffer.Read(out byte[] guidData, 16)) return;
            //var guid = new Guid(guidData);
            var packetCode = (PacketCode) buffer.ReadByte();

            IPacket packet = null;
            foreach (var packetFactory in PacketFactories)
            {
                if (!packetFactory.CanCreatePacketType(packetCode)) continue;
                packet = packetFactory.Create(packetCode, connection);
            }

            if (packet == null)
            {
                Log.Debug($"Could not find a factory for packet of type {packetCode}.");
                return;
            }

            if (!packet.Read(ref buffer))
            {
                Log.Debug($"Error reading packet of type {packetCode} from the buffer.");
                return;
            }

            Debug.Assert(Handlers != null, "Handlers != null");
            if (!Handlers.ContainsKey(packetCode))
            {
                Log.Debug($"No handlers registered for packet type {packetCode}.");
                return;
            }

            var handler = Handlers[packetCode];
            if (!(handler?.Invoke(packet) ?? false))
            {
                Log.Debug($"Error invoking handler for packet type {packetCode} (handler={handler}).");
                return;
            }

            //Log.Diagnostic($"Handled inbound {packet.Code} successfully.");
        }

        protected abstract IDictionary<TKey, TValue> CreateDictionaryLegacy<TKey, TValue>();

        protected void StartInterfaces()
        {
            mNetworkLayerInterfaces?.ForEach(networkLayerInterface => networkLayerInterface?.Start());
        }

        protected void SendPacket(IPacket packet, IConnection connection,
            TransmissionMode transmissionMode = TransmissionMode.All)
        {
            mNetworkLayerInterfaces?.ForEach(
                networkLayerInterface => networkLayerInterface?.SendPacket(packet, connection, transmissionMode));
        }

        protected void SendPacket(IPacket packet, ICollection<IConnection> connections,
            TransmissionMode transmissionMode = TransmissionMode.All)
        {
            mNetworkLayerInterfaces?.ForEach(
                networkLayerInterface => networkLayerInterface?.SendPacket(packet, connections, transmissionMode));
        }

        protected void StopInterfaces(string reason = "stopping")
        {
            mNetworkLayerInterfaces?.ForEach(networkLayerInterface => networkLayerInterface?.Stop(reason));
        }
    }
}