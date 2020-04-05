using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Intersect.Logging;
using Intersect.Memory;

using JetBrains.Annotations;

namespace Intersect.Network
{

    public abstract class AbstractNetwork : INetwork
    {

        private readonly List<INetworkLayerInterface> mNetworkLayerInterfaces;

        private bool mDisposed;

        protected AbstractNetwork([NotNull] NetworkConfiguration configuration)
        {
            mDisposed = false;

            mNetworkLayerInterfaces = new List<INetworkLayerInterface>();

            ConnectionLookup = new ConcurrentDictionary<Guid, IConnection>();

            if (configuration.Host?.ToLower() == "localhost")
            {
                configuration.Host = "127.0.0.1";
            }

            Configuration = configuration;
        }

        [NotNull]
        public ICollection<IConnection> Connections => ConnectionLookup.Values;

        [NotNull]
        public IDictionary<Guid, IConnection> ConnectionLookup { get; }

        [NotNull]
        public HandlePacket Handler { get; set; }

        public ShouldProcessPacket PreProcessHandler { get; set; }

        public int ConnectionCount => Connections.Count;

        public NetworkConfiguration Configuration { get; }

        public Guid Guid { get; protected set; }

        public bool AddConnection([NotNull] IConnection connection)
        {
            if (ConnectionLookup.ContainsKey(connection.Guid))
            {
                return false;
            }

            ConnectionLookup.Add(connection.Guid, connection);

            return true;
        }

        public bool RemoveConnection([NotNull] IConnection connection)
        {
            return !connection.IsConnected && ConnectionLookup.Remove(connection.Guid);
        }

        public void Dispose()
        {
            lock (this)
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

        public bool Disconnect(string message = "")
        {
            return Disconnect(Connections, message);
        }

        public bool Disconnect(Guid guid, string message = "")
        {
            return Disconnect(FindConnection(guid), message);
        }

        public bool Disconnect(IConnection connection, string message = "")
        {
            return Disconnect(new[] {connection}, message);
        }

        public bool Disconnect(ICollection<Guid> guids, string message = "")
        {
            return Disconnect(FindConnections(guids), message);
        }

        public bool Disconnect(ICollection<IConnection> connections, string message = "")
        {
            mNetworkLayerInterfaces?.ForEach(
                networkLayerInterface => networkLayerInterface?.Disconnect(connections, message)
            );

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
        {
            return Send(FindConnections(guids), packet);
        }

        public abstract bool Send(ICollection<IConnection> connections, IPacket packet);

        public IConnection FindConnection(Guid guid)
        {
            if (ConnectionLookup.TryGetValue(guid, out var connection))
            {
                return connection;
            }

            Log.Diagnostic($"Could not find connection {guid}.");

            return null;
        }

        public TConnection FindConnection<TConnection>(Guid guid) where TConnection : class, IConnection
        {
            return FindConnection(guid) as TConnection;
        }

        public TConnection FindConnection<TConnection>([NotNull] Func<TConnection, bool> selector)
            where TConnection : class, IConnection
        {
            return FindConnections<TConnection>().FirstOrDefault(selector);
        }

        public ICollection<IConnection> FindConnections([NotNull] ICollection<Guid> guids)
        {
            return guids.Select(FindConnection).Where(connection => connection != null).ToList();
        }

        [NotNull]
        public ICollection<TConnection> FindConnections<TConnection>() where TConnection : class, IConnection
        {
            return Connections.OfType<TConnection>().ToList();
        }

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

        private void HandleInboundMessageAvailable(INetworkLayerInterface sender)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (!sender.TryGetInboundBuffer(out var buffer, out var connection))
            {
                Log.Error("Failed to obtain packet when told a packet was available.");

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

            //Incorperate Ceras
            var data = buffer.ToBytes();

            //Get Packet From Data using Ceras
            var sw = new Stopwatch();
            sw.Start();
            var packet = (IPacket) connection.Ceras.Deserialize(data);
            if (sw.ElapsedMilliseconds > 10)
            {
                Debug.WriteLine(
                    "Took " + sw.ElapsedMilliseconds + "ms to deserialize packet: " + packet.GetType().Name
                );
            }

            //Handle any packet identification errors

            //Pass packet to handler.
            Handler.Invoke(connection, packet);
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
