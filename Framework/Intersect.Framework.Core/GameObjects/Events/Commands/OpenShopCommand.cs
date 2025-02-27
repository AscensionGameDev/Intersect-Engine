namespace Intersect.GameObjects.Events.Commands;

public partial class OpenShopCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.OpenShop;

    public Guid ShopId { get; set; }
}