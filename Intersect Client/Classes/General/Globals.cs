/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using IntersectClientExtras.Database;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Input;
using IntersectClientExtras.Sys;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.Game_Objects;
using Intersect_Client.Classes.Maps;

namespace Intersect_Client.Classes.General
{
    public static class Globals
    {
        //Engine Progression
        public static int IntroIndex = 0;
        public static long IntroStartTime =-1;
        public static long IntroDelay = 2000;
        public static bool IntroComing = true;


        //Game Lock
        public static object GameLock = new object();
        public static bool IsRunning = false;
        public static string GameError = "";

        //Game Systems
        public static GameContentManager ContentManager;
        public static GameInput InputManager;
        public static GameSystem System;
        public static GameDatabase Database;

        //Scene management
        public static bool WaitingOnServer = false;
        public static bool JoiningGame = false;

        //Map/Chunk Array
        public static Dictionary<int, MapStruct> GameMaps = new Dictionary<int, MapStruct>();
        public static List<int> MapsToRemove = new List<int>();
        public static int[] LocalMaps = new int[9];
        public static int[] MapRevision;
        public static int MapCount;
        public static float MapRenderTimer = 0f;
        public static List<FolderMap> OrderedMaps = new List<FolderMap>();
        public static MapList MapStructure = new MapList();


        //Local player information
        public static Player Me;
        public static int CurrentMap = -1;
        public static int MyIndex = -1;
        public static int MyX = 0;
        public static int MyY = 0;

        //Debugging stuff
        public static string DebugInfo = "";

        //Crucial game variables
        public static Enums.GameStates GameState = Enums.GameStates.Intro; //0 for Intro, 1 to Menu, 2 for in game
        public static bool GameLoaded;

        //Entities and stuff
        //public static List<Entity> Entities = new List<Entity>();
        public static Dictionary<int, Entity> Entities = new Dictionary<int, Entity>();
        public static Dictionary<int, Entity> LocalEntities = new Dictionary<int, Entity>();
        public static List<int> EntitiesToDispose = new List<int>();
        public static List<int> LocalEntitiesToDispose = new List<int>();

        //Game Items
        public static ItemStruct[] GameItems;

        //Game Spells
        public static SpellStruct[] GameSpells;

        //Game Npcs
        public static NpcStruct[] GameNpcs;

        //Game Resources
        public static ResourceStruct[] GameResources;

        //Game Animations
        public static AnimationStruct[] GameAnimations;

        //Game Classes
        public static ClassStruct[] GameClasses;

        //Game Quests
        public static QuestStruct[] GameQuests;

        //Game Projectiles
        public static ProjectileStruct[] GameProjectiles;

        //GUI Varaibles
        public static ArrayList ChatboxContent = new ArrayList();

        public static int AnimFrame = 0;

        public static bool LoggedIn = false;

        //Control Objects

        public static List<EventDialog> EventDialogs = new List<EventDialog>();
        public static float DirLightIntensity = .5f;
        public static bool ShouldUpdateLights = false;

        //Resource Information
        public static string[] Tilesets;

        public static int GameBorderStyle;
    }
}
