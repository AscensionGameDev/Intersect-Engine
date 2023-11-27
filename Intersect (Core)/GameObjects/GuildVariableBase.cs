using System;
using System.Linq;
using Intersect.Enums;
using Intersect.GameObjects.Switches_and_Variables;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public partial class GuildVariableBase : VariableDescriptor<GuildVariableBase>, IVariableBase
    {
        [JsonConstructor]
        public GuildVariableBase(Guid id) : base(id)
        {
            Name = "New Guild Variable";
        }

        public GuildVariableBase()
        {
            Name = "New Guild Variable";
        }

        //Identifier used for event chat variables to display the value of this variable/switch.
        //See https://www.ascensiongamedev.com/topic/749-event-text-variables/ for usage info.
        public string TextId { get; set; }

        // TODO(0.8): Rename this to DataType
        public VariableDataType Type { get; set; } = VariableDataType.Boolean;

        /// <inheritdoc />
        public string Folder { get; set; } = string.Empty;
    }
}
