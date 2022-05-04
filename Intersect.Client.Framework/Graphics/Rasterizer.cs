namespace Intersect.Client.Framework.Graphics;

public abstract class Rasterizer : GraphicsDeviceResource
{
    protected Rasterizer(GraphicsDevice? graphicsDevice) : base(graphicsDevice)
    {
    }

    public abstract CullMode CullMode { get; set; }

    public abstract float DepthBias { get; set; }

    public abstract FillMode FillMode { get; set; }

    public abstract bool MultiSampleAntiAlias { get; set; }

    public abstract bool ScissorTestEnable { get; set; }

    public abstract float SlopeScaleDepthBias { get; set; }
}
