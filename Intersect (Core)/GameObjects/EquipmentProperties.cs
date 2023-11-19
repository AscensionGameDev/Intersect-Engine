using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.GameObjects.Ranges;

namespace Intersect.GameObjects;

public partial class EquipmentProperties
{
    public EquipmentProperties()
    {
    }

    public EquipmentProperties(Guid descriptorId)
    {
        DescriptorId = descriptorId;
    }

    [Key]
    public Guid DescriptorId { get; set; }

    [ForeignKey(nameof(DescriptorId))]
    public ItemBase Descriptor { get; set; }

    [NotMapped]
    public Dictionary<Stat, ItemRange> StatRanges { get; set; }
}
