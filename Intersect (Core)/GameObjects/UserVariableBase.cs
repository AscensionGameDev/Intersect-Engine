using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Intersect.Enums;
using Intersect.GameObjects.Switches_and_Variables;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public partial class UserVariableBase : VariableDescriptor<UserVariableBase>, IVariableBase
    {
        [JsonConstructor]
        public UserVariableBase(Guid id) : base(id)
        {
            Name = "New User Variable";
        }

        public UserVariableBase()
        {
            Name = "New User Variable";
        }

        //Identifier used for event chat variables to display the value of this variable/switch.
        //See https://www.ascensiongamedev.com/topic/749-event-text-variables/ for usage info.
        public string TextId { get; set; }

        // TODO rename this
        [Column("DataType")]
        public VariableDataType Type { get; set; } = VariableDataType.Boolean;

        /// <inheritdoc />
        public string Folder { get; set; } = "";
    }
}
