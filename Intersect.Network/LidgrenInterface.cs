using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Intersect.Logging;
using Intersect.Memory;
using Intersect.Network.Packets;
using Intersect.Utilities;
using JetBrains.Annotations;
using Lidgren.Network;

namespace Intersect.Network
{
    public sealed class LidgrenInterface : INetworkLayerInterface
    {
        public delegate void HandleUnconnectedMessage(NetPeer peer, NetIncomingMessage message);

        private static readonly IConnection[] EmptyConnections = { };

        [NotNull]
        private readonly IDictionary<long, Guid> mGuidLookup;

        [NotNull]
        private readonly INetwork mNetwork;

        [NotNull]
        private readonly NetPeer mPeer;

        [NotNull]
        private readonly NetPeerConfiguration mPeerConfiguration;

        [NotNull]
        private readonly RandomNumberGenerator mRng;
        
        [NotNull]
        private readonly RSACryptoServiceProvider mRsa;

        public LidgrenInterface(INetwork network, Type peerType, RSAParameters rsaParameters)
        {
            if (peerType == null) throw new ArgumentNullException(nameof(peerType));

            mNetwork = network ?? throw new ArgumentNullException(nameof(network));

            var configuration = mNetwork.Configuration;
            if (configuration == null) throw new ArgumentNullException(nameof(mNetwork.Configuration));

            mRng = new RNGCryptoServiceProvider();

            mRsa = new RSACryptoServiceProvider();
            mRsa.ImportParameters(rsaParameters);
            mPeerConfiguration = new NetPeerConfiguration(SharedConstants.VersionName)
            {
                AcceptIncomingConnections = configuration.IsServer
            };

            mPeerConfiguration.DisableMessageType(NetIncomingMessageType.Receipt);
            mPeerConfiguration.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            mPeerConfiguration.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            mPeerConfiguration.EnableMessageType(NetIncomingMessageType.Error);

            if (configuration.IsServer)
            {
                mPeerConfiguration.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
                mPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                mPeerConfiguration.AcceptIncomingConnections = true;
                mPeerConfiguration.MaximumConnections = configuration.MaximumConnections;
                //mPeerConfiguration.LocalAddress = DnsUtils.Resolve(config.Host);
                //mPeerConfiguration.EnableUPnP = true;
                mPeerConfiguration.Port = configuration.Port;
            }

            if (Debugger.IsAttached)
            {
                mPeerConfiguration.EnableMessageType(NetIncomingMessageType.DebugMessage);
            }
            else
            {
                mPeerConfiguration.DisableMessageType(NetIncomingMessageType.DebugMessage);
            }

            if (Debugger.IsAttached)
            {
                mPeerConfiguration.ConnectionTimeout = 60;
                mPeerConfiguration.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            }
            else
            {
                mPeerConfiguration.ConnectionTimeout = 15;
                mPeerConfiguration.DisableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            }

            mPeerConfiguration.PingInterval = 2.5f;
            mPeerConfiguration.UseMessageRecycling = true;

            var constructorInfo = peerType.GetConstructor(new[] {typeof(NetPeerConfiguration)});
            if (constructorInfo == null) throw new ArgumentNullException(nameof(constructorInfo));
            var constructedPeer = constructorInfo.Invoke(new object[] { mPeerConfiguration }) as NetPeer;
            mPeer = constructedPeer ?? throw new ArgumentNullException(nameof(constructedPeer));

            mGuidLookup = new Dictionary<long, Guid>();

            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            mPeer?.RegisterReceivedCallback(peer =>
            {
                lock (mPeer)
                {
                    if (OnPacketAvailable == null)
                    {
                        Log.Debug("Unhandled inbound Lidgren message.");
                        Log.Diagnostic($"Unhandled message: {TryHandleInboundMessage()}");
                        return;
                    }

                    OnPacketAvailable(this);
                }
            });
        }

        public HandleUnconnectedMessage OnUnconnectedMessage { get; set; }
        public HandleConnectionEvent OnConnectionApproved { get; set; }

        public HandlePacketAvailable OnPacketAvailable { get; set; }

        public HandleConnectionEvent OnConnected { get; set; }
        public HandleConnectionEvent OnDisconnected { get; set; }

        public void Start()
        {
            if (mNetwork.Configuration.IsServer)
            {
                Log.Info($"Listening on {mPeerConfiguration.LocalAddress}:{mPeerConfiguration.Port}.");
                mPeer.Start();
                return;
            }

            if (!Connect())
            {
                Log.Error("Failed to make the initial connection attempt.");
            }
        }

        public bool Connect()
        {
            if (mNetwork.Configuration.IsServer)
            {
                throw new InvalidOperationException("Server interfaces cannot use Connect().");
            }

            Log.Info($"Connecting to {mNetwork.Configuration.Host}:{mNetwork.Configuration.Port}...");

            var handshakeSecret = new byte[32];
            mRng.GetNonZeroBytes(handshakeSecret);

            var connectionRsa = new RSACryptoServiceProvider(2048);

            var hail = new HailPacket(mRsa, handshakeSecret, SharedConstants.VersionData,
                connectionRsa.ExportParameters(false));
            var hailMessage = mPeer.CreateMessage(hail.EstimatedSize);

            IBuffer buffer = new LidgrenBuffer(hailMessage);
            hail.Write(ref buffer);

            if (mPeer.Status == NetPeerStatus.NotRunning)
            {
                mPeer.Start();
            }

            var connection = mPeer.Connect(mNetwork.Configuration.Host, mNetwork.Configuration.Port, hailMessage);
            var server = new LidgrenConnection(mNetwork, Guid.Empty, connection, handshakeSecret,
                connectionRsa.ExportParameters(true));

            if (mNetwork.AddConnection(server))
                return true;

            Log.Error("Failed to add connection to list.");
            connection?.Disconnect("client_error");
            return false;
        }

        public bool TryGetInboundBuffer(out IBuffer buffer, out IConnection connection)
        {
            buffer = default(IBuffer);
            connection = default(IConnection);

            var message = TryHandleInboundMessage();
            if (message == null) return true;

            var lidgrenId = message.SenderConnection?.RemoteUniqueIdentifier ?? -1;
            Debug.Assert(mGuidLookup != null, "mGuidLookup != null");
            if (!mGuidLookup.TryGetValue(lidgrenId, out Guid guid))
            {
                Log.Error($"Missing connection: {guid}");
                mPeer.Recycle(message);
                return false;
            }

            connection = mNetwork.FindConnection(guid);

            if (connection != null)
            {
                var lidgrenConnection = connection as LidgrenConnection;
                if (lidgrenConnection?.Aes == null)
                {
                    Log.Error("No provider to decrypt data with.");
                    return false;
                }

                if (!lidgrenConnection.Aes.Decrypt(message))
                {
                    Log.Error($"Error decrypting inbound Lidgren message [Connection:{connection.Guid}].");
                    return false;
                }
            }
            else
            {
                Log.Warn($"Received message from an unregistered endpoint.");
            }

            buffer = new LidgrenBuffer(message);
            return true;
        }

        public void ReleaseInboundBuffer(IBuffer buffer)
        {
            var message = (buffer as LidgrenBuffer)?.Buffer as NetIncomingMessage;
            mPeer?.Recycle(message);
        }

        public bool SendPacket(IPacket packet, IConnection connection = null,
            TransmissionMode transmissionMode = TransmissionMode.All)
        {
            if (connection == null) return SendPacket(packet, EmptyConnections, transmissionMode);

            var lidgrenConnection = connection as LidgrenConnection;
            if (lidgrenConnection == null)
            {
                Log.Diagnostic("Tried to send to a non-Lidgren connection.");
                return false;
            }

            var deliveryMethod = TranslateTransmissionMode(transmissionMode);
            if (mPeer == null) throw new ArgumentNullException(nameof(mPeer));

            if (packet == null)
            {
                Log.Diagnostic("Tried to send a null packet.");
                return false;
            }

            var sequence = 0;
            if (deliveryMethod == NetDeliveryMethod.ReliableSequenced)
                sequence = (byte) packet.Code % 32;

            var message = mPeer.CreateMessage(packet.EstimatedSize);
            if (message == null) throw new ArgumentNullException(nameof(message));
            IBuffer buffer = new LidgrenBuffer(message);
            if (!packet.Write(ref buffer))
            {
                Log.Debug($"Error writing packet to outgoing message buffer ({packet.Code}).");
                return false;
            }

            SendMessage(message, lidgrenConnection, deliveryMethod);
            return true;
        }

        public bool SendPacket(IPacket packet, ICollection<IConnection> connections,
            TransmissionMode transmissionMode = TransmissionMode.All)
        {
            var deliveryMethod = TranslateTransmissionMode(transmissionMode);
            if (mPeer == null) throw new ArgumentNullException(nameof(mPeer));

            if (packet == null)
            {
                Log.Diagnostic("Tried to send a null packet.");
                return false;
            }

            var sequence = 0;
            if (deliveryMethod == NetDeliveryMethod.ReliableSequenced)
                sequence = (byte) packet.Code % 32;

            var message = mPeer.CreateMessage(packet.EstimatedSize);
            if (message == null) throw new ArgumentNullException(nameof(message));
            IBuffer buffer = new LidgrenBuffer(message);
            if (!packet.Write(ref buffer))
            {
                Log.Debug($"Error writing packet to outgoing message buffer ({packet.Code}).");
                return false;
            }

            if (connections == null || connections.Count(connection => connection != null) < 1)
            {
                connections = mNetwork?.FindConnections<IConnection>();
            }

            var lidgrenConnections = connections?.OfType<LidgrenConnection>().ToList();
            if (lidgrenConnections?.Count > 0)
            {
                var firstConnection = lidgrenConnections.First();

                lidgrenConnections.ForEach(lidgrenConnection =>
                {
                    if (lidgrenConnection == null) return;
                    if (firstConnection == lidgrenConnection) return;
                    if (message.Data == null) throw new ArgumentNullException(nameof(message.Data));
                    var messageClone = mPeer.CreateMessage(message.Data.Length);
                    if (messageClone == null) throw new ArgumentNullException(nameof(messageClone));
                    Buffer.BlockCopy(message.Data, 0, messageClone.Data, 0, message.Data.Length);
                    messageClone.LengthBytes = message.LengthBytes;
                    SendMessage(messageClone, lidgrenConnection, deliveryMethod);
                });

                SendMessage(message, lidgrenConnections.First(), deliveryMethod);
            }
            else
            {
                Log.Diagnostic("No lidgren connections, skipping...");
            }

            return true;
        }

        public void Stop(string reason = "stopping") => Disconnect(reason);

        public void Disconnect(IConnection connection, string message)
            => Disconnect(new[] {connection}, message);

        public void Disconnect(ICollection<IConnection> connections, string message)
        {
            if (connections == null) return;
            foreach (var connection in connections)
            {
                (connection as LidgrenConnection)?.NetConnection?.Disconnect(message);
                (connection as LidgrenConnection)?.NetConnection?.Peer.FlushSendQueue();
                (connection as LidgrenConnection)?.NetConnection?.Peer.Shutdown(message);
                (connection as LidgrenConnection)?.Dispose();
            }
        }

        private NetIncomingMessage TryHandleInboundMessage()
        {
            Debug.Assert(mPeer != null, "mPeer != null");

            if (!mPeer.ReadMessage(out NetIncomingMessage message)) return null;

            var connection = message.SenderConnection;
            var lidgrenId = connection?.RemoteUniqueIdentifier ?? -1;
            var lidgrenIdHex = BitConverter.ToString(BitConverter.GetBytes(lidgrenId));

            switch (message.MessageType)
            {
                case NetIncomingMessageType.Data:
                    //Log.Diagnostic($"{message.MessageType}: {message}");
                    return message;

                case NetIncomingMessageType.StatusChanged:
                    Debug.Assert(mGuidLookup != null, "mGuidLookup != null");
                    Debug.Assert(mNetwork != null, "mNetwork != null");

                    switch (connection?.Status ?? NetConnectionStatus.None)
                    {
                        case NetConnectionStatus.None:
                        case NetConnectionStatus.InitiatedConnect:
                        case NetConnectionStatus.ReceivedInitiation:
                        case NetConnectionStatus.RespondedAwaitingApproval:
                        case NetConnectionStatus.RespondedConnect:
                            Log.Diagnostic($"{message.MessageType}: {message} [{connection?.Status}]");
                            break;
                        case NetConnectionStatus.Disconnecting:
                            Log.Debug($"{message.MessageType}: {message} [{connection?.Status}]");
                            break;

                        case NetConnectionStatus.Connected:
                            {
                                LidgrenConnection intersectConnection;
                                if (!mNetwork.Configuration.IsServer)
                                {
                                    intersectConnection = mNetwork.FindConnection<LidgrenConnection>(Guid.Empty);
                                    if (intersectConnection == null)
                                    {
                                        Log.Error("Bad state, no connection found.");
                                        mNetwork.Disconnect("client_connection_missing");
                                        connection?.Disconnect("client_connection_missing");
                                        break;
                                    }

                                    if (OnConnectionApproved != null)
                                    {
                                        FireOnConnectionApproved(this, intersectConnection);
                                    }
                                    else
                                    {
                                        Log.Error("No handlers for OnConnectionApproved.");
                                    }

                                    Debug.Assert(connection != null, "connection != null");
                                    IBuffer buffer = new LidgrenBuffer(connection.RemoteHailMessage);
                                    var approval = new ApprovalPacket(intersectConnection.Rsa);

                                    if (!approval.Read(ref buffer))
                                    {
                                        Log.Error("Unable to read approval message, disconnecting.");
                                        mNetwork?.Disconnect("client_error");
                                        connection.Disconnect("client_error");
                                        break;
                                    }

                                    if (!intersectConnection.HandleApproval(approval))
                                    {
                                        mNetwork?.Disconnect("bad_handshake_secret");
                                        connection.Disconnect("bad_handshake_secret");
                                        break;
                                    }

                                    var clientNetwork = mNetwork as ClientNetwork;
                                    if (clientNetwork == null) throw new InvalidOperationException();
                                    clientNetwork.AssignGuid(approval.Guid);

                                    Debug.Assert(mGuidLookup != null, "mGuidLookup != null");
                                    mGuidLookup.Add(connection.RemoteUniqueIdentifier, Guid.Empty);
                                }
                                else
                                {
                                    Log.Diagnostic($"{message.MessageType}: {message} [{connection?.Status}]");
                                    if (!mGuidLookup.TryGetValue(lidgrenId, out Guid guid))
                                    {
                                        Log.Error($"Unknown client connected ({lidgrenIdHex}).");
                                        connection?.Disconnect("server_unknown_client");
                                        break;
                                    }

                                    intersectConnection = mNetwork.FindConnection<LidgrenConnection>(guid);
                                }

                                if (OnConnected == null)
                                {
                                    Log.Error("No handlers for OnConnected.");
                                    break;
                                }

                                intersectConnection?.HandleConnected();
                                FireOnConnected(this, intersectConnection);
                            }
                            break;

                        case NetConnectionStatus.Disconnected:
                            {
                                Debug.Assert(connection != null, "connection != null");
                                Log.Debug($"{message.MessageType}: {message} [{connection.Status}]");

                                if (!mGuidLookup.TryGetValue(lidgrenId, out Guid guid))
                                {
                                    Log.Debug($"Unknown client disconnected ({lidgrenIdHex}).");
                                    FireOnDisconnected(this, null);
                                    break;
                                }

                                var client = mNetwork.FindConnection(guid);
                                if (client != null)
                                {
                                    client.HandleDisconnected();
                                    FireOnDisconnected(this, client);
                                    mNetwork.RemoveConnection(client);
                                }
                                
                                mGuidLookup.Remove(connection.RemoteUniqueIdentifier);
                            }
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;

                case NetIncomingMessageType.UnconnectedData:
                    OnUnconnectedMessage?.Invoke(mPeer, message);
                    Log.Diagnostic($"{message.MessageType}: {message}");
                    break;

                case NetIncomingMessageType.ConnectionApproval:
                {
                    IBuffer buffer = new LidgrenBuffer(message);
                    var hail = new HailPacket(mRsa);

                    if (!hail.Read(ref buffer))
                    {
                        Log.Error($"Failed to read hail, denying connection [{lidgrenIdHex}].");
                        Debug.Assert(connection != null, "connection != null");
                        connection.Deny("bad_hail");
                        break;
                    }

                    Debug.Assert(SharedConstants.VersionData != null, "SharedConstants.VERSION_DATA != null");
                    Debug.Assert(hail.VersionData != null, "hail.VersionData != null");
                    if (!SharedConstants.VersionData.SequenceEqual(hail.VersionData))
                    {
                        Log.Error($"Bad version detected, denying connection [{lidgrenIdHex}].");
                        Debug.Assert(connection != null, "connection != null");
                        connection.Deny("bad_version");
                        break;
                    }

                    if (OnConnectionApproved == null)
                    {
                        Log.Error($"No handlers for OnConnectionApproved, denying connection [{lidgrenIdHex}].");
                        Debug.Assert(connection != null, "connection != null");
                        connection.Deny("server_error");
                        break;
                    }

                    /* Approving connection from here-on. */
                    var aesKey = new byte[32];
                    mRng?.GetNonZeroBytes(aesKey);
                    var client = new LidgrenConnection(mNetwork, connection, aesKey, hail.RsaParameters);

                    Debug.Assert(mNetwork != null, "mNetwork != null");
                    if (!mNetwork.AddConnection(client))
                    {
                        Log.Error($"Failed to add the connection.");
                        Debug.Assert(connection != null, "connection != null");
                        connection.Deny("server_error");
                        break;
                    }

                    Debug.Assert(mGuidLookup != null, "mGuidLookup != null");
                    Debug.Assert(connection != null, "connection != null");
                    mGuidLookup.Add(connection.RemoteUniqueIdentifier, client.Guid);

                    Debug.Assert(mPeer != null, "mPeer != null");
                    var approval = new ApprovalPacket(client.Rsa, hail.HandshakeSecret, aesKey, client.Guid);
                    var approvalMessage = mPeer.CreateMessage(approval.EstimatedSize);
                    IBuffer approvalBuffer = new LidgrenBuffer(approvalMessage);
                    approval.Write(ref approvalBuffer);
                    connection.Approve(approvalMessage);
                    OnConnectionApproved(this, client);

                    break;
                }

                case NetIncomingMessageType.VerboseDebugMessage:
                    Log.Diagnostic($"{message.MessageType}: {message.ReadString()}");
                    break;

                case NetIncomingMessageType.DebugMessage:
                    Log.Debug($"{message.MessageType}: {message.ReadString()}");
                    break;

                case NetIncomingMessageType.WarningMessage:
                    Log.Warn($"{message.MessageType}: {message.ReadString()}");
                    break;

                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.Error:
                    Log.Error($"{message.MessageType}: {message.ReadString()}");
                    break;

                case NetIncomingMessageType.Receipt:
                    Log.Info($"{message.MessageType}: {message.ReadString()}");
                    break;

                case NetIncomingMessageType.DiscoveryRequest:
                case NetIncomingMessageType.DiscoveryResponse:
                case NetIncomingMessageType.NatIntroductionSuccess:
                case NetIncomingMessageType.ConnectionLatencyUpdated:
                    Log.Diagnostic($"{message.MessageType}: {message}");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }

        protected void FireOnConnected(INetworkLayerInterface sender, IConnection connection)
        {
            if (OnConnected == null)
            {
                Log.Error("No handlers for OnConnected.");
                return;
            }

            OnConnected(sender, connection);
        }

        protected void FireOnConnectionApproved(INetworkLayerInterface sender, IConnection connection)
        {
            if (OnConnectionApproved == null)
            {
                Log.Error("No handlers for OnConnectionApproved.");
                return;
            }

            OnConnectionApproved(sender, connection);
        }

        protected void FireOnDisconnected(INetworkLayerInterface sender, IConnection connection)
        {
            if (OnDisconnected == null)
            {
                Log.Error("No handlers for OnDisconnected.");
                return;
            }

            OnDisconnected(sender, connection);
        }

        private void SendMessage(NetOutgoingMessage message, LidgrenConnection connection,
            NetDeliveryMethod deliveryMethod, int sequenceChannel = 0)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (connection.NetConnection == null) throw new ArgumentNullException(nameof(connection.NetConnection));

            message.Encrypt(connection.Aes);
            connection.NetConnection.SendMessage(message, deliveryMethod, sequenceChannel);
        }

        private static NetDeliveryMethod TranslateTransmissionMode(TransmissionMode transmissionMode)
        {
            switch (transmissionMode)
            {
                case TransmissionMode.Any:
                    return NetDeliveryMethod.Unreliable;

                case TransmissionMode.Latest:
                    return NetDeliveryMethod.ReliableSequenced;

                // ReSharper disable once RedundantCaseLabel
                case TransmissionMode.All:
                default:
                    return NetDeliveryMethod.ReliableOrdered;
            }
        }

        internal bool Disconnect(string message)
        {
            mPeer.Connections?.ForEach(connection => connection?.Disconnect(message));
            return true;
        }
    }
}