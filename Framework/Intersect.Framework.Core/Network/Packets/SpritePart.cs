using MessagePack;

namespace Intersect.Network.Packets;

/// <summary>
/// One interchangeable piece of a customised sprite: the "hair" category at index 2 is drawn from
/// "{sprite}_hair_2.png", and its own colour layers "{sprite}_hair_2_c1.png", ... are tinted with <see cref="Colors"/>.
/// Index 0 means "none" for that category.
/// </summary>
[MessagePackObject]
public partial class SpritePart
{
    public const int MaxParts = 16;

    public const int MaxCategoryLength = 32;

    public const int MaxIndex = 99;

    public const int MaxColors = 8;

    public SpritePart()
    {
    }

    public SpritePart(string category, int index, Color[] colors)
    {
        Category = category;
        Index = index;
        Colors = colors ?? [];
    }

    [Key(0)]
    public string Category { get; set; } = string.Empty;

    [Key(1)]
    public int Index { get; set; }

    [Key(2)]
    public Color[] Colors { get; set; } = [];

    /// <summary>
    /// Clamps everything a client could have tampered with; returns null when the part is unusable.
    /// </summary>
    public SpritePart? Sanitized()
    {
        var category = new string((Category ?? string.Empty).Trim().ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());
        if (category.Length == 0 || category.Length > MaxCategoryLength)
        {
            return null;
        }

        return new SpritePart(
            category,
            Math.Clamp(Index, 0, MaxIndex),
            (Colors ?? []).Take(MaxColors).Select(color => color ?? Color.White).ToArray()
        );
    }

    public static SpritePart[] SanitizeAll(SpritePart[]? parts) =>
        (parts ?? [])
            .Select(part => part?.Sanitized())
            .Where(part => part != null)
            .GroupBy(part => part!.Category)
            .Select(group => group.First()!)
            .Take(MaxParts)
            .ToArray();
}
