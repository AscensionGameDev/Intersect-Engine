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
namespace Intersect_Client.Classes.General
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

        //Player Maxes
        public const int MaxStatValue = 200;
        public const int MaxStats = 5;
        public const int MaxLevel = 100;
        public const int MaxHotbar = 10;

        //Map Maxes
        public const int LayerCount = 5;

        //Item Rendering
        public static int ItemXPadding = 4;
        public static int ItemYPadding = 4;

        //Player Constraints
        public const int MaxInvItems = 35;
        public const int MaxPlayerSkills = 35;

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
