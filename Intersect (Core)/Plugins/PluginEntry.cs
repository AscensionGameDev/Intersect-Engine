namespace Intersect.Plugins
{
    /// <summary>
    /// Abstract class that virtually defines all of the methods declared by <see cref="IPluginEntry"/>.
    /// </summary>
    public abstract class PluginEntry : IPluginEntry
    {
        /// <inheritdoc />
        public virtual void OnBootstrap(IPluginBootstrapContext context) { }

        /// <inheritdoc />
        public virtual void OnStart(IPluginContext context) { }

        /// <inheritdoc />
        public virtual void OnStop(IPluginContext context) { }
    }
}
