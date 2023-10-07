using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

using Intersect.Configuration;
using Intersect.Editor.General;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Events;
using Intersect.Core;
using System.Collections.Generic;
using Intersect.Threading;
using Intersect.Plugins.Helpers;
using Intersect.Plugins.Interfaces;
using Intersect.Rsa;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Intersect.Utilities;
using Intersect.Network.Packets.Unconnected.Client;

namespace Intersect.Editor.Networking
{

    internal sealed class VirtualApplicationContext : IApplicationContext
    {
        public VirtualApplicationContext(IPacketHelper packetHelper)
        {
            PacketHelper = packetHelper;
        }

        public bool HasErrors => throw new NotImplementedException();

        public bool IsDisposed => throw new NotImplementedException();

        public bool IsStarted => throw new NotImplementedException();

        public bool IsRunning => throw new NotImplementedException();

        public ICommandLineOptions StartupOptions => throw new NotImplementedException();

        public Logger Logger => throw new NotImplementedException();

        public IPacketHelper PacketHelper { get; }

        public List<IApplicationService> Services => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public TApplicationService GetService<TApplicationService>() where TApplicationService : IApplicationService
        {
            throw new NotImplementedException();
        }

        public void Start(bool lockUntilShutdown = true)
        {
            throw new NotImplementedException();
        }

        public LockingActionQueue StartWithActionQueue()
        {
            throw new NotImplementedException();
        }
    }

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

            Log.Warn("Failed to connect to server.");
        }

        public static void InitNetwork()
        {
            if (EditorLidgrenNetwork == null)
            {
                var logger = Log.Default;
                var packetTypeRegistry = new PacketTypeRegistry(logger);
                if (!packetTypeRegistry.TryRegisterBuiltIn())
                {
                    throw new Exception("Failed to register built-in packets.");
                }

                var packetHandlerRegistry = new PacketHandlerRegistry(packetTypeRegistry, logger);
                packetHandlerRegistry.TryRegisterAvailableTypeHandlers(typeof(Network).Assembly, requireAttribute: true);
                var packetHelper = new PacketHelper(packetTypeRegistry, packetHandlerRegistry);
                PackedIntersectPacket.AddKnownTypes(packetHelper.AvailablePacketTypes);
                var virtualEditorContext = new VirtualEditorContext(packetHelper, logger);
                PacketHandler = new PacketHandler(virtualEditorContext, packetHandlerRegistry);

                var config = new NetworkConfiguration(
                    ClientConfiguration.Instance.Host, ClientConfiguration.Instance.Port
                );

                var virtualApplicationContext = new VirtualApplicationContext(packetHelper);
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("Intersect.Editor.network.handshake.bkey.pub"))
                {
                    var rsaKey = new RsaKey(stream);
                    Debug.Assert(rsaKey != null, "rsaKey != null");
                    EditorLidgrenNetwork = new ClientNetwork(virtualApplicationContext, config, rsaKey.Parameters);
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
        private const long ServerStatusPingInterval = 1_000;
#else
        private const long ServerStatusPingInterval = 15_000;
#endif

        private static long _nextServerStatusPing;

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
                if (_nextServerStatusPing <= now)
                {
                    if (TryResolveEndPoint(out var serverEndpoint))
                    {
                        var network = EditorLidgrenNetwork;
                        if (network == default)
                        {
                            Log.Info("No network created to poll for server status.");
                        }
                        else
                        {
                            network.SendUnconnected(serverEndpoint, new ServerStatusRequestPacket());
                        }
                    }
                    else
                    {
                        Log.Info($"Unable to resolve '{_lastHost}:{_lastPort}'");
                    }

                    if (Globals.LoginForm.LastNetworkStatusChangeTime + ServerStatusPingInterval + ServerStatusPingInterval / 2 < now)
                    {
                        Globals.LoginForm.SetNetworkStatus(NetworkStatus.Offline);
                    }

                    _nextServerStatusPing = now + ServerStatusPingInterval;
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

    internal sealed partial class VirtualEditorContext : IApplicationContext
    {
        internal VirtualEditorContext(PacketHelper packetHelper, Logger logger)
        {
            PacketHelper = packetHelper;
            Logger = logger;
        }

        public bool HasErrors => Network.ConnectionDenied;

        public bool IsDisposed { get; private set; }

        public bool IsStarted => IsRunning || Network.Connecting;

        public bool IsRunning => Network.Connected;

        public ICommandLineOptions StartupOptions => default;

        public Logger Logger { get; }

        public List<IApplicationService> Services { get; } = new List<IApplicationService>();

        public IPacketHelper PacketHelper { get; }

        public void Dispose() => IsDisposed = true;

        public TApplicationService GetService<TApplicationService>() where TApplicationService : IApplicationService => default;

        public void Start(bool lockUntilShutdown = true) { }

        public LockingActionQueue StartWithActionQueue() => default;
    }
}
