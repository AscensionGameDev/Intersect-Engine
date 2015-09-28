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
namespace Intersect_Editor.Classes
{
    public static class Constants
    {
        public const int MaxStats = 5;
        public const int MaxItems = 255;
        public const int MaxNpcs = 255;
        public const int MaxNpcDrops = 10;
        public const int MaxSpells = 255;
        public const int MaxAnimations = 255;
        public const int MaxResources = 255;
        public const int MaxClasses = 20;
        public const int MaxQuests = 255;

        public const int LayerCount = 5;

        //Player maxes
        public const int MaxLevel = 100;

        // Autotiles
        public const byte AutoInner = 1;
        public const byte AutoOuter = 2;
        public const byte AutoHorizontal = 3;
        public const byte AutoVertical = 4;
        public const byte AutoFill = 5;

        // Autotile types
        public const byte AutotileNone = 0;
        public const byte AutotileNormal = 1;
        public const byte AutotileFake = 2;
        public const byte AutotileAnim = 3;
        public const byte AutotileCliff = 4;

        public const byte AutotileWaterfall = 5;
        // Rendering
        public const byte RenderStateNone = 0;
        public const byte RenderStateNormal = 1;
        public const byte RenderStateAutotile = 2;

        public const int VariableCount = 100;
        public const int SwitchCount = 100;
    }
}
