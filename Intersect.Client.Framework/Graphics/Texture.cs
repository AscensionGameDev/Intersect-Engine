namespace Intersect.Client.Framework.Graphics;

public abstract class Texture : GraphicsDeviceResource
{
    protected Texture(
        GraphicsDevice? graphicsDevice,
        string? name = default,
        int width = default,
        int height = default,
        bool mipMapEnabled = default
    ) : base(graphicsDevice, name)
    {
        Width = width;
        Height = height;
        IsMipMapEnabled = mipMapEnabled;
    }

    public int Height { get; }

    public bool IsMipMapEnabled { get; }

    public int Width { get; }

    public abstract void SetData<T>(T[] data) where T : struct;
}
