using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Intersect_Client
{
    public static class Globals
    {

        public static string ServerHost = "localhost";
        public static int ServerPort = 4500;
        public static bool SoundEnabled;
        public static bool MusicEnabled;
        public static bool WaitingOnServer = false;
        public static bool JoiningGame = false;

        //Map/Chunk Array
        public static Map[] GameMaps;
        public static int[] localMaps = new int[9];
        public static int[] mapRevision;
        public static int mapCount;
        public static float mapRenderTimer = 0f;


        //Local player information
        public static int currentMap = -1;
        public static int myIndex = -1;
        public static int myX = 0;
        public static int myY = 0;

        //Debugging stuff
        public static string debugInfo = "";

        //Crucial game variables
        public static int GameState = 0; //0 for main menu, 1 for in game
        public static bool gameLoaded;

        //Entities and stuff
        public static List<Entity> entities = new List<Entity>();
        public static List<Entity> events = new List<Entity>();

        //GUI Varaibles
        public static ArrayList ChatboxContent = new ArrayList();

        public static int animFrame = 0;

        public static bool LoggedIn = false;

        //Control Objects

        public static List<EventDialog> EventDialogs = new List<EventDialog>();
        public static int GameTime = 0;
        public static float dirLightIntensity = .5f;
        public static bool shouldUpdateLights = false;

        //Resource Information
        public static string[] tilesets;
    }
}
