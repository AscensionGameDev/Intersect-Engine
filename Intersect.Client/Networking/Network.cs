﻿using Intersect.Client.Core;
using Intersect.Client.Framework.Network;
using Intersect.Client.General;
using Intersect.Client.Interface.Menu;
using Intersect.Configuration;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Events;

namespace Intersect.Client.Networking
{

    internal static partial class Network
    {

        public static bool Connecting;

        private static bool sConnected;

        public static GameSocket Socket { get; set; }

        internal static PacketHandler? PacketHandler { get; set; }

        private static int sPing;

        public static bool Connected => Socket?.IsConnected() ?? sConnected;

        public static int Ping
        {
            get => Socket?.Ping ?? sPing;
            set => sPing = value;
        }

        public static void InitNetwork(IClientContext context)
        {
            if (Socket == null)
            {
                return;
            }

            Socket.Connected += MySocket_OnConnected;
            Socket.Disconnected += MySocket_OnDisconnected;
            Socket.DataReceived += MySocket_OnDataReceived;
            Socket.ConnectionFailed += MySocket_OnConnectionFailed;
        }

        public static void TryConnect()
        {
            if (Connected)
            {
                return;
            }

            sConnected = false;
            Globals.WaitingOnServer = true;
            Socket?.Connect(ClientConfiguration.Instance.Host, ClientConfiguration.Instance.Port);
        }

        private static void MySocket_OnConnectionFailed(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs, bool denied)
        {
            Globals.WaitingOnServer = false;
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

        private static void MySocket_OnDisconnected(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs)
        {
            Globals.WaitingOnServer = false;
            Fade.Cancel();
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

        private static void MySocket_OnConnected(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs)
        {
            //Not sure how to handle this yet!
            sConnected = true;

            Globals.WaitingOnServer = false;
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

        public static void SendPacket(IntersectPacket packet)
        {
            Socket?.SendPacket(packet);
        }

        public static void Update()
        {
            Socket?.Update();
        }

    }

}
