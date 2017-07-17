using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Network;
using IntersectClientExtras.Network;
using Intersect_Client.Classes.Networking;
using Intersect.Logging;
using Intersect_Client.Classes.General;
using System.Reflection;
using Intersect.Network.Crypto;
using Intersect.Network.Crypto.Formats;
using System.Diagnostics;
using Intersect.Network.Packets.Reflectable;
using Lidgren.Network;

namespace Intersect.Client.Classes.MonoGame.Network
{
    public class IntersectNetworkSocket : GameSocket
    {
        public static ClientNetwork ClientLidgrenNetwork;

        public IntersectNetworkSocket()
        {
        }

        public override void Connect(string host, int port)
        {
            if (ClientLidgrenNetwork == null)
            {
                Log.Global.AddOutput(new ConsoleOutput());
                var config = new NetworkConfiguration(Globals.Database.ServerHost, (ushort)Globals.Database.ServerPort);
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("Intersect.Client.public-intersect.bek"))
                {
                    var rsaKey = EncryptionKey.FromStream<RsaKey>(stream);
                    Debug.Assert(rsaKey != null, "rsaKey != null");
                    ClientLidgrenNetwork = new ClientNetwork(config, rsaKey.Parameters);
                }

                if (ClientLidgrenNetwork != null)
                {
                    ClientLidgrenNetwork.Handlers[PacketCode.BinaryPacket] = HandlePacket;
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
                if (!ClientLidgrenNetwork.Send(new BinaryPacket(null) { Buffer = buffer }))
                {
                    throw new Exception("Beta 4 network send failed.");
                }
            }
        }

        public bool HandlePacket(IPacket packet)
        {
            var binaryPacket = packet as BinaryPacket;
            this.OnDataReceived(binaryPacket?.Buffer?.ToArray());
            return true;
        }

        public override void Update()
        {
            
        }

        public override void Disconnect(string reason)
        {
            ClientLidgrenNetwork?.Disconnect(reason);
        }

        public override void Dispose()
        {
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
