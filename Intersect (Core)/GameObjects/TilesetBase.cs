using System;

using Intersect.Models;

using Newtonsoft.Json;

namespace Intersect.GameObjects
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
            get => base.Name;
            set => base.Name = value?.Trim().ToLower();
        }

    }

}
