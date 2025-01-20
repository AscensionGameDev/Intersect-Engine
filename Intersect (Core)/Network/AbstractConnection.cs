using Intersect.Core;
using Microsoft.Extensions.Logging;

namespace Intersect.Network;


public abstract partial class AbstractConnection : IConnection
{
    private readonly object mDisposeLock;

    private bool mDisposed;

    protected AbstractConnection(Guid? guid = null)
    {
        mDisposeLock = new object();

        Guid = guid ?? Guid.NewGuid();
        Statistics = new ConnectionStatistics();
    }

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

    public ConnectionStatistics Statistics { get; }

    public abstract bool Send(IPacket packet, TransmissionMode mode = TransmissionMode.All);

    public virtual void HandleConnected()
    {
        IsConnected = true;

        ApplicationContext.Context.Value?.Logger.LogDebug($"Connection established to remote [{Guid}/{Ip}:{Port}].");
    }

    public void HandleApproved()
    {
        ApplicationContext.Context.Value?.Logger.LogDebug($"Connection approved to remote [{Guid}/{Ip}:{Port}].");
    }

    public virtual void HandleDisconnected()
    {
        IsConnected = false;

        ApplicationContext.Context.Value?.Logger.LogDebug($"Connection terminated to remote [{Guid}/{Ip}:{Port}].");
    }

    public abstract void Disconnect(string message = default);
}
