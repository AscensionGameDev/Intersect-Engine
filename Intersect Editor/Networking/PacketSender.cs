using Intersect.Editor.General;
using Intersect.Editor.Maps;
using Intersect.Enums;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Models;
using MapListUpdates = Intersect.Enums.MapListUpdates;

namespace Intersect.Editor.Networking
{
    public static class PacketSender
    {
        public static void SendPing()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.Ping);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendLogin(string username, string password)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.EditorLogin);
            bf.WriteString(username);
            bf.WriteString(password);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNeedMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.NeedMap);
            bf.WriteLong(mapNum);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMap(MapInstance map)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.SaveMap);
            bf.WriteInteger(map.Index);
            bf.WriteString(map.JsonData);
            var tileData = map.GenerateTileData();
            bf.WriteInteger(tileData.Length);
            bf.WriteBytes(tileData);
            var attributeData = map.AttributesData();
            bf.WriteInteger(attributeData.Length);
            bf.WriteBytes(attributeData);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCreateMap(int location, int currentMap, MapListItem parent)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.CreateMap);
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
                        bf.WriteInteger(((MapListMap) parent).MapNum);
                    }
                    else
                    {
                        bf.WriteInteger(0);
                        bf.WriteInteger(((MapListFolder) parent).FolderId);
                    }
                }
            }
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapListMove(int srcType, int srcId, int destType, int destId)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.MapListUpdate);
            bf.WriteInteger((int) MapListUpdates.MoveItem);
            bf.WriteInteger(srcType);
            bf.WriteInteger(srcId);
            bf.WriteInteger(destType);
            bf.WriteInteger(destId);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendAddFolder(MapListItem parent)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.MapListUpdate);
            bf.WriteInteger((int) MapListUpdates.AddFolder);
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
                    bf.WriteInteger(((MapListMap) parent).MapNum);
                }
                else
                {
                    bf.WriteInteger(0);
                    bf.WriteInteger(((MapListFolder) parent).FolderId);
                }
            }
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendRename(MapListItem parent, string name)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.MapListUpdate);
            bf.WriteInteger((int) MapListUpdates.Rename);
            if (parent.GetType() == typeof(MapListMap))
            {
                bf.WriteInteger(1);
                bf.WriteInteger(((MapListMap) parent).MapNum);
            }
            else
            {
                bf.WriteInteger(0);
                bf.WriteInteger(((MapListFolder) parent).FolderId);
            }
            bf.WriteString(name);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendDelete(MapListItem target)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.MapListUpdate);
            bf.WriteInteger((int) MapListUpdates.Delete);
            if (target.GetType() == typeof(MapListMap))
            {
                bf.WriteInteger(1);
                bf.WriteInteger(((MapListMap) target).MapNum);
            }
            else
            {
                bf.WriteInteger(0);
                bf.WriteInteger(((MapListFolder) target).FolderId);
            }
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNeedGrid(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.NeedGrid);
            bf.WriteLong(mapNum);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendUnlinkMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.UnlinkMap);
            bf.WriteLong(mapNum);
            bf.WriteLong(Globals.CurrentMap.Index);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendLinkMap(int adjacentMap, int linkMap, int gridX, int gridY)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.LinkMap);
            bf.WriteLong(adjacentMap);
            bf.WriteLong(linkMap);
            bf.WriteLong(gridX);
            bf.WriteLong(gridY);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCreateObject(GameObjectType type, string value = "")
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.NewGameObject);
            bf.WriteInteger((int) type);
            bf.WriteString(value);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenEditor(GameObjectType type)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.OpenObjectEditor);
            bf.WriteInteger((int) type);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendDeleteObject(IDatabaseObject obj)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.DeleteGameObject);
            bf.WriteInteger((int) obj.Type);
            bf.WriteInteger(obj.Index);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendSaveObject(IDatabaseObject obj)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.SaveGameObject);
            bf.WriteInteger((int) obj.Type);
            bf.WriteInteger(obj.Index);
            bf.WriteString(obj.JsonData);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendSaveTime(byte[] data)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.SaveTime);
            bf.WriteBytes(data);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNewTilesets(string[] tilesets)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.AddTilesets);
            bf.WriteInteger(tilesets.Length);
            for (int i = 0; i < tilesets.Length; i++)
            {
                bf.WriteString(tilesets[i]);
            }
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEnterMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.EnterMap);
            bf.WriteInteger(mapNum);
            EditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }
    }
}