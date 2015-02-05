using System;
using System.IO;

namespace Intersect_Server.Classes
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
            Database.InitMySql();
			if (File.Exists("Resources/Tilesets.dat")) {
				Globals.Tilesets = File.ReadAllLines("Resources/Tilesets.dat");
			}
            Console.WriteLine("Starting network.");
            var networkBase = new Network();
            Globals.GameTime = Globals.Rand.Next(0, 2400);
            Console.WriteLine("Randomly set game time to " + Globals.GameTime);
            Console.WriteLine("Server Started.");
            var serverLoop = new ServerLoop(networkBase);

		}


	}
}
