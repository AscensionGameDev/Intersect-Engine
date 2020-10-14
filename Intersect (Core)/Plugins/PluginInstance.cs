
using Intersect.Factories;

using JetBrains.Annotations;

using Microsoft;

namespace Intersect.Plugins
{
    /// <summary>
    /// Representation of an instance of a <see cref="Plugin"/>.
    /// </summary>
    public class PluginInstance
    {
        /// <summary>
        /// Create a <see cref="PluginInstance"/> for the given <see cref="Plugin"/>.
        /// </summary>
        /// <param name="plugin">The plugin descriptor to create an instance for.</param>
        /// <returns>A <see cref="PluginInstance"/> for <paramref name="plugin"/>.</returns>
        [NotNull]
        public static PluginInstance Create([NotNull, ValidatedNotNull] Plugin plugin)
        {
            var bootstrapContext = FactoryRegistry<IPluginBootstrapContext>.Create(plugin);
            var context = FactoryRegistry<IPluginContext>.Create(plugin);
            var entry = plugin.Reference.CreateInstance();
            return new PluginInstance(entry, bootstrapContext, context);
        }

        /// <summary>
        /// The entry point instance.
        /// </summary>
        [NotNull] public IPluginEntry Entry { get; }

        /// <summary>
        /// The context used for bootstrap lifecycle actions.
        /// </summary>
        [NotNull] public IPluginBootstrapContext BootstrapContext { get; }

        /// <summary>
        /// The context used for non-bootstrap lifecycle actions.
        /// </summary>
        [NotNull] public IPluginContext Context { get; }

        private PluginInstance(
            [NotNull] IPluginEntry entry,
            [NotNull] IPluginBootstrapContext bootstrapContext,
            [NotNull] IPluginContext context
        )
        {
            Entry = entry;
            BootstrapContext = bootstrapContext;
            Context = context;
        }

    }

}
