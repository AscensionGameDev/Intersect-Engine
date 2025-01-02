namespace Intersect.Client.Framework.Graphics;


public abstract partial class GameRenderTexture : GameTexture
{
    private static int _nextId;

    private readonly int _id;

    private bool _disposed;

    protected readonly int _width;
    protected readonly int _height;

    private GameRenderTexture(int width, int height, int id) : base($"RenderTexture#{id}")
    {
        _id = id;
        RenderTextureCount++;
        _width = width;
        _height = height;
    }

    protected GameRenderTexture(int width, int height) : this(width, height, ++_nextId)
    {
    }

    public static int RenderTextureCount { get; set; }

    public override int Width => _width;

    public override int Height => _height;

    /// <summary>
    ///     Called before a frame is drawn, if the renderer must re-created or anything it does it here.
    /// </summary>
    /// <returns></returns>
    public abstract bool Begin();

    /// <summary>
    ///     Called when the frame is done being drawn, generally used to finally display the content to the screen.
    /// </summary>
    public abstract void End();

    public bool SetActive(bool active)
    {
        return true;
    }

    /// <summary>
    ///     Clears everything off the render target with a specified color.
    /// </summary>
    public abstract void Clear(Color color);

    public abstract override object? GetTexture();

    public virtual void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _disposed = true;
        RenderTextureCount--;
    }

}
