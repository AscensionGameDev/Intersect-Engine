using System;

namespace Intersect.Network
{
    public interface INetwork : IDisposable
    {
        Guid Guid { get; }

        NetworkConfiguration Config { get; }

        bool IsRunning { get; }

        bool Start();

        bool Stop();

        void Disconnect(string message = "");

        bool Send(IPacket packet);
        bool Send(Guid guid, IPacket packet);

        IConnection FindConnection(Guid guid);
    }
}