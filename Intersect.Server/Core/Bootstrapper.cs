using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using CommandLine;

using Intersect.Logging;
using Intersect.Server.Database;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Networking.Helpers;
using Intersect.Threading;
using Intersect.Utilities;

using JetBrains.Annotations;

namespace Intersect.Server.Core
{

    internal static class Bootstrapper
    {

        static Bootstrapper()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += UnobservedTaskException;

            Console.CancelKeyPress += OnConsoleCancelKeyPress;
        }

        public static ServerContext Context { get; private set; }

        private static bool mErrored { get; set; }

        [NotNull]
        public static LockingActionQueue MainThread { get; private set; }

        public static void Start(params string[] args)
        {
            if (!PreContextSetup(args))
            {
                return;
            }

            var commandLineOptions = ParseCommandLineArgs(args);
            Context = new ServerContext(commandLineOptions);
            var noHaltOnError = Context?.StartupOptions.DoNotHaltOnError ?? false;

            if (!PostContextSetup())
            {
                return;
            }

            MainThread = ServerContext.Instance.StartWithActionQueue();
            Action action;
            while (null != (action = MainThread.NextAction))
            {
                action.Invoke();
            }

            Log.Diagnostic("Bootstrapper exited.");

            //At this point dbs should be saved and all threads should be killed. Give a message saying that the server has shutdown and to press any key to exit.
            //Having the message and the console.readline() allows the server to exit properly if the console has crashed, and it allows us to know that the server context has shutdown.
            if (mErrored)
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

        [NotNull]
        private static CommandLineOptions ParseCommandLineArgs(params string[] args)
        {
            return new Parser(
                           settings =>
                           {
                               if (settings == null)
                               {
                                   throw new ArgumentNullException(
                                       nameof(settings),
                                       @"If this is null the CommandLineParser dependency is likely broken."
                                   );
                               }

                               settings.IgnoreUnknownArguments = true;
                               settings.MaximumDisplayWidth = Console.BufferWidth;
                           }
                       ).ParseArguments<CommandLineOptions>(args)
                       .MapResult(commandLineOptions => commandLineOptions, errors => null) ??
                   throw new InvalidOperationException();
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
            [NotNull] object sender,
            [NotNull] ConsoleCancelEventArgs cancelEvent
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

            return true;
        }

        private static bool PostContextSetup()
        {
            if (Context == null)
            {
                throw new ArgumentNullException(nameof(Context));
            }

            if (!DbInterface.InitDatabase(Context))
            {
                Console.ReadKey();

                return false;
            }

            Console.WriteLine();

            Console.WriteLine(Strings.Commandoutput.playercount.ToString(DbInterface.RegisteredPlayers));
            Console.WriteLine(Strings.Commandoutput.gametime.ToString(Time.GetTime().ToString("F")));

            Time.Update();

            PacketSender.CacheGameDataPacket();

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

        #region AppDomain

        private static Assembly OnAssemblyResolve([NotNull] object sender, [NotNull] ResolveEventArgs args)
        {
            // Ignore missing resources
            if (args.Name?.Contains(".resources") ?? false)
            {
                return null;
            }

            // check for assemblies already loaded
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(fodAssembly => fodAssembly?.FullName == args.Name);

            if (assembly != null)
            {
                return assembly;
            }

            var filename = args.Name?.Split(',')[0] + ".dll";

            //Try Loading from libs/server first
            Debug.Assert(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase != null,
                "AppDomain.CurrentDomain.SetupInformation.ApplicationBase != null"
            );

            var libsFolder = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "libs", "server");
            if (File.Exists(Path.Combine(libsFolder, filename)))
            {
                return Assembly.LoadFile(Path.Combine(libsFolder, filename));
            }

            var archSpecificPath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                Environment.Is64BitProcess
                    ? Path.Combine("libs", "server", "x64")
                    : Path.Combine("libs", "server", "x86"), filename
            );

            return File.Exists(archSpecificPath) ? Assembly.LoadFile(archSpecificPath) : null;
        }

        public static void ProcessUnhandledException([NotNull] object sender, [NotNull] Exception exception)
        {
            Log.Error($"Received unhandled exception from {sender}.");
            Log.Error(exception);
            if (exception.InnerException != null)
            {
                Log.Error($"Inner Exception?");
                Log.Error(exception.InnerException);

                if (exception.InnerException.InnerException != null)
                {
                    Log.Error($"Inner Exception? Inner Exception?");
                    Log.Error(exception.InnerException.InnerException);
                }
            }
        }

        //Really basic error handler for debugging purposes
        public static void OnUnhandledException(
            [NotNull] object sender,
            [NotNull] UnhandledExceptionEventArgs unhandledExceptionEvent
        )
        {
            ProcessUnhandledException(
                sender, unhandledExceptionEvent.ExceptionObject as Exception ?? throw new InvalidOperationException()
            );

            if (!(unhandledExceptionEvent?.IsTerminating ?? false))
            {
                Console.WriteLine(Strings.Errors.errorlogged);
            }

            mErrored = true;

            //Dispose Server Context Before Waiting
            //Under no circumstances do we want the game to continue
            if (!(Context?.IsDisposed ?? true))
            {
                Context?.RequestShutdown(true);
            }
        }

        private static void UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ProcessUnhandledException(
                sender, e.Exception.InnerException as Exception ?? throw new InvalidOperationException()
            );

            mErrored = true;

            //Dispose Server Context Before Waiting
            //Under no circumstances do we want the game to continue
            if (!(Context?.IsDisposed ?? true))
            {
                Context?.RequestShutdown(true);
            }
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

        [NotNull] private static readonly HandlerRoutine ConsoleCtrlHandler = ConsoleCtrlCheck;

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
