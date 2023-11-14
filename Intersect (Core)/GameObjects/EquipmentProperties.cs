using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Intersect.GameObjects.ItemRange;

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

    public List<StatRange> StatRanges { get; set; }
}
