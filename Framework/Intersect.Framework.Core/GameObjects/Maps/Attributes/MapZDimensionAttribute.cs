using Intersect.Enums;
using Intersect.GameObjects.Annotations;

namespace Intersect.GameObjects.Maps;

public partial class MapZDimensionAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.ZDimension;

    [EditorLabel("Attributes", "ZGateway")]
    [EditorFormatted("Attributes", "FormatZLevel")]
    public byte GatewayTo { get; set; }

    [EditorLabel("Attributes", "ZBlock")]
    [EditorFormatted("Attributes", "FormatZLevel")]
    public byte BlockedLevel { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapZDimensionAttribute) base.Clone();
        att.GatewayTo = GatewayTo;
        att.BlockedLevel = BlockedLevel;

        return att;
    }
}