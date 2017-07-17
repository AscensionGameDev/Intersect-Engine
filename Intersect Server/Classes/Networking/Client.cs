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
using Intersect.Network.Packets.Reflectable;
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
        public List<Character> Characters = new List<Character>();
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
        private IConnection connection;
        public int Power = 0;

        //Sent Maps
        public Dictionary<int, Tuple<long, int>> SentMaps = new Dictionary<int, Tuple<long, int>>();

        public Client(IConnection connection)
            : this(Globals.FindOpenEntity(), connection)
        {
        }

        public Client()
            : this(Globals.FindOpenEntity())
        {
        }

        public Client(int entIndex)
            : this(entIndex, null)
        {
            this.connection = connection;
            _connectTime = Globals.System.GetTimeMs();
            _connectionTimeout = Globals.System.GetTimeMs() + _timeout;
			
			EntityIndex = entIndex;
            if (EntityIndex > -1)
            {
                Entity = (Player) Globals.Entities[EntityIndex];
            }
        }

        public void SendPacket(byte[] packetData)
        {
            var buff = new ByteBuffer();
            Debug.Assert(packetData != null, "packetData != null");
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

            Socket.SendData(buff);
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
            }
        }

        public void Disconnect(string reason = "")
        {
            if (connection != null)
            {
                connection.Dispose();
                return;
            }
        }

        public bool IsConnected()
        {
            return connection.IsConnected;
        }

        public string GetIP()
        {
            if (!IsConnected()) return "";
            return connection.Ip;
        }

        public virtual void RemoveClient(Client client)
        {
            Debug.Assert(client != null, "client != null");
            lock (Globals.ClientLock)
            {
                Globals.Clients.Remove(client);
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
            client.Socket.OnClientRemoved();
            Globals.Entities[client.EntityIndex] = null;
        }
    }

	public class Character
	{
		public int Slot = 1;
		public string Name = "";
		public string Sprite = "";
		public string Face = "";
		public int Level = 1;
		public int Class = 0;
		public string[] Equipment = new string[Options.EquipmentSlots.Count];

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