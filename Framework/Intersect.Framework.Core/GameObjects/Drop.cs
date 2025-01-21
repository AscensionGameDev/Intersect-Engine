using Newtonsoft.Json;

namespace Intersect.GameObjects;

public partial class Drop
{
    private int _maxQuantity;

    public double Chance { get; set; }

    public Guid ItemId { get; set; }

    [JsonProperty]
    [Obsolete(message: $"Use {nameof(MinQuantity)} instead, the Quantity property will be removed in 0.9-beta", error: true)]
    private int Quantity
    {
        /* Setter only [JsonProperty] annotated private property to "silently" rename Quantity to MinQuantity */
        set => MinQuantity = value;
    }

    /* By renaming Quantity to MinQuantity the "automatic" range given an original "Quantity" value of 3 will be 3 to 3, instead of 1 to 3 or 0 to 3 */
    public int MinQuantity { get; set; } = 1;

    public int MaxQuantity
    {
        /* Special getter ensures that MaxQuantity can never be less than MinQuantity, and it doesn't need a non-zero default value as a result */
        get => Math.Max(_maxQuantity, MinQuantity);
        set => _maxQuantity = value;
    }
}