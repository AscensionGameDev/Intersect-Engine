using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Intersect.Logging;

using JetBrains.Annotations;

namespace Intersect.Network
{

    public sealed class NetworkThread
    {
        [NotNull] private readonly object mLifecycleLock;

        [NotNull] private readonly PacketDispatcher mDispatcher;

        private bool mStarted;

        public NetworkThread([NotNull] PacketDispatcher dispatcher, string name = null)
        {
            mLifecycleLock = new object();
            mDispatcher = dispatcher;

            Name = name ?? "Network Worker Thread";
            CurrentThread = new Thread(Loop);
            Queue = new PacketQueue();
            Connections = new List<IConnection>();
        }

        [NotNull] public string Name { get; }

        [NotNull] public Thread CurrentThread { get; }

        [NotNull] public PacketQueue Queue { get; }

        [NotNull] public IList<IConnection> Connections { get; }

        public bool IsRunning { get; private set; }

        public void Start()
        {
            lock (mLifecycleLock)
            {
                if (mStarted)
                {
                    return;
                }

                mStarted = true;
                IsRunning = true;
            }

            CurrentThread.Start();
        }

        public void Stop()
        {
            lock (mLifecycleLock)
            {
                IsRunning = false;
            }

            Queue.Interrupt();
        }

        private void Loop()
        {
            var sw = new Stopwatch();
#if DIAGNOSTIC
            var last = 0L;
#endif
            sw.Start();
            while (IsRunning)
            {
                // ReSharper disable once PossibleNullReferenceException
                if (!Queue.TryNext(out var packet))
                {
                    continue;
                }

                //Log.Debug($"Dispatching packet '{packet.GetType().Name}' (size={(packet as BinaryPacket)?.Buffer?.Length() ?? -1}).");
                if (!mDispatcher.Dispatch(packet))
                {
                    Log.Warn($"Failed to dispatch packet '{packet}'.");
                }

#if DIAGNOSTIC
                if (last + (1 * TimeSpan.TicksPerSecond) < sw.ElapsedTicks)
                {
                    last = sw.ElapsedTicks;
                    Console.Title = $"Queue size: {Queue.Size}";
                }
#endif

                packet?.Dispose();

                Thread.Yield();
            }

            sw.Stop();

            Log.Debug($"Exiting network thread ({Name}).");
        }

    }

}
