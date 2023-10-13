// See https://aka.ms/new-console-template for more information

using Intersect;
using Intersect.Client.MonoGame.Network;
using Intersect.Configuration;
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

ClientConfiguration.ResourcesDirectory = "client-resources";
Options.ResourcesDirectory = "server-resources";
ServerContext.ResourceDirectory = "server-resources";

Thread serverThread = new(args => Bootstrapper.Start(args as string[]));
serverThread.Start(args);

Intersect.Client.Program.Main(args);
