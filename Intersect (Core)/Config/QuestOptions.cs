using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Config
{
    /// <summary>
    /// Contains configurable options pertaining quests and the layout of the quest log
    /// </summary>
    public class QuestOptions
    {
        /// <summary>
        /// Quest categories in which to separate quests into on the quest log
        /// </summary>
        public List<string> Categories { get; set; } = new List<string>()
        {
            "Main Quests",
            "Side Quests",
            "Completed Quests",
        };


        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            Categories.Clear();
        }
    }
}
