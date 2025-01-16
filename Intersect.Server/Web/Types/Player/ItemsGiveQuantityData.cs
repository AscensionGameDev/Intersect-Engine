namespace Intersect.Server.Web.Types.Player;

public readonly struct ItemsGiveQuantityData(int total, int bank, int inventory)
{
    public int Total { get; init; } = total;

    public int Bank { get; init; } = bank;

    public int Inventory { get; init; } = inventory;
}