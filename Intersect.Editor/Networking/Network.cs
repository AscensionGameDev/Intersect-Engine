using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

using Intersect.Configuration;
using Intersect.Editor.General;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Crypto;
using Intersect.Crypto.Formats;
using Intersect.Network.Events;

using JetBrains.Annotations;

namespace Intersect.Editor.Networking
{

    public static class Network
    {

        public static bool Connecting;

        public static bool ConnectionDenied;

        public static ClientNetwork EditorLidgrenNetwork;

        public static bool Connected => EditorLidgrenNetwork?.IsConnected ?? false;

        public static void InitNetwork()
        {
            if (EditorLidgrenNetwork == null)
            {
                var config = new NetworkConfiguration(
                    ClientConfiguration.Instance.Host, ClientConfiguration.Instance.Port
                );

                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("Intersect.Editor.network.handshake.bkey.pub"))
                {
                    var rsaKey = EncryptionKey.FromStream<RsaKey>(stream);
                    Debug.Assert(rsaKey != null, "rsaKey != null");
                    EditorLidgrenNetwork = new ClientNetwork(config, rsaKey.Parameters);
                }

                EditorLidgrenNetwork.Handler = PacketHandler.HandlePacket;
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

        public static void HandleDc([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs)
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

        public static void SendPacket(CerasPacket packet)
        {
            if (EditorLidgrenNetwork != null)
            {
                if (!EditorLidgrenNetwork.Send(packet))
                {
                    throw new Exception("Beta 4 network send failed.");
                }
            }
        }

    }

}
