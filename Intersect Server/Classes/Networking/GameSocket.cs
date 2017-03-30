using System;
using System.Threading.Tasks;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect.Logging;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Maps;

namespace Intersect.Server.Classes.Networking
{
    public abstract class GameSocket
    {
        protected object _bufferLock = new object();
        protected long _connectionTimeout = -1;
        protected long _connectTime;
        protected int _entityIndex;
        protected bool _isConnected = true;
        protected ByteBuffer _myBuffer = new ByteBuffer();
        protected Client _myClient;
        protected PacketHandler _packetHandler = new PacketHandler();
        protected long _pingInterval = 5000;
        protected long _pingTime;
        protected long _timeout = 20000; //20 seconds

        public GameSocket()
        {
            _connectTime = Globals.System.GetTimeMs();
            _connectionTimeout = Globals.System.GetTimeMs() + _timeout;
        }

        public void Start()
        {
            CreateClient();
        }

        public abstract void SendData(byte[] data);

        public virtual void Disconnect()
        {
            HandleDisconnect();
        }

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
            if (_myClient == null) return;
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
            if (_connectionTimeout > -1 && _connectionTimeout < Globals.System.GetTimeMs() &&
                !System.Diagnostics.Debugger.IsAttached)
            {
                HandleDisconnect();
            }
            else
            {
                if (_pingTime < Globals.System.GetTimeMs())
                {
                    PacketSender.SendPing(_myClient);
                    _pingTime = Globals.System.GetTimeMs() + _pingInterval;
                }
            }
        }

        public virtual void Pinged()
        {
            _connectionTimeout = Globals.System.GetTimeMs() + _timeout;
        }

        protected void HandleDisconnect()
        {
            if (_isConnected && _myClient != null)
            {
                try
                {
                    if (_myClient.Entity != null)
                    {
                        var en = _myClient.Entity;
                        Task.Run(() => Database.SaveCharacter(en));
                        var map = MapInstance.Lookup.Get<MapInstance>(_myClient.Entity.CurrentMap);
                        if (map != null) map.RemoveEntity(_myClient.Entity);

                        //Update parties
                        _myClient.Entity.LeaveParty();

                        //Update trade
                        _myClient.Entity.CancelTrade();

                        //Search for logout activated events and run them
                        /*foreach (var evt in EventBase.Lookup)
                        {
                            _myClient.Entity.StartCommonEvent(evt.Value, (int) EventPage.CommonEventTriggers.LeaveGame);
                        }*/

                        PacketSender.SendEntityLeave(_myClient.Entity.MyIndex, (int) EntityTypes.Player,
                            Globals.Entities[_entityIndex].CurrentMap);
                        if (!_myClient.IsEditor)
                        {
                            PacketSender.SendGlobalMsg(Strings.Get("player", "left", _myClient.Entity.MyName,
                                Options.GameName));
                        }
                        _myClient.Entity.Dispose();
                        _myClient.Entity = null;
                        Globals.Entities[_entityIndex] = null;
                    }
                    lock (Globals.ClientLock)
                    {
                        Globals.Clients.Remove(_myClient);
                        _myClient = null;
                    }
                    Disconnect();
                }
                catch (Exception ex)
                {
                    Log.Trace(ex);
                }
            }
            _isConnected = false;
        }
    }

    public delegate void DataReceivedHandler(byte[] data);

    public delegate void ConnectedHandler();

    public delegate void ConnectionFailedHandler();

    public delegate void DisconnectedHandler();
}