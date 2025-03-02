namespace Intersect.Client.Plugins.Interfaces;

public delegate void LifecycleChangeStateHandler(
    IClientPluginContext context,
    LifecycleChangeStateArgs lifecycleChangeStateArgs
);