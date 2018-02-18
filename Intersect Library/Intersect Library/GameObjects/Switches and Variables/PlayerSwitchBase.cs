using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class PlayerSwitchBase : DatabaseObject<PlayerSwitchBase>
    {
        [JsonConstructor]
        public PlayerSwitchBase(int index) : base(index)
        {
            Name = "New Player Switch";
        }
    }
}