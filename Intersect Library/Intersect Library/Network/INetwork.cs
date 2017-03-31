using System;
using System.Net;
using Lidgren.Network;

namespace Intersect.Network
{
    public interface INetwork : IDisposable
    {
        NetPeer Peer { get; }

        bool IsRunning { get; }

        bool Start();

        bool Stop();

        void Connect();

        void Disconnect(string message = "");

        bool Send(IPacket packet);
        bool Send(Guid guid, IPacket packet);
    }
}