using System;
using System.IO;
using System.Text;
using System.Xml;
using MySql.Data;
using MySql;
using MySql.Data.MySqlClient;

namespace IntersectServer
{
    public static class Database
    {
        public static MapGrid[] mapGrids;
        public static string cs = "";

        //Check Directories
        public static void CheckDirectories()
        {
            if (!Directory.Exists("resources")) { Directory.CreateDirectory("resources"); }
        }
       

        //Options File
        public static bool LoadOptions()
        {

            if (!File.Exists("resources\\config.xml"))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                XmlWriter writer = XmlWriter.Create("resources\\config.xml", settings);
                writer.WriteStartDocument();
                writer.WriteComment("Config.xml generated automatically by Intersect Client.");
                writer.WriteStartElement("Config");
                writer.WriteElementString("ServerPort", "4500");
                writer.WriteElementString("DBHost", "localhost");
                writer.WriteElementString("DBPort", "3306");
                writer.WriteElementString("DBUser", "root");
                writer.WriteElementString("DBPass", "pass");
                writer.WriteElementString("DBName", "IntersectDB");
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
            else
            {
                XmlDocument options = new XmlDocument();
                try
                {
                    options.Load("resources\\config.xml");
                    GlobalVariables.ServerPort = Int32.Parse(options.SelectSingleNode("//Config/ServerPort").InnerText);
                    GlobalVariables.MySQLHost = options.SelectSingleNode("//Config/DBHost").InnerText;
                    GlobalVariables.MySQLPort = Int32.Parse(options.SelectSingleNode("//Config/DBPort").InnerText);
                    GlobalVariables.MySQLUser = options.SelectSingleNode("//Config/DBUser").InnerText;
                    GlobalVariables.MySQLPass = options.SelectSingleNode("//Config/DBPass").InnerText;
                    GlobalVariables.MySQLDB = options.SelectSingleNode("//Config/DBName").InnerText;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        //MySql
        public static void initMySQL()
        {
            cs = @"server=" + GlobalVariables.MySQLHost + ";userid=" + GlobalVariables.MySQLUser + ";"
            + "password=" + GlobalVariables.MySQLPass + ";database=" + GlobalVariables.MySQLDB + ";";
            try
            {
                using (MySqlConnection mysqlConn = new MySqlConnection(cs))
                {
                    mysqlConn.Open();
                    Console.WriteLine("Connected to MySQL successfully.");
                    Console.WriteLine("Server has " + GetRegisteredPlayers() + " registered players.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Could not connect to the MySQL database. Players will fail to login or create accounts.");
            }
        }
        //Players
        public static bool accountExists(string accountname)
        {
            string stm = "SELECT COUNT(*) from Users WHERE user='" + accountname.ToLower() + "'";
            using (MySqlConnection mysqlConn = new MySqlConnection(cs))
            {
                mysqlConn.Open();

                MySqlCommand cmd = new MySqlCommand(stm, mysqlConn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    count = reader.GetInt32(0);

                }
                reader.Close();
                if (count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool emailInUse(string email)
        {
            string stm = "SELECT COUNT(*) from Users WHERE email='" + email.ToLower() + "'";
            using (MySqlConnection mysqlConn = new MySqlConnection(cs))
            {
                mysqlConn.Open();
                MySqlCommand cmd = new MySqlCommand(stm, mysqlConn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
                reader.Close();
                if (count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static int GetRegisteredPlayers()
        {
            string stm = "SELECT COUNT(*) from Users";
            using (MySqlConnection mysqlConn = new MySqlConnection(cs))
            {
                mysqlConn.Open();
                MySqlCommand cmd = new MySqlCommand(stm, mysqlConn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int result = 0;
                while (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
                reader.Close();
                return result;
            }
        }

        public static void CreateAccount(string username, string password, string email)
        {
            using (MySqlConnection mysqlConn = new MySqlConnection(cs))
            {
                mysqlConn.Open();
                string stm = "INSERT INTO Users (user,pass,email) VALUES ('" + username.ToLower() + "','" + password + "','" + email.ToLower() + "');";
                MySqlCommand cmd = new MySqlCommand(stm, mysqlConn);
                int result = cmd.ExecuteNonQuery();
            }
        }

        public static bool CheckPassword(string username, string password)
        {
            string stm = "SELECT pass FROM Users WHERE user = '" + username + "'";
            using (MySqlConnection mysqlConn = new MySqlConnection(cs))
            {
                mysqlConn.Open();
                MySqlCommand cmd = new MySqlCommand(stm, mysqlConn);
                MySqlDataReader reader = cmd.ExecuteReader();
                bool result = false;
                while (reader.Read())
                {
                    if (reader.GetString(0) == password)
                    {
                        result = true;
                    }
                }
                reader.Close();
                return result;
            }
        }

        public static int GetUserID(string username)
        {
            string stm = "SELECT id FROM Users WHERE user = '" + username + "'";
            using (MySqlConnection mysqlConn = new MySqlConnection(cs))
            {
                mysqlConn.Open();
                MySqlCommand cmd = new MySqlCommand(stm, mysqlConn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int result = -1;
                while (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
                reader.Close();
                return result;
            }
        }

        public static void LoadPlayer(Client client)
        {
            string stm = "SELECT * FROM Users WHERE id = " + client.id + "";
            using (MySqlConnection mysqlConn = new MySqlConnection(cs))
            {
                mysqlConn.Open();
                MySqlCommand cmd = new MySqlCommand(stm, mysqlConn);
                MySqlDataReader reader = cmd.ExecuteReader();
                Entity en = GlobalVariables.entities[client.entityIndex];
                while (reader.Read())
                {
                    reader.GetInt32(0);
                    reader.GetString(1);
                    reader.GetString(2);
                    reader.GetString(3);
                    en.currentMap = reader.GetInt32(4);
                    en.currentX = reader.GetInt32(5);
                    en.currentY = reader.GetInt32(6);
                    en.dir = reader.GetInt32(7);
                    en.mySprite = reader.GetString(8);
                    en.vital[0] = reader.GetInt32(9);
                    en.vital[1] = reader.GetInt32(10);
                    en.maxVital[0] = reader.GetInt32(11);
                    en.maxVital[1] = reader.GetInt32(12);
                    en.stat[0] = reader.GetInt32(13);
                    en.stat[1] = reader.GetInt32(14);
                    en.stat[2] = reader.GetInt32(15);
                    client.power = reader.GetInt32(16);
                }
                reader.Close();


                int i = 0;
                stm = "SELECT switchval from Switches WHERE id=" + client.id + ";";
                cmd = new MySqlCommand(stm, mysqlConn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (i < Constants.SWITCH_COUNT)
                    {
                        ((Player)en).switches[i] = Convert.ToBoolean(reader.GetInt32(0));
                        i++;
                    }
                }
                reader.Close();
                i = 0;
                stm = "SELECT variableval from Variables WHERE id=" + client.id + ";";
                cmd = new MySqlCommand(stm, mysqlConn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (i < Constants.VARIABLE_COUNT)
                    {
                        ((Player)en).variables[i] = reader.GetInt32(0);
                        i++;
                    }
                }
                reader.Close();
            }
        }

        public static void SavePlayer(Client client)
        {
            if (client == null) { return; }
            if (client.entityIndex == -1) { return; }
            if (client.entityIndex >= GlobalVariables.entities.Count) { return; }
            if (GlobalVariables.entities[client.entityIndex] == null) { return; }
            Entity en = GlobalVariables.entities[client.entityIndex];
            string stm = "UPDATE Users SET ";
            int id = GetUserID(en.myName);
            stm += "map=" + en.currentMap + ",";
            stm += "x=" + en.currentX + ",";
            stm += "y=" + en.currentY + ",";
            stm += "dir=" + en.dir + ",";
            stm += "sprite='" + en.mySprite + "',";
            stm += "vital0=" + en.vital[0] + ",";
            stm += "vital1=" + en.vital[1] + ",";
            stm += "maxvital0=" + en.maxVital[0] + ",";
            stm += "maxvital1=" + en.maxVital[1] + ",";
            stm += "stat0=" + en.stat[0] + ",";
            stm += "stat1=" + en.stat[1] + ",";
            stm += "stat2=" + en.stat[2] + ",";
            stm += "power=" + client.power + "";
            stm += " WHERE user='" + en.myName + "'";
            using (MySqlConnection mysqlConn = new MySqlConnection(cs))
            {
                mysqlConn.Open();
                MySqlCommand cmd = new MySqlCommand(stm, mysqlConn);
                int result = cmd.ExecuteNonQuery();

                stm = "SELECT COUNT(*) from Switches WHERE id=" + id;
                cmd = new MySqlCommand(stm, mysqlConn);
                MySqlDataReader reader = cmd.ExecuteReader();
                result = 0;
                while (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
                reader.Close();
                if (result < Constants.SWITCH_COUNT)
                {
                    stm = "";
                    for (int i = result; i < Constants.SWITCH_COUNT; i++)
                    {
                        stm += "INSERT INTO Switches (id,switchnum,switchval) VALUES (" + id + "," + i + ",0);\n";
                    }
                    cmd = new MySqlCommand(stm, mysqlConn);
                    result = cmd.ExecuteNonQuery();
                }
                stm = "";
                for (int i = 0; i < Constants.SWITCH_COUNT; i++)
                {
                    stm += "UPDATE Switches SET switchval=" + Convert.ToInt32(((Player)(en)).switches[i]) + " WHERE id=" + id + " AND switchnum=" + i + ";\n";
                }
                cmd = new MySqlCommand(stm, mysqlConn);
                result = cmd.ExecuteNonQuery();
                stm = "SELECT COUNT(*) from Variables WHERE id=" + id;
                cmd = new MySqlCommand(stm, mysqlConn);
                reader = cmd.ExecuteReader();
                result = 0;
                while (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
                reader.Close();
                if (result < Constants.VARIABLE_COUNT)
                {
                    stm = "";
                    for (int i = result; i < Constants.VARIABLE_COUNT; i++)
                    {
                        stm += "INSERT INTO Variables (id,variablenum,variableval) VALUES (" + id + "," + i + ",0);\n";
                    }
                    cmd = new MySqlCommand(stm, mysqlConn);
                    result = cmd.ExecuteNonQuery();
                }
                stm = "";
                for (int i = 0; i < Constants.VARIABLE_COUNT; i++)
                {
                    stm += "UPDATE Variables SET variableval=" + Convert.ToInt32(((Player)(en)).variables[i]) + " WHERE id=" + id + " AND variablenum=" + i + ";\n";
                }
                cmd = new MySqlCommand(stm, mysqlConn);
                result = cmd.ExecuteNonQuery();
            }
        }


        //Maps
        public static void LoadMaps()
        {
            string[] mapNames;
            if (!Directory.Exists("Resources/Maps"))
            {
                Directory.CreateDirectory("Resources/Maps");
            }
            mapNames = Directory.GetFiles("Resources/Maps", "*.map");
            GlobalVariables.mapCount = mapNames.Length;
            GlobalVariables.GameMaps = new Map[mapNames.Length];
            if (GlobalVariables.mapCount == 0)
            {
                Console.WriteLine("No maps found! - Creating empty first map!");
                GlobalVariables.mapCount = 1;
                GlobalVariables.GameMaps = new Map[1];
                GlobalVariables.GameMaps[0] = new Map(0);
                GlobalVariables.GameMaps[0].Save();
            }
            else
            {
                for (int i = 0; i < mapNames.Length; i++)
                {
                    GlobalVariables.GameMaps[i] = new Map(i);
                    GlobalVariables.GameMaps[i].Load(File.ReadAllBytes("Resources/Maps/" + i + ".map"));
                }
            }
            GenerateMapGrids();
        }
        public static void GenerateMapGrids()
        {
            MapGrid[] tmpGrids;
            int myGrid;
            for (int i = 0; i < GlobalVariables.mapCount; i++)
            {
                if (GlobalVariables.GameMaps[i].deleted == 0)
                {
                    if (mapGrids == null)
                    {
                        mapGrids = new MapGrid[1];
                        mapGrids[0] = new MapGrid(i, 0);
                    }
                    else
                    {
                        for (int y = 0; y < mapGrids.Length; y++)
                        {
                            if (!mapGrids[y].hasMap(i))
                            {
                                if (y == mapGrids.Length - 1)
                                {
                                    tmpGrids = (MapGrid[])mapGrids.Clone();
                                    mapGrids = new MapGrid[tmpGrids.Length + 1];
                                    tmpGrids.CopyTo(mapGrids, 0);
                                    mapGrids[mapGrids.Length - 1] = new MapGrid(i, mapGrids.Length - 1);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < GlobalVariables.mapCount; i++)
            {
                if (GlobalVariables.GameMaps[i].deleted == 0)
                {
                    GlobalVariables.GameMaps[i].surroundingMaps.Clear();
                    myGrid = GlobalVariables.GameMaps[i].mapGrid;
                    for (int x = GlobalVariables.GameMaps[i].mapGridX - 1; x <= GlobalVariables.GameMaps[i].mapGridX + 1; x++)
                    {
                        for (int y = GlobalVariables.GameMaps[i].mapGridY - 1; y <= GlobalVariables.GameMaps[i].mapGridY + 1; y++)
                        {
                            if ((x != GlobalVariables.GameMaps[i].mapGridX) || (y != GlobalVariables.GameMaps[i].mapGridY))
                            {
                                if (mapGrids[myGrid].myGrid[x, y] > -1)
                                {
                                    GlobalVariables.GameMaps[i].surroundingMaps.Add(mapGrids[myGrid].myGrid[x, y]);
                                }
                            }
                        }
                    }
                }
            }
        }
        public static int AddMap()
        {
            Map[] tmpMaps;
            tmpMaps = (Map[])GlobalVariables.GameMaps.Clone();
            GlobalVariables.mapCount++;
            GlobalVariables.GameMaps = new Map[GlobalVariables.mapCount];
            tmpMaps.CopyTo(GlobalVariables.GameMaps, 0);
            GlobalVariables.GameMaps[GlobalVariables.mapCount - 1] = new Map(GlobalVariables.mapCount - 1);
            GlobalVariables.GameMaps[GlobalVariables.mapCount - 1].Save();
            return GlobalVariables.mapCount - 1;
        }

        //Npcs
        public static void LoadFakeNpcs()
        {
            GlobalVariables.npcCount = 1;
            GlobalVariables.GameNpcs = new NPCBase[1];
            GlobalVariables.GameNpcs[0] = new NPCBase(1);
            GlobalVariables.GameNpcs[0].myName = "Slime";
            GlobalVariables.GameNpcs[0].mySprite = "145";
            GlobalVariables.GameNpcs[0].vital[0] = 80;
            GlobalVariables.GameNpcs[0].maxVital[0] = 80;
            GlobalVariables.GameNpcs[0].stat[0] = 8;
            GlobalVariables.GameNpcs[0].stat[1] = 6;
            GlobalVariables.GameNpcs[0].stat[2] = 20;
        }

        public static void LoadNpcs()
        {
            LoadFakeNpcs();
            return;
            string[] npcNames;
            if (!Directory.Exists("Resources/Npcs"))
            {
                Directory.CreateDirectory("Resources/Npcs");
            }
            npcNames = Directory.GetFiles("Resources/Npcs", "*.npc");
            GlobalVariables.npcCount = npcNames.Length;
            GlobalVariables.GameNpcs = new NPCBase[npcNames.Length];
            if (GlobalVariables.npcCount > 0)
            {
                for (int i = 0; i < npcNames.Length; i++)
                {
                    GlobalVariables.GameNpcs[i] = new NPCBase(i);
                    GlobalVariables.GameNpcs[i].Load(File.ReadAllBytes("Resources/Npcs/" + i + ".npc"));
                }
            }
        }
    }
}

