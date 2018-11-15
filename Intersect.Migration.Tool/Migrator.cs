using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Intersect.Migration.Localization;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Logging;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Utilities;
using System.Linq;

namespace Intersect.Migration
{
    public class Migrator
    {
        public static void Start(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Strings.Load();
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

            ExportDependencies(args);

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

        private static string ReadProcessOutput(string name)
        {
            try
            {
                Debug.Assert(name != null, "name != null");
                var p = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = name
                    }
                };
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                var output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                output = output.Trim();
                return output;
            }
            catch
            {
                return "";
            }
        }

        private static void ExportDependencies(string[] args)
        {
            string dllname = "";

            var os = Environment.OSVersion;
            var platformId = os.Platform;
            if (platformId == PlatformID.Unix)
            {
                var unixName = ReadProcessOutput("uname");
                if (unixName?.Contains("Darwin") ?? false) platformId = PlatformID.MacOSX;
            }

            switch (platformId)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    //Place sqlite3.dll where it's needed.
                    dllname = Environment.Is64BitProcess ? "e_sqlite3x64.dll" : "e_sqlite3x86.dll";
                    if (!ReflectionUtils.ExtractResource($"Intersect.Migration.Resources.{dllname}", "e_sqlite3.dll"))
                    {
                        Log.Error("Failed to extract sqlite library, terminating startup.");
                        Environment.Exit(-0x1000);
                    }

                    break;

                case PlatformID.MacOSX:
                    if (!ReflectionUtils.ExtractResource("Intersect.Migration.Resources.libe_sqlite3.dylib", "libe_sqlite3.dylib"))
                    {
                        Log.Error("Failed to extract lib_sqlite3.dylib, terminating startup.");
                        Environment.Exit(-0x1001);
                    }
                    break;

                case PlatformID.Xbox:
                    break;

                case PlatformID.Unix:
                    //Place libe_sqlite3.so where it's needed.
                    dllname = Environment.Is64BitProcess ? "libe_sqlite3_x64.so" : "libe_sqlite3_x86.so";
                    if (args.Contains("alpine")) dllname = "libe_sqlite3_alpine.so";
                    if (!ReflectionUtils.ExtractResource($"Intersect.Migration.Resources.{dllname}", "libe_sqlite3.so"))
                    {
                        Log.Error("Failed to extract libe_sqlite.so library, terminating startup.");
                        Environment.Exit(-0x1000);
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(platformId));
            }
        }

        //Really basic error handler for debugging purposes
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error($"Received unhandled exception from {sender}.");
            Log.Error(e?.ExceptionObject as Exception);
            Console.WriteLine(Strings.Exceptions.errorcaught);
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}