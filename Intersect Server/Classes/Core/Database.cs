using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using MySql.Data.MySqlClient;

namespace Intersect_Server.Classes
{
    public static class Database
    {
        public static MapGrid[] MapGrids;
        public static string ConnectionString = "";

        private enum MySqlFields
        {
            m_string = 0,
            m_int
        }

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
                var settings = new XmlWriterSettings { Indent = true };
                var writer = XmlWriter.Create("resources\\config.xml", settings);
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
                var options = new XmlDocument();
                try
                {
                    options.Load("resources\\config.xml");
                    var selectSingleNode = options.SelectSingleNode("//Config/ServerPort");
                    if (selectSingleNode != null)
                        Globals.ServerPort = Int32.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/DBHost");
                    if (selectSingleNode != null)
                        Globals.MySqlHost = selectSingleNode.InnerText;
                    selectSingleNode = options.SelectSingleNode("//Config/DBPort");
                    if (selectSingleNode != null)
                        Globals.MySqlPort = Int32.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/DBUser");
                    if (selectSingleNode != null)
                        Globals.MySqlUser = selectSingleNode.InnerText;
                    selectSingleNode = options.SelectSingleNode("//Config/DBPass");
                    if (selectSingleNode != null)
                        Globals.MySqlPass = selectSingleNode.InnerText;
                    selectSingleNode = options.SelectSingleNode("//Config/DBName");
                    if (selectSingleNode != null)
                        Globals.MySqldb = selectSingleNode.InnerText;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        //MySql
        public static void InitMySql()
        {
            ConnectionString = @"server=" + Globals.MySqlHost + ";userid=" + Globals.MySqlUser + ";"
            + "password=" + Globals.MySqlPass + ";";
            try
            {
                using (var mysqlConn = new MySqlConnection(ConnectionString))
                {
                    mysqlConn.Open();
                    var query = "CREATE SCHEMA IF NOT EXISTS `" + Globals.MySqldb + "`";
                    var cmd = new MySqlCommand(query, mysqlConn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                //ignore
            }
            ConnectionString = @"server=" + Globals.MySqlHost + ";userid=" + Globals.MySqlUser + ";"
            + "password=" + Globals.MySqlPass + ";database=" + Globals.MySqldb + ";";
            try
            {
                using (var mysqlConn = new MySqlConnection(ConnectionString))
                {
                    mysqlConn.Open();
                    Console.WriteLine("Connected to MySQL successfully.");
                    Console.WriteLine("Checking table integrity.");
                    CheckTables();
                    Console.WriteLine("Server has " + GetRegisteredPlayers() + " registered players.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Could not connect to the MySQL database. Players will fail to login or create accounts.");
            }
        }

        private static void CheckTables()
        {

            using (var mysqlConn = new MySqlConnection(ConnectionString))
            {
                mysqlConn.Open();
                CheckUsersTable(mysqlConn);
                CheckSwitchesTable(mysqlConn);
                CheckVariablesTable(mysqlConn);
                CheckInventoryTable(mysqlConn);
                CheckSpellsTable(mysqlConn);
                CheckHotbarTable(mysqlConn);

            }
        }

        private static void CheckUsersTable(MySqlConnection mysqlConn)
        {
            const string myTable = "users";
            var query = "CREATE TABLE IF NOT EXISTS `" + myTable + "` (`id` int(11) NOT NULL AUTO_INCREMENT,PRIMARY KEY (`id`))";
            var cmd = new MySqlCommand(query, mysqlConn);
            cmd.ExecuteNonQuery();
            query = "SHOW COLUMNS FROM " + myTable + ";";
            cmd = new MySqlCommand(query, mysqlConn);
            var reader = cmd.ExecuteReader();
            var columns = new List<string>();
            while (reader.Read())
            {
                columns.Add(reader.GetValue(0).ToString());
            }
            reader.Close();

            //Work on each field
            CheckTableField(mysqlConn, columns, "user", myTable, MySqlFields.m_string, 45);
            CheckTableField(mysqlConn, columns, "pass", myTable, MySqlFields.m_string, 45);
            CheckTableField(mysqlConn, columns, "email", myTable, MySqlFields.m_string, 100);
            CheckTableField(mysqlConn, columns, "map", myTable, MySqlFields.m_int);
            CheckTableField(mysqlConn, columns, "x", myTable, MySqlFields.m_int);
            CheckTableField(mysqlConn, columns, "y", myTable, MySqlFields.m_int);
            CheckTableField(mysqlConn, columns, "dir", myTable, MySqlFields.m_int);
            CheckTableField(mysqlConn, columns, "sprite", myTable, MySqlFields.m_string, 45);
            for (var i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                CheckTableField(mysqlConn, columns, "vital" + i, myTable, MySqlFields.m_int);
                CheckTableField(mysqlConn, columns, "maxvital" + i, myTable, MySqlFields.m_int);
            }
            for (var i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                CheckTableField(mysqlConn, columns, "stat" + i, myTable, MySqlFields.m_int);
            }
            CheckTableField(mysqlConn, columns, "statpoints", myTable, MySqlFields.m_int);
            for (var i = 0; i < Enums.EquipmentSlots.Count; i++)
            {
                CheckTableField(mysqlConn, columns, "equipment" + i, myTable, MySqlFields.m_int);
            }
            CheckTableField(mysqlConn, columns, "power", myTable, MySqlFields.m_int);
            CheckTableField(mysqlConn, columns, "face", myTable, MySqlFields.m_string);
        }

        private static void CheckSwitchesTable(MySqlConnection mysqlConn)
        {
            const string myTable = "switches";
            var query = "CREATE TABLE IF NOT EXISTS `" + myTable + "` (`id` int(11) NOT NULL)";
            var cmd = new MySqlCommand(query, mysqlConn);
            cmd.ExecuteNonQuery();
            query = "SHOW COLUMNS FROM " + myTable + ";";
            cmd = new MySqlCommand(query, mysqlConn);
            var reader = cmd.ExecuteReader();
            var columns = new List<string>();
            while (reader.Read())
            {
                columns.Add(reader.GetValue(0).ToString());
            }
            reader.Close();

            //Work on each field
            CheckTableField(mysqlConn, columns, "switchnum", myTable, MySqlFields.m_int);
            CheckTableField(mysqlConn, columns, "switchval", myTable, MySqlFields.m_int);
        }

        private static void CheckVariablesTable(MySqlConnection mysqlConn)
        {
            const string myTable = "variables";
            var query = "CREATE TABLE IF NOT EXISTS `" + myTable + "` (`id` int(11) NOT NULL)";
            var cmd = new MySqlCommand(query, mysqlConn);
            cmd.ExecuteNonQuery();
            query = "SHOW COLUMNS FROM " + myTable + ";";
            cmd = new MySqlCommand(query, mysqlConn);
            var reader = cmd.ExecuteReader();
            var columns = new List<string>();
            while (reader.Read())
            {
                columns.Add(reader.GetValue(0).ToString());
            }
            reader.Close();

            //Work on each field
            CheckTableField(mysqlConn, columns, "variablenum", myTable, MySqlFields.m_int);
            CheckTableField(mysqlConn, columns, "variableval", myTable, MySqlFields.m_int);
        }

        private static void CheckInventoryTable(MySqlConnection mysqlConn)
        {
            const string myTable = "inventories";
            var query = "CREATE TABLE IF NOT EXISTS `" + myTable + "` (`id` int(11) NOT NULL)";
            var cmd = new MySqlCommand(query, mysqlConn);
            cmd.ExecuteNonQuery();
            query = "SHOW COLUMNS FROM " + myTable + ";";
            cmd = new MySqlCommand(query, mysqlConn);
            var reader = cmd.ExecuteReader();
            var columns = new List<string>();
            while (reader.Read())
            {
                columns.Add(reader.GetValue(0).ToString());
            }
            reader.Close();

            //Work on each field
            CheckTableField(mysqlConn, columns, "slot", myTable, MySqlFields.m_int);
            CheckTableField(mysqlConn, columns, "itemnum", myTable, MySqlFields.m_int);
            CheckTableField(mysqlConn, columns, "itemval", myTable, MySqlFields.m_int);
            for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                CheckTableField(mysqlConn, columns, "statbuff" + i, myTable, MySqlFields.m_int);
            }
        }

        private static void CheckSpellsTable(MySqlConnection mysqlConn)
        {
            const string myTable = "spells";
            var query = "CREATE TABLE IF NOT EXISTS `" + myTable + "` (`id` int(11) NOT NULL)";
            var cmd = new MySqlCommand(query, mysqlConn);
            cmd.ExecuteNonQuery();
            query = "SHOW COLUMNS FROM " + myTable + ";";
            cmd = new MySqlCommand(query, mysqlConn);
            var reader = cmd.ExecuteReader();
            var columns = new List<string>();
            while (reader.Read())
            {
                columns.Add(reader.GetValue(0).ToString());
            }
            reader.Close();

            //Work on each field
            CheckTableField(mysqlConn, columns, "slot", myTable, MySqlFields.m_int);
            CheckTableField(mysqlConn, columns, "spellnum", myTable, MySqlFields.m_int);
            CheckTableField(mysqlConn, columns, "spellcd", myTable, MySqlFields.m_int);
        }

        private static void CheckHotbarTable(MySqlConnection mysqlConn)
        {
            const string myTable = "hotbar";
            var query = "CREATE TABLE IF NOT EXISTS `" + myTable + "` (`id` int(11) NOT NULL)";
            var cmd = new MySqlCommand(query, mysqlConn);
            cmd.ExecuteNonQuery();
            query = "SHOW COLUMNS FROM " + myTable + ";";
            cmd = new MySqlCommand(query, mysqlConn);
            var reader = cmd.ExecuteReader();
            var columns = new List<string>();
            while (reader.Read())
            {
                columns.Add(reader.GetValue(0).ToString());
            }
            reader.Close();

            //Work on each field
            CheckTableField(mysqlConn, columns, "slot", myTable, MySqlFields.m_int);
            CheckTableField(mysqlConn, columns, "itemtype", myTable, MySqlFields.m_int);
            CheckTableField(mysqlConn, columns, "itemslot", myTable, MySqlFields.m_int);
        }

        private static void CheckTableField(MySqlConnection mysqlConn, List<string> columns, string fieldName, string tableName, MySqlFields fieldType, int fieldLength = -1)
        {
            var query = "";
            MySqlCommand cmd;
            if (columns.Contains(fieldName)) { return; }
            switch (fieldType)
            {
                case MySqlFields.m_string:
                    if (fieldLength <= 0) { fieldLength = 100; }
                    query = "ALTER TABLE `" + tableName + "` ADD " + fieldName + " varchar(" + fieldLength + ");";
                    cmd = new MySqlCommand(query, mysqlConn);
                    cmd.ExecuteNonQuery();
                    break;
                case MySqlFields.m_int:
                    if (fieldLength <= 0) { fieldLength = 11; }
                    query = "ALTER TABLE `" + tableName + "` ADD " + fieldName + " int(" + fieldLength + ");";
                    cmd = new MySqlCommand(query, mysqlConn);
                    cmd.ExecuteNonQuery();
                    break;
            }

        }

        //Players
        public static bool AccountExists(string accountname)
        {
            var stm = "SELECT COUNT(*) from Users WHERE user='" + accountname.ToLower() + "'";
            using (var mysqlConn = new MySqlConnection(ConnectionString))
            {
                mysqlConn.Open();

                var cmd = new MySqlCommand(stm, mysqlConn);
                var reader = cmd.ExecuteReader();
                var count = 0;
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

        public static bool EmailInUse(string email)
        {
            var stm = "SELECT COUNT(*) from Users WHERE email='" + email.ToLower() + "'";
            using (var mysqlConn = new MySqlConnection(ConnectionString))
            {
                mysqlConn.Open();
                var cmd = new MySqlCommand(stm, mysqlConn);
                var reader = cmd.ExecuteReader();
                var count = 0;
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
            const string query = "SELECT COUNT(*) from Users";
            using (var mysqlConn = new MySqlConnection(ConnectionString))
            {
                mysqlConn.Open();
                var cmd = new MySqlCommand(query, mysqlConn);
                var reader = cmd.ExecuteReader();
                var result = 0;
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
            using (var mysqlConn = new MySqlConnection(ConnectionString))
            {
                mysqlConn.Open();
                var stm = "INSERT INTO Users (user,pass,email) VALUES ('" + username.ToLower() + "','" + password + "','" + email.ToLower() + "');";
                var cmd = new MySqlCommand(stm, mysqlConn);
                cmd.ExecuteNonQuery();
            }
        }

        public static bool CheckPassword(string username, string password)
        {
            var stm = "SELECT pass FROM Users WHERE user = '" + username + "'";
            using (var mysqlConn = new MySqlConnection(ConnectionString))
            {
                mysqlConn.Open();
                var cmd = new MySqlCommand(stm, mysqlConn);
                var reader = cmd.ExecuteReader();
                var result = false;
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

        public static int GetUserId(string username)
        {
            var stm = "SELECT id FROM Users WHERE user = '" + username + "'";
            using (var mysqlConn = new MySqlConnection(ConnectionString))
            {
                mysqlConn.Open();
                var cmd = new MySqlCommand(stm, mysqlConn);
                var reader = cmd.ExecuteReader();
                var result = -1;
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
            var stm = "SELECT * FROM Users WHERE id = " + client.Id + "";
            using (var mysqlConn = new MySqlConnection(ConnectionString))
            {
                mysqlConn.Open();
                var cmd = new MySqlCommand(stm, mysqlConn);
                var reader = cmd.ExecuteReader();
                var en = client.Entity;
                var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                int i;
                while (reader.Read())
                {
                    en.CurrentMap = reader.GetInt32(columns.IndexOf("map"));
                    en.CurrentX = reader.GetInt32(columns.IndexOf("x"));
                    en.CurrentY = reader.GetInt32(columns.IndexOf("y"));
                    en.Dir = reader.GetInt32(columns.IndexOf("dir"));
                    en.MySprite = reader.GetString(columns.IndexOf("sprite"));
                    for (i = 0; i < (int)Enums.Vitals.VitalCount; i++)
                    {
                        en.Vital[i] = reader.GetInt32(columns.IndexOf("vital" + i));
                        en.MaxVital[i] = reader.GetInt32(columns.IndexOf("maxvital" + i));
                    }
                    for (i = 0; i < (int)Enums.Stats.StatCount; i++)
                    {
                        en.Stat[i] = reader.GetInt32(columns.IndexOf("stat" + i));
                    }
                    en.StatPoints = reader.GetInt32(columns.IndexOf("statpoints"));
                    for (i = 0; i < Enums.EquipmentSlots.Count; i++)
                    {
                        en.Equipment[i] = reader.GetInt32(columns.IndexOf("equipment" + i));
                    }
                    client.Power = reader.GetInt32(columns.IndexOf("power"));
                    en.Face = reader.GetString(columns.IndexOf("face"));
                }
                reader.Close();


                i = 0;
                stm = "SELECT switchval from Switches WHERE id=" + client.Id + ";";
                cmd = new MySqlCommand(stm, mysqlConn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (i >= Constants.SwitchCount) continue;
                    ((Player)en).Switches[i] = Convert.ToBoolean(reader.GetInt32(0));
                    i++;
                }
                reader.Close();
                i = 0;
                stm = "SELECT variableval from Variables WHERE id=" + client.Id + ";";
                cmd = new MySqlCommand(stm, mysqlConn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (i >= Constants.VariableCount) continue;
                    ((Player)en).Variables[i] = reader.GetInt32(0);
                    i++;
                }
                reader.Close();
                i = 0;
                stm = "SELECT * from Inventories WHERE id=" + client.Id + ";";
                cmd = new MySqlCommand(stm, mysqlConn);
                reader = cmd.ExecuteReader();
                columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                while (reader.Read())
                {
                    if (reader.GetInt32(columns.IndexOf("slot")) < Constants.MaxInvItems) {
                        ((Player)en).Inventory[reader.GetInt32(columns.IndexOf("slot"))].ItemNum = reader.GetInt32(columns.IndexOf("itemnum"));
                        ((Player)en).Inventory[reader.GetInt32(columns.IndexOf("slot"))].ItemVal = reader.GetInt32(columns.IndexOf("itemval"));
                        for (int x = 0; x <(int)Enums.Stats.StatCount; x++)
                        {
                            ((Player)en).Inventory[reader.GetInt32(columns.IndexOf("slot"))].StatBoost[x] = reader.GetInt32(columns.IndexOf("statbuff" + x));
                        }
                    }
                }
                reader.Close();
                i = 0;
                stm = "SELECT * from Spells WHERE id=" + client.Id + ";";
                cmd = new MySqlCommand(stm, mysqlConn);
                reader = cmd.ExecuteReader();
                columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                while (reader.Read())
                {
                    if (reader.GetInt32(columns.IndexOf("slot")) < Constants.MaxPlayerSkills)
                    {
                        ((Player)en).Spells[reader.GetInt32(columns.IndexOf("slot"))].SpellNum = reader.GetInt32(columns.IndexOf("spellnum"));
                        ((Player)en).Spells[reader.GetInt32(columns.IndexOf("slot"))].SpellCD = reader.GetInt32(columns.IndexOf("spellcd"));
                    }
                }
                reader.Close();
                i = 0;
                stm = "SELECT * from hotbar WHERE id=" + client.Id + ";";
                cmd = new MySqlCommand(stm, mysqlConn);
                reader = cmd.ExecuteReader();
                columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                while (reader.Read())
                {
                    if (reader.GetInt32(columns.IndexOf("slot")) < Constants.MaxHotbar)
                    {
                        ((Player)en).Hotbar[reader.GetInt32(columns.IndexOf("slot"))].Type = reader.GetInt32(columns.IndexOf("itemtype"));
                        ((Player)en).Hotbar[reader.GetInt32(columns.IndexOf("slot"))].Slot = reader.GetInt32(columns.IndexOf("itemslot"));
                    }
                }
                reader.Close();
            }
        }

        public static void SavePlayer(Client client)
        {
            if (client == null) { return; }
            if (client.EntityIndex == -1) { return; }
            if (client.EntityIndex >= Globals.Entities.Count) { return; }
            if (client.Entity == null) { return; }
            var en = (Player)client.Entity;
            var query = "UPDATE Users SET ";
            var id = GetUserId(en.MyName);
            query += "map=" + en.CurrentMap + ",";
            query += "x=" + en.CurrentX + ",";
            query += "y=" + en.CurrentY + ",";
            query += "dir=" + en.Dir + ",";
            query += "sprite='" + en.MySprite + "',";
            for (var i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                query += "vital" + i + "=" + en.Vital[i] + ",";
                query += "maxvital" + i + "=" + en.MaxVital[i] + ",";
            }
            for (var i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                query += "stat" + i + "=" + en.Stat[i] + ",";
            }
            query += "statpoints=" + client.Entity.StatPoints + ",";
            for (var i = 0; i < Enums.EquipmentSlots.Count; i++)
            {
                query += "equipment" + i + "=" + en.Equipment[i] + ",";
            }
            query += "power=" + client.Power + ",";
            query += "face='" + en.Face + "' ";
            query += " WHERE user='" + en.MyName + "'";
            using (var mysqlConn = new MySqlConnection(ConnectionString))
            {
                mysqlConn.Open();
                var cmd = new MySqlCommand(query, mysqlConn);
                cmd.ExecuteNonQuery();

                //Save Switches
                query = "SELECT COUNT(*) from Switches WHERE id=" + id;
                cmd = new MySqlCommand(query, mysqlConn);
                var reader = cmd.ExecuteReader();
                var result = 0;
                while (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
                reader.Close();
                if (result < Constants.SwitchCount)
                {
                    query = "";
                    for (var i = result; i < Constants.SwitchCount; i++)
                    {
                        query += "INSERT INTO Switches (id,switchnum,switchval) VALUES (" + id + "," + i + ",0);\n";
                    }
                    cmd = new MySqlCommand(query, mysqlConn);
                    cmd.ExecuteNonQuery();
                }
                query = "";
                for (var i = 0; i < Constants.SwitchCount; i++)
                {
                    query += "UPDATE Switches SET switchval=" + Convert.ToInt32(((Player)(en)).Switches[i]) + " WHERE id=" + id + " AND switchnum=" + i + ";\n";
                }
                cmd = new MySqlCommand(query, mysqlConn);
                cmd.ExecuteNonQuery();

                //Save Variables
                query = "SELECT COUNT(*) from Variables WHERE id=" + id;
                cmd = new MySqlCommand(query, mysqlConn);
                reader = cmd.ExecuteReader();
                result = 0;
                while (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
                reader.Close();
                if (result < Constants.VariableCount)
                {
                    query = "";
                    for (var i = result; i < Constants.VariableCount; i++)
                    {
                        query += "INSERT INTO Variables (id,variablenum,variableval) VALUES (" + id + "," + i + ",0);\n";
                    }
                    cmd = new MySqlCommand(query, mysqlConn);
                    cmd.ExecuteNonQuery();
                }
                query = "";
                for (var i = 0; i < Constants.VariableCount; i++)
                {
                    query += "UPDATE Variables SET variableval=" + Convert.ToInt32(((Player)(en)).Variables[i]) + " WHERE id=" + id + " AND variablenum=" + i + ";\n";
                }
                cmd = new MySqlCommand(query, mysqlConn);
                cmd.ExecuteNonQuery();

                //Save Inventory
                query = "SELECT COUNT(*) from Inventories WHERE id=" + id;
                cmd = new MySqlCommand(query, mysqlConn);
                reader = cmd.ExecuteReader();
                result = 0;
                while (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
                reader.Close();
                if (result < Constants.MaxInvItems)
                {
                    query = "";
                    for (var i = result; i < Constants.MaxInvItems; i++)
                    {
                        query += "INSERT INTO Inventories (id,slot,itemnum,itemval";
                        for (int x = 0; x < (int)Enums.Stats.StatCount; x++){
                            query+= ", statbuff" + x;
                        }
                        query += ") VALUES (" + id + "," + i + ",-1,0";
                        for (int x = 0; x < (int)Enums.Stats.StatCount; x++){
                            query+= ", 0";
                        }
                        query += ");\n";
                    }
                    cmd = new MySqlCommand(query, mysqlConn);
                    cmd.ExecuteNonQuery();
                }
                query = "";
                for (var i = 0; i < Constants.MaxInvItems; i++)
                {
                    query += "UPDATE Inventories SET itemnum=" + en.Inventory[i].ItemNum + ", itemval=" + en.Inventory[i].ItemVal;
                    for (int x = 0; x < (int)Enums.Stats.StatCount; x++ )
                    {
                        query += ", statbuff" + x + "=" + en.Inventory[i].StatBoost[x];
                    }
                    query += " WHERE id=" + id + " AND slot=" + i + ";\n";
                }
                cmd = new MySqlCommand(query, mysqlConn);
                cmd.ExecuteNonQuery();

                //Save Spells
                query = "SELECT COUNT(*) from Spells WHERE id=" + id;
                cmd = new MySqlCommand(query, mysqlConn);
                reader = cmd.ExecuteReader();
                result = 0;
                while (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
                reader.Close();
                if (result < Constants.MaxPlayerSkills)
                {
                    query = "";
                    for (var i = result; i < Constants.MaxPlayerSkills; i++)
                    {
                        query += "INSERT INTO Spells (id,slot,spellnum,spellcd) VALUES (" + id + "," + i + ",-1,0);\n";
                    }
                    cmd = new MySqlCommand(query, mysqlConn);
                    cmd.ExecuteNonQuery();
                }
                query = "";
                for (var i = 0; i < Constants.MaxPlayerSkills; i++)
                {
                    query += "UPDATE Spells SET spellnum=" + en.Spells[i].SpellNum + ", spellcd=" + en.Spells[i].SpellCD + " WHERE id=" + id + " AND slot=" + i + ";\n";
                }
                cmd = new MySqlCommand(query, mysqlConn);
                cmd.ExecuteNonQuery();

                //Save Hotbar Slots
                query = "SELECT COUNT(*) from hotbar WHERE id=" + id;
                cmd = new MySqlCommand(query, mysqlConn);
                reader = cmd.ExecuteReader();
                result = 0;
                while (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
                reader.Close();
                if (result < Constants.MaxHotbar)
                {
                    query = "";
                    for (var i = result; i < Constants.MaxPlayerSkills; i++)
                    {
                        query += "INSERT INTO hotbar (id,slot,itemtype,itemslot) VALUES (" + id + "," + i + ",-1,-1);\n";
                    }
                    cmd = new MySqlCommand(query, mysqlConn);
                    cmd.ExecuteNonQuery();
                }
                query = "";
                for (var i = 0; i < Constants.MaxHotbar; i++)
                {
                    query += "UPDATE hotbar SET itemtype=" + en.Hotbar[i].Type + ", itemslot=" + en.Hotbar[i].Slot + " WHERE id=" + id + " AND slot=" + i + ";\n";
                }
                cmd = new MySqlCommand(query, mysqlConn);
                cmd.ExecuteNonQuery();
            }
        }


        //Maps
        public static void LoadMaps()
        {
            if (!Directory.Exists("Resources/Maps"))
            {
                Directory.CreateDirectory("Resources/Maps");
            }
            var mapNames = Directory.GetFiles("Resources/Maps", "*.map");
            Globals.MapCount = mapNames.Length;
            Globals.GameMaps = new MapStruct[mapNames.Length];
            if (Globals.MapCount == 0)
            {
                Console.WriteLine("No maps found! - Creating empty first map!");
                Globals.MapCount = 1;
                Globals.GameMaps = new MapStruct[1];
                Globals.GameMaps[0] = new MapStruct(0);
                Globals.GameMaps[0].Save();
            }
            else
            {
                for (var i = 0; i < mapNames.Length; i++)
                {
                    Globals.GameMaps[i] = new MapStruct(i);
                    Globals.GameMaps[i].Load(File.ReadAllBytes("Resources/Maps/" + i + ".map"));
                }
            }
            GenerateMapGrids();
        }

        public static void GenerateMapGrids()
        {
            for (var i = 0; i < Globals.MapCount; i++)
            {
                if (Globals.GameMaps[i].Deleted != 0) continue;
                if (MapGrids == null)
                {
                    MapGrids = new MapGrid[1];
                    MapGrids[0] = new MapGrid(i, 0);
                }
                else
                {
                    for (var y = 0; y < MapGrids.Length; y++)
                    {
                        if (!MapGrids[y].HasMap(i))
                        {
                            if (y != MapGrids.Length - 1) continue;
                            var tmpGrids = (MapGrid[])MapGrids.Clone();
                            MapGrids = new MapGrid[tmpGrids.Length + 1];
                            tmpGrids.CopyTo(MapGrids, 0);
                            MapGrids[MapGrids.Length - 1] = new MapGrid(i, MapGrids.Length - 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            for (var i = 0; i < Globals.MapCount; i++)
            {
                if (Globals.GameMaps[i].Deleted != 0) continue;
                Globals.GameMaps[i].SurroundingMaps.Clear();
                var myGrid = Globals.GameMaps[i].MapGrid;
                for (var x = Globals.GameMaps[i].MapGridX - 1; x <= Globals.GameMaps[i].MapGridX + 1; x++)
                {
                    for (var y = Globals.GameMaps[i].MapGridY - 1; y <= Globals.GameMaps[i].MapGridY + 1; y++)
                    {
                        if ((x == Globals.GameMaps[i].MapGridX) && (y == Globals.GameMaps[i].MapGridY))
                            continue;
                        if (MapGrids[myGrid].MyGrid[x, y] > -1)
                        {
                            Globals.GameMaps[i].SurroundingMaps.Add(MapGrids[myGrid].MyGrid[x, y]);
                        }
                    }
                }
            }
        }
        public static int AddMap()
        {
            var tmpMaps = (MapStruct[])Globals.GameMaps.Clone();
            Globals.MapCount++;
            Globals.GameMaps = new MapStruct[Globals.MapCount];
            tmpMaps.CopyTo(Globals.GameMaps, 0);
            Globals.GameMaps[Globals.MapCount - 1] = new MapStruct(Globals.MapCount - 1);
            Globals.GameMaps[Globals.MapCount - 1].Save();
            return Globals.MapCount - 1;
        }

        public static void LoadNpcs()
        {
            if (!Directory.Exists("Resources/Npcs"))
            {
                Directory.CreateDirectory("Resources/Npcs");
            }
            Globals.GameNpcs = new NpcStruct[Constants.MaxNpcs];
            for (var i = 0; i < Constants.MaxNpcs; i++)
            {
                Globals.GameNpcs[i] = new NpcStruct();
                if (!File.Exists("Resources/Npcs/" + i + ".npc"))
                {
                    Globals.GameNpcs[i].Save(i);
                }
                else
                {
                    Globals.GameNpcs[i].Load(File.ReadAllBytes("Resources/Npcs/" + i + ".npc"));
                }

            }
        }

        //Items
        public static void LoadItems()
        {
            if (!Directory.Exists("Resources/Items"))
            {
                Directory.CreateDirectory("Resources/Items");
            }

            Globals.GameItems = new ItemStruct[Constants.MaxItems];
            for (var i = 0; i < Constants.MaxItems; i++)
            {
                Globals.GameItems[i] = new ItemStruct();
                if (!File.Exists("Resources/Items/" + i + ".item"))
                {
                    Globals.GameItems[i].Save(i);
                }
                else
                {
                    Globals.GameItems[i].Load(File.ReadAllBytes("Resources/Items/" + i + ".item"));
                    Globals.GameItems[i].Save(i);
                }
            }
        }

        //Spells
        public static void LoadSpells()
        {
            if (!Directory.Exists("Resources/Spells"))
            {
                Directory.CreateDirectory("Resources/Spells");
            }

            Globals.GameSpells = new SpellStruct[Constants.MaxSpells];
            for (var i = 0; i < Constants.MaxSpells; i++)
            {
                Globals.GameSpells[i] = new SpellStruct();
                if (!File.Exists("Resources/Spells/" + i + ".spell"))
                {
                    Globals.GameSpells[i].Save(i);
                }
                else
                {
                    Globals.GameSpells[i].Load(File.ReadAllBytes("Resources/Spells/" + i + ".spell"));
                }
            }
        }

        //Animations
        public static void LoadAnimations()
        {
            if (!Directory.Exists("Resources/Animations"))
            {
                Directory.CreateDirectory("Resources/Animations");
            }

            Globals.GameAnimations = new AnimationStruct[Constants.MaxAnimations];
            for (var i = 0; i < Constants.MaxAnimations; i++)
            {
                Globals.GameAnimations[i] = new AnimationStruct();
                if (!File.Exists("Resources/Animations/" + i + ".anim"))
                {
                    Globals.GameAnimations[i].Save(i);
                }
                else
                {
                    Globals.GameAnimations[i].Load(File.ReadAllBytes("Resources/Animations/" + i + ".anim"));
                }
            }
        }
    }
}

