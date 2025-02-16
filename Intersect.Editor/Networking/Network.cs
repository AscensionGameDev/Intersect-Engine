using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using Intersect.Configuration;
using Intersect.Editor.General;
using Intersect.Network;
using Intersect.Core;
using Intersect.Editor.Core;
using Intersect.Framework.Core;
using Intersect.Network.Events;
using Intersect.Threading;
using Intersect.Plugins.Helpers;
using Intersect.Utilities;
using Intersect.Network.Packets.Unconnected.Client;
using Intersect.Rsa;
using Microsoft.Extensions.Logging;
using ApplicationContext = Intersect.Core.ApplicationContext;

namespace Intersect.Editor.Networking;

internal static partial class Network
{

    public static bool Connecting;

    public static bool ConnectionDenied;

    public static ClientNetwork EditorLidgrenNetwork { get; set; }

    internal static PacketHandler PacketHandler { get; private set; }

    public static bool Connected => EditorLidgrenNetwork?.IsConnected ?? false;

    public static void Connect()
    {
        if (EditorLidgrenNetwork?.Connect() ?? true)
        {
            return;
        }

        Intersect.Core.ApplicationContext.Context.Value?.Logger.LogWarning("Failed to connect to server.");
    }

    public static void InitNetwork()
    {
        if (EditorLidgrenNetwork == null)
        {
            var editorContext = ApplicationContext.GetCurrentContext<EditorContext>();
            var packetHandlerRegistry = editorContext.PacketHelper.HandlerRegistry;
            packetHandlerRegistry.TryRegisterAvailableTypeHandlers(typeof(Intersect.Editor.Networking.Network).Assembly, requireAttribute: true);
            PacketHandler = new PacketHandler(editorContext, packetHandlerRegistry);

            var config = new NetworkConfiguration(
                ClientConfiguration.Instance.Host, ClientConfiguration.Instance.Port
            );

            var executingAssembly = Assembly.GetExecutingAssembly();
            using (var stream = executingAssembly.GetManifestResourceStream("Intersect.Editor.network.handshake.bkey.pub"))
            {
                var rsaKey = new RsaKey(stream);
                Debug.Assert(rsaKey != null, "rsaKey != null");
                EditorLidgrenNetwork = new ClientNetwork(editorContext, config, rsaKey.Parameters);
            }

            EditorLidgrenNetwork.Handler = PacketHandler.HandlePacket;
            EditorLidgrenNetwork.OnDisconnected += HandleDc;
            EditorLidgrenNetwork.OnConnectionDenied += delegate
            {
                Connecting = false;
                ConnectionDenied = true;
            };
        }

        if (!Connected)
        {
            Connecting = true;
        }
    }

    /// <summary>
    /// Interval between status pings in ms (e.g. full, bad version, etc.)
    /// </summary>
#if DEBUG
    private const long ServerStatusPingInterval = 15_000;
#else
    private const long ServerStatusPingInterval = 15_000;
#endif

    private static string? _lastHost;
    private static int? _lastPort;
    private static IPEndPoint? _lastEndpoint;

    private static bool TryResolveEndPoint([NotNullWhen(true)] out IPEndPoint? endPoint)
    {
        var currentHost = ClientConfiguration.Instance.Host;
        if (!string.Equals(_lastHost, currentHost))
        {
            _lastHost = currentHost;
            _lastEndpoint = default;
        }

        var currentPort = ClientConfiguration.Instance.Port;
        if (_lastPort != currentPort)
        {
            _lastPort = currentPort;
            _lastEndpoint = default;
        }

        if (string.IsNullOrWhiteSpace(_lastHost))
        {
            endPoint = default;
            return false;
        }

        if (_lastEndpoint != default)
        {
            endPoint = _lastEndpoint;
            return true;
        }

        var address = Dns.GetHostAddresses(_lastHost).FirstOrDefault();
        var port = _lastPort;
        if (address == default || !port.HasValue)
        {
            endPoint = default;
            return false;
        }

        endPoint = new IPEndPoint(address, port.Value);
        _lastEndpoint = endPoint;
        return true;
    }

    public static void Update()
    {
        if (Globals.LoginForm?.Visible ?? false)
        {
            var now = Timing.Global.MillisecondsUtc;
            // ReSharper disable once InvertIf
            if (Globals.NextServerStatusPing <= now)
            {
                if (TryResolveEndPoint(out var serverEndpoint))
                {
                    var network = EditorLidgrenNetwork;
                    if (network == default)
                    {
                        Intersect.Core.ApplicationContext.Context.Value?.Logger.LogInformation("No network created to poll for server status.");
                    }
                    else
                    {
                        network.SendUnconnected(serverEndpoint, new ServerStatusRequestPacket());
                        Globals.NextServerStatusPing = now + ServerStatusPingInterval;
                    }
                }
                else
                {
                    Intersect.Core.ApplicationContext.Context.Value?.Logger.LogInformation($"Unable to resolve '{_lastHost}:{_lastPort}'");
                }

                if (Globals.LoginForm.LastNetworkStatusChangeTime + ServerStatusPingInterval + ServerStatusPingInterval / 2 < now)
                {
                    Globals.LoginForm.SetNetworkStatus(NetworkStatus.Offline);
                }
            }
        }

        if (!Connected && !Connecting && !ConnectionDenied)
        {
            InitNetwork();
        }
    }

    public static void DestroyNetwork()
    {
        try
        {
            EditorLidgrenNetwork?.Close();
            EditorLidgrenNetwork = null;
            PacketHandler?.Registry?.Dispose();
            PacketHandler = null;
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public static void HandleDc(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs)
    {
        DestroyNetwork();
        if (Globals.MainForm != null && Globals.MainForm.Visible)
        {
            if (Globals.MainForm.DisconnectDelegate != null)
            {
                Globals.MainForm.BeginInvoke(Globals.MainForm.DisconnectDelegate);
                Globals.MainForm.DisconnectDelegate = null;
            }
        }
        else if (Globals.LoginForm.Visible)
        {
            Connecting = false;
            InitNetwork();
        }
        else
        {
            MessageBox.Show(@"Disconnected!");
            Application.Exit();
        }
    }

    public static void SendPacket(IntersectPacket packet)
    {
        if (EditorLidgrenNetwork != null)
        {
            if (!EditorLidgrenNetwork.Send(packet))
            {
                throw new Exception("Beta 4 network send failed.");
            }
        }
    }

}