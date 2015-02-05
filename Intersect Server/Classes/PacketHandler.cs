using System;
using System.IO;

namespace Intersect_Server.Classes
{
    public class PacketHandler
    {
        public void HandlePacket(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var packetHeader = (Enums.ClientPackets)bf.ReadLong();
            packet = bf.ReadBytes(bf.Length());
            bf.Dispose();

            switch (packetHeader)
            {
                case Enums.ClientPackets.Ping:
                    HandlePing(client);
                    break;
                case Enums.ClientPackets.Login:
                    HandleLogin(client, packet);
                    break;
                case Enums.ClientPackets.NeedMap:
                    HandleNeedMap(client, packet);
                    break;
                case Enums.ClientPackets.SendMove:
                    HandlePlayerMove(client, packet);
                    break;
                case Enums.ClientPackets.LocalMessage:
                    HandleLocalMsg(client, packet);
                    break;
                case Enums.ClientPackets.EditorLogin:
                    HandleEditorLogin(client, packet);
                    break;
                case Enums.ClientPackets.SaveTilesetArray:
                    HandleTilesets(client, packet);
                    break;
                case Enums.ClientPackets.SaveMap:
                    HandleMap(client, packet);
                    break;
                case Enums.ClientPackets.CreateMap:
                    HandleCreateMap(client, packet);
                    break;
                case Enums.ClientPackets.EnterMap:
                    HandleEnterMap(client, packet);
                    break;
                case Enums.ClientPackets.TryAttack:
                    HandleTryAttack(client, packet);
                    break;
                case Enums.ClientPackets.SendDir:
                    HandleDir(client, packet);
                    break;
                case Enums.ClientPackets.EnterGame:
                    HandleEnterGame(client, packet);
                    break;
                case Enums.ClientPackets.ActivateEvent:
                    HandleActivateEvent(client, packet);
                    break;
                case Enums.ClientPackets.EventResponse:
                    HandleEventResponse(client, packet);
                    break;
                case Enums.ClientPackets.CreateAccount:
                    HandleCreateAccount(client, packet);
                    break;
                case Enums.ClientPackets.OpenItemEditor:
                    HandleItemEditor(client);
                    break;
                case Enums.ClientPackets.SaveItem:
                    HandleItemData(client, packet);
                    break;
                default:
                    Console.WriteLine(@"Non implemented packet received: " + packetHeader);
                    break;
            }

        }

        private static void HandlePing(Client client)
        {
            client.connectionTimeout = -1;
            client.pingTime = Environment.TickCount + 10000;
        }

        private static void HandleLogin(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = client.entityIndex;
            var username = bf.ReadString();
            var password = bf.ReadString();
            if (Database.AccountExists(username))
            {
                if (Database.CheckPassword(username, password))
                {
                    Globals.Entities[index] = new Player(index, client) {MyName = username};
                    Console.WriteLine(Globals.Entities[index].MyName + " logged in.");
                    client.id = Database.GetUserId(username);
                    Database.LoadPlayer(client);
                    PacketSender.SendJoinGame(client);
                }
                else
                {
                    PacketSender.SendLoginError(client, "Username or password incorrect.");
                }
            }
            else
            {
                PacketSender.SendLoginError(client, "Username or password incorrect.");
            }
            bf.Dispose();
        }

        private static void HandleNeedMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            PacketSender.SendMap(client, (int)bf.ReadLong());
        }

        private static void HandlePlayerMove(Client client, byte[] packet)
        {
            var index = client.entityIndex;
            var oldMap = Globals.Entities[index].CurrentMap;
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.Entities[index].CurrentMap = bf.ReadInteger();
            if (oldMap != Globals.Entities[index].CurrentMap)
            {
                Globals.GameMaps[Globals.Entities[index].CurrentMap].PlayerEnteredMap();
            }
            Globals.Entities[index].CurrentX = bf.ReadInteger();
            Globals.Entities[index].CurrentY = bf.ReadInteger();
            Globals.Entities[index].Dir = bf.ReadInteger();
            bf.Dispose();

            //TODO: Add Check if valid before sending the move to everyone.
            PacketSender.SendEntityMove(index, 0, Globals.Entities[index]);
        }

        private static void HandleLocalMsg(Client client, byte[] packet)
        {
            var index = client.entityIndex;
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var msg = bf.ReadString();
            if (msg == "killme")
            {
                Globals.Entities[client.entityIndex].Die();
            }
            PacketSender.SendGlobalMsg(((Player)Globals.Entities[index]).MyName + ": " + msg);
            bf.Dispose();
        }

        private static void HandleEditorLogin(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var usr = bf.ReadString();
            var pass = bf.ReadString();
            if (usr != "jcsnider" && (usr != "kibbelz" || pass != "test")) return;
            client.isEditor = true;
            PacketSender.SendJoinGame(client);
            PacketSender.SendGameData(client);
            PacketSender.SendTilesets(client);
            PacketSender.SendMapList(client);
        }

        private static void HandleTilesets(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var tilesetCount = bf.ReadLong();
            if (tilesetCount <= 0) return;
            Globals.Tilesets = new string[tilesetCount];
            for (var i = 0; i < tilesetCount; i++)
            {
                Globals.Tilesets[i] = bf.ReadString();
            }
            File.WriteAllLines("Resources/Tilesets.dat", Globals.Tilesets);

            //Send the updated tilesets to all clients.
            for (var i = 0; i < Globals.Clients.Count; i++)
            {
                if (i == client.clientIndex) continue;
                if (Globals.Clients[i] == null) continue;
                if (Globals.Clients[i].isConnected)
                {
                    PacketSender.SendTilesets(Globals.Clients[i]);
                }
            }
        }

        private static void HandleMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapNum = bf.ReadLong();
            var mapLength = bf.ReadLong();
            Globals.GameMaps[mapNum].Load(bf.ReadBytes((int)mapLength));
            Globals.GameMaps[mapNum].Save();
            foreach (var t in Globals.Clients)
            {
                if (t == null) continue;
                if (t.isEditor)
                {
                    PacketSender.SendMapList(t);
                }
            }
            bf.Dispose();
        }

        private static void HandleCreateMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            var newMap = -1;
            var tmpMap = new Map(-1);
            bf.WriteBytes(packet);
            var location = (int)bf.ReadLong();
            if (location == -1)
            {
                newMap = Database.AddMap();
                tmpMap = Globals.GameMaps[newMap];
                tmpMap.Save();
                Database.GenerateMapGrids();
                PacketSender.SendMap(client, newMap);
                PacketSender.SendEnterMap(client, newMap);
            }
            else
            {
                var relativeMap = (int)bf.ReadLong();
                switch (location)
                {
                    case 0:
                        if (Globals.GameMaps[relativeMap].Up == -1)
                        {
                            newMap = Database.AddMap();
                            tmpMap = Globals.GameMaps[newMap];
                            tmpMap.MapGrid = Globals.GameMaps[relativeMap].MapGrid;
                            tmpMap.MapGridX = Globals.GameMaps[relativeMap].MapGridX;
                            tmpMap.MapGridY = Globals.GameMaps[relativeMap].MapGridY - 1;
                            Globals.GameMaps[relativeMap].Up = newMap;

                        }
                        break;

                    case 1:
                        if (Globals.GameMaps[relativeMap].Down == -1)
                        {
                            newMap = Database.AddMap();
                            tmpMap = Globals.GameMaps[newMap];
                            tmpMap.MapGrid = Globals.GameMaps[relativeMap].MapGrid;
                            tmpMap.MapGridX = Globals.GameMaps[relativeMap].MapGridX;
                            tmpMap.MapGridY = Globals.GameMaps[relativeMap].MapGridY + 1;
                            Globals.GameMaps[relativeMap].Down = newMap;
                        }
                        break;

                    case 2:
                        if (Globals.GameMaps[relativeMap].Left == -1)
                        {
                            newMap = Database.AddMap();
                            tmpMap = Globals.GameMaps[newMap];
                            tmpMap.MapGrid = Globals.GameMaps[relativeMap].MapGrid;
                            tmpMap.MapGridX = Globals.GameMaps[relativeMap].MapGridX - 1;
                            tmpMap.MapGridY = Globals.GameMaps[relativeMap].MapGridY;
                            Globals.GameMaps[relativeMap].Left = newMap;
                        }
                        break;

                    case 3:
                        if (Globals.GameMaps[relativeMap].Right == -1)
                        {
                            newMap = Database.AddMap();
                            tmpMap = Globals.GameMaps[newMap];
                            tmpMap.MapGrid = Globals.GameMaps[relativeMap].MapGrid;
                            tmpMap.MapGridX = Globals.GameMaps[relativeMap].MapGridX + 1;
                            tmpMap.MapGridY = Globals.GameMaps[relativeMap].MapGridY;
                            Globals.GameMaps[relativeMap].Right = newMap;
                        }
                        break;

                }

                if (newMap > -1)
                {
                    Globals.GameMaps[relativeMap].Save();
                    if (tmpMap.MapGridY - 1 >= 0)
                    {
                        tmpMap.Up = Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX, tmpMap.MapGridY - 1];
                    }
                    if (tmpMap.MapGridY + 1 <= Database.MapGrids[tmpMap.MapGrid].Height)
                    {
                        tmpMap.Down = Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX, tmpMap.MapGridY + 1];
                    }

                    if (tmpMap.MapGridX - 1 >= 0) { tmpMap.Left = Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX - 1, tmpMap.MapGridY]; }

                    if (tmpMap.MapGridX + 1 <= Database.MapGrids[tmpMap.MapGrid].Width)
                    {
                        tmpMap.Right = Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX + 1, tmpMap.MapGridY];
                    }

                    tmpMap.Save();
                    Database.GenerateMapGrids();
                    PacketSender.SendMap(client, newMap);
                    PacketSender.SendEnterMap(client, newMap);
                }
            }
            bf.Dispose();
        }

        private static void HandleEnterMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            //TODO See if the player is close enough to be switching chunks.
            PacketSender.SendEnterMap(client, (int)bf.ReadLong());
            bf.Dispose();
        }

        private static void HandleTryAttack(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.Entities[client.entityIndex].TryAttack((int)bf.ReadLong());
            bf.Dispose();
        }

        private static void HandleDir(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.Entities[client.entityIndex].ChangeDir((int)bf.ReadLong());
            bf.Dispose();
        }

        private static void HandleEnterGame(Client client, byte[] packet)
        {
            var index = client.entityIndex;
            ((Player)Globals.Entities[client.entityIndex]).InGame = true;
            PacketSender.SendGameData(client);
            PacketSender.SendGameTime(client);
            PacketSender.SendPlayerMsg(client, "Welcome to the Intersect game server.");
            PacketSender.SendGlobalMsg(Globals.Entities[index].MyName + " has joined the Intersect engine");
            PacketSender.SendTilesets(client);
            for (var i = 0; i < Globals.Entities.Count; i++)
            {
                if (Globals.Entities[i] != null)
                {
                    PacketSender.SendEntityData(client, i, 0, Globals.Entities[i]);
                }
            }
            for (var i = 0; i < Globals.Clients.Count; i++)
            {
                if (i == client.clientIndex) continue;
                if (Globals.Clients[i] == null) continue;
                if (!Globals.Clients[i].isConnected) continue;
                if (!Globals.Clients[i].isEditor)
                {
                    PacketSender.SendEntityData(Globals.Clients[i], client.entityIndex,0,Globals.Entities[client.entityIndex]);
                }
            }
            Globals.Entities[index].Warp(Globals.Entities[index].CurrentMap, Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY, Globals.Entities[index].Dir);
            
        }

        private static void HandleActivateEvent(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            ((Player)(Globals.Entities[client.entityIndex])).TryActivateEvent(bf.ReadInteger());
            bf.Dispose();
        }

        private static void HandleEventResponse(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            ((Player)(Globals.Entities[client.entityIndex])).RespondToEvent(bf.ReadInteger(), bf.ReadInteger());
            bf.Dispose();
        }

        private static void HandleCreateAccount(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var username = bf.ReadString();
            var password = bf.ReadString();
            var email = bf.ReadString();
            var index = client.entityIndex;
            if (Database.AccountExists(username))
            {
                PacketSender.SendLoginError(client, "Account already exists!");
            }
            else
            {
                if (Database.EmailInUse(email))
                {
                    PacketSender.SendLoginError(client, "An account with this email address already exists.");
                }
                else
                {
                    Database.CreateAccount(username, password, email);
                    client.id = Database.GetUserId(username);
                    Globals.Entities[index].MyName = username;
                    Console.WriteLine(Globals.Entities[index].MyName + " logged in.");
                    Globals.Entities[index].MySprite = "5";
                    Globals.Entities[index].CurrentMap = Constants.SpawnMap;
                    Globals.Entities[index].CurrentX = Constants.SpawnX;
                    Globals.Entities[index].CurrentY = Constants.SpawnY;
                    Database.SavePlayer(client);
                    PacketSender.SendJoinGame(client);
                }
            }
            bf.Dispose();
        }

        private static void HandleItemData(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var itemNum = bf.ReadLong();
            Globals.GameItems[itemNum].LoadItem(bf);
            Globals.GameItems[itemNum].Save((int)itemNum);
            bf.Dispose();
        }

        private static void HandleItemEditor(Client client)
        {
            for (var i = 0; i < Constants.MaxItems; i++)
            {
                PacketSender.SendItem(client, i);
            }
            PacketSender.SendItemEditor(client);
        }
    }
}

