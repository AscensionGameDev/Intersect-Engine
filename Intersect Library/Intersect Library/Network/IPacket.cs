using System;
using Intersect.Memory;

namespace Intersect.Network
{
    public interface IPacket : IDisposable
    {
        IConnection Connection { get; }

        double Timestamp { get; }

        int EstimatedSize { get; }

        PacketCode Code { get; }

        PacketType Type { get; }

        bool Read(ref IBuffer buffer);
        bool Write(ref IBuffer buffer);
    }
}