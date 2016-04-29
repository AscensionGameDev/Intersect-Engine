/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System.Collections.Generic;
using Intersect_Library;

namespace Intersect_Editor.Classes.General
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
        public static int MaxQuests ;
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
