using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using Intersect.Logging;
using JetBrains.Annotations;
using Lidgren.Network;

namespace Intersect.Network
{
    public class ClientNetwork : AbstractNetwork, IClient
    {
        private readonly LidgrenInterface mLidgrenInterface;

        private Guid mGuid;

        public ClientNetwork([NotNull] NetworkConfiguration configuration, RSAParameters rsaParameters)
            : base(configuration)
        {
            mGuid = Guid.Empty;

            IsConnected = false;
            IsServerOnline = false;

            mLidgrenInterface = new LidgrenInterface(this, typeof(NetClient), rsaParameters);
            mLidgrenInterface.OnConnected += HandleInterfaceOnConnected;
            mLidgrenInterface.OnConnectionApproved += HandleInterfaceOnConnectonApproved;
            mLidgrenInterface.OnConnectionDenied += HandleInterfaceOnConnectonDenied;
            mLidgrenInterface.OnDisconnected += HandleInterfaceOnDisconnected;
            AddNetworkLayerInterface(mLidgrenInterface);
        }

        public HandleConnectionEvent OnConnected { get; set; }
        public HandleConnectionEvent OnConnectionApproved { get; set; }
        public HandleConnectionEvent OnConnectionDenied { get; set; }
        public HandleConnectionEvent OnDisconnected { get; set; }

        public bool IsConnected { get; private set; }
        public bool IsServerOnline { get; }

        public int Ping
        {
            get
            {
                var connection = FindConnection<LidgrenConnection>(Guid.Empty);
                //Log.Debug($"connection={connection},ping={connection?.NetConnection?.AverageRoundtripTime ?? -1}");
                if (connection == null) return -1;
                return (int) (1000 * connection.NetConnection.AverageRoundtripTime);
            }
        }

        public override Guid Guid => mGuid;

        public bool Connect()
        {
            if (IsConnected) Disconnect("client_starting_connection");
            if (mLidgrenInterface == null) return false;
            StartInterfaces();
            return true;
        }

        public override bool Send(IPacket packet)
            => mLidgrenInterface?.SendPacket(packet) ?? false;

        public override bool Send(IConnection connection, IPacket packet)
            => Send(packet);

        public override bool Send(ICollection<IConnection> connections, IPacket packet)
            => Send(packet);

        protected virtual void HandleInterfaceOnConnected(INetworkLayerInterface sender, IConnection connection)
        {
            Log.Info($"Connected [{connection?.Guid}].");
            IsConnected = true;
            OnConnected?.Invoke(sender, connection);
        }

        protected virtual void HandleInterfaceOnConnectonApproved(INetworkLayerInterface sender, IConnection connection)
        {
            Log.Info($"Connection approved [{connection?.Guid}].");
            OnConnectionApproved?.Invoke(sender, connection);
        }

        protected virtual void HandleInterfaceOnConnectonDenied(INetworkLayerInterface sender, IConnection connection)
        {
            Log.Info($"Connection denied [{connection?.Guid}].");
            OnConnectionDenied?.Invoke(sender, connection);
        }

        protected virtual void HandleInterfaceOnDisconnected(INetworkLayerInterface sender, IConnection connection)
        {
            Log.Info($"Disconnected [{connection?.Guid ?? Guid.Empty}].");
            IsConnected = false;
            OnDisconnected?.Invoke(sender, connection);
        }

        public void Close()
        {
            StopInterfaces("closing");
        }

        internal void AssignGuid(Guid guid) => mGuid = guid;

        protected override IDictionary<TKey, TValue> CreateDictionaryLegacy<TKey, TValue>()
        {
            return new ConcurrentDictionary<TKey, TValue>();
        }
    }
}