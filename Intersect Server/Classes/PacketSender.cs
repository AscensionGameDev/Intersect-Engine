using System;

namespace Intersect_Server.Classes
{
    public static class PacketSender
    {

        public static void SendPing(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.RequestPing);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
            client.connectionTimeout = Environment.TickCount + (client.timeoutLength * 1000);
        }

        public static void SendJoinGame(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.JoinGame);
            bf.WriteLong(client.entityIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMap(Client client, int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.MapData);
            bf.WriteLong(mapNum);
            if (client.isEditor)
            {
                bf.WriteLong(Globals.GameMaps[mapNum].MapData.Length);
                bf.WriteBytes(Globals.GameMaps[mapNum].MapData);
            }
            else
            {
                bf.WriteLong(Globals.GameMaps[mapNum].MapGameData.Length);
                bf.WriteBytes(Globals.GameMaps[mapNum].MapGameData);
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();

        }

        public static void SendEntityData(Client client, int sendIndex, int entityType, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityData);
            bf.WriteLong(sendIndex);
            bf.WriteInteger(entityType);
            bf.WriteString(en.MyName);
            bf.WriteString(en.MySprite);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
            SendEntityVitalsTo(client, sendIndex,entityType,en);
            SendEntityStatsTo(client, sendIndex,entityType,en);
        }

        public static void SendEntityDataToAll(int sendIndex, int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityData);
            bf.WriteLong(sendIndex);
            bf.WriteInteger(isEvent);
            bf.WriteString(en.MyName);
            bf.WriteString(en.MySprite);
            SendDataToAll(bf.ToArray());
            bf.Dispose();
            SendEntityVitals(sendIndex,isEvent,en);
            SendEntityStats(sendIndex,isEvent,en);
        }

        public static void SendEntityPositionTo(Client client, int sendIndex, int isEvent, Entity en)
        {
            if (en == null) { return; }
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityPosition);
            bf.WriteLong(sendIndex);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(en.Passable);
            bf.WriteInteger(en.HideName);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityPositionToAll(int sendIndex, int isEvent, Entity en)
        {
            if (en == null) { return; }
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityPosition);
            bf.WriteLong(sendIndex);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(en.Passable);
            bf.WriteInteger(en.HideName);
            SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityLeave(int index, int isEvent)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityLeave);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            SendDataToAllBut(index, bf.ToArray(), true);
            bf.Dispose();
        }

        public static void SendEntityLeaveTo(Client client,int index, int isEvent)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityLeave);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendDataToAll(byte[] packet)
        {
            foreach (var t in Globals.Clients)
            {
                if (t == null) continue;
                if (t.isConnected && ((Player)Globals.Entities[t.entityIndex]).InGame)
                {
                    t.SendPacket(packet);
                }
            }
        }

        public static void SendDataTo(Client client, byte[] packet)
        {
            client.SendPacket(packet);
        }

        public static void SendPlayerMsg(Client client, string message)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.ChatMessage);
            bf.WriteString(message);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendGameData(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.GameData);
            bf.WriteLong(Globals.MapCount); //Map Count
            if (client.isEditor)
            {
                for (var i = 0; i < Globals.MapCount; i++)
                {
                    bf.WriteString(Globals.GameMaps[i].MyName);
                }
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendGlobalMsg(string message)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.ChatMessage);
            bf.WriteString(message);
            SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendTilesets(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.TilesetArray);
            if (Globals.Tilesets != null)
            {
                bf.WriteLong(Globals.Tilesets.Length);
                foreach (var t in Globals.Tilesets)
                {
                    bf.WriteString(t);
                }
            }
            else
            {
                bf.WriteLong(0);
            }
            client.SendPacket(bf.ToArray());
        }

        public static void SendEnterMap(Client client, int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EnterMap);
            bf.WriteLong(mapNum);
            Globals.GameMaps[mapNum].PlayerEnteredMap();
            for (var y = Globals.GameMaps[mapNum].MapGridY - 1; y < Globals.GameMaps[mapNum].MapGridY + 2; y++)
            {
                for (var x = Globals.GameMaps[mapNum].MapGridX - 1; x < Globals.GameMaps[mapNum].MapGridX + 2; x++)
                {
                    bf.WriteLong(Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[x, y]);
                }
            }


            client.SendPacket(bf.ToArray());
            bf.Dispose();
            
        }

        public static void SendDataToAllBut(int index, byte[] packet, bool entityId)
        {
            for (var i = 0; i < Globals.Clients.Count; i++)
            {
                if (Globals.Clients[i] == null) continue;
                if ((!entityId || Globals.Clients[i].entityIndex == index) && (entityId || i == index)) continue;
                if (!Globals.Clients[i].isConnected || Globals.Clients[i].entityIndex <= -1) continue;
                if (Globals.Entities[Globals.Clients[i].entityIndex] == null) continue;
                if (((Player)Globals.Entities[Globals.Clients[i].entityIndex]).InGame)
                {
                    Globals.Clients[i].SendPacket(packet);
                }
            }
        }

        public static void SendEntityMove(int index, int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityMove);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            SendDataToAllBut(index, bf.ToArray(), true);
            bf.Dispose();
        }

        public static void SendEntityMoveTo(Client client,int index, int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityMove);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityVitals(int index,int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityVitals);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            for (var i = 0; i < (int) Enums.Vitals.VitalCount; i++)
            {
                bf.WriteInteger(en.MaxVital[i]);
                bf.WriteInteger(en.Vital[i]); 
            }
            SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityStats(int index,int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityStats);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            for (var i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                bf.WriteInteger(en.Stat[i]);
            }
            SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityVitalsTo(Client client, int index,int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityVitals);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            for (var i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                bf.WriteInteger(en.MaxVital[i]);
                bf.WriteInteger(en.Vital[i]);
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityStatsTo(Client client, int index,int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityStats);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            for (var i = 0; i < (int) Enums.Stats.StatCount; i++)
            {
                bf.WriteInteger(en.Stat[i]);
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityDir(int index, int isEvent)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityDir);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(Globals.Entities[index].Dir);
            SendDataToAllBut(index, bf.ToArray(), true);
            bf.Dispose();
        }

        public static void SendEventDialog(Client client, string prompt, int eventIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EventDialog);
            bf.WriteString(prompt);
            bf.WriteInteger(0);
            bf.WriteInteger(eventIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }
        public static void SendEventDialog(Client client, string prompt,string opt1, string opt2, string opt3, string opt4, int eventIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EventDialog);
            bf.WriteString(prompt);
            bf.WriteInteger(1);
            bf.WriteString(opt1);
            bf.WriteString(opt2);
            bf.WriteString(opt3);
            bf.WriteString(opt4);
            bf.WriteInteger(eventIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapList(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.MapList);
            bf.WriteInteger(Globals.GameMaps.Length);
            foreach (var t in Globals.GameMaps)
            {
                if (t != null)
                {
                    bf.WriteString(t.MyName);
                    bf.WriteInteger(t.Deleted);
                }
                else
                {
                    bf.WriteString("");
                    bf.WriteInteger(1);
                }
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendLoginError(Client client,string error)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.LoginError);
            bf.WriteString(error);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendGameTime(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.GameTime);
            bf.WriteInteger(Globals.GameTime);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendItem(Client client, long itemNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.ItemData);
            bf.WriteLong(itemNum);
            bf.WriteBytes(Globals.GameItems[itemNum].ItemData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendItemEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.OpenItemEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }
    }
}

