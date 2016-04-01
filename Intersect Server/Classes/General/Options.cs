using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Intersect_Server.Classes.General
{
    public static class Options
    {
        //Config XML String
        public static string ConfigXml = "";
        private static bool ConfigFailed = false;

        //Game Settings
        public static string GameName = "Intersect";
        public static string MOTD = "Welcome to Intersect!";
        public static int ServerPort = 4500;
        
        //Game Object Maxes
        public static int MaxItems = 255;
        public static int MaxShops = 255;
        public static int MaxNpcs = 255;
        public static int MaxNpcDrops = 10;
        public static int MaxSpells = 255;
        public static int MaxAnimations = 255;
        public static int MaxResources = 255;
        public static int MaxClasses = 20;
        public static int MaxQuests = 255;
        public static int MaxProjectiles = 255;
        public static int MaxCommonEvents = 255;
        public static int MaxServerVariables = 100;
        public static int MaxServerSwitches = 100;

        //Player Maxes
        public static int MaxPlayerVariables = 100;
        public static int MaxPlayerSwitches = 100;
        public static int MaxStatValue = 200;
        public static int MaxStats = 5;
        public static int MaxLevel = 100;
        public static int MaxHotbar = 10;
        public static int MaxInvItems = 35;
        public static int MaxPlayerSkills = 35;
        public static int MaxBankSlots = 100;

        //Equipment
        public static int WeaponIndex = -1;
        public static int ShieldIndex = -1;
        public static List<string> EquipmentSlots = new List<string>();
        public static List<string> PaperdollOrder = new List<string>();
        public static List<string> ToolTypes = new List<string>();

        //Misc
        public static int ItemDespawnTime = 15000; //15 seconds
        public static int ItemRespawnTime = 15000; //15 seconds

        //Maps
        public static int GameBorderStyle = 0; //0 For Smart Borders, 1 for Non-Seamless, 2 for black borders
        public static int MapWidth = 32;
        public static int MapHeight = 26;
        public static int TileWidth = 32;
        public static int TileHeight = 32;
        public static int LayerCount = 5; 

        //Options File
        public static bool LoadOptions()
        {
            
            if (!File.Exists("resources\\config.xml"))
            {
                var settings = new XmlWriterSettings { Indent = true };
                var writer = XmlWriter.Create("resources\\config.xml", settings);
                writer.WriteStartDocument();
                writer.WriteComment("Config.xml generated automatically by Intersect Server.");
                writer.WriteStartElement("Config");
                writer.WriteElementString("GameName", "Intersect");
                writer.WriteElementString("MOTD", "Welcome to Intersect!");
                writer.WriteElementString("ServerPort", "4500");

                writer.WriteStartElement("GameObjects");
                writer.WriteComment("You can increase these if you want, but you shouldn't unless you need to.");
                writer.WriteElementString("MaxItems", "255");
                writer.WriteElementString("MaxShops", "255");
                writer.WriteElementString("MaxNpcs", "255");
                writer.WriteElementString("MaxNpcDrops", "10");
                writer.WriteElementString("MaxSpells", "255");
                writer.WriteElementString("MaxAnimations", "255");
                writer.WriteElementString("MaxResources", "255");
                writer.WriteElementString("MaxClasses", "20");
                writer.WriteElementString("MaxQuests", "255");
                writer.WriteElementString("MaxProjectiles", "255");
                writer.WriteElementString("MaxCommonEvents", "255");
                writer.WriteElementString("MaxServerVariables", "100");
                writer.WriteElementString("MaxServerSwitches", "100");
                writer.WriteEndElement();

                writer.WriteStartElement("Player");
                writer.WriteElementString("MaxStat", "255");
                writer.WriteElementString("MaxLevel", "100");
                writer.WriteElementString("MaxPlayerVariables", "100");
                writer.WriteElementString("MaxPlayerSwitches", "100");
                writer.WriteElementString("MaxInventory", "35");
                writer.WriteElementString("MaxSpells", "35");
                writer.WriteElementString("MaxBank", "100");
                writer.WriteEndElement();

                writer.WriteStartElement("Equipment");
                writer.WriteElementString("WeaponSlot", "2");
                writer.WriteElementString("ShieldSlot", "3");
                writer.WriteElementString("Slot0", "Helmet");
                writer.WriteElementString("Slot1", "Armor");
                writer.WriteElementString("Slot2", "Weapon");
                writer.WriteElementString("Slot3", "Shield");
                writer.WriteElementString("Slot4", "Boots");
                writer.WriteEndElement();

                writer.WriteStartElement("Paperdoll");
                writer.WriteComment("Paperdoll is rendered in the following order. If you want to change when each piece of equipment gets rendered simply swap the equipment names.");
                writer.WriteElementString("Slot0", "Helmet");
                writer.WriteElementString("Slot1", "Armor");
                writer.WriteElementString("Slot2", "Weapon");
                writer.WriteElementString("Slot3", "Shield");
                writer.WriteElementString("Slot4", "Boots");
                writer.WriteEndElement();

                writer.WriteStartElement("ToolTypes");
                writer.WriteElementString("Slot0", "Axe");
                writer.WriteElementString("Slot1", "Picaxe");
                writer.WriteElementString("Slot2", "Shovel");
                writer.WriteElementString("Slot3", "Fishing Rod");
                writer.WriteEndElement();

                writer.WriteStartElement("Misc");
                writer.WriteElementString("ItemDespawnTime", "15000");
                writer.WriteElementString("ItemSpawnTime", "15000");
                writer.WriteEndElement();

                writer.WriteStartElement("Map");
                writer.WriteComment("MapBorder Override. 0 for seamless with scrolling that stops on world edges. 1 for non-seamless, and 2 for seamless where the camera knows no boundaries. (Black borders where the world ends)");
                writer.WriteElementString("BorderStyle", "0");
                writer.WriteComment("DO NOT TOUCH! These values will resize the maps in the engine and will CORRUPT existing maps. In MOST cases this value should not be changed. It would be wise to consult us before doing so!");
                writer.WriteElementString("MapWidth", "32");
                writer.WriteElementString("MapHeight", "26");
                writer.WriteElementString("TileWidth", "32");
                writer.WriteElementString("TileHeight", "32");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
                ConfigXml = File.ReadAllText("resources\\config.xml");
                Console.WriteLine("Configuration file was missing and regenerated by the server! Reloading Options Now.");
                return LoadOptions();
            }
            else
            {
                var options = new XmlDocument();
                ConfigXml = File.ReadAllText("resources\\config.xml");
                try
                {
                    options.LoadXml(ConfigXml);

                    //General Options
                    GameName = GetXmlStr(options, "//Config/GameName",false);
                    MOTD = GetXmlStr(options, "//Config/MOTD", false);
                    ServerPort = GetXmlInt(options, "//Config/ServerPort");

                    //Game Objects
                    MaxItems = GetXmlInt(options, "//Config/GameObjects/MaxItems");
                    MaxShops = GetXmlInt(options, "//Config/GameObjects/MaxShops");
                    MaxNpcs = GetXmlInt(options, "//Config/GameObjects/MaxNpcs");
                    MaxNpcDrops = GetXmlInt(options, "//Config/GameObjects/MaxNpcDrops");
                    MaxSpells = GetXmlInt(options, "//Config/GameObjects/MaxSpells");
                    MaxAnimations = GetXmlInt(options, "//Config/GameObjects/MaxAnimations");
                    MaxResources = GetXmlInt(options, "//Config/GameObjects/MaxResources");
                    MaxClasses = GetXmlInt(options, "//Config/GameObjects/MaxClasses");
                    MaxQuests = GetXmlInt(options, "//Config/GameObjects/MaxQuests");
                    MaxProjectiles = GetXmlInt(options, "//Config/GameObjects/MaxProjectiles");
                    MaxCommonEvents = GetXmlInt(options, "//Config/GameObjects/MaxCommonEvents");
                    MaxServerVariables = GetXmlInt(options, "//Config/GameObjects/MaxServerVariables");
                    MaxServerSwitches = GetXmlInt(options, "//Config/GameObjects/MaxServerSwitches");

                    //Player Options
                    MaxStatValue = GetXmlInt(options, "//Config/Player/MaxStat");
                    MaxLevel = GetXmlInt(options, "//Config/Player/MaxLevel");
                    MaxPlayerVariables = GetXmlInt(options, "//Config/Player/MaxPlayerVariables");
                    MaxPlayerSwitches = GetXmlInt(options, "//Config/Player/MaxPlayerSwitches");
                    MaxInvItems = GetXmlInt(options, "//Config/Player/MaxInventory");
                    MaxPlayerSkills = GetXmlInt(options, "//Config/Player/MaxSpells");
                    MaxBankSlots = GetXmlInt(options, "//Config/Player/MaxBank");

                    //Equipment
                    int slot = 0;
                    while (!string.IsNullOrEmpty(GetXmlStr(options, "//Config/Equipment/Slot" + slot, false)))
                    {
                        if (EquipmentSlots.IndexOf(GetXmlStr(options, "//Config/Equipment/Slot" + slot, false)) > -1)
                        {
                            Console.WriteLine("Tried to add the same piece of equipment twice, this is not permitted.  (Path: " + "//Config/Equipment/Slot" + slot + ")");
                            return false;
                        }
                        else
                        {
                            EquipmentSlots.Add(GetXmlStr(options, "//Config/Equipment/Slot" + slot, false));
                        }
                        slot++;
                    }
                    WeaponIndex = GetXmlInt(options, "//Config/Equipment/WeaponSlot");
                    if (WeaponIndex < -1 || WeaponIndex > EquipmentSlots.Count - 1)
                    {
                        Console.WriteLine("Weapon Slot is out of bounds! Make sure the slot exists and you are counting starting from zero! Use -1 if you do not wish to have equipable weapons in-game!  (Path: " + "//Config/Equipment/WeaponSlot)");
                    }
                    ShieldIndex = GetXmlInt(options, "//Config/Equipment/ShieldSlot");
                    if (ShieldIndex < -1 || ShieldIndex > EquipmentSlots.Count - 1)
                    {
                        Console.WriteLine("Shield Slot is out of bounds! Make sure the slot exists and you are counting starting from zero! Use -1 if you do not wish to have equipable shields in-game!  (Path: " + "//Config/Equipment/ShieldSlot)");
                    }

                    //Paperdoll
                    slot = 0;
                    while (!string.IsNullOrEmpty(GetXmlStr(options, "//Config/Paperdoll/Slot" + slot, false)))
                    {
                        if (EquipmentSlots.IndexOf(GetXmlStr(options, "//Config/Paperdoll/Slot" + slot, false)) > -1)
                        {
                            if (PaperdollOrder.IndexOf(GetXmlStr(options, "//Config/Paperdoll/Slot" + slot, false)) > -1)
                            {
                                Console.WriteLine("Tried to add the same piece of equipment to the paperdoll render order twice, this is not permitted.  (Path: " + "//Config/Paperdoll/Slot" + slot + ")");
                                return false;
                            }
                            else
                            {
                                PaperdollOrder.Add(GetXmlStr(options, "//Config/Paperdoll/Slot" + slot, false));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Tried to add a paperdoll for a piece of equipment that does not exist!  (Path: " + "//Config/Paperdoll/Slot" + slot + ")");
                            return false;
                        }
                        slot++;
                    }

                    //Tool Types
                    slot = 0;
                    while (!string.IsNullOrEmpty(GetXmlStr(options, "//Config/ToolTypes/Slot" + slot, false)))
                    {
                        if (ToolTypes.IndexOf(GetXmlStr(options, "//Config/ToolTypes/Slot" + slot, false)) > -1)
                        {
                            Console.WriteLine("Tried to add the same type of tool twice, this is not permitted.  (Path: " + "//Config/ToolTypes/Slot" + slot + ")");
                            return false;
                        }
                        else
                        {
                            ToolTypes.Add(GetXmlStr(options, "//Config/ToolTypes/Slot" + slot, false));
                        }
                        slot++;
                    }

                    //Misc
                    ItemDespawnTime = GetXmlInt(options, "//Config/Misc/ItemDespawnTime");
                    ItemRespawnTime = GetXmlInt(options, "//Config/Misc/ItemSpawnTime");

                    //Map
                    GameBorderStyle = GetXmlInt(options, "//Config/Map/BorderStyle");
                    MapWidth = GetXmlInt(options, "//Config/Map/MapWidth");
                    MapHeight = GetXmlInt(options, "//Config/Map/MapHeight");
                    if (MapWidth < 10 || MapWidth > 64 || MapHeight < 10 || MapHeight > 64)
                    {
                        Console.WriteLine("MapWidth and/or MapHeight are out of bounds. Must be between 10 and 64. The client loads 9 maps at a time, having large map sizes really hurts performance.");
                        ConfigFailed = true;
                    }
                    TileWidth = GetXmlInt(options, "//Config/Map/TileWidth");
                    TileHeight = GetXmlInt(options, "//Config/Map/TileHeight");

                    if (ConfigFailed)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to load config.xml. Exception info: " + ex.ToString());
                    return false;
                }
            }
            return !ConfigFailed;
        }

        public static byte[] GetServerConfig()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteString(GameName);

            //Game Objects
            bf.WriteInteger(MaxItems);
            bf.WriteInteger(MaxShops);
            bf.WriteInteger(MaxNpcs);
            bf.WriteInteger(MaxNpcDrops);
            bf.WriteInteger(MaxSpells);
            bf.WriteInteger(MaxAnimations);
            bf.WriteInteger(MaxResources);
            bf.WriteInteger(MaxClasses);
            bf.WriteInteger(MaxQuests);
            bf.WriteInteger(MaxProjectiles);
            bf.WriteInteger(MaxCommonEvents);
            bf.WriteInteger(MaxServerVariables);
            bf.WriteInteger(MaxServerSwitches);

            //Player Objects
            bf.WriteInteger(MaxStatValue);
            bf.WriteInteger(MaxLevel);
            bf.WriteInteger(MaxPlayerVariables);
            bf.WriteInteger(MaxPlayerSwitches);
            bf.WriteInteger(MaxHotbar);
            bf.WriteInteger(MaxInvItems);
            bf.WriteInteger(MaxPlayerSkills);
            bf.WriteInteger(MaxBankSlots);

            //Equipment
            bf.WriteInteger(EquipmentSlots.Count);
            for (int i = 0; i < EquipmentSlots.Count; i++)
            {
                bf.WriteString(EquipmentSlots[i]);
            }
            bf.WriteInteger(WeaponIndex);
            bf.WriteInteger(ShieldIndex);

            //Paperdoll
            bf.WriteInteger(PaperdollOrder.Count);
            for (int i = 0; i < PaperdollOrder.Count; i++)
            {
                bf.WriteString(PaperdollOrder[i]);
            }

            //Tool Types
            bf.WriteInteger(ToolTypes.Count);
            for (int i = 0; i < ToolTypes.Count; i++)
            {
                bf.WriteString(ToolTypes[i]);
            }

            //Misc

            //Map
            bf.WriteInteger(GameBorderStyle);
            bf.WriteInteger(MapWidth);
            bf.WriteInteger(MapHeight);
            bf.WriteInteger(TileWidth);
            bf.WriteInteger(TileHeight);

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
