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
using System.Threading;

namespace Intersect_Server.Classes
{
    class MainClass
    {

        public static void Main(string[] args)
        {
            Thread logicThread;
            Console.WriteLine(@"  _____       _                          _   ");
            Console.WriteLine(@" |_   _|     | |                        | |  ");
            Console.WriteLine(@"   | |  _ __ | |_ ___ _ __ ___  ___  ___| |_ ");
            Console.WriteLine(@"   | | | '_ \| __/ _ \ '__/ __|/ _ \/ __| __|");
            Console.WriteLine(@"  _| |_| | | | ||  __/ |  \__ \  __/ (__| |_ ");
            Console.WriteLine(@" |_____|_| |_|\__\___|_|  |___/\___|\___|\__|");
            Console.WriteLine(@"                          free 2d orpg engine");
            Console.Write("Copyright (C) 2015  JC Snider, Joe Bridges\nFor help, support, and updates visit: http://ascensiongamedev.com");
            Console.WriteLine();
            Console.WriteLine("Loading, please wait.");
            Database.CheckDirectories();
            Database.LoadOptions();
            Database.LoadNpcs();
            Database.LoadItems();
            Database.LoadSpells();
            Database.LoadAnimations();
            Database.LoadResources();
            if (Database.LoadClasses() == Constants.MaxClasses)
            {
                Console.WriteLine("Failed to load classes. Creating default class.");
                Database.CreateDefaultClass();
            }
            Database.LoadMaps();
            Database.InitMySql();
            if (File.Exists("Resources/Tilesets.dat"))
            {
                Globals.Tilesets = File.ReadAllLines("Resources/Tilesets.dat");
            }
            var networkBase = new Network();
            Globals.GameTime = Globals.Rand.Next(0, 2400);
            Console.WriteLine("Randomly set game time to " + Globals.GameTime);
            Console.WriteLine("Server Started.");
            logicThread = new Thread(() => ServerLoop.RunServerLoop(networkBase));
            logicThread.Start();
            Console.WriteLine("Type exit to shutdown the server, or help for a list of commands.");
            Console.Write("> ");
            string command = Console.ReadLine();
            while (command != "exit")
            {
                switch (command)
                {
                    case "help":
                        Console.WriteLine("To be implemented.");
                        break;
                    default:
                        Console.WriteLine("Command not recoginized. Enter help for a list of commands.");
                        break;
                }
                Console.Write("> ");
                command = Console.ReadLine();
            }
            logicThread.Abort();
            return;
        }


    }
}
