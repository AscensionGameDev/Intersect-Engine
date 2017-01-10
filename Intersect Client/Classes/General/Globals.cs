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

using System.Collections.Generic;
using IntersectClientExtras.Database;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Input;
using IntersectClientExtras.Sys;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.Items;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Color = IntersectClientExtras.GenericClasses.Color;

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
        public static bool NeedsMaps = true;
        public static bool HasGameData = false;

        //Map/Chunk Array
        public static int[,] MapGrid;
        public static List<int> GridMaps = new List<int>();
        public static long MapGridWidth;
        public static long MapGridHeight;
        public static int[] MapRevision;
        public static float MapRenderTimer = 0f;


        //Local player information
        public static Player Me;
        public static int CurrentMap = -1;
        public static int MyX = 0;
        public static int MyY = 0;

        //Debugging stuff
        public static string DebugInfo = "";

        //Crucial game variables
        public static GameStates GameState = GameStates.Intro; //0 for Intro, 1 to Menu, 2 for in game
        public static bool GameLoaded;

        //Entities and stuff
        //public static List<Entity> Entities = new List<Entity>();
        public static Dictionary<int, Entity> Entities = new Dictionary<int, Entity>();
        public static List<int> EntitiesToDispose = new List<int>();

        public static Entity GetEntity(int index, int type, long spawnTime)
        {
            if (Entities.ContainsKey(index))
            {
                if ((int) Entities[index].GetEntityType() == type && Entities[index].SpawnTime == spawnTime)
                {
                    EntitiesToDispose.Remove(Entities[index].MyIndex);
                    return Entities[index];
                }
                Entities[index].Dispose();
                Entities.Remove(index);
            }
            return null;
        }

        //Bank
        public static ItemInstance[] Bank = new ItemInstance[Options.MaxBankSlots];
        public static bool InBank = false;

        //Crafting station
        public static bool InCraft = false;

        //Trading (Only 2 people can trade at once)
        public static ItemInstance[,] Trade = new ItemInstance[2, Options.MaxInvItems];
        public static bool InTrade = false;

        //Game Shop
        //Only need 1 shop, and that is the one we see at a given moment in time.
        public static ShopBase GameShop;

        //Only need 1 bench, and that is the one we see at a given moment in time.
        public static BenchBase GameBench;

        public static int AnimFrame = 0;

        public static bool LoggedIn = false;

        //Control Objects
        public static List<EventDialog> EventDialogs = new List<EventDialog>();
        public static List<EventHold> EventHolds = new List<EventHold>();
        public static List<int> QuestOffers = new List<int>();
    }
}
