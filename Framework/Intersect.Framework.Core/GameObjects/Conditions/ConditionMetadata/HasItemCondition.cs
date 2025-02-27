﻿using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Events;

namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

public partial class HasItemCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.HasItem;

    public Guid ItemId { get; set; }

    public int Quantity { get; set; }

    /// <summary>
    /// Defines whether this event command will use a variable for processing or not.
    /// </summary>
    public bool UseVariable { get; set; } = false;

    /// <summary>
    /// Defines whether the variable used is a Player or Global variable.
    /// </summary>
    public VariableType VariableType { get; set; } = VariableType.PlayerVariable;

    /// <summary>
    /// The Variable Id to use.
    /// </summary>
    public Guid VariableId { get; set; }

    public bool CheckBank { get; set; }
}