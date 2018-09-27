using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Models;
using Newtonsoft.Json;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Crafting
{
    public class CraftingTableBase : DatabaseObject<CraftingTableBase>
    {
        [JsonIgnore]
        [Column("Crafts")]
        public string CraftsJson
        {
            get => JsonConvert.SerializeObject(Crafts, Formatting.None);
            protected set => Crafts = JsonConvert.DeserializeObject<DbList<CraftBase>>(value);
        }
        [NotMapped]
        public DbList<CraftBase> Crafts = new DbList<CraftBase>();



        [JsonConstructor]
        public CraftingTableBase(Guid id) : base(id)
        {
            Name = "New Table";
        }


        //Parameterless constructor for EF
        public CraftingTableBase()
        {
            Name = "New Table";
        }


    }
}
