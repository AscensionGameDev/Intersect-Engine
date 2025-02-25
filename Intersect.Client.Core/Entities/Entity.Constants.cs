namespace Intersect.Client.Entities;

public partial class Entity
{
    protected readonly int MapWidth = Options.Instance.Map.MapWidth;
    protected readonly int MapHeight = Options.Instance.Map.MapHeight;
    protected readonly int TileWidth = Options.Instance.Map.TileWidth;
    protected readonly int TileHeight = Options.Instance.Map.TileHeight;
}