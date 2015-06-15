using Intersect_Client.Classes.Animations;
using Intersect_Client.Classes.Items;
using Intersect_Client.Classes.NPCs;
using Intersect_Client.Classes.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Intersect_Client.Classes
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
        public static MapStruct[] GameMaps;
        public static int[] LocalMaps = new int[9];
        public static int[] MapRevision;
        public static int MapCount;
        public static float MapRenderTimer = 0f;


        //Local player information
        public static int CurrentMap = -1;
        public static int MyIndex = -1;
        public static int MyX = 0;
        public static int MyY = 0;

        //Debugging stuff
        public static string DebugInfo = "";

        //Crucial game variables
        public static int GameState = 0; //0 for main menu, 1 for in game
        public static bool GameLoaded;

        //Entities and stuff
        public static List<Entity> Entities = new List<Entity>();
        public static List<Entity> Events = new List<Entity>();

        //Game Items
        public static ItemStruct[] GameItems;

        //Game Spells
        public static SpellStruct[] GameSpells;

        //Game Npcs
        public static NpcStruct[] GameNpcs;

        //Game Animations
        public static AnimationStruct[] GameAnimations;

        //GUI Varaibles
        public static ArrayList ChatboxContent = new ArrayList();

        public static int AnimFrame = 0;

        public static bool LoggedIn = false;

        //Control Objects

        public static List<EventDialog> EventDialogs = new List<EventDialog>();
        public static int GameTime = 0;
        public static float DirLightIntensity = .5f;
        public static bool ShouldUpdateLights = false;

        //Resource Information
        public static string[] Tilesets;
    }
}
