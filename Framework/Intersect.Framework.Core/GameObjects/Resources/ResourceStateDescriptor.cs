using Microsoft.EntityFrameworkCore;

namespace Intersect.Framework.Core.GameObjects.Resources;

[Owned]
public partial class ResourceStateDescriptor
{
    public string Graphic { get; set; } = null;

    public bool RenderBelowEntities { get; set; }

    public bool GraphicFromTileset { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
}