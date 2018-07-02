using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Intersect.Collections;
using Intersect.Config;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Server.Classes.Localization;
using Intersect.Logging;
using Intersect.Models;
using Intersect.Server.Classes.Database;
using Intersect.Server.Classes.Database.GameData;
using Intersect.Server.Classes.Database.PlayerData;
using Intersect.Server.Classes.Database.PlayerData.Characters;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;
using Intersect.Server.Database;
using Intersect.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Mono.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.Server.Classes.Core
{
    public static class LegacyDatabase
    {
        public const string DIRECTORY_BACKUPS = "resources/backups";
        private const string GameDbFilename = "resources/gamedata.db";
        private const string PlayersDbFilename = "resources/playerdata.db";

        private static PlayerContext sPlayerDb;

        private static GameContext sGameDb;

        public static object MapGridLock = new object();
        public static List<MapGrid> MapGrids = new List<MapGrid>();

        public static object SqlConnectionLock = new object();

        //Check Directories
        public static void CheckDirectories()
        {
            if (!Directory.Exists("resources"))
                Directory.CreateDirectory("resources");

            if (!Directory.Exists(DIRECTORY_BACKUPS))
                Directory.CreateDirectory(DIRECTORY_BACKUPS);

            if (!Directory.Exists(Path.Combine("resources", "languages")))
                Directory.CreateDirectory(Path.Combine("resources", "languages"));
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
                sPlayerDb = new PlayerContext(DatabaseUtils.DbProvider.MySql, $"server={Options.PlayerDb.Server};database={Options.PlayerDb.Database};user={Options.PlayerDb.Username};password={Options.PlayerDb.Password}");   
            }
            sPlayerDb.Database.Migrate();

            if (Options.GameDb.Type == DatabaseOptions.DatabaseType.sqlite)
            {
                sGameDb = new GameContext(DatabaseUtils.DbProvider.Sqlite, $"Data Source={GameDbFilename}");
            }
            else
            {
                throw new Exception("Learn to walk before you try to run there fella");
            }
            sGameDb.Database.Migrate();

            LoadAllGameObjects();
            LoadTime();
            return true;
        }

        public static void SavePlayers()
        {
            sPlayerDb.SaveChanges();
        }
        

        //Players General
        public static void LoadPlayerDatabase()
        {
            Console.WriteLine(Strings.Database.usingsqlite);
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

        public static void SetPlayerPower([NotNull] string username, int power)
        {
            var user = GetUser(username);
            if (user != null)
            {
                user.Access = power;
                sPlayerDb.SaveChanges();
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
            var salt =
                BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(Convert.ToBase64String(buff))))
                    .Replace("-", "");

            //Hash the Password
            var pass =
                BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(password + salt)))
                    .Replace("-", "");

            var access = 0;
            if (RegisteredPlayers== 0)
            {
                access = 2;
            }

            var user = new User()
            {
                Name = username,
                Email = email,
                Salt = salt,
                Password = pass,
                Access = access,
            };
            sPlayerDb.Users.Add(user);
            client.SetUser(user);
            sPlayerDb.SaveChanges();
        }

        public static bool CheckPassword([NotNull] string username, [NotNull] string password)
        {
            var user = GetUser(username);
            if (user != null)
            {
                var sha = new SHA256Managed();
                var pass = user.Password;
                var salt = user.Salt;
                var temppass =
                    BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(password + salt)))
                        .Replace("-", "");
                if (temppass == pass)
                {
                    return true;
                }
            }
            return false;
        }

        public static int CheckPower([NotNull] string username)
        {
            int power = 0;
            power = sPlayerDb.Users.Where(p => string.Equals(p.Name.Trim(), username.Trim(), StringComparison.CurrentCultureIgnoreCase)).Select(p => p.Access).First();
            return power;
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

        public static void DeleteCharacterFriend([NotNull] Player player, [NotNull] Player friend)
        {
            sPlayerDb.Character_Friends.Remove(sPlayerDb.Character_Friends.SingleOrDefault(p => p.Owner == player && p.Target == friend));
        }

        //Bags
        public static Bag CreateBag(int slotCount)
        {
            var bag = new Bag(slotCount);
            sPlayerDb.Bags.Add(bag);
            return bag;
        }

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
                    var item = ItemBase.Get(bag.Slots[i].Id);
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
                switch ((GameObjectType) value)
                {
                    case GameObjectType.Class:
                        OnClassesLoaded();
                        break;

                    case GameObjectType.Map:
                        OnMapsLoaded();
                        break;
                }
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
                        NpcBase.Lookup.Set(npc.Id, npc);
                    }
                    break;
                case GameObjectType.Projectile:
                    foreach (var proj in sGameDb.Projectiles)
                    {
                        ProjectileBase.Lookup.Set(proj.Id, proj);
                        ProjectileBase.Lookup.Set(proj.Id, proj);
                    }
                    break;
                case GameObjectType.Quest:
                    foreach (var qst in sGameDb.Quests)
                    {
                        QuestBase.Lookup.Set(qst.Id, qst);
                        QuestBase.Lookup.Set(qst.Id, qst);
                    }
                    break;
                case GameObjectType.Resource:
                    foreach (var res in sGameDb.Resources)
                    {
                        ResourceBase.Lookup.Set(res.Id, res);
                        ResourceBase.Lookup.Set(res.Id, res);
                    }
                    break;
                case GameObjectType.Shop:
                    foreach (var shp in sGameDb.Shops)
                    {
                        ShopBase.Lookup.Set(shp.Id, shp);
                        ShopBase.Lookup.Set(shp.Id, shp);
                    }
                    break;
                case GameObjectType.Spell:
                    foreach (var spl in sGameDb.Spells)
                    {
                        SpellBase.Lookup.Set(spl.Id, spl);
                        SpellBase.Lookup.Set(spl.Id, spl);
                    }
                    break;
                case GameObjectType.CraftTables:
                    foreach (var craft in sGameDb.CraftingTables)
                    {
                        CraftingTableBase.Lookup.Set(craft.Id, craft);
                        CraftingTableBase.Lookup.Set(craft.Id, craft);
                    }
                    break;
                case GameObjectType.Crafts:
                    foreach (var craft in sGameDb.Crafts)
                    {
                        CraftBase.Lookup.Set(craft.Id, craft);
                        CraftBase.Lookup.Set(craft.Id, craft);
                    }
                    break;
                case GameObjectType.Map:
                    foreach (var map in sGameDb.Maps)
                    {
                        MapInstance.Lookup.Set(map.Id, map);
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
                        PlayerSwitchBase.Lookup.Set(psw.Id, psw);
                    }
                    break;
                case GameObjectType.PlayerVariable:
                    foreach (var psw in sGameDb.PlayerVariables)
                    {
                        PlayerVariableBase.Lookup.Set(psw.Id, psw);
                        PlayerVariableBase.Lookup.Set(psw.Id, psw);
                    }
                    break;
                case GameObjectType.ServerSwitch:
                    foreach (var psw in sGameDb.ServerSwitches)
                    {
                        ServerSwitchBase.Lookup.Set(psw.Id, psw);
                        ServerSwitchBase.Lookup.Set(psw.Id, psw);
                    }
                    break;
                case GameObjectType.ServerVariable:
                    foreach (var psw in sGameDb.ServerVariables)
                    {
                        ServerVariableBase.Lookup.Set(psw.Id, psw);
                        ServerVariableBase.Lookup.Set(psw.Id, psw);
                    }
                    break;
                case GameObjectType.Tileset:
                    foreach (var psw in sGameDb.Tilesets)
                    {
                        ServerVariableBase.Lookup.Set(psw.Id, psw);
                        ServerVariableBase.Lookup.Set(psw.Id, psw);
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
            IDatabaseObject dbObj = null;
            switch (gameObjectType)
            {
                case GameObjectType.Animation:
                    dbObj = new AnimationBase();
                    sGameDb.Animations.Add((AnimationBase)dbObj);
                    AnimationBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.Class:
                    dbObj = new ClassBase();
                    sGameDb.Classes.Add((ClassBase)dbObj);
                    ClassBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.Item:
                    dbObj = new ItemBase();
                    sGameDb.Items.Add((ItemBase)dbObj);
                    ItemBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.Npc:
                    dbObj = new NpcBase();
                    sGameDb.Npcs.Add((NpcBase)dbObj);
                    NpcBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.Projectile:
                    dbObj = new ProjectileBase();
                    sGameDb.Projectiles.Add((ProjectileBase)dbObj);
                    ProjectileBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.Quest:
                    dbObj = new QuestBase();
                    sGameDb.Quests.Add((QuestBase)dbObj);
                    ((QuestBase)dbObj).StartEvent = (EventBase)AddGameObject(GameObjectType.Event);
                    ((QuestBase)dbObj).EndEvent = (EventBase)AddGameObject(GameObjectType.Event);
                    ((QuestBase)dbObj).StartEvent.CommonEvent = false;
                    ((QuestBase)dbObj).EndEvent.CommonEvent = false;
                    QuestBase.Lookup.Set(dbObj.Id,dbObj);
                    break;
                case GameObjectType.Resource:
                    dbObj = new ResourceBase();
                    sGameDb.Resources.Add((ResourceBase)dbObj);
                    ResourceBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.Shop:
                    dbObj = new ShopBase();
                    sGameDb.Shops.Add((ShopBase)dbObj);
                    ShopBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.Spell:
                    dbObj = new SpellBase();
                    sGameDb.Spells.Add((SpellBase)dbObj);
                    SpellBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.CraftTables:
                    dbObj = new CraftingTableBase();
                    sGameDb.CraftingTables.Add((CraftingTableBase)dbObj);
                    CraftingTableBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.Crafts:
                    dbObj = new CraftBase();
                    sGameDb.Crafts.Add((CraftBase)dbObj);
                    CraftBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.Map:
                    dbObj = new MapInstance();
                    sGameDb.Maps.Add((MapInstance)dbObj);
                    MapInstance.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.Event:
                    dbObj = new EventBase();
                    sGameDb.Events.Add((EventBase)dbObj);
                    EventBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.PlayerSwitch:
                    dbObj = new PlayerSwitchBase();
                    sGameDb.PlayerSwitches.Add((PlayerSwitchBase)dbObj);
                    PlayerSwitchBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.PlayerVariable:
                    dbObj = new PlayerVariableBase();
                    sGameDb.PlayerVariables.Add((PlayerVariableBase)dbObj);
                    PlayerVariableBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.ServerSwitch:
                    dbObj = new ServerSwitchBase();
                    sGameDb.ServerSwitches.Add((ServerSwitchBase)dbObj);
                    ServerSwitchBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.ServerVariable:
                    dbObj = new ServerVariableBase();
                    sGameDb.ServerVariables.Add((ServerVariableBase)dbObj);
                    ServerVariableBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.Tileset:
                    dbObj = new TilesetBase();
                    sGameDb.Tilesets.Add((TilesetBase)dbObj);
                    TilesetBase.Lookup.Set(dbObj.Id, dbObj);
                    break;
                case GameObjectType.Time:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
            }
            sGameDb.SaveChanges();
            return dbObj;
        }

        public static void DeleteGameObject(IDatabaseObject gameObject)
        {
            switch (gameObject.Type)
            {
                case GameObjectType.Animation:
                    sGameDb.Animations.Remove((AnimationBase)gameObject);
                    return;
                case GameObjectType.Class:
                    sGameDb.Classes.Remove((ClassBase) gameObject);
                    return;
                case GameObjectType.Item:
                    sGameDb.Items.Remove((ItemBase)gameObject);
                    return;
                case GameObjectType.Npc:
                    sGameDb.Npcs.Remove((NpcBase) gameObject);
                    return;
                case GameObjectType.Projectile:
                    sGameDb.Projectiles.Remove((ProjectileBase) gameObject);
                    return;
                case GameObjectType.Quest:
                    sGameDb.Events.Remove(((QuestBase)gameObject).StartEvent);
                    sGameDb.Events.Remove(((QuestBase)gameObject).EndEvent);
                    foreach (var tsk in ((QuestBase) gameObject).Tasks)
                    {
                        sGameDb.Events.Remove(tsk.CompletionEvent);
                    }
                    sGameDb.Quests.Remove((QuestBase) gameObject);
                    return;
                case GameObjectType.Resource:
                    sGameDb.Resources.Remove((ResourceBase) gameObject);
                    return;
                case GameObjectType.Shop:
                    sGameDb.Shops.Remove((ShopBase) gameObject);
                    return;
                case GameObjectType.Spell:
                    sGameDb.Spells.Remove((SpellBase) gameObject);
                    return;
                case GameObjectType.CraftTables:
                    sGameDb.CraftingTables.Remove((CraftingTableBase)gameObject);
                    return;
                case GameObjectType.Crafts:
                    sGameDb.Crafts.Remove((CraftBase)gameObject);
                    return;
                case GameObjectType.Map:
                    sGameDb.Maps.Remove((MapInstance) gameObject);
                    return;
                case GameObjectType.Event:
                    sGameDb.Events.Remove((EventBase) gameObject);
                    return;
                case GameObjectType.PlayerSwitch:
                    sGameDb.PlayerSwitches.Remove((PlayerSwitchBase) gameObject);
                    return;
                case GameObjectType.PlayerVariable:
                    sGameDb.PlayerVariables.Remove((PlayerVariableBase) gameObject);
                    return;
                case GameObjectType.ServerSwitch:
                    sGameDb.ServerSwitches.Remove((ServerSwitchBase) gameObject);
                    return;
                case GameObjectType.ServerVariable:
                    sGameDb.ServerVariables.Remove((ServerVariableBase)gameObject);
                    return;
                case GameObjectType.Tileset:
                    sGameDb.Tilesets.Remove((TilesetBase) gameObject);
                    return;
                case GameObjectType.Time:
                    return;
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
                    Gender = 0
                };
                var defaultFemale = new ClassSprite()
                {
                    Sprite = "2.png",
                    Gender = 1
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
                SaveGameDatabase();
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
                sGameDb.SaveChanges();
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
                sGameDb.SaveChanges();
            }
            else
            {
                MapList.SetList(mapFolders);
            }
            foreach (var map in MapBase.Lookup)
            {
                if (MapList.GetList().FindMap(map.Value.Id) == null)
                {
                    MapList.GetList().AddMap(map.Value.Id, MapBase.Lookup);
                }
            }
            SaveGameDatabase();
            PacketSender.SendMapListToAll();
        }

        //Time
        private static void LoadTime()
        {
            var time = sGameDb.Time.FirstOrDefault();
            if (time == null)
            {
                sGameDb.Time.Add(TimeBase.GetTimeBase());
                sGameDb.SaveChanges();
            }
            else
            {
                TimeBase.SetStaticTime(time);
            }
            ServerTime.Init();
        }

        public static void SaveGameDatabase()
        {
            sGameDb.SaveChanges();
        }

        public static void SavePlayerDatabase()
        {
            sPlayerDb.SaveChanges();
        }

        public static void DeteachOwnedType([NotNull]object entity)
        {
            sGameDb.Entry(entity).State = EntityState.Detached;
        }

        public static void AttachOwnedType([NotNull]object entity)
        {
            sGameDb.Entry(entity).State = EntityState.Modified;
        }
    }
}
