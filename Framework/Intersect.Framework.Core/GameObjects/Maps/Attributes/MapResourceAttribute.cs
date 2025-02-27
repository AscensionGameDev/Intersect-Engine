using Intersect.GameObjects;
using Intersect.GameObjects.Annotations;

namespace Intersect.Framework.Core.GameObjects.Maps.Attributes;

public partial class MapResourceAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Resource;

    [EditorLabel("Attributes", "Resource")]
    [EditorReference(typeof(ResourceBase), nameof(ResourceBase.Name))]
    public Guid ResourceId { get; set; }

    [EditorLabel("Attributes", "ZDimension")]
    [EditorFormatted("Attributes", "FormatSpawnLevel")]
    public byte SpawnLevel { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapResourceAttribute) base.Clone();
        att.ResourceId = ResourceId;
        att.SpawnLevel = SpawnLevel;

        return att;
    }
}