using System;
using System.Linq;
using Intersect.Enums;
using Intersect.Models;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class GuildVariableBase : DatabaseObject<GuildVariableBase>, IFolderable
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

        public VariableDataTypes Type { get; set; } = VariableDataTypes.Boolean;

        /// <inheritdoc />
        public string Folder { get; set; } = "";

        /// <summary>
        /// Retrieve an array of variable names of the supplied data type.
        /// </summary>
        /// <param name="dataType">The data type to retrieve names of.</param>
        /// <returns>Returns an array of names.</returns>
        public static string[] GetNamesByType(VariableDataTypes dataType)
        {
            return Lookup.KeyList.OrderBy(pairs => Lookup[pairs]?.TimeCreated).Where(pairs => ((GuildVariableBase)Lookup[pairs]).Type == dataType).Select(pairs => ((GuildVariableBase)Lookup[pairs]).Name).ToArray();
        }

        /// <summary>
        /// Retrieve the list index of an Id within a specific data type list.
        /// </summary>
        /// <param name="id">The Id to look up.</param>
        /// <param name="dataType">The data type to search up.</param>
        /// <returns>Returns the list Index of the provided Id.</returns>
        public static int ListIndex(Guid id, VariableDataTypes dataType)
        {
            return Lookup.KeyList.OrderBy(pairs => Lookup[pairs]?.TimeCreated).Where(pairs => ((GuildVariableBase)Lookup[pairs]).Type == dataType).Select(pairs => ((GuildVariableBase)Lookup[pairs]).Id).ToList().IndexOf(id);
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

            return Lookup.KeyList.OrderBy(pairs => Lookup[pairs]?.TimeCreated).Where(pairs => ((GuildVariableBase)Lookup[pairs]).Type == dataType).Select(pairs => ((GuildVariableBase)Lookup[pairs]).Id).ToArray()[listIndex];
        }
    }
}
