using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Server.Entities;
using JetBrains.Annotations;
using Lidgren.Network;

namespace Intersect.Server.Networking.Lidgren
{
    public class ServerNetwork : AbstractNetwork, IServer
    {
        public ServerNetwork([NotNull] NetworkConfiguration configuration, RSAParameters rsaParameters)
            : base(configuration)
        {
            Guid = Guid.NewGuid();

            var lidgrenInterface = new LidgrenInterface(this, typeof(NetServer), rsaParameters);
            lidgrenInterface.OnConnected += HandleInterfaceOnConnected;
            lidgrenInterface.OnConnectionApproved += HandleInterfaceOnConnectonApproved;
            lidgrenInterface.OnDisconnected += HandleInterfaceOnDisconnected;
            lidgrenInterface.OnUnconnectedMessage += HandleOnUnconnectedMessage;
            //lidgrenInterface.OnUnconnectedMessage += HandleUnconnectedMessageReceived;
            AddNetworkLayerInterface(lidgrenInterface);
        }

        public HandleConnectionEvent OnConnected { get; set; }
        public HandleConnectionEvent OnConnectionApproved { get; set; }
        public HandleConnectionEvent OnDisconnected { get; set; }

        public override Guid Guid { get; }

        public bool Listen()
        {
            StartInterfaces();
            return true;
        }

        protected virtual void HandleInterfaceOnConnected(INetworkLayerInterface sender, IConnection connection)
        {
            Log.Info($"Connected [{connection?.Guid}].");
            Client.CreateBeta4Client(connection);
            OnConnected?.Invoke(sender, connection);
        }

        protected virtual void HandleInterfaceOnConnectonApproved(INetworkLayerInterface sender, IConnection connection)
        {
            Log.Info($"Connection approved [{connection?.Guid}].");
            OnConnectionApproved?.Invoke(sender, connection);
        }

        protected virtual void HandleInterfaceOnDisconnected(INetworkLayerInterface sender, IConnection connection)
        {
            Log.Info($"Disconnected [{connection?.Guid}].");
            Client.RemoveBeta4Client(connection);
            OnDisconnected?.Invoke(sender, connection);
        }

        protected virtual void HandleOnUnconnectedMessage(NetPeer peer, NetIncomingMessage message)
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

        public override bool Send(IPacket packet)
            => Send(Connections, packet);

        public override bool Send(IConnection connection, IPacket packet)
            => Send(new[] {connection}, packet);

        public override bool Send(ICollection<IConnection> connections, IPacket packet)
        {
            SendPacket(packet, connections, TransmissionMode.All);
            return true;
        }

        protected override IDictionary<TKey, TValue> CreateDictionaryLegacy<TKey, TValue>()
        {
            return new ConcurrentDictionary<TKey, TValue>();
        }
    }
}