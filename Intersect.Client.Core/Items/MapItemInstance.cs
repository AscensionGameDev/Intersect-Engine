using Intersect.Client.Framework.Items;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Network.Packets.Server;
using Newtonsoft.Json;


namespace Intersect.Client.Items;


public partial class MapItemInstance : Item, IMapItemInstance
{
    /// <summary>
    /// The Unique Id of this particular MapItemInstance so we can refer to it elsewhere.
    /// </summary>
    public Guid Id { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    [JsonIgnore] public int TileIndex => Y * Options.Instance.Map.MapWidth + X;

    public MapItemInstance() : base()
    {
    }

    public MapItemInstance(int tileIndex, Guid uniqueId, Guid itemId, Guid? bagId, int quantity, ItemProperties itemProperties) : base()
    {
        Id = uniqueId;
        X = tileIndex % Options.Instance.Map.MapWidth;
        Y = (int)Math.Floor(tileIndex / (float)Options.Instance.Map.MapWidth);
        ItemId = itemId;
        BagId = bagId;
        Quantity = quantity;
        ItemProperties = itemProperties;
    }

}
