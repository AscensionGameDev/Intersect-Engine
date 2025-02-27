using Intersect.GameObjects;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.PlayerClass;

public partial class ClassSpell
{
    [JsonProperty]
    public Guid Id { get; set; }

    public int Level { get; set; }

    public SpellDescriptor Get()
    {
        return SpellDescriptor.Get(Id);
    }
}