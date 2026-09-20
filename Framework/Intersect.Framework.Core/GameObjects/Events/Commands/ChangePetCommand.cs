namespace Intersect.Framework.Core.GameObjects.Events.Commands;

/// <summary>
/// Gives the player a pet (summoning it right away), or dismisses their current pet when
/// <see cref="PetId"/> is <see cref="Guid.Empty"/>.
/// </summary>
public partial class ChangePetCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.ChangePet;

    public Guid PetId { get; set; }
}
