using System;
using Intersect.Editor.General;
using Intersect.Editor.Maps;
using Intersect.Enums;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Models;
using Intersect.Network.Packets.Editor;

using MapListUpdates = Intersect.Enums.MapListUpdates;

namespace Intersect.Editor.Networking
{
    public static class PacketSender
    {
        public static void SendPing()
        {
            EditorNetwork.SendPacket(new PingPacket());
        }

        public static void SendLogin(string username, string password)
        {
            EditorNetwork.SendPacket(new LoginPacket(username,password));
        }

        public static void SendNeedMap(Guid mapId)
        {
            EditorNetwork.SendPacket(new NeedMapPacket(mapId));
        }

        public static void SendMap(MapInstance map)
        {
            EditorNetwork.SendPacket(new MapUpdatePacket(map.Id,map.JsonData,map.GenerateTileData(),map.AttributeData));
        }

        public static void SendCreateMap(int location, Guid currentMapId, MapListItem parent)
        {
            if (location > -1)
            {
                EditorNetwork.SendPacket(new CreateMapPacket(currentMapId, (byte) location));
            }
            else
            {
                if (parent == null)
                {
                    EditorNetwork.SendPacket(new CreateMapPacket(0, Guid.Empty));
                }
                else
                {
                    if (parent.GetType() == typeof(MapListMap))
                    {
                        EditorNetwork.SendPacket(new CreateMapPacket(1, ((MapListMap)parent).MapId));
                    }
                    else
                    {
                        EditorNetwork.SendPacket(new CreateMapPacket(0, ((MapListFolder)parent).FolderId));
                    }
                }
            }
        }

        public static void SendMapListMove(int srcType, Guid srcId, int destType, Guid destId)
        {
            EditorNetwork.SendPacket(new MapListUpdatePacket(MapListUpdates.MoveItem,srcType,srcId,destType,destId,""));
        }

        public static void SendAddFolder(MapListItem parent)
        {
            if (parent == null)
            {
                EditorNetwork.SendPacket(new MapListUpdatePacket(MapListUpdates.AddFolder, 0, Guid.Empty, 0, Guid.Empty, ""));
            }
            else
            {
                if (parent.GetType() == typeof(MapListMap))
                {
                    EditorNetwork.SendPacket(new MapListUpdatePacket(MapListUpdates.AddFolder, 0, Guid.Empty, 1, ((MapListMap)parent).MapId, ""));
                }
                else
                {
                    EditorNetwork.SendPacket(new MapListUpdatePacket(MapListUpdates.AddFolder, 0, Guid.Empty, 0, ((MapListFolder)parent).FolderId, ""));
                }
            }
        }

        public static void SendRename(MapListItem parent, string name)
        {
            if (parent.GetType() == typeof(MapListMap))
            {
                EditorNetwork.SendPacket(new MapListUpdatePacket(MapListUpdates.Rename, 1, ((MapListMap)parent).MapId, 0, Guid.Empty, name));
            }
            else
            {
                EditorNetwork.SendPacket(new MapListUpdatePacket(MapListUpdates.Rename, 0, ((MapListFolder)parent).FolderId, 0, Guid.Empty, name));
            }
        }

        public static void SendDelete(MapListItem target)
        {
            if (target.GetType() == typeof(MapListMap))
            {
                EditorNetwork.SendPacket(new MapListUpdatePacket(MapListUpdates.Delete, 1, ((MapListMap) target).MapId, 0, Guid.Empty, ""));
            }
            else
            {
                EditorNetwork.SendPacket(new MapListUpdatePacket(MapListUpdates.Delete, 0, ((MapListFolder)target).FolderId, 0, Guid.Empty, ""));
            }
        }

        public static void SendNeedGrid(Guid mapId)
        {
            EditorNetwork.SendPacket(new RequestGridPacket(mapId));
        }

        public static void SendUnlinkMap(Guid mapId)
        {
            EditorNetwork.SendPacket(new UnlinkMapPacket(mapId,Globals.CurrentMap.Id));
        }

        public static void SendLinkMap(Guid adjacentMapId, Guid linkMapId, int gridX, int gridY)
        {
            EditorNetwork.SendPacket(new LinkMapPacket(linkMapId,adjacentMapId,gridX,gridY));
        }

        public static void SendCreateObject(GameObjectType type)
        {
            EditorNetwork.SendPacket(new CreateGameObjectPacket(type));
        }

        public static void SendOpenEditor(GameObjectType type)
        {
            EditorNetwork.SendPacket(new RequestOpenEditorPacket(type));
        }

        public static void SendDeleteObject(IDatabaseObject obj)
        {
            EditorNetwork.SendPacket(new DeleteGameObjectPacket(obj.Type,obj.Id));
        }

        public static void SendSaveObject(IDatabaseObject obj)
        {
            EditorNetwork.SendPacket(new SaveGameObjectPacket(obj.Type,obj.Id,obj.JsonData));
        }

        public static void SendSaveTime(byte[] data)
        {
            EditorNetwork.SendPacket(new SaveTimeDataPacket(data));
        }

        public static void SendNewTilesets(string[] tilesets)
        {
            EditorNetwork.SendPacket(new AddTilesetsPacket(tilesets));
        }

        public static void SendEnterMap(Guid mapId)
        {
            EditorNetwork.SendPacket(new EnterMapPacket(mapId));
        }
    }
}