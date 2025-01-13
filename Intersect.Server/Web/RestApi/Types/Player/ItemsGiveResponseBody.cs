namespace Intersect.Server.Web.RestApi.Types.Player;

public readonly struct ItemsGiveResponseBody(Guid id, ItemsGiveQuantityData items)
{
    public Guid Id { get; init; } = id;

    public ItemsGiveQuantityData Quantity { get; init; } = items;
}
