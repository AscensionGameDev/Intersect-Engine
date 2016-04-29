using System;
using Intersect_Library;
using Intersect_Server.Classes.Core;
using Intersect_Server.Classes.Entities;
using Intersect_Server.Classes.General;

namespace Intersect_Server.Classes.Networking
{
    public abstract class GameSocket
    {
        protected long _connectTime;
        protected PacketHandler _packetHandler = new PacketHandler();
        protected ByteBuffer _myBuffer = new ByteBuffer();
        protected Object _bufferLock = new Object();
        protected Client _myClient;
        protected int _entityIndex;
        protected long _pingTime;
        protected long _connectionTimeout;
        protected long _timeout = 10;
        protected bool _isConnected;

        public GameSocket()
        {
            CreateClient();
            _connectTime = Environment.TickCount;
        }

        public abstract void SendData(byte[] data);
        public abstract void Update();
        public abstract void Disconnect();
        public abstract void Dispose();
        public abstract bool IsConnected();
        public event DataReceivedHandler DataReceived;
        public event ConnectedHandler Connected;
        public event ConnectionFailedHandler ConnectionFailed;
        public event DisconnectedHandler Disconnected;

        protected void OnDataReceived(byte[] data)
        {
            DataReceived(data);
        }
        protected void OnConnected()
        {
            Connected();
        }
        protected void OnConnectionFailed()
        {
            ConnectionFailed();
        }
        protected void OnDisconnected()
        {
            Disconnected();
        }

        private void CreateClient()
        {
            var tempIndex = FindOpenSocket();
            if (tempIndex > -1)
            {
                Globals.Clients[tempIndex] = new Client(tempIndex, Globals.FindOpenEntity(), this);
                _myClient = Globals.Clients[tempIndex];
                _entityIndex = Globals.Clients[tempIndex].EntityIndex;
                Globals.Entities[_entityIndex] = new Player(_entityIndex, Globals.Clients[tempIndex]);
                Globals.GeneralLogs.Add("Client connected using client index of " + tempIndex);
            }
            else
            {
                Globals.GeneralLogs.Add("Rejecting client due to lack of space.");
            }
        }

        private static int FindOpenSocket()
        {
            for (var i = 0; i < Globals.Clients.Count; i++)
            {
                if (Globals.Clients[i] == null)
                {
                    //return i; --Need to fix before allowing the reuse of indices.
                }
                else if (i == Globals.Clients.Count - 1)
                {
                    Globals.Clients.Add(null);
                    Globals.ClientThread.Add(null);
                    return Globals.Clients.Count - 1;
                }
            }
            Globals.ClientThread.Add(null);
            Globals.Clients.Add(null);
            return Globals.Clients.Count - 1;
        }

        protected void TryHandleData()
        {
            int packetLen;
            lock (_bufferLock)
            {
                while (_myBuffer.Length() >= 4)
                {
                    packetLen = _myBuffer.ReadInteger(false);
                    if (packetLen == 0)
                    {
                        break;
                    }
                    if (_myBuffer.Length() >= packetLen + 4)
                    {
                        _myBuffer.ReadInteger();
                        _packetHandler.HandlePacket(_myClient, _myBuffer.ReadBytes(packetLen));
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

        protected void HandleDisconnect()
        {
            if (_isConnected)
            {
                try
                {
                    _isConnected = false;
                    Globals.GeneralLogs.Add("Client disconnected.");
                    Database.SavePlayer(_myClient);
                    if (_entityIndex > -1 && Globals.Entities[_entityIndex] != null && Globals.Entities[_entityIndex].MyName != "")
                    {
                        PacketSender.SendEntityLeave(_entityIndex, (int)EntityTypes.Player, Globals.Entities[_entityIndex].CurrentMap);
                        if (Globals.Entities[_entityIndex] == null) { return; }
                        if (!_myClient.IsEditor)
                        {
                            PacketSender.SendGlobalMsg(Globals.Entities[_entityIndex].MyName + " has left the Intersect engine");
                        }
                        _myClient.Entity = null;
                        _myClient = null;
                        Globals.Entities[_entityIndex] = null;

                    }
                }
                catch (Exception) { }
            }
            _isConnected = false;
        }
    }

    public delegate void DataReceivedHandler(byte[] data);
    public delegate void ConnectedHandler();
    public delegate void ConnectionFailedHandler();
    public delegate void DisconnectedHandler();
}
