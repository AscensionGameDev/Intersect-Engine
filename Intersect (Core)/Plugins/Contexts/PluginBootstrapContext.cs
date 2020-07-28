using CommandLine;

using Intersect.Factories;
using Intersect.Plugins.Helpers;
using Intersect.Plugins.Interfaces;
using Intersect.Properties;

using JetBrains.Annotations;

using System;
using System.Globalization;
using System.Reflection;

namespace Intersect.Plugins.Contexts
{
    /// <summary>
    /// Common <see cref="IPluginBootstrapContext"/> implementation.
    /// </summary>
    public sealed class PluginBootstrapContext : IPluginBootstrapContext
    {
        /// <summary>
        /// Creates a <see cref="IFactory{TValue}"/> for <see cref="IPluginBootstrapContext"/>.
        /// </summary>
        /// <param name="args">the startup arguments that were parsed</param>
        /// <param name="parser">the <see cref="Parser"/> used to parse <paramref name="args"/></param>
        /// <returns>a <see cref="IFactory{TValue}"/> instance</returns>
        public static IFactory<IPluginBootstrapContext>
            CreateFactory([NotNull] string[] args, [NotNull] Parser parser) => new Factory(args, parser);

        /// <summary>
        /// Factory implementation for <see cref="IPluginBootstrapContext"/>.
        /// </summary>
        private sealed class Factory : IFactory<IPluginBootstrapContext>
        {
            [NotNull] private readonly string[] mArgs;

            [NotNull] private readonly Parser mParser;

            /// <summary>
            /// Initializes a <see cref="Factory"/> for <see cref="IPluginBootstrapContext"/>.
            /// </summary>
            /// <param name="args">the startup arguments that were parsed</param>
            /// <param name="parser">the <see cref="Parser"/> used to parse <paramref name="args"/></param>
            public Factory([NotNull] string[] args, [NotNull] Parser parser)
            {
                mArgs = args;
                mParser = parser;
            }

            /// <inheritdoc />
            public IPluginBootstrapContext Create(params object[] args)
            {
                if (args.Length < 1 || !(args[0] is Plugin plugin))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture, ExceptionMessages.PluginBootstrapContextMissingPluginArgument,
                            nameof(Plugin)
                        ), nameof(args)
                    );
                }

                return new PluginBootstrapContext(mArgs, mParser, plugin);
            }
        }

        [NotNull] private Plugin Plugin { get; }

        /// <inheritdoc />
        public ICommandLineHelper CommandLine { get; }

        /// <inheritdoc />
        public Assembly Assembly => Plugin.Reference.Assembly;

        /// <inheritdoc />
        public PluginConfiguration Configuration => Plugin.Configuration;

        /// <inheritdoc />
        public IEmbeddedResourceHelper EmbeddedResources { get; }

        /// <inheritdoc />
        public ILoggingHelper Logging => Plugin.Logging;

        /// <inheritdoc />
        public IManifestHelper Manifest => Plugin.Manifest;

        private PluginBootstrapContext([NotNull] string[] args, [NotNull] Parser parser, [NotNull] Plugin plugin)
        {
            Plugin = plugin;

            CommandLine = new CommandLineHelper(plugin.Logging.Plugin, args, parser);
            EmbeddedResources = new EmbeddedResourceHelper(Assembly);
        }

        /// <inheritdoc />
        public TConfiguration GetTypedConfiguration<TConfiguration>() where TConfiguration : PluginConfiguration =>
            Configuration as TConfiguration;
    }
}
