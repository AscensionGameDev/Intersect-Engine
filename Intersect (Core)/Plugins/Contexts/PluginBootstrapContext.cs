using CommandLine;

using Intersect.Factories;
using Intersect.Plugins.Helpers;
using Intersect.Plugins.Interfaces;
using Intersect.Properties;

using System;
using System.Globalization;
using System.Reflection;

namespace Intersect.Plugins.Contexts
{
    /// <summary>
    /// Common <see cref="IPluginBootstrapContext"/> implementation.
    /// </summary>
    public sealed partial class PluginBootstrapContext : IPluginBootstrapContext
    {
        /// <summary>
        /// Creates a <see cref="IFactory{TValue}"/> for <see cref="IPluginBootstrapContext"/>.
        /// </summary>
        /// <param name="args">the startup arguments that were parsed</param>
        /// <param name="parser">the <see cref="Parser"/> used to parse <paramref name="args"/></param>
        /// <param name="packetHelper"></param>
        /// <returns>a <see cref="IFactory{TValue}"/> instance</returns>
        public static IFactory<IPluginBootstrapContext>
            CreateFactory(string[] args, Parser parser, IPacketHelper packetHelper) => new Factory(args, parser, packetHelper);

        /// <summary>
        /// Factory implementation for <see cref="IPluginBootstrapContext"/>.
        /// </summary>
        private sealed partial class Factory : IFactory<IPluginBootstrapContext>
        {
            private readonly string[] mArgs;

            private readonly Parser mParser;

            private readonly IPacketHelper mPacketHelper;

            /// <summary>
            /// Initializes a <see cref="Factory"/> for <see cref="IPluginBootstrapContext"/>.
            /// </summary>
            /// <param name="args">the startup arguments that were parsed</param>
            /// <param name="parser">the <see cref="Parser"/> used to parse <paramref name="args"/></param>
            /// <param name="packetHelper"></param>
            public Factory(string[] args, Parser parser, IPacketHelper packetHelper)
            {
                mArgs = args;
                mParser = parser;
                mPacketHelper = packetHelper;
            }

            /// <inheritdoc />
            public IPluginBootstrapContext Create(params object[] args)
            {
                if (args.Length != 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(args), $"{nameof(args)} should have 1 argument.");
                }

                if (!(args[0] is Plugin plugin))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture, ExceptionMessages.PluginBootstrapContextMissingPluginArgument,
                            nameof(Plugin)
                        ), nameof(args)
                    );
                }

                return new PluginBootstrapContext(mArgs, mParser, plugin, mPacketHelper);
            }
        }

        private Plugin Plugin { get; }

        /// <inheritdoc />
        public ICommandLineHelper CommandLine { get; }

        /// <inheritdoc />
        public IPacketHelper Packet { get; }

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

        private PluginBootstrapContext(string[] args, Parser parser, Plugin plugin, IPacketHelper parentPacketHelper)
        {
            Plugin = plugin;

            CommandLine = new CommandLineHelper(plugin.Logging.Plugin, args, parser);
            EmbeddedResources = new EmbeddedResourceHelper(Assembly);
            Packet = new PacketHelper(parentPacketHelper);
        }

        /// <inheritdoc />
        public TConfiguration GetTypedConfiguration<TConfiguration>() where TConfiguration : PluginConfiguration =>
            Configuration as TConfiguration;
    }
}
