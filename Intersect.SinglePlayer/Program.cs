// See https://aka.ms/new-console-template for more information

using Intersect.Client.MonoGame.Network;
using Intersect.Server.Core;
using Intersect.SinglePlayer.Networking;

SinglePlayerNetwork? clientNetwork = default;
SinglePlayerNetwork? serverNetwork = default;

MonoSocket.NetworkFactory = (context, _, _, _) =>
{
    clientNetwork = new SinglePlayerNetwork(context, () => serverNetwork);
    return clientNetwork;
};

ServerContext.NetworkFactory = (context, _, _, _) =>
{
    serverNetwork = new SinglePlayerNetwork(context, () => clientNetwork);
    return serverNetwork;
};

Bootstrapper.Start(args);

Intersect.Client.Program.Main(args);
