using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Intersect.Logging;
using Intersect.Memory;
using Intersect.Network.Events;
using Intersect.Network.Packets;
using Intersect.Reflection;
using Intersect.Utilities;
using LiteNetLib;
using LiteNetLib.Utils;
using NetPeer = LiteNetLib.NetPeer;

namespace Intersect.Network.LiteNetLib;

public sealed class LiteNetLibInterface : INetworkLayerInterface, INetEventListener
{
    private sealed record InboundBuffer(IConnection Connection, IBuffer Buffer);

    private readonly ConcurrentDictionary<int, Guid> _connectionIdLookup = new();
    private readonly NetManager _manager;
    private readonly INetwork _network;
    private readonly RSA _asymmetricServer;
    private readonly RSA _asymmetricUnconnected;
    private readonly ConcurrentQueue<InboundBuffer> _pendingInboundBuffers = new();

    static LiteNetLibInterface()
    {
        NetDebug.Logger = new LiteNetLibLogger();
    }

    public LiteNetLibInterface(INetwork network, RSAParameters asymmetricParameters)
    {
        _network = network;

        _manager = new NetManager(this)
        {
            AllowPeerAddressChange = network.Configuration.IsServer,
            AutoRecycle = true,
            EnableStatistics = true,
#if DEBUG
            DisconnectTimeout = 600_000,
#else
            DisconnectTimeout = 60_000,
#endif
            IPv6Enabled = true,
            PingInterval = 5000,
            UnconnectedMessagesEnabled = true,
            UnsyncedEvents = true,
            UnsyncedDeliveryEvent = true,
            UnsyncedReceiveEvent = true,
            UseSafeMtu = true,
        };

        _asymmetricServer = RSA.Create(asymmetricParameters);
        _asymmetricUnconnected = RSA.Create(2048);
    }

    public void Dispose()
    {

    }

    public event HandleConnectionEvent? OnConnected;
    public event HandleConnectionEvent? OnConnectionApproved;
    public event HandleConnectionEvent? OnConnectionDenied;
    public event HandleConnectionRequest? OnConnectionRequested;
    public event HandleConnectionEvent? OnDisconnected;
    public event HandlePacketAvailable? OnPacketAvailable;
    public event HandleUnconnectedMessage? OnUnconnectedMessage;

    public bool TryGetInboundBuffer(out IBuffer? buffer, out IConnection? connection)
    {
        if (!_pendingInboundBuffers.TryDequeue(out var inboundBuffer))
        {
            buffer = default;
            connection = default;
            return false;
        }

        buffer = inboundBuffer.Buffer;
        connection = inboundBuffer.Connection;
        return true;
    }

    public void ReleaseInboundBuffer(IBuffer buffer) => buffer.Dispose();

    public bool SendPacket(
        IPacket packet,
        IConnection? connection = null,
        TransmissionMode transmissionMode = TransmissionMode.All
    )
    {
        connection ??= _network.Connections.FirstOrDefault();
        if (connection != default)
        {
            return connection.Send(packet, transmissionMode);
        }

        Log.Debug("No active connections.");
        return false;
    }

    public bool SendPacket(IPacket packet, ICollection<IConnection> connections, TransmissionMode transmissionMode = TransmissionMode.All)
    {
        var data = NetDataWriter.FromBytes(packet.Data, false);
        foreach (var connection in connections)
        {
            if (connection is LiteNetLibConnection liteNetLibConnection)
            {
                if (!liteNetLibConnection.Send(data, transmissionMode))
                {
                    return false;
                }
            }
            else if (!connection.Send(packet, transmissionMode))
            {
                return false;
            }
        }

        return true;
    }

    public void Start()
    {
        if (!_network.Configuration.IsServer)
        {
            if (!_manager.Start())
            {
                Log.Error("Failed to make the initial connection attempt.");
            }
        } else if (!_manager.Start(_network.Configuration.Port))
        {
            throw new Exception($"Failed to listen on port {_network.Configuration.Port}");
        }
    }

    public void Stop(string reason = "stopping")
    {
        Log.Verbose($"Stopping {nameof(LiteNetLibInterface)} (\"{reason}\")...");
        var reasonData = Encoding.UTF8.GetBytes(reason);
        _manager.DisconnectAll(reasonData, 0, reasonData.Length);
    }

    public bool Connect()
    {
        if (_network.Configuration.IsServer)
        {
            throw new NotSupportedException();
        }

        if (!_manager.IsRunning && !_manager.Start())
        {
            return false;
        }

        LiteNetLibConnection connection = new(_network, _manager, _asymmetricServer);

        if (_network.AddConnection(connection))
        {
            return true;
        }

        Log.Error("Failed to add connection to list.");
        connection.Dispose();

        return false;
    }

    public void Disconnect(IConnection connection, string? message) => Disconnect(new[] { connection }, message);

    public void Disconnect(ICollection<IConnection> connections, string? message)
    {
        foreach (var connection in connections)
        {
            connection.Disconnect(message);
            _network.RemoveConnection(connection);
        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Log.Debug($"CONN {peer.EndPoint}");
        if (peer.Tag is not Guid guid)
        {
            Log.Diagnostic("Peer tag is not a guid");
            return;
        }

        if (!_connectionIdLookup.TryAdd(peer.Id, guid))
        {
            Log.Diagnostic($"Failed to add connection ID {guid} for peer {peer.Id}");
            return;
        }

        OnConnected?.Invoke(this, new ConnectionEventArgs
        {
            Connection = _network.FindConnection(guid),
            NetworkStatus = NetworkStatus.Online,
        });
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        if (disconnectInfo.Reason == DisconnectReason.Reconnect)
        {
            Log.Debug($"RECONNECT: {peer.EndPoint}");
            return;
        }

        if (disconnectInfo.Reason != DisconnectReason.DisconnectPeerCalled)
        {
            try
            {
                var message = (disconnectInfo.AdditionalData.EndOfData
                    ? string.Empty
                    : disconnectInfo.AdditionalData.GetString());
                Log.Debug(
                    $"DCON: {peer.EndPoint} \"{disconnectInfo.Reason}\" ({disconnectInfo.SocketErrorCode}): {message}"
                );
            }
            catch (Exception exception)
            {
                Log.Debug($"DCON[ERR]: {peer.EndPoint} \"{disconnectInfo.Reason}\" ({disconnectInfo.SocketErrorCode}): {exception.Message}");
            }
        }

        if (!_connectionIdLookup.TryRemove(peer.Id, out var connectionId))
        {
            Log.Diagnostic($"No connection found for peer ID {peer.Id} ({_network.Connections.Count})");
            return;
        }

        if (_network.FindConnection(connectionId) is not LiteNetLibConnection connection)
        {
            Log.Diagnostic($"No connection found for {connectionId} ({_network.Connections.Count})");
            return;
        }

        if (!_network.RemoveConnection(connection))
        {
            Log.Warn(
                $"Failed to remove connection {connection.Guid}, was this already removed? ({_network.Connections.Count})"
            );
            return;
        }

        OnDisconnected?.Invoke(
            this,
            new ConnectionEventArgs
            {
                Connection = connection,
                NetworkStatus = NetworkStatus.Offline,
            }
        );
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Log.Error($"NERR: {socketError} from {endPoint}");
    }

    public void OnNetworkReceive(
        NetPeer peer,
        NetPacketReader reader,
        byte channelNumber,
        DeliveryMethod deliveryMethod
    )
    {
        if (!_connectionIdLookup.TryGetValue(peer.Id, out var connectionId))
        {
            Log.Warn($"RNIP: {peer.EndPoint} ({channelNumber}, {deliveryMethod})");
            return;
        }

        var genericConnection = _network.FindConnection(connectionId);
        if (genericConnection is not LiteNetLibConnection connection)
        {
            Log.Warn(
                $"RNCP: {peer.EndPoint} ({channelNumber}, {deliveryMethod}) ({genericConnection?.Guid} / {genericConnection?.GetFullishName()}"
            );
            return;
        }

        if (!reader.TryGetByte(out var packetCode))
        {
            Log.Warn($"No packet code from {peer.EndPoint} / {connection.Guid}");
            return;
        }

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (packetCode > 1)
        {
            Log.Warn($"Invalid packet code from {peer.EndPoint} / {connection.Guid}");
            return;
        }

        if (packetCode == 0)
        {
            var approvalPacket = new ApprovalPacket
            {
                EncryptedData = reader.GetRemainingBytes(),
            };

            if (connection.TryProcessApproval(peer, approvalPacket))
            {
                OnConnectionApproved?.Invoke(this, new ConnectionEventArgs
                {
                    Connection = connection,
                    NetworkStatus = NetworkStatus.Online,
                });
            }
            else
            {
                Log.Warn($"Failed to process approval from {peer.EndPoint} / {connection.Guid}");
            }

            return;
        }

        if (!connection.TryProcessInboundMessage(peer, reader, channelNumber, deliveryMethod, out var buffer))
        {
            Log.Warn($"FPIM: {peer.EndPoint} / {connection.Guid}");
            return;
        }

        _pendingInboundBuffers.Enqueue(new InboundBuffer(connection, buffer));
        OnPacketAvailable?.Invoke(this);
    }

    public bool SendUnconnectedPacket(IPEndPoint target, ReadOnlySpan<byte> data) =>
        _manager.SendUnconnectedMessage(data.ToArray(), target);

    public bool SendUnconnectedPacket(IPEndPoint target, UnconnectedPacket packet)
    {
        using var tempAsymmetric = RSA.Create(_asymmetricServer.ExportParameters(false));
        switch (packet)
        {
            case UnconnectedResponsePacket unconnectedResponsePacket:
                tempAsymmetric.ImportRSAPublicKey(unconnectedResponsePacket.ResponseKey, out _);
                break;
            case UnconnectedRequestPacket unconnectedRequestPacket when unconnectedRequestPacket.ResponseKey.Length == 0:
                unconnectedRequestPacket.ResponseKey = _asymmetricUnconnected.ExportRSAPublicKey();
                break;
        }

        var encryptedPacket = new AsymmetricEncryptedPacket(
            packet,
            tempAsymmetric.ExportParameters(false)
        );
        var encryptedPacketData = encryptedPacket.EncryptedData;
        if (encryptedPacketData == default)
        {
            Log.Debug($"Failed to encrypt data for {packet.GetFullishName()}");
            return false;
        }

#if DIAGNOSTIC
        Log.Debug($"{nameof(SendUnconnectedPacket)} {nameof(encryptedPacketData)}({encryptedPacketData.Length})={Convert.ToHexString(encryptedPacketData)}");
#endif

        NetDataWriter data = new(false, encryptedPacketData.Length + 1);
        data.Put((byte)UnconnectedPacketType.AsymmetricEncrypted);
        data.Put(encryptedPacketData);
        return _manager.SendUnconnectedMessage(data, target);
    }

    private void HandleAsymmetricEncryptedUnconnectedPacket(
        IPEndPoint remoteEndPoint,
        NetDataReader reader,
        UnconnectedMessageType messageType
    )
    {
        try
        {
            var encryptedPacketData = reader.GetRemainingBytes();
            var unconnectedAsymmetric = _network.Configuration.IsServer ? _asymmetricServer : _asymmetricUnconnected;
            var encryptedPacket = new AsymmetricEncryptedPacket
            {
                EncryptedData = encryptedPacketData,
                Parameters = unconnectedAsymmetric.ExportParameters(true),
            };

            var innerPacket = encryptedPacket.InnerPacket;
            if (innerPacket == default)
            {
                Log.Debug("Inner packet is null.");
                return;
            }

            if (innerPacket is not UnconnectedPacket unconnectedPacket)
            {
                Log.Debug($"Inner packet is not of type {typeof(UnconnectedPacket)}: {innerPacket.GetFullishName()}");
                return;
            }

            if (!_network.Helper.HandlerRegistry.TryGetHandler(innerPacket, out var handlerInstance))
            {
                Log.Warn($"No handler for {innerPacket.GetFullishName()}");
                return;
            }

            var packetSender = new LiteNetLibUnconnectedPacketSender(this, remoteEndPoint, _network);
            if (!handlerInstance.Invoke(packetSender, unconnectedPacket))
            {
                Log.Warn($"Packet handler failed for {innerPacket.GetFullishName()}");
            }
        }
        catch (Exception exception)
        {
            Log.Debug(exception);
        }
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        try
        {
            if (!reader.TryGetByte(out var unconnectedPacketTypeAsByte))
            {
                return;
            }

            var unconnectedPacketType = (UnconnectedPacketType)unconnectedPacketTypeAsByte;
            if (!Enum.IsDefined(typeof(UnconnectedPacketType), unconnectedPacketTypeAsByte))
            {
                unconnectedPacketType = UnconnectedPacketType.AsymmetricEncrypted;
            }

            switch (unconnectedPacketType)
            {
                case UnconnectedPacketType.Plaintext:
                    OnUnconnectedMessage?.Invoke(
                        new UnconnectedMessageSender(this, remoteEndPoint, default),
                        new LiteNetLibInboundBuffer(reader)
                    );
                    break;
                case UnconnectedPacketType.AsymmetricEncrypted:
                    HandleAsymmetricEncryptedUnconnectedPacket(remoteEndPoint, reader, messageType);
                    break;
                default: throw new UnreachableException();
            }
        }
        catch (Exception exception)
        {
            Log.Debug(exception);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
#if !DIAGNOSTIC
        if (latency < 1)
        {
            return;
        }
#endif

        Log.Verbose($"LTNC {peer.EndPoint} {latency}ms");
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        NetDataWriter response = new();

        var inboundBuffer = new LiteNetLibInboundBuffer(request.Data);
        var deserializedBuffer = MessagePacker.Instance.Deserialize(inboundBuffer);
        if (deserializedBuffer is not HailPacket hail || !hail.Decrypt(_asymmetricServer))
        {
            Log.Debug($"Rejecting (bad hail) {request.RemoteEndPoint}");
            response.Put((ushort)400);
            request.Reject(response);
            return;
        }

        Debug.Assert(SharedConstants.VersionData != null, "SharedConstants.VERSION_DATA != null");
        Debug.Assert(hail.VersionData != null, "hail.VersionData != null");
        if (!SharedConstants.VersionData.SequenceEqual(hail.VersionData))
        {
            Log.Debug($"Rejecting (bad version) {request.RemoteEndPoint}");
            response.Put((ushort)400);
            response.Put(NetworkStatus.VersionMismatch.ToString());
            request.Reject(response);
            return;
        }

        Log.Debug($"hail Time={hail.Adjusted / TimeSpan.TicksPerMillisecond} Offset={hail.Offset / TimeSpan.TicksPerMillisecond} Real={hail.UTC / TimeSpan.TicksPerMillisecond}");
        Log.Debug($"local Time={Timing.Global.Milliseconds} Offset={(long)Timing.Global.MillisecondsOffset} Real={Timing.Global.MillisecondsUtc}");
        Log.Debug($"real delta={(Timing.Global.TicksUtc - hail.UTC) / TimeSpan.TicksPerMillisecond}");

        if (_network.ConnectionCount >= Options.Instance.MaxClientConnections)
        {
            Log.Debug($"Rejecting (full) {request.RemoteEndPoint}");
            response.Put((ushort)503);
            response.Put(NetworkStatus.ServerFull.ToString());
            request.Reject(response);
            return;
        }

        var peer = request.Accept();
        Log.Debug($"NCPing={peer.Ping}");

        var symmetricKey = RandomNumberGenerator.GetBytes(32);
        var connection = new LiteNetLibConnection(peer, hail.RsaParameters, hail.SymmetricVersion, symmetricKey);

        if (!(OnConnectionRequested?.Invoke(this, connection) ?? true))
        {
            Log.Debug($"Rejecting (ban) {peer.EndPoint} ({connection.Guid})");
            response.Put((ushort)403);
            response.Put(NetworkStatus.Failed.ToString());
            peer.Disconnect(response);
            return;
        }

        if (!_network.AddConnection(connection))
        {
            Log.Debug($"Rejecting (error connection map) {peer.EndPoint} ({connection.Guid})");
            response.Put((ushort)500);
            response.Put(NetworkStatus.Failed.ToString());
            peer.Disconnect(response);
            return;
        }

        if (!_connectionIdLookup.TryAdd(peer.Id, connection.Guid))
        {
            Log.Debug($"Rejecting (error connection id lookup) {peer.EndPoint} ({connection.Guid})");
            response.Put((ushort)500);
            response.Put(NetworkStatus.Failed.ToString());
            peer.Disconnect(response);
            return;
        }

        ApprovalPacket approval = new(hail.RsaParameters, hail.HandshakeSecret, symmetricKey, connection.Guid);
        approval.Encrypt();

        var approvalData = approval.EncryptedData;
        var connectionData = new NetDataWriter(false, approval.EncryptedData.Length + sizeof(byte));
        connectionData.Put((byte)0);
        connectionData.Put(approvalData);
#if DIAGNOSTIC
        Log.Debug($"OnConnectionRequest approvalData({approvalData.Length})={Convert.ToHexString(approvalData)}");
        Log.Debug($"OnConnectionRequest connectionData({connectionData.Length})={Convert.ToHexString(connectionData.Data)}");
#endif
        peer.Send(connectionData, DeliveryMethod.ReliableOrdered);

        OnConnectionApproved?.Invoke(this, new ConnectionEventArgs
        {
            Connection = connection,
            NetworkStatus = NetworkStatus.Online,
        });

        OnConnected?.Invoke(
            this,
            new ConnectionEventArgs
            {
                Connection = connection,
                NetworkStatus = NetworkStatus.Online,
            }
        );

        Log.Debug($"Approved {peer.EndPoint} ({connection.Guid})");
    }
}