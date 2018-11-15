using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Collections;
using Newtonsoft.Json;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Maps.MapList
{
    public class MapList
    {
        //So EF will save this :P
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; protected set; }
        private static MapList sMapList = new MapList();
        private static List<MapListMap> sOrderedMaps = new List<MapListMap>();

        [NotMapped]
        public List<MapListItem> Items { get; set; } = new List<MapListItem>();

        [JsonIgnore]
        [Column("JsonData")]
        public string JsonData
        {
            get => JsonConvert.SerializeObject(this, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, ObjectCreationHandling = ObjectCreationHandling.Replace }).Replace("Intersect Migration Tool", "Intersect Core").Replace("Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib", "Intersect");
            set => JsonConvert.PopulateObject(value, this, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, ObjectCreationHandling = ObjectCreationHandling.Replace });
        }

        public void PostLoad(DatabaseObjectLookup gameMaps, bool isServer = true, bool isTopLevel = false)
        {
            if (isTopLevel) sOrderedMaps.Clear();
            foreach (var itm in Items.ToArray())
            {
                if (itm.Type == 0)
                {
                    var dirItm = (MapListFolder)itm;
                    dirItm.PostLoad(gameMaps, isServer);
                }
                else
                {
                    var mapItm = (MapListMap)itm;
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
                        sOrderedMaps.Add(mapItm);
                    }
                }
            }
            if (isTopLevel)
                sOrderedMaps.Sort();
        }

        public static void SetList(MapList list)
        {
            sMapList = list;
        }
        public static MapList GetList()
        {
            return sMapList;
        }

        public static List<MapListMap> GetOrderedMaps()
        {
            return sOrderedMaps;
        }

        public void AddMap(Guid mapId, long timeCreated, DatabaseObjectLookup gameMaps)
        {
            if (!gameMaps.Keys.Contains(mapId)) return;
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
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Type == 0)
                {
                    if (((MapListFolder)Items[i]).FolderId == folderId)
                    {
                        return ((MapListFolder)Items[i]);
                    }
                    if (((MapListFolder)Items[i]).Children.FindDir(folderId) != null)
                    {
                        return ((MapListFolder)Items[i]).Children.FindDir(folderId);
                    }
                }
            }
            return null;
        }

        public MapListMap FindMap(Guid mapId)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Type == 0)
                {
                    if (((MapListFolder)Items[i]).Children.FindMap(mapId) != null)
                    {
                        return ((MapListFolder)Items[i]).Children.FindMap(mapId);
                    }
                }
                else
                {
                    if (((MapListMap)Items[i]).MapId == mapId)
                    {
                        return ((MapListMap)Items[i]);
                    }
                }
            }
            return null;
        }

        public MapListFolder FindMapParent(Guid mapId, MapListFolder parent)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].GetType() == typeof(MapListFolder))
                {
                    if (((MapListFolder)Items[i]).Children.FindMapParent(mapId, (MapListFolder)Items[i]) != null)
                    {
                        return ((MapListFolder)Items[i]).Children.FindMapParent(mapId, (MapListFolder)Items[i]);
                    }
                }
                else
                {
                    if (((MapListMap)Items[i]).MapId == mapId)
                    {
                        return parent;
                    }
                }
            }
            return null;
        }

        public MapListFolder FindFolderParent(Guid folderId, MapListFolder parent)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].GetType() == typeof(MapListFolder))
                {
                    if (((MapListFolder)Items[i]).FolderId == folderId)
                    {
                        return parent;
                    }
                    if (((MapListFolder)Items[i]).Children.FindFolderParent(folderId, (MapListFolder)Items[i]) !=
                        null)
                    {
                        return ((MapListFolder)Items[i]).Children.FindFolderParent(folderId, (MapListFolder)Items[i]);
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
                    targetList = sMapList;
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
                    targetList = sMapList;
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
                    sourceList = sMapList;
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
                    sourceList = sMapList;
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
                    ((MapListFolder)dest).Children.Items.Add(source);
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
            MapListFolder parent = FindFolderParent(folderId, null);
            MapListFolder self = FindDir(folderId);
            if (parent == null)
            {
                sMapList.Items.AddRange(self.Children.Items);
                sMapList.Items.Remove(self);
            }
            else
            {
                parent.Children.Items.AddRange(self.Children.Items);
                parent.Children.Items.Remove(self);
            }
        }

        public void DeleteMap(Guid mapid)
        {
            MapListFolder parent = FindMapParent(mapid, null);
            MapListMap self = FindMap(mapid);
            if (parent == null)
            {
                sMapList.Items.Remove(self);
            }
            else
            {
                parent.Children.Items.Remove(self);
            }
        }

        public Guid FindFirstMap()
        {
            Guid lowestMap = Guid.Empty;
            if (sOrderedMaps.Count > 0)
            {
                lowestMap = sOrderedMaps[0].MapId;
            }
            return lowestMap;
        }
    }
}