namespace Intersect.GameObjects;

public partial class Drop
{
    public double Chance { get; set; }

    public Guid ItemId { get; set; }

    public int Quantity { get; set; } = 1;

    public int MinQuantity { get; set; } = 1;
}
