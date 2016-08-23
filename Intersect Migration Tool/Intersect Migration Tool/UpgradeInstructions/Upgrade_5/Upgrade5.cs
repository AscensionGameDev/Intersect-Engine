using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects;
using Mono.Data.Sqlite;
using System;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects.Events;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects.Maps;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib;

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
                case GameObject.Bench:
                    tableName = BenchBase.DatabaseTable;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return tableName;
        }

    }
}