using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

using Intersect.Logging;
using Intersect.Memory;
using Intersect.Network.Events;
using Intersect.Network.Packets;
using Intersect.Utilities;

using Lidgren.Network;

namespace Intersect.Network.Lidgren
{

    public sealed class LidgrenInterface : INetworkLayerInterface
    {

        public delegate void HandleUnconnectedMessage(NetPeer peer, NetIncomingMessage message);

        private static readonly IConnection[] EmptyConnections = { };

        private readonly IDictionary<long, Guid> mGuidLookup;

        private readonly INetwork mNetwork;

        private readonly NetPeer mPeer;

        private readonly NetPeerConfiguration mPeerConfiguration;

        private readonly RandomNumberGenerator mRng;

        private readonly RSACryptoServiceProvider mRsa;

        public LidgrenInterface(INetwork network, Type peerType, RSAParameters rsaParameters)
        {
            if (peerType == null)
            {
                throw new ArgumentNullException(nameof(peerType));
            }

            mNetwork = network ?? throw new ArgumentNullException(nameof(network));

            var configuration = mNetwork.Configuration;
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(mNetwork.Configuration));
            }

            mRng = new RNGCryptoServiceProvider();

            mRsa = new RSACryptoServiceProvider();
            mRsa.ImportParameters(rsaParameters);
            mPeerConfiguration = new NetPeerConfiguration(
                $"{VersionHelper.ExecutableVersion} {VersionHelper.LibraryVersion} {SharedConstants.VersionName}"
            )
            {
                AcceptIncomingConnections = configuration.IsServer
            };

            mPeerConfiguration.DisableMessageType(NetIncomingMessageType.Receipt);
            mPeerConfiguration.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            mPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

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
                mPeerConfiguration.ConnectionTimeout = 60;
                mPeerConfiguration.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
                mPeerConfiguration.EnableMessageType(NetIncomingMessageType.DebugMessage);
                mPeerConfiguration.EnableMessageType(NetIncomingMessageType.ErrorMessage);
                mPeerConfiguration.EnableMessageType(NetIncomingMessageType.WarningMessage);
                mPeerConfiguration.EnableMessageType(NetIncomingMessageType.Error);
            }
            else
            {
                mPeerConfiguration.ConnectionTimeout = 15;
                mPeerConfiguration.DisableMessageType(NetIncomingMessageType.VerboseDebugMessage);
                mPeerConfiguration.DisableMessageType(NetIncomingMessageType.DebugMessage);
                mPeerConfiguration.DisableMessageType(NetIncomingMessageType.ErrorMessage);
                mPeerConfiguration.DisableMessageType(NetIncomingMessageType.WarningMessage);
                mPeerConfiguration.DisableMessageType(NetIncomingMessageType.Error);
            }

            mPeerConfiguration.PingInterval = 2.5f;
            mPeerConfiguration.UseMessageRecycling = true;
            mPeerConfiguration.AutoExpandMTU = true;

            var constructorInfo = peerType.GetConstructor(new[] {typeof(NetPeerConfiguration)});
            if (constructorInfo == null)
            {
                throw new ArgumentNullException(nameof(constructorInfo));
            }

            var constructedPeer = constructorInfo.Invoke(new object[] {mPeerConfiguration}) as NetPeer;
            mPeer = constructedPeer ?? throw new ArgumentNullException(nameof(constructedPeer));

            mGuidLookup = new Dictionary<long, Guid>();

            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            
            mPeer?.RegisterReceivedCallback(
                peer =>
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
                }
            );
        }

        public HandleUnconnectedMessage OnUnconnectedMessage { get; set; }

        public HandleConnectionEvent OnConnectionApproved { get; set; }

        public HandleConnectionEvent OnConnectionDenied { get; set; }

        public HandleConnectionRequest OnConnectionRequested { get; set; }

        private bool IsDisposing { get; set; }

        public bool IsDisposed { get; private set; }

        public HandlePacketAvailable OnPacketAvailable { get; set; }

        public HandleConnectionEvent OnConnected { get; set; }

        public HandleConnectionEvent OnDisconnected { get; set; }

        public void Start()
        {
            if (mNetwork.Configuration.IsServer)
            {
                Log.Pretty.Info($"Listening on {mPeerConfiguration.LocalAddress}:{mPeerConfiguration.Port}.");
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
            var hailParameters = connectionRsa.ExportParameters(false);
            var hail = new HailPacket(mRsa, handshakeSecret, SharedConstants.VersionData, hailParameters);
            hail.Encrypt();

            var hailMessage = mPeer.CreateMessage(hail.Data.Length);
            if (hailMessage == null)
            {
                throw new InvalidOperationException();
            }

            hailMessage.Data = hail.Data;
            hailMessage.LengthBytes = hail.Data.Length;

            if (mPeer.Status == NetPeerStatus.NotRunning)
            {
                mPeer.Start();
            }

            var connection = mPeer.Connect(mNetwork.Configuration.Host, mNetwork.Configuration.Port, hailMessage);
            var server = new LidgrenConnection(
                mNetwork, Guid.Empty, connection, handshakeSecret, connectionRsa.ExportParameters(true)
            );

            if (mNetwork.AddConnection(server))
            {
                return true;
            }

            Log.Error("Failed to add connection to list.");
            connection?.Disconnect("client_error");

            return false;
        }

        protected IConnection FindConnection(NetConnection netConnection)
        {
            var lidgrenId = netConnection?.RemoteUniqueIdentifier ?? -1;
            Debug.Assert(mGuidLookup != null, "mGuidLookup != null");
            if (!mGuidLookup.TryGetValue(lidgrenId, out var guid))
            {
                return default;
            }

            return mNetwork.FindConnection(guid);
        }

        public bool TryGetInboundBuffer(out IBuffer buffer, out IConnection connection)
        {
            buffer = default;
            connection = default;

            var message = TryHandleInboundMessage();
            if (message == null)
            {
                return true;
            }

            connection = FindConnection(message.SenderConnection);
            if (connection == null)
            {
                //Log.Error($"Received message from an unregistered endpoint.");
                mPeer.Recycle(message);

                return false;
            }

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
                //Log.Warn($"Received message from an unregistered endpoint.");
            }

            buffer = new LidgrenBuffer(message);

            return true;
        }

        public void ReleaseInboundBuffer(IBuffer buffer)
        {
            var message = (buffer as LidgrenBuffer)?.Buffer as NetIncomingMessage;
            mPeer?.Recycle(message);
        }

        public bool SendPacket(
            IPacket packet,
            IConnection connection = null,
            TransmissionMode transmissionMode = TransmissionMode.All
        )
        {
            if (connection == null)
            {
                return SendPacket(packet, EmptyConnections, transmissionMode);
            }

            if (!(connection is LidgrenConnection lidgrenConnection))
            {
                Log.Diagnostic("Tried to send to a non-Lidgren connection.");

                return false;
            }

            var deliveryMethod = TranslateTransmissionMode(transmissionMode);
            if (mPeer == null)
            {
                throw new ArgumentNullException(nameof(mPeer));
            }

            if (packet == null)
            {
                Log.Diagnostic("Tried to send a null packet.");

                return false;
            }

            var message = mPeer.CreateMessage();
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            message.Data = packet.Data;
            message.LengthBytes = message.Data.Length;

            SendMessage(message, lidgrenConnection, NetDeliveryMethod.ReliableOrdered);

            return true;
        }

        public bool SendPacket(
            IPacket packet,
            ICollection<IConnection> connections,
            TransmissionMode transmissionMode = TransmissionMode.All
        )
        {
            var deliveryMethod = TranslateTransmissionMode(transmissionMode);
            if (mPeer == null)
            {
                throw new ArgumentNullException(nameof(mPeer));
            }

            if (packet == null)
            {
                Log.Diagnostic("Tried to send a null packet.");

                return false;
            }

            var message = mPeer.CreateMessage();
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            message.Data = packet.Data;
            message.LengthBytes = message.Data.Length;

            if (connections == null || connections.Count(connection => connection != null) < 1)
            {
                connections = mNetwork?.FindConnections<IConnection>();
            }

            var lidgrenConnections = connections?.OfType<LidgrenConnection>().ToList();
            if (lidgrenConnections?.Count > 0)
            {
                var firstConnection = lidgrenConnections.First();

                lidgrenConnections.ForEach(
                    lidgrenConnection =>
                    {
                        if (lidgrenConnection == null)
                        {
                            return;
                        }

                        if (firstConnection == lidgrenConnection)
                        {
                            return;
                        }

                        if (message.Data == null)
                        {
                            throw new ArgumentNullException(nameof(message.Data));
                        }

                        //var messageClone = mPeer.CreateMessage(message.Data.Length);
                        //if (messageClone == null)
                        //{
                        //    throw new ArgumentNullException(nameof(messageClone));
                        //}

                        //Buffer.BlockCopy(message.Data, 0, messageClone.Data, 0, message.Data.Length);
                        //messageClone.LengthBytes = message.LengthBytes;
                        SendMessage(message, lidgrenConnection, deliveryMethod);
                    }
                );

                SendMessage(message, lidgrenConnections.First(), deliveryMethod);
            }
            else
            {
                Log.Diagnostic("No lidgren connections, skipping...");
            }

            return true;
        }

        public void Stop(string reason = "stopping")
        {
            Disconnect(reason);
        }

        public void Disconnect(IConnection connection, string message)
        {
            Disconnect(new[] {connection}, message);
        }

        public void Disconnect(ICollection<IConnection> connections, string message)
        {
            if (connections == null)
            {
                return;
            }

            foreach (var connection in connections)
            {
                (connection as LidgrenConnection)?.NetConnection?.Disconnect(message);
                (connection as LidgrenConnection)?.NetConnection?.Peer.FlushSendQueue();
                (connection as LidgrenConnection)?.NetConnection?.Peer.Shutdown(message);
                (connection as LidgrenConnection)?.Dispose();
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(LidgrenInterface));
            }

            if (IsDisposing)
            {
                return;
            }

            IsDisposing = true;

            switch (mPeer.Status)
            {
                case NetPeerStatus.NotRunning:
                case NetPeerStatus.ShutdownRequested:
                    break;

                case NetPeerStatus.Running:
                case NetPeerStatus.Starting:
                    mPeer.Shutdown(@"Terminating.");

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mPeer.Status));
            }

            IsDisposed = true;
            IsDisposing = false;
        }

        private NetIncomingMessage TryHandleInboundMessage()
        {
            Debug.Assert(mPeer != null, "mPeer != null");

            if (!mPeer.ReadMessage(out var message))
            {
                return null;
            }

            var senderConnection = message.SenderConnection;
            var lidgrenId = senderConnection?.RemoteUniqueIdentifier ?? -1;
            var lidgrenIdHex = BitConverter.ToString(BitConverter.GetBytes(lidgrenId));

            switch (message.MessageType)
            {
                case NetIncomingMessageType.Data:

                    //Log.Diagnostic($"{message.MessageType}: {message}");
                    return message;

                case NetIncomingMessageType.StatusChanged:
                    Debug.Assert(mGuidLookup != null, "mGuidLookup != null");
                    Debug.Assert(mNetwork != null, "mNetwork != null");

                    switch (senderConnection?.Status ?? NetConnectionStatus.None)
                    {
                        case NetConnectionStatus.None:
                        case NetConnectionStatus.InitiatedConnect:
                        case NetConnectionStatus.ReceivedInitiation:
                        case NetConnectionStatus.RespondedAwaitingApproval:
                        case NetConnectionStatus.RespondedConnect:
                            Log.Diagnostic($"{message.MessageType}: {message} [{senderConnection?.Status}]");

                            break;

                        case NetConnectionStatus.Disconnecting:
                            Log.Debug($"{message.MessageType}: {message} [{senderConnection?.Status}]");

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
                                    senderConnection?.Disconnect("client_connection_missing");

                                    break;
                                }

                                FireHandler(
                                    OnConnectionApproved, nameof(OnConnectionApproved), this, new ConnectionEventArgs
                                    {
                                        NetworkStatus = NetworkStatus.Connecting,
                                        Connection = intersectConnection
                                    }
                                );

                                Debug.Assert(senderConnection != null, "connection != null");
                                var approvalPacketData = senderConnection.RemoteHailMessage.Data;
                                var approval = MessagePacker.Instance.Deserialize(approvalPacketData) as ApprovalPacket;

                                if (!(approval?.Decrypt(intersectConnection.Rsa) ?? false))
                                {
                                    Log.Error("Unable to read approval message, disconnecting.");
                                    mNetwork.Disconnect("client_error");
                                    senderConnection.Disconnect("client_error");

                                    break;
                                }

                                if (!intersectConnection.HandleApproval(approval))
                                {
                                    mNetwork.Disconnect(NetworkStatus.HandshakeFailure.ToString());
                                    senderConnection.Disconnect(NetworkStatus.HandshakeFailure.ToString());

                                    break;
                                }

                                if (!(mNetwork is ClientNetwork clientNetwork))
                                {
                                    throw new InvalidOperationException();
                                }

                                clientNetwork.AssignGuid(approval.Guid);

                                Debug.Assert(mGuidLookup != null, "mGuidLookup != null");
                                mGuidLookup.Add(senderConnection.RemoteUniqueIdentifier, Guid.Empty);
                            }
                            else
                            {
                                Log.Diagnostic($"{message.MessageType}: {message} [{senderConnection?.Status}]");
                                if (!mGuidLookup.TryGetValue(lidgrenId, out var guid))
                                {
                                    Log.Error($"Unknown client connected ({lidgrenIdHex}).");
                                    senderConnection?.Disconnect("server_unknown_client");

                                    break;
                                }

                                intersectConnection = mNetwork.FindConnection<LidgrenConnection>(guid);
                            }

                            if (OnConnected != null)
                            {
                                intersectConnection?.HandleConnected();
                            }

                            FireHandler(
                                OnConnected, nameof(OnConnected), this, new ConnectionEventArgs
                                {
                                    NetworkStatus = NetworkStatus.Online,
                                    Connection = intersectConnection
                                }
                            );
                        }

                            break;

                        case NetConnectionStatus.Disconnected:
                        {
                            Debug.Assert(senderConnection != null, "connection != null");
                            Log.Debug($"{message.MessageType}: {message} [{senderConnection.Status}]");
                            var result = (NetConnectionStatus) message.ReadByte();
                            var reason = message.ReadString();

                            NetworkStatus networkStatus;
                            try
                            {
                                switch (reason)
                                {
                                    //Lidgren won't accept a connection with a bad version and sends this message back so we need to manually handle it
                                    case "Wrong application identifier!":
                                        networkStatus = NetworkStatus.VersionMismatch;

                                        break;

                                    case "Connection timed out":
                                        networkStatus = NetworkStatus.Quitting;

                                        break;

                                    case "Failed to establish connection - no response from remote host":
                                        networkStatus = NetworkStatus.Offline;

                                        break;

                                    case "closing":
                                        networkStatus = NetworkStatus.Offline;
                                        break;

                                    default:
                                        networkStatus = (NetworkStatus) Enum.Parse(
                                            typeof(NetworkStatus), reason ?? "<null>", true
                                        );

                                        break;
                                }
                            }
                            catch (Exception exception)
                            {
                                Log.Diagnostic(exception);
                                networkStatus = NetworkStatus.Unknown;
                            }

                            HandleConnectionEvent disconnectHandler;
                            string disconnectHandlerName;
                            switch (networkStatus)
                            {
                                case NetworkStatus.HandshakeFailure:
                                case NetworkStatus.ServerFull:
                                case NetworkStatus.VersionMismatch:
                                case NetworkStatus.Failed:
                                    disconnectHandler = OnConnectionDenied;
                                    disconnectHandlerName = nameof(OnConnectionDenied);

                                    break;

                                case NetworkStatus.Connecting:
                                case NetworkStatus.Online:
                                case NetworkStatus.Offline:
                                case NetworkStatus.Quitting:
                                case NetworkStatus.Unknown:
                                    disconnectHandler = OnDisconnected;
                                    disconnectHandlerName = nameof(OnDisconnected);

                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            if (!mGuidLookup.TryGetValue(lidgrenId, out var guid))
                            {
                                Log.Debug($"Unknown client disconnected ({lidgrenIdHex}).");
                                FireHandler(
                                    disconnectHandler, disconnectHandlerName, this,
                                    new ConnectionEventArgs {NetworkStatus = networkStatus}
                                );

                                break;
                            }

                            var client = mNetwork.FindConnection(guid);
                            if (client != null)
                            {
                                client.HandleDisconnected();

                                FireHandler(
                                    disconnectHandler, disconnectHandlerName, this,
                                    new ConnectionEventArgs {Connection = client, NetworkStatus = NetworkStatus.Offline}
                                );

                                mNetwork.RemoveConnection(client);
                            }

                            mGuidLookup.Remove(senderConnection.RemoteUniqueIdentifier);
                        }

                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;

                case NetIncomingMessageType.UnconnectedData:
                    OnUnconnectedMessage?.Invoke(mPeer, message);
                    Log.Diagnostic($"Net Incoming Message: {message.MessageType}: {message}");

                    break;

                case NetIncomingMessageType.ConnectionApproval:
                {
                    try
                    {
                        var hail = MessagePacker.Instance.Deserialize(message.Data) as HailPacket;
                        if (!(hail?.Decrypt(mRsa) ?? false))
                        {
                            Log.Warn($"Failed to read hail, denying connection [{lidgrenIdHex}].");
                            senderConnection?.Deny(NetworkStatus.HandshakeFailure.ToString());

                            break;
                        }

                        Debug.Assert(SharedConstants.VersionData != null, "SharedConstants.VERSION_DATA != null");
                        Debug.Assert(hail.VersionData != null, "hail.VersionData != null");
                        if (!SharedConstants.VersionData.SequenceEqual(hail.VersionData))
                        {
                            Log.Error($"Bad version detected, denying connection [{lidgrenIdHex}].");
                            senderConnection?.Deny(NetworkStatus.VersionMismatch.ToString());

                            break;
                        }
                        
                        Log.Debug($"hail Time={hail.Adjusted / TimeSpan.TicksPerMillisecond} Offset={hail.Offset / TimeSpan.TicksPerMillisecond} Real={hail.UTC / TimeSpan.TicksPerMillisecond}");
                        Log.Debug($"local Time={Timing.Global.Milliseconds} Offset={(long)Timing.Global.MillisecondsOffset} Real={Timing.Global.MillisecondsUTC}");
                        Log.Debug($"real delta={(Timing.Global.TicksUTC - hail.UTC) / TimeSpan.TicksPerMillisecond}");
                        Log.Debug($"NCPing={(long)Math.Ceiling(senderConnection.AverageRoundtripTime * 1000)}");

                        // Check if we've got more connections than we're allowed to handle!
                        if (mNetwork.ConnectionCount >= Options.MaxConnections)
                        {
                            Log.Info($"Connection limit reached, denying connection [{lidgrenIdHex}].");
                                senderConnection?.Deny(NetworkStatus.ServerFull.ToString());
                            break;
                        }

                        if (OnConnectionApproved == null)
                        {
                            Log.Error($"No handlers for OnConnectionApproved, denying connection [{lidgrenIdHex}].");
                            senderConnection?.Deny(NetworkStatus.Failed.ToString());

                            break;
                        }

                        /* Approving connection from here-on. */
                        var aesKey = new byte[32];
                        mRng?.GetNonZeroBytes(aesKey);
                        var client = new LidgrenConnection(mNetwork, senderConnection, aesKey, hail.RsaParameters);

                        if (!OnConnectionRequested(this, client))
                        {
                            Log.Warn($"Connection blocked due to ban or ip filter!");
                            senderConnection?.Deny(NetworkStatus.Failed.ToString());

                            break;
                        }

                        Debug.Assert(mNetwork != null, "mNetwork != null");
                        if (!mNetwork.AddConnection(client))
                        {
                            Log.Error($"Failed to add the connection.");
                            senderConnection?.Deny(NetworkStatus.Failed.ToString());

                            break;
                        }

                        Debug.Assert(mGuidLookup != null, "mGuidLookup != null");
                        Debug.Assert(senderConnection != null, "connection != null");
                        mGuidLookup.Add(senderConnection.RemoteUniqueIdentifier, client.Guid);

                        Debug.Assert(mPeer != null, "mPeer != null");
                        var approval = new ApprovalPacket(client.Rsa, hail.HandshakeSecret, aesKey, client.Guid);
                        approval.Encrypt();

                        var approvalMessage = mPeer.CreateMessage(approval.Data.Length);
                        if (approvalMessage == null)
                        {
                            throw new InvalidOperationException();
                        }

                        approvalMessage.Data = approval.Data;
                        approvalMessage.LengthBytes = approvalMessage.Data.Length;
                        senderConnection.Approve(approvalMessage);

                        OnConnectionApproved(
                            this, new ConnectionEventArgs {Connection = client, NetworkStatus = NetworkStatus.Online}
                        );
                    }
                    catch
                    {
                        senderConnection?.Deny(NetworkStatus.Failed.ToString());
                    }

                    break;
                }

                case NetIncomingMessageType.VerboseDebugMessage:
                    Log.Diagnostic($"Net Incoming Message: {message.MessageType}: {message.ReadString()}");

                    break;

                case NetIncomingMessageType.DebugMessage:
                    Log.Debug($"Net Incoming Message: {message.MessageType}: {message.ReadString()}");

                    break;

                case NetIncomingMessageType.WarningMessage:
                    Log.Warn($"Net Incoming Message: {message.MessageType}: {message.ReadString()}");

                    break;

                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.Error:
                    Log.Error($"Net Incoming Message: {message.MessageType}: {message.ReadString()}");

                    break;

                case NetIncomingMessageType.Receipt:
                    Log.Info($"Net Incoming Message: {message.MessageType}: {message.ReadString()}");

                    break;

                case NetIncomingMessageType.DiscoveryRequest:
                case NetIncomingMessageType.DiscoveryResponse:
                case NetIncomingMessageType.NatIntroductionSuccess:
                    Log.Diagnostic($"Net Incoming Message: {message.MessageType}: {message}");
                    break;

                case NetIncomingMessageType.ConnectionLatencyUpdated:
                {
                    Log.Diagnostic($"Net Incoming Message: {message.MessageType}: {message}");
                    var rtt = message.ReadFloat();
                    var connection = FindConnection(senderConnection);
                    if (connection != null)
                    {
                        connection.Statistics.Ping = (long)Math.Ceiling(senderConnection.AverageRoundtripTime * 1000);
                    }
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }

        private bool FireHandler(
            HandleConnectionEvent handler,
            string name,
            INetworkLayerInterface sender,
            ConnectionEventArgs connectionEventArgs
        )
        {
            handler?.Invoke(sender, connectionEventArgs);

            if (handler == null)
            {
                Log.Error($"No handlers for '{name}'.");
            }

            return handler != null;
        }

        private void SendMessage(
            NetOutgoingMessage message,
            LidgrenConnection connection,
            NetDeliveryMethod deliveryMethod,
            int sequenceChannel = 0
        )
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (connection.NetConnection == null)
            {
                throw new ArgumentNullException(nameof(connection.NetConnection));
            }

            lock (connection.Aes)
            {
                message.Encrypt(connection.Aes);
            }
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
