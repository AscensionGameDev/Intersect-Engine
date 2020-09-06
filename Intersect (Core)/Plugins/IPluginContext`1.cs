namespace Intersect.Plugins
{
    /// <summary>
    /// Defines the API of the <typeparamref name="TPluginContext"/> during application runtime.
    /// </summary>
    /// <typeparam name="TPluginContext">a <see cref="IPluginContext"/> subtype</typeparam>
    public interface IPluginContext<TPluginContext> : IPluginContext
        where TPluginContext : IPluginContext<TPluginContext>
    {
    }
}
