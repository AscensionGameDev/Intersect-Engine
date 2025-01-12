using Newtonsoft.Json;

namespace Intersect.GameObjects.Maps;

public partial struct Tile
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Guid TilesetId;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int X;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int Y;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public byte Autotile;

    [JsonIgnore]
    public object TilesetTexture;
}
