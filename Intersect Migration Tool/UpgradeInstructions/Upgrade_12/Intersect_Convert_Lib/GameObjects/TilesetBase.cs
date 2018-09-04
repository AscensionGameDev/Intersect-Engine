using System;
using Newtonsoft.Json;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects
{
    public class TilesetBase : DatabaseObject<TilesetBase>
    {
        [JsonConstructor]
        public TilesetBase(Guid id) : base(id)
        {
            Name = "";
        }

        //Ef Parameterless Constructor
        public TilesetBase()
        {
            Name = "";
        }

        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value?.Trim().ToLower(); }
        }
    }
}