using System;
using System.Collections.Generic;
using System.Threading;

namespace Intersect_Server.Classes
{
	public static class Globals
	{

        //Options
        public static int ServerPort = 4501;
        public static string MySqlHost = "localhost";
        public static int MySqlPort = 3306;
        public static string MySqlUser = "root";
        public static string MySqlPass = "pass";
        public static string MySqldb = "IntersectDB";

        //Game Maps
		public static MapStruct[] GameMaps;
		public static int MapCount;

        //Game Items
        public static ItemStruct[] GameItems;

        //Game Npcs
        public static NpcStruct[] GameNpcs;
        public static int NpcCount;

		public static List<Client> Clients = new List<Client>();
		public static List<Thread> ClientThread = new List<Thread>();
		public static List<Entity> Entities = new List<Entity>();
		public static string[] Tilesets;

        //Game helping stuff
        public static Random Rand = new Random();

        public static int GameTime = 0;

        public static int FindOpenEntity()
        {
            for (var i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] == null)
                {
                    return i;
                }
                else if (i == Entities.Count - 1)
                {
                    Entities.Add(null);
                    return Entities.Count - 1;
                }
            }
            Entities.Add(null);
            return Entities.Count - 1;
        }
	}
}

