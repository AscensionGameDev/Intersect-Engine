namespace Intersect.Client.Framework.Graphics;

public interface IVertexBuffer : IGPUBuffer
{
    Type VertexType { get; }

    PrimitiveType PrimitiveType { get; set; }

    bool GetData<TVertex>(TVertex[] destination) where TVertex : struct;

    bool GetData<TVertex>(TVertex[] destination, int destinationOffset, int length) where TVertex : struct;

    bool GetData<TVertex>(int bufferOffset, TVertex[] destination, int destinationOffset, int length)
        where TVertex : struct;

    bool SetData<TVertex>(TVertex[] data) where TVertex : struct;

    bool SetData<TVertex>(TVertex[] data, int sourceOffset, int length) where TVertex : struct;

    bool SetData<TVertex>(int destinationOffset, TVertex[] data, int sourceOffset, int length) where TVertex : struct;

    bool SetData<TVertex>(
        int destinationOffset,
        TVertex[] data,
        int sourceOffset,
        int length,
        BufferWriteMode bufferWriteMode
    ) where TVertex : struct;
}