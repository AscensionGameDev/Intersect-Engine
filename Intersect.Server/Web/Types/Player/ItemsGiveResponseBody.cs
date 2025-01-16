namespace Intersect.Server.Web.Types.Player;

public readonly struct ItemsGiveResponseBody(Guid id, ItemsGiveQuantityData items)
{
    public Guid Id { get; init; } = id;

    public ItemsGiveQuantityData Quantity { get; init; } = items;
}
