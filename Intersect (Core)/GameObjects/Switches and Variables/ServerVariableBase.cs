using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Intersect.Enums;
using Intersect.GameObjects.Switches_and_Variables;
using Intersect.Models;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{

    public class ServerVariableBase : DatabaseObject<ServerVariableBase>, IFolderable
    {

        [JsonConstructor]
        public ServerVariableBase(Guid id) : base(id)
        {
            Name = "New Global Variable";
        }

        public ServerVariableBase()
        {
            Name = "New Global Variable";
        }

        //Identifier used for event chat variables to display the value of this variable/switch.
        //See https://www.ascensiongamedev.com/topic/749-event-text-variables/ for usage info.
        public string TextId { get; set; }

        public VariableDataTypes Type { get; set; } = VariableDataTypes.Boolean;

        [NotMapped]
        [JsonIgnore]
        public VariableValue Value { get; set; } = new VariableValue();

        [NotMapped]
        [JsonProperty("Value")]
        public dynamic ValueData { get => Value.Value; set => Value.Value = value; }

        [Column(nameof(Value))]
        [JsonIgnore]
        public string Json
        {
            get => Value.Json.ToString(Formatting.None);
            private set
            {
                if (VariableValue.TryParse(value, out var json))
                {
                    Value.Json = json;
                }
            }
        }

        /// <inheritdoc />
        public string Folder { get; set; } = "";

        /// <summary>
        /// Retrieve an array of variable names of the supplied data type.
        /// </summary>
        /// <param name="dataType">The data type to retrieve names of.</param>
        /// <returns>Returns an array of names.</returns>
        public static string[] GetNamesByType(VariableDataTypes dataType)
        {
            return Lookup.KeyList.OrderBy(pairs => Lookup[pairs]?.TimeCreated).Where(pairs => ((ServerVariableBase)Lookup[pairs]).Type == dataType).Select(pairs => ((ServerVariableBase)Lookup[pairs]).Name).ToArray();
        }

        /// <summary>
        /// Retrieve the list index of an Id within a specific data type list.
        /// </summary>
        /// <param name="id">The Id to look up.</param>
        /// <param name="dataType">The data type to search up.</param>
        /// <returns>Returns the list Index of the provided Id.</returns>
        public static int ListIndex(Guid id, VariableDataTypes dataType)
        {
            return Lookup.KeyList.OrderBy(pairs => Lookup[pairs]?.TimeCreated).Where(pairs => ((ServerVariableBase)Lookup[pairs]).Type == dataType).Select(pairs => ((ServerVariableBase)Lookup[pairs]).Id).ToList().IndexOf(id);
        }

        /// <summary>
        /// Retrieve the Id associated with a list index of a specific data type.
        /// </summary>
        /// <param name="listIndex">The list index to retrieve.</param>
        /// <param name="dataType">The data type to search up.</param>
        /// <returns>Returns the Id of the provided index.</returns>
        public static Guid IdFromList(int listIndex, VariableDataTypes dataType)
        {
            if (listIndex < 0 || listIndex > GetNamesByType(dataType).Length)
            {
                return Guid.Empty;
            }

            return Lookup.KeyList.OrderBy(pairs => Lookup[pairs]?.TimeCreated).Where(pairs => ((ServerVariableBase)Lookup[pairs]).Type == dataType).Select(pairs => ((ServerVariableBase)Lookup[pairs]).Id).ToArray()[listIndex];
        }
    }

}
