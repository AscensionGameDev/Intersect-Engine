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
        protected long _pingInterval = 2000;
        protected long _connectionTimeout = -1;
        protected long _timeout = 10000; //10 seconds
        protected bool _isConnected = true;

        public GameSocket()
        {
            _connectTime = Environment.TickCount;
            _connectionTimeout = Environment.TickCount + _timeout;
        }

        public void Start()
        {
            CreateClient();
        }

        public abstract void SendData(byte[] data);
        public abstract void Disconnect();
        public abstract void Dispose();
        public abstract bool IsConnected();
        public abstract string GetIP();
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
            var client = new Client(Globals.FindOpenEntity(), this);
            _myClient = client;
            _entityIndex = client.EntityIndex;
            Globals.Entities[_entityIndex] = new Player(_entityIndex, client);
            lock (Globals.ClientLock)
            {
                Globals.Clients.Add(client);
            }
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

        public virtual void Update()
        {
            if (_connectionTimeout > -1 && _connectionTimeout < Environment.TickCount)
            {
                HandleDisconnect();
                return;
            }
            else
            {
                if (_pingTime < Environment.TickCount)
                {
                    PacketSender.SendPing(_myClient);
                    _pingTime = Environment.TickCount + _pingInterval;
                }
            }
        }

        public virtual void Pinged()
        {
            _connectionTimeout = Environment.TickCount + _timeout;
        }

        protected void HandleDisconnect()
        {
            if (_isConnected)
            {
                try
                {
                    _isConnected = false;
                    Globals.GeneralLogs.Add("Client disconnected.");
                    Database.SaveCharacter(_myClient.Entity);
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
                    Disconnect();
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