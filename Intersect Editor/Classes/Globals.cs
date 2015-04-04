using Intersect_Editor.Forms;
using SFML.Graphics;

namespace Intersect_Editor.Classes
{
    public static class Globals
    {
        public static Network GameSocket;
        public static int MyIndex;
        public static MapStruct[] GameMaps = new MapStruct[1];
        public static FrmMain MainForm;
        public static FrmLogin LoginForm;
        public static long MapCount;
        public static int ReceivedGameData;
        public static bool InEditor;
        public static string[] Tilesets;
        public static MapRef[] MapRefs;
        public static ItemStruct[] Items;

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

        public static Light BackupLight;
        public static Light EditingLight;

    }
}
