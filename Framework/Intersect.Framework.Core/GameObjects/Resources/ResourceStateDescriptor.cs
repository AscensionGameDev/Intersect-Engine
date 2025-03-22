using Microsoft.EntityFrameworkCore;

namespace Intersect.Framework.Core.GameObjects.Resources;

[Owned]
public partial class ResourceStateDescriptor
{
    public string Texture { get; set; } = default;

    public ResourceTextureSource TextureType { get; set; } = ResourceTextureSource.Resource;

    public bool RenderBelowEntities { get; set; }

    public Guid AnimationId { get; set; }

    public int MinimumHealth { get; set; }

    public int MaximumHealth { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
}