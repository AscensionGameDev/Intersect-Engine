using System;
using Intersect.Memory;

namespace Intersect.Network
{
    public interface IPacket : IDisposable
    {
        byte[] Data();
    }
}