using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntersectEditor
{
    public static class GlobalVariables
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

    }
}
