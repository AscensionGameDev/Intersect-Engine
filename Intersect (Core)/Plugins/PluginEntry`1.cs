namespace Intersect.Plugins
{
    /// <summary>
    /// Abstract class that defines translates between the generic <see cref="IPluginEntry"/> methods and the context type specific methods of <see cref="IPluginEntry{TPluginContext}"/>.
    /// </summary>
    /// <typeparam name="TPluginContext">The specific plugin context type.</typeparam>
    public abstract class PluginEntry<TPluginContext> : PluginEntry, IPluginEntry<TPluginContext>
        where TPluginContext : IPluginContext<TPluginContext>
    {
        /// <inheritdoc />
        public override void OnStart(IPluginContext context)
        {
            if (context is TPluginContext typedPluginContext)
            {
                OnStart(typedPluginContext);
            }
        }

        /// <inheritdoc />
        public override void OnStop(IPluginContext context)
        {
            if (context is TPluginContext typedPluginContext)
            {
                OnStop(typedPluginContext);
            }
        }

        /// <inheritdoc />
        public abstract void OnStart(TPluginContext context);

        /// <inheritdoc />
        public abstract void OnStop(TPluginContext context);
    }
}
