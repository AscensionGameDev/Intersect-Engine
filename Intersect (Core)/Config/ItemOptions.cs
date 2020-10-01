using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Intersect.Config
{
    /// <summary>
    /// Contains configurable settings pertaining to how certain item related systems work in the engine. 
    /// </summary>
    public class ItemOptions
    {

        /// <summary>
        /// Defines the list of cooldown groups available for use within the engine and editor, any item assigned to this group will trigger other items the player has on them from within the same group to go on cooldown.
        /// </summary>
        public List<string> CooldownGroups { get; set; } = new List<string>() {
            "Potions",
            "Food",
            "Special"
        };

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            CooldownGroups.Clear();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            CooldownGroups = new List<string>(CooldownGroups.Distinct());
        }
    }
}
