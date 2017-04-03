using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using Intersect.Logging;
using Intersect.Memory;
using Intersect.Threading;
using Lidgren.Network;

namespace Intersect.Network
{
    public abstract class AbstractNetwork : INetwork
    {
        private readonly object mLock;
        private bool mDisposed;

        private IList<NetworkThread> mThreads;
        private IDictionary<Guid, NetworkThread> mThreadLookup;
        private IDictionary<Guid, ConnectionMetadata> mConnectionLookup;
        private IDictionary<long, Guid> mConnectionGuidLookup;

        public Thread CurrentThread { get; }
        public PacketDispatcher Dispatcher { get; }

        protected NetPeerConfiguration Config { get; }
        public NetPeer Peer { get; }

        protected RSAParameters RsaParameters { get; set; }

        public Guid Guid { get; internal set; }

        protected AbstractNetwork(NetPeerConfiguration config, NetPeer peer)
        {
            mLock = new object();

            mThreads = new List<NetworkThread>();
            mThreadLookup = new Dictionary<Guid, NetworkThread>();
            mConnectionLookup = new Dictionary<Guid, ConnectionMetadata>();

            Dispatcher = new PacketDispatcher();
            CurrentThread = new Thread(Loop);

            Config = config;
            Peer = peer;

            Guid = Guid.NewGuid();
        }

        public bool IsRunning { get; private set; }

        public void Dispose()
        {
            if (mLock == null) throw new ArgumentNullException();
            lock (mLock)
            {
                if (mDisposed) return;

                DoDispose();

                mDisposed = true;
            }
        }

        protected virtual void DoDispose() => Disconnect("Disposing...");

        public bool Start()
        {
            if (mLock == null) throw new ArgumentNullException();
            lock (mLock)
            {
                if (IsRunning) return false;

                IsRunning = true;

                RegisterPackets();
                RegisterHandlers();

                CurrentThread?.Start();

                return IsRunning;
            }
        }

        public bool Stop()
        {
            if (mLock == null) throw new ArgumentNullException();
            lock (mLock)
            {
                if (!IsRunning) return false;
                IsRunning = false;
                return true;
            }
        }

        public abstract void Connect();

        public abstract void Listen();

        public virtual void Disconnect(string message = "")
        {
            Peer?.Shutdown(message);
            Stop();
        }

        public abstract bool Send(IPacket packet);

        public virtual bool Send(Guid guid, IPacket packet)
        {
            if (!mConnectionLookup.TryGetValue(guid, out ConnectionMetadata connectionMetadata)) return false;

            var message = Peer.CreateMessage();
            IBuffer buffer = new LidgrenBuffer(message);
            if (!packet.Write(ref buffer)) throw new Exception();

            var result = Peer.SendMessage(message, connectionMetadata.Connection, NetDeliveryMethod.ReliableOrdered);
            switch (result)
            {
                case NetSendResult.Sent:
                case NetSendResult.Queued:
                    return true;

                default:
                    return false;
            }
        }

        protected virtual void RegisterPackets()
        {
            
        }

        protected abstract void RegisterHandlers();

        protected abstract void Poll();
        
        private void Loop()
        {
            while (IsRunning)
            {
                if (Peer.ReadMessage(out NetIncomingMessage message))
                {
                    var lidgrenId = message.SenderConnection.RemoteUniqueIdentifier;
                    Guid guid;
                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            if (mConnectionGuidLookup.TryGetValue(lidgrenId, out guid))
                            {
                                if (mConnectionLookup.TryGetValue(guid, out ConnectionMetadata connection))
                                {
                                    EnqueueIncomingDataMessage(connection, message);
                                    break;
                                }

                                Log.Error($"Error reading from Intersect:{guid}.");
                            }

                            Log.Error($"Error reading from Lidgren:{lidgrenId}.");
                            break;

                        case NetIncomingMessageType.VerboseDebugMessage:
                            Log.Verbose(message.ReadString());
                            break;

                        case NetIncomingMessageType.DebugMessage:
                            Log.Debug(message.ReadString());
                            break;

                        case NetIncomingMessageType.WarningMessage:
                            Log.Warn(message.ReadString());
                            break;

                        case NetIncomingMessageType.ErrorMessage:
                            Log.Error(message.ReadString());
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            var status = (NetConnectionStatus) message.ReadByte();
                            switch (status)
                            {
                                case NetConnectionStatus.Connected:
                                    if (mConnectionGuidLookup.ContainsKey(lidgrenId))
                                    {
                                        Log.Error("Client connection already exists, terminating request...");
                                        message.SenderConnection.Disconnect("Error, are you trying to reconnect?");
                                        break;
                                    }

                                    var metadata = new ConnectionMetadata(message.SenderConnection);
                                    mConnectionLookup.Add(metadata.Guid, metadata);
                                    mConnectionGuidLookup.Add(lidgrenId, metadata.Guid);

                                    metadata.NegotiateEncryption(message, RsaParameters);

                                    break;

                                case NetConnectionStatus.Disconnected:
                                case NetConnectionStatus.Disconnecting:
                                    if (mConnectionGuidLookup.TryGetValue(lidgrenId, out guid))
                                    {
                                        Log.Info($"Disconnecting client {NetUtility.ToHexString(lidgrenId)} ({guid})...");
                                        mConnectionGuidLookup.Remove(lidgrenId);
                                        mConnectionLookup.Remove(guid);
                                    }
                                    break;
                            }
                            break;
                    }

                    Peer.Recycle(message);
                }
            }
        }

        protected void AssignNetworkThread(Guid guid, NetworkThread networkThread)
        {
            if (mThreadLookup?.ContainsKey(guid) ?? false)
            {
                mThreadLookup.Remove(guid);
            }

            mThreadLookup?.Add(guid, networkThread);
        }

        private void EnqueueIncomingDataMessage(IConnection connection, NetIncomingMessage message)
        {
            if (message == null) throw new ArgumentNullException();

            if (!message.ReadBytes(16, out byte[] guidBuffer)) return;

            var guid = new Guid(guidBuffer);
            if (!mThreadLookup.TryGetValue(guid, out NetworkThread thread)) return;

            var packetGroup = (PacketGroups)message.ReadByte();
            var group = PacketRegistry.Instance.GetGroup(packetGroup);
            if (group == null) return;

            IBuffer buffer = new LidgrenBuffer(message);
            var packet = group.Create(connection, buffer);
            if (packet.Read(ref buffer))
            {
                if (thread.Queue == null) throw new ArgumentNullException();
                thread.Queue.Enqueue(packet);
            }
            else
            {
                MemoryDump.Dump(message.Data);
            }
        }

        protected abstract int CalculateNumberOfThreads();

        protected abstract IThreadYield CreateThreadYield();

        private void CreateThreads()
        {
            for (var i = 0; i < CalculateNumberOfThreads(); i++)
                mThreads?.Add(new NetworkThread(Dispatcher, CreateThreadYield(), $"Network Thread #{i}"));
        }

        protected static RSACryptoServiceProvider LoadKeyFromAssembly(Assembly pa, string pb, bool pc)
        {
            if (pa == null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(pb?.Trim())) throw new ArgumentNullException();

            using (var a = pa.GetManifestResourceStream(pb))
            {
                if (a == null) throw new ArgumentNullException();

                using (var b = new BinaryReader(new GZipStream(a, CompressionMode.Decompress)))
                {
                    var d = (pc ? ReadPrivateKey(b) : ReadPublicKey(b));

                    b.Close();

                    var e = new RSACryptoServiceProvider();
                    e.ImportParameters(d);
                    return e;
                }
            }
        }

        private static RSAParameters ReadPrivateKey(BinaryReader pa)
        {
            var c = pa.ReadInt16();

            return new RSAParameters
            {
                D = pa.ReadBytes(c >> 3),
                DP = pa.ReadBytes(c >> 4),
                DQ = pa.ReadBytes(c >> 4),
                Exponent = pa.ReadBytes(3),
                InverseQ = pa.ReadBytes(c >> 4),
                Modulus = pa.ReadBytes(c >> 3),
                P = pa.ReadBytes(c >> 4),
                Q = pa.ReadBytes(c >> 4)
            };
        }

        private static RSAParameters ReadPublicKey(BinaryReader pa)
        {
            var c = pa.ReadInt16();

            return new RSAParameters
            {
                Exponent = pa.ReadBytes(3),
                Modulus = pa.ReadBytes(c >> 3)
            };
        }

        public IConnection FindConnection(long lidgrenId)
            => mConnectionGuidLookup.TryGetValue(lidgrenId, out Guid guid)
            ? FindConnection(guid) : null;

        public IConnection FindConnection(Guid guid)
            => mConnectionLookup.TryGetValue(guid, out ConnectionMetadata connection)
            ? connection : null;
    }
}