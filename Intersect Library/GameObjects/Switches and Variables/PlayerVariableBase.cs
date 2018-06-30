using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class PlayerVariableBase : DatabaseObject<PlayerVariableBase>
    {
        [JsonConstructor]
        public PlayerVariableBase(int index) : base(index)
        {
            Name = "New Player Variable";
        }

        public PlayerVariableBase()
        {
            Name = "New Player Variable";
        }
    }
}