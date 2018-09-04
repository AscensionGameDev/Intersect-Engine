using System;
using System.Collections.Generic;
using System.IO;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Enums;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Events;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Maps;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Models;
using Intersect.Migration.UpgradeInstructions.Upgrade_12;
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
        
        private const string MAP_ATTRIBUTES_TABLE = "map_attributes";
        private const string MAP_ATTRIBUTES_MAP_ID = "map_id";
        private const string MAP_ATTRIBUTES_DATA = "data";

        //Map Tiles Table
        private const string MAP_TILES_TABLE = "map_tiles";

        private const string MAP_TILES_MAP_ID = "map_id";
        private const string MAP_TILES_DATA = "data";

        //Map List Table Constants
        private const string MAP_LIST_TABLE = "map_list";
        //Time of Day Table Constants
        private const string TIME_TABLE = "time";
        private const string TIME_DATA = "data";

        //GameObject Table Constants
        private const string GAME_OBJECT_ID = "id";

        private const string GAME_OBJECT_DELETED = "deleted";
        private const string GAME_OBJECT_DATA = "data";

        private SqliteConnection _dbConnection;
        private object _dbLock = new object();

        private int craftsIndex = 1;
        private int craftTablesIndex = 1;

        Dictionary<GameObjectType,Dictionary<int,string>> objs = new Dictionary<GameObjectType, Dictionary<int, string>>();

        public Upgrade11(SqliteConnection conn)
        {
            _dbConnection = conn;
        }

        public void Upgrade()
        {
            //Gotta Load and Save All Events
            ServerOptions.LoadOptions();
            LoadAllGameObjects();
            LoadTime();

            objs.Remove(GameObjectType.Bench);
            objs[GameObjectType.Time].Add(0, TimeBase.GetTimeJson());

            var up12 = new Upgrade12(objs);
            up12.Upgrade();
        }

        private void IncrementDatabaseVersion(SqliteConnection dbConn)
        {
            var cmd = "UPDATE " + INFO_TABLE + " SET " + DB_VERSION + " = " + (12) + ";";
            using (var createCommand = dbConn.CreateCommand())
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
                objs.Add((GameObjectType)val, new Dictionary<int, string>());
            }
            LoadGameObjects((GameObjectType)GameObjectType.Item);
            foreach (var val in Enum.GetValues(typeof(GameObjectType)))
            {
                if ((GameObjectType)val != GameObjectType.Time && (GameObjectType)val != GameObjectType.Item)
                {
                    LoadGameObjects((GameObjectType)val);
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
            var dict = objs[type];
            switch (type)
            {
                case GameObjectType.Animation:
                    var anim = new AnimationBase(index);
                    anim.Load(data);
                    AnimationBase.Lookup.Set(index, anim);
                    dict.Add(index, anim.JsonData);
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
                    dict.Add(index, cls.JsonData);
                    break;
                case GameObjectType.Item:
                    var itm = new ItemBase(index);
                    itm.Load(data);
                    ItemBase.Lookup.Set(index, itm);
                    dict.Add(index, itm.JsonData);
                    break;
                case GameObjectType.Npc:
                    var npc = new NpcBase(index);
                    npc.Load(data);
                    NpcBase.Lookup.Set(index, npc);
                    dict.Add(index, npc.JsonData);
                    break;
                case GameObjectType.Projectile:
                    var proj = new ProjectileBase(index);
                    proj.Load(data);
                    ProjectileBase.Lookup.Set(index, proj);
                    dict.Add(index, proj.JsonData);
                    break;
                case GameObjectType.Quest:
                    var qst = new QuestBase(index);
                    qst.Load(data);
                    QuestBase.Lookup.Set(index, qst);
                    dict.Add(index, qst.JsonData);
                    break;
                case GameObjectType.Resource:
                    var res = new ResourceBase(index);
                    res.Load(data);
                    ResourceBase.Lookup.Set(index, res);
                    dict.Add(index, res.JsonData);
                    break;
                case GameObjectType.Shop:
                    var shp = new ShopBase(index);
                    shp.Load(data);
                    ShopBase.Lookup.Set(index, shp);
                    dict.Add(index, shp.JsonData);
                    break;
                case GameObjectType.Spell:
                    var spl = new SpellBase(index);
                    spl.Load(data);
                    SpellBase.Lookup.Set(index, spl);
                    dict.Add(index, spl.JsonData);
                    break;
                case GameObjectType.Map:
                    var map = new MapBase(index, false);
                    MapBase.Lookup.Set(index, map);
                    map.Load(data);
                    map.TileData = GetMapTiles(index);
                    dict.Add(index, map.JsonData);
                    break;
                case GameObjectType.CommonEvent:
                    var buffer = new Intersect_Convert_Lib.ByteBuffer();
                    buffer.WriteBytes(data);
                    var evt = new EventBase(index, buffer, true);
                    EventBase.Lookup.Set(index, evt);
                    buffer.Dispose();
                    dict.Add(index, evt.JsonData);
                    break;
                case GameObjectType.PlayerSwitch:
                    var pswitch = new PlayerSwitchBase(index);
                    pswitch.Load(data);
                    PlayerSwitchBase.Lookup.Set(index, pswitch);
                    dict.Add(index, pswitch.JsonData);
                    break;
                case GameObjectType.PlayerVariable:
                    var pvar = new PlayerVariableBase(index);
                    pvar.Load(data);
                    PlayerVariableBase.Lookup.Set(index, pvar);
                    dict.Add(index, pvar.JsonData);
                    break;
                case GameObjectType.ServerSwitch:
                    var sswitch = new ServerSwitchBase(index);
                    sswitch.Load(data);
                    ServerSwitchBase.Lookup.Set(index, sswitch);
                    dict.Add(index, sswitch.JsonData);
                    break;
                case GameObjectType.ServerVariable:
                    var svar = new ServerVariableBase(index);
                    svar.Load(data);
                    ServerVariableBase.Lookup.Set(index, svar);
                    dict.Add(index, svar.JsonData);
                    break;
                case GameObjectType.Tileset:
                    var tset = new TilesetBase(index);
                    tset.Load(data);
                    TilesetBase.Lookup.Set(index, tset);
                    dict.Add(index, tset.JsonData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void LoadGameObjects(GameObjectType gameObjectType)
        {
            if (gameObjectType == GameObjectType.CraftTables || gameObjectType == GameObjectType.Crafts) return;
            var nullIssues = "";
            lock (_dbLock)
            {
                var tableName = gameObjectType.GetTable();
                ClearGameObjects(gameObjectType);
                var query = "SELECT * from " + tableName + " WHERE " + GAME_OBJECT_DELETED + "=@" +
                            GAME_OBJECT_DELETED +
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

        public IDatabaseObject AddGameObject(GameObjectType gameObjectType)
        {
            var insertQuery = $"INSERT into {gameObjectType.GetTable()} (" + GAME_OBJECT_DATA + ") VALUES (@" + GAME_OBJECT_DATA + ")" + "; SELECT last_insert_rowid();";
            var index = -1;
            using (var cmd = _dbConnection.CreateCommand())
            {
                cmd.CommandText = insertQuery;
                cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DATA, new byte[1]));
                index = (int)((long)cmd.ExecuteScalar());
            }
            if (index > -1)
            {
                IDatabaseObject dbObj = null;
                switch (gameObjectType)
                {
                    case GameObjectType.CraftTables:
                        var obje = new CraftingTableBase(index);
                        dbObj = obje;
                        CraftingTableBase.Lookup.Set(index, obje);
                        break;
                    case GameObjectType.Crafts:
                        var crft = new CraftBase(index);
                        dbObj = crft;
                        CraftBase.Lookup.Set(index, crft);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(gameObjectType), gameObjectType, null);
                }

                SaveGameObject(dbObj);
                return dbObj;
            }
            return null;
        }

        public void SaveGameObject(IDatabaseObject gameObject)
        {
            if (gameObject == null)
            {
                Log.Error("Attempted to persist null game object to the database.");
            }

            if (gameObject.Type == GameObjectType.Bench)
            {
                //Add any crafts
                var go = (BenchBase)gameObject;
                var newCrafts = new List<int>();
                var matchedId = -1;
                foreach (var craft in go.Crafts)
                {
                    var craftId = -1;
                    //check if we already have the craft anywhere
                    foreach (var exCraft in CraftBase.Lookup.Values)
                    {
                        var cft = (CraftBase) exCraft;
                        var match = true;
                        if (cft.Item != craft.Item) match = false;
                        if (cft.Time != craft.Time) match = false;

                        Dictionary<int,int> cftItems = new Dictionary<int, int>();
                        Dictionary<int,int> craftItems = new Dictionary<int, int>();
                        foreach (var item in cft.Ingredients)
                        {
                            if (cftItems.ContainsKey(item.Item))
                            {
                                cftItems[item.Item] += item.Quantity;
                            }
                            else
                            {
                                cftItems.Add(item.Item,item.Quantity);
                            }
                        }

                        foreach (var item in craft.Ingredients)
                        {
                            if (craftItems.ContainsKey(item.Item))
                            {
                                craftItems[item.Item] += item.Quantity;
                            }
                            else
                            {
                                craftItems.Add(item.Item, item.Quantity);
                            }
                        }

                        craftItems.Remove(-1);
                        cftItems.Remove(-1);

                        if (cftItems.Count == craftItems.Count)
                        {
                            foreach (var itm in cftItems.Keys)
                            {
                                if (!craftItems.ContainsKey(itm) || craftItems[itm] != cftItems[itm]) match = false;
                            }
                        }
                        else
                        {
                            match = false;
                        }

                        if (match)
                        {
                            //Found a duplicate ingredient!
                            matchedId = exCraft.Index;
                        }
                    }

                    //add the craft if needed
                    if (matchedId != -1)
                    {
                        newCrafts.Add(matchedId);
                    }
                    else
                    {
                        //Create new craft, copy over settings, save
                        var cft = new CraftBase(craftsIndex);
                        cft.Name = ItemBase.GetName(craft.Item);
                        cft.Time = craft.Time;
                        cft.Item = craft.Item;
                        cft.Ingredients = craft.Ingredients;
                        objs[GameObjectType.Crafts].Add(craftsIndex, cft.JsonData);
                        craftsIndex++;
                        newCrafts.Add(cft.Index);
                    }
                }

                //Create a table with the bench name and crafts
                var tbl = new CraftingTableBase(craftTablesIndex);
                tbl.Name = go.Name;
                tbl.Crafts = newCrafts;
                objs[GameObjectType.CraftTables].Add(craftTablesIndex, tbl.JsonData);
                craftTablesIndex++;
                return;
            }
            return;
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


        //Time
        private void LoadTime()
        {
            var query = $"SELECT * from {TIME_TABLE};";
            using (var cmd = new SqliteCommand(query, _dbConnection))
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
                }
            }
            var json = TimeBase.GetTimeJson();
        }

        public byte[] GetMapTiles(int index)
        {
            var nullIssues = "";
            var query = $"SELECT * from {MAP_TILES_TABLE} WHERE " + MAP_TILES_MAP_ID + "=@" + MAP_TILES_MAP_ID +
                        ";";
            using (var cmd = new SqliteCommand(query, _dbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_TILES_MAP_ID, index));
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        if (dataReader[MAP_TILES_DATA].GetType() != typeof(DBNull))
                        {
                            return (byte[])dataReader[MAP_TILES_DATA];
                        }
                    }
                    else
                    {
                        return new byte[Options.LayerCount * Options.MapWidth * Options.MapHeight * 13];
                    }
                }
            }
            return null;
        }
    }
}