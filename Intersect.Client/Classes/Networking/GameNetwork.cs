using System;
using Intersect.Client.Framework.Network;
using Intersect.Client.General;
using Intersect.Client.UI.Menu;
using Intersect.Config;
using Intersect.Logging;
using Intersect.Network.Packets;

namespace Intersect.Client.Networking
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
            MainMenu.OnNetworkConnecting();
            MySocket?.Connect(ClientOptions.ServerHost, ClientOptions.ServerPort);
        }

        private static void MySocket_OnConnectionFailed(bool denied)
        {
            sConnected = false;
            if (!denied) TryConnect();
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
            //PacketHandler.HandlePacket(bf);
        }

        private static void MySocket_OnDisconnected()
        {
            //Not sure how to handle this yet!
            sConnected = false;
            if (Globals.GameState == GameStates.InGame || Globals.GameState == GameStates.Loading)
            {
                Globals.ConnectionLost = true;
                MySocket?.Disconnect("");
                TryConnect();
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

        public static void SendPacket(CerasPacket packet)
        {
            MySocket?.SendPacket(packet);
        }

        public static void Update()
        {
            MySocket?.Update();
        }
    }
}