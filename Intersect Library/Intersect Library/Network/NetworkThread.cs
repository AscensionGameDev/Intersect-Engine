using System.Collections.Generic;
using System.Threading;
using Intersect.Logging;
using Intersect.Network.Packets;
using Intersect.Threading;

namespace Intersect.Network
{
    public sealed class NetworkThread
    {
        private bool mStarted;
        private readonly IThreadYield mThreadYield;
        private readonly PacketDispatcher mDispatcher;

        public string Name { get; }

        public Thread CurrentThread { get; }
        public PacketQueue Queue { get; }
        public IList<IConnection> Connections { get; }

        public bool IsRunning { get; private set; }
        
        public NetworkThread(PacketDispatcher dispatcher, IThreadYield yield, string name = null)
        {
            mThreadYield = yield ?? new ThreadYieldNet35();
            Name = name ?? "Network Worker Thread";
            CurrentThread = new Thread(Loop);
            Queue = new PacketQueue();
            mDispatcher = dispatcher;
            Connections = new List<IConnection>();
        }

        public void Start()
        {
            lock (this)
            {
                if (mStarted) return;
                mStarted = true;
                IsRunning = true;
            }

            CurrentThread.Start();
        }

        public void Stop()
        {
            lock (this)
            {
                IsRunning = false;
            }

            Queue?.Interrupt();
        }

        private void Loop()
        {
            while (IsRunning)
            {
                // ReSharper disable once PossibleNullReferenceException
                if (!Queue.TryNext(out IPacket packet)) continue;

                Log.Debug($"Dispatching packet '{packet.GetType().Name}' (size={(packet as BinaryPacket)?.Buffer?.Length() ?? -1}).");
                if (!(mDispatcher?.Dispatch(packet) ?? false))
                {
                    Log.Warn($"Failed to dispatch packet '{packet}'.");
                }

                mThreadYield?.Yield();
            }

            Log.Debug($"Exiting network thread ({Name}).");
        }
    }
}