using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;
using Intersect;
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
        public static string[] GetGameObjectList(GameObject type)
        {
            var items = new List<string>();
            switch (type)
            {
                case GameObject.Animation:
                    foreach (var obj in AnimationBase.Lookup)
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Class:
                    foreach (var obj in ClassBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Item:
                    foreach (var obj in ItemBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Npc:
                    foreach (var obj in NpcBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Projectile:
                    foreach (var obj in ProjectileBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Quest:
                    foreach (var obj in QuestBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Resource:
                    foreach (var obj in ResourceBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Shop:
                    foreach (var obj in ShopBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Spell:
                    foreach (var obj in SpellBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Bench:
                    foreach (var obj in BenchBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Map:
                    foreach (var obj in MapInstance.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.CommonEvent:
                    foreach (var obj in EventBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.PlayerSwitch:
                    foreach (var obj in PlayerSwitchBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.PlayerVariable:
                    foreach (var obj in PlayerVariableBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.ServerSwitch:
                    foreach (var obj in ServerSwitchBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.ServerVariable:
                    foreach (var obj in ServerVariableBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Tileset:
                    foreach (var obj in TilesetBase.Lookup)
                        items.Add(obj.Value.Name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return items.ToArray();
        }

        public static int GameObjectIdFromList(GameObject type, int listIndex)
        {
            if (listIndex < 0) return -1;
            switch (type)
            {
                case GameObject.Animation:
                    if (listIndex >= AnimationBase.Lookup.Count) return -1;
                    return AnimationBase.Lookup.Keys.ToList()[listIndex];
                case GameObject.Class:
                    if (listIndex >= ClassBase.ObjectCount()) return -1;
                    return ClassBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Item:
                    if (listIndex >= ItemBase.ObjectCount()) return -1;
                    return ItemBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Npc:
                    if (listIndex >= NpcBase.ObjectCount()) return -1;
                    return NpcBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Projectile:
                    if (listIndex >= ProjectileBase.ObjectCount()) return -1;
                    return ProjectileBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Quest:
                    if (listIndex >= QuestBase.ObjectCount()) return -1;
                    return QuestBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Resource:
                    if (listIndex >= ResourceBase.ObjectCount()) return -1;
                    return ResourceBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Shop:
                    if (listIndex >= ShopBase.ObjectCount()) return -1;
                    return ShopBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Spell:
                    if (listIndex >= SpellBase.ObjectCount()) return -1;
                    return SpellBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Bench:
                    if (listIndex >= BenchBase.ObjectCount()) return -1;
                    return BenchBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Map:
                    if (listIndex >= MapBase.ObjectCount()) return -1;
                    return MapBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.CommonEvent:
                    if (listIndex >= EventBase.ObjectCount()) return -1;
                    return EventBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.PlayerSwitch:
                    if (listIndex >= PlayerSwitchBase.ObjectCount()) return -1;
                    return PlayerSwitchBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.PlayerVariable:
                    if (listIndex >= PlayerVariableBase.ObjectCount()) return -1;
                    return PlayerVariableBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.ServerSwitch:
                    if (listIndex >= ServerSwitchBase.ObjectCount()) return -1;
                    return ServerSwitchBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.ServerVariable:
                    if (listIndex >= ServerVariableBase.ObjectCount()) return -1;
                    return ServerVariableBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Tileset:
                    if (listIndex >= TilesetBase.Lookup.Count) return -1;
                    return TilesetBase.Lookup.Keys.ToList()[listIndex];
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static int GameObjectListIndex(GameObject type, int id)
        {
            switch (type)
            {
                case GameObject.Animation:
                    return AnimationBase.Lookup.Keys.ToList().IndexOf(id);
                case GameObject.Class:
                    return ClassBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Item:
                    return ItemBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Npc:
                    return NpcBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Projectile:
                    return ProjectileBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Quest:
                    return QuestBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Resource:
                    return ResourceBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Shop:
                    return ShopBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Spell:
                    return SpellBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Bench:
                    return BenchBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Map:
                    return MapBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.CommonEvent:
                    return EventBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.PlayerSwitch:
                    return PlayerSwitchBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.PlayerVariable:
                    return PlayerVariableBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.ServerSwitch:
                    return ServerSwitchBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.ServerVariable:
                    return ServerVariableBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Tileset:
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
                cmd.Parameters.Add(new SqliteParameter("@" + OPTION_VALUE, value.ToString()));
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