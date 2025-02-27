using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.GameObjects.Ranges;

namespace Intersect.Framework.Core.GameObjects.Items;

public partial class EquipmentProperties
{
    public EquipmentProperties()
    {
    }

    public EquipmentProperties(ItemDescriptor descriptor)
    {
        Descriptor = descriptor;
    }

    [Key]
    public Guid DescriptorId { get; set; }

    [ForeignKey(nameof(DescriptorId))]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public ItemDescriptor Descriptor { get; set; }

    [NotMapped]
    public Dictionary<Stat, ItemRange> StatRanges { get; set; } = new();
}
