using System;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class ServerSwitchBase : DatabaseObject<ServerSwitchBase>
    {
        public bool Value { get; set; }

        [JsonConstructor]
        public ServerSwitchBase(int index) : base(index)
        {
            Name = "New Global Switch";
        }

        public ServerSwitchBase()
        {
            Name = "New Global Switch";
        }
    }
}