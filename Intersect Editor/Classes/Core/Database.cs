using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect_Editor.Classes.Maps;
using Mono.Data.Sqlite;

namespace Intersect_Editor.Classes
{
    public static class Database
    {
        private const string DbFilename = "resources/mapcache.db";

        //Map Table Constants
        private const string MAP_CACHE_TABLE = "mapcache";
        private const string MAP_CACHE_ID = "id";
        private const string MAP_CACHE_REVISION = "revision";
        private const string MAP_CACHE_DATA = "data";

        //Options Constants
        private const string OPTION_TABLE = "options";
        private const string OPTION_ID = "id";
        private const string OPTION_NAME = "name";
        private const string OPTION_VALUE = "value";
        private static SqliteConnection _dbConnection;

        //Grid Variables
        public static bool GridHideDarkness = false;
        public static bool GridHideFog = false;
        public static bool GridHideOverlay = false;
        public static bool GridHideResources = false;
        public static int GridLightColor = System.Drawing.Color.White.ToArgb();

        //Options File
        public static bool LoadOptions()
        {
            if (!Directory.Exists("resources")) Directory.CreateDirectory("resources");
            if (!File.Exists("resources/config.xml"))
            {
                var settings = new XmlWriterSettings {Indent = true};
                using (var writer = XmlWriter.Create("resources/config.xml", settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteComment("Config.xml generated automatically by Intersect Game Engine.");
                    writer.WriteStartElement("Config");
                    writer.WriteElementString("Language", "English");
                    writer.WriteElementString("Host", "localhost");
                    writer.WriteElementString("Port", "4500");
                    writer.WriteElementString("RenderCache", "true");
                        //Not used by the editor, but created here just in case we ever want to share a resource folder with a client.
                    writer.WriteElementString("MenuBGM", "");
                        //Not used by the editor, but created here just in case we ever want to share a resource folder with a client.
                    writer.WriteElementString("MenuBG", "background.png");
                        //Not used by the editor, but created here just in case we ever want to share a resource folder with a client.
                    writer.WriteElementString("Logo", "logo.png");
                        //Not used by the editor, but created here just in case we ever want to share a resource folder with a client.
                    writer.WriteElementString("IntroBG", "");
                        //Not used by the editor, but created here just in case we ever want to share a resource folder with a client.
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                }
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.LoadXml(File.ReadAllText("resources/config.xml"));
                    Options.Language = "English";
                    if (xmlDoc.SelectSingleNode("//Config/Language") != null)
                    {
                        Options.Language = xmlDoc.SelectSingleNode("//Config/Language").InnerText;
                    }
                    Globals.ServerHost = xmlDoc.SelectSingleNode("//Config/Host").InnerText;
                    Globals.ServerPort = int.Parse(xmlDoc.SelectSingleNode("//Config/Port").InnerText);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        //Game Object Handling
        public static string[] GetGameObjectList(GameObjectType type)
        {
            var items = new List<string>();
            switch (type)
            {
                case GameObjectType.Animation:
                    foreach (var obj in AnimationBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.Class:
                    foreach (var obj in ClassBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.Item:
                    foreach (var obj in ItemBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.Npc:
                    foreach (var obj in NpcBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.Projectile:
                    foreach (var obj in ProjectileBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.Quest:
                    foreach (var obj in QuestBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.Resource:
                    foreach (var obj in ResourceBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.Shop:
                    foreach (var obj in ShopBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.Spell:
                    foreach (var obj in SpellBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.Bench:
                    foreach (var obj in BenchBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.Map:
                    foreach (var obj in MapInstance.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.CommonEvent:
                    foreach (var obj in EventBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.PlayerSwitch:
                    foreach (var obj in PlayerSwitchBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.PlayerVariable:
                    foreach (var obj in PlayerVariableBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.ServerSwitch:
                    foreach (var obj in ServerSwitchBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.ServerVariable:
                    foreach (var obj in ServerVariableBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                case GameObjectType.Tileset:
                    foreach (var obj in TilesetBase.Lookup.Copy)
                        items.Add(obj.Value.Name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return items.ToArray();
        }

        public static int GameObjectIdFromList(GameObjectType type, int listIndex)
        {
            if (listIndex < 0) return -1;
            switch (type)
            {
                case GameObjectType.Animation:
                    if (listIndex >= AnimationBase.Lookup.Count) return -1;
                    return AnimationBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.Class:
                    if (listIndex >= ClassBase.Lookup.Count) return -1;
                    return ClassBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.Item:
                    if (listIndex >= ItemBase.Lookup.Count) return -1;
                    return ItemBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.Npc:
                    if (listIndex >= NpcBase.Lookup.Count) return -1;
                    return NpcBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.Projectile:
                    if (listIndex >= ProjectileBase.Lookup.Count) return -1;
                    return ProjectileBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.Quest:
                    if (listIndex >= QuestBase.Lookup.Count) return -1;
                    return QuestBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.Resource:
                    if (listIndex >= ResourceBase.Lookup.Count) return -1;
                    return ResourceBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.Shop:
                    if (listIndex >= ShopBase.Lookup.Count) return -1;
                    return ShopBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.Spell:
                    if (listIndex >= SpellBase.Lookup.Count) return -1;
                    return SpellBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.Bench:
                    if (listIndex >= BenchBase.Lookup.Count) return -1;
                    return BenchBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.Map:
                    if (listIndex >= MapBase.Lookup.Count) return -1;
                    return MapBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.CommonEvent:
                    if (listIndex >= EventBase.Lookup.Count) return -1;
                    return EventBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.PlayerSwitch:
                    if (listIndex >= PlayerSwitchBase.Lookup.Count) return -1;
                    return PlayerSwitchBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.PlayerVariable:
                    if (listIndex >= PlayerVariableBase.Lookup.Count) return -1;
                    return PlayerVariableBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.ServerSwitch:
                    if (listIndex >= ServerSwitchBase.Lookup.Count) return -1;
                    return ServerSwitchBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.ServerVariable:
                    if (listIndex >= ServerVariableBase.Lookup.Count) return -1;
                    return ServerVariableBase.Lookup.Keys.ToList()[listIndex];
                case GameObjectType.Tileset:
                    if (listIndex >= TilesetBase.Lookup.Count) return -1;
                    return TilesetBase.Lookup.Keys.ToList()[listIndex];
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static int GameObjectListIndex(GameObjectType type, int id)
        {
            switch (type)
            {
                case GameObjectType.Animation:
                    return AnimationBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.Class:
                    return ClassBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.Item:
                    return ItemBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.Npc:
                    return NpcBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.Projectile:
                    return ProjectileBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.Quest:
                    return QuestBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.Resource:
                    return ResourceBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.Shop:
                    return ShopBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.Spell:
                    return SpellBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.Bench:
                    return BenchBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.Map:
                    return MapBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.CommonEvent:
                    return EventBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.PlayerSwitch:
                    return PlayerSwitchBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.PlayerVariable:
                    return PlayerVariableBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.ServerSwitch:
                    return ServerSwitchBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.ServerVariable:
                    return ServerVariableBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObjectType.Tileset:
                    return TilesetBase.Lookup.Keys.ToList().IndexOf(id);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        //Map Cache DB
        public static void InitMapCache()
        {
            if (!Directory.Exists("resources")) Directory.CreateDirectory("resources");
            SqliteConnection.SetConfig(SQLiteConfig.Serialized);
            if (!File.Exists(DbFilename)) CreateDatabase();
            if (_dbConnection == null)
            {
                _dbConnection = new SqliteConnection("Data Source=" + DbFilename + ",Version=3");
                _dbConnection.Open();
            }
            GridHideDarkness = GetOptionBool("HideDarkness");
            GridHideFog = GetOptionBool("HideFog");
            GridHideOverlay = GetOptionBool("HideOverlay");
            GridLightColor = GetOptionInt("LightColor");
            GridHideResources = GetOptionBool("HideResources");
        }

        private static void CreateDatabase()
        {
            _dbConnection = new SqliteConnection("Data Source=" + DbFilename + ",Version=3,New=True");
            _dbConnection.Open();
            CreateOptionsTable();
            CreateMapCacheTable();
        }

        public static void CreateOptionsTable()
        {
            var cmd = "CREATE TABLE " + OPTION_TABLE + " ("
                      + OPTION_ID + " INTEGER PRIMARY KEY,"
                      + OPTION_NAME + " TEXT UNIQUE,"
                      + OPTION_VALUE + " TEXT"
                      + ");";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }

        public static void SaveGridOptions()
        {
            SaveOption("HideDarkness", GridHideDarkness.ToString());
            SaveOption("HideFog", GridHideFog.ToString());
            SaveOption("HideOverlay", GridHideOverlay.ToString());
            SaveOption("HideResources", GridHideResources.ToString());
            SaveOption("LightColor", GridLightColor.ToString());
        }

        public static void SaveOption(string name, string value)
        {
            var query = "INSERT OR REPLACE into " + OPTION_TABLE + " (" + OPTION_NAME + "," +
                        OPTION_VALUE + ")" + " VALUES " + " (@" +
                        OPTION_NAME + ",@" + OPTION_VALUE + ");";
            using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + OPTION_NAME, name));
                cmd.Parameters.Add(new SqliteParameter("@" + OPTION_VALUE, value));
                cmd.ExecuteNonQuery();
            }
        }

        public static string GetOptionStr(string name)
        {
            var query = "SELECT * from " + OPTION_TABLE + " WHERE " + OPTION_NAME + "=@" + OPTION_NAME + ";";
            using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + OPTION_NAME, name));
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    if (dataReader[OPTION_VALUE].GetType() != typeof(DBNull))
                    {
                        var data = (string) dataReader[OPTION_VALUE];
                        return data;
                    }
                }
            }
            return "";
        }

        public static int GetOptionInt(string name)
        {
            var opt = GetOptionStr(name);
            if (opt != "")
            {
                return Convert.ToInt32(opt);
            }
            return -1;
        }

        public static bool GetOptionBool(string name)
        {
            var opt = GetOptionStr(name);
            if (opt != "")
            {
                return Convert.ToBoolean(opt);
            }
            return false;
        }

        public static void CreateMapCacheTable()
        {
            var cmd = "CREATE TABLE " + MAP_CACHE_TABLE + " ("
                      + MAP_CACHE_ID + " INTEGER PRIMARY KEY,"
                      + MAP_CACHE_REVISION + " INTEGER,"
                      + MAP_CACHE_DATA + " BLOB"
                      + ");";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }

        public static int[] LoadMapCache(int id, int revision, int w, int h)
        {
            var data = LoadMapCacheRaw(id, revision);
            if (data != null)
            {
                using (var ms = new MemoryStream(data))
                {
                    var bmp = new Bitmap(Image.FromStream(ms), w, h);
                    //Gonna do really sketchy probably broken math here -- yolo
                    int[] imgData = new int[bmp.Width * bmp.Height];

                    unsafe
                    {
                        // lock bitmap
                        System.Drawing.Imaging.BitmapData origdata =
                            bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

                        uint* byteData = (uint*) origdata.Scan0;

                        // Switch bgra -> rgba
                        for (int i = 0; i < imgData.Length; i++)
                        {
                            byteData[i] = (byteData[i] & 0x000000ff) << 16 | (byteData[i] & 0x0000FF00) |
                                          (byteData[i] & 0x00FF0000) >> 16 | (byteData[i] & 0xFF000000);
                        }

                        // copy data
                        System.Runtime.InteropServices.Marshal.Copy(origdata.Scan0, imgData, 0, bmp.Width * bmp.Height);

                        byteData = null;

                        // unlock bitmap
                        bmp.UnlockBits(origdata);
                    }

                    byte[] result = new byte[imgData.Length * sizeof(int)];
                    Buffer.BlockCopy(imgData, 0, result, 0, result.Length);
                    return imgData;
                }
            }
            return null;
        }

        public static Image LoadMapCacheLegacy(int id, int revision)
        {
            var data = LoadMapCacheRaw(id, revision);
            if (data != null)
            {
                using (var ms = new MemoryStream(data))
                {
                    return Image.FromStream(ms);
                }
            }
            return null;
        }

        public static byte[] LoadMapCacheRaw(int id, int revision)
        {
            var query = "SELECT * from " + MAP_CACHE_TABLE + " WHERE " + MAP_CACHE_ID + "=@" + MAP_CACHE_ID;
            if (revision > -1)
            {
                query += " AND " + MAP_CACHE_REVISION + "=@" + MAP_CACHE_REVISION;
            }

            using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_CACHE_ID, id.ToString()));
                if (revision > -1)
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + MAP_CACHE_REVISION, revision.ToString()));
                }
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    if (dataReader[MAP_CACHE_DATA].GetType() != typeof(DBNull))
                    {
                        var data = (byte[]) dataReader[MAP_CACHE_DATA];
                        return data;
                    }
                }
            }
            return null;
        }

        public static void SaveMapCache(int id, int revision, byte[] data)
        {
            var query = "INSERT OR REPLACE into " + MAP_CACHE_TABLE + " (" + MAP_CACHE_ID + "," +
                        MAP_CACHE_REVISION + "," + MAP_CACHE_DATA + ")" + " VALUES " + " (@" +
                        MAP_CACHE_ID + ",@" + MAP_CACHE_REVISION + ",@" + MAP_CACHE_DATA + ");";
            using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_CACHE_ID, id));
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_CACHE_REVISION, revision));
                if (data != null)
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + MAP_CACHE_DATA, data));
                }
                else
                {
                    cmd.Parameters.Add(new SqliteParameter("@" + MAP_CACHE_DATA, null));
                }
                cmd.ExecuteNonQuery();
            }
        }

        public static void ClearMapCache(int id)
        {
            var query = "UPDATE " + MAP_CACHE_TABLE + " SET " + MAP_CACHE_DATA + " = @" + MAP_CACHE_DATA + " WHERE " +
                        MAP_CACHE_ID + " = @" + MAP_CACHE_ID;
            using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_CACHE_ID, id));
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_CACHE_DATA, null));
                cmd.ExecuteNonQuery();
            }
        }

        public static void ClearAllMapCache()
        {
            var query = "UPDATE " + MAP_CACHE_TABLE + " SET " + MAP_CACHE_DATA + " = @" + MAP_CACHE_DATA;
            using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_CACHE_DATA, null));
                cmd.ExecuteNonQuery();
            }
        }
    }
}