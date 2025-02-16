using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Intersect.Client.Networking;
using Intersect.Client.Plugins.Contexts;
using Intersect.Configuration;
using Intersect.Core;
using Intersect.Factories;
using Intersect.Framework.Net;
using Intersect.Network;
using Intersect.Plugins;
using Intersect.Plugins.Interfaces;
using Intersect.Reflection;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Core;

/// <summary>
/// Implements <see cref="IClientContext"/>.
/// </summary>
internal sealed partial class ClientContext : ApplicationContext<ClientContext, ClientCommandLineOptions>, IClientContext
{
    internal static bool IsSinglePlayer { get; set; }

    private IPlatformRunner? mPlatformRunner;

    internal ClientContext(
        Assembly entryAssembly,
        ClientCommandLineOptions startupOptions,
        ClientConfiguration clientConfiguration,
        ILogger logger,
        IPacketHelper packetHelper
    ) : base(entryAssembly, "Intersect", startupOptions, logger, packetHelper)
    {
        var hostNameOrAddress = clientConfiguration.Host;
        try
        {
            var address = Dns.GetHostAddresses(hostNameOrAddress).FirstOrDefault();
            IsDeveloper = !(address?.IsPublic() ?? false);
        }
        catch (SocketException socketException)
        {
            if (socketException.SocketErrorCode != SocketError.HostNotFound)
            {
                throw;
            }

            ClientNetwork.UnresolvableHostNames.Add(startupOptions.Server);
            ApplicationContext.Context.Value?.Logger.LogError(
                socketException,
                $"Failed to resolve host: '{hostNameOrAddress}'"
            );
            IsDeveloper = true;
        }

        _ = FactoryRegistry<IPluginContext>.RegisterFactory(new ClientPluginContext.Factory());
    }

    public bool IsDeveloper { get; }

    protected override bool UsesMainThread => true;

    public IPlatformRunner PlatformRunner
    {
        get => mPlatformRunner ?? throw new ArgumentNullException(nameof(PlatformRunner));
        private set => mPlatformRunner = value;
    }

    /// <inheritdoc />
    protected override void InternalStart()
    {
        Networking.Network.PacketHandler = new PacketHandler(this, PacketHelper.HandlerRegistry);
        PlatformRunner = typeof(ClientContext).Assembly.CreateInstanceOf<IPlatformRunner>();
        PlatformRunner.Start(this, PostStartup);
    }

    #region Exception Handling

    internal static void DispatchUnhandledException(
        Exception exception,
        bool isTerminating = true,
        bool wait = false
    )
    {
        var sender = Thread.CurrentThread;
        var task = Task.Factory.StartNew(
            () => HandleUnhandledException(sender, new UnhandledExceptionEventArgs(exception, isTerminating))
        );

        if (wait)
        {
            task.Wait();
        }
    }

    #endregion Exception Handling

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            PacketHelper.HandlerRegistry.Dispose();
        }
    }
}
