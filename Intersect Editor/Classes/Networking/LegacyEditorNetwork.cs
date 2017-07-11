using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Windows.Forms;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Crypto;
using Intersect.Network.Crypto.Formats;
using Intersect.Network.Packets.Reflectable;

namespace Intersect.Editor.Classes
{
    public static class LegacyEditorNetwork
    {

        public static ClientNetwork EditorLidgrenNetwork;

        public static TcpClient MySocket;
        private static NetworkStream _myStream;
        private static bool _connected;
        public static bool Connected => EditorLidgrenNetwork?.IsConnected ?? _connected;
        public static bool Connecting;
        private static byte[] _tempBuff;
        private static ByteBuffer _myBuffer = new ByteBuffer();
        private static object _bufferLock = new object();

        public static void InitNetwork()
        {
            if (EditorLidgrenNetwork != null) return;

            Log.Global.AddOutput(new ConsoleOutput());
            var config = new NetworkConfiguration(Globals.ServerHost, (ushort)Globals.ServerPort);
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Intersect.Editor.public-intersect.bek"))
            {
                var rsaKey = EncryptionKey.FromStream<RsaKey>(stream);
                Debug.Assert(rsaKey != null, "rsaKey != null");
                EditorLidgrenNetwork = new ClientNetwork(config, rsaKey.Parameters);
            }

            EditorLidgrenNetwork.Handlers[PacketCode.BinaryPacket] = PacketHandler.HandlePacket;

            if (!EditorLidgrenNetwork.Connect())
            {
                Log.Error("An error occurred while attempting to connect.");
            }

            if (EditorLidgrenNetwork != null) return;

            MySocket?.Close();

            MySocket = new TcpClient()
            {
                SendBufferSize = 256000,
                ReceiveBufferSize = 256000
            };
            _tempBuff = new byte[MySocket.ReceiveBufferSize];
            MySocket.BeginConnect(Globals.ServerHost, Globals.ServerPort, ConnectCb, null);
            Connecting = true;
        }

        public static void Update()
        {
            if (Connected)
            {
                TryHandleData();
            }
            if (!Connected && !Connecting)
            {
                InitNetwork();
            }
        }

        private static void ConnectCb(IAsyncResult result)
        {
            try
            {
                MySocket.EndConnect(result);
                if (MySocket.Connected)
                {
                    _connected = true;
                    Connecting = false;
                    _myStream = MySocket.GetStream();
                    _myStream.BeginRead(_tempBuff, 0, MySocket.ReceiveBufferSize, ReceiveCb, null);
                }
                else
                {
                    _connected = false;
                    Connecting = false;
                }
            }
            catch (Exception)
            {
                _connected = false;
                Connecting = false;
            }
        }

        public static void CheckNetwork()
        {
            if (Connected == false && Connecting == false)
            {
                InitNetwork();
            }
            else
            {
                if (!Connected)
                {
                    //PROBLEM!
                }
            }
        }

        private static void ReceiveCb(IAsyncResult result)
        {
            try
            {
                var readAmt = _myStream.EndRead(result);
                if (readAmt <= 0)
                {
                    HandleDc();
                }
                var receivedData = new byte[readAmt];
                Buffer.BlockCopy(_tempBuff, 0, receivedData, 0, readAmt);
                lock (_bufferLock)
                {
                    _myBuffer.WriteBytes(receivedData);
                }
                _myStream.BeginRead(_tempBuff, 0, MySocket.ReceiveBufferSize, ReceiveCb, null);
            }
            catch (Exception)
            {
                HandleDc();
            }
        }

        public static void DestroyNetwork()
        {
            try
            {
                _myStream.Close();
                MySocket.Close();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void HandleDc()
        {
            if (Globals.MainForm != null && Globals.MainForm.Visible)
            {
                if (Globals.MainForm.DisconnectDelegate != null)
                {
                    Globals.MainForm.BeginInvoke(Globals.MainForm.DisconnectDelegate);
                    Globals.MainForm.DisconnectDelegate = null;
                }
            }
            /*else if (Globals.LoginForm.Visible)
            {
                _connected = false;
                Connecting = false;
                InitNetwork();
            }*/
            else
            {
                MessageBox.Show(@"Disconnected!");
                Application.Exit();
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
                    buff.WriteInteger(packet.Length);
                    buff.WriteByte(1); //Compressed
                    buff.WriteBytes(packet);
                }
                else
                {
                    buff.WriteInteger(packet.Length);
                    buff.WriteByte(0); //Not Compressed
                    buff.WriteBytes(packet);
                }

                if (EditorLidgrenNetwork != null)
                {
                    if (!EditorLidgrenNetwork.Send(new BinaryPacket(null) { Buffer = buff }))
                    {
                        throw new Exception("Beta 4 network send failed.");
                    }

                    return;
                }

                _myStream.Write(buff.ToArray(), 0, buff.Count());
            }
            catch (Exception)
            {
                HandleDc();
            }
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