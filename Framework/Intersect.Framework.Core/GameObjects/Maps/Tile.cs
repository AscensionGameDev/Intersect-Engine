using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Maps;

public partial struct Tile
{
    public Guid TilesetId;

    public int X;

    public int Y;

    public byte Autotile;

    [JsonIgnore]
    public object TilesetTexture;
}
