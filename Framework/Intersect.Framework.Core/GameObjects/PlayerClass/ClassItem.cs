using Intersect.GameObjects;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.PlayerClass;

public partial class ClassItem
{
    [JsonProperty]
    public Guid Id { get; set; }

    public int Quantity { get; set; }

    public ItemBase Get()
    {
        return ItemBase.Get(Id);
    }
}