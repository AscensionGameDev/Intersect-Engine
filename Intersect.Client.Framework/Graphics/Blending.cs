namespace Intersect.Client.Framework.Graphics;

public abstract class Blending : GraphicsDeviceResource
{
    public static Blending NonPremultiplied { get; protected set; }

    protected Blending(GraphicsDevice? graphicsDevice, string? name = default) : base(graphicsDevice, name)
    {
    }

    public abstract BlendMode DestinationBlendMode { get; set; }

    public abstract BlendMode SourceBlendMode { get; set; }
}
