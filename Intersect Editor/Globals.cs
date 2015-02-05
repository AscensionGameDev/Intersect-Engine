using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace Intersect_Editor
{
    public static class Globals
    {
        public static Socket GameSocket;
        public static int myIndex;
        public static Map[] GameMaps = new Map[1];
        public static frmMain mainForm;
        public static frmLogin loginForm;
        public static long mapCount;
        public static int receivedGameData;
        public static bool inEditor;
        public static string[] tilesets;
        public static MapRef[] MapRefs;
        public static Item[] Items;

        //Animation Frame Variables
        public static int autotilemode = 0;
        public static int waterfallFrame = 0;
        public static int autotileFrame = 0;

        //Editor Variables
        public static int currentMap;
        public static int currentTileset = -1;
        public static int curSelX;
        public static int curSelY;
        public static int curTileX;
        public static int curTileY;
        public static int curSelW;
        public static int curSelH;
        public static int mouseX;
        public static int mouseY;
        public static int mouseButton = -1;
        public static int currentLayer = 0;

        //Day - Night - Light Variables
        public static bool nightEnabled = false;
        public static bool lightsChanged = true;
        public static SFML.Graphics.Color[,] nightColorArray;
        public static SFML.Graphics.Color[,] bnightColorArray;
        //Cached Texture for nighttime overlay
        public static Texture nightTex;
        public static Light backupLight;
        public static Light editingLight;

    }
}
