using Intersect.Framework.Reflection;

namespace Intersect.Plugins;

/// <summary>
/// Defines the API that plugin entry points must implement
/// in order to be discovered by the plugin loader with
/// specialized application lifecycle handlers.
/// </summary>
/// <typeparam name="TPluginContext">the specialized <see cref="IPluginContext"/> type</typeparam>
/// <see cref="Loaders.PluginLoader.LoadFrom(Core.IApplicationContext, System.Reflection.Assembly)"/>
public interface IPluginEntry<in TPluginContext> : IPluginEntry
    where TPluginContext : IPluginContext<TPluginContext>
{
    void IPluginEntry.OnStart(IPluginContext context)
    {
        if (context is not TPluginContext typedContext)
        {
            var expectedType = typeof(TPluginContext).GetName(qualified: true);
            var actualType = context.GetType().GetName(qualified: true);
            throw new ArgumentException($"Expected {expectedType} but received {actualType}", nameof(context));
        }

        OnStart(typedContext);
    }

    void IPluginEntry.OnStop(IPluginContext context)
    {
        if (context is not TPluginContext typedContext)
        {
            var expectedType = typeof(TPluginContext).GetName(qualified: true);
            var actualType = context.GetType().GetName(qualified: true);
            throw new ArgumentException($"Expected {expectedType} but received {actualType}", nameof(context));
        }

        OnStop(typedContext);
    }

    /// <summary>
    /// Invoked during application startup after basic initialization.
    /// </summary>
    /// <param name="context">the current specialized plugin context</param>
    void OnStart(TPluginContext context);

    /// <summary>
    /// Invoked during application shutdown.
    /// </summary>
    /// <param name="context">the current specialized plugin context</param>
    void OnStop(TPluginContext context);
}
