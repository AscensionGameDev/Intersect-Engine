using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Intersect.Enums;
using Intersect.GameObjects.Ranges;
using Newtonsoft.Json;

namespace Intersect.GameObjects;

public partial class EquipmentProperties
{
    public EquipmentProperties()
    {
    }

    public EquipmentProperties(ItemBase descriptor)
    {
        Descriptor = descriptor;
    }

    [Key]
    public Guid DescriptorId { get; set; }

    [ForeignKey(nameof(DescriptorId))]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public ItemBase Descriptor { get; set; }

    [NotMapped]
    public Dictionary<Stat, ItemRange> StatRanges { get; set; } = new();
}
