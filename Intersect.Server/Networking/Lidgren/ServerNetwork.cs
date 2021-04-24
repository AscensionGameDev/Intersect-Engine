using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using Amib.Threading;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Events;
using Intersect.Network.Lidgren;
using Intersect.Plugins.Interfaces;
using Intersect.Server.Core;
using Intersect.Server.Entities;

using Lidgren.Network;

namespace Intersect.Server.Networking.Lidgren
{

    // TODO: Migrate to a proper service
    internal class ServerNetwork : AbstractNetwork, IServer
    {
        /// <summary>
        /// This is our smart thread pool which we use to handle packet processing and packet sending. Min/Max Number of Threads & Idle Timeouts are set via server config.
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

        internal ServerNetwork(IServerContext context, INetworkHelper networkHelper, NetworkConfiguration configuration, RSAParameters rsaParameters) : base(
            networkHelper, configuration
        )
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));

            Guid = Guid.NewGuid();

            var lidgrenInterface = new LidgrenInterface(this, typeof(NetServer), rsaParameters);
            lidgrenInterface.OnConnected += HandleInterfaceOnConnected;
            lidgrenInterface.OnConnectionApproved += HandleInterfaceOnConnectonApproved;
            lidgrenInterface.OnDisconnected += HandleInterfaceOnDisconnected;
            lidgrenInterface.OnUnconnectedMessage += HandleOnUnconnectedMessage;
            lidgrenInterface.OnConnectionRequested += HandleConnectionRequested;
            AddNetworkLayerInterface(lidgrenInterface);
        }

        private IServerContext Context { get; }

        public HandleConnectionEvent OnConnected { get; set; }

        public HandleConnectionEvent OnConnectionApproved { get; set; }

        public HandleConnectionEvent OnDisconnected { get; set; }

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
            Client.CreateBeta4Client(Context, connectionEventArgs.Connection);
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

        protected virtual void HandleOnUnconnectedMessage(NetPeer peer, NetIncomingMessage message)
        {
            try
            {
                var packetType = message.ReadString();
                switch (packetType)
                {
                    case "status":
                        var response = peer.CreateMessage();
                        response.WriteVariableInt32(Player.OnlineCount);
                        peer.SendUnconnectedMessage(response, message.SenderEndPoint);

                        break;
                }
            }
            catch (Exception exception)
            {
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
