namespace Intersect.GameObjects.Events;

public partial class NoNpcsOnMapCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.NoNpcsOnMap;

    public bool SpecificNpc { get; set; }

    public Guid NpcId { get; set; }
}