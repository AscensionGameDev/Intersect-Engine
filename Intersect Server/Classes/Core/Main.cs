/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
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
            Console.WriteLine("Loading npcs.");
            Database.LoadNpcs();
            Console.WriteLine("Loading items.");
            Database.LoadItems();
            Console.WriteLine("Loading spells.");
            Database.LoadSpells();
            Console.WriteLine("Loading animations.");
            Database.LoadAnimations();
            Console.WriteLine("Loading resources.");
            Database.LoadResources();
            Console.WriteLine("Loading classes.");
            if (Database.LoadClasses() == Constants.MaxClasses)
            {
                Console.WriteLine("Failed to load classes. Creating default class.");
                Database.CreateDefaultClass();
            }
            Console.WriteLine("Loading maps.");
            Database.LoadMaps();
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
