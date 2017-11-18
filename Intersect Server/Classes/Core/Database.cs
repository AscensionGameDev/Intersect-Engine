using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Intersect.Collections;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Localization;
using Intersect.Logging;
using Intersect.Models;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Items;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;
using Mono.Data.Sqlite;

namespace Intersect.Server.Classes.Core
{
    public static class Database
    {
        private const string DIRECTORY_BACKUPS = "resources/backups";
        private const int DbVersion = 10;
        private const string DbFilename = "resources/intersect.db";

        //Database Variables
        private const string INFO_TABLE = "info";

        private const string DB_VERSION = "dbversion";

        //Ban Table Constants
        private const string BAN_TABLE = "bans";

        private const string BAN_ID = "id";
        private const string BAN_TIME = "time";
        private const string BAN_USER = "user";
        private const string BAN_IP = "ip";
        private const string BAN_DURATION = "duration";
        private const string BAN_REASON = "reason";
        private const string BAN_BANNER = "banner";

        //Mute Table Constants
        private const string MUTE_TABLE = "mutes";

        private const string MUTE_ID = "id";
        private const string MUTE_TIME = "time";
        private const string MUTE_USER = "user";
        private const string MUTE_IP = "ip";
        private const string MUTE_DURATION = "duration";
        private const string MUTE_REASON = "reason";
        private const string MUTE_MUTER = "muter";

        //Log Table Constants
        private const string LOG_TABLE = "logs";

        private const string LOG_ID = "id";
        private const string LOG_TIME = "time";
        private const string LOG_TYPE = "type";
        private const string LOG_INFO = "info";

        //User Table Constants
        private const string USERS_TABLE = "users";

        private const string USER_ID = "id";
        private const string USER_NAME = "user";
        private const string USER_PASS = "pass";
        private const string USER_SALT = "salt";
        private const string USER_EMAIL = "email";
        private const string USER_POWER = "power";

        //Character Table Constants
        private const string CHAR_TABLE = "characters";

        private const string CHAR_ID = "id";
        private const string CHAR_USER_ID = "user_id";
        private const string CHAR_DELETED = "deleted";
        private const string CHAR_NAME = "name";
        private const string CHAR_MAP = "map";
        private const string CHAR_X = "x";
        private const string CHAR_Y = "y";
        private const string CHAR_Z = "z";
        private const string CHAR_DIR = "dir";
        private const string CHAR_SPRITE = "sprite";
        private const string CHAR_FACE = "face";
        private const string CHAR_CLASS = "class";
        private const string CHAR_GENDER = "gender";
        private const string CHAR_LEVEL = "level";
        private const string CHAR_EXP = "exp";
        private const string CHAR_VITALS = "vitals";
        private const string CHAR_MAX_VITALS = "maxvitals";
        private const string CHAR_STATS = "stats";
        private const string CHAR_STAT_POINTS = "statpoints";
        private const string CHAR_EQUIPMENT = "equipment";
        private const string CHAR_LAST_ONLINE_TIME = "last_online";

        //Char Inventory Table Constants
        private const string CHAR_INV_TABLE = "char_inventory";

        private const string CHAR_INV_CHAR_ID = "char_id";
        private const string CHAR_INV_SLOT = "slot";
        private const string CHAR_INV_ITEM_NUM = "itemnum";
        private const string CHAR_INV_ITEM_VAL = "itemval";
        private const string CHAR_INV_ITEM_STATS = "itemstats";
        private const string CHAR_INV_ITEM_BAG_ID = "item_bag_id";

        //Char Spells Table Constants
        private const string CHAR_SPELL_TABLE = "char_spells";

        private const string CHAR_SPELL_CHAR_ID = "char_id";
        private const string CHAR_SPELL_SLOT = "slot";
        private const string CHAR_SPELL_NUM = "spellnum";
        private const string CHAR_SPELL_CD = "spellcd";

        //Char Hotbar Table Constants
        private const string CHAR_HOTBAR_TABLE = "char_hotbar";

        private const string CHAR_HOTBAR_CHAR_ID = "char_id";
        private const string CHAR_HOTBAR_SLOT = "slot";
        private const string CHAR_HOTBAR_TYPE = "type";
        private const string CHAR_HOTBAR_ITEMSLOT = "itemslot";

        //Char Bank Table Constants
        private const string CHAR_BANK_TABLE = "char_bank";

        private const string CHAR_BANK_CHAR_ID = "char_id";
        private const string CHAR_BANK_SLOT = "slot";
        private const string CHAR_BANK_ITEM_NUM = "itemnum";
        private const string CHAR_BANK_ITEM_VAL = "itemval";
        private const string CHAR_BANK_ITEM_STATS = "itemstats";
        private const string CHAR_BANK_ITEM_BAG_ID = "item_bag_id";

        //Char Switches Table Constants
        private const string CHAR_SWITCHES_TABLE = "char_switches";

        private const string CHAR_SWITCH_CHAR_ID = "char_id";
        private const string CHAR_SWITCH_SLOT = "slot";
        private const string CHAR_SWITCH_VAL = "val";

        //Char Variables Table Constants
        private const string CHAR_VARIABLES_TABLE = "char_variables";

        private const string CHAR_VARIABLE_CHAR_ID = "char_id";
        private const string CHAR_VARIABLE_SLOT = "slot";
        private const string CHAR_VARIABLE_VAL = "val";

        //Char Quests Table Constants
        private const string CHAR_QUESTS_TABLE = "char_quests";

        private const string CHAR_QUEST_CHAR_ID = "char_id";
        private const string CHAR_QUEST_ID = "quest_id";
        private const string CHAR_QUEST_TASK = "task";
        private const string CHAR_QUEST_TASK_PROGRESS = "task_progress";
        private const string CHAR_QUEST_COMPLETED = "completed";

        //Char Friendss Table Constants
        private const string CHAR_FRIENDS_TABLE = "char_friends";

        private const string CHAR_FRIEND_CHAR_ID = "char_id";
        private const string CHAR_FRIEND_ID = "friend_id";

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

        //Time of Day Table Constants
        private const string TIME_TABLE = "time";

        private const string TIME_DATA = "data";

        //Bag Table Constants
        private const string BAGS_TABLE = "bags";

        private const string BAG_ID = "bag_id";
        private const string BAG_SLOT_COUNT = "slot_count";

        //Bag Items Table Constants
        private const string BAG_ITEMS_TABLE = "bag_items";

        private const string BAG_ITEM_CONTAINER_ID = "bag_id";
        private const string BAG_ITEM_SLOT = "slot";
        private const string BAG_ITEM_NUM = "itemnum";
        private const string BAG_ITEM_VAL = "itemval";
        private const string BAG_ITEM_STATS = "itemstats";
        private const string BAG_ITEM_BAG_ID = "item_bag_id";
        private static SqliteConnection sDbConnection;

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
        }

        private static void BackupDiskCopy()
        {
            CheckDirectories();

            File.Copy("resources/intersect.db", $"{DIRECTORY_BACKUPS}/intersect_{DateTime.Now:yyyy-MM-dd hh-mm-ss}.db");
        }

        //Database setup, version checking
        public static bool InitDatabase()
        {
            SqliteConnection.SetConfig(SQLiteConfig.Serialized);

            if (File.Exists(DbFilename)) BackupDiskCopy();
            else CreateDatabase();
          
            if (sDbConnection == null)
            {
                sDbConnection = new SqliteConnection($"Data Source={DbFilename},Version=3");
                sDbConnection?.Open();
            }
            if (GetDatabaseVersion() != DbVersion)
            {
                Console.WriteLine(Strings.Get("database", "outofdate", GetDatabaseVersion(), DbVersion));
                return false;
            }
            LoadAllGameObjects();
            LoadTime();
            return true;
        }

        private static long GetDatabaseVersion()
        {
            long version = -1;
            var cmd = $"SELECT {DB_VERSION} from {INFO_TABLE};";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                version = (long) ExecuteScalar(createCommand);
            }

            return version;
        }

        private static void CreateDatabase()
        {
            sDbConnection = new SqliteConnection($"Data Source={DbFilename},Version=3,New=True");
            sDbConnection?.Open();
            CreateInfoTable();
            CreateUsersTable();
            CreateCharactersTable();
            CreateCharacterInventoryTable();
            CreateCharacterSpellsTable();
            CreateCharacterHotbarTable();
            CreateCharacterBankTable();
            CreateCharacterSwitchesTable();
            CreateCharacterVariablesTable();
            CreateCharacterQuestsTable();
            CreateCharacterFriendsTable();
            CreateMapTilesTable();
            CreateGameObjectTables();
            CreateMapListTable();
            CreateTimeTable();
            CreateBansTable();
            CreateMutesTable();
            CreateLogsTable();
            CreateBagsTable();
            CreateBagItemsTable();
        }

        private static void CreateInfoTable()
        {
            var cmd = $"CREATE TABLE {INFO_TABLE} (" + DB_VERSION + " INTEGER NOT NULL);";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
            cmd = $"INSERT into {INFO_TABLE} (" + DB_VERSION + ") VALUES (" + DbVersion + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                if (createCommand == null) return;
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateLogsTable()
        {
            var cmd = $"CREATE TABLE {LOG_TABLE} ("
                      + LOG_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                      + LOG_TIME + " TEXT,"
                      + LOG_TYPE + " TEXT,"
                      + LOG_INFO + " TEXT"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                if (createCommand == null) return;
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateMutesTable()
        {
            var cmd = $"CREATE TABLE {MUTE_TABLE} ("
                      + MUTE_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                      + MUTE_TIME + " TEXT,"
                      + MUTE_USER + " INTEGER,"
                      + MUTE_IP + " TEXT,"
                      + MUTE_DURATION + " INTEGER,"
                      + MUTE_REASON + " TEXT,"
                      + MUTE_MUTER + " TEXT"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                if (createCommand == null) return;
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateBansTable()
        {
            var cmd = $"CREATE TABLE {BAN_TABLE} ("
                      + BAN_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                      + BAN_TIME + " TEXT,"
                      + BAN_USER + " INTEGER,"
                      + BAN_IP + " TEXT,"
                      + BAN_DURATION + " INTEGER,"
                      + BAN_REASON + " TEXT,"
                      + BAN_BANNER + " TEXT"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                if (createCommand == null) return;
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateUsersTable()
        {
            var cmd = $"CREATE TABLE {USERS_TABLE} ("
                      + USER_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                      + USER_NAME + " TEXT,"
                      + USER_PASS + " TEXT,"
                      + USER_SALT + " TEXT,"
                      + USER_EMAIL + " TEXT,"
                      + USER_POWER + " INTEGER"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateCharactersTable()
        {
            var cmd = $"CREATE TABLE {CHAR_TABLE} ("
                      + CHAR_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                      + CHAR_USER_ID + " INTEGER,"
                      + CHAR_NAME + " TEXT,"
                      + CHAR_DELETED + " INTEGER,"
                      + CHAR_MAP + " INTEGER,"
                      + CHAR_X + " INTEGER,"
                      + CHAR_Y + " INTEGER,"
                      + CHAR_Z + " INTEGER,"
                      + CHAR_DIR + " INTEGER,"
                      + CHAR_SPRITE + " TEXT,"
                      + CHAR_FACE + " TEXT,"
                      + CHAR_CLASS + " INTEGER,"
                      + CHAR_GENDER + " INTEGER,"
                      + CHAR_LEVEL + " INTEGER,"
                      + CHAR_EXP + " INTEGER,"
                      + CHAR_VITALS + " TEXT,"
                      + CHAR_MAX_VITALS + " TEXT,"
                      + CHAR_STATS + " TEXT,"
                      + CHAR_STAT_POINTS + " INTEGER,"
                      + CHAR_EQUIPMENT + " TEXT,"
                      + CHAR_LAST_ONLINE_TIME + " INTEGER"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateCharacterInventoryTable()
        {
            var cmd = $"CREATE TABLE {CHAR_INV_TABLE} ("
                      + CHAR_INV_CHAR_ID + " INTEGER,"
                      + CHAR_INV_SLOT + " INTEGER,"
                      + CHAR_INV_ITEM_NUM + " INTEGER,"
                      + CHAR_INV_ITEM_VAL + " INTEGER,"
                      + CHAR_INV_ITEM_STATS + " TEXT,"
                      + CHAR_INV_ITEM_BAG_ID + " INTEGER,"
                      + " unique(`" + CHAR_INV_CHAR_ID + "`,`" + CHAR_INV_SLOT + "`)"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateCharacterSpellsTable()
        {
            var cmd = $"CREATE TABLE {CHAR_SPELL_TABLE} ("
                      + CHAR_SPELL_CHAR_ID + " INTEGER,"
                      + CHAR_SPELL_SLOT + " INTEGER,"
                      + CHAR_SPELL_NUM + " INTEGER,"
                      + CHAR_SPELL_CD + " INTEGER,"
                      + " unique('" + CHAR_SPELL_CHAR_ID + "','" + CHAR_SPELL_SLOT + "')"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateCharacterHotbarTable()
        {
            var cmd = $"CREATE TABLE {CHAR_HOTBAR_TABLE} ("
                      + CHAR_HOTBAR_CHAR_ID + " INTEGER,"
                      + CHAR_HOTBAR_SLOT + " INTEGER,"
                      + CHAR_HOTBAR_TYPE + " INTEGER,"
                      + CHAR_HOTBAR_ITEMSLOT + " INTEGER,"
                      + " unique('" + CHAR_HOTBAR_CHAR_ID + "','" + CHAR_HOTBAR_SLOT + "')"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateCharacterBankTable()
        {
            var cmd = $"CREATE TABLE {CHAR_BANK_TABLE} ("
                      + CHAR_BANK_CHAR_ID + " INTEGER,"
                      + CHAR_BANK_SLOT + " INTEGER,"
                      + CHAR_BANK_ITEM_NUM + " INTEGER,"
                      + CHAR_BANK_ITEM_VAL + " INTEGER,"
                      + CHAR_BANK_ITEM_STATS + " TEXT,"
                      + CHAR_BANK_ITEM_BAG_ID + " INTEGER,"
                      + " unique('" + CHAR_BANK_CHAR_ID + "','" + CHAR_BANK_SLOT + "')"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateCharacterSwitchesTable()
        {
            var cmd = $"CREATE TABLE {CHAR_SWITCHES_TABLE} ("
                      + CHAR_SWITCH_CHAR_ID + " INTEGER,"
                      + CHAR_SWITCH_SLOT + " INTEGER,"
                      + CHAR_SWITCH_VAL + " INTEGER,"
                      + " unique('" + CHAR_SWITCH_CHAR_ID + "','" + CHAR_SWITCH_SLOT + "')"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateCharacterVariablesTable()
        {
            var cmd = $"CREATE TABLE {CHAR_VARIABLES_TABLE} ("
                      + CHAR_VARIABLE_CHAR_ID + " INTEGER,"
                      + CHAR_VARIABLE_SLOT + " INTEGER,"
                      + CHAR_VARIABLE_VAL + " INTEGER,"
                      + " unique('" + CHAR_VARIABLE_CHAR_ID + "','" + CHAR_VARIABLE_SLOT + "')"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateCharacterQuestsTable()
        {
            var cmd = $"CREATE TABLE {CHAR_QUESTS_TABLE} ("
                      + CHAR_QUEST_CHAR_ID + " INTEGER,"
                      + CHAR_QUEST_ID + " INTEGER,"
                      + CHAR_QUEST_TASK + " INTEGER,"
                      + CHAR_QUEST_TASK_PROGRESS + " INTEGER,"
                      + CHAR_QUEST_COMPLETED + " INTEGER,"
                      + " unique('" + CHAR_QUEST_CHAR_ID + "','" + CHAR_QUEST_ID + "')"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateCharacterFriendsTable()
        {
            var cmd = $"CREATE TABLE {CHAR_FRIENDS_TABLE} ("
                      + CHAR_FRIEND_CHAR_ID + " INTEGER,"
                      + CHAR_FRIEND_ID + " INTEGER,"
                      + " unique('" + CHAR_FRIEND_CHAR_ID + "','" + CHAR_FRIEND_ID + "')"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateBagsTable()
        {
            var cmd = $"CREATE TABLE {BAGS_TABLE} ("
                      + BAG_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                      + BAG_SLOT_COUNT + " INTEGER"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateBagItemsTable()
        {
            var cmd = $"CREATE TABLE {BAG_ITEMS_TABLE} ("
                      + BAG_ITEM_CONTAINER_ID + " INTEGER,"
                      + BAG_ITEM_SLOT + " INTEGER,"
                      + BAG_ITEM_NUM + " INTEGER,"
                      + BAG_ITEM_VAL + " INTEGER,"
                      + BAG_ITEM_STATS + " TEXT,"
                      + BAG_ITEM_BAG_ID + " TEXT,"
                      + " unique('" + BAG_ITEM_CONTAINER_ID + "','" + BAG_ITEM_SLOT + "')"
                      + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
            CreateBag(1);
            //This is to bypass an issue where we use itemVal to store the bag identifier (we are terrible!)
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
            var cmd = $"CREATE TABLE {gameObjectType.GetTable()} ("
                      + GAME_OBJECT_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                      + GAME_OBJECT_DELETED + " INTEGER NOT NULL DEFAULT 0,"
                      + GAME_OBJECT_DATA + " BLOB NOT NULL" + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateMapTilesTable()
        {
            var cmd = $"CREATE TABLE {MAP_TILES_TABLE} (" + MAP_TILES_MAP_ID + " INTEGER UNIQUE, " +
                      MAP_TILES_DATA + " BLOB NOT NULL);";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateMapListTable()
        {
            var cmd = $"CREATE TABLE {MAP_LIST_TABLE} (" + MAP_LIST_DATA + " BLOB NOT NULL);";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
            InsertMapList();
        }

        private static void InsertMapList()
        {
            var cmd = $"INSERT into {MAP_LIST_TABLE} (" + MAP_LIST_DATA + ") VALUES (@" + MAP_LIST_DATA + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.Parameters.Add(new SqliteParameter("@" + MAP_LIST_DATA, new byte[1]));
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        private static void CreateTimeTable()
        {
            var cmd = $"CREATE TABLE {TIME_TABLE} (" + TIME_DATA + " BLOB NOT NULL);";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
            InsertTime();
        }

        private static void InsertTime()
        {
            var cmd = $"INSERT into {TIME_TABLE} (" + TIME_DATA + ") VALUES (@" + TIME_DATA + ");";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.Parameters.Add(new SqliteParameter("@" + TIME_DATA, new byte[1]));
                createCommand.CommandText = cmd;
                ExecuteNonQuery(createCommand);
            }
        }

        //Players General
        public static void LoadPlayerDatabase()
        {
            Console.WriteLine(Strings.Get("database", "usingsqlite"));
        }

        public static Client GetPlayerClient(string username)
        {
            //Try to fetch a player entity by username, online or offline.
            //Check Online First
            lock (Globals.ClientLock)
            {
                foreach (var client in Globals.Clients)
                {
                    if (client.Entity != null && client.MyAccount.ToLower() == username.ToLower())
                    {
                        return client;
                    }
                }
            }

            //Didn't find the player online, lets load him from our database.
            var fakeClient = new Client(-1, null);
            var en = new Player(-1, fakeClient);
            fakeClient.Entity = en;
            fakeClient.MyAccount = username;
            LoadUser(fakeClient);
            return fakeClient;
        }

        public static void SetPlayerPower(string username, int power)
        {
            if (AccountExists(username))
            {
                var client = GetPlayerClient(username);
                client.Power = power;
                SaveUser(client);
                PacketSender.SendPlayerMsg(client, Strings.Get("player", "powerchanged"), client.Entity.MyName);
                Console.WriteLine(Strings.Get("commandoutput", "powerlevel", username, power));
            }
            else
            {
                Console.WriteLine(Strings.Get("account", "doesnotexist"));
            }
        }

        //User Info
        public static bool AccountExists(string accountname)
        {
            long count = -1;
            var query = $"SELECT COUNT(*) from {USERS_TABLE} WHERE LOWER(" + USER_NAME + ")=@" + USER_NAME +
                        ";";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + USER_NAME, accountname.ToLower().Trim()));
                count = (long) ExecuteScalar(cmd);
            }

            return (count > 0);
        }

        public static bool EmailInUse(string email)
        {
            long count = -1;
            var query = $"SELECT COUNT(*) from {USERS_TABLE} WHERE LOWER(" + USER_EMAIL + ")=@" +
                        USER_EMAIL + ";";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + USER_EMAIL, email.ToLower().Trim()));
                count = (long) ExecuteScalar(cmd);
            }

            return (count > 0);
        }

        public static bool CharacterNameInUse(string name)
        {
            long count = -1;
            var query = $"SELECT COUNT(*) from {CHAR_TABLE} WHERE LOWER(" + CHAR_NAME + ")=@" + CHAR_NAME +
                        " AND " + CHAR_DELETED + " = 0;";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + CHAR_NAME, name.ToLower().Trim()));
                count = (long) ExecuteScalar(cmd);
            }

            return (count > 0);
        }

        public static int GetCharacterId(string name)
        {
            var id = -1;
            var query = $"SELECT {CHAR_ID} from {CHAR_TABLE} WHERE LOWER(" + CHAR_NAME + ")=@" + CHAR_NAME +
                        " AND " + CHAR_DELETED + " = 0;";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + CHAR_NAME, name.ToLower().Trim()));
                using (var dataReader = ExecuteReader(cmd))
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        id = Convert.ToInt32(dataReader[CHAR_ID]);
                    }
                }
            }

            return id;
        }

        public static long GetRegisteredPlayers()
        {
            long count = -1;
            var cmd = $"SELECT COUNT(*) from {USERS_TABLE};";
            using (var createCommand = sDbConnection?.CreateCommand())
            {
                createCommand.CommandText = cmd;
                count = (long) ExecuteScalar(createCommand);
            }

            return count;
        }

        public static void CreateAccount(Client client, string username, string password, string email)
        {
            var sha = new SHA256Managed();
            client.MyAccount = username;

            //Generate a Salt
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[20];
            rng.GetBytes(buff);
            client.MySalt =
                BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(Convert.ToBase64String(buff))))
                    .Replace("-", "");

            //Hash the Password
            client.MyPassword =
                BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(password + client.MySalt)))
                    .Replace("-", "");

            client.MyEmail = email;

            if (GetRegisteredPlayers() == 0)
            {
                client.Power = 2;
            }
            client.MyId = SaveUser(client, true);
        }

        public static long SaveUser(Client client, bool newUser = false)
        {
            if (client == null) return -1;
            long rowId = -1;

            var insertQuery =
                $"INSERT into {USERS_TABLE} ({USER_NAME}, {USER_EMAIL}, {USER_PASS}, {USER_SALT}, {USER_POWER})" +
                $"VALUES (@{USER_NAME}, @{USER_EMAIL}, @{USER_PASS}, @{USER_SALT}, @{USER_POWER}); SELECT last_insert_rowid();";

            var updateQuery = $"UPDATE {USERS_TABLE} SET " +
                              $"{USER_NAME}=@{USER_NAME}, " +
                              $"{USER_EMAIL}=@{USER_EMAIL}, " +
                              $"{USER_PASS}=@{USER_PASS}, " +
                              $"{USER_SALT}=@{USER_SALT}, " +
                              $"{USER_POWER}=@{USER_POWER} " +
                              $"WHERE {USER_ID}=@{USER_ID};";

            using (var cmd = new SqliteCommand(newUser ? insertQuery : updateQuery, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + USER_NAME, client.MyAccount));
                cmd.Parameters.Add(new SqliteParameter("@" + USER_EMAIL, client.MyEmail));
                cmd.Parameters.Add(new SqliteParameter("@" + USER_PASS, client.MyPassword));
                cmd.Parameters.Add(new SqliteParameter("@" + USER_SALT, client.MySalt));
                cmd.Parameters.Add(new SqliteParameter("@" + USER_POWER, client.Power));
                if (!newUser) cmd.Parameters.Add(new SqliteParameter("@" + USER_ID, client.MyId));
                if (newUser)
                {
                    rowId = (int) ((long) ExecuteScalar(cmd));
                }
                else
                {
                    ExecuteNonQuery(cmd);
                    rowId = client.MyId;
                }
            }

            return (rowId);
        }

        public static bool CheckPassword(string username, string password)
        {
            var sha = new SHA256Managed();
            var query = "SELECT " + USER_SALT + "," + USER_PASS + " from " + USERS_TABLE + " WHERE LOWER(" +
                        USER_NAME + ")=@" + USER_NAME + ";";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + USER_NAME, username.ToLower().Trim()));
                using (var dataReader = ExecuteReader(cmd))
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        var pass = dataReader[USER_PASS].ToString();
                        var salt = dataReader[USER_SALT].ToString();
                        var temppass =
                            BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(password + salt)))
                                .Replace("-", "");
                        if (temppass == pass)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static long CheckPower(string username)
        {
            long power = 0;
            var query = $"SELECT {USER_POWER} from {USERS_TABLE} WHERE LOWER(" + USER_NAME + ")=@" +
                        USER_NAME + ";";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + USER_NAME, username.ToLower().Trim()));
                power = (long) ExecuteScalar(cmd);
            }

            return power;
        }

        public static bool LoadUser(Client client)
        {
            var query = $"SELECT * from {USERS_TABLE} WHERE LOWER(" + USER_NAME + ")=@" + USER_NAME + ";";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + USER_NAME, client.MyAccount.ToLower().Trim()));
                using (var dataReader = ExecuteReader(cmd))
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        client.MyAccount = dataReader[USER_NAME].ToString();
                        client.MyPassword = dataReader[USER_PASS].ToString();
                        client.MySalt = dataReader[USER_SALT].ToString();
                        client.MyEmail = dataReader[USER_EMAIL].ToString();
                        client.Power = Convert.ToInt32(dataReader[USER_POWER]);
                        client.MyId = Convert.ToInt32(dataReader[USER_ID]);

                        return true;
                    }
                }
            }

            return false;
        }

        //Character Saving/Loading
        public static long SaveCharacter(Player player, bool newCharacter = false)
        {
            var startTime = Globals.System.GetTimeMs();
            if (player == null)
            {
                return -1;
            }
            if (player.MyClient.MyAccount == "") return -1;
            if (!newCharacter && player.MyId == -1) return -1;
            var insertQuery = $"INSERT into {CHAR_TABLE} (" + CHAR_USER_ID + "," + CHAR_NAME + "," + CHAR_MAP +
                              "," + CHAR_X + "," + CHAR_Y + "," + CHAR_Z + "," + CHAR_DIR + "," + CHAR_SPRITE + "," +
                              CHAR_FACE + "," + CHAR_CLASS + "," + CHAR_GENDER + "," + CHAR_LEVEL + "," + CHAR_EXP +
                              "," + CHAR_VITALS + "," + CHAR_MAX_VITALS + "," + CHAR_STATS + "," + CHAR_STAT_POINTS +
                              "," + CHAR_EQUIPMENT + "," + CHAR_DELETED + "," + CHAR_LAST_ONLINE_TIME + ")" +
                              " VALUES (@" + CHAR_USER_ID + ",@" + CHAR_NAME + ",@" +
                              CHAR_MAP + ",@" + CHAR_X + ",@" + CHAR_Y + ",@" + CHAR_Z + ",@" + CHAR_DIR + ",@" +
                              CHAR_SPRITE + ",@" + CHAR_FACE + ",@" + CHAR_CLASS + ",@" + CHAR_GENDER + ",@" +
                              CHAR_LEVEL + ",@" + CHAR_EXP + ",@" + CHAR_VITALS + ",@" + CHAR_MAX_VITALS + ",@" +
                              CHAR_STATS + ",@" + CHAR_STAT_POINTS + ",@" + CHAR_EQUIPMENT + ",0,@" +
                              CHAR_LAST_ONLINE_TIME + ");SELECT last_insert_rowid();";

            var updateQuery = "UPDATE " + CHAR_TABLE + " SET " + CHAR_USER_ID + "=@" + CHAR_USER_ID +
                              "," + CHAR_NAME + "=@" + CHAR_NAME + "," + CHAR_MAP + "=@" + CHAR_MAP + "," +
                              CHAR_X + "=@" + CHAR_X + "," + CHAR_Y + "=@" + CHAR_Y + "," + CHAR_Z + "=@" + CHAR_Z +
                              "," + CHAR_DIR +
                              "=@" + CHAR_DIR + "," + CHAR_SPRITE + "=@" + CHAR_SPRITE + "," + CHAR_FACE + "=@" +
                              CHAR_FACE + "," + CHAR_CLASS + "=@" + CHAR_CLASS + "," + CHAR_GENDER + "=@" +
                              CHAR_GENDER + "," + CHAR_LEVEL + "=@" + CHAR_LEVEL + "," + CHAR_EXP + "=@" + CHAR_EXP +
                              "," + CHAR_VITALS + "=@" + CHAR_VITALS + "," + CHAR_MAX_VITALS + "=@" +
                              CHAR_MAX_VITALS + "," + CHAR_STATS + "=@" + CHAR_STATS + "," + CHAR_STAT_POINTS + "=@" +
                              CHAR_STAT_POINTS + "," + CHAR_EQUIPMENT + "=@" + CHAR_EQUIPMENT + "," +
                              CHAR_LAST_ONLINE_TIME
                              + "=@" + CHAR_LAST_ONLINE_TIME + " WHERE " + CHAR_ID + "=@" + CHAR_ID +
                              ";SELECT last_insert_rowid();";
            long rowId = -1;
            using (var transaction = sDbConnection?.BeginTransaction())
            {
                using (var cmd = new SqliteCommand(newCharacter ? insertQuery : updateQuery, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_USER_ID, player.MyClient.MyId));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_NAME, player.MyName));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_MAP, player.CurrentMap));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_X, player.CurrentX));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_Y, player.CurrentY));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_Z, player.CurrentZ));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_DIR, player.Dir));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_SPRITE, player.MySprite));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_FACE, player.Face));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_CLASS, player.Class));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_GENDER, player.Gender));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_LEVEL, player.Level));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_EXP, player.Experience));
                    var vitals = "";
                    for (var i = 0; i < player.Vital.Length; i++)
                    {
                        vitals += player.Vital[i] + ",";
                    }
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_VITALS, vitals));
                    var maxVitals = "";
                    for (var i = 0; i < player.MaxVital.Length; i++)
                    {
                        maxVitals += player.MaxVital[i] + ",";
                    }
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_MAX_VITALS, maxVitals));
                    var stats = "";
                    for (var i = 0; i < player.Stat.Length; i++)
                    {
                        stats += player.Stat[i].Stat + ",";
                    }
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_STATS, stats));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_STAT_POINTS, player.StatPoints));
                    var equipment = "";
                    for (var i = 0; i < player.Equipment.Length; i++)
                    {
                        equipment += player.Equipment[i] + ",";
                    }
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_EQUIPMENT, equipment));
                    if (!newCharacter) cmd.Parameters.Add(new SqliteParameter("@" + CHAR_ID, player.MyId));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_LAST_ONLINE_TIME, DateTime.UtcNow.ToBinary()));
                    rowId = (int) ((long) ExecuteScalar(cmd));
                }
                if (newCharacter) player.MyId = rowId;
                SaveCharacterInventory(player);
                SaveCharacterSpells(player);
                SaveCharacterBank(player);
                SaveCharacterHotbar(player);
                SaveCharacterSwitches(player);
                SaveCharacterVariables(player);
                SaveCharacterQuests(player);
                SaveCharacterFriends(player);
                transaction.Commit();
            }
            if (!newCharacter)
                PacketSender.SendPlayerMsg(player.MyClient, Strings.Get("player", "saved"));
            return (rowId);
        }

        private static void SaveCharacterInventory(Player player)
        {
            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var query = "INSERT OR REPLACE into " + CHAR_INV_TABLE + " (" + CHAR_INV_CHAR_ID + "," +
                            CHAR_INV_SLOT + "," + CHAR_INV_ITEM_NUM + "," + CHAR_INV_ITEM_VAL + "," +
                            CHAR_INV_ITEM_STATS + "," + CHAR_INV_ITEM_BAG_ID + ")" + " VALUES " + " (@" +
                            CHAR_INV_CHAR_ID + ",@" + CHAR_INV_SLOT +
                            ",@" + CHAR_INV_ITEM_NUM + ",@" + CHAR_INV_ITEM_VAL + ",@" + CHAR_INV_ITEM_STATS + ",@" +
                            CHAR_INV_ITEM_BAG_ID + ")";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_INV_CHAR_ID, player.MyId));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_INV_SLOT, i));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_INV_ITEM_NUM, player.Inventory[i].ItemNum));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_INV_ITEM_VAL, player.Inventory[i].ItemVal));
                    var stats = "";
                    for (var x = 0; x < player.Inventory[i].StatBoost.Length; x++)
                    {
                        stats += player.Inventory[i].StatBoost[x] + ",";
                    }
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_INV_ITEM_STATS, stats));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_INV_ITEM_BAG_ID, player.Inventory[i].BagId));
                    ExecuteNonQuery(cmd);
                }
            }
        }

        private static void SaveCharacterSpells(Player player)
        {
            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                var query = "INSERT OR REPLACE into " + CHAR_SPELL_TABLE + " (" + CHAR_SPELL_CHAR_ID + "," +
                            CHAR_SPELL_SLOT + "," + CHAR_SPELL_NUM + "," + CHAR_SPELL_CD + ")" + " VALUES " + " (@" +
                            CHAR_SPELL_CHAR_ID + ",@" + CHAR_SPELL_SLOT + ",@" + CHAR_SPELL_NUM + ",@" +
                            CHAR_SPELL_CD + ");";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_SPELL_CHAR_ID, player.MyId));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_SPELL_SLOT, i));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_SPELL_NUM, player.Spells[i].SpellNum));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_SPELL_CD,
                        (player.Spells[i].SpellCD > Globals.System.GetTimeMs()
                            ? Globals.System.GetTimeMs() - player.Spells[i].SpellCD
                            : 0)));
                    ExecuteNonQuery(cmd);
                }
            }
        }

        private static void SaveCharacterBank(Player player)
        {
            for (var i = 0; i < Options.MaxBankSlots; i++)
            {
                var query = "INSERT OR REPLACE into " + CHAR_BANK_TABLE + " (" + CHAR_BANK_CHAR_ID + "," +
                            CHAR_BANK_SLOT + "," + CHAR_BANK_ITEM_NUM + "," + CHAR_BANK_ITEM_VAL + "," +
                            CHAR_BANK_ITEM_STATS + "," + CHAR_BANK_ITEM_BAG_ID + ")" + " VALUES " + " (@" +
                            CHAR_BANK_CHAR_ID + ",@" +
                            CHAR_BANK_SLOT + ",@" + CHAR_BANK_ITEM_NUM + ",@" + CHAR_BANK_ITEM_VAL + ",@" +
                            CHAR_BANK_ITEM_STATS + ",@" + CHAR_BANK_ITEM_BAG_ID + ");";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_BANK_CHAR_ID, player.MyId));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_BANK_SLOT, i));
                    if (player.Bank[i] != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter("@" + CHAR_BANK_ITEM_NUM, player.Bank[i].ItemNum));
                        cmd.Parameters.Add(new SqliteParameter("@" + CHAR_BANK_ITEM_VAL, player.Bank[i].ItemVal));
                        var stats = "";
                        for (var x = 0; x < player.Bank[i].StatBoost.Length; x++)
                        {
                            stats += player.Bank[i].StatBoost[x] + ",";
                        }
                        cmd.Parameters.Add(new SqliteParameter("@" + CHAR_BANK_ITEM_STATS, stats));
                        cmd.Parameters.Add(new SqliteParameter("@" + CHAR_BANK_ITEM_BAG_ID, player.Bank[i].BagId));
                    }
                    else
                    {
                        cmd.Parameters.Add(new SqliteParameter("@" + CHAR_BANK_ITEM_NUM, -1));
                        cmd.Parameters.Add(new SqliteParameter("@" + CHAR_BANK_ITEM_VAL, -1));
                        var stats = "";
                        for (var x = 0; x < Options.MaxStats; x++)
                        {
                            stats += "-1,";
                        }
                        cmd.Parameters.Add(new SqliteParameter("@" + CHAR_BANK_ITEM_STATS, stats));
                        cmd.Parameters.Add(new SqliteParameter("@" + CHAR_BANK_ITEM_BAG_ID, -1));
                    }
                    ExecuteNonQuery(cmd);
                }
            }
        }

        private static void SaveCharacterHotbar(Player player)
        {
            for (var i = 0; i < Options.MaxHotbar; i++)
            {
                var query = "INSERT OR REPLACE into " + CHAR_HOTBAR_TABLE + " (" + CHAR_HOTBAR_CHAR_ID + "," +
                            CHAR_HOTBAR_SLOT + "," + CHAR_HOTBAR_TYPE + "," + CHAR_HOTBAR_ITEMSLOT + ")" +
                            " VALUES " + " (@" + CHAR_HOTBAR_CHAR_ID + ",@" + CHAR_HOTBAR_SLOT + ",@" +
                            CHAR_HOTBAR_TYPE + ",@" + CHAR_HOTBAR_ITEMSLOT + ");";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_HOTBAR_CHAR_ID, player.MyId));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_HOTBAR_SLOT, i));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_HOTBAR_TYPE, player.Hotbar[i].Type));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_HOTBAR_ITEMSLOT, player.Hotbar[i].Slot));
                    ExecuteNonQuery(cmd);
                }
            }
        }

        private static void SaveCharacterSwitches(Player player)
        {
            foreach (var playerSwitch in player.Switches)
            {
                var query = "INSERT OR REPLACE into " + CHAR_SWITCHES_TABLE + " (" + CHAR_SWITCH_CHAR_ID + "," +
                            CHAR_SWITCH_SLOT + "," + CHAR_SWITCH_VAL + ")" + " VALUES " + " (@" +
                            CHAR_SWITCH_CHAR_ID + ",@" + CHAR_SWITCH_SLOT + ",@" + CHAR_SWITCH_VAL + ");";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_SWITCH_CHAR_ID, player.MyId));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_SWITCH_SLOT, playerSwitch.Key));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_SWITCH_VAL,
                        Convert.ToInt32(playerSwitch.Value)));
                    ExecuteNonQuery(cmd);
                }
            }
        }

        private static void SaveCharacterVariables(Player player)
        {
            foreach (var playerVariable in player.Variables)
            {
                var query = "INSERT OR REPLACE into " + CHAR_VARIABLES_TABLE + " (" + CHAR_VARIABLE_CHAR_ID + "," +
                            CHAR_VARIABLE_SLOT + "," + CHAR_VARIABLE_VAL + ")" + " VALUES " + " (@" +
                            CHAR_VARIABLE_CHAR_ID + ",@" + CHAR_VARIABLE_SLOT + ",@" + CHAR_VARIABLE_VAL + ");";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_VARIABLE_CHAR_ID, player.MyId));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_VARIABLE_SLOT, playerVariable.Key));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_VARIABLE_VAL,
                        Convert.ToInt32(playerVariable.Value)));
                    ExecuteNonQuery(cmd);
                }
            }
        }

        private static void SaveCharacterQuests(Player player)
        {
            foreach (var playerQuest in player.Quests)
            {
                var query = "INSERT OR REPLACE into " + CHAR_QUESTS_TABLE + " (" + CHAR_QUEST_CHAR_ID + "," +
                            CHAR_QUEST_ID + "," + CHAR_QUEST_TASK + "," + CHAR_QUEST_TASK_PROGRESS + "," +
                            CHAR_QUEST_COMPLETED + ")" + " VALUES " + " (@" + CHAR_QUEST_CHAR_ID + ",@" +
                            CHAR_QUEST_ID + ",@" + CHAR_QUEST_TASK + ",@" + CHAR_QUEST_TASK_PROGRESS + ",@" +
                            CHAR_QUEST_COMPLETED + ");";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_QUEST_CHAR_ID, player.MyId));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_QUEST_ID, playerQuest.Key));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_QUEST_TASK,
                        Convert.ToInt32(playerQuest.Value.task)));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_QUEST_TASK_PROGRESS,
                        Convert.ToInt32(playerQuest.Value.taskProgress)));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_QUEST_COMPLETED,
                        Convert.ToInt32(playerQuest.Value.completed)));
                    ExecuteNonQuery(cmd);
                }
            }
        }

        private static void SaveCharacterFriends(Player player)
        {
            foreach (var friend in player.Friends)
            {
                var query = "INSERT OR REPLACE into " + CHAR_FRIENDS_TABLE + " (" + CHAR_FRIEND_CHAR_ID + "," +
                            CHAR_FRIEND_ID + ")" + " VALUES " + " (@" + CHAR_FRIEND_CHAR_ID + ",@" +
                            CHAR_FRIEND_ID + ");";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_FRIEND_CHAR_ID, player.MyId));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_FRIEND_ID, friend.Key));
                    ExecuteNonQuery(cmd);
                }
            }
        }

        public static void GetCharacters(Client client)
        {
            var commaSep = new char[1];
            commaSep[0] = ',';
            try
            {
                client.Characters.Clear();
                var query = "SELECT " + CHAR_ID + "," + CHAR_NAME + "," + CHAR_SPRITE + "," + CHAR_GENDER + "," +
                            CHAR_FACE + "," + CHAR_LEVEL + "," + CHAR_CLASS + "," + CHAR_EQUIPMENT + "," +
                            CHAR_LAST_ONLINE_TIME + " FROM " + CHAR_TABLE + " WHERE " + CHAR_USER_ID + "=@" +
                            CHAR_USER_ID + " AND " + CHAR_DELETED + " = 0 ORDER BY " + CHAR_LAST_ONLINE_TIME +
                            " DESC LIMIT " + Options.MaxCharacters + ";";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_USER_ID, client.MyId));
                    using (var dataReader = ExecuteReader(cmd))
                    {
                        while (dataReader.Read())
                        {
                            var character = new Character(Convert.ToInt32(dataReader[CHAR_ID]),
                                dataReader[CHAR_NAME].ToString(), dataReader[CHAR_SPRITE].ToString(),
                                dataReader[CHAR_FACE].ToString(),
                                Convert.ToInt32(dataReader[CHAR_LEVEL]), Convert.ToInt32(dataReader[CHAR_CLASS]));
                            var equipmentString = dataReader[CHAR_EQUIPMENT].ToString()
                                .Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                            var equipmentArray = new int[Options.EquipmentSlots.Count];
                            for (var i = 0; i < (int) Options.EquipmentSlots.Count && i < equipmentString.Length; i++)
                            {
                                equipmentArray[i] = int.Parse(equipmentString[i]);
                            }
                            //Draw the equipment/paperdolls
                            for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
                            {
                                if (Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z]) > -1)
                                {
                                    if (equipmentArray[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] >
                                        -1 && equipmentArray[
                                            Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] <
                                        Options.MaxInvItems)
                                    {
                                        var itemNum = GetCharacterInventoryItem(character.Slot,
                                            equipmentArray[
                                                Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])]);

                                        if (ItemBase.Lookup.Get<ItemBase>(itemNum) != null)
                                        {
                                            var itemdata = ItemBase.Lookup.Get<ItemBase>(itemNum);
                                            if (Convert.ToInt32(dataReader[CHAR_GENDER]) == 0)
                                            {
                                                character.Equipment[z] = itemdata.MalePaperdoll;
                                            }
                                            else
                                            {
                                                character.Equipment[z] = itemdata.FemalePaperdoll;
                                            }
                                        }
                                    }
                                }
                            }
                            client.Characters.Add(character);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool LoadCharacter(Client client, int id)
        {
            var en = client.Entity;
            var commaSep = new char[1];
            commaSep[0] = ',';
            if (client.MyId == -1) return false;
            try
            {
                var query = $"SELECT * from {CHAR_TABLE} WHERE " + CHAR_USER_ID + "=@" + CHAR_USER_ID + " AND " +
                            CHAR_ID + "=@" + CHAR_ID + ";";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_USER_ID, client.MyId));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_ID, id));
                    using (var dataReader = ExecuteReader(cmd))
                    {
                        if (dataReader.HasRows && dataReader.Read())
                        {
                            en.MyId = Convert.ToInt32(dataReader[CHAR_ID]);
                            en.MyName = dataReader[CHAR_NAME].ToString();
                            en.CurrentMap = Convert.ToInt32(dataReader[CHAR_MAP]);
                            en.CurrentX = Convert.ToInt32(dataReader[CHAR_X]);
                            en.CurrentY = Convert.ToInt32(dataReader[CHAR_Y]);
                            en.CurrentZ = Convert.ToInt32(dataReader[CHAR_Z]);
                            en.Dir = Convert.ToInt32(dataReader[CHAR_DIR]);
                            en.MySprite = dataReader[CHAR_SPRITE].ToString();
                            en.Face = dataReader[CHAR_FACE].ToString();
                            en.Class = Convert.ToInt32(dataReader[CHAR_CLASS]);
                            en.Gender = Convert.ToInt32(dataReader[CHAR_GENDER]);
                            en.Level = Convert.ToInt32(dataReader[CHAR_LEVEL]);
                            en.Experience = Convert.ToInt32(dataReader[CHAR_EXP]);
                            var vitalString = dataReader[CHAR_VITALS].ToString();
                            var vitals = vitalString.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                            for (var i = 0; i < (int) Vitals.VitalCount && i < vitals.Length; i++)
                            {
                                en.Vital[i] = int.Parse(vitals[i]);
                            }
                            var maxVitalString = dataReader[CHAR_MAX_VITALS].ToString();
                            var maxVitals = maxVitalString.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                            for (var i = 0; i < (int) Vitals.VitalCount && i < maxVitals.Length; i++)
                            {
                                en.MaxVital[i] = int.Parse(maxVitals[i]);
                            }
                            var statsString = dataReader[CHAR_STATS].ToString();
                            var stats = statsString.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                            for (var i = 0; i < (int) Stats.StatCount && i < stats.Length; i++)
                            {
                                en.Stat[i].Stat = int.Parse(stats[i]);
                                if (en.Stat[i].Stat > Options.MaxStatValue) en.Stat[i].Stat = Options.MaxStatValue;
                            }
                            en.StatPoints = Convert.ToInt32(dataReader[CHAR_STAT_POINTS]);
                            var equipmentString = dataReader[CHAR_EQUIPMENT].ToString();
                            var equipment = equipmentString.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                            for (var i = 0; i < (int) Options.EquipmentSlots.Count && i < equipment.Length; i++)
                            {
                                en.Equipment[i] = int.Parse(equipment[i]);
                            }
                            if (!LoadCharacterInventory(en)) return false;
                            if (!LoadCharacterSpells(en)) return false;
                            if (!LoadCharacterBank(en)) return false;
                            if (!LoadCharacterHotbar(en)) return false;
                            if (!LoadCharacterSwitches(en)) return false;
                            if (!LoadCharacterVariables(en)) return false;
                            if (!LoadCharacterQuests(en)) return false;
                            if (!LoadCharacterFriends(en)) return false;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }

        public static void DeleteCharacter(Client client, int id)
        {
            var commaSep = new char[1];
            commaSep[0] = ',';
            if (client.MyId == -1) return;
            try
            {
                var query = "UPDATE " + CHAR_TABLE + " SET " + CHAR_DELETED + " = 1 WHERE " + CHAR_USER_ID + "=@" +
                            CHAR_USER_ID + " AND " + CHAR_ID + "=@" + CHAR_ID + ";";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_USER_ID, client.MyId));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_ID, id));
                    ExecuteNonQuery(cmd);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }

        private static bool LoadCharacterInventory(Player player)
        {
            var commaSep = new char[1];
            commaSep[0] = ',';
            try
            {
                var query = $"SELECT * from {CHAR_INV_TABLE} WHERE " + CHAR_INV_CHAR_ID + "=@" +
                            CHAR_INV_CHAR_ID + ";";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_INV_CHAR_ID, player.MyId));
                    using (var dataReader = ExecuteReader(cmd))
                    {
                        while (dataReader.Read())
                        {
                            var slot = Convert.ToInt32(dataReader[CHAR_INV_SLOT]);
                            if (slot >= 0 && slot < Options.MaxInvItems)
                            {
                                player.Inventory[slot].ItemNum = Convert.ToInt32(dataReader[CHAR_INV_ITEM_NUM]);
                                player.Inventory[slot].ItemVal = Convert.ToInt32(dataReader[CHAR_INV_ITEM_VAL]);
                                var statBoostStr = dataReader[CHAR_INV_ITEM_STATS].ToString();
                                var stats = statBoostStr.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                                for (var i = 0; i < (int) Stats.StatCount && i < stats.Length; i++)
                                {
                                    player.Inventory[slot].StatBoost[i] = int.Parse(stats[i]);
                                }
                                if (ItemBase.Lookup.Get<ItemBase>(player.Inventory[slot].ItemNum) == null)
                                {
                                    player.Inventory[slot].ItemNum = -1;
                                    player.Inventory[slot].ItemVal = 0;
                                    for (var i = 0; i < (int) Stats.StatCount && i < stats.Length; i++)
                                    {
                                        player.Inventory[slot].StatBoost[i] = 0;
                                    }
                                }
                                player.Inventory[slot].BagId = Convert.ToInt32(dataReader[CHAR_INV_ITEM_BAG_ID]);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static int GetCharacterInventoryItem(int charId, int invSlot)
        {
            var itemId = -1;
            try
            {
                var query = $"SELECT * from {CHAR_INV_TABLE} WHERE " + CHAR_INV_CHAR_ID + "=@" +
                            CHAR_INV_CHAR_ID + " AND " + CHAR_INV_SLOT + "=@" + CHAR_INV_SLOT + ";";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_INV_CHAR_ID, charId));
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_INV_SLOT, invSlot));
                    using (var dataReader = ExecuteReader(cmd))
                    {
                        while (dataReader.Read())
                        {
                            var slot = Convert.ToInt32(dataReader[CHAR_INV_SLOT]);
                            if (slot >= 0 && slot < Options.MaxInvItems && slot == invSlot)
                            {
                                return Convert.ToInt32(dataReader[CHAR_INV_ITEM_NUM]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemId;
        }

        private static bool LoadCharacterSpells(Player player)
        {
            try
            {
                var query = $"SELECT * from {CHAR_SPELL_TABLE} WHERE " + CHAR_SPELL_CHAR_ID + "=@" +
                            CHAR_SPELL_CHAR_ID + ";";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_SPELL_CHAR_ID, player.MyId));
                    using (var dataReader = ExecuteReader(cmd))
                    {
                        while (dataReader.Read())
                        {
                            var slot = Convert.ToInt32(dataReader[CHAR_SPELL_SLOT]);
                            if (slot >= 0 && slot < Options.MaxPlayerSkills)
                            {
                                player.Spells[slot].SpellNum = Convert.ToInt32(dataReader[CHAR_SPELL_NUM]);
                                player.Spells[slot].SpellCD = Globals.System.GetTimeMs() +
                                                              Convert.ToInt32(dataReader[CHAR_SPELL_CD]);
                                if (SpellBase.Lookup.Get<SpellBase>(player.Spells[slot].SpellNum) == null)
                                {
                                    player.Spells[slot].SpellNum = -1;
                                    player.Spells[slot].SpellCD = -1;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool LoadCharacterBank(Player player)
        {
            var commaSep = new char[1];
            commaSep[0] = ',';
            try
            {
                var query = $"SELECT * from {CHAR_BANK_TABLE} WHERE " + CHAR_BANK_CHAR_ID + "=@" +
                            CHAR_BANK_CHAR_ID + ";";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_BANK_CHAR_ID, player.MyId));
                    using (var dataReader = ExecuteReader(cmd))
                    {
                        while (dataReader.Read())
                        {
                            var slot = Convert.ToInt32(dataReader[CHAR_BANK_SLOT]);
                            if (slot >= 0 && slot < Options.MaxBankSlots)
                            {
                                if (player.Bank[slot] == null) player.Bank[slot] = new ItemInstance();
                                player.Bank[slot].ItemNum = Convert.ToInt32(dataReader[CHAR_BANK_ITEM_NUM]);
                                player.Bank[slot].ItemVal = Convert.ToInt32(dataReader[CHAR_BANK_ITEM_VAL]);
                                var statBoostStr = dataReader[CHAR_BANK_ITEM_STATS].ToString();
                                var stats = statBoostStr.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                                for (var i = 0; i < (int) Stats.StatCount && i < stats.Length; i++)
                                {
                                    player.Bank[slot].StatBoost[i] = int.Parse(stats[i]);
                                }
                                if (ItemBase.Lookup.Get<ItemBase>(player.Bank[slot].ItemNum) == null)
                                {
                                    player.Bank[slot].ItemNum = -1;
                                    player.Bank[slot].ItemVal = 0;
                                    for (var i = 0; i < (int) Stats.StatCount && i < stats.Length; i++)
                                    {
                                        player.Bank[slot].StatBoost[i] = 0;
                                    }
                                }
                                player.Bank[slot].BagId = Convert.ToInt32(dataReader[CHAR_BANK_ITEM_BAG_ID]);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool LoadCharacterHotbar(Player player)
        {
            try
            {
                var query = $"SELECT * from {CHAR_HOTBAR_TABLE} WHERE " + CHAR_HOTBAR_CHAR_ID + "=@" +
                            CHAR_HOTBAR_CHAR_ID + ";";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_HOTBAR_CHAR_ID, player.MyId));
                    using (var dataReader = ExecuteReader(cmd))
                    {
                        while (dataReader.Read())
                        {
                            var slot = Convert.ToInt32(dataReader[CHAR_HOTBAR_SLOT]);
                            if (slot >= 0 && slot < Options.MaxHotbar)
                            {
                                player.Hotbar[slot].Type = Convert.ToInt32(dataReader[CHAR_HOTBAR_TYPE]);
                                player.Hotbar[slot].Slot = Convert.ToInt32(dataReader[CHAR_HOTBAR_ITEMSLOT]);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool LoadCharacterSwitches(Player player)
        {
            try
            {
                var query = $"SELECT * from {CHAR_SWITCHES_TABLE} WHERE " + CHAR_SWITCH_CHAR_ID + "=@" +
                            CHAR_SWITCH_CHAR_ID + ";";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_SWITCH_CHAR_ID, player.MyId));
                    using (var dataReader = ExecuteReader(cmd))
                    {
                        while (dataReader.Read())
                        {
                            var id = Convert.ToInt32(dataReader[CHAR_SWITCH_SLOT]);
                            if (player.Switches.ContainsKey(id))
                            {
                                player.Switches[id] = Convert.ToBoolean(Convert.ToInt32(dataReader[CHAR_SWITCH_VAL]));
                            }
                            else
                            {
                                player.Switches.Add(id,
                                    Convert.ToBoolean(Convert.ToInt32(dataReader[CHAR_SWITCH_VAL])));
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool LoadCharacterVariables(Player player)
        {
            try
            {
                var query = $"SELECT * from {CHAR_VARIABLES_TABLE} WHERE " + CHAR_VARIABLE_CHAR_ID + "=@" +
                            CHAR_VARIABLE_CHAR_ID + ";";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_VARIABLE_CHAR_ID, player.MyId));
                    using (var dataReader = ExecuteReader(cmd))
                    {
                        while (dataReader.Read())
                        {
                            var id = Convert.ToInt32(dataReader[CHAR_VARIABLE_SLOT]);
                            if (player.Variables.ContainsKey(id))
                            {
                                player.Variables[id] = Convert.ToInt32(dataReader[CHAR_VARIABLE_VAL]);
                            }
                            else
                            {
                                player.Variables.Add(id, Convert.ToInt32(dataReader[CHAR_VARIABLE_VAL]));
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool LoadCharacterQuests(Player player)
        {
            try
            {
                var query = $"SELECT * from {CHAR_QUESTS_TABLE} WHERE " + CHAR_QUEST_CHAR_ID + "=@" +
                            CHAR_QUEST_CHAR_ID + ";";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_QUEST_CHAR_ID, player.MyId));
                    using (var dataReader = ExecuteReader(cmd))
                    {
                        while (dataReader.Read())
                        {
                            var id = Convert.ToInt32(dataReader[CHAR_QUEST_ID]);
                            if (player.Quests.ContainsKey(id))
                            {
                                var questProgress = player.Quests[id];
                                questProgress.task = Convert.ToInt32(dataReader[CHAR_QUEST_TASK]);
                                questProgress.taskProgress = Convert.ToInt32(dataReader[CHAR_QUEST_TASK_PROGRESS]);
                                questProgress.completed = Convert.ToInt32(dataReader[CHAR_QUEST_COMPLETED]);
                                player.Quests[id] = questProgress;
                            }
                            else
                            {
                                var questProgress = new QuestProgressStruct()
                                {
                                    task = Convert.ToInt32(dataReader[CHAR_QUEST_TASK]),
                                    taskProgress = Convert.ToInt32(dataReader[CHAR_QUEST_TASK_PROGRESS]),
                                    completed = Convert.ToInt32(dataReader[CHAR_QUEST_COMPLETED])
                                };
                                player.Quests.Add(id, questProgress);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool LoadCharacterFriends(Player player)
        {
            player.Friends.Clear();

            try
            {
                var query = "SELECT " + CHAR_TABLE + "." + CHAR_NAME + "," + CHAR_FRIENDS_TABLE + "." +
                            CHAR_FRIEND_ID + " FROM " + CHAR_FRIENDS_TABLE + " INNER JOIN " + CHAR_TABLE + " ON " +
                            CHAR_FRIENDS_TABLE + "." + CHAR_FRIEND_ID + " = " + CHAR_TABLE + "." + CHAR_ID + " WHERE " +
                            CHAR_FRIEND_CHAR_ID + "=@" + CHAR_FRIEND_CHAR_ID + ";";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + CHAR_FRIEND_CHAR_ID, player.MyId));
                    using (var dataReader = ExecuteReader(cmd))
                    {
                        while (dataReader.Read())
                        {
                            player.Friends.Add(Convert.ToInt32(dataReader[CHAR_FRIEND_ID]),
                                dataReader[CHAR_NAME].ToString());
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DeleteCharacterFriend(Player player, int key)
        {
            var insertQuery = "DELETE FROM " + CHAR_FRIENDS_TABLE + " WHERE " + CHAR_FRIEND_ID + "=@" + CHAR_FRIEND_ID +
                              " AND " + CHAR_ID + " = @" + CHAR_ID + ";";
            using (var cmd = new SqliteCommand(insertQuery, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + CHAR_FRIEND_ID, key));
                cmd.Parameters.Add(new SqliteParameter("@" + CHAR_ID, player.MyId));
                ExecuteNonQuery(cmd);
            }
        }

        //Bags
        public static int CreateBag(int slotCount)
        {
            var insertQuery = $"INSERT into {BAGS_TABLE} (" + BAG_SLOT_COUNT + ")" + "VALUES (@" + BAG_SLOT_COUNT +
                              ");SELECT last_insert_rowid();";
            var rowId = -1;
            using (var cmd = new SqliteCommand(insertQuery, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + BAG_SLOT_COUNT, slotCount));
                rowId = (int) ((long) ExecuteScalar(cmd));
            }
            return (int) (rowId);
        }

        public static void LoadBag(ItemInstance bagItem)
        {
            var commaSep = new char[1];
            commaSep[0] = ',';
            //Query the Bags table to get the number of slots...
            var query = $"SELECT * from {BAGS_TABLE} WHERE " + BAG_ID + " =@" + BAG_ID + ";";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + BAG_ID, bagItem.BagId));
                using (var dataReader = ExecuteReader(cmd))
                {
                    while (dataReader.Read())
                    {
                        var slots = Convert.ToInt32(dataReader[BAG_SLOT_COUNT]);
                        bagItem.BagInstance = new BagInstance(slots);
                    }
                }
            }
            if (bagItem.BagInstance != null)
            {
                //Then query the bag items table to get all the item data...
                query = $"SELECT * from {BAG_ITEMS_TABLE} WHERE " + BAG_ITEM_CONTAINER_ID + " = @" +
                        BAG_ITEM_CONTAINER_ID + ";";
                using (var cmd = new SqliteCommand(query, sDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + BAG_ITEM_CONTAINER_ID, bagItem.BagId));
                    using (var dataReader = ExecuteReader(cmd))
                    {
                        while (dataReader.Read())
                        {
                            var slot = Convert.ToInt32(dataReader[BAG_ITEM_SLOT]);
                            if (slot >= 0 && slot < bagItem.BagInstance.Slots)
                            {
                                bagItem.BagInstance.Items[slot].ItemNum = Convert.ToInt32(dataReader[BAG_ITEM_NUM]);
                                bagItem.BagInstance.Items[slot].ItemVal = Convert.ToInt32(dataReader[BAG_ITEM_VAL]);
                                var statBoostStr = dataReader[BAG_ITEM_STATS].ToString();
                                var stats = statBoostStr.Split(commaSep, StringSplitOptions.RemoveEmptyEntries);
                                for (var i = 0; i < (int) Stats.StatCount && i < stats.Length; i++)
                                {
                                    bagItem.BagInstance.Items[slot].StatBoost[i] = int.Parse(stats[i]);
                                }
                                if (ItemBase.Lookup.Get<ItemBase>(bagItem.BagInstance.Items[slot].ItemNum) == null)
                                {
                                    bagItem.BagInstance.Items[slot].ItemNum = -1;
                                    bagItem.BagInstance.Items[slot].ItemVal = 0;
                                    for (var i = 0; i < (int) Stats.StatCount && i < stats.Length; i++)
                                    {
                                        bagItem.BagInstance.Items[slot].StatBoost[i] = 0;
                                    }
                                }
                                bagItem.BagInstance.Items[slot].BagId = Convert.ToInt32(dataReader[BAG_ITEM_BAG_ID]);
                            }
                        }
                    }
                }
            }
        }

        public static bool BagEmpty(int bagId)
        {
            var bagItem = new ItemInstance(-1, 0, bagId);
            LoadBag(bagItem);
            for (var i = 0; i < bagItem.BagInstance.Slots; i++)
            {
                if (bagItem.BagInstance.Items[i] != null)
                {
                    var item = ItemBase.Lookup.Get<ItemBase>(bagItem.BagInstance.Items[i].ItemNum);
                    if (item != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void SaveBagItem(int bagId, int slot, ItemInstance bagItem)
        {
            var query = "INSERT OR REPLACE into " + BAG_ITEMS_TABLE + " (" + BAG_ITEM_CONTAINER_ID + "," +
                        BAG_ITEM_SLOT +
                        "," + BAG_ITEM_NUM + "," +
                        BAG_ITEM_VAL + "," + BAG_ITEM_STATS + "," + BAG_ITEM_BAG_ID + ")" + " VALUES " + " (@" +
                        BAG_ITEM_CONTAINER_ID + ",@" + BAG_ITEM_SLOT + ",@" + BAG_ITEM_NUM + ",@" + BAG_ITEM_VAL +
                        ",@" +
                        BAG_ITEM_STATS + ",@" + BAG_ITEM_BAG_ID + ");";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + BAG_ITEM_CONTAINER_ID, bagId));
                cmd.Parameters.Add(new SqliteParameter("@" + BAG_ITEM_SLOT, slot));
                if (bagItem != null)
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + BAG_ITEM_NUM, bagItem.ItemNum));
                    cmd.Parameters.Add(new SqliteParameter("@" + BAG_ITEM_VAL, bagItem.ItemVal));
                    var stats = "";
                    for (var x = 0; x < bagItem.StatBoost.Length; x++)
                    {
                        stats += bagItem.StatBoost[x] + ",";
                    }
                    cmd.Parameters.Add(new SqliteParameter("@" + BAG_ITEM_STATS, stats));
                }
                else
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + BAG_ITEM_NUM, -1));
                    cmd.Parameters.Add(new SqliteParameter("@" + BAG_ITEM_VAL, -1));
                    var stats = "";
                    for (var x = 0; x < Options.MaxStats; x++)
                    {
                        stats += "0,";
                    }
                    cmd.Parameters.Add(new SqliteParameter("@" + BAG_ITEM_STATS, stats));
                }
                cmd.Parameters.Add(new SqliteParameter("@" + BAG_ITEM_BAG_ID, bagId));
                ExecuteNonQuery(cmd);
            }
        }

        //Bans and Mutes
        public static void AddMute(Client player, int duration, string reason, string muter, string ip)
        {
            var query = "INSERT OR REPLACE into " + MUTE_TABLE + " (" + MUTE_ID + "," +
                        MUTE_TIME + "," + MUTE_USER + "," + MUTE_IP + "," + MUTE_DURATION + "," +
                        MUTE_REASON + "," + MUTE_MUTER + ")" + " VALUES " + " (@" +
                        MUTE_ID + ",@" + MUTE_TIME + ",@" + MUTE_USER + ",@" + MUTE_IP + ",@" +
                        MUTE_DURATION + ",@" + MUTE_REASON + ",@" + MUTE_MUTER + ");";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MUTE_ID, player.MyId));
                cmd.Parameters.Add(new SqliteParameter("@" + MUTE_TIME, DateTime.UtcNow.ToBinary()));
                cmd.Parameters.Add(new SqliteParameter("@" + MUTE_USER, player.MyAccount));
                cmd.Parameters.Add(new SqliteParameter("@" + MUTE_IP, ip));
                var t = DateTime.UtcNow.AddDays(duration);
                cmd.Parameters.Add(new SqliteParameter("@" + MUTE_DURATION, t.ToBinary()));
                cmd.Parameters.Add(new SqliteParameter("@" + MUTE_REASON, reason));
                cmd.Parameters.Add(new SqliteParameter("@" + MUTE_MUTER, muter));
                ExecuteNonQuery(cmd);
            }
        }

        public static void DeleteMute(string account)
        {
            var insertQuery = "DELETE FROM " + MUTE_TABLE + " WHERE " + MUTE_USER + "=@" + MUTE_USER + ";";
            using (var cmd = new SqliteCommand(insertQuery, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MUTE_USER, account));
                ExecuteNonQuery(cmd);
            }
        }

        public static string CheckMute(string account, string ip)
        {
            var query = "SELECT " + MUTE_DURATION + "," + MUTE_TIME + "," + MUTE_MUTER + "," + MUTE_REASON +
                        " from " + MUTE_TABLE + " WHERE (LOWER(" + MUTE_USER + ")=@" + MUTE_USER + ((ip.Trim().Length > 0) ?  " OR " + MUTE_IP +
                        "=@" + MUTE_IP : "") + ")" + ";";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MUTE_USER, account.ToLower().Trim()));
                cmd.Parameters.Add(new SqliteParameter("@" + MUTE_IP, ip.Trim()));
                using (var dataReader = ExecuteReader(cmd))
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        var duration = DateTime.FromBinary(Convert.ToInt64(dataReader[MUTE_DURATION]));
                        var banStart = DateTime.FromBinary(Convert.ToInt64(dataReader[MUTE_TIME]));
                        var banner = Convert.ToString(dataReader[MUTE_MUTER]);
                        var reason = Convert.ToString(dataReader[MUTE_REASON]);
                        if (duration.CompareTo(DateTime.Today) <= 0) //Check that enough time has passed
                        {
                            DeleteMute(account);
                            return null;
                        }
                        else
                        {
                            return Strings.Get("account", "mutestatus", banStart, banner, duration, reason);
                        }
                    }
                }

                return null;
            }
        }

        public static void AddBan(Client player, int duration, string reason, string banner, string ip)
        {
            var query = "INSERT OR REPLACE into " + BAN_TABLE + " (" + BAN_ID + "," +
                        BAN_TIME + "," + BAN_USER + "," + BAN_IP + "," + BAN_DURATION + "," +
                        BAN_REASON + "," + BAN_BANNER + ")" + " VALUES " + " (@" +
                        BAN_ID + ",@" + BAN_TIME + ",@" + BAN_USER + ",@" + BAN_IP + ",@" +
                        BAN_DURATION + ",@" + BAN_REASON + ",@" + BAN_BANNER + ");";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + BAN_ID, player.MyId));
                cmd.Parameters.Add(new SqliteParameter("@" + BAN_TIME, DateTime.UtcNow.ToBinary()));
                cmd.Parameters.Add(new SqliteParameter("@" + BAN_USER, player.MyAccount));
                cmd.Parameters.Add(new SqliteParameter("@" + BAN_IP, ip));
                var t = DateTime.UtcNow.AddDays(duration);
                cmd.Parameters.Add(new SqliteParameter("@" + BAN_DURATION, t.ToBinary()));
                cmd.Parameters.Add(new SqliteParameter("@" + BAN_REASON, reason));
                cmd.Parameters.Add(new SqliteParameter("@" + BAN_BANNER, banner));
                ExecuteNonQuery(cmd);
            }
        }

        public static void DeleteBan(string account)
        {
            var insertQuery = "DELETE FROM " + BAN_TABLE + " WHERE " + BAN_USER + "=@" + BAN_USER + ";";
            using (var cmd = new SqliteCommand(insertQuery, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + BAN_USER, account));
                ExecuteNonQuery(cmd);
            }
        }

        public static string CheckBan(string account, string ip)
        {
            var query = "SELECT " + BAN_DURATION + "," + BAN_TIME + "," + BAN_BANNER + "," + BAN_REASON +
                        " from " + BAN_TABLE + " WHERE (LOWER(" + BAN_USER + ")=@" + BAN_USER + (ip.Trim().Length > 0 ? (" OR " + BAN_IP + "=@" +
                        BAN_IP) : "") + ")" + ";";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + BAN_USER, account.ToLower().Trim()));
                cmd.Parameters.Add(new SqliteParameter("@" + BAN_IP, ip.Trim()));
                using (var dataReader = ExecuteReader(cmd))
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        var duration = DateTime.FromBinary(Convert.ToInt64(dataReader[BAN_DURATION]));
                        var banStart = DateTime.FromBinary(Convert.ToInt64(dataReader[BAN_TIME]));
                        var banner = Convert.ToString(dataReader[BAN_BANNER]);
                        var reason = Convert.ToString(dataReader[BAN_REASON]);
                        if (duration.CompareTo(DateTime.Today) <= 0) //Check that enough time has passed
                        {
                            DeleteBan(account);
                            return null;
                        }
                        else
                        {
                            return Strings.Get("account", "banstatus", banStart, banner, duration, reason);
                        }
                    }
                }

                return null;
            }
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

        private static void LoadGameObject(GameObjectType type, int index, byte[] data)
        {
            switch (type)
            {
                case GameObjectType.Animation:
                    var anim = new AnimationBase(index);
                    anim.Load(data);
                    AnimationBase.Lookup.Set(index, anim);
                    break;
                case GameObjectType.Class:
                    var cls = new ClassBase(index);
                    cls.Load(data);
                    ClassBase.Lookup.Set(index, cls);
                    break;
                case GameObjectType.Item:
                    var itm = new ItemBase(index);
                    itm.Load(data);
                    ItemBase.Lookup.Set(index, itm);
                    break;
                case GameObjectType.Npc:
                    var npc = new NpcBase(index);
                    npc.Load(data);
                    NpcBase.Lookup.Set(index, npc);
                    break;
                case GameObjectType.Projectile:
                    var proj = new ProjectileBase(index);
                    proj.Load(data);
                    ProjectileBase.Lookup.Set(index, proj);
                    break;
                case GameObjectType.Quest:
                    var qst = new QuestBase(index);
                    qst.Load(data);
                    QuestBase.Lookup.Set(index, qst);
                    break;
                case GameObjectType.Resource:
                    var res = new ResourceBase(index);
                    res.Load(data);
                    ResourceBase.Lookup.Set(index, res);
                    break;
                case GameObjectType.Shop:
                    var shp = new ShopBase(index);
                    shp.Load(data);
                    ShopBase.Lookup.Set(index, shp);
                    break;
                case GameObjectType.Spell:
                    var spl = new SpellBase(index);
                    spl.Load(data);
                    SpellBase.Lookup.Set(index, spl);
                    break;
                case GameObjectType.Bench:
                    var cft = new BenchBase(index);
                    cft.Load(data);
                    BenchBase.Lookup.Set(index, cft);
                    break;
                case GameObjectType.Map:
                    var map = new MapInstance(index);
                    MapInstance.Lookup.Set(index, map);
                    map.Load(data);
                    break;
                case GameObjectType.CommonEvent:
                    var buffer = new ByteBuffer();
                    buffer.WriteBytes(data);
                    var evt = new EventBase(index, buffer, true);
                    EventBase.Lookup.Set(index, evt);
                    buffer.Dispose();
                    break;
                case GameObjectType.PlayerSwitch:
                    var pswitch = new PlayerSwitchBase(index);
                    pswitch.Load(data);
                    PlayerSwitchBase.Lookup.Set(index, pswitch);
                    break;
                case GameObjectType.PlayerVariable:
                    var pvar = new PlayerVariableBase(index);
                    pvar.Load(data);
                    PlayerVariableBase.Lookup.Set(index, pvar);
                    break;
                case GameObjectType.ServerSwitch:
                    var sswitch = new ServerSwitchBase(index);
                    sswitch.Load(data);
                    ServerSwitchBase.Lookup.Set(index, sswitch);
                    break;
                case GameObjectType.ServerVariable:
                    var svar = new ServerVariableBase(index);
                    svar.Load(data);
                    ServerVariableBase.Lookup.Set(index, svar);
                    break;
                case GameObjectType.Tileset:
                    var tset = new TilesetBase(index);
                    tset.Load(data);
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
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 0.ToString()));
                using (var dataReader = ExecuteReader(cmd))
                {
                    while (dataReader.Read())
                    {
                        var index = Convert.ToInt32(dataReader[GAME_OBJECT_ID]);
                        if (dataReader[GAME_OBJECT_DATA].GetType() != typeof(DBNull))
                        {
                            var data = (byte[]) dataReader[GAME_OBJECT_DATA];
                            if (data.Length > 1)
                            {
                                LoadGameObject(gameObjectType, index, (byte[]) dataReader[GAME_OBJECT_DATA]);
                            }
                        }
                        else
                        {
                            nullIssues += Strings.Get("database", "nullfound", index, tableName) + Environment.NewLine;
                        }
                    }
                }
            }
            if (nullIssues != "")
            {
                throw (new Exception(Strings.Get("database", "nullerror") + Environment.NewLine + nullIssues));
            }
        }

        public static void SaveGameObject(IDatabaseObject gameObject)
        {
            if (gameObject == null)
            {
                Log.Error("Attempted to persist null game object to the database.");
            }

            var insertQuery = "UPDATE " + gameObject.DatabaseTable + " set " + GAME_OBJECT_DELETED + "=@" +
                              GAME_OBJECT_DELETED + "," + GAME_OBJECT_DATA + "=@" + GAME_OBJECT_DATA + " WHERE " +
                              GAME_OBJECT_ID + "=@" + GAME_OBJECT_ID + ";";
            using (var cmd = new SqliteCommand(insertQuery, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_ID, gameObject.Index));
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, "0"));
                if (gameObject.BinaryData != null)
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DATA, gameObject.BinaryData));
                    try
                    {
                        var returnVal = ExecuteNonQuery(cmd);
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
            using (var cmd = new SqliteCommand(insertQuery, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DATA, new byte[1]));
                index = (int) ((long) ExecuteScalar(cmd));
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
                        var objf = new EventBase(index, -1, -1, true);
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
            using (var cmd = new SqliteCommand(insertQuery, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_ID, gameObject.Index));
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 1.ToString()));
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DATA, gameObject.BinaryData));
                ExecuteNonQuery(cmd);
            }
            gameObject.Delete();
        }

        //Map Tiles Saving/Loading
        public static byte[] GetMapTiles(int index)
        {
            var nullIssues = "";
            var query = $"SELECT * from {MAP_TILES_TABLE} WHERE " + MAP_TILES_MAP_ID + "=@" + MAP_TILES_MAP_ID +
                        ";";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_TILES_MAP_ID, index));
                using (var dataReader = ExecuteReader(cmd))
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        if (dataReader[MAP_TILES_DATA].GetType() != typeof(DBNull))
                        {
                            return (byte[]) dataReader[MAP_TILES_DATA];
                        }
                        else
                        {
                            nullIssues += Strings.Get("database", "nullfound", index, MAP_TILES_TABLE) +
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
                throw (new Exception(Strings.Get("database", "nullerror") + Environment.NewLine + nullIssues));
            }
            return null;
        }

        public static void SaveMapTiles(int index, byte[] data)
        {
            if (data == null) return;
            var query = "INSERT OR REPLACE into " + MAP_TILES_TABLE + " (" + MAP_TILES_MAP_ID + "," + MAP_TILES_DATA +
                        ")" + " VALUES " + " (@" + MAP_TILES_MAP_ID + ",@" + MAP_TILES_DATA + ")";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_TILES_MAP_ID, index));
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_TILES_DATA, data));
                ExecuteNonQuery(cmd);
            }
        }

        //Post Loading Functions
        private static void OnMapsLoaded()
        {
            if (MapBase.Lookup.Count == 0)
            {
                Console.WriteLine(Strings.Get("database", "nomaps"));
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
                Console.WriteLine(Strings.Get("database", "noclasses"));
                var cls = (ClassBase) AddGameObject(GameObjectType.Class);
                cls.Name = Strings.Get("database", "default");
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
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                using (var dataReader = ExecuteReader(cmd))
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
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_LIST_DATA,
                    MapList.GetList().Data(MapBase.Lookup)));
                ExecuteNonQuery(cmd);
            }
        }

        //Time
        private static void LoadTime()
        {
            var query = $"SELECT * from {TIME_TABLE};";
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                using (var dataReader = ExecuteReader(cmd))
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (dataReader[TIME_DATA].GetType() != typeof(DBNull))
                            {
                                var data = (byte[]) dataReader[TIME_DATA];
                                if (data.Length > 1)
                                {
                                    TimeBase.GetTimeBase().LoadTimeBase(data);
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
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + TIME_DATA,
                    TimeBase.GetTimeBase().SaveTimeBase()));
                ExecuteNonQuery(cmd);
            }
        }

        public static int ExecuteNonQuery(SqliteCommand command)
        {
            lock (SqlConnectionLock)
            {
                using (var transaction = sDbConnection?.BeginTransaction())
                {
                    var returnVal = command.ExecuteNonQuery();
                    transaction.Commit();
                    return returnVal;
                }
            }
        }

        public static SqliteDataReader ExecuteReader(SqliteCommand command)
        {
            lock (SqlConnectionLock)
            {
                return command.ExecuteReader();
            }
        }

        public static object ExecuteScalar(SqliteCommand command)
        {
            lock (SqlConnectionLock)
            {
                return command.ExecuteScalar();
            }
        }
    }
}