using Intersect.Client.Framework.Graphics;

namespace Intersect.Editor.MonoGame.Graphics;

internal class MonoGameVertexBuffer : VertexBuffer
{
    internal readonly Microsoft.Xna.Framework.Graphics.VertexBuffer _vertexBuffer;

    internal MonoGameVertexBuffer(
        GraphicsDevice graphicsDevice,
        Microsoft.Xna.Framework.Graphics.VertexBuffer vertexBuffer,
        VertexDeclaration vertexDeclaration,
        int vertexCount,
        BufferUsage bufferUsage,
        bool dynamic
    ) : base(graphicsDevice, vertexDeclaration, vertexCount, bufferUsage, dynamic)
    {
        _vertexBuffer = vertexBuffer;
    }

    public override void GetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride = 0) =>
        _vertexBuffer.GetData(offsetInBytes, data, startIndex, elementCount, vertexStride);

    public override void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride) =>
        _vertexBuffer.SetData(offsetInBytes, data, startIndex, elementCount, vertexStride);
}
