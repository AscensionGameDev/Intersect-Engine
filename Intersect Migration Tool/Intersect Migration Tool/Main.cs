using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Intersect_Migration_Tool
{
    class MainClass
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Console.WriteLine(@"  _____       _                          _   ");
            Console.WriteLine(@" |_   _|     | |                        | |  ");
            Console.WriteLine(@"   | |  _ __ | |_ ___ _ __ ___  ___  ___| |_ ");
            Console.WriteLine(@"   | | | '_ \| __/ _ \ '__/ __|/ _ \/ __| __|");
            Console.WriteLine(@"  _| |_| | | | ||  __/ |  \__ \  __/ (__| |_ ");
            Console.WriteLine(@" |_____|_| |_|\__\___|_|  |___/\___|\___|\__|");
            Console.WriteLine(@"                          free 2d orpg engine");
            Console.WriteLine("Copyright (C) 2015  JC Snider, Joe Bridges");
            Console.WriteLine("Version " + Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine("For help, support, and updates visit: http://ascensiongamedev.com");

            Console.WriteLine("");
            Console.WriteLine("Intersect Migration Tool");
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("Use this to update your database when deploying the versions of Intersect.");

            if (Directory.Exists("resources") && File.Exists("resources/intersect.db"))
            {
                Database.InitDatabase();
                if (Database.GetDatabaseVersion() < Database.DbVersion && Database.GetDatabaseVersion() >=1)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Out-of-date database found. Version: " + Database.GetDatabaseVersion() + " Latest Version: " + Database.DbVersion);
                    Console.WriteLine("Do you want to upgrade? (y/n)");
                    var key = Console.ReadKey(true);
                    while (key.Key != ConsoleKey.Y && key.Key != ConsoleKey.N)
                    {
                        key = Console.ReadKey(true);
                    }
                    if (key.Key == ConsoleKey.Y)
                    {
                        Console.WriteLine("Starting Upgrade Process");
                        Database.Upgrade();
                        Console.WriteLine("");
                        Console.WriteLine("Press any key to exit.");
                    }
                    else
                    {
                        Console.WriteLine("Canceling Upgrade");
                        Console.WriteLine("");
                        Console.WriteLine("Press any key to exit.");
                    }
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("Database does not appear to be out of date. Version: " + Database.GetDatabaseVersion() + " Latest Version: " + Database.DbVersion);
                    Console.WriteLine("Is this migration tool up to date?");
                    Console.WriteLine("");
                    Console.WriteLine("Press any key to exit.");
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Failed to find an Intersect database to upgrade.");
                Console.WriteLine("Make sure you are launching this tool from the server directory.");
                Console.WriteLine("");
                Console.WriteLine("Press any key to exit.");
            }
            Console.ReadKey();
        }



        //Really basic error handler for debugging purposes
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!Directory.Exists("resources")) Directory.CreateDirectory("resources");
            using (StreamWriter writer = new StreamWriter("resources/migration_errors.log", true))
            {
                writer.WriteLine("Message :" + ((Exception)e.ExceptionObject).Message + "<br/>" + Environment.NewLine +
                                 "StackTrace :" + ((Exception)e.ExceptionObject).StackTrace +
                                 "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine +
                                 "-----------------------------------------------------------------------------" +
                                 Environment.NewLine);
            }
            Console.WriteLine("The Intersect Migration tool has encountered an error and must close. Error information can be found in resources/migration_errors.log. Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}
