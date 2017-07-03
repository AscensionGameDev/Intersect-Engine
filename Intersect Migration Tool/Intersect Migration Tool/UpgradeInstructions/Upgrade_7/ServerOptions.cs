using System;
using System.IO;
using System.Xml;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_7
{
    public static class ServerOptions
    {
        //Config XML String
        public static string ConfigXml = "";
        private static bool ConfigFailed;

        //Misc
        public static int ItemDespawnTime = 15000; //15 seconds
        public static int ItemRespawnTime = 15000; //15 seconds

        //Combat
        public static int RegenTime = 3000; //3 seconds

        //Options File
        public static bool LoadOptions()
        {
            if (!Directory.Exists("resources")) Directory.CreateDirectory("resources");
            if (!File.Exists("resources/config.xml"))
            {
                Console.WriteLine("Configuration file was missing!");
                return false;
            }
            else
            {
                var options = new XmlDocument();
                ConfigXml = File.ReadAllText("resources/config.xml");
                try
                {
                    options.LoadXml(ConfigXml);

                    //General Options
                    Intersect_Convert_Lib.Options.Language = GetXmlStr(options, "//Config/Language", false);
                    if (Intersect_Convert_Lib.Options.Language == "") Intersect_Convert_Lib.Options.Language = "English";
                    Intersect_Convert_Lib.Options.GameName = GetXmlStr(options, "//Config/GameName", false);
                    Intersect_Convert_Lib.Options.ServerPort = GetXmlInt(options, "//Config/ServerPort");

                    //Player Options
                    Intersect_Convert_Lib.Options.MaxStatValue = GetXmlInt(options, "//Config/Player/MaxStat");
                    Intersect_Convert_Lib.Options.MaxLevel = GetXmlInt(options, "//Config/Player/MaxLevel");
                    Intersect_Convert_Lib.Options.MaxInvItems = GetXmlInt(options, "//Config/Player/MaxInventory");
                    Intersect_Convert_Lib.Options.MaxPlayerSkills = GetXmlInt(options, "//Config/Player/MaxSpells");
                    Intersect_Convert_Lib.Options.MaxBankSlots = GetXmlInt(options, "//Config/Player/MaxBank");

                    //Equipment
                    int slot = 0;
                    while (!string.IsNullOrEmpty(GetXmlStr(options, "//Config/Equipment/Slot" + slot, false)))
                    {
                        if (
                            Intersect_Convert_Lib.Options.EquipmentSlots.IndexOf(GetXmlStr(options, "//Config/Equipment/Slot" + slot, false)) >
                            -1)
                        {
                            Console.WriteLine(
                                "Tried to add the same piece of equipment twice, this is not permitted.  (Path: " +
                                "//Config/Equipment/Slot" + slot + ")");
                            return false;
                        }
                        else
                        {
                            Intersect_Convert_Lib.Options.EquipmentSlots.Add(GetXmlStr(options, "//Config/Equipment/Slot" + slot, false));
                        }
                        slot++;
                    }
                    Intersect_Convert_Lib.Options.WeaponIndex = GetXmlInt(options, "//Config/Equipment/WeaponSlot");
                    if (Intersect_Convert_Lib.Options.WeaponIndex < -1 || Intersect_Convert_Lib.Options.WeaponIndex > Intersect_Convert_Lib.Options.EquipmentSlots.Count - 1)
                    {
                        Console.WriteLine(
                            "Weapon Slot is out of bounds! Make sure the slot exists and you are counting starting from zero! Use -1 if you do not wish to have equipable weapons in-game!  (Path: " +
                            "//Config/Equipment/WeaponSlot)");
                    }
                    Intersect_Convert_Lib.Options.ShieldIndex = GetXmlInt(options, "//Config/Equipment/ShieldSlot");
                    if (Intersect_Convert_Lib.Options.ShieldIndex < -1 || Intersect_Convert_Lib.Options.ShieldIndex > Intersect_Convert_Lib.Options.EquipmentSlots.Count - 1)
                    {
                        Console.WriteLine(
                            "Shield Slot is out of bounds! Make sure the slot exists and you are counting starting from zero! Use -1 if you do not wish to have equipable shields in-game!  (Path: " +
                            "//Config/Equipment/ShieldSlot)");
                    }

                    //Paperdoll
                    slot = 0;
                    while (!string.IsNullOrEmpty(GetXmlStr(options, "//Config/Paperdoll/Slot" + slot, false)))
                    {
                        if (
                            Intersect_Convert_Lib.Options.EquipmentSlots.IndexOf(GetXmlStr(options, "//Config/Paperdoll/Slot" + slot, false)) >
                            -1)
                        {
                            if (
                                Intersect_Convert_Lib.Options.PaperdollOrder.IndexOf(GetXmlStr(options, "//Config/Paperdoll/Slot" + slot,
                                    false)) > -1)
                            {
                                Console.WriteLine(
                                    "Tried to add the same piece of equipment to the paperdoll render order twice, this is not permitted.  (Path: " +
                                    "//Config/Paperdoll/Slot" + slot + ")");
                                return false;
                            }
                            else
                            {
                                Intersect_Convert_Lib.Options.PaperdollOrder.Add(GetXmlStr(options, "//Config/Paperdoll/Slot" + slot, false));
                            }
                        }
                        else
                        {
                            Console.WriteLine(
                                "Tried to add a paperdoll for a piece of equipment that does not exist!  (Path: " +
                                "//Config/Paperdoll/Slot" + slot + ")");
                            return false;
                        }
                        slot++;
                    }

                    //Tool Types
                    slot = 0;
                    while (!string.IsNullOrEmpty(GetXmlStr(options, "//Config/ToolTypes/Slot" + slot, false)))
                    {
                        if (Intersect_Convert_Lib.Options.ToolTypes.IndexOf(GetXmlStr(options, "//Config/ToolTypes/Slot" + slot, false)) > -1)
                        {
                            Console.WriteLine(
                                "Tried to add the same type of tool twice, this is not permitted.  (Path: " +
                                "//Config/ToolTypes/Slot" + slot + ")");
                            return false;
                        }
                        else
                        {
                            Intersect_Convert_Lib.Options.ToolTypes.Add(GetXmlStr(options, "//Config/ToolTypes/Slot" + slot, false));
                        }
                        slot++;
                    }

                    //Misc
                    ItemDespawnTime = GetXmlInt(options, "//Config/Misc/ItemDespawnTime");
                    ItemRespawnTime = GetXmlInt(options, "//Config/Misc/ItemSpawnTime");

                    //Combat
                    RegenTime = GetXmlInt(options, "//Config/Combat/RegenTime");
                    Intersect_Convert_Lib.Options.MinAttackRate = GetXmlInt(options, "//Config/Combat/MinAttackRate");
                    Intersect_Convert_Lib.Options.MaxAttackRate = GetXmlInt(options, "//Config/Combat/MaxAttackRate");
                    Intersect_Convert_Lib.Options.BlockingSlow = GetXmlInt(options, "//Config/Combat/BlockingSlow") / 100;
                    Intersect_Convert_Lib.Options.CritChance = GetXmlInt(options, "//Config/Combat/CritChance");
                    Intersect_Convert_Lib.Options.BlockingSlow = GetXmlInt(options, "//Config/Combat/CritMultiplier") / 100;
                    Intersect_Convert_Lib.Options.MaxDashSpeed = GetXmlInt(options, "//Config/Combat/MaxDashSpeed");

                    //Map
                    Intersect_Convert_Lib.Options.GameBorderStyle = GetXmlInt(options, "//Config/Map/BorderStyle");
                    var zdimension = GetXmlStr(options, "//Config/Map/ZDimensionVisible", false);
                    if (zdimension != "")
                    {
                        Intersect_Convert_Lib.Options.ZDimensionVisible = Convert.ToBoolean(zdimension);
                    }
                    Intersect_Convert_Lib.Options.MapWidth = GetXmlInt(options, "//Config/Map/MapWidth");
                    Intersect_Convert_Lib.Options.MapHeight = GetXmlInt(options, "//Config/Map/MapHeight");
                    if (Intersect_Convert_Lib.Options.MapWidth < 10 || Intersect_Convert_Lib.Options.MapWidth > 64 || Intersect_Convert_Lib.Options.MapHeight < 10 ||
                        Intersect_Convert_Lib.Options.MapHeight > 64)
                    {
                        Console.WriteLine(
                            "MapWidth and/or MapHeight are out of bounds. Must be between 10 and 64. The client loads 9 maps at a time, having large map sizes really hurts performance.");
                        ConfigFailed = true;
                    }
                    Intersect_Convert_Lib.Options.TileWidth = GetXmlInt(options, "//Config/Map/TileWidth");
                    Intersect_Convert_Lib.Options.TileHeight = GetXmlInt(options, "//Config/Map/TileHeight");

                    if (ConfigFailed)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to load config.xml. Exception info: " + ex);
                    return false;
                }
            }
            return !ConfigFailed;
        }

        public static byte[] GetServerConfig()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteString(Intersect_Convert_Lib.Options.GameName);

            //Game Objects
            bf.WriteInteger(Intersect_Convert_Lib.Options.MaxNpcDrops);

            //Player Objects
            bf.WriteInteger(Intersect_Convert_Lib.Options.MaxStatValue);
            bf.WriteInteger(Intersect_Convert_Lib.Options.MaxLevel);
            bf.WriteInteger(Intersect_Convert_Lib.Options.MaxHotbar);
            bf.WriteInteger(Intersect_Convert_Lib.Options.MaxInvItems);
            bf.WriteInteger(Intersect_Convert_Lib.Options.MaxPlayerSkills);
            bf.WriteInteger(Intersect_Convert_Lib.Options.MaxBankSlots);

            //Passability
            for (int i = 0; i < Enum.GetNames(typeof(Intersect_Convert_Lib.MapZones)).Length; i++)
            {
                bf.WriteBoolean(Intersect_Convert_Lib.Options.PlayerPassable[i]);
            }

            //Equipment
            bf.WriteInteger(Intersect_Convert_Lib.Options.EquipmentSlots.Count);
            for (int i = 0; i < Intersect_Convert_Lib.Options.EquipmentSlots.Count; i++)
            {
                bf.WriteString(Intersect_Convert_Lib.Options.EquipmentSlots[i]);
            }
            bf.WriteInteger(Intersect_Convert_Lib.Options.WeaponIndex);
            bf.WriteInteger(Intersect_Convert_Lib.Options.ShieldIndex);

            //Paperdoll
            for (int i = 0; i < Intersect_Convert_Lib.Options.NewPaperdollOrder.Length; i++)
            {
                bf.WriteInteger(Intersect_Convert_Lib.Options.NewPaperdollOrder[i].Count);
                for (int x = 0; x < Intersect_Convert_Lib.Options.NewPaperdollOrder[i].Count; x++)
                {
                    bf.WriteString(Intersect_Convert_Lib.Options.NewPaperdollOrder[i][x]);
                }
            }

            //Tool Types
            bf.WriteInteger(Intersect_Convert_Lib.Options.ToolTypes.Count);
            for (int i = 0; i < Intersect_Convert_Lib.Options.ToolTypes.Count; i++)
            {
                bf.WriteString(Intersect_Convert_Lib.Options.ToolTypes[i]);
            }

            //Combat
            bf.WriteInteger(Intersect_Convert_Lib.Options.MinAttackRate);
            bf.WriteInteger(Intersect_Convert_Lib.Options.MaxAttackRate);
            bf.WriteDouble(Intersect_Convert_Lib.Options.BlockingSlow);
            bf.WriteInteger(Intersect_Convert_Lib.Options.CritChance);
            bf.WriteDouble(Intersect_Convert_Lib.Options.CritMultiplier);
            bf.WriteInteger(Intersect_Convert_Lib.Options.MaxDashSpeed);

            //Map
            bf.WriteInteger(Intersect_Convert_Lib.Options.GameBorderStyle);
            bf.WriteBoolean(Intersect_Convert_Lib.Options.ZDimensionVisible);
            bf.WriteInteger(Intersect_Convert_Lib.Options.MapWidth);
            bf.WriteInteger(Intersect_Convert_Lib.Options.MapHeight);
            bf.WriteInteger(Intersect_Convert_Lib.Options.TileWidth);
            bf.WriteInteger(Intersect_Convert_Lib.Options.TileHeight);

            return bf.ToArray();
        }

        private static int GetXmlInt(XmlDocument xmlDoc, string xmlPath, bool required = true)
        {
            var selectSingleNode = xmlDoc.SelectSingleNode(xmlPath);
            int returnVal = 0;
            if (selectSingleNode == null)
            {
                if (required)
                {
                    Console.WriteLine("Path does not exist in config.xml  (Path: " + xmlPath + ")");
                    ConfigFailed = true;
                }
            }
            else if (!int.TryParse(selectSingleNode.InnerText, out returnVal))
            {
                if (required)
                {
                    Console.WriteLine("Failed to load value from config.xml  (Path: " + xmlPath + ")");
                    ConfigFailed = true;
                }
            }
            return returnVal;
        }

        private static string GetXmlStr(XmlDocument xmlDoc, string xmlPath, bool required = true)
        {
            var selectSingleNode = xmlDoc.SelectSingleNode(xmlPath);
            string returnVal = "";
            if (selectSingleNode == null)
            {
                if (required)
                {
                    Console.WriteLine("Path does not exist in config.xml  (Path: " + xmlPath + ")");
                    ConfigFailed = true;
                }
            }
            else
            {
                returnVal = selectSingleNode.InnerText;
            }
            return returnVal;
        }
    }
}