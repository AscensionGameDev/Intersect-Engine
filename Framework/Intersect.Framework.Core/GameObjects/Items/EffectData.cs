using Microsoft.EntityFrameworkCore;

namespace Intersect.Framework.Core.GameObjects.Items;

[Owned]
public partial class EffectData
{
    public EffectData()
    {
        Type = default;
        Percentage = default;
    }

    public EffectData(ItemEffect type, int percentage)
    {
        Type = type;
        Percentage = percentage;
    }

    public ItemEffect Type { get; set; }

    public int Percentage { get; set; }
}