using System;
using Intersect;
using Intersect.Config;
using Intersect.Logging;
using IntersectClientExtras.Network;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.Networking
{
    public static class GameNetwork
    {
        public static GameSocket MySocket;

        private static bool sConnected;
        public static bool Connecting;

        private static int sPing;
        public static bool Connected => MySocket?.IsConnected() ?? sConnected;

        public static int Ping
        {
            get { return MySocket?.Ping() ?? sPing; }
            set { sPing = value; }
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
            sConnected = false;
            MySocket?.Connect(ClientOptions.ServerHost, ClientOptions.ServerPort);
        }

        private static void MySocket_OnConnectionFailed()
        {
            sConnected = false;
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
            sConnected = false;
            if (Globals.GameState == GameStates.InGame || Globals.GameState == GameStates.Loading)
            {
                Globals.IsRunning = false;
            }
            else
            {
                MySocket?.Disconnect("");
                TryConnect();
            }

        }

        private static void MySocket_OnConnected()
        {
            //Not sure how to handle this yet!
            sConnected = true;
        }

        public static void Close(string reason)
        {
            try
            {
                sConnected = false;
                Connecting = false;
                MySocket?.Disconnect(reason);
                MySocket?.Dispose();
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