/*
    Intersect Game Engine (Server)
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

using System.CodeDom;

namespace Intersect_Server.Classes
{
    public static class Constants
    {
        //Game Object Maxes
        public const int MaxItems = 255;
        public const int MaxShops = 255;
        public const int MaxNpcs = 255;
        public const int MaxNpcDrops = 10;
        public const int MaxSpells = 255;
        public const int MaxAnimations = 255;
        public const int MaxResources = 255;
        public const int MaxClasses = 20;
        public const int MaxQuests = 255;
        public const int MaxProjectiles = 255;
        public const int MaxCommonEvents = 255;
        public const int MaxServerVariables = 100;
        public const int MaxServerSwitches = 100;
        public const int MaxPlayerVariables = 100;
        public const int MaxPlayerSwitches = 100;

        //Player Maxes
        public const int MaxStatValue = 200;
        public const int MaxStats = 5;
        public const int MaxLevel = 100;
        public const int MaxHotbar = 10;

        //Shop Processing
        public const int CurrencyItem = 0; //Index of item to be used in shops.

        //Item Processing
        public const int ItemDespawnTime = 15000; //15 seconds
        public const int ItemRespawnTime = 15000; //15 seconds

        //Player Constraints
        public const int MaxInvItems = 35;
        public const int MaxPlayerSkills = 35;
        public const int MaxBankSlots = 100;

        public const int LayerCount = 5;
    }
}

