using Intersect.Client.Plugins;
using Intersect.Framework.Multitarget;
using Intersect.Framework.Reflection;
using Intersect.Plugins;
using Intersect.Server.Plugins;
using Microsoft.Extensions.Logging;

namespace Intersect.Examples.Plugin.Multitarget;

public class ExampleMultitargetPluginEntry : MultitargetPluginEntry
{
    public override void OnBootstrap(IPluginBootstrapContext context)
    {
        context.Logging.Plugin.LogInformation(
            "Starting multi-target plugin {PluginName} v{PluginVersion} for a {PluginHostContext}...",
            context.Manifest.Name,
            context.Manifest.Version,
            context.HostPluginContextType.GetName(qualified: true)
        );
    }

    public override void OnStart(IClientPluginContext context)
    {
        context.Logging.Plugin.LogInformation(
            "Starting multi-target plugin {PluginName} v{PluginVersion} in client mode...",
            context.Manifest.Name,
            context.Manifest.Version
        );
    }

    public override void OnStop(IClientPluginContext context)
    {
        context.Logging.Plugin.LogInformation(
            "Stopping multi-target plugin {PluginName} v{PluginVersion} in client mode...",
            context.Manifest.Name,
            context.Manifest.Version
        );
    }

    public override void OnStart(IServerPluginContext context)
    {
        context.Logging.Plugin.LogInformation(
            "Starting multi-target plugin {PluginName} v{PluginVersion} in server mode...",
            context.Manifest.Name,
            context.Manifest.Version
        );
    }

    public override void OnStop(IServerPluginContext context)
    {
        context.Logging.Plugin.LogInformation(
            "Stopping multi-target plugin {PluginName} v{PluginVersion} in server mode...",
            context.Manifest.Name,
            context.Manifest.Version
        );
    }
}