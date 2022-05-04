namespace Intersect.Client.Framework.Graphics;

public abstract class DepthStencil : GraphicsDeviceResource
{
    public static DepthStencil DepthRead { get; protected set; }

    protected DepthStencil(GraphicsDevice? graphicsDevice, string? name = default)
        : base(graphicsDevice, name)
    {
    }

    public abstract bool DepthBufferEnable { get; set; }

    public abstract bool DepthBufferWriteEnable { get; set; }
}
