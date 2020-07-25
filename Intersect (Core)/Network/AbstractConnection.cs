using System;

using Intersect.Logging;

using JetBrains.Annotations;

namespace Intersect.Network
{

    public abstract class AbstractConnection : IConnection
    {
        [NotNull] private readonly object mDisposeLock;

        private bool mDisposed;

        protected AbstractConnection(Guid? guid = null)
        {
            mDisposeLock = new object();

            Guid = guid ?? Guid.NewGuid();
        }

        public Ceras Ceras { get; } = new Ceras(true);

        public virtual void Dispose()
        {
            lock (mDisposeLock)
            {
                if (mDisposed)
                {
                    return;
                }

                mDisposed = true;
            }
        }

        public Guid Guid { get; }

        public bool IsConnected { get; private set; }

        public abstract string Ip { get; }

        public abstract int Port { get; }

        public abstract bool Send(IPacket packet);

        public virtual void HandleConnected()
        {
            IsConnected = true;

            Log.Debug($"Connection established to remote [{Guid}/{Ip}:{Port}].");
        }

        public void HandleApproved()
        {
            Log.Debug($"Connection approved to remote [{Guid}/{Ip}:{Port}].");
        }

        public virtual void HandleDisconnected()
        {
            IsConnected = false;

            Log.Debug($"Connection terminated to remote [{Guid}/{Ip}:{Port}].");
        }

    }

}
