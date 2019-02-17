using Intersect.Logging;
using Intersect.Server.Localization;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using CommandLine;
using JetBrains.Annotations;

namespace Intersect.Server.Core
{
    internal static class Bootstrapper
    {
        static Bootstrapper()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private static ServerContext sContext;

        public static void Start(params string[] args)
        {
            if (RunningOnWindows())
            {
                SetConsoleCtrlHandler(ConsoleCtrlCheck, true);
            }

            var commandLineOptions = ParseCommandLineArgs(args);
            sContext = new ServerContext(commandLineOptions);
            ServerStatic.Start(args);
        }

        [NotNull]
        private static CommandLineOptions ParseCommandLineArgs(params string[] args)
        {
            return new Parser()
                       .ParseArguments<CommandLineOptions>(args)
                       .MapResult(
                           commandLineOptions => commandLineOptions,
                           errors => null
                       ) ?? throw new InvalidOperationException();
        }

        #region AppDomain

        private static Assembly OnAssemblyResolve([NotNull] object sender, [NotNull] ResolveEventArgs args)
        {
            // Ignore missing resources
            if (args.Name?.Contains(".resources") ?? false)
            {
                
return null;
            }

            // check for assemblies already loaded
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(fodAssembly => fodAssembly?.FullName == args.Name);
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
                    : Path.Combine("libs", "server", "x86"),
                filename
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
        public static void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEvent)
        {
            ProcessUnhandledException(sender, unhandledExceptionEvent?.ExceptionObject as Exception);
            if (!(unhandledExceptionEvent?.IsTerminating ?? false))
            {
                Console.WriteLine(Strings.Errors.errorlogged);
            }

            if (sContext?.StartupOptions.DoNotHaltOnError ?? false)
            {
                Console.WriteLine(Strings.Errors.errorservercrashnohalt);
            }
            else
            {
                Console.WriteLine(Strings.Errors.errorservercrash);
                Console.ReadKey();
            }

            sContext?.Dispose();
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

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            // Put your own handler here
            switch (ctrlType)
            {
                case CtrlTypes.CtrlCEvent:
                case CtrlTypes.CtrlBreakEvent:
                    //Handled Elsewhere
                    break;

                case CtrlTypes.CtrlCloseEvent:
                    ServerContext.Instance.Dispose();
                    //Shutdown();
                    break;

                case CtrlTypes.CtrlLogoffEvent:
                case CtrlTypes.CtrlShutdownEvent:
                    ServerContext.Instance.Dispose();
                    //Shutdown();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(ctrlType), ctrlType, null);
            }

            return true;
        }

        private static bool RunningOnWindows()
        {
            var os = Environment.OSVersion;
            var pid = os.Platform;
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return true;
            }

            return false;
        }

        #endregion
    }
}