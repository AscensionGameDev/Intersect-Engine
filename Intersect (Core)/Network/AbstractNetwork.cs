using System.Collections.Concurrent;
using System.Net;
using Intersect.Core;
using Intersect.Logging;
using Intersect.Memory;
using Intersect.Plugins.Interfaces;
using Intersect.Utilities;

namespace Intersect.Network
{
    public abstract partial class AbstractNetwork : INetwork
    {
        private record struct PendingUnconnectedMessage(Guid MessageId, HandleUnconnectedMessage ResponseHandler);

        private bool mDisposed;

        private readonly object mDisposeLock;

        private readonly List<INetworkLayerInterface> mNetworkLayerInterfaces;

        private readonly PriorityQueue<PendingUnconnectedMessage, long> _pendingUnconnectedCallbacks = new();

        protected AbstractNetwork(IApplicationContext applicationContext, NetworkConfiguration configuration)
        {
            mDisposed = false;
            mDisposeLock = new object();

            ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            Helper = applicationContext.PacketHelper ??
                     throw new ArgumentException(
                         $"{nameof(IApplicationContext.PacketHelper)} was null.",
                         nameof(applicationContext)
                     );

            mNetworkLayerInterfaces = new List<INetworkLayerInterface>();

            ConnectionLookup = new ConcurrentDictionary<Guid, IConnection>();

            if (configuration.Host?.ToLower() == "localhost")
            {
                configuration.Host = "127.0.0.1";
            }

            Configuration = configuration;
        }

        public IApplicationContext ApplicationContext { get; }

        public IPacketHelper Helper { get; }
        public abstract bool IsConnected { get; }

        public ICollection<IConnection> Connections => ConnectionLookup.Values;

        public IDictionary<Guid, IConnection> ConnectionLookup { get; }

        public HandlePacket Handler { get; set; }

        public ShouldProcessPacket PreProcessHandler { get; set; }

        public int ConnectionCount => Connections.Count;
        public abstract event HandleConnectionEvent OnConnected;
        public abstract event HandleConnectionEvent OnConnectionApproved;
        public abstract event HandleConnectionEvent OnConnectionDenied;
        public abstract event HandleConnectionRequest OnConnectionRequested;
        public abstract event HandleConnectionEvent OnDisconnected;
        public abstract event HandlePacketAvailable OnPacketAvailable;
        public abstract event HandleUnconnectedMessage OnUnconnectedMessage;

        public NetworkConfiguration Configuration { get; }

        public Guid Id { get; protected set; }

        public bool AddConnection(IConnection connection)
        {
            if (ConnectionLookup.ContainsKey(connection.Guid))
            {
                return false;
            }

            ConnectionLookup.Add(connection.Guid, connection);

            return true;
        }

        public bool RemoveConnection(IConnection connection) =>
            !connection.IsConnected && ConnectionLookup.Remove(connection.Guid);

        public void Dispose()
        {
            lock (mDisposeLock)
            {
                if (mDisposed)
                {
                    return;
                }

                mDisposed = true;
            }

            if (!Disconnect(NetworkStatus.Quitting.ToString()))
            {
                Log.Error("Error disconnecting while disposing.");
            }

            mNetworkLayerInterfaces?.ForEach(networkLayerInterface => networkLayerInterface?.Dispose());

            ConnectionLookup.Clear();
        }

        public abstract void Close();

        public bool Disconnect(string message = "") => Disconnect(Connections, message);

        public bool Disconnect(Guid guid, string message = "") => Disconnect(FindConnection(guid), message);

        public bool Disconnect(IConnection connection, string message = "")
        {
            return Disconnect(new[] {connection}, message);
        }

        public bool Disconnect(ICollection<Guid> guids, string message = "") =>
            Disconnect(FindConnections(guids), message);

        public bool Disconnect(ICollection<IConnection> connections, string message = "")
        {
            mNetworkLayerInterfaces?.ForEach(
                networkLayerInterface => networkLayerInterface?.Disconnect(connections, message)
            );

            return true;
        }

        public bool SendUnconnected(
            IPEndPoint endPoint,
            UnconnectedPacket packet,
            HandleUnconnectedMessage? responseCallback = default,
            long timeout = INetwork.DefaultUnconnectedMessageTimeout
        )
        {
            if (!SendUnconnected(endPoint, packet))
            {
                return false;
            }

            if (responseCallback != default)
            {
                _pendingUnconnectedCallbacks.Enqueue(
                    new PendingUnconnectedMessage(packet.MessageId, responseCallback),
                    timeout
                );
            }

            return true;
        }

        protected abstract bool SendUnconnected(IPEndPoint endPoint, UnconnectedPacket packet);

        public void Update()
        {
            var now = Timing.Global.MillisecondsUtc;
            while (_pendingUnconnectedCallbacks.TryPeek(out _, out var time) && time < now)
            {
                if (!_pendingUnconnectedCallbacks.TryDequeue(out _, out _))
                {
                    return;
                }
            }
        }

        public abstract bool Send(IPacket packet, TransmissionMode mode = TransmissionMode.All);

        public bool Send(Guid guid, IPacket packet, TransmissionMode mode = TransmissionMode.All)
        {
            var connection = FindConnection(guid);

            return connection != null && Send(connection, packet, mode);
        }

        public abstract bool Send(IConnection connection, IPacket packet, TransmissionMode mode = TransmissionMode.All);

        public bool Send(ICollection<Guid> guids, IPacket packet, TransmissionMode mode = TransmissionMode.All) => Send(FindConnections(guids), packet, mode);

        public abstract bool Send(ICollection<IConnection> connections, IPacket packet, TransmissionMode mode = TransmissionMode.All);

        public IConnection FindConnection(Guid guid)
        {
            if (ConnectionLookup.TryGetValue(guid, out var connection))
            {
                return connection;
            }

            Log.Diagnostic($"Could not find connection {guid}.");

            return null;
        }

        public TConnection FindConnection<TConnection>(Guid guid) where TConnection : class, IConnection =>
            FindConnection(guid) as TConnection;

        public TConnection FindConnection<TConnection>(Func<TConnection, bool> selector)
            where TConnection : class, IConnection =>
            FindConnections<TConnection>().FirstOrDefault(selector);

        public ICollection<IConnection> FindConnections(ICollection<Guid> guids)
        {
            return guids.Select(FindConnection).Where(connection => connection != null).ToList();
        }

        public ICollection<TConnection> FindConnections<TConnection>() where TConnection : class, IConnection =>
            Connections.OfType<TConnection>().ToList();

        protected void AddNetworkLayerInterface(INetworkLayerInterface networkLayerInterface)
        {
            if (mNetworkLayerInterfaces == null)
            {
                throw new ArgumentNullException(nameof(mNetworkLayerInterfaces));
            }

            if (networkLayerInterface == null)
            {
                throw new ArgumentNullException(nameof(networkLayerInterface));
            }

            networkLayerInterface.OnPacketAvailable += HandleInboundMessageAvailable;
            mNetworkLayerInterfaces.Add(networkLayerInterface);
        }

        protected bool RunForInterface<TNetworkLayerInterface>(Func<TNetworkLayerInterface, bool> action)
        {
            if (action != default)
            {
                return mNetworkLayerInterfaces.OfType<TNetworkLayerInterface>().Any(action);
            }

            Log.Error(new ArgumentNullException(nameof(action)));
            return false;

        }

        private void HandleInboundMessageAvailable(INetworkLayerInterface sender)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (!sender.TryGetInboundBuffer(out var buffer, out var connection))
            {
                //Log.Error("Failed to obtain packet when told a packet was available.");

                return;
            }

            HandleInboundData(buffer, connection);

            sender.ReleaseInboundBuffer(buffer);
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
            if (buffer == default(IBuffer))
            {
                return;
            }

            if (buffer.Length < 1)
            {
                return;
            }

            if (PreProcessHandler != null)
            {
                if (!PreProcessHandler.Invoke(connection, buffer.Length))
                {
                    return;
                }
            }

            // Incorporate Ceras
            var data = buffer.ToBytes();

            // Get Packet From Data using MessagePack
            var packet = (IPacket) MessagePacker.Instance.Deserialize(data);
            if (packet != null)
            {
                if (Handler != null)
                {
                    // Pass packet to handler.
                    Handler(connection, packet);
                }
                else
                {
                    Log.Error("Handler == null, this shouldn't happen! Tell JC");
                    Disconnect(connection, NetworkStatus.Unknown.ToString());
                }
            }
            else
            {
                Disconnect(connection, NetworkStatus.VersionMismatch.ToString());
            }
        }

        protected abstract IDictionary<TKey, TValue> CreateDictionaryLegacy<TKey, TValue>();

        protected void StartInterfaces()
        {
            mNetworkLayerInterfaces?.ForEach(networkLayerInterface => networkLayerInterface?.Start());
        }

        protected void SendPacket(
            IPacket packet,
            IConnection connection,
            TransmissionMode transmissionMode = TransmissionMode.All
        )
        {
            mNetworkLayerInterfaces?.ForEach(
                networkLayerInterface => networkLayerInterface?.SendPacket(packet, connection, transmissionMode)
            );
        }

        protected void SendPacket(
            IPacket packet,
            ICollection<IConnection> connections,
            TransmissionMode transmissionMode = TransmissionMode.All
        )
        {
            mNetworkLayerInterfaces?.ForEach(
                networkLayerInterface => networkLayerInterface?.SendPacket(packet, connections, transmissionMode)
            );
        }

        protected void StopInterfaces(string reason = "stopping")
        {
            mNetworkLayerInterfaces?.ForEach(networkLayerInterface => networkLayerInterface?.Stop(reason));
        }
    }
}
