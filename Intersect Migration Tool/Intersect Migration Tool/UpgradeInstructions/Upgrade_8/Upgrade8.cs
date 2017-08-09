using System;
using Intersect.Logging;
using Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.Enums;
using Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects;
using Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects.Events;
using Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects.Maps;
using Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;
using Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.Models;
using Mono.Data.Sqlite;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_8
{
    public class Upgrade8
    {
        //GameObject Table Constants
        private const string GAME_OBJECT_ID = "id";
        private const string GAME_OBJECT_DELETED = "deleted";
        private const string GAME_OBJECT_DATA = "data";

        //Map Tiles Table
        private const string MAP_TILES_TABLE = "map_tiles";
        private const string MAP_TILES_MAP_ID = "map_id";
        private const string MAP_TILES_DATA = "data";

        //Char Friends Table Constants
        private const string CHAR_FRIENDS_TABLE = "char_friends";
        private const string CHAR_FRIEND_CHAR_ID = "char_id";
        private const string CHAR_FRIEND_ID = "friend_id";

        //Character Table Constants
        private const string CHAR_TABLE = "characters";
        private const string CHAR_DELETED = "deleted";
        private const string CHAR_LAST_ONLINE_TIME = "last_online";

        //Map List Table Constants
        private const string MAP_LIST_TABLE = "map_list";
        private const string MAP_LIST_DATA = "data";

        //Time of Day Table Constants
        private const string TIME_TABLE = "time";
        private const string TIME_DATA = "data";

        private SqliteConnection _dbConnection;
        private object _dbLock = new object();

        public Upgrade8(SqliteConnection connection)
        {
            _dbConnection = connection;
        }

        public void Upgrade()
        {
            //Gotta Load and Save All Events
            ServerOptions.LoadOptions();
            CreateMapTilesTable();
            LoadAllGameObjects();
            CreateCharacterFriendsTable();
            AddDeletedColumnToCharacters();
            AddLastOnlineColumnToCharacters();
            AddNotNullToGameObjectTables();
            FixSimpleTable(TIME_TABLE,TIME_DATA);
            FixSimpleTable(MAP_LIST_TABLE,MAP_LIST_DATA);
        }

        //Game Object Saving/Loading
        private void LoadAllGameObjects()
        {
            foreach (var val in Enum.GetValues(typeof(GameObjectType)))
            {
                if ((GameObjectType) val != GameObjectType.Time)
                {
                    LoadGameObjects((GameObjectType) val);
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
                    break;
                case GameObjectType.CommonEvent:
                    var buffer = new ByteBuffer();
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
                var query = "SELECT * from " + tableName + " WHERE " + GAME_OBJECT_DELETED + "=@" + GAME_OBJECT_DELETED +
                            ";";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 0.ToString()));
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var index = Convert.ToInt32(dataReader[GAME_OBJECT_ID]);
                            if (dataReader[GAME_OBJECT_DATA].GetType() != typeof(DBNull))
                            {
                                LoadGameObject(gameObjectType, index, (byte[]) dataReader[GAME_OBJECT_DATA]);
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
                throw (new Exception("Tried to load one or more null game objects!" + Environment.NewLine + nullIssues));
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
                using (SqliteCommand cmd = new SqliteCommand(insertQuery, _dbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_ID, gameObject.Index));
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 0.ToString()));
                    if (gameObject.BinaryData != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DATA, gameObject.BinaryData));
                    }
                    else
                    {
                        throw (new Exception("Tried to save a null game object (should be deleted instead?) Table: " +
                                             gameObject.DatabaseTable + " Id: " + gameObject.Index));
                    }
                    cmd.ExecuteNonQuery();
                }
            }

            if (gameObject.Type == GameObjectType.Map)
            {
                var map = (MapBase) gameObject;
                if (map.TileData != null)
                {
                    SaveMapTiles(map.Index, map.TileData);
                }
            }
        }

        private void CreateCharacterFriendsTable()
        {
            var cmd = "CREATE TABLE " + CHAR_FRIENDS_TABLE + " ("
                      + CHAR_FRIEND_CHAR_ID + " INTEGER,"
                      + CHAR_FRIEND_ID + " INTEGER,"
                      + " unique('" + CHAR_FRIEND_CHAR_ID + "','" + CHAR_FRIEND_ID + "')"
                      + ");";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }

        private void CreateMapTilesTable()
        {
            var cmd = "CREATE TABLE " + MAP_TILES_TABLE + " (" + MAP_TILES_MAP_ID + " INTEGER UNIQUE, " + MAP_TILES_DATA + " BLOB NOT NULL);";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }

        public void SaveMapTiles(int index, byte[] data)
        {
            if (data == null) return;
            var query = "INSERT OR REPLACE into " + MAP_TILES_TABLE + " (" + MAP_TILES_MAP_ID + "," + MAP_TILES_DATA + ")" + " VALUES " + " (@" + MAP_TILES_MAP_ID + ",@" + MAP_TILES_DATA + ")";
            using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_TILES_MAP_ID, index));
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_TILES_DATA, data));
                cmd.ExecuteNonQuery();
            }
        }

        private void AddDeletedColumnToCharacters()
        {
            var cmd = "ALTER TABLE " + CHAR_TABLE + " ADD " + CHAR_DELETED + " INTEGER;";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }

            cmd = "UPDATE " + CHAR_TABLE + " set " + CHAR_DELETED + " = @" + CHAR_DELETED + ";";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.Parameters.Add(new SqliteParameter("@" + CHAR_DELETED, 0));
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }

        private void AddLastOnlineColumnToCharacters()
        {
            var cmd = "ALTER TABLE " + CHAR_TABLE + " ADD " + CHAR_LAST_ONLINE_TIME + " INTEGER;";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }

            cmd = "UPDATE " + CHAR_TABLE + " set " + CHAR_LAST_ONLINE_TIME + " = @" + CHAR_LAST_ONLINE_TIME + ";";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.Parameters.Add(new SqliteParameter("@" + CHAR_LAST_ONLINE_TIME, DateTime.UtcNow.ToBinary()));
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }

        private void AddNotNullToGameObjectTables()
        {
            //Loop through each type of game object. //Delete anything that is currently null.
            foreach (var value in Enum.GetValues(typeof(GameObjectType)))
            {
                var type = (GameObjectType)value;
                if (type == GameObjectType.Time) continue;
                using (SqliteTransaction transaction = _dbConnection.BeginTransaction())
                {
                    //Rename Table to Old
                    var query = "ALTER TABLE " + type.GetTable() + " RENAME TO " + type.GetTable() + "_old;";
                    using (var cmd = _dbConnection.CreateCommand())
                    {
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }

                    //Create new table
                    query = "CREATE TABLE " + type.GetTable() + " ("
                              + GAME_OBJECT_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                              + GAME_OBJECT_DELETED + " INTEGER NOT NULL DEFAULT 0,"
                              + GAME_OBJECT_DATA + " BLOB NOT NULL" + ");";
                    using (var cmd = _dbConnection.CreateCommand())
                    {
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }

                    //Import from old table
                    query = "INSERT INTO " + type.GetTable() + " ("
                            + GAME_OBJECT_ID + ","
                            + GAME_OBJECT_DELETED + ","
                            + GAME_OBJECT_DATA + ") SELECT " + GAME_OBJECT_ID + "," + GAME_OBJECT_DELETED + "," + GAME_OBJECT_DATA + " FROM " + type.GetTable() + "_old ORDER BY " + GAME_OBJECT_ID + " ASC;";
                    using (var cmd = _dbConnection.CreateCommand())
                    {
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }

                    //Delete backup table
                    query = "DROP TABLE " + type.GetTable() + "_old;";
                    using (var cmd = _dbConnection.CreateCommand())
                    {
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        private void FixSimpleTable(string tablename, string datacol)
        {
            using (SqliteTransaction transaction = _dbConnection.BeginTransaction())
            {
                //Rename Table to Old
                var query = "ALTER TABLE " + tablename + " RENAME TO " + tablename + "_old;";
                using (var cmd = _dbConnection.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }

                //Create new table
                query = "CREATE TABLE " + tablename + " ("
                        + datacol + " BLOB NOT NULL" + ");";
                using (var cmd = _dbConnection.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }

                //Import from old table
                query = "INSERT INTO " + tablename + " ("
                        + datacol + ") SELECT " + datacol + " FROM " + tablename + "_old;";
                using (var cmd = _dbConnection.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }

                //Delete backup table
                query = "DROP TABLE " + tablename + "_old;";
                using (var cmd = _dbConnection.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }
    }
}