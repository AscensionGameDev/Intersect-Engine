using System.Collections.Generic;

namespace Intersect_Library
{
    public static class Options
    {
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

        public static void LoadFromServer(ByteBuffer bf)
        {
            //General
            GameName = bf.ReadString();

            //Game Objects
            MaxItems = bf.ReadInteger();
            MaxShops = bf.ReadInteger();
            MaxNpcs = bf.ReadInteger();
            MaxNpcDrops = bf.ReadInteger();
            MaxSpells = bf.ReadInteger();
            MaxAnimations = bf.ReadInteger();
            MaxResources = bf.ReadInteger();
            MaxClasses = bf.ReadInteger();
            MaxQuests = bf.ReadInteger();
            MaxProjectiles = bf.ReadInteger();
            MaxCommonEvents = bf.ReadInteger();
            MaxServerVariables = bf.ReadInteger();
            MaxServerSwitches = bf.ReadInteger();

            //Player Objects
            MaxStatValue = bf.ReadInteger();
            MaxLevel = bf.ReadInteger();
            MaxPlayerVariables = bf.ReadInteger();
            MaxPlayerSwitches = bf.ReadInteger();
            MaxHotbar = bf.ReadInteger();
            MaxInvItems = bf.ReadInteger();
            MaxPlayerSkills = bf.ReadInteger();
            MaxBankSlots = bf.ReadInteger();

            //Equipment
            int count = bf.ReadInteger();
            for (int i = 0; i < count; i++)
            {
                EquipmentSlots.Add(bf.ReadString());
            }
            WeaponIndex = bf.ReadInteger();
            ShieldIndex = bf.ReadInteger();

            //Paperdoll
            count = bf.ReadInteger();
            for (int i = 0; i < count; i++)
            {
                PaperdollOrder.Add(bf.ReadString());
            }

            //Tool Types
            count = bf.ReadInteger();
            for (int i = 0; i < count; i++)
            {
                ToolTypes.Add(bf.ReadString());
            }

            //Misc

            //Map
            bf.ReadInteger();
            MapWidth = bf.ReadInteger();
            MapHeight = bf.ReadInteger();
            TileWidth = bf.ReadInteger();
            TileHeight = bf.ReadInteger();
        }
    }
}
