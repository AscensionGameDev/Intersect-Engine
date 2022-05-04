using System.Numerics;

namespace Intersect.Client.Framework.Graphics;

public abstract class Effect : GraphicsDeviceResource
{
    protected Effect(GraphicsDevice? graphicsDevice, string? name = default)
        : base(graphicsDevice, name)
    {
    }

    public abstract Matrix4x4 Projection { get; set; }
    
    public abstract Texture Texture { get; set; }
    
    public abstract bool TextureEnabled { get; set; }
    
    public abstract bool VertexColorEnabled { get; set; }
    
    public abstract Matrix4x4 View { get; set; }
    
    public abstract Matrix4x4 World { get; set; }

    public abstract void OnEachPass(Action action);
}
