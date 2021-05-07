using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using CommandLine;

using Intersect.Factories;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Plugins;
using Intersect.Plugins.Contexts;
using Intersect.Plugins.Helpers;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Metrics;
using Intersect.Server.Networking;
using Intersect.Server.Networking.Helpers;
using Intersect.Threading;
using Intersect.Utilities;

namespace Intersect.Server.Core
{
    internal static class Bootstrapper
    {

        static Bootstrapper()
        {
            Console.CancelKeyPress += OnConsoleCancelKeyPress;
        }

        public static IServerContext Context { get; private set; }

        public static LockingActionQueue MainThread { get; private set; }

        public static void Start(params string[] args)
        {
            (string[] Args, Parser Parser, ServerCommandLineOptions CommandLineOptions) parsedArguments = ParseCommandLineArgs(args);
            if (!string.IsNullOrWhiteSpace(parsedArguments.CommandLineOptions.WorkingDirectory))
            {
                var workingDirectory = parsedArguments.CommandLineOptions.WorkingDirectory.Trim();
                if (Directory.Exists(workingDirectory))
                {
                    Directory.SetCurrentDirectory(workingDirectory);
                }
            }

            if (!PreContextSetup(args))
            {
                return;
            }

            var logger = Log.Default;
            var packetTypeRegistry = new PacketTypeRegistry(logger);
            if (!packetTypeRegistry.TryRegisterBuiltIn())
            {
                logger.Error("Failed to load built-in packet types.");
                return;
            }

            var packetHandlerRegistry = new PacketHandlerRegistry(packetTypeRegistry, logger);
            var networkHelper = new NetworkHelper(packetTypeRegistry, packetHandlerRegistry);

            FactoryRegistry<IPluginBootstrapContext>.RegisterFactory(
                PluginBootstrapContext.CreateFactory(
                    parsedArguments.Args ?? Array.Empty<string>(),
                    parsedArguments.Parser,
                    networkHelper
                )
            );

            Context = new ServerContext(parsedArguments.CommandLineOptions, logger, networkHelper);
            var noHaltOnError = Context?.StartupOptions.DoNotHaltOnError ?? false;

            if (!PostContextSetup())
            {
                return;
            }

            MainThread = Context.StartWithActionQueue();
            Action action;
            while (null != (action = MainThread.NextAction))
            {
                action.Invoke();
            }

            Log.Diagnostic("Bootstrapper exited.");

            // At this point dbs should be saved and all threads should be killed. Give a message saying that the server has shutdown and to press any key to exit.
            // Having the message and the console.readline() allows the server to exit properly if the console has crashed, and it allows us to know that the server context has shutdown.
            if (Context.HasErrors)
            {
                if (noHaltOnError)
                {
                    Console.WriteLine(Strings.Errors.errorservercrashnohalt);
                }
                else
                {
                    Console.WriteLine(Strings.Errors.errorservercrash);
                    Console.ReadLine();
                }
            }
        }

        private static ValueTuple<string[], Parser, ServerCommandLineOptions> ParseCommandLineArgs(params string[] args)
        {
            var parser = new Parser(
                parserSettings =>
                    {
                        if (parserSettings == null)
                        {
                            throw new ArgumentNullException(
                                nameof(parserSettings), @"If this is null the CommandLineParser dependency is likely broken."
                            );
                        }

                        parserSettings.AutoHelp = true;
                        parserSettings.AutoVersion = true;
                        parserSettings.IgnoreUnknownArguments = true;
                        parserSettings.MaximumDisplayWidth = Console.BufferWidth;
                    }
                );

            var options = parser.ParseArguments<ServerCommandLineOptions>(args)
                .MapResult(commandLineOptions => commandLineOptions, errors => default);

            return (args, parser, options);
        }

        #region Networking

        internal static void CheckNetwork()
        {
            //Check to see if AGD can see this server. If so let the owner know :)
            if (Options.OpenPortChecker && !Context.StartupOptions.NoNetworkCheck)
            {
                var serverAccessible = PortChecker.CanYouSeeMe(Options.ServerPort, out var externalIp);

                Console.WriteLine(Strings.Portchecking.connectioninfo);
                if (!string.IsNullOrEmpty(externalIp))
                {
                    Console.WriteLine(Strings.Portchecking.publicip, externalIp);
                    Console.WriteLine(Strings.Portchecking.publicport, Options.ServerPort);

                    Console.WriteLine();
                    if (serverAccessible)
                    {
                        Console.WriteLine(Strings.Portchecking.accessible);
                        Console.WriteLine(Strings.Portchecking.letothersjoin);
                    }
                    else
                    {
                        Console.WriteLine(Strings.Portchecking.notaccessible);
                        Console.WriteLine(Strings.Portchecking.debuggingsteps);
                        Console.WriteLine(Strings.Portchecking.checkfirewalls);
                        Console.WriteLine(Strings.Portchecking.checkantivirus);
                        Console.WriteLine(Strings.Portchecking.screwed);
                        Console.WriteLine();
                        if (!UpnP.ForwardingSucceeded())
                        {
                            Console.WriteLine(Strings.Portchecking.checkrouterupnp);
                        }
                    }
                }
                else
                {
                    Console.WriteLine(Strings.Portchecking.notconnected);
                }

                Console.WriteLine();
            }
        }

        #endregion

        #region System Console

        private static void OnConsoleCancelKeyPress(
            object sender,
            ConsoleCancelEventArgs cancelEvent
        )
        {
            ServerContext.Instance.RequestShutdown(true);

            //Shutdown();
            cancelEvent.Cancel = true;
        }

        #endregion

        #region Pre-Context

        private static bool PreContextSetup(params string[] args)
        {
            if (RunningOnWindows())
            {
                SetConsoleCtrlHandler(ConsoleCtrlHandler, true);
            }

            if (!Strings.Load())
            {
                Console.WriteLine(Strings.Errors.ErrorLoadingStrings);
                Console.ReadKey();

                return false;
            }

            if (!Options.LoadFromDisk())
            {
                Console.WriteLine(Strings.Errors.errorloadingconfig);
                Console.ReadKey();

                return false;
            }

            if (!Directory.Exists(Path.Combine("resources", "notifications")))
            {
                Directory.CreateDirectory(Path.Combine("resources", "notifications"));
            }

            if (!File.Exists(Path.Combine("resources", "notifications", "PasswordReset.html")))
            {
                ReflectionUtils.ExtractResource(
                    "Intersect.Server.Resources.notifications.PasswordReset.html",
                    Path.Combine("resources", "notifications", "PasswordReset.html")
                );
            }

            DbInterface.InitializeDbLoggers();
            DbInterface.CheckDirectories();

            PrintIntroduction();

            ExportDependencies(args);

            Formulas.LoadFormulas();

            CustomColors.Load();

            if (Options.Instance.Metrics.Enable)
            {
                MetricsRoot.Init();
            }

            return true;
        }

        private static bool PostContextSetup()
        {
            if (Context == null)
            {
                throw new ArgumentNullException(nameof(Context));
            }

            //Configure System Threadpool
            ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);
            ThreadPool.GetMinThreads(out int minWorkerThreads, out int minCompletionPortThreads);
            if (Options.Instance.Processing.MaxSystemThreadpoolWorkerThreads >= Environment.ProcessorCount)
            {
                maxWorkerThreads = Options.Instance.Processing.MaxSystemThreadpoolWorkerThreads;
            }
            if (Options.Instance.Processing.MaxSystemThreadpoolIOThreads >= Environment.ProcessorCount)
            {
                maxCompletionPortThreads = Options.Instance.Processing.MaxSystemThreadpoolIOThreads;
            }
            ThreadPool.SetMaxThreads(maxWorkerThreads, maxCompletionPortThreads);
            if (Options.Instance.Processing.MinSystemThreadpoolWorkerThreads >= Environment.ProcessorCount)
            {
                minWorkerThreads = Options.Instance.Processing.MinSystemThreadpoolWorkerThreads;
            }
            if (Options.Instance.Processing.MinSystemThreadpoolIOThreads >= Environment.ProcessorCount)
            {
                minCompletionPortThreads = Options.Instance.Processing.MinSystemThreadpoolIOThreads;
            }
            ThreadPool.SetMinThreads(minWorkerThreads, minCompletionPortThreads);

            if (!DbInterface.InitDatabase(Context))
            {
                Console.ReadKey();

                return false;
            }

            Console.WriteLine();

            Console.WriteLine(Strings.Commandoutput.playercount.ToString(Player.Count()));
            Console.WriteLine(Strings.Commandoutput.gametime.ToString(Time.GetTime().ToString("F")));

            Time.Update();

            PacketSender.CacheGameDataPacket();

            if (Options.Instance.Guild.DeleteStaleGuildsAfterDays > 0)
            {
                Guild.WipeStaleGuilds();
            }

            return true;
        }

        private static void PrintIntroduction()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(@"  _____       _                          _   ");
            Console.WriteLine(@" |_   _|     | |                        | |  ");
            Console.WriteLine(@"   | |  _ __ | |_ ___ _ __ ___  ___  ___| |_ ");
            Console.WriteLine(@"   | | | '_ \| __/ _ \ '__/ __|/ _ \/ __| __|");
            Console.WriteLine(@"  _| |_| | | | ||  __/ |  \__ \  __/ (__| |_ ");
            Console.WriteLine(@" |_____|_| |_|\__\___|_|  |___/\___|\___|\__|");
            Console.WriteLine(Strings.Intro.tagline);
            Console.WriteLine(@"Copyright (C) 2020 Ascension Game Dev");
            Console.WriteLine(Strings.Intro.version.ToString(Assembly.GetExecutingAssembly().GetName().Version));
            Console.WriteLine(Strings.Intro.support);
            Console.WriteLine(Strings.Intro.loading);
        }

        #endregion

        #region Dependencies

        private static void ClearDlls()
        {
            DeleteIfExists("libe_sqlite3.so");
            DeleteIfExists("e_sqlite3.dll");
            DeleteIfExists("libe_sqlite3.dylib");
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

        internal static bool DeleteIfExists(string filename)
        {
            try
            {
                Debug.Assert(filename != null, "filename != null");
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void ExportDependencies(params string[] args)
        {
            ClearDlls();

            var platformId = Environment.OSVersion.Platform;
            if (platformId == PlatformID.Unix)
            {
                var unixName = ReadProcessOutput("uname") ?? "";
                if (unixName.Contains("Darwin"))
                {
                    platformId = PlatformID.MacOSX;
                }
            }

            string sqliteResourceName = null;
            string sqliteFileName = null;
            switch (platformId)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    sqliteResourceName = Environment.Is64BitProcess ? "e_sqlite3x64.dll" : "e_sqlite3x86.dll";
                    sqliteFileName = "e_sqlite3.dll";

                    break;

                case PlatformID.MacOSX:
                    sqliteResourceName = "libe_sqlite3.dylib";
                    sqliteFileName = "libe_sqlite3.dylib";

                    break;

                case PlatformID.Unix:
                    sqliteResourceName = Environment.Is64BitProcess ? "libe_sqlite3_x64.so" : "libe_sqlite3_x86.so";
                    sqliteFileName = "libe_sqlite3.so";

                    break;

                case PlatformID.Xbox:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(platformId));
            }

            if (string.IsNullOrWhiteSpace(sqliteResourceName) || string.IsNullOrWhiteSpace(sqliteFileName))
            {
                return;
            }

            sqliteResourceName = $"Intersect.Server.Resources.{sqliteResourceName}";
            if (ReflectionUtils.ExtractResource(sqliteResourceName, sqliteFileName))
            {
                return;
            }

            Log.Error($"Failed to extract {sqliteFileName} library, terminating startup.");
            Environment.Exit(-0x1000);
        }

        #endregion

        #region Unmanaged

        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes ctrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {

            CtrlCEvent = 0,

            CtrlBreakEvent,

            CtrlCloseEvent,

            CtrlLogoffEvent = 5,

            CtrlShutdownEvent

        }

        private static readonly HandlerRoutine ConsoleCtrlHandler = ConsoleCtrlCheck;

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CtrlCEvent:
                case CtrlTypes.CtrlBreakEvent:
                    // Handled Elsewhere
                    break;

                case CtrlTypes.CtrlCloseEvent:
                case CtrlTypes.CtrlLogoffEvent:
                case CtrlTypes.CtrlShutdownEvent:
                    // We can't just request a shutdown -- from testing the
                    // Dispose() task never actually runs soon enough so the
                    // operating system kills the application before it
                    // actually does any clean-up and data persistence.
                    ServerContext.Instance.Dispose();

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(ctrlType), ctrlType, null);
            }

            return true;
        }

        private static bool RunningOnWindows()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return true;

                case PlatformID.MacOSX:
                case PlatformID.Unix:
                case PlatformID.Xbox:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

    }

}
