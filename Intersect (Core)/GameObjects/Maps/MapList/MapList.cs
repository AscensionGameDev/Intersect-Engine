using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Collections;

using Newtonsoft.Json;

namespace Intersect.GameObjects.Maps.MapList
{

    public class MapList
    {

        //So EF will save this :P
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; protected set; }

        [NotMapped]
        public List<MapListItem> Items { get; set; } = new List<MapListItem>();

        public static MapList List { get; set; } = new MapList();

        public static List<MapListMap> OrderedMaps { get; } = new List<MapListMap>();

        [JsonIgnore]
        [Column("JsonData")]
        public string JsonData
        {
            get => JsonConvert.SerializeObject(
                this,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );
            set => JsonConvert.PopulateObject(
                value, this,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );
        }

        public void PostLoad(DatabaseObjectLookup gameMaps, bool isServer = true, bool isTopLevel = false)
        {
            if (isTopLevel)
            {
                OrderedMaps.Clear();
            }

            foreach (var itm in Items.ToArray())
            {
                if (itm.Type == 0)
                {
                    var dirItm = (MapListFolder) itm;
                    dirItm.PostLoad(gameMaps, isServer);
                }
                else
                {
                    var mapItm = (MapListMap) itm;
                    var removed = false;
                    if (isServer)
                    {
                        if (gameMaps.Get<MapBase>(mapItm.MapId) == null)
                        {
                            Items.Remove(itm);
                            removed = true;
                        }
                    }

                    if (!removed)
                    {
                        mapItm.PostLoad(gameMaps, isServer);
                        OrderedMaps.Add(mapItm);
                    }
                }
            }

            if (isTopLevel)
            {
                OrderedMaps.Sort();
            }
        }

        public void AddMap(Guid mapId, long timeCreated, DatabaseObjectLookup gameMaps)
        {
            if (!gameMaps.Keys.Contains(mapId))
            {
                return;
            }

            var tmp = new MapListMap()
            {
                Name = gameMaps[mapId].Name,
                MapId = mapId,
                TimeCreated = timeCreated
            };

            Items.Add(tmp);
        }

        public void AddFolder(string folderName)
        {
            var tmp = new MapListFolder()
            {
                Name = folderName,
                FolderId = Guid.NewGuid()
            };

            Items.Add(tmp);
        }

        public MapListFolder FindDir(Guid folderId)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].Type == 0)
                {
                    if (((MapListFolder) Items[i]).FolderId == folderId)
                    {
                        return (MapListFolder) Items[i];
                    }

                    if (((MapListFolder) Items[i]).Children.FindDir(folderId) != null)
                    {
                        return ((MapListFolder) Items[i]).Children.FindDir(folderId);
                    }
                }
            }

            return null;
        }

        public MapListMap FindMap(Guid mapId)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].Type == 0)
                {
                    if (((MapListFolder) Items[i]).Children.FindMap(mapId) != null)
                    {
                        return ((MapListFolder) Items[i]).Children.FindMap(mapId);
                    }
                }
                else
                {
                    if (((MapListMap) Items[i]).MapId == mapId)
                    {
                        return (MapListMap) Items[i];
                    }
                }
            }

            return null;
        }

        public MapListFolder FindMapParent(Guid mapId, MapListFolder parent)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].GetType() == typeof(MapListFolder))
                {
                    if (((MapListFolder) Items[i]).Children.FindMapParent(mapId, (MapListFolder) Items[i]) != null)
                    {
                        return ((MapListFolder) Items[i]).Children.FindMapParent(mapId, (MapListFolder) Items[i]);
                    }
                }
                else
                {
                    if (((MapListMap) Items[i]).MapId == mapId)
                    {
                        return parent;
                    }
                }
            }

            return null;
        }

        public MapListFolder FindFolderParent(Guid folderId, MapListFolder parent)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].GetType() == typeof(MapListFolder))
                {
                    if (((MapListFolder) Items[i]).FolderId == folderId)
                    {
                        return parent;
                    }

                    if (((MapListFolder) Items[i]).Children.FindFolderParent(folderId, (MapListFolder) Items[i]) !=
                        null)
                    {
                        return ((MapListFolder) Items[i]).Children.FindFolderParent(folderId, (MapListFolder) Items[i]);
                    }
                }
            }

            return null;
        }

        public void HandleMove(int srcType, Guid srcId, int destType, Guid destId)
        {
            MapListFolder sourceParent = null;
            MapListFolder destParent = null;
            MapList targetList = null;
            MapList sourceList = null;
            MapListItem source = null;
            MapListItem dest = null;
            if (destType == 0)
            {
                destParent = FindFolderParent(destId, null);
                if (destParent == null)
                {
                    targetList = List;
                }
                else
                {
                    targetList = destParent.Children;
                }

                dest = FindDir(destId);
            }
            else
            {
                destParent = FindMapParent(destId, null);
                if (destParent == null)
                {
                    targetList = List;
                }
                else
                {
                    targetList = destParent.Children;
                }

                dest = FindMap(destId);
            }

            if (srcType == 0)
            {
                sourceParent = FindFolderParent(srcId, null);
                if (sourceParent == null)
                {
                    sourceList = List;
                }
                else
                {
                    sourceList = sourceParent.Children;
                }

                source = FindDir(srcId);
            }
            else
            {
                sourceParent = FindMapParent(srcId, null);
                if (sourceParent == null)
                {
                    sourceList = List;
                }
                else
                {
                    sourceList = sourceParent.Children;
                }

                source = FindMap(srcId);
            }

            if (targetList != null && dest != null && sourceList != null && source != null)
            {
                if (destType == 0)
                {
                    ((MapListFolder) dest).Children.Items.Add(source);
                    sourceList.Items.Remove(source);
                }
                else
                {
                    sourceList.Items.Remove(source);
                    targetList.Items.Insert(targetList.Items.IndexOf(dest), source);
                }
            }
            else if (targetList != null && sourceList != null && source != null)
            {
                if (destType == -1)
                {
                    targetList.Items.Add(source);
                    sourceList.Items.Remove(source);
                }
            }

            //Save Map List
            //PacketSender.SendMapListToEditors();
        }

        public void DeleteFolder(Guid folderId)
        {
            var parent = FindFolderParent(folderId, null);
            var self = FindDir(folderId);
            if (parent == null)
            {
                List.Items.AddRange(self.Children.Items);
                List.Items.Remove(self);
            }
            else
            {
                parent.Children.Items.AddRange(self.Children.Items);
                parent.Children.Items.Remove(self);
            }
        }

        public void DeleteMap(Guid mapid)
        {
            var parent = FindMapParent(mapid, null);
            var self = FindMap(mapid);
            if (parent == null)
            {
                List.Items.Remove(self);
            }
            else
            {
                parent.Children.Items.Remove(self);
            }
        }

        public void UpdateMap(Guid mapId)
        {
            var map = FindMap(mapId);
            var mapInstance = MapBase.Get(mapId);
            if (map != null && mapInstance != null)
            {
                map.Name = mapInstance.Name;
                map.TimeCreated = mapInstance.TimeCreated;
            }
        }

        public Guid FindFirstMap()
        {
            var lowestMap = Guid.Empty;
            if (OrderedMaps.Count > 0)
            {
                lowestMap = OrderedMaps[0].MapId;
            }

            return lowestMap;
        }

    }

}
