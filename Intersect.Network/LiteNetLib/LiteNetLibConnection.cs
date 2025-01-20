using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Intersect.Core;
using Intersect.Framework.Reflection;
using Intersect.Memory;
using Intersect.Network.Packets;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Logging;

namespace Intersect.Network.LiteNetLib;

public sealed class LiteNetLibConnection : AbstractConnection
{
    private readonly RSA _asymmetric;
    private readonly NetPeer _peer;
    private readonly ISymmetricAlgorithm _symmetric;

    internal LiteNetLibConnection(
        NetPeer peer,
        RSAParameters asymmetricParameters,
        byte symmetricVersion,
        ReadOnlySpan<byte> symmetricKey
    )
    {
        if (symmetricKey == default || symmetricKey.Length != 32)
        {
            throw new ArgumentException(
                $"Expected a 256-bit key but was {symmetricKey.Length << 3} bits.",
                nameof(symmetricKey)
            );
        }

        _asymmetric = RSA.Create(asymmetricParameters);
        _peer = peer;
        _peer.Tag = Guid;
        _symmetric = SymmetricAlgorithm.PickForVersion(symmetricVersion, symmetricKey);
        ApplicationContext.Context.Value?.Logger.LogDebug($"Created {_symmetric.GetFullishName()} for {_peer} ({Guid})");
    }

    internal LiteNetLibConnection(INetwork network, NetManager manager, RSA interfaceAsymmetric)
    {
        _asymmetric = RSA.Create(2048);
        var handshakeSecret = RandomNumberGenerator.GetBytes(32);
        var hailParameters = _asymmetric.ExportParameters(false);
        var hail = new HailPacket(interfaceAsymmetric, handshakeSecret, SharedConstants.VersionData, hailParameters);
        hail.Encrypt();

        var connectionData = NetDataWriter.FromBytes(hail.Data, false);

        ApplicationContext.Context.Value?.Logger.LogInformation($"Connecting to {network.Configuration.Host}:{network.Configuration.Port}...");

#if DIAGNOSTIC
        if (Debugger.IsAttached)
        {
            ApplicationContext.Context.Value?.Logger.LogInformation($"Connection packet data: {Convert.ToHexString(hail.Data)}");
            ApplicationContext.Context.Value?.Logger.LogInformation($"Connection packet data: {Convert.ToHexString(connectionData.Data)}");
        }
#endif

        _peer = manager.Connect(network.Configuration.Host, network.Configuration.Port, connectionData);
        _peer.Tag = Guid;
        _symmetric = SymmetricAlgorithm.PickForPlatform();
        ApplicationContext.Context.Value?.Logger.LogDebug($"Created {_symmetric.GetFullishName()} for {_peer} ({Guid})");
    }

    public override string Ip => _peer.Address.ToString();

    public override int Port => _peer.Port;

    internal bool TryProcessApproval(
        NetPeer peer,
        ApprovalPacket approvalPacket
    )
    {
        if (approvalPacket.Decrypt(_asymmetric))
        {
            return _symmetric.SetKey(approvalPacket.AesKey);
        }

        ApplicationContext.Context.Value?.Logger.LogError("Failed to decrypt approval response packet.");
        return false;
    }

    internal bool TryProcessInboundMessage(
        NetPeer peer,
        NetPacketReader reader,
        byte channelNumber,
        DeliveryMethod deliveryMethod,
        [NotNullWhen(true)] out IBuffer? buffer
    )
    {
        buffer = default;

        var cipherdata = reader.GetRemainingBytes();
        if (cipherdata == default)
        {
            return false;
        }

#if DIAGNOSTIC
        ApplicationContext.Context.Value?.Logger.LogDebug($"TryProcessInboundMessage() cipherdata({cipherdata.Length})={Convert.ToHexString(cipherdata)}");
#endif

        var decryptionResult = _symmetric.TryDecrypt(cipherdata, out var plaindata);
        switch (decryptionResult)
        {
            case EncryptionResult.Success:
                break;
            case EncryptionResult.NoHeader:
            case EncryptionResult.InvalidVersion:
            case EncryptionResult.InvalidNonce:
            case EncryptionResult.InvalidTag:
            case EncryptionResult.EmptyInput:
            case EncryptionResult.SizeMismatch:
            case EncryptionResult.Error:
                ApplicationContext.Context.Value?.Logger.LogWarning($"RIEP: {Guid} {decryptionResult}");
                return false;
            default:
                throw new UnreachableException();
        }

        buffer = new MemoryBuffer(plaindata.ToArray());
        return true;
    }

    public override bool Send(IPacket packet, TransmissionMode transmissionMode = TransmissionMode.All)
    {
        var packetData = packet.Data;
        var encryptionResult = _symmetric.TryEncrypt(packetData, out var cipherdata);
        switch (encryptionResult)
        {
            case EncryptionResult.Success:
                break;

            case EncryptionResult.NoHeader:
            case EncryptionResult.InvalidVersion:
            case EncryptionResult.InvalidNonce:
            case EncryptionResult.InvalidTag:
            case EncryptionResult.EmptyInput:
            case EncryptionResult.SizeMismatch:
            case EncryptionResult.Error:
                ApplicationContext.Context.Value?.Logger.LogWarning($"RIEP: {Guid} {encryptionResult}");
                return false;

            default:
                throw new UnreachableException();
        }

#if DIAGNOSTIC
        ApplicationContext.Context.Value?.Logger.LogDebug($"Send({transmissionMode}) cipherdata({cipherdata.Length})={Convert.ToHexString(cipherdata)}");
#endif
        NetDataWriter data = new(false, cipherdata.Length + sizeof(byte));
        data.Put((byte)1);
        data.Put(cipherdata.ToArray());
        return Send(data, transmissionMode);
    }

    public bool Send(NetDataWriter data, TransmissionMode transmissionMode = TransmissionMode.All)
    {
        try
        {
            _peer.Send(data, transmissionMode.ToDeliveryMethod());
            return true;
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogDebug(exception, "Failed to send data to {Endpoint}", _peer);
            return false;
        }
    }

    public override void Disconnect(string? message = default)
    {
        NetDataWriter writer = new();
        writer.Put(string.IsNullOrEmpty(message) ? string.Empty : message);
        _peer.Disconnect(writer.CopyData());
    }

    public override void Dispose()
    {
        _peer.Disconnect();
        base.Dispose();
    }
}