using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using CommandLine;
using Intersect.Factories;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Plugins;
using Intersect.Plugins.Contexts;
using Intersect.Plugins.Helpers;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Metrics;
using Intersect.Server.Networking;
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

        internal static event Action OnPostContextSetupCompleted;

        public static IServerContext Context { get; private set; }

        public static LockingActionQueue MainThread { get; private set; }

        public static void Start(params string[] args)
        {
            (string[] Args, Parser Parser, ServerCommandLineOptions CommandLineOptions) parsedArguments =
                ParseCommandLineArgs(args);
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
                Console.Error.WriteLine("[FATAL] Pre-context setup failed.");
                return;
            }

            Console.WriteLine("Pre-context setup finished.");

            var logger = Log.Default;
            var packetTypeRegistry = new PacketTypeRegistry(logger);
            if (!packetTypeRegistry.TryRegisterBuiltIn())
            {
                logger.Error("[FATAL] Failed to load built-in packet types.");
                return;
            }

            Console.WriteLine("Built-in packets registered to the packet type registry.");

            var packetHandlerRegistry = new PacketHandlerRegistry(packetTypeRegistry, logger);
            var packetHelper = new PacketHelper(packetTypeRegistry, packetHandlerRegistry);

            FactoryRegistry<IPluginBootstrapContext>.RegisterFactory(
                PluginBootstrapContext.CreateFactory(
                    parsedArguments.Args ?? Array.Empty<string>(),
                    parsedArguments.Parser,
                    packetHelper
                )
            );

            Console.WriteLine("Creating server context...");

            Context = ServerContext.ServerContextFactory.Invoke(
                parsedArguments.CommandLineOptions,
                logger,
                packetHelper
            );
            var noHaltOnError = Context?.StartupOptions.DoNotHaltOnError ?? false;

            if (!PostContextSetup())
            {
                Console.Error.WriteLine("[FATAL] Post-context setup failed.");
                return;
            }

            OnPostContextSetupCompleted?.Invoke();

            Console.WriteLine("Finished post-context setup.");

            Console.WriteLine("Starting main thread...");
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
                            nameof(parserSettings),
                            @"If this is null the CommandLineParser dependency is likely broken."
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

        #region System Console

        private static void OnConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs cancelEvent)
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

            if (ServerContext.IsDefaultResourceDirectory)
            {
                if (!Directory.Exists(Path.Combine(ServerContext.ResourceDirectory, "notifications")))
                {
                    Directory.CreateDirectory(Path.Combine(ServerContext.ResourceDirectory, "notifications"));
                }

                if (!File.Exists(Path.Combine(ServerContext.ResourceDirectory, "notifications", "PasswordReset.html")))
                {
                    ReflectionUtils.ExtractResource(
                        "Intersect.Server.Resources.notifications.PasswordReset.html",
                        Path.Combine(ServerContext.ResourceDirectory, "notifications", "PasswordReset.html")
                    );
                }
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
            Console.WriteLine("Starting post-context setup...");

            if (Context == null)
            {
                Console.Error.WriteLine("No context?");
                throw new ArgumentNullException(nameof(Context));
            }

            Console.WriteLine("Configuring thread pool...");

            //Configure System Threadpool
            ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxCompletionPortThreads);
            ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
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
                Console.Error.WriteLine("Failed to initialize the database.");

                Console.ReadKey();

                return false;
            }

            Time.Update();

            Console.WriteLine();
            Console.WriteLine(Strings.Commandoutput.ServerInfo);
            Console.WriteLine(Strings.Commandoutput.AccountCount.ToString(Database.PlayerData.User.Count()));
            Console.WriteLine(Strings.Commandoutput.CharacterCount.ToString(Player.Count()));
            Console.WriteLine(Strings.Commandoutput.NpcCount.ToString(NpcBase.Lookup.Count));
            Console.WriteLine(Strings.Commandoutput.SpellCount.ToString(SpellBase.Lookup.Count));
            Console.WriteLine(Strings.Commandoutput.MapCount.ToString(MapBase.Lookup.Count));
            Console.WriteLine(Strings.Commandoutput.EventCount.ToString(EventBase.Lookup.Count));
            Console.WriteLine(Strings.Commandoutput.ItemCount.ToString(ItemBase.Lookup.Count));
            Console.WriteLine();
            Console.WriteLine(Strings.Commandoutput.gametime.ToString(Time.GetTime().ToString("F")));
            Console.WriteLine();

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
            Console.WriteLine();
            Console.WriteLine(Strings.Intro.loading);
        }

        #endregion

        #region Dependencies

        private static void ClearDlls()
        {
            Console.WriteLine("Deleting old dependencies...");

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
            Console.WriteLine("Exporting dependencies...");

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

            var platformBaseRid = "unknown";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                platformBaseRid = "linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                platformBaseRid = "osx";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                platformBaseRid = "win";
            }

            platformBaseRid = $"{platformBaseRid}_{RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant()}";

            var assembly = typeof(Bootstrapper).Assembly;
            var resourceNameRoot = $"Intersect.Server.Resources.runtimes.{platformBaseRid}";
            var resourceNames = assembly.GetManifestResourceNames();
            var matchingResourceNames = resourceNames
                .Where(name => name.StartsWith(resourceNameRoot) && name.Contains("e_sqlite3"))
                .ToArray();

            var sqliteResourceName = matchingResourceNames.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(sqliteResourceName))
            {
                throw new MissingManifestResourceException($"No matching manifest resource found for e_sqlite3");
            }

            var sqliteFileName = string.Join(
                '.',
                sqliteResourceName.Split('.').SkipWhile(part => !part.Contains("e_sqlite3"))
            );

            if (ReflectionUtils.ExtractResource(sqliteResourceName, sqliteFileName, assembly))
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