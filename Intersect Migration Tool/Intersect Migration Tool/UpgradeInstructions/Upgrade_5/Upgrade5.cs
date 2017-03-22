using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects;
using Mono.Data.Sqlite;
using System;
using Intersect_Library;
using Intersect_Library.Logging;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects.Events;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects.Maps;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;
using GameObject = Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObject;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5
{
    public class Upgrade5
    {
        private SqliteConnection _dbConnection;
        private Object _dbLock = new Object();

        //GameObject Table Constants
        private const string GAME_OBJECT_ID = "id";
        private const string GAME_OBJECT_DELETED = "deleted";
        private const string GAME_OBJECT_DATA = "data";




        public Upgrade5(SqliteConnection connection)
        {
            _dbConnection = connection;
        }

        public void Upgrade()
        {
            //Gotta Load and Save EVERYTHING
            //We are loading non-unicode strings and replacing them with unicode ones
            ServerOptions.LoadOptions();
            CreateGameObjectTable(GameObject.Bench);
            LoadAllGameObjects();
        }


        private void CreateGameObjectTable(GameObject gameObject)
        {
            var cmd = "CREATE TABLE " + GetGameObjectTable(gameObject) + " ("
                + GAME_OBJECT_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
                + GAME_OBJECT_DELETED + " INTEGER NOT NULL DEFAULT 0,"
                + GAME_OBJECT_DATA + " BLOB" + ");";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }

        //Game Object Saving/Loading
        private void LoadAllGameObjects()
        {

            foreach (var val in Enum.GetValues(typeof(GameObject)))
            {
                if ((GameObject)val != GameObject.Time)
                {
                    LoadGameObjects((GameObject)val);
                }
            }

        }
        //Game Object Saving/Loading
        private string GetGameObjectTable(GameObject type)
        {
            var tableName = "";
            switch (type)
            {
                case GameObject.Animation:
                    tableName = AnimationBase.DATABASE_TABLE;
                    break;
                case GameObject.Bench:
                    tableName = BenchBase.DATABASE_TABLE;
                    break;
                case GameObject.Class:
                    tableName = ClassBase.DATABASE_TABLE;
                    break;
                case GameObject.Item:
                    tableName = ItemBase.DATABASE_TABLE;
                    break;
                case GameObject.Npc:
                    tableName = NpcBase.DATABASE_TABLE;
                    break;
                case GameObject.Projectile:
                    tableName = ProjectileBase.DATABASE_TABLE;
                    break;
                case GameObject.Quest:
                    tableName = QuestBase.DATABASE_TABLE;
                    break;
                case GameObject.Resource:
                    tableName = ResourceBase.DATABASE_TABLE;
                    break;
                case GameObject.Shop:
                    tableName = ShopBase.DATABASE_TABLE;
                    break;
                case GameObject.Spell:
                    tableName = SpellBase.DATABASE_TABLE;
                    break;
                case GameObject.Map:
                    tableName = MapBase.DATABASE_TABLE;
                    break;
                case GameObject.CommonEvent:
                    tableName = EventBase.DATABASE_TABLE;
                    break;
                case GameObject.PlayerSwitch:
                    tableName = PlayerSwitchBase.DATABASE_TABLE;
                    break;
                case GameObject.PlayerVariable:
                    tableName = PlayerVariableBase.DATABASE_TABLE;
                    break;
                case GameObject.ServerSwitch:
                    tableName = ServerSwitchBase.DATABASE_TABLE;
                    break;
                case GameObject.ServerVariable:
                    tableName = ServerVariableBase.DATABASE_TABLE;
                    break;
                case GameObject.Tileset:
                    tableName = TilesetBase.DATABASE_TABLE;
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
                case GameObject.Bench:
                    BenchBase.ClearObjects();
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
                    SaveGameObject(anim);
                    break;
                case GameObject.Bench:
                    var bench = new BenchBase(index);
                    bench.Load(data);
                    BenchBase.AddObject(index, bench);
                    SaveGameObject(bench);
                    break;
                case GameObject.Class:
                    var cls = new ClassBase(index);
                    cls.Load(data);
                    ClassBase.AddObject(index, cls);
                    SaveGameObject(cls);
                    break;
                case GameObject.Item:
                    var itm = new ItemBase(index);
                    itm.Load(data);
                    ItemBase.AddObject(index, itm);
                    SaveGameObject(itm);
                    break;
                case GameObject.Npc:
                    var npc = new NpcBase(index);
                    npc.Load(data);
                    NpcBase.AddObject(index, npc);
                    SaveGameObject(npc);
                    break;
                case GameObject.Projectile:
                    var proj = new ProjectileBase(index);
                    proj.Load(data);
                    ProjectileBase.AddObject(index, proj);
                    SaveGameObject(proj);
                    break;
                case GameObject.Quest:
                    var qst = new QuestBase(index);
                    qst.Load(data);
                    QuestBase.AddObject(index, qst);
                    SaveGameObject(qst);
                    break;
                case GameObject.Resource:
                    var res = new ResourceBase(index);
                    res.Load(data);
                    ResourceBase.AddObject(index, res);
                    SaveGameObject(res);
                    break;
                case GameObject.Shop:
                    var shp = new ShopBase(index);
                    shp.Load(data);
                    ShopBase.AddObject(index, shp);
                    SaveGameObject(shp);
                    break;
                case GameObject.Spell:
                    var spl = new SpellBase(index);
                    spl.Load(data);
                    SpellBase.AddObject(index, spl);
                    SaveGameObject(spl);
                    break;
                case GameObject.Map:
                    var map = new MapBase(index, false);
                    MapBase.AddObject(index, map);
                    map.Load(data);
                    SaveGameObject(map);
                    break;
                case GameObject.CommonEvent:
                    var buffer = new ByteBuffer();
                    buffer.WriteBytes(data);
                    var evt = new EventBase(index, buffer, true);
                    EventBase.AddObject(index, evt);
                    buffer.Dispose();
                    SaveGameObject(evt);
                    break;
                case GameObject.PlayerSwitch:
                    var pswitch = new PlayerSwitchBase(index);
                    pswitch.Load(data);
                    PlayerSwitchBase.AddObject(index, pswitch);
                    SaveGameObject(pswitch);
                    break;
                case GameObject.PlayerVariable:
                    var pvar = new PlayerVariableBase(index);
                    pvar.Load(data);
                    PlayerVariableBase.AddObject(index, pvar);
                    SaveGameObject(pvar);
                    break;
                case GameObject.ServerSwitch:
                    var sswitch = new ServerSwitchBase(index);
                    sswitch.Load(data);
                    ServerSwitchBase.AddObject(index, sswitch);
                    SaveGameObject(sswitch);
                    break;
                case GameObject.ServerVariable:
                    var svar = new ServerVariableBase(index);
                    svar.Load(data);
                    ServerVariableBase.AddObject(index, svar);
                    SaveGameObject(svar);
                    break;
                case GameObject.Tileset:
                    var tset = new TilesetBase(index);
                    tset.Load(data);
                    TilesetBase.AddObject(index, tset);
                    SaveGameObject(tset);
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
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var index = Convert.ToInt32(dataReader[GAME_OBJECT_ID]);
                            if (dataReader[GAME_OBJECT_DATA].GetType() != typeof(DBNull))
                            {
                                LoadGameObject(type, index, (byte[]) dataReader[GAME_OBJECT_DATA]);
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
        public void SaveGameObject(DatabaseObject gameObject)
        {
            if (gameObject == null)
            {
                Log.Error("Attempted to persist null game object to the database.");
            }

            lock (_dbLock)
            {
                var insertQuery = "UPDATE " + gameObject.GetTable() + " set " + GAME_OBJECT_DELETED + "=@" +
                                  GAME_OBJECT_DELETED + "," + GAME_OBJECT_DATA + "=@" + GAME_OBJECT_DATA + " WHERE " +
                                  GAME_OBJECT_ID + "=@" + GAME_OBJECT_ID + ";";
                using (SqliteCommand cmd = new SqliteCommand(insertQuery, _dbConnection))
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_ID, gameObject.GetId()));
                    cmd.Parameters.Add(new SqliteParameter("@" + GAME_OBJECT_DELETED, 0.ToString()));
                    if (gameObject.GetData() != null)
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

    }
}