using System;
using System.Drawing;
using System.IO;

using Intersect.Configuration;

using Mono.Data.Sqlite;

namespace Intersect.Editor
{

    public static class Database
    {

        private const string DB_FILENAME = "resources/mapcache.db";

        private const string MAP_CACHE_DATA = "data";

        private const string MAP_CACHE_ID = "id";

        private const string MAP_CACHE_REVISION = "revision";

        //Map Table Constants
        private const string MAP_CACHE_TABLE = "mapcache";

        private const string OPTION_ID = "id";

        private const string OPTION_NAME = "name";

        //Options Constants
        private const string OPTION_TABLE = "Options";

        private const string OPTION_VALUE = "value";

        //Grid Variables
        public static bool GridHideDarkness;

        public static bool GridHideFog;

        public static bool GridHideOverlay;

        public static bool GridHideResources;

        public static bool GridHideEvents;

        public static int GridLightColor = System.Drawing.Color.White.ToArgb();

        private static SqliteConnection sDbConnection;

        //Options File
        public static void LoadOptions()
        {
            if (!Directory.Exists("resources"))
            {
                Directory.CreateDirectory("resources");
            }

            /* Load configuration */
            ClientConfiguration.LoadAndSave(ClientConfiguration.DefaultPath);
        }

        //Map Cache DB
        public static void InitMapCache()
        {
            if (!Directory.Exists("resources"))
            {
                Directory.CreateDirectory("resources");
            }

            SqliteConnection.SetConfig(SQLiteConfig.Serialized);
            if (!File.Exists(DB_FILENAME))
            {
                CreateDatabase();
            }

            if (sDbConnection == null)
            {
                sDbConnection = new SqliteConnection("Data Source=" + DB_FILENAME + ",Version=3");
                sDbConnection.Open();
            }

            GridHideDarkness = GetOptionBool("HideDarkness");
            GridHideFog = GetOptionBool("HideFog");
            GridHideOverlay = GetOptionBool("HideOverlay");
            GridLightColor = GetOptionInt("LightColor");
            GridHideResources = GetOptionBool("HideResources");
            GridHideEvents = GetOptionBool("HideEvents");
        }

        private static void CreateDatabase()
        {
            sDbConnection = new SqliteConnection("Data Source=" + DB_FILENAME + ",Version=3,New=True");
            sDbConnection.Open();
            CreateOptionsTable();
            CreateMapCacheTable();
        }

        public static void CreateOptionsTable()
        {
            var cmd = "CREATE TABLE " +
                      OPTION_TABLE +
                      " (" +
                      OPTION_ID +
                      " INTEGER PRIMARY KEY," +
                      OPTION_NAME +
                      " TEXT UNIQUE," +
                      OPTION_VALUE +
                      " TEXT" +
                      ");";

            using (var createCommand = sDbConnection.CreateCommand())
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
            SaveOption("HideEvents", GridHideEvents.ToString());
            SaveOption("LightColor", GridLightColor.ToString());
        }

        public static void SaveOption(string name, string value)
        {
            var query = "INSERT OR REPLACE into " +
                        OPTION_TABLE +
                        " (" +
                        OPTION_NAME +
                        "," +
                        OPTION_VALUE +
                        ")" +
                        " VALUES " +
                        " (@" +
                        OPTION_NAME +
                        ",@" +
                        OPTION_VALUE +
                        ");";

            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + OPTION_NAME, name));
                cmd.Parameters.Add(new SqliteParameter("@" + OPTION_VALUE, value));
                cmd.ExecuteNonQuery();
            }
        }

        public static string GetOptionStr(string name)
        {
            var query = "SELECT * from " + OPTION_TABLE + " WHERE " + OPTION_NAME + "=@" + OPTION_NAME + ";";
            using (var cmd = new SqliteCommand(query, sDbConnection))
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
            var cmd = "CREATE TABLE " +
                      MAP_CACHE_TABLE +
                      " (" +
                      MAP_CACHE_ID +
                      " TEXT PRIMARY KEY," +
                      MAP_CACHE_REVISION +
                      " INTEGER," +
                      MAP_CACHE_DATA +
                      " BLOB" +
                      ");";

            using (var createCommand = sDbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }

        public static int[] LoadMapCache(Guid id, int revision, int w, int h)
        {
            var data = LoadMapCacheRaw(id, revision);
            if (data != null)
            {
                using (var ms = new MemoryStream(data))
                {
                    var bmp = new Bitmap(Image.FromStream(ms), w, h);

                    //Gonna do really sketchy probably broken math here -- yolo
                    var imgData = new int[bmp.Width * bmp.Height];

                    unsafe
                    {
                        // lock bitmap
                        var origdata = bmp.LockBits(
                            new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
                            bmp.PixelFormat
                        );

                        var byteData = (uint*) origdata.Scan0;

                        // Switch bgra -> rgba
                        for (var i = 0; i < imgData.Length; i++)
                        {
                            byteData[i] = (byteData[i] & 0x000000ff) << 16 |
                                          (byteData[i] & 0x0000FF00) |
                                          (byteData[i] & 0x00FF0000) >> 16 |
                                          (byteData[i] & 0xFF000000);
                        }

                        // copy data
                        System.Runtime.InteropServices.Marshal.Copy(origdata.Scan0, imgData, 0, bmp.Width * bmp.Height);

                        byteData = null;

                        // unlock bitmap
                        bmp.UnlockBits(origdata);
                    }

                    var result = new byte[imgData.Length * sizeof(int)];
                    Buffer.BlockCopy(imgData, 0, result, 0, result.Length);

                    return imgData;
                }
            }

            return null;
        }

        public static Image LoadMapCacheLegacy(Guid id, int revision)
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

        public static byte[] LoadMapCacheRaw(Guid id, int revision)
        {
            var query = "SELECT * from " + MAP_CACHE_TABLE + " WHERE " + MAP_CACHE_ID + "=@" + MAP_CACHE_ID;
            if (revision > -1)
            {
                query += " AND " + MAP_CACHE_REVISION + "=@" + MAP_CACHE_REVISION;
            }

            using (var cmd = new SqliteCommand(query, sDbConnection))
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

        public static void SaveMapCache(Guid id, int revision, byte[] data)
        {
            var query = "INSERT OR REPLACE into " +
                        MAP_CACHE_TABLE +
                        " (" +
                        MAP_CACHE_ID +
                        "," +
                        MAP_CACHE_REVISION +
                        "," +
                        MAP_CACHE_DATA +
                        ")" +
                        " VALUES " +
                        " (@" +
                        MAP_CACHE_ID +
                        ",@" +
                        MAP_CACHE_REVISION +
                        ",@" +
                        MAP_CACHE_DATA +
                        ");";

            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_CACHE_ID, id.ToString()));
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

        public static void ClearMapCache(Guid id)
        {
            var query = "UPDATE " +
                        MAP_CACHE_TABLE +
                        " SET " +
                        MAP_CACHE_DATA +
                        " = @" +
                        MAP_CACHE_DATA +
                        " WHERE " +
                        MAP_CACHE_ID +
                        " = @" +
                        MAP_CACHE_ID;

            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_CACHE_ID, id.ToString()));
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_CACHE_DATA, null));
                cmd.ExecuteNonQuery();
            }
        }

        public static void ClearAllMapCache()
        {
            var query = "UPDATE " + MAP_CACHE_TABLE + " SET " + MAP_CACHE_DATA + " = @" + MAP_CACHE_DATA;
            using (var cmd = new SqliteCommand(query, sDbConnection))
            {
                cmd.Parameters.Add(new SqliteParameter("@" + MAP_CACHE_DATA, null));
                cmd.ExecuteNonQuery();
            }
        }

    }

}
