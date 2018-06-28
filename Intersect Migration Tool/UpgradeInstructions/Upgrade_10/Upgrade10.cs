using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib.Enums;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib.GameObjects;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib.GameObjects.Events;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib.GameObjects.Maps;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;
using Mono.Data.Sqlite;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib.Models;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_10
{
    class Upgrade10
    {
        //GameObject Table Constants
        private const string GAME_OBJECT_ID = "id";

        private const string GAME_OBJECT_DELETED = "deleted";
        private const string GAME_OBJECT_DATA = "data";

        private SqliteConnection mDbConnection;
        private object mDbLock = new object();

        public Upgrade10(SqliteConnection connection)
        {
            mDbConnection = connection;
        }

        public void Upgrade()
        {
            //Gotta Load and Save All Events
            ServerOptions.LoadOptions();
            LoadAllGameObjects();
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
            lock (mDbLock)
            {
                var tableName = gameObjectType.GetTable();
                ClearGameObjects(gameObjectType);
                var query = "SELECT * from " + tableName + " WHERE " + GAME_OBJECT_DELETED + "=@" +
                            GAME_OBJECT_DELETED +
                            ";";
                using (SqliteCommand cmd = new SqliteCommand(query, mDbConnection))
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

            lock (mDbLock)
            {
                var insertQuery = "UPDATE " + gameObject.DatabaseTable + " set " + GAME_OBJECT_DELETED + "=@" +
                                  GAME_OBJECT_DELETED + "," + GAME_OBJECT_DATA + "=@" + GAME_OBJECT_DATA + " WHERE " +
                                  GAME_OBJECT_ID + "=@" + GAME_OBJECT_ID + ";";
                using (SqliteCommand cmd = new SqliteCommand(insertQuery, mDbConnection))
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
        }
    }
}
