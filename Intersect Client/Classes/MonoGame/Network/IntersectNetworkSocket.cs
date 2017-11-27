using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Intersect.Config;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Crypto;
using Intersect.Network.Crypto.Formats;
using Intersect.Network.Packets.Reflectable;
using IntersectClientExtras.Network;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect.Client.Classes.MonoGame.Network
{
    public class IntersectNetworkSocket : GameSocket
    {
        public static ClientNetwork ClientLidgrenNetwork;
        public static ConcurrentQueue<IPacket> PacketQueue = new ConcurrentQueue<IPacket>();

        public IntersectNetworkSocket()
        {
            Log.Global.AddOutput(new ConsoleOutput());
        }

        public override void Connect(string host, int port)
        {
            if (ClientLidgrenNetwork != null)
            {
                ClientLidgrenNetwork.Close();
                ClientLidgrenNetwork = null;
            }
            if (ClientLidgrenNetwork == null)
            {
                var config = new NetworkConfiguration(ClientOptions.ServerHost, ClientOptions.ServerPort);
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("Intersect.Client.public-intersect.bek"))
                {
                    var rsaKey = EncryptionKey.FromStream<RsaKey>(stream);
                    Debug.Assert(rsaKey != null, "rsaKey != null");
                    ClientLidgrenNetwork = new ClientNetwork(config, rsaKey.Parameters);
                }

                if (ClientLidgrenNetwork != null)
                {
                    ClientLidgrenNetwork.Handlers[PacketCode.BinaryPacket] = AddPacketToQueue;
                    ClientLidgrenNetwork.OnDisconnected = delegate(INetworkLayerInterface sender, IConnection connection)
                    {
                        this.OnDisconnected();
                    };
                    if (!ClientLidgrenNetwork.Connect())
                    {
                        Log.Error("An error occurred while attempting to connect.");
                    }
                }
            }
        }

        public override void SendData(byte[] data)
        {
            if (ClientLidgrenNetwork != null)
            {
                var buffer = new ByteBuffer();
                buffer.WriteBytes(data);
                if (!ClientLidgrenNetwork.Send(new BinaryPacket(null) {Buffer = buffer}))
                {
                    throw new Exception("Beta 4 network send failed.");
                }
            }
        }

        public static bool AddPacketToQueue(IPacket packet)
        {
            PacketQueue.Enqueue(packet);
            return true;
        }

        public override void Update()
        {
            var packetCount = PacketQueue.Count;
            for (int i = 0; i < packetCount; i++)
            {
                IPacket dequeued;
                if (PacketQueue.TryDequeue(out dequeued))
                {
                    PacketHandler.HandlePacket(dequeued);
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