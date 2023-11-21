using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography;
using Amib.Threading;
using Intersect.Core;
using Intersect.Logging;
using Intersect.Memory;
using Intersect.Network;
using Intersect.Network.Events;
using Intersect.Network.Lidgren;
using Intersect.Server.Core;
using Lidgren.Network;

namespace Intersect.Server.Networking.Lidgren
{

    // TODO: Migrate to a proper service
    internal partial class ServerNetwork : AbstractNetwork, IServer
    {
        /// <summary>
        /// This is our smart thread pool which we use to handle packet processing and packet sending.
        /// Min/Max Number of Threads and Idle Timeouts are set via server config.
        /// </summary>
        public static SmartThreadPool Pool = new SmartThreadPool(
            new STPStartInfo()
            {
                ThreadPoolName = "NetworkPool",
                IdleTimeout = 20000,
                MinWorkerThreads = Options.Instance.Processing.MinNetworkThreads,
                MaxWorkerThreads = Options.Instance.Processing.MaxNetworkThreads
            }
        );

        internal ServerNetwork(
            IServerContext context,
            IApplicationContext applicationContext,
            NetworkConfiguration configuration,
            RSAParameters rsaParameters
        ) : base(
            applicationContext, configuration
        )
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));

            Id = Guid.NewGuid();

            var networkLayerInterface = new LidgrenInterface(this, typeof(NetServer), rsaParameters);
            networkLayerInterface.OnConnected += HandleInterfaceOnConnected;
            networkLayerInterface.OnConnectionApproved += HandleInterfaceOnConnectonApproved;
            networkLayerInterface.OnDisconnected += HandleInterfaceOnDisconnected;
            networkLayerInterface.OnUnconnectedMessage += HandleOnUnconnectedMessage;
            networkLayerInterface.OnConnectionRequested += HandleConnectionRequested;
            AddNetworkLayerInterface(networkLayerInterface);
        }

        private IServerContext Context { get; }

        public override bool IsConnected => Connections.Any();

        public override event HandleConnectionEvent OnConnected;
        public override event HandleConnectionEvent OnConnectionApproved;
        public override event HandleConnectionEvent OnConnectionDenied;
        public override event HandleConnectionRequest OnConnectionRequested;
        public override event HandleConnectionEvent OnDisconnected;
        public override event HandlePacketAvailable OnPacketAvailable;
        public override event HandleUnconnectedMessage OnUnconnectedMessage;

        public override void Close() => Disconnect("closing");

        public bool Listen()
        {
            StartInterfaces();

            return true;
        }

        protected virtual void HandleInterfaceOnConnected(
            INetworkLayerInterface sender,
            ConnectionEventArgs connectionEventArgs
        )
        {
            Log.Info($"Connected [{connectionEventArgs.Connection?.Guid}].");
            var client =Client.CreateBeta4Client(Context, this, connectionEventArgs.Connection);
            PacketSender.SendPing(client);
            OnConnected?.Invoke(sender, connectionEventArgs);
        }

        protected virtual void HandleInterfaceOnConnectonApproved(
            INetworkLayerInterface sender,
            ConnectionEventArgs connectionEventArgs
        )
        {
            Log.Info($"Connection approved [{connectionEventArgs.Connection?.Guid}].");
            OnConnectionApproved?.Invoke(sender, connectionEventArgs);
        }

        protected virtual void HandleInterfaceOnDisconnected(
            INetworkLayerInterface sender,
            ConnectionEventArgs connectionEventArgs
        )
        {
            Log.Info($"Disconnected [{connectionEventArgs.Connection?.Guid}].");
            Client.RemoveBeta4Client(connectionEventArgs.Connection);
            OnDisconnected?.Invoke(sender, connectionEventArgs);
        }

        protected virtual void HandleOnUnconnectedMessage(UnconnectedMessageSender sender, IBuffer message)
        {
            try
            {
                var packetType = message.ReadString();
                switch (packetType)
                {
                    case "status":
                        // TODO: Response buffers
                        // var response = peer.CreateMessage();
                        // response.WriteVariableInt32(Player.OnlineCount);
                        // peer.SendUnconnectedMessage(response, message.SenderEndPoint);
                        break;
                }
            }
#if DIAGNOSTIC
            catch (Exception exception)
#endif
            catch
            {
#if DIAGNOSTIC
                Log.Diagnostic(exception);
#else
                // ignored
#endif
            }
        }

        protected virtual bool HandleConnectionRequested(INetworkLayerInterface sender, IConnection connection)
        {
            if (string.IsNullOrEmpty(connection?.Ip))
            {
                return false;
            }

            return true;
        }

        protected override bool SendUnconnected(IPEndPoint endPoint, UnconnectedPacket packet)
        {
            throw new NotImplementedException();
        }

        public override bool Send(IPacket packet, TransmissionMode mode = TransmissionMode.All)
        {
            return Send(Connections, packet, mode);
        }

        public override bool Send(IConnection connection, IPacket packet, TransmissionMode mode = TransmissionMode.All)
        {
            return Send(new[] {connection}, packet, mode);
        }

        public override bool Send(ICollection<IConnection> connections, IPacket packet, TransmissionMode mode = TransmissionMode.All)
        {
            SendPacket(packet, connections, mode);

            return true;
        }

        protected override IDictionary<TKey, TValue> CreateDictionaryLegacy<TKey, TValue>()
        {
            return new ConcurrentDictionary<TKey, TValue>();
        }
    }
}
