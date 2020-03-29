using System;

using Intersect.Enums;
using Intersect.Models;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{

    public class PlayerVariableBase : DatabaseObject<PlayerVariableBase>, IFolderable
    {

        [JsonConstructor]
        public PlayerVariableBase(Guid id) : base(id)
        {
            Name = "New Player Variable";
        }

        public PlayerVariableBase()
        {
            Name = "New Player Variable";
        }

        //Identifier used for event chat variables to display the value of this variable/switch.
        //See https://www.ascensiongamedev.com/topic/749-event-text-variables/ for usage info.
        public string TextId { get; set; }

        public VariableDataTypes Type { get; set; } = VariableDataTypes.Boolean;

        /// <inheritdoc />
        public string Folder { get; set; } = "";

    }

}
