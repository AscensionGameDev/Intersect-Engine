namespace Intersect.Framework.Core.GameObjects.Conditions;

public partial class Condition
{
    public virtual ConditionType Type { get; }

    public bool Negated { get; set; }

    /// <summary>
    /// Configures whether or not this condition does or does not have an else branch.
    /// </summary>
    public bool ElseEnabled { get; set; } = true;
}