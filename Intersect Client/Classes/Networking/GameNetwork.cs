using System;
using Intersect;
using Intersect.Logging;
using IntersectClientExtras.Network;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.Networking
{
    public static class GameNetwork
    {
        public static GameSocket MySocket;

        private static bool _connected;
        public static bool Connecting;

        private static int mPing;
        public static bool Connected => MySocket?.IsConnected() ?? _connected;

        public static int Ping
        {
            get { return MySocket?.Ping() ?? mPing; }
            set { mPing = value; }
        }

        public static void InitNetwork()
        {
            if (MySocket == null) return;
            MySocket.Connected += MySocket_OnConnected;
            MySocket.Disconnected += MySocket_OnDisconnected;
            MySocket.DataReceived += MySocket_OnDataReceived;
            MySocket.ConnectionFailed += MySocket_OnConnectionFailed;
            TryConnect();
        }

        private static void TryConnect()
        {
            MySocket.Connect(Globals.Database.ServerHost, Globals.Database.ServerPort);
        }

        private static void MySocket_OnConnectionFailed()
        {
            TryConnect();
        }

        private static void MySocket_OnDataReceived(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);

            var compressed = bf.ReadBoolean();

            try
            {
                //Compressed?
                if (compressed)
                {
                    packet = bf.ReadBytes(bf.Length());
                    var data = Compression.DecompressPacket(packet);
                    bf = new ByteBuffer();
                    bf.WriteBytes(data);
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Buffer length: {bf.Length()}");
                Log.Error($"Packet length: {packet.Length}");
                Log.Error($"Is Compressed: {compressed}");
                Log.Error(exception);
                return;
            }
            PacketHandler.HandlePacket(bf);
        }

        private static void MySocket_OnDisconnected()
        {
            //Not sure how to handle this yet!
            Globals.IsRunning = false;
        }

        private static void MySocket_OnConnected()
        {
            //Not sure how to handle this yet!
            _connected = true;
        }

        public static void Close(string reason)
        {
            try
            {
                _connected = false;
                Connecting = false;
                MySocket.Disconnect(reason);
                MySocket.Dispose();
                MySocket = null;
            }
            catch (Exception exception)
            {
                Log.Trace(exception);
            }
        }

        public static void SendPacket(byte[] packet)
        {
            try
            {
                var buff = new ByteBuffer();
                if (packet.Length > 800)
                {
                    packet = Compression.CompressPacket(packet);
                    buff.WriteByte(1); //Compressed
                    buff.WriteBytes(packet);
                }
                else
                {
                    buff.WriteByte(0); //Not Compressed
                    buff.WriteBytes(packet);
                }

                MySocket?.SendData(buff.ToArray());
            }
            catch (Exception exception)
            {
                Log.Trace(exception);
            }
        }

        public static void Update()
        {
            MySocket?.Update();
        }
    }
}