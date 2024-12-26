using Ceras;

namespace Intersect.Network;

public sealed partial class CerasDefaultBufferPool : ICerasBufferPool
{
    public byte[] RentBuffer(int minimumSize)
    {
        return System.Buffers.ArrayPool<byte>.Shared.Rent(minimumSize);
    }

    public void Return(byte[] buffer)
    {
        System.Buffers.ArrayPool<byte>.Shared.Return(buffer, false);
    }
}