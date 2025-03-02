namespace Intersect.Client.Plugins.Interfaces;

public delegate void GameUpdateHandler(
    IClientPluginContext context,
    GameUpdateArgs gameUpdateArgs
);