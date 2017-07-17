using Intersect.Editor.Classes.Maps;
using Intersect.Enums;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Models;

namespace Intersect.Editor.Classes
{
    public static class PacketSender
    {
        public static void SendPing()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.Ping);
            LegacyEditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendLogin(string username, string password)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.EditorLogin);
            bf.WriteString(username);
            bf.WriteString(password);
            LegacyEditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNeedMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.NeedMap);
            bf.WriteLong(mapNum);
            LegacyEditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMap(MapInstance map)
        {
            var bf = new ByteBuffer();
            var mapData = map.GetMapData(false);
            bf.WriteLong((int) ClientPackets.SaveMap);
            bf.WriteInteger(map.Index);
            bf.WriteInteger(mapData.Length);
            bf.WriteBytes(mapData);
            var tileData = map.GenerateTileData();
            bf.WriteInteger(tileData.Length);
            bf.WriteBytes(tileData);
            LegacyEditorNetwork.SendPacket(bf.ToArray());
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
            LegacyEditorNetwork.SendPacket(bf.ToArray());
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
            LegacyEditorNetwork.SendPacket(bf.ToArray());
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
            LegacyEditorNetwork.SendPacket(bf.ToArray());
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
            LegacyEditorNetwork.SendPacket(bf.ToArray());
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
            LegacyEditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNeedGrid(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.NeedGrid);
            bf.WriteLong(mapNum);
            LegacyEditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendUnlinkMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.UnlinkMap);
            bf.WriteLong(mapNum);
            bf.WriteLong(Globals.CurrentMap.Index);
            LegacyEditorNetwork.SendPacket(bf.ToArray());
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
            LegacyEditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCreateObject(GameObjectType type, string value = "")
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.NewGameObject);
            bf.WriteInteger((int) type);
            bf.WriteString(value);
            LegacyEditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenEditor(GameObjectType type)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.OpenObjectEditor);
            bf.WriteInteger((int) type);
            LegacyEditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendDeleteObject(IDatabaseObject obj)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.DeleteGameObject);
            bf.WriteInteger((int) obj.Type);
            bf.WriteInteger(obj.Index);
            LegacyEditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendSaveObject(IDatabaseObject obj)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.SaveGameObject);
            bf.WriteInteger((int) obj.Type);
            bf.WriteInteger(obj.Index);
            bf.WriteBytes(obj.BinaryData);
            LegacyEditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendSaveTime(byte[] data)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.SaveTime);
            bf.WriteBytes(data);
            LegacyEditorNetwork.SendPacket(bf.ToArray());
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
            LegacyEditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEnterMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.EnterMap);
            bf.WriteInteger(mapNum);
            LegacyEditorNetwork.SendPacket(bf.ToArray());
            bf.Dispose();
        }
    }
}