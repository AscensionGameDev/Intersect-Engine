using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Intersect;
using Intersect.Client.Core;
using Intersect.Client.MonoGame.Network;
using Intersect.Configuration;
using Intersect.Framework.Reflection;
using Intersect.Server.Core;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.SinglePlayer.Networking;
using Bootstrapper = Intersect.Server.Core.Bootstrapper;
using Console = System.Console;

var executingAssembly = Assembly.GetExecutingAssembly();
Console.WriteLine($"Starting {executingAssembly.GetMetadataName()}...");

const string singleplayer = "singleplayer";
var singleplayerPassword = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(singleplayer)));

ClientConfiguration.ResourcesDirectory = "client-resources";
Options.ResourcesDirectory = "server-resources";
ServerContext.ResourceDirectory = "server-resources";

ClientContext.IsSinglePlayer = true;

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

try
{
    Bootstrapper.OnPostContextSetupCompleted += () =>
    {
        if (User.Count() < 1)
        {
            DbInterface.CreateAccount(
                default,
                singleplayer,
                singleplayerPassword,
                singleplayer,
#if DEBUG
                grantFirstUserAdmin: true
#else
                grantFirstUserAdmin: false
#endif
            );
        }
    };

    Thread serverThread = new(args => Bootstrapper.Start(executingAssembly, args as string[]));
    serverThread.Start(args.Append("--migrate-automatically").Distinct().ToArray());

    Intersect.Client.Core.Program.Main(executingAssembly, args);
}
finally
{
    Bootstrapper.Context?.RequestShutdown();
}


