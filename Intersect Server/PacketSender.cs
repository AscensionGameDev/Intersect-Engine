using System;
namespace IntersectServer
{
    public static class PacketSender
    {

        public static void SendPing(Client client)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(0);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
            client.connectionTimeout = Environment.TickCount + (client.timeoutLength * 1000);
        }

        public static void SendJoinGame(Client client)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(1);
            bf.WriteLong(client.entityIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMap(Client client, int mapNum)
        {
            Console.WriteLine("Sending the map!");
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(2);
            bf.WriteLong(mapNum);
            if (client.isEditor)
            {
                bf.WriteLong(GlobalVariables.GameMaps[mapNum].mapData.Length);
                bf.WriteBytes(GlobalVariables.GameMaps[mapNum].mapData);
            }
            else
            {
                bf.WriteLong(GlobalVariables.GameMaps[mapNum].mapGameData.Length);
                bf.WriteBytes(GlobalVariables.GameMaps[mapNum].mapGameData);
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();

        }

        public static void SendEntityData(Client client, int sendIndex, int entityType, Entity en)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(3);
            bf.WriteLong(sendIndex);
            bf.WriteInteger(entityType);
            bf.WriteString(en.myName);
            bf.WriteString(en.mySprite);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
            SendEntityVitalsTo(client, sendIndex,entityType,en);
            SendEntityStatsTo(client, sendIndex,entityType,en);
        }

        public static void SendEntityDataToAll(int sendIndex, int isEvent, Entity en)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(3);
            bf.WriteLong(sendIndex);
            bf.WriteInteger(isEvent);
            bf.WriteString(en.myName);
            bf.WriteString(en.mySprite);
            SendDataToAll(bf.ToArray());
            bf.Dispose();
            SendEntityVitals(sendIndex,isEvent,en);
            SendEntityStats(sendIndex,isEvent,en);
        }

        public static void SendEntityPositionTo(Client client, int sendIndex, int isEvent, Entity en)
        {
            if (en == null) { return; }
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(4);
            bf.WriteLong(sendIndex);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.currentMap);
            bf.WriteInteger(en.currentX);
            bf.WriteInteger(en.currentY);
            bf.WriteInteger(en.dir);
            bf.WriteInteger(en.passable);
            bf.WriteInteger(en.hideName);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityPositionToAll(int sendIndex, int isEvent, Entity en)
        {
            if (en == null) { return; }
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(4);
            bf.WriteLong(sendIndex);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.currentMap);
            bf.WriteInteger(en.currentX);
            bf.WriteInteger(en.currentY);
            bf.WriteInteger(en.dir);
            bf.WriteInteger(en.passable);
            bf.WriteInteger(en.hideName);
            SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityLeave(int index, int isEvent)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(5);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            SendDataToAllBut(index, bf.ToArray(), true);
            bf.Dispose();
        }

        public static void SendEntityLeaveTo(Client client,int index, int isEvent)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(5);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendDataToAll(byte[] packet)
        {
            for (int i = 0; i < GlobalVariables.clients.Count; i++)
            {
                if (GlobalVariables.clients[i] != null)
                {
                    if (GlobalVariables.clients[i].isConnected && ((Player)GlobalVariables.entities[GlobalVariables.clients[i].entityIndex]).inGame)
                    {
                        GlobalVariables.clients[i].SendPacket(packet);
                    }
                }
            }
        }

        public static void SendDataTo(Client client, byte[] packet)
        {
            client.SendPacket(packet);
        }

        public static void SendPlayerMsg(Client client, string message)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(6);
            bf.WriteString(message);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendGameData(Client client)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(7);
            bf.WriteLong(GlobalVariables.mapCount); //Map Count
            if (client.isEditor)
            {
                for (int i = 0; i < GlobalVariables.mapCount; i++)
                {
                    bf.WriteString(GlobalVariables.GameMaps[i].myName);
                }
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendGlobalMsg(string message)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(6);
            bf.WriteString(message);
            SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendTilesets(Client client)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(8);
            if (GlobalVariables.tilesets != null)
            {
                bf.WriteLong(GlobalVariables.tilesets.Length);
                for (int i = 0; i < GlobalVariables.tilesets.Length; i++)
                {
                    bf.WriteString(GlobalVariables.tilesets[i]);
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
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(9);
            bf.WriteLong(mapNum);
            GlobalVariables.GameMaps[mapNum].PlayerEnteredMap();
            for (int y = GlobalVariables.GameMaps[mapNum].mapGridY - 1; y < GlobalVariables.GameMaps[mapNum].mapGridY + 2; y++)
            {
                for (int x = GlobalVariables.GameMaps[mapNum].mapGridX - 1; x < GlobalVariables.GameMaps[mapNum].mapGridX + 2; x++)
                {
                    bf.WriteLong(Database.mapGrids[GlobalVariables.GameMaps[mapNum].mapGrid].myGrid[x, y]);
                }
            }


            client.SendPacket(bf.ToArray());
            bf.Dispose();
            
        }

        public static void SendDataToAllBut(int index, byte[] packet, bool entityID)
        {
            for (int i = 0; i < GlobalVariables.clients.Count; i++)
            {

                if (GlobalVariables.clients[i] != null)
                {
                    if ((entityID && GlobalVariables.clients[i].entityIndex != index) || (!entityID && i != index))
                    {
                        if (GlobalVariables.clients[i].isConnected && GlobalVariables.clients[i].entityIndex > -1)
                        {
                            if (GlobalVariables.entities[GlobalVariables.clients[i].entityIndex] != null)
                            {
                                if (((Player)GlobalVariables.entities[GlobalVariables.clients[i].entityIndex]).inGame)
                                {
                                    GlobalVariables.clients[i].SendPacket(packet);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void SendEntityMove(int index, int isEvent, Entity en)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(10);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.currentMap);
            bf.WriteInteger(en.currentX);
            bf.WriteInteger(en.currentY);
            bf.WriteInteger(en.dir);
            PacketSender.SendDataToAllBut(index, bf.ToArray(), true);
            bf.Dispose();
        }

        public static void SendEntityMoveTo(Client client,int index, int isEvent, Entity en)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(10);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.currentMap);
            bf.WriteInteger(en.currentX);
            bf.WriteInteger(en.currentY);
            bf.WriteInteger(en.dir);
            PacketSender.SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityVitals(int index,int isEvent, Entity en)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(11);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.maxVital[0]);
            bf.WriteInteger(en.maxVital[1]);
            bf.WriteInteger(en.vital[0]);
            bf.WriteInteger(en.vital[1]);
            PacketSender.SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityStats(int index,int isEvent, Entity en)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(12);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.stat[0]);
            bf.WriteInteger(en.stat[1]);
            bf.WriteInteger(en.stat[2]);
            PacketSender.SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityVitalsTo(Client client, int index,int isEvent, Entity en)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(11);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.maxVital[0]);
            bf.WriteInteger(en.maxVital[1]);
            bf.WriteInteger(en.vital[0]);
            bf.WriteInteger(en.vital[1]);
            PacketSender.SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityStatsTo(Client client, int index,int isEvent, Entity en)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(12);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.stat[0]);
            bf.WriteInteger(en.stat[1]);
            bf.WriteInteger(en.stat[2]);
            PacketSender.SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityDir(int index, int isEvent)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(13);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(GlobalVariables.entities[index].dir);
            PacketSender.SendDataToAllBut(index, bf.ToArray(), true);
            bf.Dispose();
        }

        public static void SendEventDialog(Client client, string prompt, int eventIndex)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(14);
            bf.WriteString(prompt);
            bf.WriteInteger(0);
            bf.WriteInteger(eventIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }
        public static void SendEventDialog(Client client, string prompt,string opt1, string opt2, string opt3, string opt4, int eventIndex)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(14);
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
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(15);
            bf.WriteInteger(GlobalVariables.GameMaps.Length);
            for (int i = 0; i < GlobalVariables.GameMaps.Length;i++ )
            {
                if (GlobalVariables.GameMaps[i] != null)
                {
                    bf.WriteString(GlobalVariables.GameMaps[i].myName);
                    bf.WriteInteger(GlobalVariables.GameMaps[i].deleted);
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
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(16);
            bf.WriteString(error);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendGameTime(Client client)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(17);
            bf.WriteInteger(GlobalVariables.GameTime);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }
    }
}

