﻿namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class SetGuildBankSlotsCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.SetGuildBankSlots;

    public VariableType VariableType { get; set; }

    public Guid VariableId { get; set; }
}