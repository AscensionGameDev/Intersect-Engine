using System;
using System.Collections.Generic;
using System.Threading;
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
        private IDictionary<Guid, NetConnection> mConnectionLookup;

        public Thread CurrentThread { get; }
        public PacketDispatcher Dispatcher { get; }

        protected NetPeerConfiguration Config { get; }
        public NetPeer Peer { get; }

        protected AbstractNetwork(NetPeerConfiguration config, NetPeer peer)
        {
            mLock = new object();

            mThreads = new List<NetworkThread>();
            mThreadLookup = new Dictionary<Guid, NetworkThread>();
            mConnectionLookup = new Dictionary<Guid, NetConnection>();

            Dispatcher = new PacketDispatcher();
            CurrentThread = new Thread(Loop);

            Config = config;
            Peer = peer;
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
            if (!mConnectionLookup.TryGetValue(guid, out NetConnection connection)) return false;

            var message = Peer.CreateMessage();
            if (!packet.Write(ref message)) throw new Exception();

            var result = Peer.SendMessage(message, connection, NetDeliveryMethod.ReliableOrdered);
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
                Poll();
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

        protected void EnqueueIncomingDataMessage(NetIncomingMessage message)
        {
            if (message == null) throw new ArgumentNullException();

            if (!message.ReadBytes(16, out byte[] guidBuffer)) return;

            var guid = new Guid(guidBuffer);
            if (!mThreadLookup.TryGetValue(guid, out NetworkThread thread)) return;

            var packetGroup = (PacketGroups)message.ReadByte();
            var group = PacketRegistry.Instance.GetGroup(packetGroup);
            if (group == null) return;

            var packet = group.Create(message);
            thread.Queue.Enqueue(packet);
        }

        protected abstract int CalculateNumberOfThreads();

        protected abstract IThreadYield CreateThreadYield();

        private void CreateThreads()
        {
            for (var i = 0; i < CalculateNumberOfThreads(); i++)
                mThreads?.Add(new NetworkThread(Dispatcher, CreateThreadYield(), $"Network Thread #{i}"));
        }
    }
}