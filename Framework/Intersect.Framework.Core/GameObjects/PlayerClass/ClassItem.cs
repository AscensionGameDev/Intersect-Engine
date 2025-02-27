using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.PlayerClass;

public partial class ClassItem
{
    [JsonProperty]
    public Guid Id { get; set; }

    public int Quantity { get; set; }

    public ItemDescriptor Get()
    {
        return ItemDescriptor.Get(Id);
    }
}