using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Options;

namespace Intersect.OpenPortChecker;

public sealed class PortChecker : INetEventListener, INetLogger
{
    private static readonly byte[] CheckPortRequestData;

    private readonly ILogger<PortChecker> _logger;
    private readonly NetManager _manager;
    private readonly IOptions<PortCheckerOptions> _options;
    private readonly PriorityQueue<Guid, DateTime> _pendingRequestExpiries = new();
    private readonly ConcurrentDictionary<Guid, PendingRequest> _pendingRequests = new();

    static PortChecker()
    {
        NetDataWriter writer = new();
        writer.Put(default(byte));
        writer.Put("open_port_check");
        CheckPortRequestData = writer.CopyData();
    }

    public PortChecker(ILoggerFactory loggerProvider, IOptions<PortCheckerOptions> options, IHostApplicationLifetime applicationLifetime)
    {
        applicationLifetime.ApplicationStopping.Register(() => PruneTasks(onlyExpired: false));
        _logger = loggerProvider.CreateLogger<PortChecker>();
        _options = options;
        _manager = new NetManager(this)
        {
            AllowPeerAddressChange = true,
            AutoRecycle = true,
            EnableStatistics = true,
#if DEBUG
            DisconnectTimeout = 600_000,
#else
            DisconnectTimeout = 60_000,
#endif
            IPv6Enabled = _options.Value.EnableIPv6,
            PingInterval = 5000,
            UnconnectedMessagesEnabled = true,
            UnsyncedEvents = true,
            UnsyncedDeliveryEvent = true,
            UnsyncedReceiveEvent = true,
            UseSafeMtu = true,
        };
        _manager.Start();

        NetDebug.Logger = this;
    }

    public void OnPeerConnected(NetPeer peer)
    {
        _logger.LogDebug("Peer connected {EndPoint}", peer.EndPoint);
        peer.Disconnect();
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        _logger.LogDebug(
            "Peer disconnected {EndPoint} - {DisconnectInfo} ({SocketErrorCode})",
            peer.EndPoint,
            disconnectInfo.Reason,
            disconnectInfo.SocketErrorCode
        );
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        _logger.LogError("Network error {EndPoint} - {SocketError}", endPoint, socketError);
    }

    public void OnNetworkReceive(
        NetPeer peer,
        NetPacketReader reader,
        byte channelNumber,
        DeliveryMethod deliveryMethod
    )
    {
        // Do nothing
    }

    public void OnNetworkReceiveUnconnected(
        IPEndPoint remoteEndPoint,
        NetPacketReader reader,
        UnconnectedMessageType messageType
    )
    {
        if (reader.TryGetString(out var secret))
        {
            var guidData = reader.GetRemainingBytes();
            if (guidData?.Length == 16)
            {
                Guid requestId = new(guidData);
                if (_pendingRequests.TryRemove(requestId, out var pendingRequest))
                {
                    _logger.LogDebug("Responding to {EndPoint} with secret", remoteEndPoint);
                    pendingRequest.TaskCompletionSource.SetResult(secret);
                }
                else
                {
                    _logger.LogDebug("Failed to remove {RequestId} from the list of pending requests", requestId);
                }
            }
            else
            {
                var invalidGuidDataLength = guidData?.Length ?? -1;
                _logger.LogDebug(
                    "Received invalid Guid from {EndPoint} ({InvalidGuidData}, {InvalidGuidDataLength})",
                    remoteEndPoint,
                    invalidGuidDataLength < 1 ? "<null>" : Convert.ToHexString(guidData!),
                    invalidGuidDataLength
                );
            }
        }
        else
        {
            _logger.LogDebug(
                "Received bad packet from {EndPoint} ({PacketLength}, {MessageType})",
                remoteEndPoint,
                reader.RawDataSize,
                messageType
            );
        }

        PruneTasks();
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        // Do nothing
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        _logger.LogDebug("Received connection request from {EndPoint}", request.RemoteEndPoint);
        request.Reject();
    }

    public void PruneTasks(bool onlyExpired = true)
    {
        if (onlyExpired == false)
        {
            while (!_pendingRequests.IsEmpty)
            {
                if (!_pendingRequests.TryRemove(_pendingRequests.Keys.First(), out var pendingRequest))
                {
                    continue;
                }

                pendingRequest.TaskCompletionSource.SetCanceled();
            }

            _pendingRequestExpiries.Clear();
        }

        var now = DateTime.UtcNow;
        while (_pendingRequestExpiries.TryPeek(out var _, out var pendingRequestExpiry))
        {
            if (pendingRequestExpiry > now)
            {
                break;
            }

            if (!_pendingRequestExpiries.TryDequeue(out var id, out pendingRequestExpiry))
            {
                break;
            }

            if (pendingRequestExpiry > now)
            {
                _pendingRequestExpiries.Enqueue(id, pendingRequestExpiry);
            }
            else if (_pendingRequests.TryRemove(id, out var pendingRequest))
            {
                pendingRequest.TaskCompletionSource.SetResult(null);
            }
        }
    }

    public async Task<string?> CheckPort(IPEndPoint endPoint)
    {
        _logger.LogDebug("Checking port: {EndPoint}", endPoint);

        if (_pendingRequests.Count > 100)
        {
            PruneTasks();
        }

        NetDataWriter writer = new(false, CheckPortRequestData.Length + Guid.Empty.ToByteArray().Length);
        var bufferSeconds = _pendingRequests.Count < 10 ? 9 : Math.Max(0, 10 - Math.Log10(_pendingRequests.Count));
        if (double.IsNaN(bufferSeconds) || !double.IsFinite(bufferSeconds))
        {
            bufferSeconds = 9;
        }

        var expiry = DateTime.UtcNow.AddSeconds(1 + bufferSeconds);
        PendingRequest pendingRequest = new(Guid.NewGuid(), new TaskCompletionSource<string?>());

        writer.Put(CheckPortRequestData);
        writer.Put(pendingRequest.Id.ToByteArray());

        if (!_pendingRequests.TryAdd(pendingRequest.Id, pendingRequest))
        {
            _logger.LogError(
                "Failed to add pending request for {EndPoint} ({PendingRequests} requests currently pending)",
                endPoint,
                _pendingRequests.Count
            );
            return default;
        }

        _pendingRequestExpiries.Enqueue(pendingRequest.Id, expiry);
        _manager.SendUnconnectedMessage(writer, endPoint);
        _logger.LogDebug("Waiting for response from {EndPoint} ({Id})", endPoint, pendingRequest.Id);
        var secret = await pendingRequest.TaskCompletionSource.Task;
        _logger.LogDebug("Received secret from {EndPoint} ({Id})", endPoint, pendingRequest.Id);
        return secret;
    }

    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        LogLevel logLevel = level switch
        {
            NetLogLevel.Warning => LogLevel.Warning,
            NetLogLevel.Error => LogLevel.Error,
            NetLogLevel.Trace => LogLevel.Trace,
            NetLogLevel.Info => LogLevel.Information,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
        };
        _logger.Log(logLevel, str, args);
    }
}