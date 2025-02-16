using System.Diagnostics;
using System.Reflection;
using CommandLine;
using Intersect.Configuration;
using Intersect.Core;
using Intersect.Factories;
using Intersect.Framework.Logging;
using Intersect.Network;
using Intersect.Plugins;
using Intersect.Plugins.Contexts;
using Intersect.Plugins.Helpers;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Intersect.Client.Core;

internal static partial class Bootstrapper
{
    public static void Start(Assembly entryAssembly, params string[] args)
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

        LoggingLevelSwitch loggingLevelSwitch =
            new(Debugger.IsAttached ? LogEventLevel.Debug : LogEventLevel.Information);

        var executingAssembly = Assembly.GetExecutingAssembly();
        var (_, logger) = new LoggerConfiguration().CreateLoggerForIntersect(
            entryAssembly,
            "Client",
            loggingLevelSwitch
        );

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
                ApplicationContext.Context.Value?.Logger.LogWarning(
                    "Failed to set working directory to '{Path}', path does not exist: {ResolvedPath}",
                    workingDirectory,
                    resolvedWorkingDirectory
                );
            }
        }

        var clientConfiguration = ClientConfiguration.LoadAndSave();
        loggingLevelSwitch.MinimumLevel = LevelConvert.ToSerilogLevel(clientConfiguration.LogLevel);

        if (commandLineOptions.Server is { } server)
        {
            var serverParts = server.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var host = serverParts.First();

            var portPart = serverParts.Skip(1).FirstOrDefault();
            if (string.IsNullOrEmpty(portPart))
            {
                portPart = "5400";
            }
            var port = ushort.Parse(portPart);

            clientConfiguration.Host = host;
            clientConfiguration.Port = port;
        }

        commandLineOptions = commandLineOptions with
        {
            Server = $"{clientConfiguration.Host}:{clientConfiguration.Port}",
        };

        ClientContext context = new(entryAssembly, commandLineOptions, clientConfiguration, logger, packetHelper);
        context.Start();
    }

    private static ClientCommandLineOptions HandleParsedArguments(ClientCommandLineOptions clientCommandLineOptions) =>
        clientCommandLineOptions;

    private static ClientCommandLineOptions HandleParserErrors(IEnumerable<Error> errors)
    {
        var errorsAsList = errors.ToList();
        var fatalParsingError = errorsAsList.Any(error => error.StopsProcessing);
        var errorString = string.Join(", ", errorsAsList.ToList().Select(error => error.ToString()));

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
