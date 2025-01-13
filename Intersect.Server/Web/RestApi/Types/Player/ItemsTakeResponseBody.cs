namespace Intersect.Server.Web.RestApi.Types.Player;

public readonly struct ItemsTakeResponseBody(Guid itemId, int quantity)
{
    public Guid ItemId { get; } = itemId;

    public int Quantity { get; } = quantity;
}
