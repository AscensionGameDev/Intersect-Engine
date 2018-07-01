using System;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class PlayerSwitchBase : DatabaseObject<PlayerSwitchBase>
    {
        [JsonConstructor]
        public PlayerSwitchBase(Guid id) : base(id)
        {
            Name = "New Player Switch";
        }

        public PlayerSwitchBase()
        {
            Name = "New Player Switch";
        }
    }
}