using System;
using System.Collections.Generic;
using System.Diagnostics;

using Intersect.Client.Entities;
using Intersect.Client.Entities.Events;
using Intersect.Client.Framework.Database;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Input;
using Intersect.Client.Framework.Sys;
using Intersect.Client.Items;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Network.Packets;

using JetBrains.Annotations;

namespace Intersect.Client.General
{
    public static class Globals
    {
        //Engine Progression
        public static int IntroIndex = 0;

        public static long IntroStartTime = -1;
        public static long IntroDelay = 2000;
        public static bool IntroComing = true;

        //Game Lock
        public static object GameLock = new object();

        public static bool IsRunning = false;

        //Game Systems
        public static GameContentManager ContentManager;

        [NotNull] public static GameInput InputManager;
        [NotNull] public static GameSystem System;
        [NotNull] public static GameDatabase Database;

        //Scene management
        public static bool WaitingOnServer = false;

        public static bool JoiningGame = false;
        public static bool NeedsMaps = true;
        public static bool HasGameData = false;

        //Map/Chunk Array
        public static Guid[,] MapGrid;

        public static List<Guid> GridMaps = new List<Guid>();
        public static long MapGridWidth;
        public static long MapGridHeight;

        //Local player information
        public static Player Me;
        public static int CurrentMap = -1;
        public static int MyX = 0;
        public static int MyY = 0;

        //Crucial game variables
        public static GameStates GameState = GameStates.Intro; //0 for Intro, 1 to Menu, 2 for in game
        public static bool ConnectionLost;

        //Entities and stuff
        //public static List<Entity> Entities = new List<Entity>();
        public static Dictionary<Guid, Entity> Entities = new Dictionary<Guid, Entity>();

        public static List<Guid> EntitiesToDispose = new List<Guid>();

        //Bank
        public static ItemInstance[] Bank;

        public static bool InBank = false;

        //Bag
        public static ItemInstance[] Bag = null;

        public static bool InBag = false;

        //Crafting station
        public static bool InCraft = false;

        //Trading (Only 2 people can trade at once)
        public static ItemInstance[,] Trade;

        public static bool InTrade = false;

        //Game Shop
        //Only need 1 shop, and that is the one we see at a given moment in time.
        public static ShopBase GameShop;

        //Only need 1 table, and that is the one we see at a given moment in time.
        public static CraftingTableBase ActiveCraftingTable;

        public static int AnimFrame = 0;

        public static bool LoggedIn = false;

        public static Random Random = new Random();

        //Control Objects
        public static List<EventDialog> EventDialogs = new List<EventDialog>();

        //Event Guid and the Map its associated with
        public static Dictionary<Guid,Dictionary<Guid,ByteBuffer>> PendingEvents = new Dictionary<Guid, Dictionary<Guid, ByteBuffer>>();
        public static Dictionary<Guid,Guid> EventHolds = new Dictionary<Guid,Guid>();
        public static List<Guid> QuestOffers = new List<Guid>();
        public static bool MoveRouteActive = false;

        public static Entity GetEntity(Guid id, EntityTypes type)
        {
            if (Entities.ContainsKey(id))
            {
                if (Entities[id].GetEntityType() == type)
                {
                    EntitiesToDispose.Remove(Entities[id].Id);
                    return Entities[id];
                }
                Entities[id].Dispose();
                Entities.Remove(id);
            }
            return null;
        }

        public static Stopwatch sw = new Stopwatch();
        public static Stopwatch packetSw = new Stopwatch();
        public static Stopwatch tSetSw = new Stopwatch();

        public static bool gotFirstAnim = false;
        public static int packetCount = 0;
        internal static void PlayPressed()
        {
            sw.Reset();
            sw.Start();
            packetSw.Reset();
            packetSw.Start();
        }

        internal static void GotFirstAnimation()
        {
            if (gotFirstAnim) return;
            gotFirstAnim = true;
            Debug.WriteLine("Got first animation at " +sw.ElapsedMilliseconds + " ms");
        }

        internal static void GotGameData()
        {
            if (sw.IsRunning)
            {
                Debug.WriteLine("Took " + sw.ElapsedMilliseconds + "ms to get game data! (" + packetSw.ElapsedMilliseconds + "ms spent processing " + packetCount + " packets.)");
            }
        }

        internal static void HandlePacket(CerasPacket packet)
        {
            if (sw.IsRunning)
            {
                Debug.WriteLine(sw.ElapsedMilliseconds + "  : " + packet.GetType().Name);
            }
        }

        internal static void InGame()
        {
            if (sw.IsRunning)
            {
                sw.Stop();
                Debug.WriteLine("Took " + sw.ElapsedMilliseconds + "ms to get in-game! (" + packetSw.ElapsedMilliseconds + "ms spent processing " + packetCount + " packets., " + tSetSw.ElapsedMilliseconds + "ms spent loading tilesets)");
            }
        }
    }


}