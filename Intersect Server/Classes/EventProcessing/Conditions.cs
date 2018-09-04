using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Conditions;
using Intersect.GameObjects.Events;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Maps;

namespace Intersect.Server.Classes.Events
{
    public static class Conditions
    {
        public static bool CanSpawnPage(EventPage page, Player player, EventInstance activeInstance)
        {
            return MeetsConditionLists(page.ConditionLists, player, activeInstance);
        }

        public static bool MeetsConditionLists(ConditionLists lists, Player player, EventInstance eventInstance, bool singleList = true, QuestBase questBase = null)
        {
            if (player == null) return false;
            //If no condition lists then this passes
            if (lists.Lists.Count == 0)
                return true;

            for (int i = 0; i < lists.Lists.Count; i++)
            {
                if (MeetsConditionList(lists.Lists[i], player, eventInstance, questBase))
                    //Checks to see if all conditions in this list are met
                {
                    //If all conditions are met.. and we only need a single list to pass then return true
                    if (singleList)
                        return true;

                    continue;
                }

                //If not.. and we need all lists to pass then return false
                if (!singleList)
                    return false;
            }
            //There were condition lists. If single list was true then we failed every single list and should return false.
            //If single list was false (meaning we needed to pass all lists) then we've made it.. return true.
            return !singleList;
        }

        public static bool MeetsConditionList(ConditionList list, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            for (int i = 0; i < list.Conditions.Count; i++)
            {
                var meetsCondition = MeetsCondition((dynamic)list.Conditions[i], player, eventInstance, questBase);
                if (list.Conditions[i].Negated) meetsCondition = !meetsCondition;
                if (!meetsCondition) return false;
            }
            return true;
        }

        public static bool MeetsCondition(PlayerSwitchCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            var switchVal = player.GetSwitchValue(condition.SwitchId);
            return switchVal == condition.Value;
        }

        public static bool MeetsCondition(PlayerVariableCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            var varVal = player.GetVariableValue(condition.VariableId);
            switch (condition.Comparator) //Comparator
            {
                case VariableComparators.Equal:
                    if (varVal == condition.Value) return true;
                    break;
                case VariableComparators.GreaterOrEqual:
                    if (varVal >= condition.Value) return true;
                    break;
                case VariableComparators.LesserOrEqual:
                    if (varVal <= condition.Value) return true;
                    break;
                case VariableComparators.Greater:
                    if (varVal > condition.Value) return true;
                    break;
                case VariableComparators.Less:
                    if (varVal < condition.Value) return true;
                    break;
                case VariableComparators.NotEqual:
                    if (varVal != condition.Value) return true;
                    break;
            }
            return false;
        }

        public static bool MeetsCondition(ServerSwitchCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            var servSwitch = false;
            if (ServerSwitchBase.Get(condition.SwitchId) != null) servSwitch = ServerSwitchBase.Get(condition.SwitchId).Value;
            return servSwitch == condition.Value;
        }

        public static bool MeetsCondition(ServerVariableCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            var varVal = 0;
            if (ServerVariableBase.Get(condition.VariableId) != null) varVal = ServerVariableBase.Get(condition.VariableId).Value;
            switch (condition.Comparator) //Comparator
            {
                case VariableComparators.Equal:
                    if (varVal == condition.Value) return true;
                    break;
                case VariableComparators.GreaterOrEqual:
                    if (varVal >= condition.Value) return true;
                    break;
                case VariableComparators.LesserOrEqual:
                    if (varVal <= condition.Value) return true;
                    break;
                case VariableComparators.Greater:
                    if (varVal > condition.Value) return true;
                    break;
                case VariableComparators.Less:
                    if (varVal < condition.Value) return true;
                    break;
                case VariableComparators.NotEqual:
                    if (varVal != condition.Value) return true;
                    break;
            }
            return false;
        }

        public static bool MeetsCondition(HasItemCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            if (player.FindItem(condition.ItemId, condition.Quantity) > -1)
            {
                return true;
            }
            return false;
        }

        public static bool MeetsCondition(ClassIsCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            if (player.ClassId == condition.ClassId)
            {
                return true;
            }
            return false;
        }

        public static bool MeetsCondition(KnowsSpellCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            if (player.KnowsSpell(condition.SpellId))
            {
                return true;
            }
            return false;
        }

        public static bool MeetsCondition(LevelOrStatCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            var lvlStat = 0;
            if (condition.ComparingLevel)
            {
                lvlStat = player.Level;
            }
            else
            {
                lvlStat = player.Stat[(int)condition.Stat].Value();
                if (condition.IgnoreBuffs) lvlStat = player.Stat[(int)condition.Stat].Stat;
            }
            switch (condition.Comparator) //Comparator
            {
                case VariableComparators.Equal:
                    if (lvlStat == condition.Value) return true;
                    break;
                case VariableComparators.GreaterOrEqual:
                    if (lvlStat >= condition.Value) return true;
                    break;
                case VariableComparators.LesserOrEqual:
                    if (lvlStat <= condition.Value) return true;
                    break;
                case VariableComparators.Greater:
                    if (lvlStat > condition.Value) return true;
                    break;
                case VariableComparators.Less:
                    if (lvlStat < condition.Value) return true;
                    break;
                case VariableComparators.NotEqual:
                    if (lvlStat != condition.Value) return true;
                    break;
            }
            return false;
        }

        public static bool MeetsCondition(SelfSwitchCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            if (eventInstance != null)
            {
                return eventInstance.SelfSwitch[condition.SwitchIndex] == condition.Value;
            }
            return false;
        }

        public static bool MeetsCondition(PowerIsCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            return player.MyClient.Access > condition.Power;
        }

        public static bool MeetsCondition(TimeBetweenCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            if (condition.Ranges[0] > -1 && condition.Ranges[1] > -1 && condition.Ranges[0] < 1440 / TimeBase.GetTimeBase().RangeInterval && condition.Ranges[1] < 1440 / TimeBase.GetTimeBase().RangeInterval)
            {
                return (ServerTime.GetTimeRange() >= condition.Ranges[0] && ServerTime.GetTimeRange() <= condition.Ranges[1]);
            }
            return true;
        }

        public static bool MeetsCondition(CanStartQuestCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            var startQuest = QuestBase.Get(condition.QuestId);
            if (startQuest == questBase)
            {
                //We cannot check and see if we meet quest requirements if we are already checking to see if we meet quest requirements :P
                return true;
            }
            if (startQuest != null)
            {
                return player.CanStartQuest(startQuest);
            }
            return false;
        }

        public static bool MeetsCondition(QuestInProgressCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            var questInProgress = QuestBase.Get(condition.QuestId);
            if (questInProgress != null)
            {
                return player.QuestInProgress(questInProgress, condition.Progress, condition.TaskId);
            }
            return false;
        }

        public static bool MeetsCondition(QuestCompletedCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            var questCompleted = QuestBase.Get(condition.QuestId);
            if (questCompleted != null)
            {
                return player.QuestCompleted(questCompleted);
            }
            return false;
        }

        public static bool MeetsCondition(NoNpcsOnMapCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            if (eventInstance != null)
            {
                if (eventInstance.NpcDeathTriggerd == true) return false; //Only call it once
                MapInstance m = MapInstance.Get(eventInstance.MapId);
                for (int i = 0; i < m.Spawns.Count; i++)
                {
                    if (m.NpcSpawnInstances.ContainsKey(m.Spawns[i]))
                    {
                        if (m.NpcSpawnInstances[m.Spawns[i]].Entity.Dead == false)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public static bool MeetsCondition(GenderIsCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            return player.Gender == condition.Gender;
        }

        public static bool MeetsCondition(MapIsCondition condition, Player player, EventInstance eventInstance, QuestBase questBase)
        {
            return player.MapId == condition.MapId;
        }
    }
}
