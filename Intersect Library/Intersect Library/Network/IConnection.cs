using System;
using Intersect.Memory;

namespace Intersect.Network
{
    public interface IConnection : IDisposable
    {
        Guid Guid { get; }

        bool IsConnected { get; }
        string Ip { get; }

        IBuffer CreateBuffer();

        bool Send(IPacket packet);
        bool Send(Guid guid, IPacket packet);
    }
}