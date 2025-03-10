using System.Diagnostics;
using System.Reflection;
using Intersect.Editor.Forms;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Framework.Logging;
using Intersect.Framework.Reflection;
using Intersect.Framework.Utilities;
using Intersect.Network;
using Intersect.Plugins.Helpers;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ApplicationContext = Intersect.Core.ApplicationContext;


namespace Intersect.Editor.Core;

public static class Program
{
    private const string IconManifestResourceName = "Intersect.Editor.intersect-logo-qu.ico";
    internal static readonly Icon? Icon;

    static Program()
    {
        var iconStream = typeof(Program).Assembly.GetManifestResourceStream(IconManifestResourceName);
        if (iconStream == default)
        {
            Console.Error.WriteLine($"Unable to find embedded resource with name '{IconManifestResourceName}'.");
        }
        else
        {
            Icon = new Icon(iconStream);
            Console.WriteLine("Loaded embedded application icon successfully");
        }
    }

    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        Console.WriteLine(
            $"Starting {executingAssembly.GetMetadataName()} in {Environment.CurrentDirectory}...\n\t{string.Join(' ', args)}"
        );

        var executingAssemblyName = executingAssembly.GetName();

        LoggingLevelSwitch loggingLevelSwitch =
            new(Debugger.IsAttached ? LogEventLevel.Debug : LogEventLevel.Information);

        var (loggerFactory, logger) = new LoggerConfiguration().CreateLoggerForIntersect(
            executingAssembly,
            "Editor",
            loggingLevelSwitch
        );

        var packetTypeRegistry = new PacketTypeRegistry(
            loggerFactory.CreateLogger<PacketTypeRegistry>(),
            typeof(IntersectPacket).Assembly
        );
        if (!packetTypeRegistry.TryRegisterBuiltIn())
        {
            throw new Exception("Failed to register built-in packets.");
        }

        var packetHandlerRegistry = new PacketHandlerRegistry(
            packetTypeRegistry,
            loggerFactory.CreateLogger<PacketHandlerRegistry>()
        );
        var packetHelper = new PacketHelper(packetTypeRegistry, packetHandlerRegistry);
        PackedIntersectPacket.AddKnownTypes(packetHelper.AvailablePacketTypes);
        EditorContext editorContext = new(executingAssembly, packetHelper, logger);

        ApplicationContext.CurrentContext.Logger.LogTrace("Starting editor...");

        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        Application.ThreadException += Application_ThreadException;
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        ApplicationContext.CurrentContext.Logger.LogTrace("Unpacking libraries...");

        //Place sqlite3.dll where it's needed.
        var dllname = Environment.Is64BitProcess ? "sqlite3x64.dll" : "sqlite3x86.dll";
        using (var resourceStream = Assembly.GetExecutingAssembly()
                   .GetManifestResourceStream("Intersect.Editor.Resources." + dllname))
        {
            Debug.Assert(resourceStream != null, "resourceStream != null");
            using (var fileStream = new FileStream("sqlite3.dll", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var data = new byte[resourceStream.Length];
                resourceStream.Read(data, 0, (int)resourceStream.Length);
                fileStream.Write(data, 0, data.Length);
            }
        }

        ApplicationContext.CurrentContext.Logger.LogTrace("Libraries unpacked.");

        ApplicationContext.CurrentContext.Logger.LogTrace("Creating forms...");
        Globals.UpdateForm = new FrmUpdate();
        ApplicationContext.CurrentContext.Logger.LogTrace("Forms created.");

        ApplicationContext.CurrentContext.Logger.LogTrace("Starting application.");
        Application.Run(Globals.UpdateForm);
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        CurrentDomain_UnhandledException(null, new UnhandledExceptionEventArgs(e.Exception, true));
    }

    //Really basic error handler for debugging purposes
    public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        ApplicationContext.CurrentContext.Logger.LogError(args.ExceptionObject as Exception, "Unhandled exception");
        MessageBox.Show(Strings.Errors.UnhandledError);

        Environment.Exit(1);
    }
}