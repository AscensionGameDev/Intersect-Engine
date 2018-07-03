using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;

namespace Intersect.GameObjects.Events
{
    public enum ConditionTypes
    {
        PlayerSwitch = 0,
        PlayerVariable,
        ServerSwitch,
        ServerVariable,
        HasItem,
        ClassIs,
        KnowsSpell,
        LevelOrStat,
        SelfSwitch, //Only works for events.. not for checking if you can destroy a resource or something like that
        PowerIs,
        TimeBetween,
        CanStartQuest,
        QuestInProgress,
        QuestCompleted,
        NoNpcsOnMap,
        GenderIs,
    }

    public class Condition
    {
        public ConditionTypes Type { get; set; }
    }

    public class PlayerSwitchCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.PlayerSwitch;
        public Guid SwitchId { get; set; }
        public bool Value { get; set; }
    }

    public class PlayerVariableCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.PlayerVariable;
        public Guid VariableId { get; set; }
        public VariableComparators Comparator { get; set; } = VariableComparators.Equal;
        public int Value { get; set; }
    }

    public class ServerSwitchCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.ServerSwitch;
        public Guid SwitchId { get; set; }
        public bool Value { get; set; }
    }

    public class ServerVariableCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.ServerVariable;
        public Guid VariableId { get; set; }
        public VariableComparators Comparator { get; set; } = VariableComparators.Equal;
        public int Value { get; set; }
    }

    public class HasItemCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.HasItem;
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class ClassIsCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.ClassIs;
        public Guid ClassId { get; set; }
    }

    public class KnowsSpellCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.KnowsSpell;
        public Guid SpellId { get; set; }
    }

    public class LevelOrStatCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.LevelOrStat;
        public bool ComparingLevel { get; set; }
        public Stats Stat { get; set; }
        public VariableComparators Comparator { get; set; } = VariableComparators.Equal;
        public int Value { get; set; }
    }

    public class SelfSwitchCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.SelfSwitch;
        public int SwitchIndex { get; set; } //0 through 3
        public bool Value { get; set; }
    }

    public class PowerIsCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.PowerIs;
        public byte Power { get; set; }
    }

    public class TimeBetweenCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.TimeBetween;
        public int[] Ranges { get; set; } = new int[2];
    }

    public class CanStartQuestCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.CanStartQuest;
        public Guid QuestId { get; set; }
    }

    public class QuestInProgressCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.QuestInProgress;
        public Guid QuestId { get; set; }
        public QuestProgress Progress { get; set; } = QuestProgress.OnAnyTask;
        public Guid TaskId { get; set; }
    }

    public class QuestCompletedCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.QuestCompleted;
        public Guid QuestId { get; set; }
    }

    public class NoNpcsOnMapCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.NoNpcsOnMap;
    }

    public class GenderIsCondition : Condition
    {
        public new ConditionTypes Type = ConditionTypes.GenderIs;
        public byte Gender { get; set; }
    }
}
