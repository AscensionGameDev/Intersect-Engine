using System;
using System.IO;
using System.Net.Sockets;

namespace IntersectServer
{
	class MainClass
	{

		public static void Main (string[] args)
		{
            Console.WriteLine("Starting server.");
            Database.CheckDirectories();
            Database.LoadOptions();
            Console.WriteLine("Loading maps.");
			Database.LoadMaps ();
            Console.WriteLine("Loading npcs.");
            Database.LoadNpcs();
            Console.WriteLine("Loading items.");
            Database.LoadItems();
            Console.WriteLine("Opening MySQL connection.");
            Database.initMySQL();
			if (File.Exists("Resources/Tilesets.dat")) {
				GlobalVariables.tilesets = File.ReadAllLines("Resources/Tilesets.dat");
			}
            Console.WriteLine("Starting network.");
            NetworkBase networkBase = new NetworkBase();
            GlobalVariables.GameTime = GlobalVariables.rand.Next(0, 2400);
            Console.WriteLine("Randomly set game time to " + GlobalVariables.GameTime);
            Console.WriteLine("Server Started.");
            ServerLoop serverLoop = new ServerLoop(networkBase);

		}


	}
}
