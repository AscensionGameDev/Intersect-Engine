using System;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class ServerVariableBase : DatabaseObject<ServerVariableBase>
    {
        public int Value { get; set; }

        [JsonConstructor]
        public ServerVariableBase(Guid id) : base(id)
        {
            Name = "New Global Variable";
        }

        public ServerVariableBase()
        {
            Name = "New Global Variable";
        }
    }
}