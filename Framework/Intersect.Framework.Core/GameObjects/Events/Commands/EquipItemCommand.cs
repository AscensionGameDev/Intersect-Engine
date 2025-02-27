namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class EquipItemCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.EquipItem;

    public Guid ItemId { get; set; }

    public int Slot { get; set; }

    public bool Unequip { get; set; }

    public bool IsItem { get; set; } = true;

    public bool TriggerCooldown { get; set; }

}