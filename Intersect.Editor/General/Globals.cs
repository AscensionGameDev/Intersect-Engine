using Intersect.Editor.Forms;
using Intersect.Editor.Forms.DockingElements;
using Intersect.Editor.Forms.Editors;
using Intersect.Editor.Localization;
using Intersect.Editor.Maps;
using Intersect.Enums;
using Intersect.GameObjects;

namespace Intersect.Editor.General;


public static partial class Globals
{

    public static int AutotileFrame = 0;

    //Animation Frame Variables
    public static int Autotilemode = 0;

    public static LightBase BackupLight;

    public static bool ClosingEditor;

    public static int CopyMapSelH;

    public static int CopyMapSelW;

    public static int CopyMapSelX;

    public static int CopyMapSelY;

    //Cut/Copy Variables
    public static MapInstance CopySource;

    public static int CurMapSelH;

    public static int CurMapSelW;

    public static int CurMapSelX;

    public static int CurMapSelY;

    public static int CurrentEditor = -1;

    public static string CurrentLayer = string.Empty;

    //Editor Variables
    public static MapInstance CurrentMap = null;

    public static TilesetBase CurrentTileset = null;

    public static EditingTool CurrentTool = EditingTool.Brush;

    public static int CurSelH;

    public static int CurSelW;

    public static int CurSelX;

    public static int CurSelY;

    public static int CurTileX;

    public static int CurTileY;

    public static bool Dragging = false;

    public static LightBase EditingLight;

    //Editor Loop Variables
    public static Thread EditorThread;

    public static int FetchCount;

    //Preview Fetching Variables
    public static bool FetchingMapPreviews = false;

    public static bool GridView;

    public static bool HasCopy;

    public static bool HasGameData = false;

    public static bool InEditor;

    public static bool IsPaste;

    public static Guid LoadingMap = Guid.Empty;

    public static FrmUpdate UpdateForm;

    public static FrmLogin LoginForm;

    public static FrmMain MainForm;

    public static FrmMapEditor MapEditorWindow;

    public static MapGrid MapGrid;

    public static FrmMapGrid MapGridWindowNew;

    //Docking Window References
    public static FrmMapLayers MapLayersWindow;

    public static FrmMapList MapListWindow;

    public static FrmMapProperties MapPropertiesWindow;

    public static List<Guid> MapsToFetch;

    public static List<Guid> MapsToScreenshot = new List<Guid>();

    public static int MouseButton = -1;

    public static int MouseX;

    public static int MouseY;

    public static int MyIndex;

    public static FrmProgress PackingProgressForm;

    public static FrmProgress PreviewProgressForm;

    public static FrmProgress UpdateCreationProgressForm;

    //Network Variables
    public static int ReconnectTime = 3000;

    public static long NextServerStatusPing { get; set; }

    //Game Object Editors
    public static FrmResource ResourceEditor;

    public static EditingTool SavedTool = EditingTool.Brush;

    public static bool SavingOnClose;

    public static MapInstance SelectionSource;

    //Selection Moving Copying and Pasting
    public static int SelectionType = (int) SelectionTypes.AllLayers;

    public static int TileDragX = 0;

    public static int TileDragY = 0;

    public static int TotalTileDragX = 0;

    public static int TotalTileDragY = 0;

    public static bool ViewingMapProperties = false;

    public static int WaterfallFrame = 0;

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
        return Strings.Colors.presets[(int) color];
    }

    public static string GetStatName(int statnum)
    {
        switch (statnum)
        {
            case (int) Stat.Attack:
                return "Attack";
            case (int) Stat.AbilityPower:
                return "Ability Power";
            case (int) Stat.Defense:
                return "Defense";
            case (int) Stat.MagicResist:
                return "Magic Resist";
            case (int) Stat.Speed:
                return "Speed";
            default:
                return "Invalid Stat";
        }
    }

}
