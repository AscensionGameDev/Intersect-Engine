using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.GameObjects;
using Intersect.GameObjects.Annotations;
using Intersect.Localization;

namespace Intersect.Framework.Core.GameObjects.Maps.Attributes;

public partial class MapCritterAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Critter;

    [EditorLabel("Attributes", "CritterSprite")]
    [EditorDisplay(
        EmptyBehavior = EmptyBehavior.ShowNoneOnNullOrEmpty,
        StringBehavior = StringBehavior.Trim
    )]
    public string Sprite { get; set; }

    [EditorLabel("Attributes", "CritterAnimation")]
    [EditorReference(typeof(AnimationDescriptor), nameof(AnimationDescriptor.Name))]
    public Guid AnimationId { get; set; }

    //Movement types will mimic npc options?
    //Random
    //Turn
    //Still
    [EditorLabel("Attributes", "CritterMovement")]
    [EditorDictionary("Attributes", "CritterMovements")]
    public byte Movement { get; set; }

    //Time in MS to traverse a tile once moving
    [EditorLabel("Attributes", "CritterSpeed")]
    [EditorTime]
    public int Speed { get; set; }

    //Time in MS between movements?
    [EditorLabel("Attributes", "CritterFrequency")]
    [EditorTime]
    public int Frequency { get; set; }

    //Lower, Middle, Upper
    [EditorLabel("Attributes", "CritterLayer")]
    [EditorDictionary("Attributes", "CritterLayers")]
    public byte Layer { get; set; }

    [EditorLabel("Attributes", "CritterDirection")]
    [EditorDictionary(nameof(Direction), "CritterDirection")]
    public byte Direction { get; set; }

    [EditorLabel("Attributes", "CritterIgnoreNpcAvoids")]
    [EditorBoolean(Style = BooleanStyle.YesNo)]
    public bool IgnoreNpcAvoids { get; set; }

    [EditorLabel("Attributes", "CritterBlockPlayers")]
    [EditorBoolean(Style = BooleanStyle.YesNo)]
    public bool BlockPlayers { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapCritterAttribute)base.Clone();
        att.Sprite = Sprite;
        att.AnimationId = AnimationId;
        att.Movement = Movement;
        att.Speed = Speed;
        att.Frequency = Frequency;
        att.Layer = Layer;
        att.IgnoreNpcAvoids = IgnoreNpcAvoids;

        return att;
    }
}