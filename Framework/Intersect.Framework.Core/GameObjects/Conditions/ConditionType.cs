namespace Intersect.Framework.Core.GameObjects.Conditions;

public enum ConditionType
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

    IsInCombat,
}