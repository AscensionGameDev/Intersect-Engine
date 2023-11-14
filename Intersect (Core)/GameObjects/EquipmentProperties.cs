using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Intersect.GameObjects.ItemRange;

namespace Intersect.GameObjects;
public class EquipmentProperties
{
    public EquipmentProperties()
    {
    }

    public EquipmentProperties(Guid descriptorId)
    {
        ItemDescriptorId = descriptorId;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private set; }

    public Guid ItemDescriptorId { get; set; }

    [ForeignKey(nameof(ItemDescriptorId))]
    public ItemBase Item { get; set; }

    public List<StatRange> StatRanges { get; set; }
}
