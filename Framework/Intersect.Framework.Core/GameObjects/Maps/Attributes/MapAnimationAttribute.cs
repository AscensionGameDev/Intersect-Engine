using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.GameObjects;
using Intersect.GameObjects.Annotations;
using Intersect.Localization;

namespace Intersect.Framework.Core.GameObjects.Maps.Attributes;

public partial class MapAnimationAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Animation;

    [EditorLabel("Attributes", "MapAnimation")]
    [EditorReference(typeof(AnimationDescriptor), nameof(AnimationDescriptor.Name))]
    public Guid AnimationId { get; set; }

    [EditorLabel("Attributes", "MapAnimationBlock")]
    [EditorBoolean(Style = BooleanStyle.YesNo)]
    public bool IsBlock { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapAnimationAttribute) base.Clone();
        att.AnimationId = AnimationId;
        att.IsBlock = IsBlock;

        return att;
    }
}