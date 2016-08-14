using System;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_1.Intersect_Convert_Lib;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_1.Intersect_Convert_Lib.GameObjects;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_1.Intersect_Convert_Lib.GameObjects.Events;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_1.Intersect_Convert_Lib.GameObjects.Maps;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_1.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;
using Mono.Data.Sqlite;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_1
{
    public class Upgrade1
    {
        private SqliteConnection _dbConnection;
        private Object _dbLock = new Object();

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

        //Char Quests Table Constants
        private const string CHAR_QUESTS_TABLE = "char_quests";
        private const string CHAR_QUEST_CHAR_ID = "char_id";
        private const string CHAR_QUEST_ID = "quest_id";
        private const string CHAR_QUEST_TASK = "task";
        private const string CHAR_QUEST_TASK_PROGRESS = "task_progress";
        private const string CHAR_QUEST_COMPLETED = "completed";

        //GameObject Table Constants
        private const string GAME_OBJECT_ID = "id";
        private const string GAME_OBJECT_DELETED = "deleted";
        private const string GAME_OBJECT_DATA = "data";

        //Map List Table Constants
        private const string MAP_LIST_TABLE = "map_list";
        private const string MAP_LIST_DATA = "data";

        public Upgrade1(SqliteConnection connection)
        {
            _dbConnection = connection;
        }

        public void Upgrade()
        {
            //Upgrade Steps
            ServerOptions.LoadOptions();
            CreateCharacterQuestsTable();
            CreateLogsTable();
            CreateBansTable();
            CreateMutesTable();
            ConvertNpcs();
            ConvertClasses();
            ConvertItems();
            ConvertSpells();
            ConvertEvents();
            ConvertMaps();
        }

        //This adds a spell list to npcs that we can use later.
        public void ConvertNpcs()
        {
            LoadGameObjects(GameObject.Npc);
            foreach (var obj in NpcBase.GetObjects())
            {
                SaveGameObject(obj.Value);
            }
        }
        public void ConvertClasses()
        {
            LoadGameObjects(GameObject.Class);
            foreach (var obj in ClassBase.GetObjects())
            {
                SaveGameObject(obj.Value);
            }
        }
        public void ConvertItems()
        {
            LoadGameObjects(GameObject.Item);
            foreach (var obj in ItemBase.GetObjects())
            {
                SaveGameObject(obj.Value);
            }
        }
        public void ConvertSpells()
        {
            LoadGameObjects(GameObject.Spell);
            foreach (var obj in SpellBase.GetObjects())
            {
                SaveGameObject(obj.Value);
            }
        }
        public void ConvertEvents()
        {
            LoadGameObjects(GameObject.CommonEvent);
            foreach (var obj in EventBase.GetObjects())
            {
                SaveGameObject(obj.Value);
            }
        }
        public void ConvertMaps()
        {
            LoadGameObjects(GameObject.Map);
            foreach (var obj in MapBase.GetObjects())
            {
                SaveGameObject(obj.Value);
            }
        }

        //Game Object Saving/Loading
        private string GetGameObjectTable(GameObject type)
        {
            var tableName = "";
            switch (type)
            {
                case GameObject.Animation:
                    tableName = AnimationBase.DatabaseTable;
                    break;
                case GameObject.Class:
                    tableName = ClassBase.DatabaseTable;
                    break;
                case GameObject.Item:
                    tableName = ItemBase.DatabaseTable;
                    break;
                case GameObject.Npc:
                    tableName = NpcBase.DatabaseTable;
                    break;
                case GameObject.Projectile:
                    tableName = ProjectileBase.DatabaseTable;
                    break;
                case GameObject.Quest:
                    tableName = QuestBase.DatabaseTable;
                    break;
                case GameObject.Resource:
                    tableName = ResourceBase.DatabaseTable;
                    break;
                case GameObject.Shop:
                    tableName = ShopBase.DatabaseTable;
                    break;
                case GameObject.Spell:
                    tableName = SpellBase.DatabaseTable;
                    break;
                case GameObject.Map:
                    tableName = MapBase.DatabaseTable;
                    break;
                case GameObject.CommonEvent:
                    tableName = EventBase.DatabaseTable;
                    break;
                case GameObject.PlayerSwitch:
                    tableName = PlayerSwitchBase.DatabaseTable;
                    break;
                case GameObject.PlayerVariable:
                    tableName = PlayerVariableBase.DatabaseTable;
                    break;
                case GameObject.ServerSwitch:
                    tableName = ServerSwitchBase.DatabaseTable;
                    break;
                case GameObject.ServerVariable:
                    tableName = ServerVariableBase.DatabaseTable;
                    break;
                case GameObject.Tileset:
                    tableName = TilesetBase.DatabaseTable;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return tableName;
        }
        private void ClearGameObjects(GameObject type)
        {
            switch (type)
            {
                case GameObject.Animation:
                    AnimationBase.ClearObjects();
                    break;
                case GameObject.Class:
                    ClassBase.ClearObjects();
                    break;
                case GameObject.Item:
                    ItemBase.ClearObjects();
                    break;
                case GameObject.Npc:
                    NpcBase.ClearObjects();
                    break;
                case GameObject.Projectile:
                    ProjectileBase.ClearObjects();
                    break;
                case GameObject.Quest:
                    QuestBase.ClearObjects();
                    break;
                case GameObject.Resource:
                    ResourceBase.ClearObjects();
                    break;
                case GameObject.Shop:
                    ShopBase.ClearObjects();
                    break;
                case GameObject.Spell:
                    SpellBase.ClearObjects();
                    break;
                case GameObject.Map:
                    MapBase.ClearObjects();
                    break;
                case GameObject.CommonEvent:
                    EventBase.ClearObjects();
                    break;
                case GameObject.PlayerSwitch:
                    PlayerSwitchBase.ClearObjects();
                    break;
                case GameObject.PlayerVariable:
                    PlayerVariableBase.ClearObjects();
                    break;
                case GameObject.ServerSwitch:
                    ServerSwitchBase.ClearObjects();
                    break;
                case GameObject.ServerVariable:
                    ServerVariableBase.ClearObjects();
                    break;
                case GameObject.Tileset:
                    TilesetBase.ClearObjects();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        private void LoadGameObject(GameObject type, int index, byte[] data)
        {
            switch (type)
            {
                case GameObject.Animation:
                    var anim = new AnimationBase(index);
                    anim.Load(data);
                    AnimationBase.AddObject(index, anim);
                    break;
                case GameObject.Class:
                    var cls = new ClassBase(index);
                    cls.Load(data);
                    ClassBase.AddObject(index, cls);
                    break;
                case GameObject.Item:
                    var itm = new ItemBase(index);
                    itm.Load(data);
                    ItemBase.AddObject(index, itm);
                    break;
                case GameObject.Npc:
                    var npc = new NpcBase(index);
                    npc.Load(data);
                    NpcBase.AddObject(index, npc);
                    break;
                case GameObject.Projectile:
                    var proj = new ProjectileBase(index);
                    proj.Load(data);
                    ProjectileBase.AddObject(index, proj);
                    break;
                case GameObject.Quest:
                    var qst = new QuestBase(index);
                    qst.Load(data);
                    QuestBase.AddObject(index, qst);
                    break;
                case GameObject.Resource:
                    var res = new ResourceBase(index);
                    res.Load(data);
                    ResourceBase.AddObject(index, res);
                    break;
                case GameObject.Shop:
                    var shp = new ShopBase(index);
                    shp.Load(data);
                    ShopBase.AddObject(index, shp);
                    break;
                case GameObject.Spell:
                    var spl = new SpellBase(index);
                    spl.Load(data);
                    SpellBase.AddObject(index, spl);
                    break;
                case GameObject.Map:
                    var map = new MapBase(index,false);
                    MapBase.AddObject(index, map);
                    map.Load(data);
                    break;
                case GameObject.CommonEvent:
                    var buffer = new ByteBuffer();
                    buffer.WriteBytes(data);
                    var evt = new EventBase(index, buffer, true);
                    EventBase.AddObject(index, evt);
                    buffer.Dispose();
                    break;
                case GameObject.PlayerSwitch:
                    var pswitch = new PlayerSwitchBase(index);
                    pswitch.Load(data);
                    PlayerSwitchBase.AddObject(index, pswitch);
                    break;
                case GameObject.PlayerVariable:
                    var pvar = new PlayerVariableBase(index);
                    pvar.Load(data);
                    PlayerVariableBase.AddObject(index, pvar);
                    break;
                case GameObject.ServerSwitch:
                    var sswitch = new ServerSwitchBase(index);
                    sswitch.Load(data);
                    ServerSwitchBase.AddObject(index, sswitch);
                    break;
                case GameObject.ServerVariable:
                    var svar = new ServerVariableBase(index);
                    svar.Load(data);
                    ServerVariableBase.AddObject(index, svar);
                    break;
                case GameObject.Tileset:
                    var tset = new TilesetBase(index);
                    tset.Load(data);
                    TilesetBase.AddObject(index, tset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        public void LoadGameObjects(GameObject type)
        {
            var nullIssues = "";
            lock (_dbLock)
            {
                var tableName = GetGameObjectTable(type);
                ClearGameObjects(type);
                var query = "SELECT * from " + tableName + " WHERE " + GAME_OBJECT_DELETED + "=@" + GAME_OBJECT_DELETED +
                            ";";
                using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 0.ToString()));
                    var dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        var index = Convert.ToInt32(dataReader[GAME_OBJECT_ID]);
                        if (dataReader[MAP_LIST_DATA].GetType() != typeof(System.DBNull))
                        {
                            LoadGameObject(type, index, (byte[])dataReader[GAME_OBJECT_DATA]);
                        }
                        else
                        {
                            nullIssues += "Tried to load null value for index " + index + " of " + tableName + Environment.NewLine;
                        }
                    }
                }
            }
            if (nullIssues != "")
            {
                throw (new Exception("Tried to load one or more null game objects!" + Environment.NewLine + nullIssues));
            }
        }
        public void SaveGameObject(DatabaseObject gameObject)
        {
            lock (_dbLock)
            {
                var insertQuery = "UPDATE " + gameObject.GetTable() + " set " + GAME_OBJECT_DELETED + "=@" +
                                  GAME_OBJECT_DELETED + "," + GAME_OBJECT_DATA + "=@" + GAME_OBJECT_DATA + " WHERE " +
                                  GAME_OBJECT_ID + "=@" + GAME_OBJECT_ID + ";";
                using (SqliteCommand cmd = new SqliteCommand(insertQuery, _dbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_ID, gameObject.GetId()));
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 0.ToString()));
                    if (gameObject != null && gameObject.GetData() != null)
                    {
                        cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DATA, gameObject.GetData()));
                    }
                    else
                    {
                        throw (new Exception("Tried to save a null game object (should be deleted instead?) Table: " +
                                             gameObject.GetTable() + " Id: " + gameObject.GetId()));
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }
        

        //Create Missing Tables
        private void CreateLogsTable()
        {
            var cmd = "CREATE TABLE " + LOG_TABLE + " ("
                        + LOG_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                        + LOG_TIME + " TEXT,"
                        + LOG_TYPE + " TEXT,"
                        + LOG_INFO + " TEXT"
                        + ");";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }
        private void CreateMutesTable()
        {
            var cmd = "CREATE TABLE " + MUTE_TABLE + " ("
                + MUTE_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                + MUTE_TIME + " TEXT,"
                + MUTE_USER + " INTEGER,"
                + MUTE_IP + " TEXT,"
                + MUTE_DURATION + " INTEGER,"
                + MUTE_REASON + " TEXT,"
                + MUTE_MUTER + " INTEGER"
                + ");";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }
        private void CreateBansTable()
        {
            var cmd = "CREATE TABLE " + BAN_TABLE + " ("
                + BAN_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                + BAN_TIME + " TEXT,"
                + BAN_USER + " INTEGER,"
                + BAN_IP + " TEXT,"
                + BAN_DURATION + " INTEGER,"
                + BAN_REASON + " TEXT,"
                + BAN_BANNER + " INTEGER"
                + ");";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }
        private void CreateCharacterQuestsTable()
        {
            var cmd = "CREATE TABLE " + CHAR_QUESTS_TABLE + " ("
                + CHAR_QUEST_CHAR_ID + " INTEGER,"
                + CHAR_QUEST_ID + " INTEGER,"
                + CHAR_QUEST_TASK + " INTEGER,"
                + CHAR_QUEST_TASK_PROGRESS + " INTEGER,"
                + CHAR_QUEST_COMPLETED + " INTEGER,"
                + " unique('" + CHAR_QUEST_CHAR_ID + "','" + CHAR_QUEST_ID + "')"
                + ");";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }
    }
}
