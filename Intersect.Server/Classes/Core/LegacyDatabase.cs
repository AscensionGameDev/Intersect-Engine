using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Intersect.Collections;
using Intersect.Config;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Models;
using Intersect.Server.Database;
using Intersect.Server.Database.GameData;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Characters;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server
{
    public static class LegacyDatabase
    {
        private const string GameDbFilename = "resources/gamedata.db";
        private const string PlayersDbFilename = "resources/playerdata.db";

        private static PlayerContext sPlayerDb;
        private static GameContext sGameDb;

        private static Task sSaveGameDbTask;
        private static Task sSavePlayerDbTask;

        public static object MapGridLock = new object();
        public static List<MapGrid> MapGrids = new List<MapGrid>();

        private static object mSavingGameLock = new object();
        private static object mSavingPlayerLock = new object();

        //Check Directories
        public static void CheckDirectories()
        {
            if (!Directory.Exists("resources"))
                Directory.CreateDirectory("resources");
        }

        //As of now Database writes only occur on player saving & when editors make game changes
        //Database writes are actually pretty rare. And even player saves are offloaded as tasks so
        //if delayed it won't matter much.
        //TODO: Options for saving frequency and number of backups to keep.
        public static void BackupDatabase()
        {
            
        }

        //Database setup, version checking
        public static bool InitDatabase()
        {
            //Connect to new player database
            if (Options.PlayerDb.Type == DatabaseOptions.DatabaseType.sqlite)
            {
                sPlayerDb = new PlayerContext(DatabaseUtils.DbProvider.Sqlite, $"Data Source={PlayersDbFilename}");
            }
            else
            {
                sPlayerDb = new PlayerContext(DatabaseUtils.DbProvider.MySql, $"server={Options.PlayerDb.Server};port={Options.PlayerDb.Port};database={Options.PlayerDb.Database};user={Options.PlayerDb.Username};password={Options.PlayerDb.Password}");   
            }
            sPlayerDb.Database.Migrate();

            if (Options.GameDb.Type == DatabaseOptions.DatabaseType.sqlite)
            {
                sGameDb = new GameContext(DatabaseUtils.DbProvider.Sqlite, $"Data Source={GameDbFilename}");
            }
            else
            {
                sGameDb = new GameContext(DatabaseUtils.DbProvider.MySql, $"server={Options.GameDb.Server};port={Options.GameDb.Port};database={Options.GameDb.Database};user={Options.GameDb.Username};password={Options.GameDb.Password}");
            }

            sGameDb.Database.Migrate();

            LoadAllGameObjects();
            LoadTime();
            OnClassesLoaded();
            OnMapsLoaded();
            SaveGameDatabaseAsync();
            return true;
        }

        public static Client GetPlayerClient([NotNull] string username)
        {
            //Try to fetch a player entity by username, online or offline.
            //Check Online First
            lock (Globals.ClientLock)
            {
                foreach (var client in Globals.Clients)
                {
                    if (client.Entity != null && client.Name.ToLower() == username.ToLower())
                    {
                        return client;
                    }
                }
            }
            return null;
        }

        public static void SetPlayerPower([NotNull] string username, UserRights power)
        {
            var user = GetUser(username);
            if (user != null)
            {
                user.Power = power;
                SavePlayerDatabaseAsync();
            }
            else
            {
                Console.WriteLine(Strings.Account.doesnotexist);
            }
        }

        //User Info
        public static bool AccountExists([NotNull] string accountname)
        {
            return sPlayerDb.Users.Any(p => string.Equals(p.Name.Trim(), accountname.Trim(), StringComparison.CurrentCultureIgnoreCase));
        }

        public static User GetUser([NotNull] string username)
        {
            return User.GetUser(sPlayerDb, username);
        }

        public static Player GetUserCharacter(User user, Guid characterId)
        {
            foreach (var character in user.Characters)
            {
                if (character.Id == characterId) return character;
            }
            return null;
        }

        public static bool EmailInUse([NotNull]string email)
        {
            return sPlayerDb.Users.Any(p => string.Equals(p.Email.Trim(), email.Trim(), StringComparison.CurrentCultureIgnoreCase));
        }

        public static bool CharacterNameInUse([NotNull]string name)
        {
            return sPlayerDb.Characters.Any(p => string.Equals(p.Name.Trim(), name.Trim(), StringComparison.CurrentCultureIgnoreCase));
        }

        public static Guid? GetCharacterId([NotNull]string name)
        {
            return sPlayerDb.Characters.Where(p => string.Equals(p.Name.Trim(), name.Trim(), StringComparison.CurrentCultureIgnoreCase))?.First()?.Id;
        }

        public static Player GetCharacter(Guid id)
        {
            return User.GetCharacter(sPlayerDb, id);
        }

        public static Player GetCharacter(string name)
        {
            return User.GetCharacter(sPlayerDb, name);
        }

        public static long RegisteredPlayers => sPlayerDb.Users.Count();

        public static void CreateAccount([NotNull] Client client, [NotNull] string username, [NotNull] string password, [NotNull] string email)
        {
            var sha = new SHA256Managed();

            //Generate a Salt
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[20];
            rng.GetBytes(buff);
            var salt = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(Convert.ToBase64String(buff)))).Replace("-", "");

            //Hash the Password
            var pass = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(password + salt))).Replace("-", "");

            var rights = UserRights.None;
            if (RegisteredPlayers== 0)
            {
                rights = UserRights.Admin;
            }

            var user = new User()
            {
                Name = username,
                Email = email,
                Salt = salt,
                Password = pass,
                Power = rights,
            };
            sPlayerDb.Users.Add(user);
            client.SetUser(user);
            SavePlayerDatabaseAsync();
        }

        public static bool CheckPassword([NotNull] string username, [NotNull] string password)
        {
            var user = sPlayerDb.Users.Where(p => p.Name.ToLower() == username.ToLower()).Select(p => new { p.Password, p.Salt }).FirstOrDefault();
            if (user != null)
            {
                var sha = new SHA256Managed();
                var pass = user.Password;
                var salt = user.Salt;
                var temppass = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(password + salt))).Replace("-", "");
                if (temppass == pass)
                {
                    return true;
                }
            }
            return false;
        }

        public static UserRights CheckAccess([NotNull] string username)
        {
            var user = sPlayerDb.Users.Where(p => string.Equals(p.Name.Trim(), username.Trim(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (user != null) return user.Power;
            return UserRights.None;
        }

        public static bool LoadUser([NotNull] Client client, [NotNull] string username)
        {
            var user = GetUser(username);
            if (user != null)
            {
                client.SetUser(user);
                return true;
            }
            return false;
        }
        
        public static void DeleteCharacter(Guid characterId)
        {
            sPlayerDb.Characters.Remove(sPlayerDb.Characters.Find(characterId));
        }

        //Bags
        public static Bag GetBag(Item item)
        {
            if (item.BagId == null) return null;
            return Bag.GetBag(sPlayerDb,(Guid)item.BagId);
        }

        public static bool BagEmpty([NotNull] Bag bag)
        {
            for (var i = 0; i < bag.Slots.Count; i++)
            {
                if (bag.Slots[i] != null)
                {
                    var item = ItemBase.Get(bag.Slots[i].ItemId);
                    if (item != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
       

        //Game Object Saving/Loading
        private static void LoadAllGameObjects()
        {
            foreach (var value in Enum.GetValues(typeof(GameObjectType)))
            {
                Debug.Assert(value != null, "value != null");
                var type = (GameObjectType) value;
                if (type == GameObjectType.Time) continue;
                LoadGameObjects(type);
            }
        }

        private static void ClearGameObjects(GameObjectType type)
        {
            switch (type)
            {
                case GameObjectType.Animation:
                    AnimationBase.Lookup.Clear();
                    break;
                case GameObjectType.Class:
                    ClassBase.Lookup.Clear();
                    break;
                case GameObjectType.Item:
                    ItemBase.Lookup.Clear();
                    break;
                case GameObjectType.Npc:
                    NpcBase.Lookup.Clear();
                    break;
                case GameObjectType.Projectile:
                    ProjectileBase.Lookup.Clear();
                    break;
                case GameObjectType.Quest:
                    QuestBase.Lookup.Clear();
                    break;
                case GameObjectType.Resource:
                    ResourceBase.Lookup.Clear();
                    break;
                case GameObjectType.Shop:
                    ShopBase.Lookup.Clear();
                    break;
                case GameObjectType.Spell:
                    SpellBase.Lookup.Clear();
                    break;
                case GameObjectType.CraftTables:
                    CraftingTableBase.Lookup.Clear();
                    break;
                case GameObjectType.Crafts:
                    CraftBase.Lookup.Clear();
                    break;
                case GameObjectType.Map:
                    MapBase.Lookup.Clear();
                    break;
                case GameObjectType.Event:
                    EventBase.Lookup.Clear();
                    break;
                case GameObjectType.PlayerSwitch:
                    PlayerSwitchBase.Lookup.Clear();
                    break;
                case GameObjectType.PlayerVariable:
                    PlayerVariableBase.Lookup.Clear();
                    break;
                case GameObjectType.ServerSwitch:
                    ServerSwitchBase.Lookup.Clear();
                    break;
                case GameObjectType.ServerVariable:
                    ServerVariableBase.Lookup.Clear();
                    break;
                case GameObjectType.Tileset:
                    TilesetBase.Lookup.Clear();
                    break;
                case GameObjectType.Time:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static void LoadGameObjects(GameObjectType gameObjectType)
        {
            ClearGameObjects(gameObjectType);
            switch (gameObjectType)
            {
                case GameObjectType.Animation:
                    foreach (var anim in sGameDb.Animations)
                    {
                        AnimationBase.Lookup.Set(anim.Id, anim);
                    }
                    break;
                case GameObjectType.Class:
                    foreach (var cls in sGameDb.Classes)
                    {
                        ClassBase.Lookup.Set(cls.Id, cls);
                    }
                    break;
                case GameObjectType.Item:
                    foreach (var itm in sGameDb.Items)
                    {
                        ItemBase.Lookup.Set(itm.Id, itm);
                    }
                    break;
                case GameObjectType.Npc:
                    foreach (var npc in sGameDb.Npcs)
                    {
                        NpcBase.Lookup.Set(npc.Id, npc);
                    }
                    break;
                case GameObjectType.Projectile:
                    foreach (var proj in sGameDb.Projectiles)
                    {
                        ProjectileBase.Lookup.Set(proj.Id, proj);
                    }
                    break;
                case GameObjectType.Quest:
                    foreach (var qst in sGameDb.Quests)
                    {
                        QuestBase.Lookup.Set(qst.Id, qst);
                    }
                    break;
                case GameObjectType.Resource:
                    foreach (var res in sGameDb.Resources)
                    {
                        ResourceBase.Lookup.Set(res.Id, res);
                    }
                    break;
                case GameObjectType.Shop:
                    foreach (var shp in sGameDb.Shops)
                    {
                        ShopBase.Lookup.Set(shp.Id, shp);
                    }
                    break;
                case GameObjectType.Spell:
                    foreach (var spl in sGameDb.Spells)
                    {
                        SpellBase.Lookup.Set(spl.Id, spl);
                    }
                    break;
                case GameObjectType.CraftTables:
                    foreach (var craft in sGameDb.CraftingTables)
                    {
                        CraftingTableBase.Lookup.Set(craft.Id, craft);
                    }
                    break;
                case GameObjectType.Crafts:
                    foreach (var craft in sGameDb.Crafts)
                    {
                        CraftBase.Lookup.Set(craft.Id, craft);
                    }
                    break;
                case GameObjectType.Map:
                    var maps = sGameDb.Maps.ToArray();
                    foreach (var map in maps)
                    {
                        MapInstance.Lookup.Set(map.Id, map);
                    }
                    break;
                case GameObjectType.Event:
                    foreach (var evt in sGameDb.Events)
                    {
                        EventBase.Lookup.Set(evt.Id, evt);
                    }
                    break;
                case GameObjectType.PlayerSwitch:
                    foreach (var psw in sGameDb.PlayerSwitches)
                    {
                        PlayerSwitchBase.Lookup.Set(psw.Id, psw);
                    }
                    break;
                case GameObjectType.PlayerVariable:
                    foreach (var psw in sGameDb.PlayerVariables)
                    {
                        PlayerVariableBase.Lookup.Set(psw.Id, psw);
                    }
                    break;
                case GameObjectType.ServerSwitch:
                    foreach (var psw in sGameDb.ServerSwitches)
                    {
                        ServerSwitchBase.Lookup.Set(psw.Id, psw);
                    }
                    break;
                case GameObjectType.ServerVariable:
                    foreach (var psw in sGameDb.ServerVariables)
                    {
                        ServerVariableBase.Lookup.Set(psw.Id, psw);
                    }
                    break;
                case GameObjectType.Tileset:
                    foreach (var psw in sGameDb.Tilesets)
                    {
                        TilesetBase.Lookup.Set(psw.Id, psw);
                    }
                    break;
                case GameObjectType.Time:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
            }
        }

        public static IDatabaseObject AddGameObject(GameObjectType gameObjectType)
        {
            return AddGameObject(gameObjectType, Guid.Empty);
        }

        public static IDatabaseObject AddGameObject(GameObjectType gameObjectType, Guid predefinedid)
        {
            if (predefinedid == Guid.Empty) predefinedid = Guid.NewGuid();
            IDatabaseObject dbObj = null;
            switch (gameObjectType)
            {
                case GameObjectType.Animation: dbObj = new AnimationBase(predefinedid); break;
                case GameObjectType.Class: dbObj = new ClassBase(predefinedid); break;
                case GameObjectType.Item: dbObj = new ItemBase(predefinedid); break;
                case GameObjectType.Npc: dbObj = new NpcBase(predefinedid); break;
                case GameObjectType.Projectile: dbObj = new ProjectileBase(predefinedid); break;
                case GameObjectType.Resource: dbObj = new ResourceBase(predefinedid); break;
                case GameObjectType.Shop: dbObj = new ShopBase(predefinedid); break;
                case GameObjectType.Spell: dbObj = new SpellBase(predefinedid); break;
                case GameObjectType.CraftTables: dbObj = new CraftingTableBase(predefinedid); break;
                case GameObjectType.Crafts: dbObj = new CraftBase(predefinedid); break;
                case GameObjectType.Map: dbObj = new MapInstance(predefinedid); break;
                case GameObjectType.Event: dbObj = new EventBase(predefinedid); break;
                case GameObjectType.PlayerSwitch: dbObj = new PlayerSwitchBase(predefinedid); break;
                case GameObjectType.PlayerVariable: dbObj = new PlayerVariableBase(predefinedid); break;
                case GameObjectType.ServerSwitch: dbObj = new ServerSwitchBase(predefinedid); break;
                case GameObjectType.ServerVariable: dbObj = new ServerVariableBase(predefinedid); break;
                case GameObjectType.Tileset: dbObj = new TilesetBase(predefinedid); break;
                case GameObjectType.Time: break;

                case GameObjectType.Quest:
                    dbObj = new QuestBase(predefinedid);
                    ((QuestBase)dbObj).StartEvent = (EventBase)AddGameObject(GameObjectType.Event);
                    ((QuestBase)dbObj).EndEvent = (EventBase)AddGameObject(GameObjectType.Event);
                    ((QuestBase)dbObj).StartEvent.CommonEvent = false;
                    ((QuestBase)dbObj).EndEvent.CommonEvent = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
            }

            return dbObj == null ? null : AddGameObject(gameObjectType, dbObj);
        }

        public static IDatabaseObject AddGameObject(GameObjectType gameObjectType, [NotNull] IDatabaseObject dbObj)
        {
            lock (mSavingGameLock)
            {
                switch (gameObjectType)
                {
                    case GameObjectType.Animation:
                        sGameDb.Animations.Add((AnimationBase) dbObj);
                        AnimationBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.Class:
                        sGameDb.Classes.Add((ClassBase) dbObj);
                        ClassBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.Item:
                        sGameDb.Items.Add((ItemBase) dbObj);
                        ItemBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.Npc:
                        sGameDb.Npcs.Add((NpcBase) dbObj);
                        NpcBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.Projectile:
                        sGameDb.Projectiles.Add((ProjectileBase) dbObj);
                        ProjectileBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.Quest:
                        sGameDb.Quests.Add((QuestBase) dbObj);
                        QuestBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.Resource:
                        sGameDb.Resources.Add((ResourceBase) dbObj);
                        ResourceBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.Shop:
                        sGameDb.Shops.Add((ShopBase) dbObj);
                        ShopBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.Spell:
                        sGameDb.Spells.Add((SpellBase) dbObj);
                        SpellBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.CraftTables:
                        sGameDb.CraftingTables.Add((CraftingTableBase) dbObj);
                        CraftingTableBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.Crafts:
                        sGameDb.Crafts.Add((CraftBase) dbObj);
                        CraftBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.Map:
                        sGameDb.Maps.Add((MapInstance) dbObj);
                        MapInstance.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.Event:
                        sGameDb.Events.Add((EventBase) dbObj);
                        EventBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.PlayerSwitch:
                        sGameDb.PlayerSwitches.Add((PlayerSwitchBase) dbObj);
                        PlayerSwitchBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.PlayerVariable:
                        sGameDb.PlayerVariables.Add((PlayerVariableBase) dbObj);
                        PlayerVariableBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.ServerSwitch:
                        sGameDb.ServerSwitches.Add((ServerSwitchBase) dbObj);
                        ServerSwitchBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.ServerVariable:
                        sGameDb.ServerVariables.Add((ServerVariableBase) dbObj);
                        ServerVariableBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.Tileset:
                        sGameDb.Tilesets.Add((TilesetBase) dbObj);
                        TilesetBase.Lookup.Set(dbObj.Id, dbObj);
                        break;

                    case GameObjectType.Time:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
                }
            }

            if (Globals.ServerStarted)
            {
                SaveGameDatabaseAsync();
            }

            return dbObj;
        }

        public static void DeleteGameObject(IDatabaseObject gameObject)
        {
            lock (mSavingGameLock)
            {
                switch (gameObject.Type)
                {
                    case GameObjectType.Animation:
                        sGameDb.Animations.Remove((AnimationBase) gameObject);
                        break;
                    case GameObjectType.Class:
                        sGameDb.Classes.Remove((ClassBase) gameObject);
                        break;
                    case GameObjectType.Item:
                        sGameDb.Items.Remove((ItemBase) gameObject);
                        break;
                    case GameObjectType.Npc:
                        sGameDb.Npcs.Remove((NpcBase) gameObject);
                        break;
                    case GameObjectType.Projectile:
                        sGameDb.Projectiles.Remove((ProjectileBase) gameObject);
                        break;
                    case GameObjectType.Quest:
                        sGameDb.Events.Remove(((QuestBase) gameObject).StartEvent);
                        EventBase.Lookup.Delete(((QuestBase) gameObject).StartEvent);
                        sGameDb.Events.Remove(((QuestBase) gameObject).EndEvent);
                        EventBase.Lookup.Delete(((QuestBase) gameObject).EndEvent);
                        foreach (var tsk in ((QuestBase) gameObject).Tasks)
                        {
                            sGameDb.Events.Remove(tsk.CompletionEvent);
                            EventBase.Lookup.Delete(tsk.CompletionEvent);
                        }

                        sGameDb.Quests.Remove((QuestBase) gameObject);
                        break;
                    case GameObjectType.Resource:
                        sGameDb.Resources.Remove((ResourceBase) gameObject);
                        break;
                    case GameObjectType.Shop:
                        sGameDb.Shops.Remove((ShopBase) gameObject);
                        break;
                    case GameObjectType.Spell:
                        sGameDb.Spells.Remove((SpellBase) gameObject);
                        break;
                    case GameObjectType.CraftTables:
                        sGameDb.CraftingTables.Remove((CraftingTableBase) gameObject);
                        break;
                    case GameObjectType.Crafts:
                        sGameDb.Crafts.Remove((CraftBase) gameObject);
                        break;
                    case GameObjectType.Map:
                        sGameDb.Maps.Remove((MapInstance) gameObject);
                        MapInstance.Lookup.Delete(gameObject);
                        break;
                    case GameObjectType.Event:
                        sGameDb.Events.Remove((EventBase) gameObject);
                        break;
                    case GameObjectType.PlayerSwitch:
                        sGameDb.PlayerSwitches.Remove((PlayerSwitchBase) gameObject);
                        break;
                    case GameObjectType.PlayerVariable:
                        sGameDb.PlayerVariables.Remove((PlayerVariableBase) gameObject);
                        break;
                    case GameObjectType.ServerSwitch:
                        sGameDb.ServerSwitches.Remove((ServerSwitchBase) gameObject);
                        break;
                    case GameObjectType.ServerVariable:
                        sGameDb.ServerVariables.Remove((ServerVariableBase) gameObject);
                        break;
                    case GameObjectType.Tileset:
                        sGameDb.Tilesets.Remove((TilesetBase) gameObject);
                        break;
                    case GameObjectType.Time:
                        break;
                }

                if (gameObject.Type.GetLookup().Values.Contains(gameObject))
                {
                    if (!gameObject.Type.GetLookup().Delete(gameObject))
                    {
                        throw new Exception();
                    }
                }
            }
        }

        //Post Loading Functions
        private static void OnMapsLoaded()
        {
            if (MapBase.Lookup.Count == 0)
            {
                Console.WriteLine(Strings.Database.nomaps);
                AddGameObject(GameObjectType.Map);
            }

            GenerateMapGrids();
            LoadMapFolders();
            CheckAllMapConnections();

            foreach (var map in MapInstance.Lookup)
            {
                ((MapInstance)map.Value).Initialize();
            }
        }

        private static void OnClassesLoaded()
        {
            if (ClassBase.Lookup.Count == 0)
            {
                Console.WriteLine(Strings.Database.noclasses);
                var cls = (ClassBase) AddGameObject(GameObjectType.Class);
                cls.Name = Strings.Database.Default;
                var defaultMale = new ClassSprite()
                {
                    Sprite = "1.png",
                    Gender = Gender.Male
                };
                var defaultFemale = new ClassSprite()
                {
                    Sprite = "2.png",
                    Gender = Gender.Female
                };
                cls.Sprites.Add(defaultMale);
                cls.Sprites.Add(defaultFemale);
                for (var i = 0; i < (int) Vitals.VitalCount; i++)
                {
                    cls.BaseVital[i] = 20;
                }
                for (var i = 0; i < (int) Stats.StatCount; i++)
                {
                    cls.BaseStat[i] = 20;
                }
            }
        }

        //Extra Map Helper Functions
        public static void CheckAllMapConnections()
        {
            foreach (MapBase map in MapInstance.Lookup.Values)
            {
                CheckMapConnections(map, MapInstance.Lookup);
            }
        }

        public static void CheckMapConnections(MapBase map, DatabaseObjectLookup maps)
        {
            var updated = false;
            if (!maps.Keys.Contains(map.Up) && map.Up != Guid.Empty)
            {
                map.Up = Guid.Empty;
                updated = true;
            }
            if (!maps.Keys.Contains(map.Down) && map.Down != Guid.Empty)
            {
                map.Down = Guid.Empty;
                updated = true;
            }
            if (!maps.Keys.Contains(map.Left) && map.Left != Guid.Empty)
            {
                map.Left = Guid.Empty;
                updated = true;
            }
            if (!maps.Keys.Contains(map.Right) && map.Right != Guid.Empty)
            {
                map.Right = Guid.Empty;
                updated = true;
            }
            if (updated)
            {
                SaveGameDatabaseAsync();
                PacketSender.SendMapToEditors(map.Id);
            }
        }

        public static void GenerateMapGrids()
        {
            lock (MapGridLock)
            {
                MapGrids.Clear();
                foreach (var map in MapInstance.Lookup.Values)
                {
                    if (MapGrids.Count == 0)
                    {
                        MapGrids.Add(new MapGrid(map.Id, 0));
                    }
                    else
                    {
                        for (var y = 0; y < MapGrids.Count; y++)
                        {
                            if (!MapGrids[y].HasMap(map.Id))
                            {
                                if (y != MapGrids.Count - 1) continue;
                                MapGrids.Add(new MapGrid(map.Id, MapGrids.Count));
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                foreach (MapInstance map in MapInstance.Lookup.Values)
                {
                    map.SurroundingMaps.Clear();
                    var myGrid = map.MapGrid;
                    for (var x = map.MapGridX - 1; x <= map.MapGridX + 1; x++)
                    {
                        for (var y = map.MapGridY - 1; y <= map.MapGridY + 1; y++)
                        {
                            if ((x == map.MapGridX) && (y == map.MapGridY))
                                continue;
                            if (x >= MapGrids[myGrid].XMin && x < MapGrids[myGrid].XMax && y >= MapGrids[myGrid].YMin &&
                                y < MapGrids[myGrid].YMax && MapGrids[myGrid].MyGrid[x, y] != Guid.Empty)
                            {
                                map.SurroundingMaps.Add(MapGrids[myGrid].MyGrid[x, y]);
                            }
                        }
                    }
                }
                for (var i = 0; i < MapGrids.Count; i++)
                {
                    PacketSender.SendMapGridToAll(i);
                }
            }
        }

        //Map Folders
        private static void LoadMapFolders()
        {
            var mapFolders = sGameDb.MapFolders.FirstOrDefault();
            if (mapFolders == null)
            {
                sGameDb.MapFolders.Add(MapList.GetList());
            }
            else
            {
                MapList.SetList(mapFolders);
            }
            foreach (var map in MapBase.Lookup)
            {
                if (MapList.GetList().FindMap(map.Value.Id) == null)
                {
                    MapList.GetList().AddMap(map.Value.Id,map.Value.TimeCreated, MapBase.Lookup);
                }
            }
            MapList.GetList().PostLoad(MapBase.Lookup, true, true);
            PacketSender.SendMapListToAll();
        }

        //Time
        private static void LoadTime()
        {
            var time = sGameDb.Time.FirstOrDefault();
            if (time == null)
            {
                sGameDb.Time.Add(TimeBase.GetTimeBase());
            }
            else
            {
                TimeBase.SetStaticTime(time);
            }
            ServerTime.Init();
        }

        public static void SaveGameDatabaseAsync()
        {
            if ((sSaveGameDbTask == null) || !(sSaveGameDbTask.IsCompleted == false ||
                                              sSaveGameDbTask.Status == TaskStatus.Running ||
                                              sSaveGameDbTask.Status == TaskStatus.WaitingToRun ||
                                              sSaveGameDbTask.Status == TaskStatus.WaitingForActivation))
            {
                sSaveGameDbTask = Task.Factory.StartNew(SaveGameDb);
            }
        }

        public static void SavePlayerDatabaseAsync()
        {
            if ((sSavePlayerDbTask == null) || !(sSavePlayerDbTask.IsCompleted == false ||
                                                 sSavePlayerDbTask.Status == TaskStatus.Running ||
                                                 sSavePlayerDbTask.Status == TaskStatus.WaitingToRun ||
                                                 sSavePlayerDbTask.Status == TaskStatus.WaitingForActivation))
            {
                sSavePlayerDbTask = Task.Factory.StartNew(SavePlayerDb);
            }
        }

        public static void SaveGameDatabase()
        {
            SaveGameDb();
        }

        public static void SavePlayerDatabase()
        {
            SavePlayerDb();
        }

        private static void SaveGameDb()
        {
            if (sGameDb == null) return;
            lock (mSavingGameLock)
            {
                sGameDb.SaveChanges();
            }
        }

        private static void SavePlayerDb()
        {
            if (sPlayerDb == null) return;
            lock (mSavingPlayerLock)
            {
                sPlayerDb.SaveChanges();
            }
        }

        //Migration Code
        public static void Migrate(DatabaseOptions orig, DatabaseOptions.DatabaseType convType)
        {
            var gameDb = orig == Options.GameDb;
            PlayerContext newPlayerContext = null;
            GameContext newGameContext = null;
            var newOpts = new DatabaseOptions();
            newOpts.Type = DatabaseOptions.DatabaseType.sqlite;

            //MySql Creds
            var host = "";
            var user = "";
            var pass = "";
            var database = "";
            var port = 3306;
            var dbConnected = false;
            if (convType == DatabaseOptions.DatabaseType.mysql)
            {
                while (!dbConnected)
                {
                    Console.WriteLine(Strings.Migration.entermysqlinfo);
                    Console.Write(Strings.Migration.mysqlhost);
                    host = Console.ReadLine().Trim();
                    Console.Write(Strings.Migration.mysqlport);
                    var portinput = Console.ReadLine().Trim();
                    if (string.IsNullOrEmpty(portinput)) portinput = "3306";
                    port = int.Parse(portinput);
                    Console.Write(Strings.Migration.mysqldatabase);
                    database = Console.ReadLine().Trim();
                    Console.Write(Strings.Migration.mysqluser);
                    user = Console.ReadLine().Trim();
                    Console.Write(Strings.Migration.mysqlpass);
                    pass = GetPassword();

                    Console.WriteLine();
                    Console.WriteLine(Strings.Migration.mysqlconnecting);
                    var connString = $"server={host};port={port};database={database};user={user};password={pass}";
                    try
                    {
                        if (gameDb)
                        {
                            newGameContext = new GameContext(DatabaseUtils.DbProvider.MySql, connString);
                            if (newGameContext.IsEmpty())
                            {
                                newGameContext.Database.EnsureDeleted();
                                newGameContext.Database.Migrate();
                            }
                            else
                            {
                                Console.WriteLine(Strings.Migration.mysqlnotempty);
                                return;
                            }
                        }
                        else
                        {
                            newPlayerContext = new PlayerContext(DatabaseUtils.DbProvider.MySql, connString);
                            if (newPlayerContext.IsEmpty())
                            {
                                newPlayerContext.Database.EnsureDeleted();
                                newPlayerContext.Database.Migrate();
                            }
                            else
                            {
                                Console.WriteLine(Strings.Migration.mysqlnotempty);
                                return;
                            }
                        }

                        newOpts.Type = DatabaseOptions.DatabaseType.mysql;
                        newOpts.Server = host;
                        newOpts.Port = port;
                        newOpts.Database = database;
                        newOpts.Username = user;
                        newOpts.Password = pass;

                        dbConnected = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(Strings.Migration.mysqlconnectionerror.ToString(ex));
                        Console.WriteLine();
                        Console.WriteLine(Strings.Migration.mysqltryagain);
                        var key = Console.ReadKey().KeyChar;
                        Console.WriteLine();
                        if (key.ToString() != Strings.Migration.tryagaincharacter)
                        {
                            Console.WriteLine(Strings.Migration.migrationcancelled);
                            return;
                        }
                    }
                }
            }
            else if (convType == DatabaseOptions.DatabaseType.sqlite)
            {
                //If the file exists make sure it is safe to delete
                var dbExists = ((gameDb && File.Exists(GameDbFilename)) || (!gameDb && File.Exists(PlayersDbFilename)));
                if (dbExists)
                {
                    Console.WriteLine();
                    var filename = gameDb ? GameDbFilename : PlayersDbFilename;
                    Console.WriteLine(Strings.Migration.sqlitealreadyexists.ToString(filename));
                    var key = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                    if (key.ToString() != Strings.Migration.overwritecharacter)
                    {
                        Console.WriteLine(Strings.Migration.migrationcancelled);
                        return;
                    }
                }
                if (gameDb)
                {
                    newGameContext = new GameContext(DatabaseUtils.DbProvider.Sqlite, $"Data Source={GameDbFilename}");
                    newGameContext.Database.EnsureDeleted();
                    newGameContext.Database.Migrate();
                }
                else
                {
                    newPlayerContext = new PlayerContext(DatabaseUtils.DbProvider.Sqlite, $"Data Source={PlayersDbFilename}");
                    newPlayerContext.Database.EnsureDeleted();
                    newPlayerContext.Database.Migrate();
                }
            }

            //Shut down server, start migration.
            Console.WriteLine(Strings.Migration.stoppingserver);

            //This variable will end the server loop and save any pending changes
            Globals.ServerStarted = false;

            while (!Globals.ServerStopped)
            {
                System.Threading.Thread.Sleep(100);
            }

            lock (mSavingGameLock)
            {
                lock (mSavingPlayerLock)
                {
                    Console.WriteLine(Strings.Migration.startingmigration);
                    if (gameDb && newGameContext != null)
                    {
                        MigrateDbSet(sGameDb.Animations, newGameContext.Animations);
                        MigrateDbSet(sGameDb.Classes, newGameContext.Classes);
                        MigrateDbSet(sGameDb.CraftingTables, newGameContext.CraftingTables);
                        MigrateDbSet(sGameDb.Crafts, newGameContext.Crafts);
                        MigrateDbSet(sGameDb.Events, newGameContext.Events);
                        MigrateDbSet(sGameDb.Items, newGameContext.Items);
                        MigrateDbSet(sGameDb.MapFolders, newGameContext.MapFolders);
                        MigrateDbSet(sGameDb.Maps, newGameContext.Maps);
                        MigrateDbSet(sGameDb.Npcs, newGameContext.Npcs);
                        MigrateDbSet(sGameDb.Projectiles, newGameContext.Projectiles);
                        MigrateDbSet(sGameDb.Quests, newGameContext.Quests);
                        MigrateDbSet(sGameDb.Resources, newGameContext.Resources);
                        MigrateDbSet(sGameDb.Shops, newGameContext.Shops);
                        MigrateDbSet(sGameDb.Spells, newGameContext.Spells);
                        MigrateDbSet(sGameDb.ServerVariables, newGameContext.ServerVariables);
                        MigrateDbSet(sGameDb.ServerSwitches, newGameContext.ServerSwitches);
                        MigrateDbSet(sGameDb.PlayerSwitches, newGameContext.PlayerSwitches);
                        MigrateDbSet(sGameDb.PlayerVariables, newGameContext.PlayerVariables);
                        MigrateDbSet(sGameDb.Tilesets, newGameContext.Tilesets);
                        MigrateDbSet(sGameDb.Time, newGameContext.Time);
                        newGameContext.SaveChanges();
                        Options.GameDb = newOpts;
                        Options.SaveToDisk();
                    }
                    else if (!gameDb && newPlayerContext != null)
                    {
                        MigrateDbSet(sPlayerDb.Users, newPlayerContext.Users);
                        MigrateDbSet(sPlayerDb.Characters, newPlayerContext.Characters);
                        MigrateDbSet(sPlayerDb.Character_Friends, newPlayerContext.Character_Friends);
                        MigrateDbSet(sPlayerDb.Character_Spells, newPlayerContext.Character_Spells);
                        MigrateDbSet(sPlayerDb.Character_Switches, newPlayerContext.Character_Switches);
                        MigrateDbSet(sPlayerDb.Character_Variables, newPlayerContext.Character_Variables);
                        MigrateDbSet(sPlayerDb.Character_Hotbar, newPlayerContext.Character_Hotbar);
                        MigrateDbSet(sPlayerDb.Character_Quests, newPlayerContext.Character_Quests);
                        MigrateDbSet(sPlayerDb.Bags, newPlayerContext.Bags);
                        MigrateDbSet(sPlayerDb.Character_Items, newPlayerContext.Character_Items);
                        MigrateDbSet(sPlayerDb.Character_Bank, newPlayerContext.Character_Bank);
                        MigrateDbSet(sPlayerDb.Bag_Items, newPlayerContext.Bag_Items);
                        MigrateDbSet(sPlayerDb.Mutes, newPlayerContext.Mutes);
                        MigrateDbSet(sPlayerDb.Bans, newPlayerContext.Bans);
                        newPlayerContext.SaveChanges();
                        Options.PlayerDb = newOpts;
                        Options.SaveToDisk();
                    }
                }
            }
            Console.WriteLine(Strings.Migration.migrationcomplete);
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static void MigrateDbSet<T>(DbSet<T> oldDbSet, DbSet<T> newDbSet) where T : class
        {
            foreach (var itm in oldDbSet)
                newDbSet.Add(itm);
        }

        //Code taken from Stackoverflow on 9/20/2018
        //Answer by Dai and Damian Leszczyński - Vash
        //https://stackoverflow.com/questions/3404421/password-masking-console-application
        public static string GetPassword()
        {
            var pwd = "";
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.Remove(pwd.Length - 2, 1);
                        Console.Write("\b \b");
                    }
                }
                else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
                {
                    pwd = pwd + i.KeyChar;
                    Console.Write("*");
                }
            }
            return pwd;
        }
    }
}
