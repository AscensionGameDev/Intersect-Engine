using System;
using Intersect.Memory;

namespace Intersect.Network
{
    public interface IConnection : IDisposable
    {
        Guid Guid { get; }

        IBuffer CreateBuffer();

        bool Send(IPacket packet);
        bool Send(Guid guid, IPacket packet);
    }
}