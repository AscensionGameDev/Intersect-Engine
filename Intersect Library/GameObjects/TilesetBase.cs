using System.Text;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class TilesetBase : DatabaseObject<TilesetBase>
    {
        [JsonConstructor]
        public TilesetBase(int index) : base(index)
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