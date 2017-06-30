using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Intersect.Enums;
using Intersect.Localization;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Maps;
using Lidgren.Network;

namespace Intersect.Server.Classes.Networking
{
    public class Client
    {
        public int EditorMap = -1;
        public Player Entity;
        public int EntityIndex;

        //Client Properties
        public bool IsEditor;

        //Adminastrative punnishments
        public bool Muted = false;
        public string MuteReason = "";

        //Game Incorperation Variables
        public string MyAccount = "";
        public string MyEmail = "";
        public long MyId = -1;
        public string MyPassword = "";
        public string MySalt = "";

        //Network Variables
        private GameSocket mySocket;
        private IConnection connection;
        public int Power = 0;
        private ConcurrentQueue<byte[]> sendQueue = new ConcurrentQueue<byte[]>();

        //Sent Maps
        public Dictionary<int, Tuple<long, int>> SentMaps = new Dictionary<int, Tuple<long, int>>();

        //Processing Thead
        private Thread updateThread;

        public Client(IConnection connection)
            : this(Globals.FindOpenEntity(), connection)
        {
        }

        public Client(int entIndex, IConnection connection)
            : this(entIndex, null, connection)
        {
            _connectTime = Globals.System.GetTimeMs();
            _connectionTimeout = Globals.System.GetTimeMs() + _timeout;
        }

        public Client(int entIndex, GameSocket socket, IConnection connection = null)
        {
            mySocket = socket;
            this.connection = connection;
            EntityIndex = entIndex;
            if (EntityIndex > -1)
            {
                Entity = (Player) Globals.Entities[EntityIndex];
            }
            var gameSocketConnected = mySocket != null && mySocket.IsConnected();
            var beta4SocketConnected = this.connection != null && this.connection.IsConnected;
            if (gameSocketConnected)
            {
                PacketSender.SendPing(this);
            }
            updateThread = new Thread(Update);
            updateThread.Start();
        }

        public void SendPacket(byte[] packetData)
        {
            var buff = new ByteBuffer();
            Debug.Assert(packetData != null, "packetData != null");
            if (packetData.Length > 800)
            {
                packetData = Compression.CompressPacket(packetData);
                buff.WriteInteger(packetData.Length + 1);
                buff.WriteByte(1); //Compressed
                buff.WriteBytes(packetData);
            }
            else
            {
                buff.WriteInteger(packetData.Length + 1);
                buff.WriteByte(0); //Not Compressed
                buff.WriteBytes(packetData);
            }

            if (connection != null)
            {
                connection.Send(new BinaryPacket(null) { Buffer = buff });
            }
            else
            {
                sendQueue?.Enqueue(buff.ToArray());
            }
        }

        public void SendShit()
        {
            //var timer = new System.Timers.Timer(5000);
            //timer.Elapsed += (source, args) =>
            //{
            var random = new CryptoRandom();
            var b = 25000;
            for (var a = 0; a < b; a++)
            {
                Log.Diagnostic($"Sending shit... {a}/{b}");
                for (var c = 10; c < 50; c++)
                {
                    var cap = 10 + (c + random.NextDouble() * 25) % 40;
                    SendPacket(CreateShitPacket(true, -1, false, false, null, 0));
                    for (var i = 0; i < cap; i++)
                    {
                        var shit = CreateShit();
                        var shitSize = Encoding.Unicode.GetByteCount(shit);
                        SendPacket(CreateShitPacket(true, i, false, true, null, 0));
                        SendPacket(CreateShitPacket(true, i, true, false, shit, shitSize));
                        SendPacket(CreateShitPacket(true, i, false, false, null, 0));
                    }
                    SendPacket(CreateShitPacket(false, -1, false, false, null, 0));
                }
                SendPacket(CreateShitPacket(false, -2, false, false, null, 0));
            }
            //};
            //timer.Start();
        }

        private byte[] CreateShitPacket(bool shitting, int num, bool data, bool start, string shit, int shitSize)
        {
            using (var bf = new ByteBuffer())
            {
                bf.WriteLong((int)ServerPackets.Shit);
                bf.WriteBoolean(shitting);
                bf.WriteInteger(num);
                if (num == -1) return bf.ToArray();
                bf.WriteBoolean(data);
                if (data)
                {
                    bf.WriteString(shit);
                    bf.WriteInteger(shitSize);
                }
                else bf.WriteBoolean(start);
                return bf.ToArray();
            }
        }

        private string CreateShit()
        {
            var shit = new byte[512];
            new Random().NextBytes(shit);
            return BitConverter.ToString(shit);
        }

        private long _connectTime;
        private long _connectionTimeout;
        protected long _timeout = 20000; //20 seconds

        public void Pinged()
        {
            if (connection != null)
            {
                _connectionTimeout = Globals.System.GetTimeMs() + _timeout;
                return;
            }

            if (mySocket != null && IsConnected())
            {
                mySocket?.Pinged();
            }
        }

        public void Disconnect(string reason = "")
        {
            if (connection != null)
            {
                connection.Dispose();
                return;
            }

            if (reason == "")
            {
                mySocket?.Disconnect();
            }
            else
            {
                //send abort packet and then disconnect?
            }
        }

        public async void Update()
        {
            if (connection == null)
            {
                try
                {
                    while (mySocket != null && IsConnected() && Globals.ServerStarted)
                    {
                        mySocket.Update();
                        while (sendQueue.TryDequeue(out byte[] data))
                        {
                            if (data != null)
                            {
                                mySocket.SendData(data);
                            }
                        }
                        await Task.Delay(10);
                    }
                }
                catch (Exception ex)
                {
                    Log.Trace(ex);
                    mySocket.Disconnect();
                }
            }
        }

        public bool IsConnected()
        {
            if (connection != null) return connection.IsConnected;
            return mySocket != null && mySocket.IsConnected();
        }

        public string GetIP()
        {
            if (!IsConnected()) return "";

            return connection != null ? connection.Ip : mySocket?.GetIP();
        }

        public static Client CreateBeta4Client(IConnection connection)
        {
            var client = new Client(connection);
            Globals.Entities[client.EntityIndex] = new Player(client.EntityIndex, client);
            lock (Globals.ClientLock)
            {
                Globals.Clients.Add(client);
                Globals.ClientLookup.Add(connection.Guid, client);
            }
            return client;
        }

        public static void RemoveBeta4Client(IConnection connection)
        {
            var client = FindBeta4Client(connection);

            Debug.Assert(client != null, "client != null");
            lock (Globals.ClientLock)
            {
                Globals.Clients.Remove(client);
                Globals.ClientLookup.Remove(connection.Guid);
            }

            Log.Debug(string.IsNullOrWhiteSpace(client.MyAccount)
                //? $"Client disconnected ({(client.IsEditor ? "[editor]" : "[client]")})"
                // TODO: Transmit client information on network start so we can determine editor vs client
                ? $"Client disconnected ([menu])"
                : $"Client disconnected ({client.MyAccount}->{client.Entity?.MyName ?? "[editor]"})");

            if (client.Entity == null) return;

            var en = client.Entity;
            Task.Run(() => Database.SaveCharacter(en));
            var map = MapInstance.Lookup.Get<MapInstance>(client.Entity.CurrentMap);
            map?.RemoveEntity(client.Entity);

            //Update parties
            client.Entity.LeaveParty();

            //Update trade
            client.Entity.CancelTrade();

            //Clear all event spawned NPC's
            var entities = client.Entity.SpawnedNpcs.ToArray();
            foreach (var t in entities)
            {
                if (t == null || t.GetType() != typeof(Npc)) continue;
                if (t.Despawnable) t.Die(0);
            }
            client.Entity.SpawnedNpcs.Clear();

            PacketSender.SendEntityLeave(client.Entity.MyIndex, (int)EntityTypes.Player,
            Globals.Entities[client.EntityIndex].CurrentMap);
            if (!client.IsEditor)
            {
                PacketSender.SendGlobalMsg(Strings.Get("player", "left", client.Entity.MyName, Options.GameName));
            }
            client.Entity.Dispose();
            client.Entity = null;
            Globals.Entities[client.EntityIndex] = null;
        }

        public static Client FindBeta4Client(IConnection connection)
        {
            lock (Globals.ClientLock)
            {
                return Globals.Clients.Find(client => client?.connection == connection);
            }
        }
    }
}