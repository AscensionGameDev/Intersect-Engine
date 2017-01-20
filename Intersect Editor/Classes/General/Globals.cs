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
using Intersect_Editor.Classes.Core;
using Intersect_Editor.Forms.DockingElements;
using Intersect_Library.Localization;

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
        public static EditorSystem System = new EditorSystem();

        public static int MyIndex;
        public static frmMain MainForm;
        public static FrmLogin LoginForm;
        public static bool HasGameData = false;
        public static bool InEditor;
        public static bool ClosingEditor;

        //Animation Frame Variables
        public static int Autotilemode = 0;
        public static int WaterfallFrame = 0;
        public static int AutotileFrame = 0;

        //Editor Variables
        public static MapInstance CurrentMap = null;
        public static int LoadingMap = -1;
        public static TilesetBase CurrentTileset = null;
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

        public static bool GridView;

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
        public static frmMapGrid MapGridWindowNew;
        public static MapGrid MapGrid;

        //Preview Fetching Variables
        public static bool FetchingMapPreviews = false;
        public static List<int> MapsToFetch;
        public static List<int> MapsToScreenshot = new List<int>();
        public static int FetchCount;
        public static frmProgress PreviewProgressForm;


        public static LightBase BackupLight;
        public static LightBase EditingLight;

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
            return Strings.Get("colors", ((int) color).ToString());
        }

        public static string GetStatName(int statnum)
        {
            switch (statnum)
            {
                case (int)Stats.Attack:
                    return "Attack";
                case (int)Stats.AbilityPower:
                    return "Ability Power";
                case (int)Stats.Defense:
                    return "Defense";
                case (int)Stats.MagicResist:
                    return "Magic Resist";
                case (int)Stats.Speed:
                    return "Speed";
                default:
                    return "Invalid Stat";
            }
        }
    }
}
