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
        /// Defines the number of cooldown groups available for use within the engine and editor, any item assigned to a group will trigger other items the player has on them from within the same group to go on cooldown.
        /// </summary>
        public int CooldownGroups { get; set; } = 5;

    }
}
