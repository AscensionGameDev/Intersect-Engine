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

using Intersect_Editor.Forms;
using System.Collections.Generic;
using System.Threading;
using Intersect_Editor.Classes.General;
using Intersect_Editor.Classes.Maps;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps;

namespace Intersect_Editor.Classes
{
    public static class Globals
    {
        //Network Variables
        public static string ServerHost = "localhost";
        public static int ServerPort = 4500;
        public static int ReconnectTime = 3000;

        //Editor Loop Variables
        public static Thread EditorThread;
        public static int CurrentEditor = -1;

        public static int MyIndex;
        public static Dictionary<int, MapInstance> GameMaps = new Dictionary<int, MapInstance>();
        public static frmMain MainForm;
        public static FrmLogin LoginForm;
        public static int ReceivedGameData;
        public static bool InEditor;
        public static bool ClosingEditor;
        public static string[] Tilesets;

        public static ItemStruct[] GameItems;
        public static NpcStruct[] GameNpcs;
        public static SpellStruct[] GameSpells;
        public static AnimationStruct[] GameAnimations;
        public static ResourceStruct[] GameResources;
        public static ClassStruct[] GameClasses;
        public static QuestStruct[] GameQuests;
        public static ProjectileStruct[] GameProjectiles;
        public static EventStruct[] CommonEvents;
        public static ShopStruct[] GameShops;

        //Server Switches and Variables
        public static string[] ServerSwitches;
        public static bool[] ServerSwitchValues;
        public static string[] ServerVariables;
        public static int[] ServerVariableValues;

        //Player Switches and Variables
        public static string[] PlayerSwitches;
        public static string[] PlayerVariables;

        //Animation Frame Variables
        public static int Autotilemode = 0;
        public static int WaterfallFrame = 0;
        public static int AutotileFrame = 0;

        //Editor Variables
        public static int CurrentMap = -1;
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
        public static int CurrentTool = (int) EdittingTool.Pen;
        public static int CurMapSelX;
        public static int CurMapSelY;
        public static int CurMapSelW;
        public static int CurMapSelH;

        //Selection Moving Copying and Pasting
        public static int SelectionType = (int) SelectionTypes.AllLayers;
        public static bool Dragging = false;
        public static int TileDragX = 0;
        public static int TileDragY = 0;
        public static int TotalTileDragX = 0;
        public static int TotalTileDragY = 0;
        public static MapInstance SelectionSource;

        //Cut/Copy Variables
        public static MapInstance CopySource;
        public static int CopyMapSelX;
        public static int CopyMapSelY;
        public static int CopyMapSelW;
        public static int CopyMapSelH;
        public static bool HasCopy;
        public static bool IsPaste;

        //Game Object Editors
        public static frmResource ResourceEditor;

        //Docking Window References
        public static frmMapLayers MapLayersWindow;
        public static frmMapEditor MapEditorWindow;
        public static frmMapList MapListWindow;
        public static frmMapProperties MapPropertiesWindow;
        public static frmGridView MapGridWindow;

        //Preview Fetching Variables
        public static bool FetchingMapPreviews = false;
        public static List<int> MapsToFetch;
        public static int FetchCount;
        public static frmProgress PreviewProgressForm;


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

        public static string GetColorName(Color.ChatColor color)
        {
            switch (color)
            {
                case Color.ChatColor.Black:
                    return "Black";
                case Color.ChatColor.White:
                    return "White";
                case Color.ChatColor.Blue:
                    return "Blue";
                case Color.ChatColor.Red:
                    return "Red";
                case Color.ChatColor.Green:
                    return "Green";
                case Color.ChatColor.Yellow:
                    return "Yellow";
                case Color.ChatColor.Orange:
                    return "Orange";
                case Color.ChatColor.Purple:
                    return "Purple";
                case Color.ChatColor.Gray:
                    return "Gray";
                case Color.ChatColor.Cyan:
                    return "Cyan";
                case Color.ChatColor.Pink:
                    return "Pink";
                default:
                    return "No Color";
            }
        }
    }
}
