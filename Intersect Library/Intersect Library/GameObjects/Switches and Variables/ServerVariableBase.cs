using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class ServerVariableBase : DatabaseObject<ServerVariableBase>
    {
        public int Value;

        [JsonConstructor]
        public ServerVariableBase(int index) : base(index)
        {
            Name = "New Global Variable";
        }
    }
}