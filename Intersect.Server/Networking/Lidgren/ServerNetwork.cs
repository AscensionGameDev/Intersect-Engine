using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;

using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Events;
using Intersect.Network.Lidgren;
using Intersect.Server.Entities;

using JetBrains.Annotations;

using Lidgren.Network;

namespace Intersect.Server.Networking.Lidgren
{
    public class ServerNetwork : AbstractNetwork, IServer
    {
        public ServerNetwork([NotNull] NetworkConfiguration configuration, RSAParameters rsaParameters) : base(
            configuration
        )
        {
            Guid = Guid.NewGuid();

            var lidgrenInterface = new LidgrenInterface(this, typeof(NetServer), rsaParameters);
            lidgrenInterface.OnConnected += HandleInterfaceOnConnected;
            lidgrenInterface.OnConnectionApproved += HandleInterfaceOnConnectonApproved;
            lidgrenInterface.OnDisconnected += HandleInterfaceOnDisconnected;
            lidgrenInterface.OnUnconnectedMessage += HandleOnUnconnectedMessage;
            lidgrenInterface.OnConnectionRequested += HandleConnectionRequested;
            AddNetworkLayerInterface(lidgrenInterface);
        }

        public HandleConnectionEvent OnConnected { get; set; }

        public HandleConnectionEvent OnConnectionApproved { get; set; }

        public HandleConnectionEvent OnDisconnected { get; set; }

        public bool Listen()
        {
            StartInterfaces();

            return true;
        }

        protected virtual void HandleInterfaceOnConnected(
            [NotNull] INetworkLayerInterface sender,
            [NotNull] ConnectionEventArgs connectionEventArgs
        )
        {
            Log.Info($"Connected [{connectionEventArgs.Connection?.Guid}].");
            Client.CreateBeta4Client(connectionEventArgs.Connection);
            OnConnected?.Invoke(sender, connectionEventArgs);
        }

        protected virtual void HandleInterfaceOnConnectonApproved(
            [NotNull] INetworkLayerInterface sender,
            [NotNull] ConnectionEventArgs connectionEventArgs
        )
        {
            Log.Info($"Connection approved [{connectionEventArgs.Connection?.Guid}].");
            OnConnectionApproved?.Invoke(sender, connectionEventArgs);
        }

        protected virtual void HandleInterfaceOnDisconnected(
            [NotNull] INetworkLayerInterface sender,
            [NotNull] ConnectionEventArgs connectionEventArgs
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
                Log.Error(exception);
            }
        }

        protected virtual bool HandleConnectionRequested(INetworkLayerInterface sender, IConnection connection)
        {
            if (string.IsNullOrEmpty(connection?.Ip))
            {
                return false;
            }

            return string.IsNullOrEmpty(Database.PlayerData.Ban.CheckBan(connection.Ip.Trim())) &&
                   Options.Instance.SecurityOpts.CheckIp(connection.Ip.Trim());
        }

        public override bool Send(IPacket packet)
        {
            return Send(Connections, packet);
        }

        public override bool Send(IConnection connection, IPacket packet)
        {
            return Send(new[] {connection}, packet);
        }

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
