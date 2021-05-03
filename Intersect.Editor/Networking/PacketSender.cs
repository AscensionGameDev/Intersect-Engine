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

    public static partial class PacketSender
    {

        public static void SendPing()
        {
            Network.SendPacket(new PingPacket());
        }

        public static void SendLogin(string username, string password)
        {
            Network.SendPacket(new LoginPacket(username, password));
        }

        public static void SendNeedMap(Guid mapId)
        {
            Network.SendPacket(new NeedMapPacket(mapId));
        }

        public static void SendMap(MapInstance map)
        {
            Network.SendPacket(new MapUpdatePacket(map.Id, map.JsonData, map.GenerateTileData(), map.AttributeData));
        }

        public static void SendCreateMap(int location, Guid currentMapId, MapListItem parent)
        {
            if (location > -1)
            {
                Network.SendPacket(new CreateMapPacket(currentMapId, (byte) location));
            }
            else
            {
                if (parent == null)
                {
                    Network.SendPacket(new CreateMapPacket(0, Guid.Empty));
                }
                else
                {
                    if (parent.GetType() == typeof(MapListMap))
                    {
                        Network.SendPacket(new CreateMapPacket(1, ((MapListMap) parent).MapId));
                    }
                    else
                    {
                        Network.SendPacket(new CreateMapPacket(0, ((MapListFolder) parent).FolderId));
                    }
                }
            }
        }

        public static void SendMapListMove(int srcType, Guid srcId, int destType, Guid destId)
        {
            Network.SendPacket(new MapListUpdatePacket(MapListUpdates.MoveItem, srcType, srcId, destType, destId, ""));
        }

        public static void SendAddFolder(MapListItem parent)
        {
            if (parent == null)
            {
                Network.SendPacket(new MapListUpdatePacket(MapListUpdates.AddFolder, 0, Guid.Empty, 0, Guid.Empty, ""));
            }
            else
            {
                if (parent.GetType() == typeof(MapListMap))
                {
                    Network.SendPacket(
                        new MapListUpdatePacket(
                            MapListUpdates.AddFolder, 0, Guid.Empty, 1, ((MapListMap) parent).MapId, ""
                        )
                    );
                }
                else
                {
                    Network.SendPacket(
                        new MapListUpdatePacket(
                            MapListUpdates.AddFolder, 0, Guid.Empty, 0, ((MapListFolder) parent).FolderId, ""
                        )
                    );
                }
            }
        }

        public static void SendRename(MapListItem parent, string name)
        {
            if (parent.GetType() == typeof(MapListMap))
            {
                Network.SendPacket(
                    new MapListUpdatePacket(MapListUpdates.Rename, 1, ((MapListMap) parent).MapId, 0, Guid.Empty, name)
                );
            }
            else
            {
                Network.SendPacket(
                    new MapListUpdatePacket(
                        MapListUpdates.Rename, 0, ((MapListFolder) parent).FolderId, 0, Guid.Empty, name
                    )
                );
            }
        }

        public static void SendDelete(MapListItem target)
        {
            if (target.GetType() == typeof(MapListMap))
            {
                Network.SendPacket(
                    new MapListUpdatePacket(MapListUpdates.Delete, 1, ((MapListMap) target).MapId, 0, Guid.Empty, "")
                );
            }
            else
            {
                Network.SendPacket(
                    new MapListUpdatePacket(
                        MapListUpdates.Delete, 0, ((MapListFolder) target).FolderId, 0, Guid.Empty, ""
                    )
                );
            }
        }

        public static void SendNeedGrid(Guid mapId)
        {
            Network.SendPacket(new RequestGridPacket(mapId));
        }

        public static void SendUnlinkMap(Guid mapId)
        {
            Network.SendPacket(new UnlinkMapPacket(mapId, Globals.CurrentMap.Id));
        }

        public static void SendLinkMap(Guid adjacentMapId, Guid linkMapId, int gridX, int gridY)
        {
            Network.SendPacket(new LinkMapPacket(linkMapId, adjacentMapId, gridX, gridY));
        }

        public static void SendCreateObject(GameObjectType type)
        {
            Network.SendPacket(new CreateGameObjectPacket(type));
        }

        public static void SendOpenEditor(GameObjectType type)
        {
            if (Globals.CurrentEditor != -1)
            {
                return;
            }

            Network.SendPacket(new RequestOpenEditorPacket(type));
        }

        public static void SendDeleteObject(IDatabaseObject obj)
        {
            Network.SendPacket(new DeleteGameObjectPacket(obj.Type, obj.Id));
        }

        public static void SendSaveObject(IDatabaseObject obj)
        {
            Network.SendPacket(new SaveGameObjectPacket(obj.Type, obj.Id, obj.JsonData));
        }

        public static void SendSaveTime(string timeJson)
        {
            Network.SendPacket(new SaveTimeDataPacket(timeJson));
        }

        public static void SendNewTilesets(string[] tilesets)
        {
            Network.SendPacket(new AddTilesetsPacket(tilesets));
        }

        public static void SendEnterMap(Guid mapId)
        {
            Network.SendPacket(new EnterMapPacket(mapId));
        }

    }

}
