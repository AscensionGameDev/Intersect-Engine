using System.Runtime.InteropServices;

namespace Intersect.Client.Framework.Graphics;

public abstract class VertexBuffer : GraphicsDeviceResource
{
    private readonly bool _dynamic;

    protected VertexBuffer(
        GraphicsDevice graphicsDevice,
        VertexDeclaration vertexDeclaration,
        int vertexCount,
        BufferUsage bufferUsage,
        bool dynamic
    ) : base(graphicsDevice)
    {
        _dynamic = dynamic;

        BufferUsage = bufferUsage;
        VertexCount = vertexCount;
        VertexDeclaration = vertexDeclaration;
    }

    ~VertexBuffer()
    {
    }

    public BufferUsage BufferUsage { get; private set; }

    public int VertexCount { get; private set; }

    public VertexDeclaration VertexDeclaration { get; private set; }

    public void GetData<T>(T[] data) where T : struct
    {
        var elementSizeInBytes = Marshal.SizeOf<T>();
        GetData(0, data, 0, data.Length, elementSizeInBytes);
    }

    public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct => GetData(0, data, startIndex, elementCount, 0);

    public abstract void GetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride = 0) where T : struct;

    public void SetData<T>(T[] data) where T : struct
    {
        var elementSizeInBytes = Marshal.SizeOf<T>();
        SetData(0, data, 0, data.Length, elementSizeInBytes);
    }

    public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
    {
        var elementSizeInBytes = Marshal.SizeOf<T>();
        SetData(0, data, startIndex, elementCount, elementSizeInBytes);
    }

    public abstract void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride) where T : struct;
}
