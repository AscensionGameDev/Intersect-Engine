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
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Server.Classes.Localization;
using Intersect.Logging;
using Intersect.Models;
using Intersect.Server.Classes.Database.PlayerData;
using Intersect.Server.Classes.Database.PlayerData.Characters;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;
using Intersect.Server.Database;
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
        private const int DbVersion = 11;

        private const string GameDbFilename = "resources/gamedata.db";
        private const string PlayersDbFilename = "resources/playerdata.db";

        //Database Variables
        private const string INFO_TABLE = "info";

        private const string DB_VERSION = "dbversion";

        //Log Table Constants
        private const string LOG_TABLE = "logs";

        private const string LOG_ID = "id";
        private const string LOG_TIME = "time";
        private const string LOG_TYPE = "type";
        private const string LOG_INFO = "info";

        //GameObject Table Constants
        private const string GAME_OBJECT_ID = "id";

        private const string GAME_OBJECT_DELETED = "deleted";
        private const string GAME_OBJECT_DATA = "data";

        //Map Tiles Table
        private const string MAP_TILES_TABLE = "map_tiles";
        private const string MAP_TILES_MAP_ID = "map_id";
        private const string MAP_TILES_DATA = "data";

        //Map List Table Constants
        private const string MAP_LIST_TABLE = "map_list";
        private const string MAP_LIST_DATA = "data";

        //Map Attributes Table Constants
        private const string MAP_ATTRIBUTES_TABLE = "map_attributes";
        private const string MAP_ATTRIBUTES_MAP_ID = "map_id";
        private const string MAP_ATTRIBUTES_DATA = "data";

        //Time of Day Table Constants
        private const string TIME_TABLE = "time";
        private const string TIME_DATA = "data";

        private static DatabaseConnection sGameDbConnection;

        private static PlayerContext sPlayerDb;

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
            sGameDbConnection?.Backup();
        }

        //Database setup, version checking
        public static bool InitDatabase()
        {
            SqliteConnection.SetConfig(SQLiteConfig.Serialized);

            sGameDbConnection = new DatabaseConnection(GameDbFilename, SetupGameDatabase);

            sGameDbConnection.Open();

            //Connect to new player database
            if (Options.PlayerDb.Type == DatabaseOptions.DatabaseType.sqlite)
            {
                sPlayerDb = new PlayerContext(PlayerContext.DbProvider.Sqlite, $"Data Source={PlayersDbFilename}");
            }
            else
            {
                sPlayerDb = new PlayerContext(PlayerContext.DbProvider.MySql, $"server={Options.PlayerDb.Server};database={Options.PlayerDb.Database};user={Options.PlayerDb.Username};password={Options.PlayerDb.Password}");   
            }
            sPlayerDb.Database.EnsureDeleted();
            sPlayerDb.Database.Migrate();

            if (sGameDbConnection.GetVersion() != DbVersion)
            {
                Console.WriteLine(Strings.Database.gamedboutofdate.ToString(sGameDbConnection.GetVersion(), DbVersion));
                return false;
            }

            LoadAllGameObjects();
            LoadTime();
            return true;
        }

        public static void SavePlayers()
        {
            sPlayerDb.SaveChanges();
        }

        private static void SetupGameDatabase(Object sender, EventArgs e)
        {
            sGameDbConnection = ((DatabaseConnection)sender);
            CreateInfoTable(sGameDbConnection);
            CreateMapTilesTable();
            CreateMapAttributesTable();
            CreateGameObjectTables();
            CreateMapListTable();
            CreateTimeTable();
            CreateLogsTable(sGameDbConnection);
        }

        private static void CreateTable(TableDescriptor tableDescriptor, DatabaseConnection conn)
        {
            using (var command = conn?.CreateCommand())
            {
                Debug.Assert(command != null, "command != null");
                command.CommandText = $"CREATE TABLE {tableDescriptor};";
                conn.ExecuteNonQuery(command);
            }
        }

        private static void CreateInfoTable(DatabaseConnection conn)
        {
            var columns = new List<ColumnDescriptor>()
            {
                new ColumnDescriptor(DB_VERSION, DataType.Integer)
            };

            CreateTable(new TableDescriptor(INFO_TABLE, columns),conn);

            var cmd = $"INSERT into {INFO_TABLE} (" + DB_VERSION + ") VALUES (" + DbVersion + ");";
            using (var createCommand = conn?.CreateCommand())
            {
                if (createCommand == null) return;
                createCommand.CommandText = cmd;
                conn.ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateLogsTable(DatabaseConnection conn)
        {
            var columns = new List<ColumnDescriptor>()
            {
                new ColumnDescriptor(LOG_ID, DataType.Integer) { PrimaryKey = true, Autoincrement = true },
                new ColumnDescriptor(LOG_TIME, DataType.Text),
                new ColumnDescriptor(LOG_TYPE, DataType.Text),
                new ColumnDescriptor(LOG_INFO, DataType.Text)
            };

            CreateTable(new TableDescriptor(LOG_TABLE, columns), conn);
        }

        private static void CreateGameObjectTables()
        {
            foreach (var val in Enum.GetValues(typeof(GameObjectType)))
            {
                if ((GameObjectType) val != GameObjectType.Time)
                    CreateGameObjectTable((GameObjectType) val);
            }
        }

        private static void CreateGameObjectTable(GameObjectType gameObjectType)
        {
            var columns = new List<ColumnDescriptor>()
            {
                new ColumnDescriptor(GAME_OBJECT_ID, DataType.Integer) { PrimaryKey = true, Autoincrement = true },
                new ColumnDescriptor(GAME_OBJECT_DELETED, DataType.Integer) { NotNull = true, Default = 0},
                new ColumnDescriptor(GAME_OBJECT_DATA, DataType.Text) { NotNull = true }
            };

            CreateTable(new TableDescriptor(gameObjectType.GetTable(), columns),sGameDbConnection);
        }

        private static void CreateMapTilesTable()
        {
            var columns = new List<ColumnDescriptor>()
            {
                new ColumnDescriptor(MAP_TILES_MAP_ID, DataType.Integer) { Unique = true },
                new ColumnDescriptor(MAP_TILES_DATA) { NotNull = true }
            };

            CreateTable(new TableDescriptor(MAP_TILES_TABLE, columns),sGameDbConnection);
        }

        private static void CreateMapListTable()
        {
            var columns = new List<ColumnDescriptor>()
            {
                new ColumnDescriptor(MAP_LIST_DATA) { NotNull = true }
            };

            CreateTable(new TableDescriptor(MAP_LIST_TABLE, columns),sGameDbConnection);
            InsertMapList();
        }

        private static void InsertMapList()
        {
            var cmd = $"INSERT into {MAP_LIST_TABLE} (" + MAP_LIST_DATA + ") VALUES (@" + MAP_LIST_DATA + ");";
            using (var createCommand = sGameDbConnection?.CreateCommand())
            {
                createCommand.Parameters.Add(new SqliteParameter("@" + MAP_LIST_DATA, new byte[1]));
                createCommand.CommandText = cmd;
                sGameDbConnection.ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateTimeTable()
        {
            var columns = new List<ColumnDescriptor>()
            {
                new ColumnDescriptor(TIME_DATA, DataType.Text) { NotNull = true }
            };

            CreateTable(new TableDescriptor(TIME_TABLE, columns), sGameDbConnection);
            InsertTime();
        }

        private static void InsertTime()
        {
            var cmd = $"INSERT into {TIME_TABLE} (" + TIME_DATA + ") VALUES (@" + TIME_DATA + ");";
            using (var createCommand = sGameDbConnection?.CreateCommand())
            {
                createCommand.Parameters.Add(new SqliteParameter("@" + TIME_DATA, new byte[1]));
                createCommand.CommandText = cmd;
                sGameDbConnection.ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateMapAttributesTable()
        {
            var columns = new List<ColumnDescriptor>()
            {
                new ColumnDescriptor(MAP_ATTRIBUTES_MAP_ID, DataType.Integer) { Unique = true },
                new ColumnDescriptor(MAP_ATTRIBUTES_DATA) { NotNull = true }
            };

            CreateTable(new TableDescriptor(MAP_ATTRIBUTES_TABLE, columns), sGameDbConnection);
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
            return sPlayerDb.Users.Where(p => string.Equals(p.Name.Trim(), username.Trim(), StringComparison.CurrentCultureIgnoreCase))?.First();
        }

        public static Character GetUserCharacter(User user, Guid characterId)
        {
            foreach (var character in user.Characters)
            {
                if (character.Id == characterId) return character;
            }
            return null;
        }

        public static Character GetCharacter(Guid id)
        {
            return sPlayerDb.Characters.Where(p => p.Id == id)?.First();
        }

        public static Character GetCharacter(string name)
        {
            return sPlayerDb.Characters.Where(p => string.Equals(p.Name.Trim(), name.Trim(), StringComparison.CurrentCultureIgnoreCase))?.First();
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

        public static void DeleteCharacterFriend([NotNull] Player player, [NotNull] Character friend)
        {
            sPlayerDb.Character_Friends.Remove(sPlayerDb.Character_Friends.SingleOrDefault(p => p.Owner == player.Character && p.Target == friend));
        }

        //Bags
        public static Bag CreateBag(int slotCount)
        {
            var bag = new Bag(slotCount);
            sPlayerDb.Bags.Add(bag);
            return bag;
        }

        public static bool BagEmpty([NotNull] Bag bag)
        {
            for (var i = 0; i < bag.Items.Count; i++)
            {
                if (bag.Items[i] != null)
                {
                    var item = ItemBase.Lookup.Get<ItemBase>(bag.Items[i].ItemNum);
                    if (item != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        //Bans and Mutes
        public static void AddMute([NotNull] Client player, int duration, [NotNull] string reason, [NotNull] string muter, string ip)
        {
            var mute = new Mute()
            {
                Player = player.User,
                StartTime = DateTime.UtcNow,
                Reason = reason,
                Ip = ip,
                EndTime = DateTime.UtcNow.AddDays(duration),
                Muter = muter
            };
            sPlayerDb.Mutes.Add(mute);
        }

        public static void DeleteMute([NotNull] User user)
        {
            sPlayerDb.Mutes.Remove(sPlayerDb.Mutes.SingleOrDefault(p => p.Player == user));
        }

        public static string CheckMute([NotNull] User user, string ip)
        {
            Mute mute = sPlayerDb.Mutes.SingleOrDefault(p => p.Player == user);
            if (mute == null) mute = sPlayerDb.Mutes.SingleOrDefault(p => p.Ip == ip);
            if (mute != null)
            {
                return Strings.Account.mutestatus.ToString(mute.StartTime, mute.Muter, mute.EndTime, mute.Reason);
            }
            return null;
        }

        public static void AddBan(Client player, int duration, string reason, string banner, string ip)
        {
            var ban = new Ban()
            {
                Player = player.User,
                StartTime = DateTime.UtcNow,
                Reason = reason,
                Ip = ip,
                EndTime = DateTime.UtcNow.AddDays(duration),
                Banner = banner
            };
            sPlayerDb.Bans.Add(ban);
        }

        public static void DeleteBan([NotNull] User user)
        {
            sPlayerDb.Mutes.Remove(sPlayerDb.Mutes.SingleOrDefault(p => p.Player == user));
        }

        public static string CheckBan([NotNull] User user, string ip)
        {
            Ban ban = sPlayerDb.Bans.SingleOrDefault(p => p.Player == user);
            if (ban == null) ban = sPlayerDb.Bans.SingleOrDefault(p => p.Ip == ip);
            if (ban != null)
            {
                return Strings.Account.banstatus.ToString(ban.StartTime, ban.Banner, ban.EndTime, ban.Reason);
            }
            return null;
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
                case GameObjectType.Bench:
                    BenchBase.Lookup.Clear();
                    break;
                case GameObjectType.Map:
                    MapBase.Lookup.Clear();
                    break;
                case GameObjectType.CommonEvent:
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

        private static void LoadGameObject(GameObjectType type, int index, string json)
        {
            JObject jObj;
            jObj = JObject.Parse(json);
            jObj.Add("Index", index);

            //In each case, do
            //obj = JsonConvert.DeserializeObject<AnimationBase>(jObj.ToString());
            //Then the Lookup.Set
            switch (type)
            {
                case GameObjectType.Animation:
                    var anim = JsonConvert.DeserializeObject<AnimationBase>(jObj.ToString());
                    AnimationBase.Lookup.Set(index, anim);
                    break;
                case GameObjectType.Class:
                    var cls = JsonConvert.DeserializeObject<ClassBase>(jObj.ToString());
                    ClassBase.Lookup.Set(index, cls);
                    break;
                case GameObjectType.Item:
                    var itm = JsonConvert.DeserializeObject<ItemBase>(jObj.ToString());
                    ItemBase.Lookup.Set(index, itm);
                    break;
                case GameObjectType.Npc:
                    var npc = JsonConvert.DeserializeObject<NpcBase>(jObj.ToString());
                    NpcBase.Lookup.Set(index, npc);
                    break;
                case GameObjectType.Projectile:
                    var proj = JsonConvert.DeserializeObject<ProjectileBase>(jObj.ToString());
                    ProjectileBase.Lookup.Set(index, proj);
                    break;
                case GameObjectType.Quest:
                    var qst = JsonConvert.DeserializeObject<QuestBase>(jObj.ToString());
                    QuestBase.Lookup.Set(index, qst);
                    break;
                case GameObjectType.Resource:
                    var res = JsonConvert.DeserializeObject<ResourceBase>(jObj.ToString());
                    ResourceBase.Lookup.Set(index, res);
                    break;
                case GameObjectType.Shop:
                    var shp = JsonConvert.DeserializeObject<ShopBase>(jObj.ToString());
                    ShopBase.Lookup.Set(index, shp);
                    break;
                case GameObjectType.Spell:
                    var spl = JsonConvert.DeserializeObject<SpellBase>(jObj.ToString());
                    SpellBase.Lookup.Set(index, spl);
                    break;
                case GameObjectType.Bench:
                    var cft = JsonConvert.DeserializeObject<BenchBase>(jObj.ToString());
                    BenchBase.Lookup.Set(index, cft);
                    break;
                case GameObjectType.Map:
                    var map = JsonConvert.DeserializeObject<MapInstance>(jObj.ToString());
                    MapInstance.Lookup.Set(index, map);
                    GetMapAttributes(map);
                    map.Initialize();
                    break;
                case GameObjectType.CommonEvent:
                    var evt = JsonConvert.DeserializeObject<EventBase>(jObj.ToString());
                    EventBase.Lookup.Set(index, evt);
                    break;
                case GameObjectType.PlayerSwitch:
                    var pswitch = JsonConvert.DeserializeObject<PlayerSwitchBase>(jObj.ToString());
                    PlayerSwitchBase.Lookup.Set(index, pswitch);
                    break;
                case GameObjectType.PlayerVariable:
                    var pvar = JsonConvert.DeserializeObject<PlayerVariableBase>(jObj.ToString());
                    PlayerVariableBase.Lookup.Set(index, pvar);
                    break;
                case GameObjectType.ServerSwitch:
                    var sswitch = JsonConvert.DeserializeObject<ServerSwitchBase>(jObj.ToString());
                    ServerSwitchBase.Lookup.Set(index, sswitch);
                    break;
                case GameObjectType.ServerVariable:
                    var svar = JsonConvert.DeserializeObject<ServerVariableBase>(jObj.ToString());
                    ServerVariableBase.Lookup.Set(index, svar);
                    break;
                case GameObjectType.Tileset:
                    var tset = JsonConvert.DeserializeObject<TilesetBase>(jObj.ToString());
                    TilesetBase.Lookup.Set(index, tset);
                    break;
                case GameObjectType.Time:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static void LoadGameObjects(GameObjectType gameObjectType)
        {
            var nullIssues = "";
            var tableName = gameObjectType.GetTable();
            ClearGameObjects(gameObjectType);
            var query = $"SELECT * from {tableName} WHERE " + GAME_OBJECT_DELETED + "=@" + GAME_OBJECT_DELETED +
                        ";";
            using (var cmd = sGameDbConnection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 0.ToString()));
                using (var dataReader = sGameDbConnection.ExecuteReader(cmd))
                {
                    while (dataReader.Read())
                    {
                        var index = Convert.ToInt32(dataReader[GAME_OBJECT_ID]);
                        if (dataReader[GAME_OBJECT_DATA].GetType() != typeof(DBNull))
                        {
                            var json = (string) dataReader[GAME_OBJECT_DATA];
                            if (!string.IsNullOrEmpty(json))
                            {
                                LoadGameObject(gameObjectType, index, json);
                            }
                        }
                        else
                        {
                            nullIssues += Strings.Database.nullfound.ToString( index, tableName) + Environment.NewLine;
                        }
                    }
                }
            }
            if (nullIssues != "")
            {
                throw (new Exception(Strings.Database.nullerror + Environment.NewLine + nullIssues));
            }
        }

        public static void SaveGameObject(IDatabaseObject gameObject)
        {
            if (gameObject == null)
            {
                Log.Error("Attempted to persist null game object to the Database.");
            }

            var insertQuery = "UPDATE " + gameObject.DatabaseTable + " set " + GAME_OBJECT_DELETED + "=@" +
                              GAME_OBJECT_DELETED + "," + GAME_OBJECT_DATA + "=@" + GAME_OBJECT_DATA + " WHERE " +
                              GAME_OBJECT_ID + "=@" + GAME_OBJECT_ID + ";";
            using (var cmd = sGameDbConnection.CreateCommand())
            {
                cmd.CommandText = insertQuery;
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_ID, gameObject.Index));
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, "0"));
                if (!string.IsNullOrEmpty(gameObject.JsonData))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DATA, gameObject.JsonData));
                    try
                    {
                        var returnVal = sGameDbConnection.ExecuteNonQuery(cmd);
                        if (returnVal <= 0)
                        {
                            throw new Exception("ExecuteNonQuery updating game object failed!");
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception);
                        throw exception;
                    }
                }
            }

            if (gameObject.Type != GameObjectType.Map) return;
            var map = (MapBase) gameObject;
            SaveMapAttributes(map.Index, map.AttributesData());
            if (map.TileData != null)
            {
                SaveMapTiles(map.Index, map.TileData);
            }
        }

        public static IDatabaseObject AddGameObject(GameObjectType gameObjectType)
        {
            var insertQuery = $"INSERT into {gameObjectType.GetTable()} (" + GAME_OBJECT_DATA + ") VALUES (@" +
                              GAME_OBJECT_DATA + ")" + "; SELECT last_insert_rowid();";
            var index = -1;
            using (var cmd = sGameDbConnection.CreateCommand())
            {
                cmd.CommandText = insertQuery;
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DATA, new byte[1]));
                index = (int) ((long) sGameDbConnection.ExecuteScalar(cmd));
            }
            if (index > -1)
            {
                IDatabaseObject dbObj = null;
                switch (gameObjectType)
                {
                    case GameObjectType.Animation:
                        var obja = new AnimationBase(index);
                        dbObj = obja;
                        AnimationBase.Lookup.Set(index, obja);
                        break;
                    case GameObjectType.Class:
                        var objc = new ClassBase(index);
                        dbObj = objc;
                        ClassBase.Lookup.Set(index, objc);
                        break;
                    case GameObjectType.Item:
                        var objd = new ItemBase(index);
                        dbObj = objd;
                        ItemBase.Lookup.Set(index, objd);
                        break;
                    case GameObjectType.Npc:
                        var objq = new NpcBase(index);
                        dbObj = objq;
                        NpcBase.Lookup.Set(index, objq);
                        break;
                    case GameObjectType.Projectile:
                        var objwe = new ProjectileBase(index);
                        dbObj = objwe;
                        ProjectileBase.Lookup.Set(index, objwe);
                        break;
                    case GameObjectType.Quest:
                        var objqw = new QuestBase(index);
                        dbObj = objqw;
                        QuestBase.Lookup.Set(index, objqw);
                        break;
                    case GameObjectType.Resource:
                        var objy = new ResourceBase(index);
                        dbObj = objy;
                        ResourceBase.Lookup.Set(index, objy);
                        break;
                    case GameObjectType.Shop:
                        var objt = new ShopBase(index);
                        dbObj = objt;
                        ShopBase.Lookup.Set(index, objt);
                        break;
                    case GameObjectType.Spell:
                        var objr = new SpellBase(index);
                        dbObj = objr;
                        SpellBase.Lookup.Set(index, objr);
                        break;
                    case GameObjectType.Bench:
                        var obje = new BenchBase(index);
                        dbObj = obje;
                        BenchBase.Lookup.Set(index, obje);
                        break;
                    case GameObjectType.Map:
                        var objw = new MapInstance(index);
                        dbObj = objw;
                        MapInstance.Lookup.Set(index, objw);
                        break;
                    case GameObjectType.CommonEvent:
                        var objf = new EventBase(index, -1, -1, -1,true);
                        dbObj = objf;
                        EventBase.Lookup.Set(index, objf);
                        break;
                    case GameObjectType.PlayerSwitch:
                        var objz = new PlayerSwitchBase(index);
                        dbObj = objz;
                        PlayerSwitchBase.Lookup.Set(index, objz);
                        break;
                    case GameObjectType.PlayerVariable:
                        var objx = new PlayerVariableBase(index);
                        dbObj = objx;
                        PlayerVariableBase.Lookup.Set(index, objx);
                        break;
                    case GameObjectType.ServerSwitch:
                        var ssbobj = new ServerSwitchBase(index);
                        dbObj = ssbobj;
                        ServerSwitchBase.Lookup.Set(index, ssbobj);
                        break;
                    case GameObjectType.ServerVariable:
                        var svbobj = new ServerVariableBase(index);
                        dbObj = svbobj;
                        ServerVariableBase.Lookup.Set(index, svbobj);
                        break;
                    case GameObjectType.Tileset:
                        var tset = new TilesetBase(index);
                        dbObj = tset;
                        TilesetBase.Lookup.Set(index, tset);
                        break;
                    case GameObjectType.Time:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
                }

                SaveGameObject(dbObj);
                return dbObj;
            }

            return null;
        }

        public static void DeleteGameObject(IDatabaseObject gameObject)
        {
            var insertQuery = "UPDATE " + gameObject.DatabaseTable + " set " + GAME_OBJECT_DELETED + "=@" +
                              GAME_OBJECT_DELETED + " WHERE " +
                              GAME_OBJECT_ID + "=@" + GAME_OBJECT_ID + ";";
            using (var cmd = sGameDbConnection.CreateCommand())
            {
                cmd.CommandText = insertQuery;
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_ID, gameObject.Index));
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 1.ToString()));
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DATA, gameObject.JsonData));
                sGameDbConnection.ExecuteNonQuery(cmd);
            }
            gameObject.Delete();
        }

        //Map Tiles Saving/Loading
        public static byte[] GetMapTiles(int index)
        {
            var nullIssues = "";
            var query = $"SELECT * from {MAP_TILES_TABLE} WHERE " + MAP_TILES_MAP_ID + "=@" + MAP_TILES_MAP_ID +
                        ";";
            using (var cmd = sGameDbConnection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_TILES_MAP_ID, index));
                using (var dataReader = sGameDbConnection.ExecuteReader(cmd))
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        if (dataReader[MAP_TILES_DATA].GetType() != typeof(DBNull))
                        {
                            return (byte[]) dataReader[MAP_TILES_DATA];
                        }
                        else
                        {
                            nullIssues += Strings.Database.nullfound.ToString( index, MAP_TILES_TABLE) +
                                          Environment.NewLine;
                        }
                    }
                    else
                    {
                        return new byte[Options.LayerCount * Options.MapWidth * Options.MapHeight * 13];
                    }
                }
            }
            if (nullIssues != "")
            {
                throw (new Exception(Strings.Database.nullerror + Environment.NewLine + nullIssues));
            }
            return null;
        }

        //Map Tiles Saving/Loading
        public static void GetMapAttributes(MapInstance map)
        {
            var nullIssues = "";
            var query = $"SELECT * from {MAP_ATTRIBUTES_TABLE} WHERE " + MAP_ATTRIBUTES_MAP_ID + "=@" + MAP_ATTRIBUTES_MAP_ID +
                        ";";
            using (var cmd = sGameDbConnection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_ATTRIBUTES_MAP_ID, map.Index));
                using (var dataReader = sGameDbConnection.ExecuteReader(cmd))
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        if (dataReader[MAP_ATTRIBUTES_DATA].GetType() != typeof(DBNull))
                        {
                            map.LoadAttributes((byte[]) dataReader[MAP_ATTRIBUTES_DATA]);
                        }
                        else
                        {
                            nullIssues += Strings.Database.nullfound.ToString(map.Index, MAP_ATTRIBUTES_TABLE) +
                                          Environment.NewLine;
                        }
                    }
                    else
                    {
                        //Gotta calculate :/
                        map.LoadAttributes(new byte[Options.MapWidth * Options.MapHeight * 4]);
                    }
                }
            }
            if (nullIssues != "")
            {
                throw (new Exception(Strings.Database.nullerror + Environment.NewLine + nullIssues));
            }
        }

        public static void SaveMapTiles(int index, byte[] data)
        {
            if (data == null) return;
            var query = "INSERT OR REPLACE into " + MAP_TILES_TABLE + " (" + MAP_TILES_MAP_ID + "," + MAP_TILES_DATA +
                        ")" + " VALUES " + " (@" + MAP_TILES_MAP_ID + ",@" + MAP_TILES_DATA + ")";
            using (var cmd = sGameDbConnection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_TILES_MAP_ID, index));
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_TILES_DATA, data));
                sGameDbConnection.ExecuteNonQuery(cmd);
            }
        }

        public static void SaveMapAttributes(int index, byte[] data)
        {
            if (data == null) return;
            var query = "INSERT OR REPLACE into " + MAP_ATTRIBUTES_TABLE + " (" + MAP_ATTRIBUTES_MAP_ID + "," + MAP_ATTRIBUTES_DATA +
                        ")" + " VALUES " + " (@" + MAP_ATTRIBUTES_MAP_ID + ",@" + MAP_ATTRIBUTES_DATA + ")";
            using (var cmd = sGameDbConnection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_ATTRIBUTES_MAP_ID, index));
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_ATTRIBUTES_DATA, data));
                sGameDbConnection.ExecuteNonQuery(cmd);
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
                SaveGameObject(cls);
            }
        }

        //Extra Map Helper Functions
        public static void CheckAllMapConnections()
        {
            foreach (MapBase map in MapInstance.Lookup.IndexValues)
            {
                CheckMapConnections(map, MapInstance.Lookup);
            }
        }

        public static void CheckMapConnections(MapBase map, DatabaseObjectLookup maps)
        {
            var updated = false;
            if (!maps.IndexKeys.Contains(map.Up) && map.Up != -1)
            {
                map.Up = -1;
                updated = true;
            }
            if (!maps.IndexKeys.Contains(map.Down) && map.Down != -1)
            {
                map.Down = -1;
                updated = true;
            }
            if (!maps.IndexKeys.Contains(map.Left) && map.Left != -1)
            {
                map.Left = -1;
                updated = true;
            }
            if (!maps.IndexKeys.Contains(map.Right) && map.Right != -1)
            {
                map.Right = -1;
                updated = true;
            }
            if (updated)
            {
                SaveGameObject(map);
                PacketSender.SendMapToEditors(map.Index);
            }
        }

        public static void GenerateMapGrids()
        {
            lock (MapGridLock)
            {
                MapGrids.Clear();
                foreach (var map in MapInstance.Lookup.IndexValues)
                {
                    if (MapGrids.Count == 0)
                    {
                        MapGrids.Add(new MapGrid(map.Index, 0));
                    }
                    else
                    {
                        for (var y = 0; y < MapGrids.Count; y++)
                        {
                            if (!MapGrids[y].HasMap(map.Index))
                            {
                                if (y != MapGrids.Count - 1) continue;
                                MapGrids.Add(new MapGrid(map.Index, MapGrids.Count));
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                foreach (MapInstance map in MapInstance.Lookup.IndexValues)
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
                                y < MapGrids[myGrid].YMax && MapGrids[myGrid].MyGrid[x, y] > -1)
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
            var query = $"SELECT * from {MAP_LIST_TABLE};";
            using (var cmd = sGameDbConnection.CreateCommand())
            {
                cmd.CommandText = query;
                using (var dataReader = sGameDbConnection.ExecuteReader(cmd))
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (dataReader[MAP_LIST_DATA].GetType() != typeof(DBNull))
                            {
                                var data = (byte[]) dataReader[MAP_LIST_DATA];
                                if (data.Length > 1)
                                {
                                    var myBuffer = new ByteBuffer();
                                    myBuffer.WriteBytes(data);
                                    MapList.GetList().Load(myBuffer, MapBase.Lookup, true, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        InsertMapList();
                    }
                }
            }
            foreach (var map in MapBase.Lookup)
            {
                if (MapList.GetList().FindMap(map.Value.Index) == null)
                {
                    MapList.GetList().AddMap(map.Value.Index, MapBase.Lookup);
                }
            }
            SaveMapFolders();
            PacketSender.SendMapListToAll();
        }

        public static void SaveMapFolders()
        {
            var query = "UPDATE " + MAP_LIST_TABLE + " set " + MAP_LIST_DATA + "=@" + MAP_LIST_DATA + ";";
            using (var cmd = sGameDbConnection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_LIST_DATA,
                    MapList.GetList().Data(MapBase.Lookup)));
                sGameDbConnection.ExecuteNonQuery(cmd);
            }
        }

        //Time
        private static void LoadTime()
        {
            var query = $"SELECT * from {TIME_TABLE};";
            using (var cmd = sGameDbConnection.CreateCommand())
            {
                cmd.CommandText = query;
                using (var dataReader = sGameDbConnection.ExecuteReader(cmd))
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (dataReader[TIME_DATA].GetType() != typeof(DBNull))
                            {
                                var json = (string) dataReader[TIME_DATA];
                                if (!string.IsNullOrEmpty(json))
                                {
                                    TimeBase.GetTimeBase().LoadFromJson(json);
                                }
                            }
                        }
                    }
                    else
                    {
                        InsertTime();
                    }
                }
            }

            SaveTime();
            ServerTime.Init();
        }

        public static void SaveTime()
        {
            var query = "UPDATE " + TIME_TABLE + " set " + TIME_DATA + "=@" + TIME_DATA + ";";
            using (var cmd = sGameDbConnection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.Add(new SqliteParameter("@" + TIME_DATA,
                    TimeBase.GetTimeJson()));
                sGameDbConnection.ExecuteNonQuery(cmd);
            }
        }
    }
}
