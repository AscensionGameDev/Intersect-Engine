using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Conditions;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Switches_and_Variables;
using Intersect.Server.General;
using Intersect.Server.Maps;

namespace Intersect.Server.Entities.Events
{
    public static partial class Conditions
    {
        public static bool CanSpawnPage(EventPage page, Player player, Event activeInstance)
        {
            return MeetsConditionLists(page.ConditionLists, player, activeInstance);
        }

        public static bool MeetsConditionLists(
            ConditionLists lists,
            Player player,
            Event eventInstance,
            bool singleList = true,
            QuestBase questBase = null
        )
        {
            if (player == null)
            {
                return false;
            }

            //If no condition lists then this passes
            if (lists.Lists.Count == 0)
            {
                return true;
            }

            for (var i = 0; i < lists.Lists.Count; i++)
            {
                if (MeetsConditionList(lists.Lists[i], player, eventInstance, questBase))

                //Checks to see if all conditions in this list are met
                {
                    //If all conditions are met.. and we only need a single list to pass then return true
                    if (singleList)
                    {
                        return true;
                    }

                    continue;
                }

                //If not.. and we need all lists to pass then return false
                if (!singleList)
                {
                    return false;
                }
            }

            //There were condition lists. If single list was true then we failed every single list and should return false.
            //If single list was false (meaning we needed to pass all lists) then we've made it.. return true.
            return !singleList;
        }

        public static bool MeetsConditionList(
            ConditionList list,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            for (var i = 0; i < list.Conditions.Count; i++)
            {
                var meetsCondition = MeetsCondition(list.Conditions[i], player, eventInstance, questBase);

                if (!meetsCondition)
                {
                    return false;
                }
            }

            return true;
        }

        

        public static bool MeetsCondition(
            Condition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            var result = ConditionHandlerRegistry.CheckCondition(condition, player, eventInstance, questBase);
            if (condition.Negated)
            {
                result = !result;
            }
            return result;
        }

        public static bool MeetsCondition(
            VariableIsCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            VariableValue value = null;
            if (condition.VariableType == VariableTypes.PlayerVariable)
            {
                value = player.GetVariableValue(condition.VariableId);
            }
            else if (condition.VariableType == VariableTypes.ServerVariable)
            {
                value = ServerVariableBase.Get(condition.VariableId)?.Value;
            }

            if (value == null)
            {
                value = new VariableValue();
            }

            return CheckVariableComparison(value, condition.Comparison, player, eventInstance);
        }

        public static bool MeetsCondition(
            HasItemCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            var quantity = condition.Quantity;
            if (condition.UseVariable)
            {
                switch (condition.VariableType)
                {
                    case VariableTypes.PlayerVariable:
                        quantity = (int)player.GetVariableValue(condition.VariableId).Integer;

                        break;
                    case VariableTypes.ServerVariable:
                        quantity = (int)ServerVariableBase.Get(condition.VariableId)?.Value.Integer;
                        break;
                }
            }

            return player.CountItems(condition.ItemId) >= quantity;
        }

        public static bool MeetsCondition(
            ClassIsCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            if (player.ClassId == condition.ClassId)
            {
                return true;
            }

            return false;
        }

        public static bool MeetsCondition(
            KnowsSpellCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            if (player.KnowsSpell(condition.SpellId))
            {
                return true;
            }

            return false;
        }

        public static bool MeetsCondition(
            LevelOrStatCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            var lvlStat = 0;
            if (condition.ComparingLevel)
            {
                lvlStat = player.Level;
            }
            else
            {
                lvlStat = player.Stat[(int)condition.Stat].Value();
                if (condition.IgnoreBuffs)
                {
                    lvlStat = player.Stat[(int)condition.Stat].BaseStat +
                              player.StatPointAllocations[(int)condition.Stat];
                }
            }

            switch (condition.Comparator) //Comparator
            {
                case VariableComparators.Equal:
                    if (lvlStat == condition.Value)
                    {
                        return true;
                    }

                    break;
                case VariableComparators.GreaterOrEqual:
                    if (lvlStat >= condition.Value)
                    {
                        return true;
                    }

                    break;
                case VariableComparators.LesserOrEqual:
                    if (lvlStat <= condition.Value)
                    {
                        return true;
                    }

                    break;
                case VariableComparators.Greater:
                    if (lvlStat > condition.Value)
                    {
                        return true;
                    }

                    break;
                case VariableComparators.Less:
                    if (lvlStat < condition.Value)
                    {
                        return true;
                    }

                    break;
                case VariableComparators.NotEqual:
                    if (lvlStat != condition.Value)
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }

        public static bool MeetsCondition(
            SelfSwitchCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            if (eventInstance != null)
            {
                if (eventInstance.Global)
                {
                    if (MapInstance.Get(eventInstance.MapId).GlobalEventInstances.TryGetValue(eventInstance.BaseEvent, out Event evt))
                    {
                        if (evt != null)
                        {
                            return evt.SelfSwitch[condition.SwitchIndex] == condition.Value;
                        }
                    }
                }
                else
                {
                    return eventInstance.SelfSwitch[condition.SwitchIndex] == condition.Value;
                }
            }

            return false;
        }

        public static bool MeetsCondition(
            AccessIsCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            var power = player.Power;
            if (condition.Access == 0)
            {
                return power.Ban || power.Kick || power.Mute;
            }
            else if (condition.Access > 0)
            {
                return power.Editor;
            }

            return false;
        }

        public static bool MeetsCondition(
            TimeBetweenCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            if (condition.Ranges[0] > -1 &&
                condition.Ranges[1] > -1 &&
                condition.Ranges[0] < 1440 / TimeBase.GetTimeBase().RangeInterval &&
                condition.Ranges[1] < 1440 / TimeBase.GetTimeBase().RangeInterval)
            {
                return Time.GetTimeRange() >= condition.Ranges[0] && Time.GetTimeRange() <= condition.Ranges[1];
            }

            return true;
        }

        public static bool MeetsCondition(
            CanStartQuestCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
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

        public static bool MeetsCondition(
            QuestInProgressCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            return player.QuestInProgress(condition.QuestId, condition.Progress, condition.TaskId);
        }

        public static bool MeetsCondition(
            QuestCompletedCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            return player.QuestCompleted(condition.QuestId);
        }

        public static bool MeetsCondition(
            NoNpcsOnMapCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            var map = MapInstance.Get(eventInstance?.MapId ?? Guid.Empty);
            if (map == null)
            {
                map = MapInstance.Get(player.MapId);
            }

            if (map != null)
            {
                var entities = map.GetEntities();
                foreach (var en in entities)
                {
                    if (en.GetType() == typeof(Npc))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static bool MeetsCondition(
            GenderIsCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            return player.Gender == condition.Gender;
        }

        public static bool MeetsCondition(
            MapIsCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            return player.MapId == condition.MapId;
        }

        public static bool MeetsCondition(
            IsItemEquippedCondition condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (player.Equipment[i] >= 0)
                {
                    if (player.Items[player.Equipment[i]].ItemId == condition.ItemId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool MeetsCondition(
            HasFreeInventorySlots condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {

            var quantity = condition.Quantity;
            if (condition.UseVariable)
            {
                switch (condition.VariableType)
                {
                    case VariableTypes.PlayerVariable:
                        quantity = (int)player.GetVariableValue(condition.VariableId).Integer;

                        break;
                    case VariableTypes.ServerVariable:
                        quantity = (int)ServerVariableBase.Get(condition.VariableId)?.Value.Integer;
                        break;
                }
            }

            // Check if the user has (or does not have when negated) the desired amount of inventory slots.
            var slots = player.FindOpenInventorySlots().Count;

            return slots >= quantity;
        }

        public static bool MeetsCondition(
            InGuildWithRank condition,
            Player player,
            Event eventInstance,
            QuestBase questBase
        )
        {
            return player.Guild != null && player.GuildRank <= condition.Rank;
        }

        public static bool MeetsCondition(
            MapZoneTypeIs condition,
            Player player,
            Event eventInstance,
            QuestBase questBase)
        {
            return player.Map?.ZoneType == condition.ZoneType;
        }

        //Variable Comparison Processing

        public static bool CheckVariableComparison(
            VariableValue currentValue,
            VariableCompaison comparison,
            Player player,
            Event instance
        )
        {
            return VariableCheckHandlerRegistry.CheckVariableComparison(currentValue, comparison, player, instance);
        }

        public static bool CheckVariableComparison(
            VariableValue currentValue,
            BooleanVariableComparison comparison,
            Player player,
            Event instance
        )
        {
            VariableValue compValue = null;
            if (comparison.CompareVariableId != Guid.Empty)
            {
                if (comparison.CompareVariableType == VariableTypes.PlayerVariable)
                {
                    compValue = player.GetVariableValue(comparison.CompareVariableId);
                }
                else if (comparison.CompareVariableType == VariableTypes.ServerVariable)
                {
                    compValue = ServerVariableBase.Get(comparison.CompareVariableId)?.Value;
                }
            }
            else
            {
                compValue = new VariableValue();
                compValue.Boolean = comparison.Value;
            }

            if (compValue == null)
            {
                compValue = new VariableValue();
            }

            if (currentValue.Type == 0)
            {
                currentValue.Boolean = false;
            }

            if (compValue.Type != currentValue.Type)
            {
                return false;
            }

            if (comparison.ComparingEqual)
            {
                return currentValue.Boolean == compValue.Boolean;
            }
            else
            {
                return currentValue.Boolean != compValue.Boolean;
            }
        }

        public static bool CheckVariableComparison(
            VariableValue currentValue,
            IntegerVariableComparison comparison,
            Player player,
            Event instance
        )
        {
            long compareAgainst = 0;

            VariableValue compValue = null;
            if (comparison.CompareVariableId != Guid.Empty)
            {
                if (comparison.CompareVariableType == VariableTypes.PlayerVariable)
                {
                    compValue = player.GetVariableValue(comparison.CompareVariableId);
                }
                else if (comparison.CompareVariableType == VariableTypes.ServerVariable)
                {
                    compValue = ServerVariableBase.Get(comparison.CompareVariableId)?.Value;
                }
            }
            else
            {
                compValue = new VariableValue();
                compValue.Integer = comparison.Value;
            }

            if (compValue == null)
            {
                compValue = new VariableValue();
            }

            if (currentValue.Type == 0)
            {
                currentValue.Integer = 0;
            }

            if (compValue.Type != currentValue.Type)
            {
                return false;
            }

            var varVal = currentValue.Integer;
            compareAgainst = compValue.Integer;

            switch (comparison.Comparator) //Comparator
            {
                case VariableComparators.Equal:
                    if (varVal == compareAgainst)
                    {
                        return true;
                    }

                    break;
                case VariableComparators.GreaterOrEqual:
                    if (varVal >= compareAgainst)
                    {
                        return true;
                    }

                    break;
                case VariableComparators.LesserOrEqual:
                    if (varVal <= compareAgainst)
                    {
                        return true;
                    }

                    break;
                case VariableComparators.Greater:
                    if (varVal > compareAgainst)
                    {
                        return true;
                    }

                    break;
                case VariableComparators.Less:
                    if (varVal < compareAgainst)
                    {
                        return true;
                    }

                    break;
                case VariableComparators.NotEqual:
                    if (varVal != compareAgainst)
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }

        public static bool CheckVariableComparison(
            VariableValue currentValue,
            StringVariableComparison comparison,
            Player player,
            Event instance
        )
        {
            var varVal = CommandProcessing.ParseEventText(currentValue.String ?? "", player, instance);
            var compareAgainst = CommandProcessing.ParseEventText(comparison.Value ?? "", player, instance);

            switch (comparison.Comparator)
            {
                case StringVariableComparators.Equal:
                    return varVal == compareAgainst;
                case StringVariableComparators.Contains:
                    return varVal.Contains(compareAgainst);
            }

            return false;
        }

    }

}
