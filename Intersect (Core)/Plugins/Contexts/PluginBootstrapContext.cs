using CommandLine;
using Intersect.Factories;
using Intersect.Plugins.Helpers;
using Intersect.Plugins.Interfaces;
using Intersect.Properties;
using System.Globalization;
using System.Reflection;
using Intersect.Framework.Reflection;

namespace Intersect.Plugins.Contexts;

/// <summary>
/// Common <see cref="IPluginBootstrapContext"/> implementation.
/// </summary>
public sealed partial class PluginBootstrapContext : IPluginBootstrapContext
{
    /// <summary>
    /// Creates a <see cref="IFactory{TValue}"/> for <see cref="IPluginBootstrapContext"/>.
    /// </summary>
    /// <param name="hostPluginContextType">The <see cref="System.Type"/> of the plugin context that will be created by the plugin host.</param>
    /// <param name="args">the startup arguments that were parsed</param>
    /// <param name="parser">the <see cref="Parser"/> used to parse <paramref name="args"/></param>
    /// <param name="packetHelper"></param>
    /// <returns>a <see cref="IFactory{TValue}"/> instance</returns>
    public static IFactory<IPluginBootstrapContext>
        CreateFactory(Type hostPluginContextType, string[] args, Parser parser, IPacketHelper packetHelper) => new Factory(hostPluginContextType, args, parser, packetHelper);

    /// <summary>
    /// Factory implementation for <see cref="IPluginBootstrapContext"/>.
    /// </summary>
    private sealed partial class Factory : IFactory<IPluginBootstrapContext>
    {
        private readonly Type _hostPluginContextType;
        private readonly string[] _args;
        private readonly Parser _parser;
        private readonly IPacketHelper _packetHelper;

        /// <summary>
        /// Initializes a <see cref="Factory"/> for <see cref="IPluginBootstrapContext"/>.
        /// </summary>
        /// <param name="hostPluginContextType">The <see cref="System.Type"/> of the plugin context that will be created by the plugin host.</param>
        /// <param name="args">the startup arguments that were parsed</param>
        /// <param name="parser">the <see cref="Parser"/> used to parse <paramref name="args"/></param>
        /// <param name="packetHelper"></param>
        public Factory(Type hostPluginContextType, string[] args, Parser parser, IPacketHelper packetHelper)
        {
            _hostPluginContextType = hostPluginContextType;
            _args = args;
            _parser = parser;
            _packetHelper = packetHelper;
        }

        /// <inheritdoc />
        public IPluginBootstrapContext Create(params object[] args)
        {
            if (args.Length != 1)
            {
                var qualifiedPluginTypeName = typeof(Plugin).GetName(qualified: true);
                throw new ArgumentException(
                    $"`{nameof(args)}` should have 1 exactly argument (a {qualifiedPluginTypeName}).",
                    nameof(args)
                );
            }

            if (args[0] is not Plugin plugin)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentUICulture,
                        ExceptionMessages.PluginBootstrapContextMissingPluginArgument,
                        nameof(Plugin)
                    ),
                    nameof(args)
                );
            }

            return new PluginBootstrapContext(_hostPluginContextType, _args, _parser, plugin, _packetHelper);
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

    public Type HostPluginContextType { get; }

    /// <inheritdoc />
    public ILoggingHelper Logging => Plugin.Logging;

    /// <inheritdoc />
    public IManifestHelper Manifest => Plugin.Manifest;

    private PluginBootstrapContext(Type hostPluginContextType, string[] args, Parser parser, Plugin plugin, IPacketHelper parentPacketHelper)
    {
        HostPluginContextType = hostPluginContextType;
        Plugin = plugin;

        CommandLine = new CommandLineHelper(plugin.Logging.Plugin, args, parser);
        EmbeddedResources = new EmbeddedResourceHelper(Assembly);
        Packet = new PacketHelper(parentPacketHelper);
    }

    /// <inheritdoc />
    public TConfiguration GetTypedConfiguration<TConfiguration>() where TConfiguration : PluginConfiguration =>
        Configuration as TConfiguration;
}
