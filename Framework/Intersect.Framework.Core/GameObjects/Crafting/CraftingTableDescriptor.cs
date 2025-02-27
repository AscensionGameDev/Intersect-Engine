using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Crafting;

public partial class CraftingTableDescriptor : DatabaseObject<CraftingTableDescriptor>, IFolderable
{
    [NotMapped]
    public DbList<CraftingRecipeDescriptor> Crafts { get; set; } = [];

    [JsonConstructor]
    public CraftingTableDescriptor(Guid id) : base(id)
    {
        Name = "New Table";
    }

    //Parameterless constructor for EF
    public CraftingTableDescriptor()
    {
        Name = "New Table";
    }

    [JsonIgnore]
    [Column("Crafts")]
    public string CraftsJson
    {
        get => JsonConvert.SerializeObject(Crafts, Formatting.None);
        protected set => Crafts = JsonConvert.DeserializeObject<DbList<CraftingRecipeDescriptor>>(value);
    }

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;
}
