using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography;
using Intersect.Core;
using Intersect.Logging;
using Intersect.Network.Events;
using Intersect.Network.LiteNetLib;

namespace Intersect.Network
{
    public partial class ClientNetwork : AbstractNetwork, IClient
    {
        private static readonly NetworkLayerInterfaceFactory DefaultNetworkLayerInterfaceFactory =
            (network, parameters) => new LiteNetLibInterface(network, parameters);

        internal static NetworkLayerInterfaceFactory? NetworkLayerInterfaceFactory { get; set; }

        private readonly INetworkLayerInterface _interface;
        private bool _isConnected;

        public ClientNetwork(
            IApplicationContext applicationContext,
            NetworkConfiguration configuration,
            RSAParameters rsaParameters
        ) : base(
            applicationContext,
            configuration
        )
        {
            Id = Guid.Empty;

            _isConnected = false;

            _interface =
                (NetworkLayerInterfaceFactory ?? DefaultNetworkLayerInterfaceFactory).Invoke(this, rsaParameters);
            _interface.OnConnected += HandleInterfaceOnConnected;
            _interface.OnConnectionApproved += HandleInterfaceOnConnectonApproved;
            _interface.OnConnectionDenied += HandleInterfaceOnConnectonDenied;
            _interface.OnDisconnected += HandleInterfaceOnDisconnected;
            AddNetworkLayerInterface(_interface);
            StartInterfaces();
        }

        public override event HandleConnectionEvent OnConnected;
        public override event HandleConnectionEvent OnConnectionApproved;
        public override event HandleConnectionEvent OnConnectionDenied;
        public override event HandleConnectionRequest OnConnectionRequested;
        public override event HandleConnectionEvent OnDisconnected;
        public override event HandlePacketAvailable OnPacketAvailable;
        public override event HandleUnconnectedMessage OnUnconnectedMessage;

        public IConnection Connection => Connections.FirstOrDefault();

        public override bool IsConnected => _isConnected;

        public bool IsServerOnline => IsConnected;

        public int Ping => (int)(Connections.FirstOrDefault()?.Statistics.Ping ?? -1);

        public bool Connect()
        {
            return IsConnected || _interface.Connect();
        }

        protected override bool SendUnconnected(IPEndPoint endPoint, UnconnectedPacket packet) =>
            _interface.SendUnconnectedPacket(endPoint, packet);

        public override bool Send(IPacket packet, TransmissionMode mode = TransmissionMode.All)
        {
            return _interface?.SendPacket(packet, (IConnection)null, mode) ?? false;
        }

        public override bool Send(IConnection connection, IPacket packet, TransmissionMode mode = TransmissionMode.All)
        {
            return Send(packet, mode);
        }

        public override bool Send(ICollection<IConnection> connections, IPacket packet, TransmissionMode mode = TransmissionMode.All)
        {
            return Send(packet, mode);
        }

        protected virtual void HandleInterfaceOnConnected(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs)
        {
            Log.Info($"Connected [{connectionEventArgs.Connection?.Guid}].");
            _isConnected = true;
            OnConnected?.Invoke(sender, connectionEventArgs);
        }

        protected virtual void HandleInterfaceOnConnectonApproved(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs)
        {
            Log.Info($"Connection approved [{connectionEventArgs.Connection?.Guid}].");
            OnConnectionApproved?.Invoke(sender, connectionEventArgs);
        }

        protected virtual void HandleInterfaceOnConnectonDenied(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs)
        {
            Log.Info($"Connection denied [{connectionEventArgs.Connection?.Guid}].");
            OnConnectionDenied?.Invoke(sender, connectionEventArgs);
        }

        protected virtual void HandleInterfaceOnDisconnected(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs)
        {
            Log.Info($"Disconnected [{connectionEventArgs.Connection?.Guid ?? Guid.Empty}].");
            _isConnected = false;
            OnDisconnected?.Invoke(sender, connectionEventArgs);
        }

        public override void Close()
        {
            StopInterfaces("closing");
        }

        internal void AssignId(Guid id)
        {
            Id = id;
        }

        protected override IDictionary<TKey, TValue> CreateDictionaryLegacy<TKey, TValue>()
        {
            return new ConcurrentDictionary<TKey, TValue>();
        }
    }
}
