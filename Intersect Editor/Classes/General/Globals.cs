using System.Collections.Generic;
using System.Threading;
using Intersect.Editor.Classes.Core;
using Intersect.Editor.Classes.General;
using Intersect.Editor.Classes.Maps;
using Intersect.Editor.Forms;
using Intersect.Editor.Forms.DockingElements;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Localization;

namespace Intersect.Editor.Classes
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
                case (int) Stats.Attack:
                    return "Attack";
                case (int) Stats.AbilityPower:
                    return "Ability Power";
                case (int) Stats.Defense:
                    return "Defense";
                case (int) Stats.MagicResist:
                    return "Magic Resist";
                case (int) Stats.Speed:
                    return "Speed";
                default:
                    return "Invalid Stat";
            }
        }
    }
}