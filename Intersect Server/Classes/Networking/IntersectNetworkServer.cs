using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets.Reflectable;
using Intersect.Server.Classes.Networking;
using Lidgren.Network;

namespace Intersect.Server.Network
{
    public class IntersectNetworkServer : AbstractNetwork, IServer
    {
        public static IntersectNetworkServer ServerNetwork;
        public HandleConnectionEvent OnConnected { get; set; }
        public HandleConnectionEvent OnConnectionApproved { get; set; }
        public HandleConnectionEvent OnDisconnected { get; set; }

        public override Guid Guid { get; }

        public IntersectNetworkServer(NetworkConfiguration configuration, RSAParameters rsaParameters)
            : base(configuration)
        {
            Guid = Guid.NewGuid();

            var lidgrenInterface = new LidgrenInterface(this, typeof(NetServer), rsaParameters);
            lidgrenInterface.OnConnected += HandleInterfaceOnConnected;
            lidgrenInterface.OnConnectionApproved += HandleInterfaceOnConnectonApproved;
            lidgrenInterface.OnDisconnected += HandleInterfaceOnDisconnected;
            lidgrenInterface.OnUnconnectedMessage += HandleOnUnconnectedMessage;
            AddNetworkLayerInterface(lidgrenInterface);
        }

        protected virtual void HandleInterfaceOnConnected(INetworkLayerInterface sender, IConnection connection)
        {
            Log.Info($"Connected [{connection?.Guid}].");
            new IntersectNetworkSocket(connection).CreateClient();
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
            ((Client) connection.UserData).Disconnect();
            OnDisconnected?.Invoke(sender, connection);
        }

        protected virtual void HandleOnUnconnectedMessage(NetPeer peer, NetIncomingMessage message)
        {
            var packetType = message.ReadString();
            switch (packetType)
            {
                case "status":
                    var response = peer.CreateMessage();
                    response.WriteVariableInt32(peer.ConnectionsCount);
                    peer.SendUnconnectedMessage(response,message.SenderEndPoint);
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

        public bool Listen()
        {
            StartInterfaces();
            return true;
        }

        public void Stop()
        {
            StopInterfaces();
        }
    }
}