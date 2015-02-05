using System;
using System.IO;

namespace Intersect_Client
{
    public static class PacketHandler
    {
        public static void HandlePacket(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int packetHeader = (int)bf.ReadLong();
            switch (packetHeader)
            {
                case 0:
                    PacketSender.SendPing();
                    break;
                case 1:
                    HandleJoinGame(bf.ReadBytes(bf.Length()));
                    break;

                case 2:
                    HandleMapData(bf.ReadBytes(bf.Length()));
                    break;
                case 3:
                    HandleEntityData(bf.ReadBytes(bf.Length()));
                    break;
                case 4:
                    HandlePositionInfo(bf.ReadBytes(bf.Length()));
                    break;
                case 5:
                    HandleLeave(bf.ReadBytes(bf.Length()));
                    break;

                case 6:
                    HandleMsg(bf.ReadBytes(bf.Length()));
                    break;

                case 7:
                    HandleGameData(bf.ReadBytes(bf.Length()));
                    break;

                case 8:
                    HandleTilesets(bf.ReadBytes(bf.Length()));
                    break;
                case 9:
                    HandleEnterMap(bf.ReadBytes(bf.Length()));
                    break;
                case 10:
                    HandleEntityMove(bf.ReadBytes(bf.Length()));
                    break;
                case 11:
                    HandleVitals(bf.ReadBytes(bf.Length()));
                    break;
                case 12:
                    HandleStats(bf.ReadBytes(bf.Length()));
                    break;
                case 13:
                    HandleEntityDir(bf.ReadBytes(bf.Length()));
                    break;
                case 14:
                    HandleEventDialog(bf.ReadBytes(bf.Length()));
                    break;
                case 16:
                    HandleLoginError(bf.ReadBytes(bf.Length()));
                    break;
                case 17:
                    HandleGameTime(bf.ReadBytes(bf.Length()));
                    break;
            }
        }

        private static void HandleJoinGame(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.myIndex = (int)bf.ReadLong();
            EntityManager.JoinGame();
            Globals.JoiningGame = true;
        }

        private static void HandleMapData(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            long mapNum = bf.ReadLong();
            long mapLength = bf.ReadLong();
            byte[] mapData = bf.ReadBytes((int)mapLength);
            Globals.GameMaps[mapNum] = new Map((int)mapNum, mapData);


        }

        private static void HandleEntityData(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int i = (int)bf.ReadLong();
            int entityType = bf.ReadInteger();
            if (entityType == 0)
            {
                if (i == Globals.myIndex)
                {
                    EntityManager.AddPlayer(i, bf.ReadString(), bf.ReadString(), true);
                }
                else
                {
                    EntityManager.AddPlayer(i, bf.ReadString(), bf.ReadString(), false);
                }
            }
            else
            {
                EntityManager.AddEvent(i, bf.ReadString(), bf.ReadString(), false);
            }

        }

        private static void HandlePositionInfo(byte[] packet)
        {
            int index;
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            index = (int)bf.ReadLong();
            int isEvent = bf.ReadInteger();
            if (isEvent == 0)
            {
                if (index >= Globals.entities.Count) { return; }
                if (Globals.entities[index] == null) { return; }
                Globals.entities[index].currentMap = bf.ReadInteger();
                Globals.entities[index].currentX = bf.ReadInteger();
                Globals.entities[index].currentY = bf.ReadInteger();
                Globals.entities[index].dir = bf.ReadInteger();
                Globals.entities[index].passable = bf.ReadInteger();
                Globals.entities[index].hideName = bf.ReadInteger();
                if (index == Globals.myIndex)
                {
                    if (Globals.currentMap != Globals.entities[index].currentMap)
                    {
                        Globals.currentMap = Globals.entities[index].currentMap;
                        //Initiate loading screen, we got probz
                        Graphics.fadeStage = 2;
                        Graphics.fadeAmt = 255.0f;
                        Globals.gameLoaded = false;
                        Globals.localMaps[4] = -1;
                    }

                }
            }
            else
            {
                if (index >= Globals.events.Count) { return; }
                if (Globals.events[index] == null) { return; }
                Globals.events[index].currentMap = bf.ReadInteger();
                Globals.events[index].currentX = bf.ReadInteger();
                Globals.events[index].currentY = bf.ReadInteger();
                Globals.events[index].dir = bf.ReadInteger();
                Globals.events[index].passable = bf.ReadInteger();
                Globals.events[index].hideName = bf.ReadInteger();
            }
        }

        private static void HandleLeave(byte[] packet)
        {
            int index;
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            index = (int)bf.ReadLong();
            int isEvent = bf.ReadInteger();
            EntityManager.RemoveEntity(index, isEvent);

        }

        private static void HandleMsg(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.ChatboxContent.Add(bf.ReadString());

        }

        private static void HandleGameData(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapCount = (int)bf.ReadLong();
            Globals.GameMaps = new Map[mapCount];
            Globals.mapCount = mapCount;
            //Database.LoadMapRevisions();
        }

        private static void HandleTilesets(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            long tilesetCount = bf.ReadLong();
            if (tilesetCount > 0)
            {
                Globals.tilesets = new string[tilesetCount];
                for (int i = 0; i < tilesetCount; i++)
                {
                    Globals.tilesets[i] = bf.ReadString();
                }
                Graphics.LoadTilesets(Globals.tilesets);
            }
        }

        private static void HandleEnterMap(byte[] packet)
        {
            Console.WriteLine("Got the map data");
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = (int)bf.ReadLong();
            if (Globals.currentMap == mapNum || Globals.currentMap == -1)
            {
                for (int i = 0; i < 9; i++)
                {
                    Globals.localMaps[i] = (int)bf.ReadLong();
                    if (Globals.localMaps[i] > -1)
                    {
                        if (Globals.GameMaps[Globals.localMaps[i]] == null)
                        {
                            PacketSender.SendNeedMap(Globals.localMaps[i]);
                        }
                    }

                }
                for (int i = 0; i < Globals.GameMaps.Length; i++)
                {
                    if (Globals.GameMaps[i] != null)
                    {
                        if (Globals.GameMaps[i].cacheCleared == false)
                        {
                            for (int x = 0; x < 9; x++)
                            {
                                if (Globals.localMaps[x] == i)
                                {
                                    break;
                                }
                                else
                                {
                                    if (x == 8)
                                    {
                                        Globals.GameMaps[i].ClearCache();
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        private static void HandleEntityMove(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int index = (int)bf.ReadLong();
            int isEvent = bf.ReadInteger();
            if (isEvent == 0)
            {
                if (index >= Globals.entities.Count) { return; }
                if (Globals.entities[index] == null) { return; }
                Globals.entities[index].currentMap = bf.ReadInteger();
                Globals.entities[index].currentX = bf.ReadInteger();
                Globals.entities[index].currentY = bf.ReadInteger();
                Globals.entities[index].dir = bf.ReadInteger();
                Globals.entities[index].isMoving = true;
                switch (Globals.entities[index].dir)
                {
                    case 0:
                        Globals.entities[index].offsetY = 32;
                        Globals.entities[index].offsetX = 0;
                        break;
                    case 1:
                        Globals.entities[index].offsetY = -32;
                        Globals.entities[index].offsetX = 0;
                        break;
                    case 2:
                        Globals.entities[index].offsetY = 0;
                        Globals.entities[index].offsetX = 32;
                        break;
                    case 3:
                        Globals.entities[index].offsetY = 0;
                        Globals.entities[index].offsetX = -32;
                        break;
                }
            }
            else
            {
                if (index >= Globals.events.Count) { return; }
                if (Globals.events[index] == null) { return; }
                Globals.events[index].currentMap = bf.ReadInteger();
                Globals.events[index].currentX = bf.ReadInteger();
                Globals.events[index].currentY = bf.ReadInteger();
                Globals.events[index].dir = bf.ReadInteger();
                Globals.events[index].isMoving = true;
                switch (Globals.events[index].dir)
                {
                    case 0:
                        Globals.events[index].offsetY = 32;
                        Globals.events[index].offsetX = 0;
                        break;
                    case 1:
                        Globals.events[index].offsetY = -32;
                        Globals.events[index].offsetX = 0;
                        break;
                    case 2:
                        Globals.events[index].offsetY = 0;
                        Globals.events[index].offsetX = 32;
                        break;
                    case 3:
                        Globals.events[index].offsetY = 0;
                        Globals.events[index].offsetX = -32;
                        break;
                }
            }
        }

        private static void HandleVitals(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int index = (int)bf.ReadLong();
            int isEvent = bf.ReadInteger();
            if (isEvent == 0)
            {
                if (index >= Globals.entities.Count) { return; }
                if (Globals.entities[index] == null) { return; }
                Globals.entities[index].maxVital[0] = bf.ReadInteger();
                Globals.entities[index].maxVital[1] = bf.ReadInteger();
                Globals.entities[index].vital[0] = bf.ReadInteger();
                Globals.entities[index].vital[1] = bf.ReadInteger();
            }
            else
            {
                if (index >= Globals.events.Count) { return; }
                if (Globals.events[index] == null) { return; }
                Globals.events[index].maxVital[0] = bf.ReadInteger();
                Globals.events[index].maxVital[1] = bf.ReadInteger();
                Globals.events[index].vital[0] = bf.ReadInteger();
                Globals.events[index].vital[1] = bf.ReadInteger();
            }
        }

        private static void HandleStats(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int index = (int)bf.ReadLong();
            int isEvent = bf.ReadInteger();
            if (isEvent == 0)
            {
                if (index >= Globals.entities.Count) { return; }
                if (Globals.entities[index] == null) { return; }
                Globals.entities[index].stat[0] = bf.ReadInteger();
                Globals.entities[index].stat[1] = bf.ReadInteger();
                Globals.entities[index].stat[2] = bf.ReadInteger();
            }
            else
            {
                if (index >= Globals.events.Count) { return; }
                if (Globals.events[index] == null) { return; }
                Globals.events[index].stat[0] = bf.ReadInteger();
                Globals.events[index].stat[1] = bf.ReadInteger();
                Globals.events[index].stat[2] = bf.ReadInteger();
            }
        }

        private static void HandleEntityDir(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int index = (int)bf.ReadLong();
            int isEvent = bf.ReadInteger();
            if (isEvent == 0)
            {
                if (index >= Globals.entities.Count) { return; }
                if (Globals.entities[index] == null) { return; }
                Globals.entities[index].dir = bf.ReadInteger();
            }
            else
            {
                if (index >= Globals.events.Count) { return; }
                if (Globals.events[index] == null) { return; }
                Globals.events[index].dir = bf.ReadInteger();
            }

        }

        private static void HandleEventDialog(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            EventDialog ed = new EventDialog();
            bf.WriteBytes(packet);
            ed.prompt = bf.ReadString();
            ed.type = bf.ReadInteger();
            if (ed.type == 0)
            {

            }
            else
            {
                ed.opt1 = bf.ReadString();
                ed.opt2 = bf.ReadString();
                ed.opt3 = bf.ReadString();
                ed.opt4 = bf.ReadString();
            }
            ed.eventIndex = bf.ReadInteger();
            Globals.EventDialogs.Add(ed);
        }

        private static void HandleLoginError(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            string error = bf.ReadString();
            Graphics.fadeStage = 1;
            Graphics.fadeAmt = 250;
            Globals.WaitingOnServer = false;
            GUI.AddError(error);
        }

        private static void HandleGameTime(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.GameTime = bf.ReadInteger();
        }

    }
}