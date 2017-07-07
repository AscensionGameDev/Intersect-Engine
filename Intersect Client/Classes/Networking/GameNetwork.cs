using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Intersect;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Crypto;
using Intersect.Network.Crypto.Formats;
using Intersect.Network.Packets.Reflectable;
using IntersectClientExtras.Network;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.Networking
{
    public static class GameNetwork
    {

        public static ClientNetwork ClientLidgrenNetwork;

        public static GameSocket MySocket;

        private static bool _connected;
        public static bool Connected => ClientLidgrenNetwork?.IsConnected ?? _connected;
        public static bool Connecting;
        private static byte[] _tempBuff;
        private static ByteBuffer _myBuffer = new ByteBuffer();
        private static object _bufferLock = new object();
        public static int Ping = 0;

        public static void InitNetwork()
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

            ClientLidgrenNetwork.Handlers[PacketCode.BinaryPacket] = PacketHandler.HandlePacket;

            if (!ClientLidgrenNetwork.Connect())
            {
                Log.Error("An error occurred while attempting to connect.");
            }

            if (ClientLidgrenNetwork != null) return;

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

        public static void PushData(byte[] data)
        {
            lock (_bufferLock)
            {
                _myBuffer.WriteBytes(data);
            }
            TryHandleData();
        }

        private static void MySocket_OnDataReceived(byte[] data)
        {
            lock (_bufferLock)
            {
                _myBuffer.WriteBytes(data);
            }
            TryHandleData();
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

        public static void Close()
        {
            try
            {
                _connected = false;
                Connecting = false;
                MySocket.Disconnect();
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
                    buff.WriteInteger(packet.Length + 1);
                    buff.WriteByte(1); //Compressed
                    buff.WriteBytes(packet);
                }
                else
                {
                    buff.WriteInteger(packet.Length + 1);
                    buff.WriteByte(0); //Not Compressed
                    buff.WriteBytes(packet);
                }

                if (ClientLidgrenNetwork != null)
                {
                    if (!ClientLidgrenNetwork.Send(new BinaryPacket(null) {Buffer = buff}))
                    {
                        throw new Exception("Beta 4 network send failed.");
                    }

                    return;
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

        private static void TryHandleData()
        {
            lock (_bufferLock)
            {
                while (_myBuffer.Length() >= 4)
                {
                    var packetLen = _myBuffer.ReadInteger(false);
                    if (_myBuffer.Length() >= packetLen + 4)
                    {
                        _myBuffer.ReadInteger();
                        PacketHandler.HandlePacket(_myBuffer.ReadBytes(packetLen));
                    }
                    else
                    {
                        break;
                    }
                }
                if (_myBuffer.Length() == 0)
                {
                    _myBuffer.Clear();
                }
            }
        }
    }
}