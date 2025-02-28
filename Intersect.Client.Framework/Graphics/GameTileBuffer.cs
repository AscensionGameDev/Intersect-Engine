namespace Intersect.Client.Framework.Graphics;

public abstract partial class GameTileBuffer
{
    public abstract IIndexBuffer IndexBuffer { get; }

    public abstract IVertexBuffer VertexBuffer { get; }

    public abstract IGameTexture? Texture { get; }

    public static int TileBufferCount { get; set; }

    public abstract bool Supported { get; }

    public abstract bool TryAddTile(IGameTexture texture, int x, int y, int srcX, int srcY, int srcW, int srcH);

    public abstract bool TryUpdateTile(IGameTexture texture, int x, int y, int srcX, int srcY, int srcW, int srcH);

    public abstract bool SetData();

    public abstract void Dispose();
}