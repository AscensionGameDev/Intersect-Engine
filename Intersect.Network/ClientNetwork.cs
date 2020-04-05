using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;

using Intersect.Logging;
using Intersect.Network.Events;
using Intersect.Network.Lidgren;

using JetBrains.Annotations;

using Lidgren.Network;

namespace Intersect.Network
{

    public class ClientNetwork : AbstractNetwork, IClient
    {

        private readonly LidgrenInterface mLidgrenInterface;

        public ClientNetwork([NotNull] NetworkConfiguration configuration, RSAParameters rsaParameters) : base(
            configuration
        )
        {
            Guid = Guid.Empty;

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
                if (connection == null)
                {
                    return -1;
                }

                return (int) (1000 * connection.NetConnection.AverageRoundtripTime);
            }
        }

        public bool Connect()
        {
            if (IsConnected)
            {
                Disconnect("client_starting_connection");
            }

            if (mLidgrenInterface == null)
            {
                return false;
            }

            StartInterfaces();

            return true;
        }

        public override bool Send(IPacket packet)
        {
            return mLidgrenInterface?.SendPacket(packet) ?? false;
        }

        public override bool Send(IConnection connection, IPacket packet)
        {
            return Send(packet);
        }

        public override bool Send(ICollection<IConnection> connections, IPacket packet)
        {
            return Send(packet);
        }

        protected virtual void HandleInterfaceOnConnected([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs)
        {
            Log.Info($"Connected [{connectionEventArgs.Connection?.Guid}].");
            IsConnected = true;
            OnConnected?.Invoke(sender, connectionEventArgs);
        }

        protected virtual void HandleInterfaceOnConnectonApproved([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs)
        {
            Log.Info($"Connection approved [{connectionEventArgs.Connection?.Guid}].");
            OnConnectionApproved?.Invoke(sender, connectionEventArgs);
        }

        protected virtual void HandleInterfaceOnConnectonDenied([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs)
        {
            Log.Info($"Connection denied [{connectionEventArgs.Connection?.Guid}].");
            OnConnectionDenied?.Invoke(sender, connectionEventArgs);
        }

        protected virtual void HandleInterfaceOnDisconnected([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs)
        {
            Log.Info($"Disconnected [{connectionEventArgs.Connection?.Guid ?? Guid.Empty}].");
            IsConnected = false;
            OnDisconnected?.Invoke(sender, connectionEventArgs);
        }

        public void Close()
        {
            StopInterfaces("closing");
        }

        internal void AssignGuid(Guid guid)
        {
            Guid = guid;
        }

        protected override IDictionary<TKey, TValue> CreateDictionaryLegacy<TKey, TValue>()
        {
            return new ConcurrentDictionary<TKey, TValue>();
        }

    }

}
