namespace Intersect.Client.Framework.Graphics;

public interface IIndexBuffer : IGPUBuffer
{
    Type IndexType { get; }

    bool GetData<TIndex>(TIndex[] destination) where TIndex : struct;

    bool GetData<TIndex>(TIndex[] destination, int destinationOffset, int length) where TIndex : struct;

    bool GetData<TIndex>(int bufferOffset, TIndex[] destination, int destinationOffset, int length)
        where TIndex : struct;

    bool SetData<TIndex>(TIndex[] data) where TIndex : struct;

    bool SetData<TIndex>(TIndex[] data, int sourceOffset, int length) where TIndex : struct;

    bool SetData<TIndex>(int destinationOffset, TIndex[] data, int sourceOffset, int length) where TIndex : struct;

    bool SetData<TIndex>(
        int destinationOffset,
        TIndex[] data,
        int sourceOffset,
        int length,
        BufferWriteMode bufferWriteMode
    ) where TIndex : struct;
}