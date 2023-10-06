using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;

using Intersect.Client.Framework.Network;
using Intersect.Configuration;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets;
using Intersect.Utilities;
using Intersect.Client.Core;
using Intersect.Client.General;
using Intersect.Client.Interface.Menu;
using Intersect.Network.Packets.Unconnected.Client;
using Intersect.Rsa;

namespace Intersect.Client.MonoGame.Network
{

    internal partial class MonoSocket : GameSocket
    {
        /// <summary>
        /// Interval between status pings in ms (e.g. full, bad version, etc.)
        /// </summary>
#if DEBUG
        private const long ServerStatusPingInterval = 1_000;
#else
        private const long ServerStatusPingInterval = 15_000;
#endif

        public static ClientNetwork? ClientNetwork { get; set; }

        private ClientNetwork? _network;

        public override INetwork GetNetwork() => Network!;

        public ClientNetwork? Network
        {
            get
            {
                if (_network != default)
                {
                    return _network;
                }

                ClientNetwork network;
                var config = new NetworkConfiguration(ClientConfiguration.Instance.Host, ClientConfiguration.Instance.Port);
                var assembly = Assembly.GetExecutingAssembly();

                using var stream = assembly.GetManifestResourceStream("Intersect.Client.network.handshake.bkey.pub");
                var rsaKey = new RsaKey(stream);
                Debug.Assert(rsaKey != null, "rsaKey != null");
                network = new ClientNetwork(Context, config, rsaKey.Parameters)
                {
                    Handler = AddPacketToQueue,
                };

                network.OnConnected += OnConnected;
                network.OnDisconnected += OnDisconnected;
                network.OnConnectionDenied += (sender, connectionEventArgs) => OnConnectionFailed(sender, connectionEventArgs, true);
                _network = network;
                ClientNetwork = _network;
                return _network;
            }
            set
            {
                _network = value;
                ClientNetwork = _network;
            }
        }

        public static ConcurrentQueue<KeyValuePair<IConnection, IPacket>> PacketQueue =
            new ConcurrentQueue<KeyValuePair<IConnection, IPacket>>();

        private IClientContext Context { get; }

        private long _nextServerStatusPing;

        private string? _lastHost;
        private int? _lastPort;
        private IPEndPoint? _lastEndpoint;

        internal MonoSocket(IClientContext context)
        {
            Context = context;
        }

        private bool TryResolveEndPoint([NotNullWhen(true)] out IPEndPoint? endPoint)
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

        public override void Connect(string host, int port)
        {
            var network = Network;
            if (network == default)
            {
                Log.Error("Failed to recreate the network.");
                return;
            }

            if (!network.Connect())
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
                if (_nextServerStatusPing <= now)
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
                    else
                    {
                        Log.Info($"Unable to resolve '{_lastHost}:{_lastPort}'");
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
            ClientNetwork?.Disconnect(reason);
        }

        public override void Dispose()
        {
            ClientNetwork?.Close();
            ClientNetwork?.Dispose();
            ClientNetwork = null;
        }

        public override bool IsConnected()
        {
            return ClientNetwork?.IsConnected ?? false;
        }

        public override int Ping
        {
            get
            {
                return ClientNetwork?.Ping ?? -1;
            }
        }
    }

}
