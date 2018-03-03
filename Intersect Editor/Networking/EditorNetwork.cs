using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using Intersect.Config;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Crypto;
using Intersect.Network.Crypto.Formats;
using Intersect.Network.Packets.Reflectable;

namespace Intersect.Editor.Classes
{
    public static class EditorNetwork
    {
        public static ClientNetwork EditorLidgrenNetwork;
        public static bool Connecting;
        public static bool ConnectionDenied;
        public static bool Connected => EditorLidgrenNetwork?.IsConnected ?? false;

        public static void InitNetwork()
        {
            if (EditorLidgrenNetwork == null)
            {
                Log.Global.AddOutput(new ConsoleOutput());
                var config = new NetworkConfiguration(ClientOptions.ServerHost, ClientOptions.ServerPort);
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("Intersect.Editor.public-intersect.bek"))
                {
                    var rsaKey = EncryptionKey.FromStream<RsaKey>(stream);
                    Debug.Assert(rsaKey != null, "rsaKey != null");
                    EditorLidgrenNetwork = new ClientNetwork(config, rsaKey.Parameters);
                }

                EditorLidgrenNetwork.Handlers[PacketCode.BinaryPacket] = PacketHandler.HandlePacket;
                EditorLidgrenNetwork.OnDisconnected += HandleDc;
                EditorLidgrenNetwork.OnConnectionDenied += delegate
                {
                    Connecting = false;
                    ConnectionDenied = true;
                };
            }

            if (!Connected)
            {
                Connecting = true;
                if (!EditorLidgrenNetwork.Connect())
                {
                    Log.Error("An error occurred while attempting to connect.");
                }
            }
        }

        public static void Update()
        {
            if (!Connected && !Connecting && !ConnectionDenied)
            {
                InitNetwork();
            }
        }

        public static void DestroyNetwork()
        {
            try
            {
                EditorLidgrenNetwork.Close();
                EditorLidgrenNetwork = null;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void HandleDc(INetworkLayerInterface netInterface,IConnection netConnection)
        {
            DestroyNetwork();
            if (Globals.MainForm != null && Globals.MainForm.Visible)
            {
                if (Globals.MainForm.DisconnectDelegate != null)
                {
                    Globals.MainForm.BeginInvoke(Globals.MainForm.DisconnectDelegate);
                    Globals.MainForm.DisconnectDelegate = null;
                }
            }
            else if (Globals.LoginForm.Visible)
            {
                Connecting = false;
                InitNetwork();
            }
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
                    buff.WriteByte(1); //Compressed
                    buff.WriteBytes(packet);
                }
                else
                {
                    buff.WriteByte(0); //Not Compressed
                    buff.WriteBytes(packet);
                }

                if (EditorLidgrenNetwork != null)
                {
                    if (!EditorLidgrenNetwork.Send(new BinaryPacket(null) {Buffer = buff}))
                    {
                        throw new Exception("Beta 4 network send failed.");
                    }
                }
            }
            catch (Exception)
            {
                HandleDc(null,null);
            }
        }
    }
}