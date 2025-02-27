namespace Intersect.GameObjects.Events;

public partial class KnowsSpellCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.KnowsSpell;

    public Guid SpellId { get; set; }
}