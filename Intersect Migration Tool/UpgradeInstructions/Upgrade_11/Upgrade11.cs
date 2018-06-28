using System;
using System.IO;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Enums;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Events;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Maps;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Models;
using Mono.Data.Sqlite;
using Log = Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Logging.Log;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_11
{
    public class Upgrade11
    {
        //Database Variables
        private const string INFO_TABLE = "info";

        private const string DB_VERSION = "dbversion";

        //Log Table Constants
        private const string LOG_TABLE = "logs";

        //Ban Table Constants
        private const string BAN_TABLE = "bans";
        //Mute Table Constants
        private const string MUTE_TABLE = "mutes";
        //User Table Constants
        private const string USERS_TABLE = "users";
        //Character Table Constants
        private const string CHAR_TABLE = "characters";
        //Char Inventory Table Constants
        private const string CHAR_INV_TABLE = "char_inventory";
        //Char Spells Table Constants
        private const string CHAR_SPELL_TABLE = "char_spells";
        //Char Hotbar Table Constants
        private const string CHAR_HOTBAR_TABLE = "char_hotbar";
        //Char Bank Table Constants
        private const string CHAR_BANK_TABLE = "char_bank";
        //Char Switches Table Constants
        private const string CHAR_SWITCHES_TABLE = "char_switches";
        //Char Variables Table Constants
        private const string CHAR_VARIABLES_TABLE = "char_variables";
        //Char Quests Table Constants
        private const string CHAR_QUESTS_TABLE = "char_quests";
        //Char Friendss Table Constants
        private const string CHAR_FRIENDS_TABLE = "char_friends";
        //Bag Table Constants
        private const string BAGS_TABLE = "bags";
        //Bag Items Table Constants
        private const string BAG_ITEMS_TABLE = "bag_items";







        //Map Tiles Table
        private const string MAP_TILES_TABLE = "map_tiles";

        private const string MAP_ATTRIBUTES_TABLE = "map_attributes";
        private const string MAP_ATTRIBUTES_MAP_ID = "map_id";
        private const string MAP_ATTRIBUTES_DATA = "data";

        //Map List Table Constants
        private const string MAP_LIST_TABLE = "map_list";
        //Time of Day Table Constants
        private const string TIME_TABLE = "time";
        private const string TIME_DATA = "data";




        //GameObject Table Constants
        private const string GAME_OBJECT_ID = "id";

        private const string GAME_OBJECT_DELETED = "deleted";
        private const string GAME_OBJECT_DATA = "data";

        private SqliteConnection _gameDbConnection;
        private SqliteConnection _playerDbConnection;
        private object _dbLock = new object();

        public Upgrade11()
        {
        }

        public void Upgrade()
        {
            //Gotta Load and Save All Events
            ServerOptions.LoadOptions();
            File.Copy(Path.Combine("resources", "intersect.db"), Path.Combine("resources", "gamedata.db"), true);
            File.Copy(Path.Combine("resources", "intersect.db"), Path.Combine("resources", "playerdata.db"), true);
            _gameDbConnection = new SqliteConnection("Data Source=" + Path.Combine("resources", "gamedata.db") + ";Version=3");
            _gameDbConnection.Open();
            _playerDbConnection = new SqliteConnection("Data Source=" + Path.Combine("resources", "playerdata.db") + ";Version=3");
            _playerDbConnection.Open();
            CleanPlayerDatabase();
            CleanGameDatabase();
            _playerDbConnection.Close();
            _gameDbConnection.Close();
            //File.Delete(Path.Combine("resources", "intersect.db"));
        }

        void CleanPlayerDatabase()
        {
            DeleteTable(LOG_TABLE, _playerDbConnection);
            DeleteTable(MAP_LIST_TABLE, _playerDbConnection);
            DeleteTable(MAP_TILES_TABLE, _playerDbConnection);
            DeleteTable(TIME_TABLE, _playerDbConnection);
            DeleteGameObjectTables(_playerDbConnection);
            IncrementDatabaseVersion(_playerDbConnection);
            VaccumDatbase(_playerDbConnection);
        }

        void CleanGameDatabase()
        {
            DeleteTable(LOG_TABLE, _gameDbConnection);
            DeleteTable(BAN_TABLE, _gameDbConnection);
            DeleteTable(MUTE_TABLE, _gameDbConnection);
            DeleteTable(USERS_TABLE, _gameDbConnection);
            DeleteTable(CHAR_TABLE, _gameDbConnection);
            DeleteTable(CHAR_INV_TABLE, _gameDbConnection);
            DeleteTable(CHAR_SPELL_TABLE, _gameDbConnection);
            DeleteTable(CHAR_HOTBAR_TABLE, _gameDbConnection);
            DeleteTable(CHAR_BANK_TABLE, _gameDbConnection);
            DeleteTable(CHAR_SWITCHES_TABLE, _gameDbConnection);
            DeleteTable(CHAR_VARIABLES_TABLE, _gameDbConnection);
            DeleteTable(CHAR_QUESTS_TABLE, _gameDbConnection);
            DeleteTable(CHAR_FRIENDS_TABLE, _gameDbConnection);
            DeleteTable(BAGS_TABLE, _gameDbConnection);
            DeleteTable(BAG_ITEMS_TABLE, _gameDbConnection);
            MigrateGameDatabase();
            IncrementDatabaseVersion(_gameDbConnection);
            VaccumDatbase(_gameDbConnection);
        }

        void MigrateGameDatabase()
        {
            MoveGameObjectTables(_gameDbConnection);

            var sql = "ALTER TABLE " + TIME_TABLE + " RENAME TO " + TIME_TABLE + "_old" + ";";
            using (SqliteCommand cmd = new SqliteCommand(sql, _gameDbConnection))
            {
                cmd.ExecuteNonQuery();
            }

            CreateTimeTable();
            LoadTime();
            DeleteTable(TIME_TABLE + "_old", _gameDbConnection);
            CreateMapAttributesTable();
            LoadAllGameObjects();
            DeleteOldGameObjectTables(_gameDbConnection);
        }

        private void IncrementDatabaseVersion(SqliteConnection dbConn)
        {
            var cmd = "UPDATE " + INFO_TABLE + " SET " + DB_VERSION + " = " + (11) + ";";
            using (var createCommand = dbConn.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }

        //Game Object Saving/Loading
        private void MoveGameObjectTables(SqliteConnection dbConn)
        {
            foreach (var val in Enum.GetValues(typeof(GameObjectType)))
            {
                if ((GameObjectType)val != GameObjectType.Time)
                {
                    MoveTable(((GameObjectType)val).GetTable(), ((GameObjectType)val).GetTable() + "_old", dbConn);
                }
            }
        }

        private void DeleteOldGameObjectTables(SqliteConnection dbConn)
        {
            foreach (var val in Enum.GetValues(typeof(GameObjectType)))
            {
                if ((GameObjectType)val != GameObjectType.Time)
                {
                    DeleteTable(((GameObjectType)val).GetTable() + "_old", dbConn);
                }
            }
        }

        void VaccumDatbase(SqliteConnection dbConn)
        {
            var sql = "VACUUM;";
            using (SqliteCommand cmd = new SqliteCommand(sql, dbConn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        void DeleteTable(string tableName, SqliteConnection connection)
        {
            var sql = "DROP TABLE " + tableName + ";";
            using (SqliteCommand cmd = new SqliteCommand(sql, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        void MoveTable(string tableName, string newTableName, SqliteConnection connection)
        {
            var sql = "ALTER TABLE " + tableName + " RENAME TO " + newTableName + ";";
            using (SqliteCommand cmd = new SqliteCommand(sql, connection))
            {
                cmd.ExecuteNonQuery();
            }

            sql = "CREATE TABLE " + tableName + " ("
                      + GAME_OBJECT_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                      + GAME_OBJECT_DELETED + " INTEGER NOT NULL DEFAULT 0,"
                      + GAME_OBJECT_DATA + " TEXT NOT NULL" + ");";
            using (var createCommand = _gameDbConnection.CreateCommand())
            {
                createCommand.CommandText = sql;
                createCommand.ExecuteNonQuery();
            }

            sql = "INSERT INTO " + tableName + " SELECT * FROM " + newTableName + ";";
            using (SqliteCommand cmd = new SqliteCommand(sql, connection))
            {
                cmd.ExecuteNonQuery();
            }

            sql = "DELETE FROM " + tableName + " WHERE DELETED=1;";
            using (SqliteCommand cmd = new SqliteCommand(sql, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }


        private void CreateMapAttributesTable()
        {
            var cmd = "CREATE TABLE " + MAP_ATTRIBUTES_TABLE + " (" + MAP_ATTRIBUTES_MAP_ID + " INTEGER UNIQUE, " +
                      MAP_ATTRIBUTES_DATA + " BLOB NOT NULL);";
            using (var createCommand = _gameDbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }

        //Game Object Saving/Loading
        private void LoadAllGameObjects()
        {
            foreach (var val in Enum.GetValues(typeof(GameObjectType)))
            {
                if ((GameObjectType)val != GameObjectType.Time)
                {
                    LoadGameObjects((GameObjectType)val);
                }
            }
        }

        //Game Object Saving/Loading
        private void DeleteGameObjectTables(SqliteConnection dbConn)
        {
            foreach (var val in Enum.GetValues(typeof(GameObjectType)))
            {
                if ((GameObjectType)val != GameObjectType.Time)
                {
                    DeleteTable(((GameObjectType)val).GetTable(), dbConn);
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

        private void LoadGameObject(GameObjectType type, int index, byte[] data)
        {
            switch (type)
            {
                case GameObjectType.Animation:
                    var anim = new AnimationBase(index);
                    anim.Load(data);
                    AnimationBase.Lookup.Set(index, anim);
                    SaveGameObject(anim);
                    break;
                case GameObjectType.Bench:
                    var bench = new BenchBase(index);
                    bench.Load(data);
                    BenchBase.Lookup.Set(index, bench);
                    SaveGameObject(bench);
                    break;
                case GameObjectType.Class:
                    var cls = new ClassBase(index);
                    cls.Load(data);
                    ClassBase.Lookup.Set(index, cls);
                    SaveGameObject(cls);
                    break;
                case GameObjectType.Item:
                    var itm = new ItemBase(index);
                    itm.Load(data);
                    ItemBase.Lookup.Set(index, itm);
                    SaveGameObject(itm);
                    break;
                case GameObjectType.Npc:
                    var npc = new NpcBase(index);
                    npc.Load(data);
                    NpcBase.Lookup.Set(index, npc);
                    SaveGameObject(npc);
                    break;
                case GameObjectType.Projectile:
                    var proj = new ProjectileBase(index);
                    proj.Load(data);
                    ProjectileBase.Lookup.Set(index, proj);
                    SaveGameObject(proj);
                    break;
                case GameObjectType.Quest:
                    var qst = new QuestBase(index);
                    qst.Load(data);
                    QuestBase.Lookup.Set(index, qst);
                    SaveGameObject(qst);
                    break;
                case GameObjectType.Resource:
                    var res = new ResourceBase(index);
                    res.Load(data);
                    ResourceBase.Lookup.Set(index, res);
                    SaveGameObject(res);
                    break;
                case GameObjectType.Shop:
                    var shp = new ShopBase(index);
                    shp.Load(data);
                    ShopBase.Lookup.Set(index, shp);
                    SaveGameObject(shp);
                    break;
                case GameObjectType.Spell:
                    var spl = new SpellBase(index);
                    spl.Load(data);

                    SpellBase.Lookup.Set(index, spl);
                    SaveGameObject(spl);
                    break;
                case GameObjectType.Map:
                    var map = new MapBase(index, false);
                    MapBase.Lookup.Set(index, map);
                    map.Load(data);
                    SaveGameObject(map);
                    SaveMapAttributes(map.Index, map.AttributesData());
                    break;
                case GameObjectType.CommonEvent:
                    var buffer = new Intersect_Convert_Lib.ByteBuffer();
                    buffer.WriteBytes(data);
                    var evt = new EventBase(index, buffer, true);
                    EventBase.Lookup.Set(index, evt);
                    buffer.Dispose();
                    SaveGameObject(evt);
                    break;
                case GameObjectType.PlayerSwitch:
                    var pswitch = new PlayerSwitchBase(index);
                    pswitch.Load(data);
                    PlayerSwitchBase.Lookup.Set(index, pswitch);
                    SaveGameObject(pswitch);
                    break;
                case GameObjectType.PlayerVariable:
                    var pvar = new PlayerVariableBase(index);
                    pvar.Load(data);
                    PlayerVariableBase.Lookup.Set(index, pvar);
                    SaveGameObject(pvar);
                    break;
                case GameObjectType.ServerSwitch:
                    var sswitch = new ServerSwitchBase(index);
                    sswitch.Load(data);
                    ServerSwitchBase.Lookup.Set(index, sswitch);
                    SaveGameObject(sswitch);
                    break;
                case GameObjectType.ServerVariable:
                    var svar = new ServerVariableBase(index);
                    svar.Load(data);
                    ServerVariableBase.Lookup.Set(index, svar);
                    SaveGameObject(svar);
                    break;
                case GameObjectType.Tileset:
                    var tset = new TilesetBase(index);
                    tset.Load(data);
                    TilesetBase.Lookup.Set(index, tset);
                    SaveGameObject(tset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void LoadGameObjects(GameObjectType gameObjectType)
        {
            var nullIssues = "";
            lock (_dbLock)
            {
                var tableName = gameObjectType.GetTable();
                ClearGameObjects(gameObjectType);
                var query = "SELECT * from " + tableName + "_old" + " WHERE " + GAME_OBJECT_DELETED + "=@" +
                            GAME_OBJECT_DELETED +
                            ";";
                using (SqliteCommand cmd = new SqliteCommand(query, _gameDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 0.ToString()));
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var index = Convert.ToInt32(dataReader[GAME_OBJECT_ID]);
                            if (dataReader[GAME_OBJECT_DATA].GetType() != typeof(DBNull))
                            {
                                LoadGameObject(gameObjectType, index, (byte[])dataReader[GAME_OBJECT_DATA]);
                            }
                            else
                            {
                                nullIssues += "Tried to load null value for index " + index + " of " + tableName +
                                              Environment.NewLine;
                            }
                        }
                    }
                }
            }
            if (nullIssues != "")
            {
                throw (new Exception("Tried to load one or more null game objects!" + Environment.NewLine +
                                     nullIssues));
            }
        }

        public void SaveGameObject(IDatabaseObject gameObject)
        {
            if (gameObject == null)
            {
                Log.Error("Attempted to persist null game object to the database.");
            }

            lock (_dbLock)
            {
                var insertQuery = "UPDATE " + gameObject.DatabaseTable + " set " + GAME_OBJECT_DELETED + "=@" +
                                  GAME_OBJECT_DELETED + "," + GAME_OBJECT_DATA + "=@" + GAME_OBJECT_DATA + " WHERE " +
                                  GAME_OBJECT_ID + "=@" + GAME_OBJECT_ID + ";";
                using (SqliteCommand cmd = new SqliteCommand(insertQuery, _gameDbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_ID, gameObject.Index));
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 0.ToString()));
                    if (gameObject.BinaryData != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DATA, gameObject.JsonData));
                    }
                    else
                    {
                        throw (new Exception("Tried to save a null game object (should be deleted instead?) Table: " +
                                             gameObject.DatabaseTable + " Id: " + gameObject.Index));
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveMapAttributes(int index, byte[] data)
        {
            if (data == null) return;
            var query = "INSERT OR REPLACE into " + MAP_ATTRIBUTES_TABLE + " (" + MAP_ATTRIBUTES_MAP_ID + "," + MAP_ATTRIBUTES_DATA +
                        ")" + " VALUES " + " (@" + MAP_ATTRIBUTES_MAP_ID + ",@" + MAP_ATTRIBUTES_DATA + ")";
            using (SqliteCommand cmd = new SqliteCommand(query, _gameDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_ATTRIBUTES_MAP_ID, index));
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_ATTRIBUTES_DATA, data));
                cmd.ExecuteNonQuery();
            }
        }

        private void CreateTimeTable()
        {
            var cmd = "CREATE TABLE " + TIME_TABLE + " (" + TIME_DATA + " TEXT NOT NULL);";
            using (var createCommand = _gameDbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
            InsertTime();
        }


        //Time
        private void LoadTime()
        {
            var query = $"SELECT * from {TIME_TABLE}_old;";
            using (var cmd = new SqliteCommand(query, _gameDbConnection))
            {
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (dataReader[TIME_DATA].GetType() != typeof(DBNull))
                            {
                                var data = (byte[])dataReader[TIME_DATA];
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
        }

        private void InsertTime()
        {
            var cmd = $"INSERT into {TIME_TABLE} (" + TIME_DATA + ") VALUES (@" + TIME_DATA + ");";
            using (var createCommand = _gameDbConnection?.CreateCommand())
            {
                createCommand.Parameters.Add(new SqliteParameter("@" + TIME_DATA, ""));
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }

        public void SaveTime()
        {
            var query = "UPDATE " + TIME_TABLE + " set " + TIME_DATA + "=@" + TIME_DATA + ";";
            using (var cmd = new SqliteCommand(query, _gameDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + TIME_DATA,
                    TimeBase.GetTimeJson()));
                cmd.ExecuteNonQuery();
            }
        }
    }
}