using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Intersect.Client.Networking;
using Intersect.Client.Plugins.Contexts;
using Intersect.Configuration;
using Intersect.Core;
using Intersect.Factories;
using Intersect.Framework.Core.Network.Packets.Security;
using Intersect.Framework.Core.Security;
using Intersect.Framework.Net;
using Intersect.Framework.Reflection;
using Intersect.Network;
using Intersect.Plugins;
using Intersect.Plugins.Interfaces;
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
        PermissionSet.ActivePermissionSetChanged += PermissionSetOnActivePermissionSetChanged;
        PermissionSet.PermissionSetUpdated += PermissionSetOnPermissionSetUpdated;

        var hostNameOrAddress = clientConfiguration.Host;
        try
        {
            var address = Dns.GetHostAddresses(hostNameOrAddress).FirstOrDefault();
            IsDeveloper = !(address?.IsPublic() ?? true);
        }
        catch (SocketException socketException)
        {
            if (socketException.SocketErrorCode != SocketError.HostNotFound)
            {
                throw;
            }

            ClientNetwork.UnresolvableHostNames.Add(hostNameOrAddress);
            ApplicationContext.Context.Value?.Logger.LogError(
                socketException,
                "Failed to resolve host '{HostNameOrAddress}'",
                hostNameOrAddress
            );
            IsDeveloper = true;
        }

        _ = FactoryRegistry<IPluginContext>.RegisterFactory(new ClientPluginContext.Factory());
    }

    private void PermissionSetOnPermissionSetUpdated(PermissionSet permissionSet)
    {
        if (permissionSet != PermissionSet.ActivePermissionSet)
        {
            return;
        }

        IsDeveloper = permissionSet.IsGranted(Permission.EngineVersion);
        PermissionsChanged?.Invoke(permissionSet);
    }

    private void PermissionSetOnActivePermissionSetChanged(string activePermissionSetName)
    {
        PermissionSetOnPermissionSetUpdated(PermissionSet.ActivePermissionSet);
    }

    public event PermissionSetChangedHandler? PermissionsChanged;

    public bool IsDeveloper { get; private set; }

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
