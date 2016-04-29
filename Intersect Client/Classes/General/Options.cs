/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System.Collections.Generic;
using Intersect_Library;

namespace Intersect_Client.Classes.General
{
    public static class Options
    {
        //Game Settings
        public static string GameName = "Intersect";

        //Game Object Maxes
        public static int MaxItems;
        public static int MaxShops;
        public static int MaxNpcs;
        public static int MaxNpcDrops;
        public static int MaxSpells;
        public static int MaxAnimations;
        public static int MaxResources;
        public static int MaxClasses;
        public static int MaxQuests;
        public static int MaxProjectiles;
        public static int MaxCommonEvents;
        public static int MaxServerVariables;
        public static int MaxServerSwitches;

        //Player Maxes
        public static int MaxPlayerVariables;
        public static int MaxPlayerSwitches;
        public static int MaxStatValue;
        public static int MaxStats = 5;
        public static int MaxLevel;
        public static int MaxHotbar;
        public static int MaxInvItems;
        public static int MaxPlayerSkills;
        public static int MaxBankSlots;

        //Equipment
        public static int WeaponIndex = -1;
        public static int ShieldIndex = -1;
        public static List<string> EquipmentSlots = new List<string>();
        public static List<string> PaperdollOrder = new List<string>();
        public static List<string> ToolTypes = new List<string>();

        //Maps
        public static int MapWidth;
        public static int MapHeight;
        public static int TileWidth;
        public static int TileHeight;
        public const int LayerCount = 5;

        public static void LoadServerConfig(ByteBuffer bf)
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
