using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets.Reflectable;
using Intersect.Server.Classes.General;
using Intersect.Server.Network;

namespace Intersect.Server.Classes.Networking
{
    public class IntersectNetworkSocket : GameSocket
    {
        private IConnection _connection;
        private PacketHandler _packetHandler = new PacketHandler();

        public IntersectNetworkSocket(IConnection connection)
        {
            _connection = connection;
            _isConnected = true;
        }

        public override void CreateClient()
        {
            base.CreateClient();
            _connection.UserData = base._myClient;
            lock (Globals.ClientLock)
            {
                Globals.ClientLookup.Add(_connection.Guid, base._myClient);
            }
        }

        public override void OnClientRemoved()
        {
            base.OnClientRemoved();
            lock (Globals.ClientLock)
            {
                Globals.ClientLookup.Remove(_connection.Guid);
            }
        }

        public override void SendData(ByteBuffer bf)
        {
            try
            {
                IntersectNetworkServer.ServerNetwork.Send(_connection, new BinaryPacket(null) {Buffer = bf});
            }
            catch (Exception ex)
            {
                Log.Trace(ex);
                HandleDisconnect();
            }
        }

        public static bool DataReceived(IPacket packet)
        {
            var binaryPacket = packet as BinaryPacket;
            ((Client) packet.Connection.UserData).Socket.HandlePacket(binaryPacket.Buffer);
            return true;
        }

        public override void Update()
        {
            //Completely event driven, no need to update here.
        }

        public override void Disconnect()
        {
            base.Disconnect();
            if (_connection.IsConnected)
            {
                IntersectNetworkServer.ServerNetwork.RemoveConnection(_connection);
            }
        }

        public override void Dispose()
        {
            Disconnect();
        }

        public override bool IsConnected()
        {
            return _isConnected;
        }

        public override string GetIP()
        {
            return _connection?.Ip ?? "";
        }
    }
}
