using Intersect.Client.Framework.Graphics;

namespace Intersect.Editor.MonoGame.Graphics;

internal class MonoGameIndexBuffer : IndexBuffer
{
    internal readonly Microsoft.Xna.Framework.Graphics.IndexBuffer _indexBuffer;

    internal MonoGameIndexBuffer(GraphicsDevice graphicsDevice, Microsoft.Xna.Framework.Graphics.IndexBuffer indexBuffer)
        : base(graphicsDevice, indexBuffer.IndexElementSize.FromMonoGame(), indexBuffer.IndexCount, indexBuffer.BufferUsage.FromMonoGame(), false)
    {
        _indexBuffer = indexBuffer;
    }

    public override void GetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) =>
        _indexBuffer.GetData(offsetInBytes, data, startIndex, elementCount);

    public override void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) =>
        _indexBuffer.SetData(offsetInBytes, data, startIndex, elementCount);
}
