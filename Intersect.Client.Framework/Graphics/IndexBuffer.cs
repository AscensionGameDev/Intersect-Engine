namespace Intersect.Client.Framework.Graphics;

public abstract class IndexBuffer : GraphicsDeviceResource
{
    private readonly bool _dynamic;

    protected IndexBuffer(GraphicsDevice graphicsDevice, IndexElementSize indexElementSize, int indexCount, BufferUsage bufferUsage, bool dynamic)
        : base(graphicsDevice)
    {
        _dynamic = dynamic;

        BufferUsage = bufferUsage;
        IndexCount = indexCount;
        IndexElementSize = indexElementSize;
    }

    ~IndexBuffer()
    {
    }

    public BufferUsage BufferUsage { get; private set; }

    public int IndexCount { get; private set; }

    public IndexElementSize IndexElementSize { get; private set; }

    public void GetData<T>(T[] data) where T : struct => GetData(0, data, 0, data.Length);

    public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct => GetData(0, data, startIndex, elementCount);

    public abstract void GetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) where T : struct;

    public void SetData<T>(T[] data) where T : struct => SetData(0, data, 0, data.Length);

    public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct => SetData(0, data, startIndex, elementCount);

    public abstract void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) where T : struct;
}
