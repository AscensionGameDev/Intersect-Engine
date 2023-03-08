using System;

using Intersect.Enums;

namespace Intersect.GameObjects.Events
{

    public enum ConditionTypes
    {

        VariableIs = 0,

        HasItem = 4,

        ClassIs,

        KnowsSpell,

        LevelOrStat,

        SelfSwitch, //Only works for events.. not for checking if you can destroy a resource or something like that

        AccessIs,

        TimeBetween,

        CanStartQuest,

        QuestInProgress,

        QuestCompleted,

        NoNpcsOnMap,

        GenderIs,

        MapIs,

        IsItemEquipped,

        HasFreeInventorySlots,

        InGuildWithRank,

        MapZoneTypeIs,

        CheckEquipment,

    }

    public partial class Condition
    {

        public virtual ConditionTypes Type { get; }

        public bool Negated { get; set; }

        /// <summary>
        /// Configures whether or not this condition does or does not have an else branch.
        /// </summary>
        public bool ElseEnabled { get; set; } = true;

    }

    public partial class VariableIsCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.VariableIs;

        public VariableType VariableType { get; set; } = VariableType.PlayerVariable;

        public Guid VariableId { get; set; }

        public VariableComparison Comparison { get; set; } = new VariableComparison();

    }

    public partial class HasItemCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.HasItem;

        public Guid ItemId { get; set; }

        public int Quantity { get; set; }

        /// <summary>
        /// Defines whether this event command will use a variable for processing or not.
        /// </summary>
        public bool UseVariable { get; set; } = false;

        /// <summary>
        /// Defines whether the variable used is a Player or Global variable.
        /// </summary>
        public VariableType VariableType { get; set; } = VariableType.PlayerVariable;

        /// <summary>
        /// The Variable Id to use.
        /// </summary>
        public Guid VariableId { get; set; }

        public bool CheckBank { get; set; }

    }

    public partial class ClassIsCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.ClassIs;

        public Guid ClassId { get; set; }

    }

    public partial class KnowsSpellCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.KnowsSpell;

        public Guid SpellId { get; set; }

    }

    public partial class LevelOrStatCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.LevelOrStat;

        public bool ComparingLevel { get; set; }

        public Stat Stat { get; set; }

        public VariableComparator Comparator { get; set; } = VariableComparator.Equal;

        public int Value { get; set; }

        public bool IgnoreBuffs { get; set; }

    }

    public partial class SelfSwitchCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.SelfSwitch;

        public int SwitchIndex { get; set; } //0 through 3

        public bool Value { get; set; }

    }

    public partial class AccessIsCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.AccessIs;

        public Access Access { get; set; }

    }

    public partial class TimeBetweenCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.TimeBetween;

        public int[] Ranges { get; set; } = new int[2];

    }

    public partial class CanStartQuestCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.CanStartQuest;

        public Guid QuestId { get; set; }

    }

    public partial class QuestInProgressCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.QuestInProgress;

        public Guid QuestId { get; set; }

        public QuestProgressState Progress { get; set; } = QuestProgressState.OnAnyTask;

        public Guid TaskId { get; set; }

    }

    public partial class QuestCompletedCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.QuestCompleted;

        public Guid QuestId { get; set; }

    }

    public partial class NoNpcsOnMapCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.NoNpcsOnMap;

        public bool SpecificNpc { get; set; }

        public Guid NpcId { get; set; }

    }

    public partial class GenderIsCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.GenderIs;

        public Gender Gender { get; set; } = Gender.Male;

    }

    public partial class MapIsCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.MapIs;

        public Guid MapId { get; set; }

    }

    public partial class IsItemEquippedCondition : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.IsItemEquipped;

        public Guid ItemId { get; set; }

    }

    /// <summary>
    /// Defines the condition class used when checking for a player's free inventory slots.
    /// </summary>
    public partial class HasFreeInventorySlots : Condition
    {
        /// <summary>
        /// Defines the type of condition.
        /// </summary>
        public override ConditionTypes Type { get; } = ConditionTypes.HasFreeInventorySlots;

        /// <summary>
        /// Defines the amount of inventory slots that need to be free to clear this condition.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Defines whether this event command will use a variable for processing or not.
        /// </summary>
        public bool UseVariable { get; set; } = false;

        /// <summary>
        /// Defines whether the variable used is a Player or Global variable.
        /// </summary>
        public VariableType VariableType { get; set; } = VariableType.PlayerVariable;

        /// <summary>
        /// The Variable Id to use.
        /// </summary>
        public Guid VariableId { get; set; }

    }

    /// <summary>
    /// Defines the condition class used when checking whether a player is in a guild with at least a specified rank
    /// </summary>
    public partial class InGuildWithRank : Condition
    {
        /// <summary>
        /// Defines the type of condition
        /// </summary>
        public override ConditionTypes Type { get; } = ConditionTypes.InGuildWithRank;

        /// <summary>
        /// The guild rank the condition checks for as a minimum
        /// </summary>
        public int Rank { get; set; }
    }

    /// <summary>
    /// Defines the condition class used when checking whether a player is on a specific map zone type.
    /// </summary>
    public partial class MapZoneTypeIs : Condition
    {
        /// <summary>
        /// Defines the type of condition.
        /// </summary>
        public override ConditionTypes Type { get; } = ConditionTypes.MapZoneTypeIs;

        /// <summary>
        /// Defines the map Zone Type to compare to.
        /// </summary>
        public MapZone ZoneType { get; set; }
    }

    public partial class VariableComparison
    {

    }

    public partial class BooleanVariableComparison : VariableComparison
    {

        public VariableType CompareVariableType { get; set; } = VariableType.PlayerVariable;

        public Guid CompareVariableId { get; set; }

        public bool ComparingEqual { get; set; }

        public bool Value { get; set; }

    }

    public partial class IntegerVariableComparison : VariableComparison
    {

        public VariableComparator Comparator { get; set; } = VariableComparator.Equal;

        public VariableType CompareVariableType { get; set; } = VariableType.PlayerVariable;

        public Guid CompareVariableId { get; set; }

        public long Value { get; set; }

        public bool TimeSystem { get; set; }

    }

    public partial class StringVariableComparison : VariableComparison
    {

        public StringVariableComparator Comparator { get; set; } = StringVariableComparator.Equal;

        public string Value { get; set; }

    }

    public partial class CheckEquippedSlot : Condition
    {

        public override ConditionTypes Type { get; } = ConditionTypes.CheckEquipment;

        public string Name { get; set; }

    }

}
