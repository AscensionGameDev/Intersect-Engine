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
using Intersect_Editor.Forms;
using SFML.Graphics;
using System.Threading;

namespace Intersect_Editor.Classes
{
    public static class Globals
    {
        //Network Variables
        public static string ServerHost = "localhost";
        public static int ServerPort = 6000;
        public static int ReconnectTime = 3000;

        //Editor Loop Variables
        public static Thread EditorThread;
        public static int CurrentEditor = -1;

        public static int MyIndex;
        public static MapStruct[] GameMaps = new MapStruct[1];
        public static FrmMain MainForm;
        public static FrmLogin LoginForm;
        public static long MapCount;
        public static int ReceivedGameData;
        public static bool InEditor;
        public static string[] Tilesets;
        public static MapRef[] MapRefs;

        public static ItemStruct[] GameItems;
        public static NpcStruct[] GameNpcs;
        public static SpellStruct[] GameSpells;
        public static AnimationStruct[] GameAnimations;
        public static ResourceStruct[] GameResources;

        //Animation Frame Variables
        public static int Autotilemode = 0;
        public static int WaterfallFrame = 0;
        public static int AutotileFrame = 0;

        //Editor Variables
        public static int CurrentMap;
        public static int CurrentTileset = -1;
        public static int CurSelX;
        public static int CurSelY;
        public static int CurTileX;
        public static int CurTileY;
        public static int CurSelW;
        public static int CurSelH;
        public static int MouseX;
        public static int MouseY;
        public static int MouseButton = -1;
        public static int CurrentLayer = 0;
        public static bool ViewingMapProperties = false;

        //Game Object Editors
        public static frmResource ResourceEditor;


        public static Light BackupLight;
        public static Light EditingLight;

        public static string IntToDir(int index)
        {
            switch (index)
            {
                case 0:
                    return "Up";
                case 1:
                    return "Down";
                case 2:
                    return "Left";
                case 3:
                    return "Right";
                default:
                    return "Unknown Direction";
            }
        }

    }
}
