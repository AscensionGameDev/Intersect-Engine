﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Intersect.Client.Framework.Network;
using Intersect.Configuration;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Crypto;
using Intersect.Crypto.Formats;

namespace Intersect.Client.MonoGame.Network
{

    public class MonoSocket : GameSocket
    {

        public static ClientNetwork ClientLidgrenNetwork;

        public static ConcurrentQueue<KeyValuePair<IConnection, IPacket>> PacketQueue =
            new ConcurrentQueue<KeyValuePair<IConnection, IPacket>>();

        public override void Connect(string host, int port)
        {
            if (ClientLidgrenNetwork != null)
            {
                ClientLidgrenNetwork.Close();
                ClientLidgrenNetwork = null;
            }

            var config = new NetworkConfiguration(ClientConfiguration.Instance.Host, ClientConfiguration.Instance.Port);
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Intersect.Client.network.handshake.bkey.pub"))
            {
                var rsaKey = EncryptionKey.FromStream<RsaKey>(stream);
                Debug.Assert(rsaKey != null, "rsaKey != null");
                ClientLidgrenNetwork = new ClientNetwork(config, rsaKey.Parameters);
            }

            if (ClientLidgrenNetwork == null)
            {
                return;
            }

            ClientLidgrenNetwork.Handler = AddPacketToQueue;
            ClientLidgrenNetwork.OnConnected += OnConnected;
            ClientLidgrenNetwork.OnDisconnected += OnDisconnected;
            ClientLidgrenNetwork.OnConnectionDenied += (sender, connectionEventArgs) => OnConnectionFailed(sender, connectionEventArgs, true);

            if (!ClientLidgrenNetwork.Connect())
            {
                Log.Error("An error occurred while attempting to connect.");
            }
        }

        public override void SendPacket(object packet)
        {
            if (packet is CerasPacket && ClientLidgrenNetwork != null)
            {
                ClientLidgrenNetwork.Send((CerasPacket) packet);
            }
        }

        public static bool AddPacketToQueue(IConnection connection, IPacket packet)
        {
            PacketQueue.Enqueue(new KeyValuePair<IConnection, IPacket>(connection, packet));

            return true;
        }

        public override void Update()
        {
            var packetCount = PacketQueue.Count;
            for (var i = 0; i < packetCount; i++)
            {
                KeyValuePair<IConnection, IPacket> dequeued;
                if (PacketQueue.TryDequeue(out dequeued))
                {
                    OnDataReceived(dequeued.Value);
                }
            }
        }

        public override void Disconnect(string reason)
        {
            ClientLidgrenNetwork?.Disconnect(reason);
        }

        public override void Dispose()
        {
            ClientLidgrenNetwork?.Close();
            ClientLidgrenNetwork?.Dispose();
            ClientLidgrenNetwork = null;
        }

        public override bool IsConnected()
        {
            return ClientLidgrenNetwork?.IsConnected ?? false;
        }

        public override int Ping
        {
            get
            {
                return ClientLidgrenNetwork?.Ping ?? -1;
            }
        }
    }

}
