using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Intersect.Client.Framework.Network;
using Intersect.Client.Networking;
using Intersect.Config;
using Intersect.Logging;
using Intersect.Logging.Output;
using Intersect.Network;
using Intersect.Network.Crypto;
using Intersect.Network.Crypto.Formats;
using Intersect.Network.Packets;

namespace Intersect.Client.MonoGame.Network
{
    public class IntersectNetworkSocket : GameSocket
    {
        public static ClientNetwork ClientLidgrenNetwork;
        public static ConcurrentQueue<KeyValuePair<IConnection,IPacket>> PacketQueue = new ConcurrentQueue<KeyValuePair<IConnection,IPacket>>();

        public IntersectNetworkSocket()
        {
        }

        public override void Connect(string host, int port)
        {
            if (ClientLidgrenNetwork != null)
            {
                ClientLidgrenNetwork.Close();
                ClientLidgrenNetwork = null;
            }

            var config = new NetworkConfiguration(ClientOptions.ServerHost, ClientOptions.ServerPort);
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Intersect.Client.public-intersect.bek"))
            {
                var rsaKey = EncryptionKey.FromStream<RsaKey>(stream);
                Debug.Assert(rsaKey != null, "rsaKey != null");
                ClientLidgrenNetwork = new ClientNetwork(config, rsaKey.Parameters);
            }

            if (ClientLidgrenNetwork == null) return;
            ClientLidgrenNetwork.Handler = AddPacketToQueue;
            ClientLidgrenNetwork.OnConnected += delegate { OnConnected(); };
            ClientLidgrenNetwork.OnDisconnected += delegate { OnDisconnected(); };
            ClientLidgrenNetwork.OnConnectionDenied += delegate { OnConnectionFailed(true); };

            if (!ClientLidgrenNetwork.Connect())
            {
                Log.Error("An error occurred while attempting to connect.");
            }
        }

        public override void SendPacket(object packet)
        {
            if (packet is CerasPacket)
            {
                ClientLidgrenNetwork.Send((CerasPacket) packet);
            }
        }

        public static bool AddPacketToQueue(IConnection connection, IPacket packet)
        {
            PacketQueue.Enqueue(new KeyValuePair<IConnection, IPacket>(connection,packet));
            return true;
        }

        public override void Update()
        {
            var packetCount = PacketQueue.Count;
            for (int i = 0; i < packetCount; i++)
            {
                KeyValuePair<IConnection,IPacket> dequeued;
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
            return ClientLidgrenNetwork.IsConnected;
        }

        public override int Ping()
        {
            return ClientLidgrenNetwork?.Ping ?? -1;
        }
    }
}