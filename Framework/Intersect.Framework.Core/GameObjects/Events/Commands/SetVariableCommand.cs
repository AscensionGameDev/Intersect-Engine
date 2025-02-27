using Intersect.Enums;

namespace Intersect.GameObjects.Events.Commands;

public partial class SetVariableCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.SetVariable;

    public VariableType VariableType { get; set; } = VariableType.PlayerVariable;

    public Guid VariableId { get; set; }

    public bool SyncParty { get; set; }

    public VariableMod Modification { get; set; }
}