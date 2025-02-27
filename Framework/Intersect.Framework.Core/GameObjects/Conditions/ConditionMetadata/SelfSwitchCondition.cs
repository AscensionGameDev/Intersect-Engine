﻿namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

public partial class SelfSwitchCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.SelfSwitch;

    public int SwitchIndex { get; set; } //0 through 3

    public bool Value { get; set; }
}