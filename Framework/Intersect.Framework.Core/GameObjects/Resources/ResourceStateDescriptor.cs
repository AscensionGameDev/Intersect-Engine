using Microsoft.EntityFrameworkCore;

namespace Intersect.Framework.Core.GameObjects.Resources;

[Owned]
public partial class ResourceStateDescriptor
{
    public string Texture { get; set; } = default;

    public ResourceGraphicType GraphicType { get; set; } = ResourceGraphicType.Graphic;

    public bool RenderBelowEntities { get; set; }

    public Guid AnimationId { get; set; } = Guid.Empty;

    public int MinHp { get; set; }

    public int MaxHp { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
}