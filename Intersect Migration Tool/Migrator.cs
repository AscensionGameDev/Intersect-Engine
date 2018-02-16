using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Intersect.Migration.Localization;

namespace Intersect.Migration
{
    public class Migrator
    {
        public static void Start(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Strings.Load(Database.GetLanguageFromConfig());
            Console.WriteLine(@"  _____       _                          _   ");
            Console.WriteLine(@" |_   _|     | |                        | |  ");
            Console.WriteLine(@"   | |  _ __ | |_ ___ _ __ ___  ___  ___| |_ ");
            Console.WriteLine(@"   | | | '_ \| __/ _ \ '__/ __|/ _ \/ __| __|");
            Console.WriteLine(@"  _| |_| | | | ||  __/ |  \__ \  __/ (__| |_ ");
            Console.WriteLine(@" |_____|_| |_|\__\___|_|  |___/\___|\___|\__|");
            Console.WriteLine(Strings.Main.tagline);
            Console.WriteLine("Copyright (C) 2018  Ascension Game Dev");
            Console.WriteLine(Strings.Main.version.ToString( Assembly.GetExecutingAssembly().GetName().Version));
            Console.WriteLine(Strings.Intro.support);

            Console.WriteLine("");
            Console.WriteLine(Strings.Main.title);
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine(Strings.Intro.purpose);

            if (Directory.Exists("resources") && File.Exists("resources/intersect.db"))
            {
                Database.InitDatabase();
                if (Database.GetDatabaseVersion() < Database.DbVersion && Database.GetDatabaseVersion() >= 1)
                {
                    Console.WriteLine("");
                    Console.WriteLine(Strings.Intro.outofdate.ToString( Database.GetDatabaseVersion(),
                        Database.DbVersion));
                    Console.WriteLine(Strings.Intro.confirmupgrade.ToString( Strings.Characters.yes,
                        Strings.Characters.no));
                    var key = Console.ReadKey(true);
                    while (key.KeyChar != Strings.Characters.yes.ToString()[0] &&
                           key.KeyChar != Strings.Characters.no.ToString()[0])
                    {
                        key = Console.ReadKey(true);
                    }
                    if (key.KeyChar == Strings.Characters.yes.ToString()[0])
                    {
                        Console.WriteLine(Strings.Intro.starting);
                        Database.Upgrade();
                        Console.WriteLine("");
                        Console.WriteLine(Strings.Main.exit);
                    }
                    else
                    {
                        Console.WriteLine(Strings.Intro.cancelling);
                        Console.WriteLine("");
                        Console.WriteLine(Strings.Main.exit);
                    }
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine(Strings.Intro.uptodate.ToString( Database.GetDatabaseVersion(),
                        Database.DbVersion));
                    Console.WriteLine(Strings.Intro.tooloutofdate);
                    Console.WriteLine("");
                    Console.WriteLine(Strings.Main.exit);
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine(Strings.Intro.nodatabase);
                Console.WriteLine(Strings.Intro.confirmdirectory);
                Console.WriteLine("");
                Console.WriteLine(Strings.Main.exit);
            }
            Console.ReadKey();
        }

        //Really basic error handler for debugging purposes
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!Directory.Exists("resources")) Directory.CreateDirectory("resources");
            using (var writer = new StreamWriter("resources/migration_errors.log", true))
            {
                writer.WriteLine("Message :" + ((Exception)e.ExceptionObject).Message + "<br/>" + Environment.NewLine +
                                 "StackTrace :" + ((Exception)e.ExceptionObject).StackTrace +
                                 "" + Environment.NewLine + "Date :" + DateTime.Now);
                writer.WriteLine(Environment.NewLine +
                                 "-----------------------------------------------------------------------------" +
                                 Environment.NewLine);
            }
            Console.WriteLine(Strings.Exceptions.errorcaught);
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}