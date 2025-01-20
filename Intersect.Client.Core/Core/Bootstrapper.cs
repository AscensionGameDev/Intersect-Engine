using System.Diagnostics;
using System.Reflection;
using CommandLine;
using Intersect.Core;
using Intersect.Factories;
using Intersect.Network;
using Intersect.Plugins;
using Intersect.Plugins.Contexts;
using Intersect.Plugins.Helpers;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Intersect.Client.Core;

internal static partial class Bootstrapper
{
    public static void Start(params string[] args)
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
            }
        );

        var commandLineOptions = parser.ParseArguments<ClientCommandLineOptions>(args)
            .MapResult(HandleParsedArguments, HandleParserErrors);

        var executableName = Path.GetFileNameWithoutExtension(
            Process.GetCurrentProcess().MainModule?.FileName ?? Assembly.GetExecutingAssembly().GetName().Name
        );
        var loggerConfiguration = new LoggerConfiguration().MinimumLevel
            .Is(Debugger.IsAttached ? LogEventLevel.Debug : LogEventLevel.Information).Enrich.FromLogContext().WriteTo
            .Console().WriteTo.File(
                Path.Combine(
                    "logs",
                    $"{executableName}-{Process.GetCurrentProcess().StartTime:yyyy_MM_dd-HH_mm_ss_fff}.log"
                )
            ).WriteTo.File(
                Path.Combine("logs", $"errors-{executableName}.log"),
                restrictedToMinimumLevel: LogEventLevel.Error
            );

        var logger = new SerilogLoggerFactory(loggerConfiguration.CreateLogger()).CreateLogger("Client");

        var packetTypeRegistry = new PacketTypeRegistry(logger, typeof(SharedConstants).Assembly);
        if (!packetTypeRegistry.TryRegisterBuiltIn())
        {
            logger.LogError("Failed to register built-in packets.");
            return;
        }

        var packetHandlerRegistry = new PacketHandlerRegistry(packetTypeRegistry, logger);
        var packetHelper = new PacketHelper(packetTypeRegistry, packetHandlerRegistry);
        _ = FactoryRegistry<IPluginBootstrapContext>.RegisterFactory(PluginBootstrapContext.CreateFactory(args, parser, packetHelper));

        if (!string.IsNullOrWhiteSpace(commandLineOptions.WorkingDirectory))
        {
            var workingDirectory = commandLineOptions.WorkingDirectory.Trim();
            var resolvedWorkingDirectory = Path.GetFullPath(workingDirectory);
            if (Directory.Exists(resolvedWorkingDirectory))
            {
                Environment.CurrentDirectory = resolvedWorkingDirectory;
            }
            else
            {
                ApplicationContext.Context.Value?.Logger.LogWarning($"Failed to set working directory to '{workingDirectory}', path does not exist: {resolvedWorkingDirectory}");
            }
        }

        var context = new ClientContext(commandLineOptions, logger, packetHelper);
        ApplicationContext.Context.Value = context;
        context.Start();
    }

    private static ClientCommandLineOptions HandleParsedArguments(ClientCommandLineOptions clientCommandLineOptions) =>
        clientCommandLineOptions;

    private static ClientCommandLineOptions HandleParserErrors(IEnumerable<Error> errors)
    {
        var errorsAsList = errors?.ToList();
        var fatalParsingError = errorsAsList?.Any(error => error?.StopsProcessing ?? false) ?? false;
        var errorString = string.Join(", ", errorsAsList?.ToList().Select(error => error?.ToString()) ?? []);

        var exception = new ArgumentException(
            $@"Error parsing command line arguments, received the following errors: {errorString}"
        );

        if (fatalParsingError)
        {
            ApplicationContext.Context.Value?.Logger.LogCritical(
                exception,
                "Critical error during command line argument parsing"
            );
        }
        else
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Error occurred during command line argument parsing"
            );
        }

        return default;
    }
}
