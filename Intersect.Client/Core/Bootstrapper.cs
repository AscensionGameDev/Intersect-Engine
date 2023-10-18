using CommandLine;

using Intersect.Factories;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Plugins;
using Intersect.Plugins.Contexts;
using Intersect.Plugins.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Intersect.Client.Core
{

    internal static partial class Bootstrapper
    {
        public static ClientContext Context { get; private set; }

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

            var logger = Log.Default;
            var packetTypeRegistry = new PacketTypeRegistry(logger);
            if (!packetTypeRegistry.TryRegisterBuiltIn())
            {
                logger.Error("Failed to register built-in packets.");
                return;
            }

            var packetHandlerRegistry = new PacketHandlerRegistry(packetTypeRegistry, logger);
            var packetHelper = new PacketHelper(packetTypeRegistry, packetHandlerRegistry);
            FactoryRegistry<IPluginBootstrapContext>.RegisterFactory(PluginBootstrapContext.CreateFactory(args, parser, packetHelper));

            var commandLineOptions = parser.ParseArguments<ClientCommandLineOptions>(args)
                .MapResult(HandleParsedArguments, HandleParserErrors);

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
                    Log.Warn($"Failed to set working directory to '{workingDirectory}', path does not exist: {resolvedWorkingDirectory}");
                }
            }

            Context = new ClientContext(commandLineOptions, logger, packetHelper);
            Context.Start();
        }

        private static ClientCommandLineOptions HandleParsedArguments(ClientCommandLineOptions clientCommandLineOptions) =>
            clientCommandLineOptions;

        private static ClientCommandLineOptions HandleParserErrors(IEnumerable<Error> errors)
        {
            var errorsAsList = errors?.ToList();

            var fatalParsingError = errorsAsList?.Any(error => error?.StopsProcessing ?? false) ?? false;

            var errorString = string.Join(
                ", ", errorsAsList?.ToList().Select(error => error?.ToString()) ?? Array.Empty<string>()
            );

            var exception = new ArgumentException(
                $@"Error parsing command line arguments, received the following errors: {errorString}"
            );

            if (fatalParsingError)
            {
                Log.Error(exception);
            }
            else
            {
                Log.Warn(exception);
            }

            return default;
        }

    }

}
