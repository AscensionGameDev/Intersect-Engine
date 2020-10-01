using System;

using Intersect.Logging;

namespace Intersect.Network
{

    public abstract class AbstractConnection : IConnection
    {

        private bool mDisposed;

        protected AbstractConnection(Guid? guid = null)
        {
            if (!guid.HasValue)
            {
                guid = Guid.NewGuid();
            }

            Guid = guid.Value;
            Statistics = new ConnectionStatistics();
        }

        public Ceras Ceras { get; } = new Ceras(true);

        public virtual void Dispose()
        {
            lock (this)
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

        public ConnectionStatistics Statistics { get; }

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

            Log.Debug($"Connectioned terminated to remote [{Guid}/{Ip}:{Port}].");
        }

    }

}
