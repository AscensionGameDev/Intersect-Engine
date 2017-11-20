using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;
using Intersect.Localization;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets.Reflectable;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Maps;
using Lidgren.Network;

namespace Intersect.Server.Classes.Networking
{
    public class Client
    {
        private long mConnectionTimeout;

        private long mConnectTime;
        protected long mTimeout = 20000; //20 seconds
        public List<Character> Characters = new List<Character>();

        //Network Variables
        private IConnection mConnection;

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
        public int Power = 0;
        private ConcurrentQueue<byte[]> mSendQueue = new ConcurrentQueue<byte[]>();

        //Sent Maps
        public Dictionary<int, Tuple<long, int>> SentMaps = new Dictionary<int, Tuple<long, int>>();

        public Client(IConnection connection)
            : this(Globals.FindOpenEntity(), connection)
        {
        }

        public Client(int entIndex, IConnection connection = null)
        {
            this.mConnection = connection;
            mConnectTime = Globals.System.GetTimeMs();
            mConnectionTimeout = Globals.System.GetTimeMs() + mTimeout;
            EntityIndex = entIndex;
            if (EntityIndex > -1)
            {
                Entity = (Player) Globals.Entities[EntityIndex];
            }
        }

        private int mPacketCount = 0;
        private bool mDebugPackets = false;
        public void SendPacket(byte[] packetData)
        {
            var buff = new ByteBuffer();
            Debug.Assert(packetData != null, "packetData != null");
            mPacketCount++;
            if (mDebugPackets)
            {
                var bf = new ByteBuffer();
                bf.WriteBytes(packetData);
                var header = (ServerPackets) bf.ReadLong();
                Console.WriteLine("Sent " + header + " - " + mPacketCount);
            }
            if (packetData.Length > 800)
            {
                packetData = Compression.CompressPacket(packetData);
                buff.WriteByte(1); //Compressed
                buff.WriteBytes(packetData);
            }
            else
            {
                buff.WriteByte(0); //Not Compressed
                buff.WriteBytes(packetData);
            }

            if (mConnection != null)
            {
                mConnection.Send(new BinaryPacket(null) {Buffer = buff});
            }
            else
            {
                mSendQueue?.Enqueue(buff.ToArray());
            }
        }

        public void SendShit()
        {
            //var timer = new System.Timers.Timer(5000);
            //timer.Elapsed += (source, args) =>
            //{
            var random = new CryptoRandom();
            var b = 0;
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
                bf.WriteLong((int) ServerPackets.Shit);
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

        public void Pinged()
        {
            if (mConnection != null)
            {
                mConnectionTimeout = Globals.System.GetTimeMs() + mTimeout;
            }
        }

        public void Disconnect(string reason = "")
        {
            if (mConnection != null)
            {
                mConnection.Dispose();
                return;
            }
        }

        public bool IsConnected()
        {
            return mConnection.IsConnected;
        }

        public string GetIp()
        {
            if (!IsConnected()) return "";

            return mConnection.Ip;
        }

        public static Client CreateBeta4Client(IConnection connection)
        {
            var client = new Client(connection);
            try
            {
                Globals.Entities[client.EntityIndex] = new Player(client.EntityIndex, client);
                lock (Globals.ClientLock)
                {
                    Globals.Clients.Add(client);
                    Globals.ClientLookup.Add(connection.Guid, client);
                }
                return client;
            }
            finally
            {
                client.SendShit();
            }
        }

        public static void RemoveBeta4Client(IConnection connection)
        {
            if (connection == null) return;

            var client = FindBeta4Client(connection);
            if (client == null) return;

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

            PacketSender.SendEntityLeave(client.Entity.MyIndex, (int) EntityTypes.Player, client.Entity.CurrentMap);
            if (!client.IsEditor)
            {
                PacketSender.SendGlobalMsg(Strings.Get("player", "left", client.Entity.MyName, Options.GameName));
            }
            client.Entity.Dispose();
            client.Entity = null;
            if (client.EntityIndex <= Globals.Entities.Count)
                Globals.Entities[client.EntityIndex] = null;
        }

        public static Client FindBeta4Client(IConnection connection)
        {
            lock (Globals.ClientLock)
            {
                return Globals.Clients.Find(client => client?.mConnection == connection);
            }
        }
    }

    public class Character
    {
        public int Class = 0;
        public string[] Equipment = new string[Options.EquipmentSlots.Count];
        public string Face = "";
        public int Level = 1;
        public string Name = "";
        public int Slot = 1;
        public string Sprite = "";

        public Character(int slot, string name, string sprite, string face, int level, int charClass)
        {
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                Equipment[i] = "";
            }
            Slot = slot;
            Name = name;
            Sprite = sprite;
            Face = face;
            Level = level;
            Class = charClass;
        }
    }
}