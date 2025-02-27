using Intersect.Enums;

namespace Intersect.GameObjects.Events.Commands;

public partial class ChangeGenderCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.ChangeGender;

    public Gender Gender { get; set; } = Gender.Male;
}