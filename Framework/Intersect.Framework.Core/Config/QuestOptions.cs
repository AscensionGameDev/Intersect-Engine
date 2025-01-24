using System.Runtime.Serialization;

namespace Intersect.Config;

/// <summary>
/// Contains configurable options pertaining quests and the layout of the quest log
/// </summary>
public partial class QuestOptions
{
    /// <summary>
    /// Quest categories in which to separate quests into on the quest log
    /// </summary>
    public List<string> Categories { get; set; } =
    [
        "Main Quests",
        "Side Quests",
        "Completed Quests",
    ];


    [OnDeserializing]
    internal void OnDeserializingMethod(StreamingContext context)
    {
        Categories.Clear();
    }
}
