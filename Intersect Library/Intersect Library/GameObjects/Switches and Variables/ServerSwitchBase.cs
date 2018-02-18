using System;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class ServerSwitchBase : DatabaseObject<ServerSwitchBase>
    {
        public bool Value;

        [JsonConstructor]
        public ServerSwitchBase(int index) : base(index)
        {
            Name = "New Global Switch";
        }
    }
}