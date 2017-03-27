using System;
using System.IO;
using System.Reflection;
using Intersect.Localization;

namespace Intersect_Migration_Tool
{
    class MainClass
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Strings.Init(Strings.IntersectComponent.Migrator, Database.GetLanguageFromConfig());
            Console.WriteLine(@"  _____       _                          _   ");
            Console.WriteLine(@" |_   _|     | |                        | |  ");
            Console.WriteLine(@"   | |  _ __ | |_ ___ _ __ ___  ___  ___| |_ ");
            Console.WriteLine(@"   | | | '_ \| __/ _ \ '__/ __|/ _ \/ __| __|");
            Console.WriteLine(@"  _| |_| | | | ||  __/ |  \__ \  __/ (__| |_ ");
            Console.WriteLine(@" |_____|_| |_|\__\___|_|  |___/\___|\___|\__|");
            Console.WriteLine(Strings.Get("main", "tagline"));
            Console.WriteLine("Copyright (C) 2017  Ascension Game Dev");
            Console.WriteLine(Strings.Get("main", "version", Assembly.GetExecutingAssembly().GetName().Version));
            Console.WriteLine(Strings.Get("intro", "support"));

            Console.WriteLine("");
            Console.WriteLine(Strings.Get("main", "title"));
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine(Strings.Get("intro", "purpose"));

            if (Directory.Exists("resources") && File.Exists("resources/intersect.db"))
            {
                Database.InitDatabase();
                if (Database.GetDatabaseVersion() < Database.DbVersion && Database.GetDatabaseVersion() >= 1)
                {
                    Console.WriteLine("");
                    Console.WriteLine(Strings.Get("intro", "outofdate", Database.GetDatabaseVersion(),
                        Database.DbVersion));
                    Console.WriteLine(Strings.Get("intro", "confirmupgrade", Strings.Get("characters", "yes"),
                        Strings.Get("characters", "no")));
                    var key = Console.ReadKey(true);
                    while (key.KeyChar != Strings.Get("characters", "yes")[0] &&
                           key.KeyChar != Strings.Get("characters", "no")[0])
                    {
                        key = Console.ReadKey(true);
                    }
                    if (key.KeyChar == Strings.Get("characters", "yes")[0])
                    {
                        Console.WriteLine(Strings.Get("intro", "starting"));
                        Database.Upgrade();
                        Console.WriteLine("");
                        Console.WriteLine(Strings.Get("main", "exit"));
                    }
                    else
                    {
                        Console.WriteLine(Strings.Get("intro", "cancelling"));
                        Console.WriteLine("");
                        Console.WriteLine(Strings.Get("main", "exit"));
                    }
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine(Strings.Get("intro", "uptodate", Database.GetDatabaseVersion(), Database.DbVersion));
                    Console.WriteLine(Strings.Get("intro", "tooloutofdate"));
                    Console.WriteLine("");
                    Console.WriteLine(Strings.Get("main", "exit"));
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine(Strings.Get("intro", "nodatabase"));
                Console.WriteLine(Strings.Get("intro", "confirmdirectory"));
                Console.WriteLine("");
                Console.WriteLine(Strings.Get("main", "exit"));
            }
            Console.ReadKey();
        }

        //Really basic error handler for debugging purposes
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!Directory.Exists("resources")) Directory.CreateDirectory("resources");
            using (StreamWriter writer = new StreamWriter("resources/migration_errors.log", true))
            {
                writer.WriteLine("Message :" + ((Exception) e.ExceptionObject).Message + "<br/>" + Environment.NewLine +
                                 "StackTrace :" + ((Exception) e.ExceptionObject).StackTrace +
                                 "" + Environment.NewLine + "Date :" + DateTime.Now);
                writer.WriteLine(Environment.NewLine +
                                 "-----------------------------------------------------------------------------" +
                                 Environment.NewLine);
            }
            Console.WriteLine(Strings.Get("exceptions", "errorcaught"));
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}