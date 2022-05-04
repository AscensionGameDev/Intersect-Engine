using Intersect.Numerics;

namespace Intersect.Client.Framework.Graphics;

using Color = Intersect.Graphics.Color;
using Point = Numerics.Point;

public abstract class GraphicsDevice
{
    private readonly object _resourcesLock = new();

    private readonly List<WeakReference> _resources = new();

    public abstract Point BackBufferSize { get; set; }

    public abstract Color BlendFactor { get; set; }

    public abstract Blending Blending { get; set; }

    public abstract Rectangle ClippingBounds { get; set; }

    public abstract DepthStencil DepthStencil { get; set; }

    public abstract Rasterizer Rasterizer { get; set; }

    public abstract Viewport Viewport { get; set; }

    public abstract Effect CreateEffect();

    public abstract IndexBuffer CreateIndexBuffer(IndexElementSize indexElementSize, int indexCount, BufferUsage bufferUsage);

    public abstract IndexBuffer CreateIndexBuffer(Type indexType, int indexCount, BufferUsage usage);

    public abstract Rasterizer CreateRasterizer(
        CullMode cullMode = default,
        float depthBias = default,
        FillMode fillMode = default,
        bool multiSampleAntiAlias = default,
        bool scissorTestEnable = default,
        float slopeScaleDepthBias = default
    );

    public abstract Texture CreateTexture(int width, int height, bool mipMapEnabled);

    public abstract VertexBuffer CreateVertexBuffer(VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage bufferUsage);

    public abstract VertexBuffer CreateVertexBuffer(VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage bufferUsage, bool dynamic);

    public abstract VertexBuffer CreateVertexBuffer(Type type, int vertexCount, BufferUsage bufferUsage);

    public abstract void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount);

    public abstract void SetIndexBuffer(IndexBuffer indexBuffer);

    public abstract void SetVertexBuffer(VertexBuffer vertexBuffer, int vertexOffset = 0);

    internal virtual void AddResource(WeakReference resourceReference)
    {
        lock (_resourcesLock)
        {
            _resources.Add(resourceReference);
        }
    }

    internal virtual void RemoveResource(WeakReference resourceReference)
    {
        lock (_resourcesLock)
        {
            _ = _resources.Remove(resourceReference);
        }
    }
}
