using System;

using Intersect.Client.Framework.Network;
using Intersect.Client.General;
using Intersect.Client.Interface.Menu;
using Intersect.Configuration;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Events;

using JetBrains.Annotations;

namespace Intersect.Client.Networking
{

    public static class Network
    {

        public static bool Connecting;

        private static bool sConnected;

        public static GameSocket Socket;

        private static int sPing;

        public static bool Connected => Socket?.IsConnected() ?? sConnected;

        public static int Ping
        {
            get => Socket?.Ping ?? sPing;
            set => sPing = value;
        }

        public static void InitNetwork()
        {
            if (Socket == null)
            {
                return;
            }

            Socket.Connected += MySocket_OnConnected;
            Socket.Disconnected += MySocket_OnDisconnected;
            Socket.DataReceived += MySocket_OnDataReceived;
            Socket.ConnectionFailed += MySocket_OnConnectionFailed;
            TryConnect();
        }

        private static void TryConnect()
        {
            sConnected = false;
            MainMenu.OnNetworkConnecting();
            Socket?.Connect(ClientConfiguration.Instance.Host, ClientConfiguration.Instance.Port);
        }

        private static void MySocket_OnConnectionFailed([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs, bool denied)
        {
            sConnected = false;
            if (!denied)
            {
                TryConnect();
            }
        }

        private static void MySocket_OnDataReceived(IPacket packet)
        {
            PacketHandler.HandlePacket(packet);
        }

        private static void MySocket_OnDisconnected([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs)
        {
            //Not sure how to handle this yet!
            sConnected = false;
            if (Globals.GameState == GameStates.InGame || Globals.GameState == GameStates.Loading)
            {
                Globals.ConnectionLost = true;
                Socket?.Disconnect("");
                TryConnect();
            }
            else
            {
                Socket?.Disconnect("");
                TryConnect();
            }
        }

        private static void MySocket_OnConnected([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs)
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
                Socket?.Disconnect(reason);
                Socket?.Dispose();
                Socket = null;
            }
            catch (Exception exception)
            {
                Log.Trace(exception);
            }
        }

        public static void SendPacket(CerasPacket packet)
        {
            Socket?.SendPacket(packet);
        }

        public static void Update()
        {
            Socket?.Update();
        }

    }

}
