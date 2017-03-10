

using Intersect_Editor.Classes.Maps;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Maps;
using Intersect_Library.GameObjects.Maps.MapList;

namespace Intersect_Editor.Classes
{
    public static class PacketSender
    {

        public static void SendPing()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.Ping);
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendLogin(string username, string password)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.EditorLogin);
            bf.WriteString(username);
            bf.WriteString(password);
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNeedMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.NeedMap);
            bf.WriteLong(mapNum);
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMap(MapBase map)
        {
            var bf = new ByteBuffer();
            var mapData = map.GetMapData(false);
            bf.WriteLong((int)ClientPackets.SaveMap);
            bf.WriteLong(map.GetId());
            bf.WriteLong(mapData.Length);
            bf.WriteBytes(mapData);
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCreateMap(int location, int currentMap, MapListItem parent)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.CreateMap);
            bf.WriteInteger(location);
            if (location > -1)
            {
                bf.WriteLong(currentMap);
            }
            else
            {
                if (parent == null)
                {
                    bf.WriteInteger(-1);
                    bf.WriteInteger(-1);
                }
                else
                {
                    if (parent.GetType() == typeof(MapListMap))
                    {
                        bf.WriteInteger(1);
                        bf.WriteInteger(((MapListMap)parent).MapNum);
                    }
                    else
                    {
                        bf.WriteInteger(0);
                        bf.WriteInteger(((MapListFolder)parent).FolderId);
                    }
                }
            }
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapListMove(int srcType, int srcId, int destType, int destId)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.MapListUpdate);
            bf.WriteInteger((int)MapListUpdates.MoveItem);
            bf.WriteInteger(srcType);
            bf.WriteInteger(srcId);
            bf.WriteInteger(destType);
            bf.WriteInteger(destId);
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendAddFolder(MapListItem parent)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.MapListUpdate);
            bf.WriteInteger((int)MapListUpdates.AddFolder);
            if (parent == null)
            {
                bf.WriteInteger(-1);
                bf.WriteInteger(-1);
            }
            else
            {
                if (parent.GetType() == typeof(MapListMap))
                {
                    bf.WriteInteger(1);
                    bf.WriteInteger(((MapListMap)parent).MapNum);
                }
                else
                {
                    bf.WriteInteger(0);
                    bf.WriteInteger(((MapListFolder)parent).FolderId);
                }
            }
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendRename(MapListItem parent, string name)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.MapListUpdate);
            bf.WriteInteger((int)MapListUpdates.Rename);
            if (parent.GetType() == typeof(MapListMap))
            {
                bf.WriteInteger(1);
                bf.WriteInteger(((MapListMap)parent).MapNum);
            }
            else
            {
                bf.WriteInteger(0);
                bf.WriteInteger(((MapListFolder)parent).FolderId);
            }
            bf.WriteString(name);
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendDelete(MapListItem target)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.MapListUpdate);
            bf.WriteInteger((int)MapListUpdates.Delete);
            if (target.GetType() == typeof(MapListMap))
            {
                bf.WriteInteger(1);
                bf.WriteInteger(((MapListMap)target).MapNum);
            }
            else
            {
                bf.WriteInteger(0);
                bf.WriteInteger(((MapListFolder)target).FolderId);
            }
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNeedGrid(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.NeedGrid);
            bf.WriteLong(mapNum);
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendUnlinkMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.UnlinkMap);
            bf.WriteLong(mapNum);
            bf.WriteLong(Globals.CurrentMap.GetId());
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendLinkMap(int adjacentMap, int linkMap, int gridX, int gridY)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.LinkMap);
            bf.WriteLong(adjacentMap);
            bf.WriteLong(linkMap);
            bf.WriteLong(gridX);
            bf.WriteLong(gridY);
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCreateObject(GameObject type, string value = "")
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.NewGameObject);
            bf.WriteInteger((int) type);
            bf.WriteString(value);
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenEditor(GameObject type)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.OpenObjectEditor);
            bf.WriteInteger((int)type);
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendDeleteObject(DatabaseObject obj)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.DeleteGameObject);
            bf.WriteInteger((int)obj.GetGameObjectType());
            bf.WriteInteger(obj.GetId());
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendSaveObject(DatabaseObject obj)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.SaveGameObject);
            bf.WriteInteger((int)obj.GetGameObjectType());
            bf.WriteInteger(obj.GetId());
            bf.WriteBytes(obj.GetData());
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendSaveTime(byte[] data)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.SaveTime);
            bf.WriteBytes(data);
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNewTilesets(string[] tilesets)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.AddTilesets);
            bf.WriteInteger(tilesets.Length);
            for (int i = 0; i < tilesets.Length; i++)
            {
                bf.WriteString(tilesets[i]);
            }
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEnterMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.EnterMap);
            bf.WriteInteger(mapNum);
            Network.SendPacket(bf.ToArray());
            bf.Dispose();
        }
    }
}
