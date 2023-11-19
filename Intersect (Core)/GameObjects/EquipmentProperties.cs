using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private set; }

    public Guid DescriptorId { get; set; }

    [ForeignKey(nameof(DescriptorId))]
    public ItemBase Descriptor { get; set; }

    [NotMapped]
    public Dictionary<Stat, ItemRange> StatRanges { get; set; }
}