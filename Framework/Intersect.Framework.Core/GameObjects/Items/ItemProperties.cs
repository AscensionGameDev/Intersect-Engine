using Intersect.Enums;
using MessagePack;

namespace Intersect.Framework.Core.GameObjects.Items;

[MessagePackObject]
public partial class ItemProperties
{
    public ItemProperties()
    {
    }

    public ItemProperties(ItemProperties other)
    {
        if (other == default)
        {
            throw new ArgumentNullException(nameof(other));
        }

        Array.Copy(other.StatModifiers, StatModifiers, Enum.GetValues<Stat>().Length);
        EnchantLevel = other.EnchantLevel;
    }

    [Key(0)]
    public int[] StatModifiers { get; set; } = new int[Enum.GetValues<Stat>().Length];

    /// <summary>Enchantment level (0 = none, 1-10) applied with a rune; drives the flame aura of a weapon.</summary>
    [Key(1)]
    public byte EnchantLevel { get; set; }
}
