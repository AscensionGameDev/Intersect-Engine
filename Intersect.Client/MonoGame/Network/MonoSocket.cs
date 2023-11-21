using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Resources;
using Intersect.Client.Framework.Network;
using Intersect.Configuration;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets;
using Intersect.Utilities;
using Intersect.Client.Core;
using Intersect.Client.General;
using Intersect.Client.Interface.Menu;
using Intersect.Client.Localization;
using Intersect.Network.Packets.Unconnected.Client;
using Intersect.Rsa;

namespace Intersect.Client.MonoGame.Network
{
    internal partial class MonoSocket : GameSocket
    {
        internal static NetworkFactory? NetworkFactory { get; set; }

        private static readonly NetworkFactory DefaultNetworkFactory =
            (context, parameters, handlePacket, _) =>
            {
                var config = new NetworkConfiguration(
                    ClientConfiguration.Instance.Host,
                    ClientConfiguration.Instance.Port
                );
                return new ClientNetwork(context, config, parameters)
                {
                    Handler = handlePacket,
                };
            };

        /// <summary>
        /// Interval between status pings in ms (e.g. full, bad version, etc.)
        /// </summary>
#if DEBUG
        private const long ServerStatusPingInterval = 1_000;
#else
        private const long ServerStatusPingInterval = 15_000;
#endif

        private const string AsymmetricKeyManifestResourceName = "Intersect.Client.network.handshake.bkey.pub";

        private IClient? _network;

        public override IClient Network
        {
            get
            {
                if (_network != default)
                {
                    return _network;
                }

                using var asymmetricKeyStream =
                    typeof(MonoSocket).Assembly.GetManifestResourceStream(AsymmetricKeyManifestResourceName);
                if (asymmetricKeyStream == default)
                {
                    throw new MissingManifestResourceException(
                        $"Unable to get manifest resource stream for '{AsymmetricKeyManifestResourceName}'"
                    );
                }

                var rsaKey = new RsaKey(asymmetricKeyStream);
                var network = (NetworkFactory ?? DefaultNetworkFactory).Invoke(
                    Context,
                    rsaKey.Parameters,
                    AddPacketToQueue,
                    default
                );

                network.OnConnected += OnConnected;
                network.OnDisconnected += OnDisconnected;
                network.OnConnectionDenied += (sender, connectionEventArgs) => OnConnectionFailed(sender, connectionEventArgs, true);
                _network = network as IClient;
                Debug.Assert(_network != default);
                return _network;
            }
        }

        private static readonly ConcurrentQueue<KeyValuePair<IConnection, IPacket>> PacketQueue = new();

        private IClientContext Context { get; }

        private long _nextServerStatusPing;

        private string? _lastHost;
        private int? _lastPort;
        private IPEndPoint? _lastEndpoint;
        private volatile bool _resolvingHost;

        private static readonly HashSet<string> UnresolvableHostNames = new();

        public static MonoSocket Instance { get; private set; } = default!;

        internal MonoSocket(IClientContext context)
        {
            Context = context;
            Instance = this;
        }

        public override bool IsConnected => Network.IsConnected;

        public override int Ping => Network.Ping;

        private bool TryResolveEndPoint([NotNullWhen(true)] out IPEndPoint? endPoint)
        {
            var currentHost = ClientConfiguration.Instance.Host;
            if (UnresolvableHostNames.Contains(currentHost))
            {
                endPoint = default;
                return false;
            }

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

            try
            {
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
            catch (SocketException socketException)
            {
                if (socketException.SocketErrorCode != SocketError.HostNotFound)
                {
                    throw;
                }

                UnresolvableHostNames.Add(_lastHost);
                Interface.Interface.ShowError(Strings.Errors.HostNotFound);
                Log.Error(socketException, $"Failed to resolve host: '{_lastHost}'");
                endPoint = default;
                return false;
            }
        }

        public override void Connect(string host, int port)
        {
            if (!Network.Connect())
            {
                Log.Error("An error occurred while attempting to connect.");
            }
        }

        public override void SendPacket(object packet)
        {
            if (packet is not IntersectPacket intersectPacket)
            {
                return;
            }

            _network?.Send(intersectPacket);
        }

        public static bool AddPacketToQueue(IConnection connection, IPacket packet)
        {
            if (packet is AbstractTimedPacket timedPacket)
            {
                Timing.Global.Synchronize(timedPacket.UTC, timedPacket.Offset);
            }

            PacketQueue.Enqueue(new KeyValuePair<IConnection, IPacket>(connection, packet));

            return true;
        }

        public override void Update()
        {
            while (PacketQueue.TryDequeue(out KeyValuePair<IConnection, IPacket> dequeued))
            {
                OnDataReceived(dequeued.Value);
            }

            // ReSharper disable once InvertIf
            if (Globals.GameState == GameStates.Menu)
            {
                var now = Timing.Global.MillisecondsUtc;
                // ReSharper disable once InvertIf
                if (_nextServerStatusPing <= now || MainMenu.LastNetworkStatusChangeTime < 0)
                {
                    if (!_resolvingHost)
                    {
                        _resolvingHost = true;
                        Task.Run(
                            () =>
                            {
                                try
                                {
                                    if (TryResolveEndPoint(out var serverEndpoint))
                                    {
                                        var network = Network;
                                        if (network == default)
                                        {
                                            Log.Info("No network created to poll for server status.");
                                        }
                                        else
                                        {
                                            network.SendUnconnected(serverEndpoint, new ServerStatusRequestPacket());
                                        }
                                    }
                                    else if (!UnresolvableHostNames.Contains(_lastHost))
                                    {
                                        Log.Info($"Unable to resolve '{_lastHost}:{_lastPort}'");
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Log.Error(exception);
                                }

                                _resolvingHost = false;
                            }
                        );
                    }

                    if (MainMenu.LastNetworkStatusChangeTime + ServerStatusPingInterval < now)
                    {
                        MainMenu.SetNetworkStatus(NetworkStatus.Offline);
                    }

                    _nextServerStatusPing = now + ServerStatusPingInterval;
                }
            }
        }

        public override void Disconnect(string reason)
        {
            Network.Disconnect(reason);
        }

        public override void Dispose()
        {
            _network?.Close();
            _network?.Dispose();
            _network = default;
        }
    }
}
