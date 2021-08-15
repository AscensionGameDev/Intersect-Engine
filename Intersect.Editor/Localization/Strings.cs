using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Localization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.Editor.Localization
{

    public static partial class Strings
    {

        public static string GetEventConditionalDesc(VariableIsCondition condition)
        {
            var pVar = GetVariableComparisonString((dynamic) condition.Comparison);

            if (condition.VariableType == VariableTypes.PlayerVariable)
            {
                return Strings.EventConditionDesc.playervariable.ToString(
                    PlayerVariableBase.GetName(condition.VariableId), pVar
                );
            }
            else if (condition.VariableType == VariableTypes.ServerVariable)
            {
                return Strings.EventConditionDesc.globalvariable.ToString(
                    ServerVariableBase.GetName(condition.VariableId), pVar
                );
            }

            return "";
        }

        public static string GetEventConditionalDesc(HasItemCondition condition)
        {
            if (condition.UseVariable)
            {
                var amount = string.Empty;
                switch (condition.VariableType)
                {
                    case VariableTypes.PlayerVariable:
                        amount = string.Format(@"({0}: {1})", Strings.EventConditional.playervariable, PlayerVariableBase.GetName(condition.VariableId));
                        break;
                    case VariableTypes.ServerVariable:
                        amount = string.Format(@"({0}: {1})", Strings.EventConditional.globalvariable, ServerVariableBase.GetName(condition.VariableId));
                        break;
                }

                return Strings.EventConditionDesc.hasitem.ToString(amount, ItemBase.GetName(condition.ItemId));
            }
            else
            {
                return Strings.EventConditionDesc.hasitem.ToString(condition.Quantity, ItemBase.GetName(condition.ItemId));
            }
        }

        public static string GetEventConditionalDesc(IsItemEquippedCondition condition)
        {
            return Strings.EventConditionDesc.hasitemequipped.ToString(ItemBase.GetName(condition.ItemId));
        }

        public static string GetEventConditionalDesc(ClassIsCondition condition)
        {
            return Strings.EventConditionDesc.Class.ToString(ClassBase.GetName(condition.ClassId));
        }

        public static string GetEventConditionalDesc(KnowsSpellCondition condition)
        {
            return Strings.EventConditionDesc.knowsspell.ToString(SpellBase.GetName(condition.SpellId));
        }

        public static string GetEventConditionalDesc(LevelOrStatCondition condition)
        {
            var pLvl = "";
            switch (condition.Comparator)
            {
                case VariableComparators.Equal:
                    pLvl = Strings.EventConditionDesc.equal.ToString(condition.Value);

                    break;
                case VariableComparators.GreaterOrEqual:
                    pLvl = Strings.EventConditionDesc.greaterequal.ToString(condition.Value);

                    break;
                case VariableComparators.LesserOrEqual:
                    pLvl = Strings.EventConditionDesc.lessthanequal.ToString(condition.Value);

                    break;
                case VariableComparators.Greater:
                    pLvl = Strings.EventConditionDesc.greater.ToString(condition.Value);

                    break;
                case VariableComparators.Less:
                    pLvl = Strings.EventConditionDesc.lessthan.ToString(condition.Value);

                    break;
                case VariableComparators.NotEqual:
                    pLvl = Strings.EventConditionDesc.notequal.ToString(condition.Value);

                    break;
            }

            var lvlorstat = "";
            if (condition.ComparingLevel)
            {
                lvlorstat = Strings.EventConditionDesc.level;
            }
            else
            {
                lvlorstat = Strings.Combat.stats[(int) condition.Stat];
            }

            return Strings.EventConditionDesc.levelorstat.ToString(lvlorstat, pLvl);
        }

        public static string GetEventConditionalDesc(SelfSwitchCondition condition)
        {
            var sValue = Strings.EventConditionDesc.False;
            if (condition.Value)
            {
                sValue = Strings.EventConditionDesc.True;
            }

            return Strings.EventConditionDesc.selfswitch.ToString(
                Strings.EventConditionDesc.selfswitches[condition.SwitchIndex], sValue
            );
        }

        public static string GetEventConditionalDesc(AccessIsCondition condition)
        {
            if (condition.Access == Access.None)
            {
                return Strings.EventConditionDesc.power.ToString(Strings.EventConditionDesc.modadmin);
            }
            else
            {
                return Strings.EventConditionDesc.power.ToString(Strings.EventConditionDesc.admin);
            }
        }

        public static string GetEventConditionalDesc(TimeBetweenCondition condition)
        {
            var timeRanges = new List<string>();
            var time = new DateTime(2000, 1, 1, 0, 0, 0);
            for (var i = 0; i < 1440; i += TimeBase.GetTimeBase().RangeInterval)
            {
                var addRange = time.ToString("h:mm:ss tt") + " to ";
                time = time.AddMinutes(TimeBase.GetTimeBase().RangeInterval);
                addRange += time.ToString("h:mm:ss tt");
                timeRanges.Add(addRange);
            }

            var time1 = "";
            var time2 = "";
            if (condition.Ranges[0] > -1 && condition.Ranges[0] < timeRanges.Count)
            {
                time1 = timeRanges[condition.Ranges[0]];
            }
            else
            {
                time1 = Strings.EventConditionDesc.timeinvalid;
            }

            if (condition.Ranges[1] > -1 && condition.Ranges[1] < timeRanges.Count)
            {
                time2 = timeRanges[condition.Ranges[1]];
            }
            else
            {
                time2 = Strings.EventConditionDesc.timeinvalid;
            }

            return Strings.EventConditionDesc.time.ToString(time1, time2);
        }

        public static string GetEventConditionalDesc(CanStartQuestCondition condition)
        {
            return Strings.EventConditionDesc.startquest.ToString(QuestBase.GetName(condition.QuestId));
        }

        public static string GetEventConditionalDesc(QuestInProgressCondition condition)
        {
            var quest = QuestBase.Get(condition.QuestId);
            if (quest != null)
            {
                QuestBase.QuestTask task = null;
                foreach (var tsk in quest.Tasks)
                {
                    if (tsk.Id == condition.TaskId)
                    {
                        task = tsk;
                    }
                }

                var taskName = task != null
                    ? task.GetTaskString(Strings.TaskEditor.descriptions)
                    : Strings.EventConditionDesc.tasknotfound.ToString();

                switch (condition.Progress)
                {
                    case QuestProgressState.BeforeTask:
                        return Strings.EventConditionDesc.questinprogress.ToString(
                            QuestBase.GetName(condition.QuestId),
                            Strings.EventConditionDesc.beforetask.ToString(taskName)
                        );
                    case QuestProgressState.AfterTask:
                        return Strings.EventConditionDesc.questinprogress.ToString(
                            QuestBase.GetName(condition.QuestId),
                            Strings.EventConditionDesc.aftertask.ToString(taskName)
                        );
                    case QuestProgressState.OnTask:
                        return Strings.EventConditionDesc.questinprogress.ToString(
                            QuestBase.GetName(condition.QuestId), Strings.EventConditionDesc.ontask.ToString(taskName)
                        );
                    default: //On Any task
                        return Strings.EventConditionDesc.questinprogress.ToString(
                            QuestBase.GetName(condition.QuestId), Strings.EventConditionDesc.onanytask
                        );
                }
            }

            return Strings.EventConditionDesc.questinprogress.ToString(QuestBase.GetName(condition.QuestId));
        }

        public static string GetEventConditionalDesc(QuestCompletedCondition condition)
        {
            return Strings.EventConditionDesc.questcompleted.ToString(QuestBase.GetName(condition.QuestId));
        }

        public static string GetEventConditionalDesc(NoNpcsOnMapCondition condition)
        {
            return Strings.EventConditionDesc.nonpcsonmap;
        }

        public static string GetEventConditionalDesc(GenderIsCondition condition)
        {
            return Strings.EventConditionDesc.gender.ToString(
                condition.Gender == 0 ? Strings.EventConditionDesc.male : Strings.EventConditionDesc.female
            );
        }

        public static string GetEventConditionalDesc(MapIsCondition condition)
        {
            var map = Intersect.GameObjects.Maps.MapList.MapList.List.FindMap(condition.MapId);
            if (map != null)
            {
                return Strings.EventConditionDesc.map.ToString(map.Name);
            }

            return Strings.EventConditionDesc.map.ToString(EventConditionDesc.mapnotfound);
        }

        public static string GetEventConditionalDesc(InGuildWithRank condition)
        {
            return Strings.EventConditionDesc.guild.ToString(Intersect.Options.Instance.Guild.Ranks[Math.Max(0, Math.Min(Intersect.Options.Instance.Guild.Ranks.Length - 1, condition.Rank))].Title);
        }

        public static string GetEventConditionalDesc(HasFreeInventorySlots condition)
        {
            if (condition.UseVariable)
            {
                var amount = string.Empty;
                switch (condition.VariableType)
                {
                    case VariableTypes.PlayerVariable:
                        amount = string.Format(@"({0}: {1})", Strings.EventConditional.playervariable, PlayerVariableBase.GetName(condition.VariableId));
                        break;
                    case VariableTypes.ServerVariable:
                        amount = string.Format(@"({0}: {1})", Strings.EventConditional.globalvariable, ServerVariableBase.GetName(condition.VariableId));
                        break;
                }

                return Strings.EventConditionDesc.HasFreeInventorySlots.ToString(amount);
            }
            else
            {
                return Strings.EventConditionDesc.HasFreeInventorySlots.ToString(condition.Quantity);
            }
        }

        public static string GetEventConditionalDesc(MapZoneTypeIs condition)
        {
            return Strings.EventConditionDesc.MapZoneTypeIs.ToString(Strings.MapProperties.zones[(int)condition.ZoneType]);
        }

        public static string GetVariableComparisonString(VariableCompaison comparison)
        {
            return "";
        }

        public static string GetVariableComparisonString(BooleanVariableComparison comparison)
        {
            var value = "";
            var pVar = "";

            if (comparison.CompareVariableId == Guid.Empty)
            {
                value = comparison.Value.ToString();
            }
            else
            {
                if (comparison.CompareVariableType == VariableTypes.PlayerVariable)
                {
                    value = Strings.EventConditionDesc.playervariablevalue.ToString(
                        PlayerVariableBase.GetName(comparison.CompareVariableId)
                    );
                }
                else if (comparison.CompareVariableType == VariableTypes.ServerVariable)
                {
                    value = Strings.EventConditionDesc.globalvariablevalue.ToString(
                        ServerVariableBase.GetName(comparison.CompareVariableId)
                    );
                }
            }

            if (comparison.ComparingEqual)
            {
                pVar = Strings.EventConditionDesc.equal.ToString(value);
            }
            else
            {
                pVar = Strings.EventConditionDesc.notequal.ToString(value);
            }

            return pVar;
        }

        public static string GetVariableComparisonString(IntegerVariableComparison comparison)
        {
            var value = "";
            var pVar = "";

            if (comparison.CompareVariableId == Guid.Empty)
            {
                value = comparison.Value.ToString();
            }
            else
            {
                if (comparison.CompareVariableType == VariableTypes.PlayerVariable)
                {
                    value = Strings.EventConditionDesc.playervariablevalue.ToString(
                        PlayerVariableBase.GetName(comparison.CompareVariableId)
                    );
                }
                else if (comparison.CompareVariableType == VariableTypes.ServerVariable)
                {
                    value = Strings.EventConditionDesc.globalvariablevalue.ToString(
                        ServerVariableBase.GetName(comparison.CompareVariableId)
                    );
                }
            }

            switch (comparison.Comparator)
            {
                case VariableComparators.Equal:
                    pVar = Strings.EventConditionDesc.equal.ToString(value);

                    break;
                case VariableComparators.GreaterOrEqual:
                    pVar = Strings.EventConditionDesc.greaterequal.ToString(value);

                    break;
                case VariableComparators.LesserOrEqual:
                    pVar = Strings.EventConditionDesc.lessthanequal.ToString(value);

                    break;
                case VariableComparators.Greater:
                    pVar = Strings.EventConditionDesc.greater.ToString(value);

                    break;
                case VariableComparators.Less:
                    pVar = Strings.EventConditionDesc.lessthan.ToString(value);

                    break;
                case VariableComparators.NotEqual:
                    pVar = Strings.EventConditionDesc.notequal.ToString(value);

                    break;
            }

            return pVar;
        }

        public static string GetVariableComparisonString(StringVariableComparison comparison)
        {
            switch (comparison.Comparator)
            {
                case StringVariableComparators.Equal:
                    return Strings.EventConditionDesc.equal.ToString(comparison.Value);
                case StringVariableComparators.Contains:
                    return Strings.EventConditionDesc.contains.ToString(comparison.Value);
            }

            return "";
        }

        public static void Load()
        {
            if (File.Exists(Path.Combine("resources", "editor_strings.json")))
            {
                var strings = new Dictionary<string, Dictionary<string, object>>();
                strings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(
                    File.ReadAllText(Path.Combine("resources", "editor_strings.json"))
                );

                var type = typeof(Strings);

                var fields = new List<Type>();
                fields.AddRange(
                    type.GetNestedTypes(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                );

                foreach (var p in fields)
                {
                    if (!strings.ContainsKey(p.Name))
                    {
                        continue;
                    }

                    var dict = strings[p.Name];
                    foreach (var fieldInfo in p.GetFields(
                        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
                    ))
                    {
                        var fieldValue = fieldInfo.GetValue(null);
                        if (!dict.ContainsKey(fieldInfo.Name.ToLower()))
                        {
                            continue;
                        }

                        if (fieldValue is LocalizedString)
                        {
                            fieldInfo.SetValue(null, new LocalizedString((string) dict[fieldInfo.Name.ToLower()]));
                        }
                        else if (fieldValue is Dictionary<int, LocalizedString>)
                        {
                            var existingDict = (Dictionary<int, LocalizedString>) fieldInfo.GetValue(null);
                            var values = ((JObject) dict[fieldInfo.Name]).ToObject<Dictionary<int, string>>();
                            var dic = values.ToDictionary<KeyValuePair<int, string>, int, LocalizedString>(
                                val => val.Key, val => val.Value
                            );

                            foreach (var val in dic)
                            {
                                existingDict[val.Key] = val.Value;
                            }
                        }
                        else if (fieldValue is Dictionary<string, LocalizedString>)
                        {
                            var existingDict = (Dictionary<string, LocalizedString>) fieldInfo.GetValue(null);
                            var pairs = ((JObject) dict[fieldInfo.Name])?.ToObject<Dictionary<string, string>>() ??
                                        new Dictionary<string, string>();

                            foreach (var pair in pairs)
                            {
                                if (pair.Key == null)
                                {
                                    continue;
                                }

                                existingDict[pair.Key.ToLower()] = pair.Value;
                            }
                        }
                    }
                }
            }

            Save();
        }

        public static void Save()
        {
            var strings = new Dictionary<string, Dictionary<string, object>>();
            var type = typeof(Strings);
            var fields = type.GetNestedTypes(
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
            );

            foreach (var p in fields)
            {
                var dict = new Dictionary<string, object>();
                foreach (var p1 in p.GetFields(
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
                ))
                {
                    if (p1.GetValue(null).GetType() == typeof(LocalizedString))
                    {
                        dict.Add(p1.Name.ToLower(), ((LocalizedString) p1.GetValue(null)).ToString());
                    }
                    else if (p1.GetValue(null).GetType() == typeof(Dictionary<int, LocalizedString>))
                    {
                        var dic = new Dictionary<int, string>();
                        foreach (var val in (Dictionary<int, LocalizedString>) p1.GetValue(null))
                        {
                            dic.Add(val.Key, val.Value.ToString());
                        }

                        dict.Add(p1.Name, dic);
                    }
                    else if (p1.GetValue(null).GetType() == typeof(Dictionary<string, LocalizedString>))
                    {
                        var dic = new Dictionary<string, string>();
                        foreach (var val in (Dictionary<string, LocalizedString>) p1.GetValue(null))
                        {
                            dic.Add(val.Key.ToLower(), val.Value.ToString());
                        }

                        dict.Add(p1.Name, dic);
                    }
                }

                strings.Add(p.Name, dict);
            }

            var languageDirectory = Path.Combine("resources");
            if (Directory.Exists(languageDirectory))
            {
                File.WriteAllText(
                    Path.Combine(languageDirectory, "editor_strings.json"),
                    JsonConvert.SerializeObject(strings, Formatting.Indented)
                );
            }
        }

        public struct About
        {

            public static LocalizedString site =
                @"Click here to visit the Ascension Game Dev community for support, updates and more!";

            public static LocalizedString title = @"About";

            public static LocalizedString version = @"v{00}";

        }

        public struct AnimationEditor
        {

            public static LocalizedString animations = @"Animations";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString copy = @"Copy Animation";

            public static LocalizedString delete = @"Delete Animation";

            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete this animation? This action cannot be reverted!";

            public static LocalizedString deletetitle = @"Delete Animation";

            public static LocalizedString folderlabel = @"Folder:";

            public static LocalizedString foldertitle = @"Add Folder";

            public static LocalizedString folderprompt = @"Enter a name for the folder you'd like to add:";

            public static LocalizedString general = @"General";

            public static LocalizedString lowerclone = @"Clone From Previous";

            public static LocalizedString lowerframe = @"Frame: {00}";

            public static LocalizedString lowerframecount = @"Graphic Frame Count:";

            public static LocalizedString lowerframeduration = @"Frame Duration (ms):";

            public static LocalizedString lowerframeoptions = @"Frame Options";

            public static LocalizedString lowergraphic = @"Graphic:";

            public static LocalizedString lowergroup = @"Lower Layer (Below Target) ";

            public static LocalizedString lowerhorizontalframes = @"Graphic Horizontal Frames:";

            public static LocalizedString lowerloopcount = @"Loop Count:";

            public static LocalizedString disablelowerrotations = @"Disable Rotations";

            public static LocalizedString lowerplay = @"Play Lower Animation";

            public static LocalizedString lowerplayback = @"Playback";

            public static LocalizedString lowerstop = @"Stop Lower Animation";

            public static LocalizedString lowerverticalframes = @"Graphic Vertical Frames:";

            public static LocalizedString name = @"Name:";

            public static LocalizedString New = @"New Animation";

            public static LocalizedString paste = @"Paste Animation";

            public static LocalizedString renderaboveplayer = @"Render Above Player";

            public static LocalizedString renderbelowfringe = @"Render Below Fringe";

            public static LocalizedString save = @"Save";

            public static LocalizedString searchplaceholder = @"Search...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sortalphabetically = @"Order Alphabetically";

            public static LocalizedString simulatedarkness = @"Simulate Darkness: {00}";

            public static LocalizedString sound = @"Sound:";

            public static LocalizedString soundcomplete = @"Complete Sound Playback After Anim Dies";

            public static LocalizedString swap = @"Swap Upper/Lower";

            public static LocalizedString title = @"Animation Editor";

            public static LocalizedString undo = @"Undo Changes";

            public static LocalizedString undoprompt =
                @"Are you sure you want to undo changes made to this animation? This action cannot be reverted!";

            public static LocalizedString undotitle = @"Undo Changes";

            public static LocalizedString upperclone = @"Clone From Previous";

            public static LocalizedString upperframe = @"Frame: {00}";

            public static LocalizedString upperframecount = @"Graphic Frame Count:";

            public static LocalizedString disableupperrotations = @"Disable Rotations";

            public static LocalizedString upperframeduration = @"Frame Duration (ms):";

            public static LocalizedString upperframeoptions = @"Frame Options";

            public static LocalizedString uppergraphic = @"Graphic:";

            public static LocalizedString uppergroup = @"Upper Layer (Above Target) ";

            public static LocalizedString upperhorizontalframes = @"Graphic Horizontal Frames:";

            public static LocalizedString upperloopcount = @"Loop Count:";

            public static LocalizedString upperplay = @"Play Upper Animation";

            public static LocalizedString upperplayback = @"Playback";

            public static LocalizedString upperstop = @"Stop Upper Animation";

            public static LocalizedString upperverticalframes = @"Graphic Vertical Frames:";

        }

        public struct Attributes
        {

            public static LocalizedString animation = @"Animation:";

            public static LocalizedString blocked = @"Blocked";

            public static LocalizedString critter = @"Critter";

            public static Dictionary<int, LocalizedString> crittermovements = new Dictionary<int, LocalizedString>
            {
                {0, @"Move Randomly"},
                {1, @"Turn Randomly"},
                {2, @"Stand Still"},
            };

            public static Dictionary<int, LocalizedString> critterlayers = new Dictionary<int, LocalizedString>
            {
                {0, @"Below Player"},
                {1, @"Same as Player"},
                {2, @"Above Player"},
            };

            public static LocalizedString crittersprite = @"Sprite:";

            public static LocalizedString critteranimation = @"Animation:";

            public static LocalizedString crittermovement = @"Movement:";

            public static LocalizedString critterlayer = @"Layer:";

            public static LocalizedString critterspeed = @"Speed (ms):";

            public static LocalizedString critterfrequency = @"Freq (ms):";

            public static LocalizedString critterignorenpcavoids = @"Ignore Npc Avoids";

            public static LocalizedString critterblockplayers = @"Block Players";

            public static LocalizedString critterdirection = @"Direction:";

            public static LocalizedString dir = @"Dir:";

            public static LocalizedString distance = @"Distance (In Tiles):";

            public static LocalizedString grapple = @"Grapple Stone";

            public static LocalizedString item = @"Item:";

            public static LocalizedString itemspawn = @"Item Spawn";

            public static LocalizedString mapanimation = @"Animation";

            public static LocalizedString mapanimationblock = @"Block Tile";

            public static LocalizedString mapsound = @"Map Sound";

            public static LocalizedString npcavoid = @"NPC Avoid";

            public static LocalizedString quantity = @"Quantity:";

            public static LocalizedString resource = @"Resource:";

            public static LocalizedString resourcespawn = @"Resource";

            public static LocalizedString slide = @"Slide";

            public static LocalizedString sound = @"Sound:";

            public static LocalizedString warp = @"Warp";

            public static LocalizedString zblock = @"Block";

            public static LocalizedString zdimension = @"Z-Dimension";

            public static LocalizedString zgateway = @"Gateway";

            public static LocalizedString zlevel1 = @"Level 1";

            public static LocalizedString zlevel2 = @"Level 2";

            public static LocalizedString znone = @"None";

        }

        public struct ClassEditor
        {

            public static LocalizedString abilitypowerboost = @"Ability Pwr (+{00}):";

            public static LocalizedString addicon = @"+";

            public static LocalizedString addspell = @"Add";

            public static LocalizedString armorboost = @"Armor (+{00}):";

            public static LocalizedString attackanimation = @"Attack Animation:";

            public static LocalizedString attackboost = @"Attack (+{00}):";

            public static LocalizedString attackspeed = @"Attack Speed";

            public static LocalizedString attackspeedmodifier = @"Modifier:";

            public static Dictionary<int, LocalizedString> attackspeedmodifiers = new Dictionary<int, LocalizedString>
            {
                {0, @"Disabled"},
                {1, @"Static (ms)"},
            };

            public static LocalizedString attackspeedvalue = @"Value:";

            public static LocalizedString baseabilitypower = @"Ability Pwr:";

            public static LocalizedString basearmor = @"Armor:";

            public static LocalizedString baseattack = @"Attack:";

            public static LocalizedString basedamage = @"Base Damage:";

            public static LocalizedString basehp = @"HP:";

            public static LocalizedString basemagicresist = @"Magic Resist:";

            public static LocalizedString basemp = @"Mana:";

            public static LocalizedString basepoints = @"Points:";

            public static LocalizedString basespeed = @"Speed:";

            public static LocalizedString basestats = @"Base Stats:";

            public static LocalizedString boostpercent = @" %";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString classes = @"Classes";

            public static LocalizedString combat = @"Combat (Unarmed)";

            public static LocalizedString copy = @"Copy Class";

            public static LocalizedString critchance = @"Crit Chance (%):";

            public static LocalizedString critmultiplier = @"Crit Multiplier (Default 1.5x):";

            public static LocalizedString damagetype = @"Damage Type:";

            public static LocalizedString delete = @"Delete Class";

            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete this class? This action cannot be reverted!";

            public static LocalizedString deletetitle = @"Delete Class";

            public static LocalizedString expgrid = "Exp Grid";

            public static LocalizedString experiencegrid = "Experience Grid";

            public static LocalizedString face = @"Face:";

            public static LocalizedString female = @"Female";

            public static LocalizedString folderlabel = @"Folder:";

            public static LocalizedString foldertitle = @"Add Folder";

            public static LocalizedString folderprompt = @"Enter a name for the folder you'd like to add:";

            public static LocalizedString gender = @"Gender";

            public static LocalizedString general = @"General";

            public static LocalizedString gridlevel = "Level";

            public static LocalizedString gridpaste = "Paste";

            public static LocalizedString gridtnl = "Exp TNL";

            public static LocalizedString gridtotalexp = "Total Exp";

            public static LocalizedString gridreset = "Reset";

            public static LocalizedString gridclose = "Close";

            public static LocalizedString hpboost = @"Max HP (+{00}):";

            public static LocalizedString hpregen = @"HP (%);";

            public static LocalizedString learntspells = @"Spells";

            public static LocalizedString levelboosts = @"Level Up Boosts";

            public static LocalizedString levelexp = @"Base Exp to Level:";

            public static LocalizedString levelexpscale = @"Exp Increase (Per Level %):";

            public static LocalizedString leveling = @"Leveling Up";

            public static LocalizedString locked = @"Locked";

            public static LocalizedString magicresistboost = @"Magic Resist (+{00}):";

            public static LocalizedString male = @"Male";

            public static LocalizedString mpboost = @"Max MP (+{00}):";

            public static LocalizedString mpregen = @"MP (%):";

            public static LocalizedString name = @"Name:";

            public static LocalizedString New = @"New Class";

            public static LocalizedString paste = @"Paste Class";

            public static LocalizedString percentageboost = @"Percentage";

            public static LocalizedString pointsboost = @"Points (+):";

            public static LocalizedString regen = @"Regen";

            public static LocalizedString regenhint = @"% of HP/Mana to restore per tick.

Tick timer saved in server config.json.";

            public static LocalizedString removeicon = @"-";

            public static LocalizedString removespell = @"Remove";

            public static LocalizedString save = @"Save";

            public static LocalizedString scalingamount = @"Scaling Amount (%):";

            public static LocalizedString scalingstat = @"Scaling Stat:";

            public static LocalizedString searchplaceholder = @"Search...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sortalphabetically = @"Order Alphabetically";

            public static LocalizedString spawnitems = @"Spawn Items";

            public static LocalizedString spawnitem = @"Item:";

            public static LocalizedString spawnamount = @"Amount:";

            public static LocalizedString spawnitemadd = @"Add";

            public static LocalizedString spawnitemremove = @"Remove";

            public static LocalizedString spawnitemdisplay = @"{00} x{01}";

            public static LocalizedString spawnpoint = @"Spawn Point";

            public static LocalizedString speedboost = @"Speed (+{00}):";

            public static LocalizedString spell = @"Spell:";

            public static LocalizedString spellitem = @"{00}. {01} - Level: {02}";

            public static LocalizedString spelllevel = @"Level:";

            public static LocalizedString sprite = @"Sprite:";

            public static LocalizedString spriteface = @"Sprite and Face";

            public static LocalizedString spriteitemfemale = @"{00}. {01} - F";

            public static LocalizedString spriteitemmale = @"{00}. {01} - M";

            public static LocalizedString spriteoptions = @"Options:";

            public static LocalizedString staticboost = @"Static";

            public static LocalizedString title = @"Class Editor";

            public static LocalizedString undo = @"Undo Changes";

            public static LocalizedString undoprompt =
                @"Are you sure you want to undo changes made to this class? This action cannot be reverted!";

            public static LocalizedString undotitle = @"Undo Changes";

        }

        public struct Colors
        {

            public static Dictionary<int, LocalizedString> presets = new Dictionary<int, LocalizedString>
            {
                {0, @"Black"},
                {1, @"White"},
                {2, @"Pink"},
                {3, @"Blue"},
                {4, @"Red"},
                {5, @"Green"},
                {6, @"Yellow"},
                {7, @"Orange"},
                {8, @"Purple"},
                {9, @"Gray"},
                {10, @"Cyan"}
            };

        }

        public struct Combat
        {

            public static Dictionary<int, LocalizedString> damagetypes = new Dictionary<int, LocalizedString>
            {
                {0, @"Physical"},
                {1, @"Magic"},
                {2, @"True"},
            };

            public static LocalizedString exp = @"Experience";

            public static Dictionary<int, LocalizedString> stats = new Dictionary<int, LocalizedString>
            {
                {0, @"Attack"},
                {1, @"Ability Power"},
                {2, @"Defense"},
                {3, @"Magic Resist"},
                {4, @"Speed"},
            };

            public static Dictionary<int, LocalizedString> vitals = new Dictionary<int, LocalizedString>
            {
                {0, @"Health"},
                {1, @"Mana"},
            };

        }

        public struct CommonEventEditor
        {

            public static LocalizedString copy = @"Copy Event";

            public static LocalizedString delete = @"Delete";

            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete this event? This action cannot be reverted!";

            public static LocalizedString events = @"Common Events";

            public static LocalizedString folderlabel = @"Folder:";

            public static LocalizedString foldertitle = @"Add Folder";

            public static LocalizedString folderprompt = @"Enter a name for the folder you'd like to add:";

            public static LocalizedString New = @"New";

            public static LocalizedString paste = @"Paste Event";

            public static LocalizedString pasteprompt =
                @"You cannot undo an event-paste operation! Are you sure you want to overwrite this event?";

            public static LocalizedString pastetitle = @"Paste Warning!";

            public static LocalizedString searchplaceholder = @"Search...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sortalphabetically = @"Order Alphabetically";

            public static LocalizedString title = @"Common Event Editor";

        }

        public struct CraftingTableEditor
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString cancel = @"Cancel";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString copy = @"Copy Table";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString crafts = @"Available Crafts";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString delete = @"Delete Table";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete this table? This action cannot be reverted!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString folderlabel = @"Folder:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString foldertitle = @"Add Folder";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString folderprompt = @"Enter a name for the folder you'd like to add:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString general = @"General";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString name = @"Name:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString New = @"New Table";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString paste = @"Paste Table";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString save = @"Save";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString searchplaceholder = @"Search...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sortalphabetically = @"Order Alphabetically";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString tables = @"Tables";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString title = @"Crafting Tables Editor";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString undo = @"Undo Changes";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString undoprompt =
                @"Are you sure you want to undo changes made to this table? This action cannot be reverted!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString undotitle = @"Undo Changes";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString addcraftlabel = @"Add Item To Be Crafted:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString add = @"Add Selected";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString remove = @"Remove Selected";
        }

        public struct CraftsEditor
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString copy = @"Copy Craft";

            public static LocalizedString crafts = @"Crafts";

            public static LocalizedString craftquantity = @"Quantity:";

            public static LocalizedString delete = @"Delete Craft";

            public static LocalizedString deletecraft = @"Delete";

            public static LocalizedString deleteingredient = @"Delete";

            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete this craft? This action cannot be reverted!";

            public static LocalizedString deletetitle = @"Delete Craft";

            public static LocalizedString duplicatecraft = @"Duplicate";

            public static LocalizedString duplicateingredient = @"Duplicate";

            public static LocalizedString folderlabel = @"Folder:";

            public static LocalizedString foldertitle = @"Add Folder";

            public static LocalizedString folderprompt = @"Enter a name for the folder you'd like to add:";

            public static LocalizedString general = @"General";

            public static LocalizedString ingredientitem = @"Item:";

            public static LocalizedString ingredientlistitem = @"Ingredient: {00} x{01}";

            public static LocalizedString ingredientnone = @"None";

            public static LocalizedString ingredientquantity = @"Quantity:";

            public static LocalizedString ingredients = @"Ingredients";

            public static LocalizedString item = @"Item:";

            public static LocalizedString name = @"Name:";

            public static LocalizedString New = @"New Craft";

            public static LocalizedString newingredient = @"New";

            public static LocalizedString paste = @"Paste Craft";

            public static LocalizedString save = @"Save";

            public static LocalizedString searchplaceholder = @"Search...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sortalphabetically = @"Order Alphabetically";

            public static LocalizedString time = @"Time (ms):";

            public static LocalizedString title = @"Crafts Editor";

            public static LocalizedString undo = @"Undo Changes";

            public static LocalizedString undoprompt =
                @"Are you sure you want to undo changes made to this craft? This action cannot be reverted!";

            public static LocalizedString undotitle = @"Undo Changes";
            
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString commonevent = @"Common Event:";

        }

        public struct Directions
        {

            public static Dictionary<int, LocalizedString> dir = new Dictionary<int, LocalizedString>()
            {
                {-1, @"Retain Direction"},
                {0, @"Up"},
                {1, @"Down"},
                {2, @"Left"},
                {3, @"Right"}
            };

        }

        public struct DynamicRequirements
        {

            public static LocalizedString addcondition = @"Add Condition";

            public static LocalizedString addlist = @"Add List";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString conditioneditor = @"Add/Edit Condition";

            public static LocalizedString conditionlist = @"Conditiond";

            public static LocalizedString conditionlists = @"Condition Lists";

            public static LocalizedString instructionsevent =
                @"Below are condition lists. If conditions are met on any of the lists then the event can spawn/run.";

            public static LocalizedString instructionsitem =
                @"Below are condition lists. If conditions are met on any of the lists then the player can use the item.";

            public static LocalizedString instructionsnpcfriend =
                @"Below are condition lists. If conditions are met on any of the lists then the npc will be friendly/protect the player and cannot be hurt by the player.";

            public static LocalizedString instructionsnpcattackonsight =
                @"Below are condition lists. If conditions are met on any of the lists player will be attacked on sight.";

            public static LocalizedString instructionsnpcdontattackonsight =
                @"Below are condition lists. If conditions are met on any of the lists then the player will not be attacked on sight by this npc.";

            public static LocalizedString instructionsnpccanbeattacked =
                @"Below are condition lists. If there are conditions, and they are not met, then the player will not be able to attack this npc.";

            public static LocalizedString instructionsquest =
                @"Below are condition lists. If conditions are met on any of the lists then the player can start the quest.";

            public static LocalizedString instructionsresource =
                @"Below are condition lists. If conditions are met on any of the lists then the player can harvest the resource.";

            public static LocalizedString instructionsspell =
                @"Below are condition lists. If conditions are met on any of the lists then the player can use cast the spell.";

            public static LocalizedString listname = @"Desc:";

            public static LocalizedString removecondition = @"Remove Condition";

            public static LocalizedString removelist = @"Remove List";

            public static LocalizedString save = @"Save";

            public static LocalizedString title = @"Dynamic Requirements";

        }

        public struct Errors
        {

            public static LocalizedString disconnected = @"Disconnected!";

            public static LocalizedString disconnectedclosing =
                @"You have lost connection to the server. The editor will now close.";

            public static LocalizedString disconnectedsave =
                @"You have been disconnected from the server! Would you like to export this map before closing this editor?";

            public static LocalizedString disconnectedsavecaption = @"Disconnected -- Export Map?";

            public static LocalizedString importfailed =
                @"Cannot import map. Currently selected map is not an Intersect map file or was exported with a different version of the Intersect editor!";

            public static LocalizedString importfailedcaption = @"Failed to import map!";

            public static LocalizedString resourcesnotfound =
                @"The resources directory could not be found! Intersect will now close.";

            public static LocalizedString resourcesnotfoundtitle = @"Resources not found!";

        }

        public struct EventChangeFace
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Face:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Change Face";

        }

        public struct EventShowPicture
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Picture:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Show Picture";

            public static LocalizedString checkbox = @"Click To Close Image?";

            public static LocalizedString size = @"Size:";

            public static LocalizedString original = @"Original";

            public static LocalizedString fullscreen = @"Full Screen";

            public static LocalizedString halfscreen = @"Half Screen";

            public static LocalizedString stretchtofit = @"Stretch To Fit";

            public static LocalizedString hide = @"Hide After (ms):";

            public static LocalizedString wait = @"Wait Until Closed?";

        }

        public struct EventChangeGender
        {

            public static LocalizedString cancel = @"Cancel";

            public static Dictionary<int, LocalizedString> genders = new Dictionary<int, LocalizedString>
            {
                {0, @"Male"},
                {1, @"Female"}
            };

            public static LocalizedString label = @"Gender:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Change Gender";

        }

        public struct EventChangeItems
        {

            public static LocalizedString action = @"Action:";

            public static Dictionary<int, LocalizedString> actions = new Dictionary<int, LocalizedString>
            {
                {0, @"Give"},
                {1, @"Take"},
            };

            public static LocalizedString amount = @"Amount:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString AmountType = @"Amount Type";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Variable = @"Variable";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Manual = @"Manual";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString PlayerVariable = @"Player Variable";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString ServerVariable = @"Global Variable";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString item = @"Item:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Method = @"Method:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static Dictionary<int, LocalizedString> Methods = new Dictionary<int, LocalizedString>
            {
                {0, @"Normal"},
                {1, @"Allow Overflow"},
                {2, @"Up to Amount" },
            };

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Change Player Items";

        }

        public struct EventEquipItems
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString item = @"Item:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Equip/Unequip Player Items";

            public static LocalizedString unequip = @"Unequip?";

        }

        public struct EventChangeVital
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString labelhealth = @"Set Health:";

            public static LocalizedString labelmana = @"Set Mana:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Change Vital";

        }

        public struct EventChangeLevel
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Set Level:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Change Level";

        }

        public struct EventChangeNameColor
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Change Name Color";

            public static LocalizedString select = @"Select Color";

            public static LocalizedString adminoverride = @"Override Admin Name Color?";

            public static LocalizedString remove = @"Remove Name Color?";

        }

        public struct EventChangeName
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Change Name";

            public static LocalizedString variable = @"Player Variable:";

        }

        public struct EventChangeSpells
        {

            public static LocalizedString action = @"Action: ";

            public static Dictionary<int, LocalizedString> actions = new Dictionary<int, LocalizedString>
            {
                {0, @"Add"},
                {1, @"Remove"}
            };

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString spell = @"Spell: ";

            public static LocalizedString title = @"Change Player Spells";

        }

        public struct EventChangeSprite
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Sprite:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Change Sprite";

        }

        public struct EventChangePlayerLabel
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Change Player Label";

            public static LocalizedString select = @"Select Color";

            public static LocalizedString copyplayernamecolor = @"Copy Player Name Color?";

            public static LocalizedString value = @"Value:";

            public static LocalizedString hint = @"Text variables work with strings. Click here for info!";

            public static LocalizedString position = @"Label Position:";

            public static Dictionary<int, LocalizedString> positions = new Dictionary<int, LocalizedString>
            {
                {0, @"Above Character Name"},
                {1, @"Below Character Name"},
            };

        }

        public struct EventChatboxText
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString channel = @"Channel:";

            public static Dictionary<int, LocalizedString> channels = new Dictionary<int, LocalizedString>
            {
                {0, @"Player"},
                {1, @"Local"},
                {2, @"Global"},
            };

            public static LocalizedString color = @"Color:";

            public static LocalizedString commands = @"Chat Commands";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString text = @"Text:";

            public static LocalizedString title = @"Add Chatbox Text";

        }

        public struct EventCommandList
        {

            public static LocalizedString addvariable = @"Add {00}";

            public static LocalizedString admin = @"Owner/Developer";

            public static LocalizedString animationonevent = @"On Event {00} [X Offset: {01} Y Offset: {02} {03}]";

            public static LocalizedString animationonmap = @"[On Map {00} X: {01} Y: {02} Dir: {03}]";

            public static LocalizedString animationonplayer = @"On Player [X Offset: {00} Y Offset: {01} {02}]";

            public static LocalizedString animationrelativedir = @"Spawn Relative To Direction";

            public static LocalizedString animationrelativerotate = @"Spawn And Rotate Relative To Direction";

            public static LocalizedString animationrotatedir = @"Rotate Relative To Direction";

            public static LocalizedString changename = @"Change Name to Variable: {00}";

            public static LocalizedString changeitems = @"Change Player Items [{00}]";

            public static LocalizedString equipitem = @"Equip Player Item [{00}]";

            public static LocalizedString unequipitem = @"Unequip Player Item [{00}]";

            public static LocalizedString changespells = @"Change Player Spells [{00}]";

            public static LocalizedString chatboxtext = @"Show Chatbox Text [Channel: {00}, Color: {01}] - {02}";

            public static LocalizedString chatglobal = @"Global";

            public static LocalizedString chatlocal = @"Local";

            public static LocalizedString chatplayer = @"Player";

            public static LocalizedString commonevent = @"Start Common Event: {00}";

            public static LocalizedString completetask = @"Complete Quest Task [Quest: {00}, Task: {01}]";

            public static LocalizedString conditionalbranch = @"Conditional Branch: [{00}]";

            public static LocalizedString conditionalelse = @"Else";

            public static LocalizedString conditionalend = @"End Branch";

            public static LocalizedString deletedevent = @"Deleted Event!";

            public static LocalizedString despawnnpcs = @"Despawn NPCs";

            public static LocalizedString dividevariable = @"Divide {00}";

            public static LocalizedString dupglobalvariable = @"Global Variable: {00}'s Value";

            public static LocalizedString dupplayervariable = @"Player Variable: {00}'s Value";

            public static LocalizedString addglobalvariable = @"Add Global Variable: {00}'s Value";

            public static LocalizedString addplayervariable = @"Add Player Variable: {00}'s Value";

            public static LocalizedString subtractglobalvariable = @"Subtract Global Variable: {00}'s Value";

            public static LocalizedString subtractplayervariable = @"Subtract Player Variable: {00}'s Value";

            public static LocalizedString multiplyglobalvariable = @"Multiply Global Variable: {00}'s Value";

            public static LocalizedString multiplyplayervariable = @"Multiply Player Variable: {00}'s Value";

            public static LocalizedString divideglobalvariable = @"Divide Global Variable: {00}'s Value";

            public static LocalizedString divideplayervariable = @"Divide Player Variable: {00}'s Value";

            public static LocalizedString leftshiftglobalvariable = @"Left Bit Shift Global Variable: {00}'s Value";

            public static LocalizedString leftshiftplayervariable = @"Left Bit Shift Player Variable: {00}'s Value";

            public static LocalizedString rightshiftglobalvariable = @"Right Bit Shift Global Variable: {00}'s Value";

            public static LocalizedString rightshiftplayervariable = @"Right Bit Shift Player Variable: {00}'s Value";

            public static LocalizedString enditemchange = @"End Item Change";

            public static LocalizedString endoptions = @"End Options";

            public static LocalizedString endquest = @"End Quest [{00}, {01}]";

            public static LocalizedString endspell = @"End Spell Change";

            public static LocalizedString endname = @"End Name Change";

            public static LocalizedString endstartquest = @"End Start Quest";

            public static LocalizedString exitevent = @"Exit Event Processing";

            public static LocalizedString fadeoutbgm = @"Fadeout BGM";

            public static LocalizedString False = @"False";

            public static LocalizedString female = @"Female";

            public static LocalizedString forcedstart = @"Forced Start";

            public static LocalizedString forget = @"Remove: Spell {00}";

            public static LocalizedString give = @"Give: Item {00}";

            public static LocalizedString giveexp = @"Give Player {00} Experience";

            public static LocalizedString globalswitch = @"Set Global Switch {00} to {01}";

            public static LocalizedString globalvariable = @"Set Global Variable {00} ({01})";

            public static LocalizedString gotolabel = @"Go to Label {00}";

            public static LocalizedString hidepicture = @"Hide Picture";

            public static LocalizedString hideplayer = @"Hide Player";

            public static LocalizedString holdplayer = @"Hold Player";

            public static LocalizedString invalid = @"Invalid Command";

            public static LocalizedString itemnotchanged = @"Item(s) Not Given/Taken (Doesn't have/Inventory full)";

            public static LocalizedString itemschanged = @"Item(s) Given/Taken Successfully";

            public static LocalizedString label = @"Label: {00}";

            public static LocalizedString leftshiftvariable = @"Left Bit Shift {00}";

            public static LocalizedString levelup = @"Level Up Player";

            public static LocalizedString linestart = @"@>";

            public static LocalizedString male = @"Male";

            public static LocalizedString mapnotfound = @"NOT FOUND";

            public static LocalizedString moderator = @"Moderator";

            public static LocalizedString moveroute = @"Set Move Route for {00}";

            public static LocalizedString moverouteevent = @"Event #{00}";

            public static LocalizedString moverouteplayer = @"Player";

            public static LocalizedString multiplyvariable = @"Multiply {00}";

            public static LocalizedString notcommon = @"Cannot use this command in common events.";

            public static LocalizedString notcommoncaption = @"Common Event Warning!";

            public static LocalizedString openbank = @"Open Bank";

            public static LocalizedString opencrafting = @"Open Crafting Table [{00}]";

            public static LocalizedString openshop = @"Open Shop [{00}]";

            public static LocalizedString playanimation = @"Play Animation {00} {01}";

            public static LocalizedString playbgm = @"Play BGM [File: {00}]";

            public static LocalizedString playervariable = @"Set Player Variable {00} ({01})";

            public static LocalizedString playsound = @"Play Sound [File: {00}]";

            public static LocalizedString questnotstarted =
                @"Quest Declined or Failed to Start (Reqs not met, already started, etc)";

            public static LocalizedString queststarted = @"Quest Accepted/Started Successfully";

            public static LocalizedString randvariable = @"Random Number {00} to {01}";

            public static LocalizedString regularuser = @"Regulator User";

            public static LocalizedString releaseplayer = @"Release Player";

            public static LocalizedString restorehp = @"Restore Player HP";

            public static LocalizedString restoremp = @"Restore Player MP";

            public static LocalizedString restorehpby = @"Adjust Player HP ({00})";

            public static LocalizedString restorempby = @"Adjust Player MP ({00})";

            public static LocalizedString rightshiftvariable = @"Right Bit Shift {00}";

            public static LocalizedString runcompletionevent = @"Running Completion Event";

            public static LocalizedString selfswitch = @"Set Self Switch {00} to {01}";

            public static LocalizedString showplayer = @"Show Player";

            public static Dictionary<int, LocalizedString> selfswitches = new Dictionary<int, LocalizedString>
            {
                {0, @"A"},
                {1, @"B"},
                {2, @"C"},
                {3, @"D"},
            };

            public static LocalizedString setaccess = @"Set Player Access to {00}";

            public static LocalizedString setclass = @"Set Class [{00}]";

            public static LocalizedString setface = @"Set Player Face to {00}";

            public static LocalizedString setnamecolor = @"Set Player Name Color";

            public static LocalizedString removenamecolor = @"Remove Player Name Color";

            public static LocalizedString changeplayerlabel = @"Change Player Label to {00}";

            public static LocalizedString setgender = @"Set Player Gender to {00}";

            public static LocalizedString setlevel = @"Set Player Level To: {00}";

            public static LocalizedString setsprite = @"Set Player Sprite to {00}";

            public static LocalizedString setvariable = @"Set to {00}";

            public static LocalizedString replace = @"Replace {00} with {01}";

            public static LocalizedString showoffer = @"Show Offer Window";

            public static LocalizedString showoptions = @"Show Options: {00}";

            public static LocalizedString variableinput = @"Input Variable: {00}";

            public static LocalizedString showpicture = @"Show Picture";

            public static LocalizedString showtext = @"Show Text: {00}";

            public static LocalizedString skipcompletionevent = @"Without Running Completion Event";

            public static LocalizedString spawnnpc = @"Spawn Npc {00} {01}";

            public static LocalizedString spawnonevent = @"On Event #{00} [X Offset: {01} Y Offset: {02} Dir: {03}]";

            public static LocalizedString spawnonmap = @"[On Map {00} X: {01} Y: {02} Dir: {03}]";

            public static LocalizedString spawnonplayer = @"On Player [X Offset: {00} Y Offset: {01} Dir: {02}]";

            public static LocalizedString spellfailed =
                @"Spell Not Taught/Moved (Already Knew/Spellbook full/Didn't Know)";

            public static LocalizedString spellsucceeded = @"Spell Taught/Removed Successfully";

            public static LocalizedString namesucceeded = @"Name Changed Successfully";

            public static LocalizedString namefailed = @"Name Not Changed (Taken, Invalid etc.)";

            public static LocalizedString startquest = @"Start Quest [{00}, {01}]";

            public static LocalizedString stopsounds = @"Stop Sounds";

            public static LocalizedString subtractvariable = @"Subtract {00}";

            public static LocalizedString systemtimevariable = @"System Time (ms)";

            public static LocalizedString take = @"Take: Item {00}";

            public static LocalizedString taskundefined = @"Undefined";

            public static LocalizedString teach = @"Teach: Spell {00}";

            public static LocalizedString True = @"True";

            public static LocalizedString unknown = @"Unknown Command";

            public static LocalizedString unknownrole = @"Unknown Access";

            public static LocalizedString wait = @"Wait {00}ms";

            public static LocalizedString waitforroute = @"Wait for Move Route Completion of {00}";

            public static LocalizedString warp = @"Warp Player [Map: {00} X: {01} Y: {02} Dir: {03}]";

            public static LocalizedString whenoption = @"When [{00}]";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString ChangePlayerColor = @"Change Player Color to: R: {00} G: {01} B: {02} A: {03}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString createguild = @"Create Guild [Player Variable {00} as name]";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString guildcreated = @"Guild created successfully.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString guildfailed = @"Guild failed to create.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString endcreateguild = @"End Create Guild";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString disbandguild = @"Disband Guild";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString guildisbanded = @"Guild disbanded successfully.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString guilddisbandfailed = @"Guild failed to disband.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString enddisbandguild = @"End Disband Guild";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString openguildbank = @"Open Guild Bank";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString setguildbankslots = @"Set Guild Bank Slots";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString resetstatpointallocations = @"Reset Player Stat Point Allocations";
        }

        public struct EventChangePlayerColor
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Cancel = @"Cancel";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Okay = @"Ok";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Title = @"Change Player Color";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Red = @"Red:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Green = @"Green:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Blue = @"Blue:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Alpha = @"Alpha:";

        }

        public struct EventCommands
        {

            public static Dictionary<string, LocalizedString> commands = new Dictionary<string, LocalizedString>()
            {
                {"addchatboxtext", @"Add Chatbox Text"},
                {"changeclass", @"Change Class"},
                {"changeface", @"Change Face"},
                {"changegender", @"Change Gender"},
                {"changeitems", @"Change Items"},
                {"changelevel", @"Change Level"},
                {"changespells", @"Change Spells"},
                {"changesprite", @"Change Sprite"},
                {"completequesttask", @"Complete Quest Task"},
                {"conditionalbranch", @"Conditional Branch"},
                {"despawnnpcs", @"Despawn NPC"},
                {"dialogue", @"Dialogue"},
                {"endquest", @"End Quest"},
                {"etc", @"Etc"},
                {"exiteventprocess", @"Exit Event Process"},
                {"fadeoutbgm", @"Fadeout BGM"},
                {"giveexperience", @"Give Experience"},
                {"gotolabel", @"Go To Label"},
                {"hidepicture", @"Hide Picture"},
                {"holdplayer", @"Hold Player"},
                {"label", @"Label"},
                {"levelup", @"Level Up"},
                {"logicflow", @"Logic Flow"},
                {"movement", @"Movement"},
                {"openbank", @"Open Bank"},
                {"opencraftingstation", @"Open Crafting Station"},
                {"openshop", @"Open Shop"},
                {"playanimation", @"Play Animation"},
                {"playbgm", @"Play BGM"},
                {"playercontrol", @"Player Control"},
                {"playsound", @"Play Sound"},
                {"questcontrol", @"Quest Control"},
                {"releaseplayer", @"Release Player"},
                {"restorehp", @"Restore HP"},
                {"restoremp", @"Restore MP"},
                {"setaccess", @"Set Access"},
                {"setmoveroute", @"Set Move Route"},
                {"setselfswitch", @"Set Self Switch"},
                {"setswitch", @"Set Switch"},
                {"setvariable", @"Set Variable"},
                {"shopandbank", @"Shop and Bank"},
                {"showoptions", @"Show Options"},
                {"showpicture", @"Show Picture"},
                {"showtext", @"Show Text"},
                {"spawnnpc", @"Spawn NPC"},
                {"specialeffects", @"Special Effects"},
                {"startcommonevent", @"Start Common Event"},
                {"startquest", @"Start Quest"},
                {"stopsounds", @"Stop Sounds"},
                {"wait", @"Wait..."},
                {"waitmoveroute", @"Wait for Route Completion"},
                {"warpplayer", @"Warp Player"},
                {"hideplayer", @"Hide Player"},
                {"showplayer", @"Show Player"},
                {"equipitem", @"Equip/Unequip Item"},
                {"changenamecolor", @"Change Name Color"},
                {"inputvariable", @"Input Variable"},
                {"changeplayerlabel", @"Change Player Label"},
                {"changeplayercolor", @"Change Player Color" },
                {"changename", @"Change Player Name" },
                {"guilds", @"Guilds"},
                {"createguild", @"Create Guild"},
                {"disbandguild", "Disband Guild" },
                {"openguildbank", @"Open Guild Bank"},
                {"setguildbankslots", @"Set Guild Bank Slots Count"},
                {"resetstatallocations", @"Reset Stat Point Allocations"},
            };

        }

        public struct EventCompleteQuestTask
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString quest = @"Quest:";

            public static LocalizedString task = @"Task:";

            public static LocalizedString title = @"Complete Quest Task";

        }

        public struct EventConditional
        {

            public static LocalizedString and = @"And";

            public static LocalizedString booleanvariable = @"Boolean Variable:";

            public static LocalizedString booleanequal = @"Equal To";

            public static LocalizedString booleannotequal = @"Not Equal To";

            public static LocalizedString stringvariable = @"String Variable:";

            public static LocalizedString stringtip = @"Text variables work here. Click here for a list!";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString canstartquest = @"Can Start Quest";

            public static LocalizedString Class = @"Class:";

            public static LocalizedString classis = @"Class Is";

            public static LocalizedString commoneventsonly = @"This condition works for Common Events activation only!";

            public static LocalizedString comparator = @"Comparator:";

            public static Dictionary<int, LocalizedString> stringcomparators = new Dictionary<int, LocalizedString>
            {
                {0, @"Equal To"},
                {1, @"Contains"}
            };

            public static Dictionary<int, LocalizedString> comparators = new Dictionary<int, LocalizedString>
            {
                {0, @"Equal To"},
                {1, @"Greater Than or Equal To"},
                {2, @"Less Than or Equal To"},
                {3, @"Greater Than"},
                {4, @"Less Than"},
                {5, @"Does Not Equal"}
            };

            public static Dictionary<int, LocalizedString> conditions = new Dictionary<int, LocalizedString>
            {
                {0, @"Variable Is..."},
                {4, @"Has item..."},
                {5, @"Class is..."},
                {6, @"Knows spell..."},
                {7, @"Level or Stat is...."},
                {8, @"Self Switch is...."},
                {9, @"Power level is...."},
                {10, @"Time is between...."},
                {11, @"Can Start Quest...."},
                {12, @"Quest In Progress...."},
                {13, @"Quest Completed...."},
                {14, @"No NPCs on Map"},
                {15, @"Gender is..."},
                {16, @"Map is..."},
                {17, @"Item Equipped is..."},
                {18, @"Has X free Inventory slots..." },
                {19, @"In Guild With At Least Rank..." },
                {20, @"Map Zone Type is..." }
            };

            public static LocalizedString endrange = @"End Range:";

            public static LocalizedString False = @"False";

            public static LocalizedString female = @"Female";

            public static LocalizedString gender = @"Gender:";

            public static LocalizedString genderis = @"Gender Is..";

            public static LocalizedString globalswitch = @"Global Switch";

            public static LocalizedString globalvariable = @"Global Variable";

            public static LocalizedString globalvariablevalue = @"Global Variable Value: ";

            public static LocalizedString hasatleast = @"Has at least:";

            public static LocalizedString hasitem = @"Has Item";

            public static LocalizedString hasitemequipped = @"Has Equipped Item";

            public static LocalizedString ignorestatbuffs = @"Ignore equipment & spell buffs.";

            public static LocalizedString item = @"Item:";

            public static LocalizedString knowsspell = @"Knows Spell";

            public static LocalizedString level = @"Level";

            public static LocalizedString levelorstat = @"Level or Stat Is....";

            public static LocalizedString levelstatitem = @"Level or Stat:";

            public static LocalizedString levelstatvalue = @"Value:";

            public static LocalizedString male = @"Male";

            public static LocalizedString mapis = @"Map Is...";

            public static LocalizedString negated = @"Negated";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString HasElse = @"Has Else";

            public static LocalizedString numericvariable = @"Numeric Variable:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString playervariable = @"Player Variable";

            public static LocalizedString playervariablevalue = @"Player Variable Value: ";

            public static LocalizedString power = @"Power:";

            public static LocalizedString power0 = @"Mod or Admin";

            public static LocalizedString power1 = @"Admin";

            public static LocalizedString poweris = @"Power Is";

            public static Dictionary<int, LocalizedString> questcomparators = new Dictionary<int, LocalizedString>
            {
                {0, @"On Any Task"},
                {1, @"Before Task..."},
                {2, @"After Task..."},
                {3, @"On Task..."},
            };

            public static LocalizedString questcompleted = @"Quest Completed";

            public static LocalizedString questcompletedlabel = @"Quest:";

            public static LocalizedString questinprogress = @"Quest In Progress";

            public static LocalizedString questis = @"Is:";

            public static LocalizedString questprogress = @"Quest:";

            public static LocalizedString selectmap = @"Select Map";

            public static LocalizedString selectvariable = @"Select Variable:";

            public static LocalizedString selfswitch = @"Self Switch:";

            public static Dictionary<int, LocalizedString> selfswitches = new Dictionary<int, LocalizedString>
            {
                {0, @"A"},
                {1, @"B"},
                {2, @"C"},
                {3, @"D"},
            };

            public static LocalizedString selfswitchis = @"Self Switch Is";

            public static LocalizedString spell = @"Spell:";

            public static LocalizedString startquest = @"Quest:";

            public static LocalizedString startrange = @"Start Range:";

            public static LocalizedString switchis = @"Is";

            public static LocalizedString task = @"Task:";

            public static LocalizedString time = @"Time is between:";

            public static LocalizedString title = @"Conditional";

            public static LocalizedString to = @"to";

            public static LocalizedString True = @"True";

            public static LocalizedString type = @"Condition Type:";

            public static LocalizedString value = @"Static Value:";

            public static LocalizedString variable = @"Variable Is...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString FreeInventorySlots = @"Has X free Inventory slots";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString AmountType = @"Amount Type";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString VariableLabel = @"Variable";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Manual = @"Manual";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString inguild = @"In Guild With At Least Rank...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString rank = @"Rank:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString MapZoneTypeIs = @"Map Zone Type is:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString MapZoneTypeLabel = @"Zone Type:";
        }

        public struct EventConditionDesc
        {

            public static LocalizedString admin = @"Admin";

            public static LocalizedString aftertask = @", After Task: {00}";

            public static LocalizedString beforetask = @", Before Task: {00}";

            public static LocalizedString Class = @"Player's class is {00}";

            public static LocalizedString contains = @"contains {00}";

            public static LocalizedString equal = @"is equal to {00}";

            public static LocalizedString False = @"False";

            public static LocalizedString female = @"Female";

            public static LocalizedString gender = @"Player's Gender is {00}";

            public static LocalizedString globalvariable = @"Global Variable: {00} {01}";

            public static LocalizedString globalvariablevalue = @"Global Variable: {00}'s Value";

            public static LocalizedString greater = @"is greater than {00}";

            public static LocalizedString greaterequal = @"is greater than or equal to {00}";

            public static LocalizedString guild = @"Player is in Guild with at least rank: {00}";

            public static LocalizedString hasitem = @"Player has at least {00} of Item {01}";

            public static LocalizedString hasitemequipped = @"Player has Item {00} equipped";

            public static LocalizedString knowsspell = @"Player knows Spell {00}";

            public static LocalizedString lessthan = @"is less than {00}";

            public static LocalizedString lessthanequal = @"is less than or equal to {00}";

            public static LocalizedString level = @"Level";

            public static LocalizedString levelorstat = @"{00} {01}";

            public static LocalizedString male = @"Male";

            public static LocalizedString map = @"Player's Map is {00}";

            public static LocalizedString mapnotfound = @"NOT FOUND";

            public static LocalizedString modadmin = @"Mod or Admin";

            public static LocalizedString negated = @"NOT [{00}]";

            public static LocalizedString nonpcsonmap = @"No NPCs on the map";

            public static LocalizedString notequal = @"does not equal {00}";

            public static LocalizedString onanytask = @", On Any Task";

            public static LocalizedString ontask = @", On Task: {00}";

            public static LocalizedString playerdeath = @"Player Death";

            public static LocalizedString playervariable = @"Player Variable: {00} {01}";

            public static LocalizedString playervariablevalue = @"Player Variable: {00}'s Value";

            public static LocalizedString power = @"Player's Power is {00}";

            public static LocalizedString questcompleted = @"Quest is Completed: {00}";

            public static LocalizedString questinprogress = @"Quest In Progress: {00} {01}";

            public static LocalizedString selfswitch = @"Self Switch {00} is {01}";

            [JsonProperty]
            public static LocalizedString HasFreeInventorySlots = @"Player has {00} free inventory slot(s)";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString MapZoneTypeIs = @"Map Zone Type is {00}";

            public static Dictionary<int, LocalizedString> selfswitches = new Dictionary<int, LocalizedString>
            {
                {0, @"A"},
                {1, @"B"},
                {2, @"C"},
                {3, @"D"},
            };

            public static LocalizedString startquest = @"Can Start Quest: {00}";

            public static LocalizedString tasknotfound = @"Not Found";

            public static LocalizedString time = @"Time is between {00} and {01}";

            public static LocalizedString timeinvalid = @"invalid";

            public static LocalizedString True = @"True";

        }

        public struct EventCreateGuild
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Cancel = @"Cancel";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString SelectVariable = @"Player Variable containing Guild Name:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Okay = @"Ok";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Title = @"Create Guild";

        }

        public struct EventGuildSetBankSlotsCount
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Variable = @"Variable";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString PlayerVariable = @"Player Variable";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString ServerVariable = @"Global Variable";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString cancel = @"Cancel";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString okay = @"Ok";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString title = @"Set Guild Bank Slots Count";
        }

        public struct EventEditor
        {

            public static LocalizedString addcommand = @"Add Commands";

            public static LocalizedString animation = @"Animation";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString clearpage = @"Clear Page";

            public static LocalizedString command = @"/Command: /";

            public static LocalizedString commandlist = @"Commands:";

            public static Dictionary<int, LocalizedString> commontriggers = new Dictionary<int, LocalizedString>
            {
                {0, @"None"},
                {1, @"Login"},
                {2, @"Level Up"},
                {3, @"On Respawn"},
                {4, @"/Command"},
                {5, @"Autorun"},
                {6, @"PVP Kill"},
                {7, @"PVP Death"},
                {8, @"Player Interact"},
                {9, @"Equipment Changed"},
                {10, @"Player Variable Changed"},
                {11, @"Server Variable Changed"},
                {12, @"Guild Member Joined"},
                {13, @"Guild Member Left"},
                {14, @"Guild Member Kicked"},
            };

            public static LocalizedString conditions = @"Conditions";

            public static LocalizedString copypage = @"Copy Page";

            public static LocalizedString copycommand = @"Copy";

            public static LocalizedString cutcommand = @"Cut";

            public static LocalizedString deletecommand = @"Delete";

            public static LocalizedString deletepage = @"Delete Page";

            public static LocalizedString directionfix = @"Dir Fix";

            public static LocalizedString disableinspector = @"Disable Inspector";

            public static LocalizedString editcommand = @"Edit";

            public static LocalizedString editconditions = @"Spawn/Execution Conditions";

            public static LocalizedString entityoptions = @"Entity Options";

            public static LocalizedString eventpreview = @"Preview";

            public static LocalizedString extras = @"Extras:";

            public static LocalizedString face = @"Face:";

            public static LocalizedString frequency = @"Freq:";

            public static Dictionary<int, LocalizedString> frequencies = new Dictionary<int, LocalizedString>
            {
                {0, @"Not Very Often"},
                {1, @"Not Often"},
                {2, @"Normal"},
                {3, @"Often"},
                {4, @"Very Often"},
            };

            public static LocalizedString general = @"General";

            public static LocalizedString global = @"Global Event";

            public static LocalizedString hidename = @"Hide Name";

            public static LocalizedString insertcommand = @"Insert";

            public static LocalizedString inspector = @"Entity Inspector Options";

            public static LocalizedString inspectordesc = @"Inspector Description:";

            public static LocalizedString interactionfreeze = @"Interaction Freeze";

            public static LocalizedString layer = @"Layer:";

            public static Dictionary<int, LocalizedString> layers = new Dictionary<int, LocalizedString>
            {
                {0, @"Below Player"},
                {1, @"Same as Player"},
                {2, @"Above Player"},
            };

            public static LocalizedString movement = @"Movement";

            public static LocalizedString movementtype = @"Type:";

            public static Dictionary<int, LocalizedString> movementtypes = new Dictionary<int, LocalizedString>
            {
                {0, @"None"},
                {1, @"Random"},
                {2, @"Move Route"},
            };

            public static LocalizedString name = @"Name:";

            public static LocalizedString newpage = @"New Page";

            public static LocalizedString pageoptions = @"Page Options";

            public static LocalizedString passable = @"Passable";

            public static LocalizedString pastecommand = @"Paste";

            public static LocalizedString pastepage = @"Paste Page";

            public static LocalizedString projectile = @"Projectile:";

            public static LocalizedString save = @"Save";

            public static LocalizedString savecaption = @"Save Event?";

            public static LocalizedString savedialogue = @"Do you want to save changes to this event?";

            public static LocalizedString setroute = @"Set Route...";

            public static LocalizedString speed = @"Speed:";

            public static Dictionary<int, LocalizedString> speeds = new Dictionary<int, LocalizedString>
            {
                {0, @"Slowest"},
                {1, @"Slower"},
                {2, @"Normal"},
                {3, @"Faster"},
                {4, @"Fastest"},
            };

            public static LocalizedString title = @"Event Editor - {00}";

            public static LocalizedString trigger = @"Trigger";

            public static Dictionary<int, LocalizedString> triggers = new Dictionary<int, LocalizedString>
            {
                {0, @"Action Button"},
                {1, @"Player Collide"},
                {2, @"Autorun"},
                {3, "Player Bump"},
            };

            public static LocalizedString walkinganim = @"Walking Anim";

        }

        public struct EventEndQuest
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Quest:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString skipcompletion = @"Do not run completion event?";

            public static LocalizedString title = @"End Quest";

        }

        public struct EventGiveExperience
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Give Experience:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Give Experience";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString AmountType = @"Amount Type";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Variable = @"Variable";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Manual = @"Manual";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString PlayerVariable = @"Player Variable";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString ServerVariable = @"Global Variable";

        }

        public struct EventGotoLabel
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Label:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Go To Label";

        }

        public struct EventGraphic
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString graphic = @"Graphic:";

            public static LocalizedString graphictype0 = @"None";

            public static LocalizedString graphictype1 = @"Sprite";

            public static LocalizedString graphictype2 = @"Tileset";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString preview = @"Graphic Preview";

            public static LocalizedString title = @"Graphic Selector";

            public static LocalizedString type = @"Graphic Type:";

        }

        public struct EventLabel
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Label:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Label";

        }

        public struct EventMoveRoute
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString command = @"Commands";

            public static Dictionary<string, LocalizedString> commands = new Dictionary<string, LocalizedString>()
            {
                {"directionfixoff", @"Direction Fix: Off"},
                {"directionfixon", @"Direction Fix: On"},
                {"etc", @"Etc"},
                {"facedown", @"Face Down"},
                {"faceleft", @"Face Left"},
                {"faceright", @"Face Right"},
                {"faceup", @"Face Up"},
                {"faster", @"Faster"},
                {"fastest", @"Fastest"},
                {"frequencynormal", @"Normal"},
                {"hidename", @"Hide Name"},
                {"higher", @"Higher"},
                {"highest", @"Highest"},
                {"lower", @"Lower"},
                {"lowest", @"Lowest"},
                {"move", @"Move"},
                {"moveawayfromplayer", @"Move Away From Player"},
                {"movedown", @"Move Down"},
                {"moveleft", @"Move Left"},
                {"moverandomly", @"Move Randomly"},
                {"moveright", @"Move Right"},
                {"movetowardplayer", @"Move Toward Player"},
                {"moveup", @"Move Up"},
                {"setanimation", @"Set Animation..."},
                {"setattribute", @"Set Attribute"},
                {"setgraphic", @"Set Graphic..."},
                {"setlayerabove", @"Set Layer: Above Player"},
                {"setlayerbelow", @"Set Layer: Below Player"},
                {"setlayersame", @"Set Layer: Same as Player"},
                {"setmovementfrequency", @"Set Movement Frequency"},
                {"setspeed", @"Set Speed"},
                {"showname", @"Show Name"},
                {"slower", @"Slower"},
                {"slowest", @"Slowest"},
                {"speednormal", @"Normal"},
                {"stepbackward", @"Step Backward"},
                {"stepforward", @"Step Forward"},
                {"turn", @"Turn"},
                {"turn180", @"Turn 180*"},
                {"turn90clockwise", @"Turn 90* Clockwise"},
                {"turn90counterclockwise", @"Turn 90* Counter Clockwise"},
                {"turnawayfromplayer", @"Turn Away From Player"},
                {"turnrandomly", @"Turn Randomly"},
                {"turntowardplayer", @"Turn Toward Player"},
                {"wait100", @"Wait 100ms"},
                {"wait1000", @"Wait 1000ms"},
                {"wait500", @"Wait 500ms"},
                {"walkinganimoff", @"Walking Animation: Off"},
                {"walkinganimon", @"Walking Animation: On"},
                {"walkthroughoff", @"Walkthrough: Off"},
                {"walkthroughon", @"Walkthrough: On"},
            };

            public static LocalizedString ignoreblocked = @"Ignore if Blocked";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString player = @"Player";

            public static LocalizedString repeatroute = @"Repeat Route";

            public static LocalizedString thisevent = @"[THIS EVENT]";

            public static LocalizedString title = @"Move Route";

        }

        public struct EventOpenCrafting
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Table:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Open Crafting";

        }

        public struct EventOpenShop
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Shop:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Open Shop";

        }

        public struct EventPlayAnimation
        {

            public static LocalizedString animation = @"Animation:";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString entity = @"Entity:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString player = @"Player";

            public static LocalizedString relativelocation = @"Relative Location:";

            public static LocalizedString rotaterelative = @"Rotate Relative to Direction";

            public static LocalizedString spawnrelative = @"Spawn Relative to Direction";

            public static LocalizedString spawntype = @"Spawn Type:";

            public static LocalizedString spawntype0 = @"Specific Tile";

            public static LocalizedString spawntype1 = @"On/Around Entity";

            public static LocalizedString This = @"[THIS EVENT]";

            public static LocalizedString title = @"Play Animation";

        }

        public struct EventPlayBgm
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"BGM:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Play BGM";

        }

        public struct EventPlayBgs
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Sound:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Play BGS";

        }

        public struct EventSelfSwitch
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString False = @"False";

            public static LocalizedString label = @"Set Self Switch:";

            public static LocalizedString okay = @"Ok";

            public static Dictionary<int, LocalizedString> selfswitches = new Dictionary<int, LocalizedString>
            {
                {0, @"A"},
                {1, @"B"},
                {2, @"C"},
                {3, @"D"},
            };

            public static LocalizedString title = @"Set Self Switch";

            public static LocalizedString True = @"True";

        }

        public struct EventSetAccess
        {

            public static LocalizedString access0 = @"Regular User";

            public static LocalizedString access1 = @"In-Game Moderator";

            public static LocalizedString access2 = @"Owner/Designer (Allows editor access)";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Access:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Set Access";

        }

        public struct EventSetAnimation
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Animation:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Set Animation";

        }

        public struct EventSetClass
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Class:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Set Class";

        }

        public struct EventSetVariable
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString global = @"Global Variable";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString player = @"Player Variable";

            public static LocalizedString title = @"Set Variable";

            public static LocalizedString syncparty = @"Party Sync?";

            public static LocalizedString label = @"Select Variable:";

            public static LocalizedString booleanlabel = @"Boolean Variable:";

            public static LocalizedString booleantrue = @"True";

            public static LocalizedString booleanfalse = @"False";

            public static LocalizedString booleanccloneglobalvariablevalue = @"Global Variable Value: ";

            public static LocalizedString booleancloneplayervariablevalue = @"Player Variable Value: ";

            public static LocalizedString numericlabel = @"Integer Variable:";

            public static LocalizedString numericadd = @"Add";

            public static LocalizedString numericdivide = @"Divide";

            public static LocalizedString numericleftshift = @"LShift";

            public static LocalizedString numericcloneglobalvariablevalue = @"Global Variable Value: ";

            public static LocalizedString numericcloneplayervariablevalue = @"Player Variable Value: ";

            public static LocalizedString numericmultiply = @"Multiply";

            public static LocalizedString numericrandom = @"Random";

            public static LocalizedString numericrandomhigh = @"High:";

            public static LocalizedString numericrandomlow = @"Low:";

            public static LocalizedString numericrandomdesc = @"Random Number:";

            public static LocalizedString numericrightshift = @"RShift";

            public static LocalizedString numericset = @"Set";

            public static LocalizedString numericsubtract = @"Subtract";

            public static LocalizedString numericsystemtime = @"System Time (ms)";

            public static LocalizedString stringlabel = @"String Variable:";

            public static LocalizedString stringset = @"Set";

            public static LocalizedString stringreplace = @"Replace";

            public static LocalizedString stringsetvalue = @"Value:";

            public static LocalizedString stringreplacefind = @"Find:";

            public static LocalizedString stringreplacereplace = @"Replace:";

            public static LocalizedString stringtip = @"Text variables work with strings. Click here for a list!";

        }

        public struct EventShowOptions
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString commands = @"Chat Commands";

            public static LocalizedString face = @"Face:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString option1 = @"Option 1";

            public static LocalizedString option2 = @"Option 2";

            public static LocalizedString option3 = @"Option 3";

            public static LocalizedString option4 = @"Option 4";

            public static LocalizedString text = @"Text:";

            public static LocalizedString title = @"Show Options";

        }

        public struct EventShowText
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString commands = @"Chat Commands";

            public static LocalizedString face = @"Face:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString text = @"Text:";

            public static LocalizedString title = @"Show Text";

        }

        public struct EventInput
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString commands = @"Chat Commands";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString text = @"Text:";

            public static LocalizedString titlestr = @"Title:";

            public static LocalizedString title = @"Input Variable";

            public static LocalizedString playervariable = @"Player Variable";

            public static LocalizedString globalvariable = @"Global Variable";

            public static LocalizedString minval = @"Minimum Value";

            public static LocalizedString maxval = @"Maximum Value";

            public static LocalizedString minlength = @"Minimum Length";

            public static LocalizedString maxlength = @"Maximum Length";

        }

        public struct EventSpawnNpc
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString entity = @"Entity:";

            public static LocalizedString npc = @"NPC:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString player = @"Player";

            public static LocalizedString relativelocation = @"Relative Location:";

            public static LocalizedString spawnrelative = @"Relative to Entity Direction";

            public static LocalizedString spawntype = @"Spawn Type:";

            public static LocalizedString spawntype0 = @"Specific Tile";

            public static LocalizedString spawntype1 = @"On/Around Entity";

            public static LocalizedString This = @"[THIS EVENT]";

            public static LocalizedString title = @"Spawn Npc";

        }

        public struct EventStartCommonEvent
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Common Event:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Start Common Event";

        }

        public struct EventStartQuest
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Quest:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString showwindow = @"Show Offer Window?";

            public static LocalizedString title = @"Start Quest";

        }

        public struct EventWait
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Wait (ms):";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Wait";

        }

        public struct EventWaitForRouteCompletion
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString label = @"Entity:";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString player = @"Player";

            public static LocalizedString This = @"[THIS EVENT]";

            public static LocalizedString title = @"Wait for Move Route Completion";

        }

        public struct EventWarp
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Warp";

        }

        public struct General
        {

            public static LocalizedString none = @"None";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Missing = @"Missing Translation";

        }

        public struct ItemEditor
        {

            public static LocalizedString abilitypowerbonus = @"Ability Pwr:";

            public static LocalizedString animation = @"Animation:";

            public static LocalizedString attackanimation = @"Attack Animation:";

            public static LocalizedString attackbonus = @"Attack:";

            public static LocalizedString attackspeed = @"Attack Speed";

            public static LocalizedString attackspeedmodifier = @"Modifier:";

            public static Dictionary<int, LocalizedString> attackspeedmodifiers = new Dictionary<int, LocalizedString>
            {
                {0, @"Disabled"},
                {1, @"Static (ms)"},
                {2, @"Percentage"},
            };

            public static LocalizedString attackspeedvalue = @"Value:";

            public static LocalizedString bagpanel = @"Bag:";

            public static LocalizedString bagslots = @"Bag Slots:";

            public static LocalizedString basedamage = @"Base Damage:";

            public static LocalizedString bonusamount = @"Effect Amount (%):";

            public static LocalizedString bonuseffect = @"Bonus Effect:";

            public static Dictionary<int, LocalizedString> bonuseffects = new Dictionary<int, LocalizedString>
            {
                {0, @"None"},
                {1, @"Cooldown Reduction"},
                {2, @"Life Steal"},
                {3, @"Tenacity"},
                {4, @"Luck"},
                {5, @"EXP"},
            };

            public static LocalizedString bonuses = @"Stat Bonuses";

            public static LocalizedString bonusrange = @"Stat Bonus Range (+-):";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString CanDrop = @"Can Drop?";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString DeathDropChance = @"Drop chance on Death (%):";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString CanBag = @"Can Bag?";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString CanBank = @"Can Bank?";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString CanGuildBank = @"Can Guild Bank?";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString CanTrade = @"Can Trade?";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString CanSell = @"Can Sell?";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString consumeablepanel = @"Consumable";

            public static LocalizedString consumeamount = @"Amount:";

            public static LocalizedString cooldown = @"Cooldown (ms):";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString CooldownGroup = @"Cooldown Group:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString CooldownGroupTitle = @"Add Cooldown Group";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString CooldownGroupPrompt = @"Enter a name for the cooldown group you'd like to add:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString IgnoreGlobalCooldown = @"Ignore Global Cooldown?";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString IgnoreCooldownReduction = @"Ignore Cooldown Reduction?";

            public static LocalizedString copy = @"Copy Item";

            public static LocalizedString critchance = @"Crit Chance (%):";

            public static LocalizedString critmultiplier = @"Crit Multiplier (Default 1.5x):";

            public static LocalizedString damagetype = @"Damage Type:";

            public static LocalizedString defensebonus = @"Defense:";

            public static LocalizedString delete = @"Delete Item";

            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete this item? This action cannot be reverted!";

            public static LocalizedString deletetitle = @"Delete Item";

            public static LocalizedString description = @"Desc:";

            public static LocalizedString equipment = @"Equipment";

            public static LocalizedString equipmentanimation = @"Equipment Animation:";

            public static LocalizedString Event = @"Event:";

            public static LocalizedString eventpanel = @"Event";

            public static LocalizedString femalepaperdoll = @"Female Paperdoll:";

            public static LocalizedString folderlabel = @"Folder:";

            public static LocalizedString foldertitle = @"Add Folder";

            public static LocalizedString folderprompt = @"Enter a name for the folder you'd like to add:";

            public static LocalizedString general = @"General";

            public static LocalizedString health = @"Health:";

            public static LocalizedString items = @"Items";

            public static LocalizedString magicresistbonus = @"Magic Resist:";

            public static LocalizedString malepaperdoll = @"Male Paperdoll:";

            public static LocalizedString mana = @"Mana:";

            public static LocalizedString regen = @"Regen";

            public static LocalizedString regenhint = @"% of HP/Mana to restore per tick.
Tick timer saved in server config.json.";

            public static LocalizedString hpregen = @"HP (%);";

            public static LocalizedString mpregen = @"MP (%):";

            public static LocalizedString name = @"Name:";

            public static LocalizedString New = @"New Item";

            public static LocalizedString paste = @"Paste Item";

            public static LocalizedString picture = @"Pic:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Red = @"Red:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Green = @"Green:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Blue = @"Blue:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Alpha = @"Alpha:";

            public static LocalizedString price = @"Price:";

            public static Dictionary<int, LocalizedString> rarity = new Dictionary<int, LocalizedString>
            {
                {0, @"None"},
                {1, @"Common"},
                {2, @"Uncommon"},
                {3, @"Rare"},
                {4, @"Epic"},
                {5, @"Legendary"},
            };

            public static LocalizedString projectile = @"Projectile:";

            public static LocalizedString requirementsgroup = @"Requirements";

            public static LocalizedString cannotuse = @"Cannot Use Message:";

            public static LocalizedString requirements = @"Edit Usage Requirements";

            public static LocalizedString save = @"Save";

            public static LocalizedString scalingamount = @"Scaling Amount (%):";

            public static LocalizedString scalingstat = @"Scaling Stat:";

            public static LocalizedString searchplaceholder = @"Search...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sortalphabetically = @"Order Alphabetically";

            public static LocalizedString slot = @"Equipment Slot:";

            public static LocalizedString speedbonus = @"Speed:";

            public static LocalizedString spell = @"Spell:";

            public static LocalizedString spellpanel = @"Spell";

            public static LocalizedString quickcast = @"Quick Cast Spell?";

            public static LocalizedString destroyspell = @"Destroy On Use?";

            public static LocalizedString SingleUseEvent = @"Destroy On Use?";

            public static LocalizedString stackable = @"Stackable?";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString InventoryStackLimit = @"Inventory Stack Limit:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString BankStackLimit = @"Bank Stack Limit:";

            public static LocalizedString title = @"Item Editor";

            public static LocalizedString tooltype = @"Tool Type:";

            public static LocalizedString twohanded = @"2 Hand";

            public static LocalizedString type = @"Type:";

            public static Dictionary<int, LocalizedString> types = new Dictionary<int, LocalizedString>
            {
                {0, @"None"},
                {1, @"Equipment"},
                {2, @"Consumable"},
                {3, @"Currency"},
                {4, @"Spell"},
                {5, @"Event"},
                {6, @"Bag"},
            };

            public static LocalizedString undo = @"Undo Changes";

            public static LocalizedString undoprompt =
                @"Are you sure you want to undo changes made to this item? This action cannot be reverted!";

            public static LocalizedString undotitle = @"Undo Changes";

            public static LocalizedString vital = @"Vital:";

            public static LocalizedString vitalbonuses = @"Vital Bonuses";

            public static LocalizedString weaponproperties = @"Weapon Properties";

        }

        public struct LightEditor
        {

            public static LocalizedString color = @"Color:";

            public static LocalizedString expandamt = @"Expand Amt:";

            public static LocalizedString intensity = @"Intensity:";

            public static LocalizedString revert = @"Revert";

            public static LocalizedString save = @"Save";

            public static LocalizedString selectcolor = @"Select Color";

            public static LocalizedString size = @"Size:";

            public static LocalizedString title = @"Light Editor";

            public static LocalizedString xoffset = @"Offset X:";

            public static LocalizedString yoffset = @"Offset Y:";

        }

        public struct Login
        {

            public static LocalizedString connected = @"Connected to server. Ready to login!";

            public static LocalizedString connecting = @"Connecting to server...";

            public static LocalizedString failedtoconnect = @"Failed to connect, retrying in {00} seconds.";

            public static LocalizedString Denied = @"Connection denied. Version mismatch?";

            public static LocalizedString gettingstarted = @"Getting Started?
        1. Start the Intersect Server
        2. Open the Intersect Client and Create an Account
        3. Login to that account here to start designing your game!";

            public static LocalizedString login = @"Login";

            public static LocalizedString password = @"Password: ";

            public static LocalizedString raptr =
                @"Please close AMD Gaming Evolved before logging into the Intersect editor.";

            public static LocalizedString rememberme = @"Remember Me";

            public static LocalizedString title = @"Intersect Editor Login";

            public static LocalizedString username = @"Username: ";

            public static LocalizedString version = @"Editor v{00}";

        }

        public struct MainForm
        {

            public static LocalizedString about = @"About";

            public static LocalizedString alllayers = @"All Layers";

            public static LocalizedString animationeditor = @"Animation Editor";

            public static LocalizedString classeditor = @"Class Editor";

            public static LocalizedString commoneventeditor = @"Common Event Editor";

            public static LocalizedString copy = @"Copy";

            public static LocalizedString craftingtableeditor = @"Crafting Table Editor";

            public static LocalizedString craftingeditor = @"Crafts Editor";

            public static LocalizedString currentonly = @"Current Layer Only";

            public static LocalizedString cut = @"Cut";

            public static LocalizedString darkness = @"Darkness";

            public static LocalizedString droppler = @"Droppler Tool";

            public static LocalizedString edit = @"Edit";

            public static LocalizedString editors = @"Game Editors";

            public static LocalizedString erase = @"Erase";

            public static LocalizedString exit = @"Exit";

            public static LocalizedString exportmap = @"Export Map";

            public static LocalizedString externaltools = @"External Tools";

            public static LocalizedString file = @"File";

            public static LocalizedString fill = @"Fill";

            public static LocalizedString fog = @"Fog";

            public static LocalizedString fps = @"FPS: {00}";

            public static LocalizedString grid = @"Map Grid";

            public static LocalizedString help = @"Help";

            public static LocalizedString importmap = @"Import Map";

            public static LocalizedString itemeditor = @"Item Editor";

            public static LocalizedString lighting = @"Toggle Time of Day Simulation On/Off";

            public static LocalizedString loc = @"CurX: {00}  CurY: {01}";

            public static LocalizedString newmap = @"New Map";

            public static LocalizedString npceditor = @"Npc Editor";

            public static LocalizedString options = @"Options";

            public static LocalizedString overlay = @"Overlay";

            public static LocalizedString paste = @"Paste";

            public static LocalizedString pen = @"Pen Tool";

            public static LocalizedString postquestion = @"Post Question";

            public static LocalizedString projectileeditor = @"Projectile Editor";

            public static LocalizedString questeditor = @"Quest Editor";

            public static LocalizedString rectangle = @"Rectangle Tool";

            public static LocalizedString redo = @"Redo";

            public static LocalizedString fliphorizontal = @"Flip Horizontal";

            public static LocalizedString flipvertical = @"Flip Vertical";

            public static LocalizedString reportbug = @"Report Bug";

            public static LocalizedString resourceeditor = @"Resource Editor";

            public static LocalizedString resources = @"Resources";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Events = @"Events";

            public static LocalizedString revision = @"Revision: {00}";

            public static LocalizedString run = @"Run Client";

            public static LocalizedString savemap = @"Save Map";

            public static LocalizedString screenshot = @"Screenshot Map";

            public static LocalizedString selection = @"Selection Tool";

            public static LocalizedString selectlayers = @"Select...";

            public static LocalizedString shopeditor = @"Shop Editor";

            public static LocalizedString spelleditor = @"Spell Editor";

            public static LocalizedString variableeditor = @"Variable Editor";

            public static LocalizedString tilepreview = @"Tile Preview";

            public static LocalizedString timeeditor = @"Time Editor";

            public static LocalizedString title = @"Intersect Editor - {00}";

            public static LocalizedString tools = @"Tools";

            public static LocalizedString toolsdir = @"tools";

            public static LocalizedString undo = @"Undo";

            public static LocalizedString view = @"View";

        }

        public struct MapCacheProgress
        {

            public static LocalizedString title = @"Saving Map Cache";

            public static LocalizedString remaining = @"{00} maps remaining.";

        }

        public struct MapGrid
        {

            public static LocalizedString clearandfetch =
                @"Are you sure you want to clear the existing previews and fetch previews for each map on this grid? This could take several minutes based on the number of maps in this grid!";

            public static LocalizedString downloadall = @"Re-Download All Previews";

            public static LocalizedString downloadmissing = @"Download Missing Previews";

            public static LocalizedString fetchcaption = @"Fetch Preview?";

            public static LocalizedString fetchingmaps = @"Fetching Map Previews";

            public static LocalizedString fetchingprogress = @"Fetching Maps {00} / {01}";

            public static LocalizedString gridlines = @"Show/Hide Grid Lines";

            public static LocalizedString justfetch =
                @"Are you sure you want to fetch previews for each map? This could take several minutes based on the number of maps in this grid!";

            public static LocalizedString keepmapcache =
                @"Use your current settings with the map cache? (No to disable lighting/fogs/other effects)";

            public static LocalizedString link = @"Link Map";

            public static LocalizedString mapcachecaption = @"Map Cache Options";

            public static LocalizedString preview = @"Fetch Preview";

            public static LocalizedString recache = @"Recache Map";

            public static LocalizedString savescreenshotconfirm =
                @"Are you sure you want to save a screenshot of your world to a file? This could take several minutes!";

            public static LocalizedString savescreenshotdialogue = @"Save a screenshot of the world";

            public static LocalizedString savescreenshottitle = @"Save Screenshot?";

            public static LocalizedString savingrow = @"Saving Row: {00} / {01}";

            public static LocalizedString savingscreenshot = @"Saving Screenshot";

            public static LocalizedString screenshotworld = @"Take a world screenshot";

            public static LocalizedString title = @"Map Grid";

            public static LocalizedString unlink = @"Unlink Map";

            public static LocalizedString unlinkcaption = @"Unlink Map";

            public static LocalizedString unlinkprompt = @"Are you sure you want to unlink map {00}?";

        }

        public struct MapLayers
        {

            public static LocalizedString attributes = @"Attributes";

            public static LocalizedString eventinstructions = @"Double click a tile on the map to create an event!";

            public static LocalizedString events = @"Events";

            public static LocalizedString lightinstructions =
                @"Lower the maps brightness and double click on a tile to create a light!";

            public static LocalizedString lights = @"Lights";

            public static LocalizedString npcs = @"Npcs";

            public static LocalizedString tiles = @"Tiles";

            public static LocalizedString title = @"Map Layers";

        }

        public struct MapList
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString alphabetical = @"Alphabetical";

            public static LocalizedString copyid = @"Copy Id";

            public static LocalizedString delete = @"Delete";

            public static LocalizedString deleteconfirm = @"Are you sure you want to delete {00}?";

            public static LocalizedString newfolder = @"New Folder";

            public static LocalizedString newmap = @"New Map";

            public static LocalizedString rename = @"Rename";

            public static LocalizedString selectcurrent = @"Select Current Map";

            public static LocalizedString selecttodelete = @"Please select a folder or map to delete.";

            public static LocalizedString selecttorename = @"Please select a folder or map to rename.";

            public static LocalizedString title = @"Map List";

        }

        public struct Mapping
        {

            public static LocalizedString createmap = @"Create new map.";

            public static LocalizedString createmapdialogue = @"Do you want to create a map here?";

            public static LocalizedString diagonalwarning = @"Cannot create maps diagonally!";

            public static LocalizedString editortitle = @"Map Editor";

            public static LocalizedString eraselayer = @"Erase Layer";

            public static LocalizedString eraselayerdialogue = @"Are you sure you want to erase this layer?";

            public static LocalizedString filllayer = @"Fill Layer";

            public static LocalizedString filllayerdialogue = @"Are you sure you want to fill this layer?";

            public static LocalizedString newmap = @"Are you sure you want to create a new, unconnected map?";

            public static LocalizedString newmapcaption = @"New Map";

            public static LocalizedString savemap = @"Save current map?";

            public static LocalizedString savemapdialogue = @"Do you want to save your current map?";

            public static LocalizedString savemapdialoguesure = @"Are you sure you want to save your current map?";

            public static LocalizedString mapnotsaved = @"Unsaved changes!";

            public static LocalizedString maphaschangesdialog =
                @"There are unsaved changes to this map, are you sure you want to exit?";

        }

        public struct MapProperties
        {

            public static Dictionary<string, string> categories = new Dictionary<string, string>()
            {
                {"general", @"General"},
                {"lighting", @"Lighting"},
                {"misc", @"Misc"},
                {"overlay", @"Overlay"},
                {"fog", @"Fog"},
                {"audio", @"Audio"},
                {"weather", @"Weather"}
            };

            public static Dictionary<string, string> displaynames = new Dictionary<string, string>()
            {
                {"ahue", @"AHue"},
                {"bhue", @"BHue"},
                {"brightness", @"Brightness"},
                {"fog", @"Fog"},
                {"fogalpha", @"Fog Alpha"},
                {"fogxspeed", @"Fog X Speed"},
                {"fogyspeed", @"Fog Y Speed"},
                {"ghue", @"GHue"},
                {"isindoors", @"Is Indoors"},
                {"music", @"Music"},
                {"name", @"Name"},
                {"overlaygraphic", @"Overlay Graphic"},
                {"panorama", @"Panorama"},
                {"playerlightcolor", @"Player Light Color"},
                {"playerlightexpand", @"Player Light Expand"},
                {"playerlightintensity", @"Player Light Intensity"},
                {"playerlightsize", @"Player Light Size"},
                {"rhue", @"RHue"},
                {"sound", @"Sound"},
                {"zonetype", @"Zone Type"},
                {"weather", @"Weather"},
                {"weatherxspeed", @"Weather X Speed"},
                {"weatheryspeed", @"Weather Y Speed"},
                {"weatherintensity", @"Weather Intensity"},
            };

            public static Dictionary<string, string> descriptions = new Dictionary<string, string>()
            {
                {
                    "ahuedesc",
                    @"How strong the overlay appears. (Range: 0 [transparent/invisible] to 255 [solid/can't see map])"
                },
                {"bhuedesc", @"The amount of blue in the overlay. (Range: 0 to 255)"},
                {"brightnessdesc", @"How bright is this map? (Range: 0 to 100)."},
                {
                    "fogalphadesc",
                    @"How strong the fog overlay appears. (Range: 0 [transparent/invisible] to 255 [solid/can't see map])"
                },
                {"fogdesc", @"The overlayed image on the map. Generally used for fogs or sun beam effects."},
                {"fogxspeeddesc", @"Fog Horizontal Speed (Range: -5 to 5)"},
                {"fogyspeeddesc", @"Fog Vertical Speed (Range: -5 to 5)"},
                {"ghuedesc", @"The amount of green in the overlay. (Range: 0 to 255)"},
                {"isindoorsdesc", @"Is this map indoors?"},
                {"musicdesc", @"Looping background music for this map."},
                {"namedesc", @"The name of this map."},
                {"overlaygraphicdesc", @"This is an image that appears above the map. (Not shown in editor.)"},
                {
                    "panoramadesc",
                    @"This is an image that appears behind the map. It can be seen where no tiles are placed."
                },
                {"playerlightcolordesc", @"Which color is the players light? (Default: White)"},
                {"playerlightexpanddesc", @"How far into the light does the effect start fading? (0.00 to 1.00)"},
                {"playerlightintensitydesc", @"How strong the light is at its brightest point. (0 to 255)"},
                {"playerlightsizedesc", @"How large is the light around the player? (In pixels 0-1000)"},
                {"rhuedesc", @"The amount of red in the overlay. (Range: 0 to 255)"},
                {"sounddesc", @"Looping sound effect for this map."},
                {"zonedesc", @"The type of map this is."},
                {"weatherdesc", @"The animation for each weather particle."},
                {
                    "weatherxspeeddesc",
                    @"How fast horizontally weather particles move across the screen. (Range -5 to 5)"
                },
                {"weatheryspeeddesc", @"How fast vertically weather particles move across the screen. (Range -5 to 5)"},
                {"weatherintensitydesc", @"How intence the weather is (number of particles). (Range 0 to 100)"},
            };

            public static LocalizedString title = @"Map Properties";

            public static Dictionary<int, LocalizedString> zones = new Dictionary<int, LocalizedString>
            {
                {0, @"Normal"},
                {1, @"Safe"},
                {2, @"Arena"},
            };

        }

        public struct NpcEditor
        {

            public static LocalizedString abilitypower = @"Ability Pwr:";

            public static LocalizedString addhostility = @"Add";

            public static LocalizedString addspell = @"Add";

            public static LocalizedString aggressive = @"Aggressive";

            public static LocalizedString attack = @"Attack:";

            public static LocalizedString attackallies = @"Attack Allies?";

            public static LocalizedString attackanimation = @"Attack Animation:";

            public static LocalizedString attackonsightconditions = @"Attack Player on Sight";

            public static LocalizedString attackspeed = @"Attack Speed";

            public static LocalizedString attackspeedmodifier = @"Modifier:";

            public static Dictionary<int, LocalizedString> attackspeedmodifiers = new Dictionary<int, LocalizedString>
            {
                {0, @"Disabled"},
                {1, @"Static (ms)"},
            };

            public static LocalizedString attackspeedvalue = @"Value:";

            public static LocalizedString basedamage = @"Base Damage:";

            public static LocalizedString behavior = @"Behavior:";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString combat = @"Combat";

            public static LocalizedString commonevents = @"Common Events";

            public static LocalizedString conditions = @"Conditions";

            public static LocalizedString copy = @"Copy Npc";

            public static LocalizedString critchance = @"Crit Chance (%):";

            public static LocalizedString critmultiplier = @"Crit Multiplier (Default 1.5x):";

            public static LocalizedString damagetype = @"Damage Type:";

            public static LocalizedString defense = @"Defense:";

            public static LocalizedString delete = @"Delete Npc";

            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete this npc? This action cannot be reverted!";

            public static LocalizedString deletetitle = @"Delete Item";

            public static LocalizedString dontattackonsightconditions = @"Should Not Attack Player on Sight";

            public static LocalizedString drops = @"Drops";

            public static LocalizedString dropitem = @"Item:";

            public static LocalizedString dropamount = @"Amount:";

            public static LocalizedString dropchance = @"Chance (%):";

            public static LocalizedString dropadd = @"Add";

            public static LocalizedString dropremove = @"Remove";

            public static LocalizedString dropdisplay = @"{00} x{01} - {02}%";

            public static LocalizedString enabled = @"Enabled?";

            public static LocalizedString exp = @"Exp:";

            public static LocalizedString flee = @"Flee Health %";

            public static LocalizedString focusdamagedealer = @"Focus Highest Damage Dealer:";

            public static LocalizedString folderlabel = @"Folder:";

            public static LocalizedString foldertitle = @"Add Folder";

            public static LocalizedString folderprompt = @"Enter a name for the folder you'd like to add:";

            public static LocalizedString frequency = @"Freq:";

            public static Dictionary<int, LocalizedString> frequencies = new Dictionary<int, LocalizedString>
            {
                {0, @"Not Very Often"},
                {1, @"Not Often"},
                {2, @"Normal"},
                {3, @"Often"},
                {4, @"Very Often"},
            };

            public static LocalizedString general = @"General";

            public static LocalizedString hp = @"HP:";

            public static LocalizedString hpregen = @"HP (%);";

            public static LocalizedString individualizedloot = @"Spawn loot for all attackers?";

            public static LocalizedString magicresist = @"Magic Resist:";

            public static LocalizedString mana = @"Mana:";

            public static LocalizedString movement = @"Movement";

            public static LocalizedString resetradius = @"Reset Radius:";

            public static Dictionary<int, LocalizedString> movements = new Dictionary<int, LocalizedString>
            {
                {0, @"Move Randomly"},
                {1, @"Turn Randomly"},
                {2, @"Stand Still"},
                {3, @"Static"},
            };

            public static LocalizedString mpregen = @"MP (%):";

            public static LocalizedString name = @"Name:";

            public static LocalizedString New = @"New Npc";

            public static LocalizedString npc = @"NPC:";

            public static LocalizedString npcs = @"Npcs";

            public static LocalizedString npcvsnpc = @"NPC vs NPC Combat/Hostility  ";

            public static LocalizedString ondeathevent = @"On Death (for killer):";

            public static LocalizedString ondeathpartyevent = @"On Death (for party):";

            public static LocalizedString paste = @"Paste Npc";

            public static LocalizedString playercanattackconditions = @"Player Can Attack (Default: True)";

            public static LocalizedString playerfriendprotectorconditions = @"Player Friend/Protector";

            public static LocalizedString regen = @"Regen";

            public static LocalizedString regenhint = @"% of HP/Mana to restore per tick.

Tick timer saved in server config.json.";

            public static LocalizedString removehostility = @"Remove";

            public static LocalizedString removespell = @"Remove";

            public static LocalizedString save = @"Save";

            public static LocalizedString scalingamount = @"Scaling Amount (%):";

            public static LocalizedString scalingstat = @"Scaling Stat:";

            public static LocalizedString searchplaceholder = @"Search...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sortalphabetically = @"Order Alphabetically";

            public static LocalizedString sightrange = @"Sight Range:";

            public static LocalizedString spawnduration = @"Spawn Duration: (ms)";

            public static LocalizedString speed = @"Speed:";

            public static LocalizedString spell = @"Spell:";

            public static LocalizedString spells = @"Spells";

            public static LocalizedString sprite = @"Sprite";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Red = @"Red:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Green = @"Green:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Blue = @"Blue:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Alpha = @"Alpha:";

            public static LocalizedString stats = @"Stats:";

            public static LocalizedString swarm = @"Swarm";

            public static LocalizedString title = @"Npc Editor";

            public static LocalizedString undo = @"Undo Changes";

            public static LocalizedString undoprompt =
                @"Are you sure you want to undo changes made to this npc? This action cannot be reverted!";

            public static LocalizedString undotitle = @"Undo Changes";

        }

        public struct NpcSpawns
        {

            public static LocalizedString add = @"Add";

            public static LocalizedString addremove = @"Add/Remove Map NPCs";

            public static LocalizedString declaredlocation = @"Declared";

            public static LocalizedString direction = @"Direction:";

            public static LocalizedString randomdirection = @"Random";

            public static LocalizedString randomlocation = @"Random";

            public static LocalizedString remove = @"Remove";

            public static LocalizedString spawndeclared = @"Spawn Location: Declared";

            public static LocalizedString spawnrandom = @"Spawn Location: Random";

        }

        public struct Options
        {

            public static LocalizedString browsebtn = @"Browse";

            public static LocalizedString dialogueallfiles = @"All Files";

            public static LocalizedString dialogueheader = @"Browse for client...";

            public static LocalizedString generaltab = @"General";

            public static LocalizedString pathgroup = @"Client Path";

            public static LocalizedString tilesetwarning = @"Suppress large tileset size warning.";

            public static LocalizedString title = @"Options";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString UpdateTab = @"Update";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString PackageUpdates = @"Package assets when generating updates.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString PackageOptions = @"Asset Packing Options";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString MusicBatch = @"Music Batch Size";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString SoundBatch = @"Sound Batch Size";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString TextureSize = @"Texture Pack Size";
        }

        public struct ProgressForm
        {

            public static LocalizedString cancel = @"Cancel";

        }

        public struct ProjectileEditor
        {

            public static LocalizedString addanimation = @"Add";

            public static LocalizedString ammo = @"Ammunition Requirements:";

            public static LocalizedString ammoamount = @"Amount:";

            public static LocalizedString ammoitem = @"Item:";

            public static LocalizedString animation = @"Animation:";

            public static LocalizedString animationline = @"[Spawn Range: {00} - {01}]  Animation: {02}";

            public static LocalizedString animations = @"Animations";

            public static LocalizedString autorotate = @"Auto Rotate Animation?";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString collisions = @"Ignore Collision:";

            public static LocalizedString copy = @"Copy Projectile";

            public static LocalizedString delete = @"Delete Projectile";

            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete this projectile? This action cannot be reverted!";

            public static LocalizedString deletetitle = @"Delete Projectile";

            public static LocalizedString folderlabel = @"Folder:";

            public static LocalizedString foldertitle = @"Add Folder";

            public static LocalizedString folderprompt = @"Enter a name for the folder you'd like to add:";

            public static LocalizedString grapple = @"Grapple hook?";

            public static LocalizedString ignoreactiveresources = @"Active Resources";

            public static LocalizedString ignoreblocks = @"Map Blocks";

            public static LocalizedString ignoreinactiveresources = @"Inactive Resources";

            public static LocalizedString ignorezdimension = @"Z-Dimension Blocks";

            public static LocalizedString piercetarget = @"Pierce Target?";

            public static LocalizedString knockback = @"Knockback:";

            public static LocalizedString name = @"Name:";

            public static LocalizedString New = @"New Projectile";

            public static LocalizedString paste = @"Paste Projectile";

            public static LocalizedString projectiles = @"Projectiles";

            public static LocalizedString properties = @"Properties";

            public static LocalizedString quantity = @"Quantity:";

            public static LocalizedString range = @"Range:";

            public static LocalizedString removeanimation = @"Remove";

            public static LocalizedString save = @"Save";

            public static LocalizedString searchplaceholder = @"Search...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sortalphabetically = @"Order Alphabetically";

            public static LocalizedString spawndelay = @"Spawn Delay (ms):";

            public static LocalizedString spawnrange = @"Spawn Range: {00} - {01}";

            public static LocalizedString spawns = @"Projectile Spawns";

            public static LocalizedString speed = @"Speed (ms):";

            public static LocalizedString spell = @"Collision Spell:";

            public static LocalizedString title = @"Projectile Editor";

            public static LocalizedString undo = @"Undo Changes";

            public static LocalizedString undoprompt =
                @"Are you sure you want to undo changes made to this projectile? This action cannot be reverted!";

            public static LocalizedString undotitle = @"Undo Changes";

        }

        public struct QuestEditor
        {

            public static LocalizedString actions = @"Quest Actions:";

            public static LocalizedString addtask = @"Add Task";

            public static LocalizedString beforeofferdesc = @"Before Offer Description:";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString completeddesc = @"Completed Description:";

            public static LocalizedString copy = @"Copy Quest";

            public static LocalizedString delete = @"Delete Quest";

            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete this quest? This action cannot be reverted!";

            public static LocalizedString deletetitle = @"Delete Quest";

            public static LocalizedString editendevent = @"Edit Quest Completion Event";

            public static LocalizedString editrequirements = @"Edit Quest Requirements";

            public static LocalizedString editstartevent = @"Edit Quest Start Event";

            public static LocalizedString endevent = @"Quest: {00} - Completion Event";

            public static LocalizedString folderlabel = @"Folder:";

            public static LocalizedString foldertitle = @"Add Folder";

            public static LocalizedString folderprompt = @"Enter a name for the folder you'd like to add:";

            public static LocalizedString general = @"General";

            public static LocalizedString inprogressdesc = @"In Progress Description:";

            public static LocalizedString logoptions = @"Quest Log Options";

            public static LocalizedString name = @"Name:";

            public static LocalizedString New = @"New Quest";

            public static LocalizedString offerdesc = @"Offer Description:";

            public static LocalizedString onend = @"On Quest Completion:";

            public static LocalizedString onstart = @"On Quest Start:";

            public static LocalizedString options = @"Progression Options";

            public static LocalizedString paste = @"Paste Quest";

            public static LocalizedString quests = @"Quests";

            public static LocalizedString quit = @"Can Quit Quest?";

            public static LocalizedString removetask = @"Remove Task";

            public static LocalizedString repeatable = @"Quest Repeatable?";

            public static LocalizedString requirements = @"Quest Requirements";

            public static LocalizedString save = @"Save";

            public static LocalizedString searchplaceholder = @"Search...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sortalphabetically = @"Order Alphabetically";

            public static LocalizedString showafter = @"Show in quest log after completing quest?";

            public static LocalizedString showbefore = @"Show in quest log before accepting quest?";

            public static LocalizedString startevent = @"Quest: {00} - Start Event";

            public static LocalizedString tasks = @"Quest Tasks";

            public static LocalizedString title = @"Quest Editor";

            public static LocalizedString undo = @"Undo Changes";

            public static LocalizedString undoprompt =
                @"Are you sure you want to undo changes made to this quest? This action cannot be reverted!";

            public static LocalizedString undotitle = @"Undo Changes";

            public static LocalizedString unstartedcategory = @"Unstarted Category:";

            public static LocalizedString inprogressgategory = @"In Progress Category:";

            public static LocalizedString completedcategory = @"Completed Category:";

            public static LocalizedString donotshowunlessreqsmet = @"Do not show in quest log unless requirements are met";

            public static LocalizedString order = @"Quest Log Sort Order:";
        }

        public struct ResourceEditor
        {

            public static LocalizedString animation = @"Animation:";

            public static LocalizedString belowentities = @"Below Entities";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString commonevent = @"Common Event";

            public static LocalizedString copy = @"Copy Resource";

            public static LocalizedString delete = @"Delete Resource";

            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete this resource? This action cannot be reverted!";

            public static LocalizedString deletetitle = @"Delete Item";

            public static LocalizedString drops = @"Drops";

            public static LocalizedString dropitem = @"Item:";

            public static LocalizedString dropamount = @"Amount:";

            public static LocalizedString dropchance = @"Chance (%):";

            public static LocalizedString dropadd = @"Add";

            public static LocalizedString dropremove = @"Remove";

            public static LocalizedString dropdisplay = @"{00} x{01} - {02}%";

            public static LocalizedString exhaustedgraphic = @"Exhausted Graphic:";

            public static LocalizedString folderlabel = @"Folder:";

            public static LocalizedString foldertitle = @"Add Folder";

            public static LocalizedString folderprompt = @"Enter a name for the folder you'd like to add:";

            public static LocalizedString fromtileset = @"From Tileset";

            public static LocalizedString general = @"General";

            public static LocalizedString graphics = @"Graphics";

            public static LocalizedString harvestevent = @"Event:";

            public static LocalizedString hpregen = @"HP (%);";

            public static LocalizedString initialgraphic = @"Initial Graphic:";

            public static LocalizedString maxhp = @"Max HP:";

            public static LocalizedString minhp = @"Min HP:";

            public static LocalizedString name = @"Name:";

            public static LocalizedString New = @"New Resource";

            public static LocalizedString paste = @"Paste Resource";

            public static LocalizedString regen = @"Regen";

            public static LocalizedString regenhint = @"% of HP to restore per tick.

Tick timer saved in server config.json.";

            public static LocalizedString requirementsgroup = @"Requirements";

            public static LocalizedString requirements = @"Harvesting Requirements";

            public static LocalizedString cannotharvest = @"Cannot Harvest Message:";

            public static LocalizedString resources = @"Resources";

            public static LocalizedString save = @"Save";

            public static LocalizedString searchplaceholder = @"Search...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sortalphabetically = @"Order Alphabetically";

            public static LocalizedString spawnduration = @"Spawn Duration: (ms)";

            public static LocalizedString title = @"Resource Editor";

            public static LocalizedString tooltype = @"Tool Type:";

            public static LocalizedString undo = @"Undo Changes";

            public static LocalizedString undoprompt =
                @"Are you sure you want to undo changes made to this resource? This action cannot be reverted!";

            public static LocalizedString undotitle = @"Undo Changes";

            public static LocalizedString walkableafter = @"Walkable after resource removal?";

            public static LocalizedString walkablebefore = @"Walkable before resource removal?";

        }

        public struct ShopEditor
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString addboughtitem = @"Add Item:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString addlabel = @"Add Item to be Sold:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString addsolditem = @"Add Selected";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString blacklist = @"Blacklist";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString buycost = @"Sell Amount:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString buydesc = @"Buy Item {00} For ({01}) Item {02}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString buyfor = @"Buy For:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString cancel = @"Cancel";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString copy = @"Copy Shop";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString defaultcurrency = @"Default Currency:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString delete = @"Delete Shop";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete this shop? This action cannot be reverted!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString deletetitle = @"Delete Item";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString dontbuy = @"Don't Buy Item {00}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString folderlabel = @"Folder:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString foldertitle = @"Add Folder";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString folderprompt = @"Enter a name for the folder you'd like to add:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString general = @"General";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString itemsboughtblacklist = @"Items Bought (Blacklist - Don't Buy Listed Items)  ";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString itemsboughtwhitelist = @"Items Bought (Whitelist - Buy Listed Items)  ";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString itemssold = @"Items Sold";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString name = @"Name:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString New = @"New Shop";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString paste = @"Paste Shop";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString removeboughtitem = @"Remove Selected";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString removesolditem = @"Remove Selected";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString save = @"Save";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString searchplaceholder = @"Search...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sortalphabetically = @"Order Alphabetically";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sellcost = @"Sell Cost:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString selldesc = @"Sell Item {00} For ({01}) Item {02}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sellfor = @"Sell for:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString shops = @"Shops";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString title = @"Shop Editor";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString undo = @"Undo Changes";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString undoprompt =
                @"Are you sure you want to undo changes made to this shop? This action cannot be reverted!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString undotitle = @"Undo Changes";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString whitelist = @"Whitelist";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString buysound = @"Buy Sound:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sellsound = @"Sell Sound:";
        }

        public struct SpellEditor
        {

            public static LocalizedString abilitypower = @"Ability Pwr:";

            public static LocalizedString attack = @"Attack:";

            public static LocalizedString boostduration = @"Stat Boost/Effect Duration";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString castanimation = @"Cast Animation:";

            public static LocalizedString castrange = @"Cast Range (tiles):";

            public static LocalizedString casttime = @"Cast Time (ms):";

            public static LocalizedString combatspell = @"Combat Spell";

            public static LocalizedString cooldown = @"Cooldown (ms):";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString CooldownGroup = @"Cooldown Group:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString CooldownGroupTitle = @"Add Cooldown Group";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString CooldownGroupPrompt = @"Enter a name for the cooldown group you'd like to add:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString IgnoreGlobalCooldown = @"Ignore Global Cooldown?";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString IgnoreCooldownReduction = @"Ignore Cooldown Reduction?";

            public static LocalizedString copy = @"Copy Spell";

            public static LocalizedString cost = @"Spell Cost:";

            public static LocalizedString critchance = @"Crit Chance (%):";

            public static LocalizedString critmultiplier = @"Crit Multiplier (Default 1.5x):";

            public static LocalizedString damagegroup = @"Damage";

            public static LocalizedString damagetype = @"Damage Type:";

            public static LocalizedString dash = @"Dash";

            public static LocalizedString dashcollisions = @"Ignore Collision:";

            public static LocalizedString dashrange = @"Dash Range (tiles): {00}";

            public static LocalizedString defense = @"Defense:";

            public static LocalizedString delete = @"Delete Spell";

            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete this spell? This action cannot be reverted!";

            public static LocalizedString deletetitle = @"Delete Spell";

            public static LocalizedString description = @"Desc:";

            public static LocalizedString duration = @"Duration: (ms)";

            public static Dictionary<int, LocalizedString> effects = new Dictionary<int, LocalizedString>
            {
                {0, @"None"},
                {1, @"Silence"},
                {2, @"Stun"},
                {3, @"Snare"},
                {4, @"Blind"},
                {5, @"Stealth"},
                {6, @"Transform"},
                {7, @"Cleanse"},
                {8, @"Invulnerable"},
                {9, @"Shield"},
                {10, @"Sleep"},
                {11, @"OnHit"},
                {12, @"Taunt"},
            };

            public static LocalizedString effectgroup = @"Effect";

            public static LocalizedString effectlabel = @"Extra Effect:";

            public static LocalizedString Event = @"Event";

            public static LocalizedString eventlabel = @"Event:";

            public static LocalizedString folderlabel = @"Folder:";

            public static LocalizedString foldertitle = @"Add Folder";

            public static LocalizedString folderprompt = @"Enter a name for the folder you'd like to add:";

            public static LocalizedString friendly = @"Friendly";

            public static LocalizedString general = @"General";

            public static LocalizedString hitanimation = @"Hit Animation:";

            public static LocalizedString bound = @"Bound?";

            public static LocalizedString hitradius = @"Hit Radius:";

            public static LocalizedString hotdot = @"Heal/Damage Over Time";

            public static LocalizedString hotdottick = @"Tick (ms):";

            public static LocalizedString hpcost = @"HP Cost:";

            public static LocalizedString hpdamage = @"HP Damage:";

            public static LocalizedString icon = @"Icon:";

            public static LocalizedString ignoreactiveresources = @"Active Resources";

            public static LocalizedString ignoreblocks = @"Map Blocks";

            public static LocalizedString ignoreinactiveresources = @"Inactive Resources";

            public static LocalizedString ignorezdimension = @"Z-Dimension Blocks";

            public static LocalizedString ishotdot = @"HOT/DOT?";

            public static LocalizedString tickanimation = @"Tick Animation:";

            public static LocalizedString magicresist = @"Magic Resist:";

            public static LocalizedString manacost = @"Mana Cost:";

            public static LocalizedString mpdamage = @"Mana Damage:";

            public static LocalizedString name = @"Name:";

            public static LocalizedString New = @"New Spell";

            public static LocalizedString paste = @"Paste Spell";

            public static LocalizedString projectile = @"Projectile:";

            public static LocalizedString requirements = @"Casting Requirements";

            public static LocalizedString cannotcast = @"Cannot Cast Message:";

            public static LocalizedString requirementsbutton = @"Edit Casting Requirements";

            public static LocalizedString save = @"Save";

            public static LocalizedString scalingamount = @"Scaling Amount (%):";

            public static LocalizedString scalingstat = @"Scaling Stat:";

            public static LocalizedString searchplaceholder = @"Search...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sortalphabetically = @"Order Alphabetically";

            public static LocalizedString speed = @"Speed:";

            public static LocalizedString spells = @"Spells";

            public static LocalizedString stats = @"Stat Modifiers";

            public static LocalizedString targetting = @"Targetting Info";

            public static LocalizedString targettype = @"Target Type:";

            public static Dictionary<int, LocalizedString> targettypes = new Dictionary<int, LocalizedString>
            {
                {0, @"Self"},
                {1, @"Single Target (includes self)"},
                {2, @"AOE"},
                {3, @"Linear (projectile)"},
                {4, @"On Hit"},
                {5, @"Trap"}
            };

            public static LocalizedString title = @"Spell Editor";

            public static LocalizedString transformsprite = @"Sprite:";

            public static LocalizedString type = @"Type:";

            public static Dictionary<int, LocalizedString> types = new Dictionary<int, LocalizedString>
            {
                {0, @"Combat Spell"},
                {1, @"Warp to Map"},
                {2, @"Warp to Target"},
                {3, @"Dash"},
                {4, @"Event"},
            };

            public static LocalizedString undo = @"Undo Changes";

            public static LocalizedString undoprompt =
                @"Are you sure you want to undo changes made to this spell? This action cannot be reverted!";

            public static LocalizedString undotitle = @"Undo Changes";

            public static LocalizedString warptomap = @"Warp Caster:";

        }

        public struct VariableEditor
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString delete = @"Delete";

            public static LocalizedString deletecaption = @"Delete?";

            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete this variable? This action cannot be reverted!";

            public static LocalizedString editor = @"Variable Editor";

            public static LocalizedString False = @"False";

            public static LocalizedString folderlabel = @"Folder:";

            public static LocalizedString foldertitle = @"Add Folder";

            public static LocalizedString folderprompt = @"Enter a name for the folder you'd like to add:";

            public static LocalizedString globalvariable = @"Server Variable";

            public static LocalizedString globalvariables = @"Global Variables";

            public static LocalizedString list = @"Variable List";

            public static LocalizedString name = @"Name:";

            public static LocalizedString New = @"New";

            public static LocalizedString playervariable = @"Player Variable";

            public static LocalizedString playervariables = @"Player Variables";

            public static LocalizedString save = @"Save";

            public static LocalizedString searchplaceholder = @"Search...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString sortalphabetically = @"Order Alphabetically";

            public static LocalizedString textidgv = @"Text Id: \gv ";

            public static LocalizedString textidpv = @"Text Id: \pv ";

            public static LocalizedString title = @"Variable Editor";

            public static LocalizedString True = @"True";

            public static LocalizedString type = @"Variable Type";

            public static Dictionary<int, LocalizedString> types = new Dictionary<int, LocalizedString>
            {
                {1, @"Boolean"},
                {2, @"Integer"},
                {3, @"String"},
            };

            public static LocalizedString undo = @"Undo Changes";

            public static LocalizedString value = @"Value:";

        }

        public struct TaskEditor
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString completionevent = @"Quest: {00} - Task Completion Event";

            public static LocalizedString desc = @"Desc:";

            public static LocalizedString editcompletionevent = @"Edit Task Completion Event";

            public static LocalizedString editor = @"Task Editor";

            public static LocalizedString eventdriven =
                @"Event Driven: The description should lead the player to an event. The event will then complete the task using the complete quest task command.";

            public static LocalizedString gatheramount = @"Amount:";

            public static LocalizedString gatheritems = @"Gather Item(s)";

            public static LocalizedString item = @"Item:";

            public static LocalizedString killnpcs = @"Kill NPC(s)";

            public static LocalizedString npc = @"NPC:";

            public static LocalizedString npcamount = @"Amount:";

            public static LocalizedString ok = @"Ok";

            public static LocalizedString title = @"Add/Edit Quest Task";

            public static LocalizedString type = @"Task Type:";

            public static Dictionary<int, LocalizedString> types = new Dictionary<int, LocalizedString>
            {
                {0, @"Event Driven"},
                {1, @"Gather Item(s)"},
                {2, @"Kill NPC(s)"},
            };

            public static Dictionary<int, LocalizedString> descriptions = new Dictionary<int, LocalizedString>
            {
                {0, @"Event Driven - {00}"},
                {1, @"Gather Items [{00} x{01}] - {02}"},
                {2, @"Kill Npc(s) [{00} x{01}] - {02}"},
            };

        }

        public struct AssetPacking
        {

            public static LocalizedString title = "Packing assets, please wait!";

            public static LocalizedString deleting = "Deleting old packs...";

            public static LocalizedString collecting = "Creating texture packing list...";

            public static LocalizedString calculating = "Calculating texture rects...";

            public static LocalizedString exporting = "Exporting textures...";

            public static LocalizedString sounds = "Packing up sounds...";

            public static LocalizedString music = "Packing up music...";

            public static LocalizedString done = "Done!";

        }

        public struct Tiles
        {

            public static LocalizedString animated = @"Animated  [VX Format]";

            public static LocalizedString animatedxp = @"Animated  [XP Format]";

            public static LocalizedString autotile = @"Autotile     [VX Format]";

            public static LocalizedString autotilexp = @"Autotile     [XP Format]";

            public static LocalizedString cliff = @"Cliff           [VX Format]";

            public static LocalizedString fake = @"Fake         [VX Format]";

            public static LocalizedString layer = @"Layer:";

            public static Dictionary<string, LocalizedString> maplayers = new Dictionary<string, LocalizedString>
            {
                { @"ground", @"Ground" },
                { @"mask 1", @"Mask 1" },
                { @"mask 2", @"Mask 2" },
                { @"fringe 1", @"Fringe 1" },
                { @"fringe 2", @"Fringe 2" }
            };

            public static LocalizedString normal = @"Normal";

            public static LocalizedString tileset = @"Tileset:";

            public static LocalizedString tiletype = @"Tile Type:";

            public static LocalizedString waterfall = @"Waterfall   [VX Format]";

        }

        public struct TimeEditor
        {

            public static LocalizedString brightness = @"Brightness: {00}%";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString colorpaneldesc = @"Double click the panel above to change the overlay color.";

            public static LocalizedString interval = @"Invervals:";

            public static Dictionary<int, LocalizedString> intervals = new Dictionary<int, LocalizedString>
            {
                {0, @"24 hours"},
                {1, @"12 hours"},
                {2, @"8 hours"},
                {3, @"6 hours"},
                {4, @"4 hours"},
                {5, @"3 hours"},
                {6, @"2 hours"},
                {7, @"1 hour"},
                {8, @"45 minutes"},
                {9, @"30 minutes"},
                {10, @"15 minutes"},
                {11, @"10 minutes"},
            };

            public static LocalizedString overlay = @"Range Overlay";

            public static LocalizedString rate = @"Time Rate:";

            public static LocalizedString ratedesc = @"Enter 1 for normal rate of time.
Values larger than one for faster days. 
Values between 0 and 1 for longer days. 
Negative values for time to flow backwards.";

            public static LocalizedString ratesuffix = @"x Normal";

            public static LocalizedString save = @"Save";

            public static LocalizedString settings = @"Time Settings";

            public static LocalizedString sync = @"Sync With Server";

            public static LocalizedString times = @"Time of Day";

            public static LocalizedString title = @"Time Editor (Day/Night Settings)";

            public static LocalizedString to = @"to";

        }
        
        public struct Update
        {

            public static LocalizedString Title = @"Intersect Editor - Updating";

            public static LocalizedString Checking = @"Checking for updates, please wait!";

            public static LocalizedString Updating = @"Downloading updates, {00}% done!";

            public static LocalizedString Restart = @"Update complete! Relaunching!";

            public static LocalizedString Done = @"Update complete! Launching game!";

            public static LocalizedString Error = @"Error: {00}";

            public static LocalizedString Files = @"{00} Files Remaining";

            public static LocalizedString Size = @"{00} Left";

            public static LocalizedString Percent = @"{00}%";

        }

        public struct UpdatePacking
        {

            public static LocalizedString Title = @"Packaging Updater Files, Please Wait!";

            public static LocalizedString Deleting = @"Deleting existing or unchanged files..";

            public static LocalizedString Differential = @"An update already exists in this folder, would you like to generate a differential update (only files that have changed)?";

            public static LocalizedString DifferentialTitle = @"Create differential update?";

            public static LocalizedString Empty = @"You must select an empty folder, or a folder already containing an Intersect update!";

            public static LocalizedString InvalidBase = @"You cannot create the update within the editor folder, the update would include itself!";

            public static LocalizedString Error = @"Error!";

            public static LocalizedString Calculating = @"Calculating checksums, and creating update list...";

            public static LocalizedString Done = @"Done!";

        }

        public struct Warping
        {

            public static LocalizedString direction = @"Dir: {00}";

            public static LocalizedString map = @"Map: {00}";

            public static LocalizedString visual = @"Open Visual Interface";

            public static LocalizedString x = @"X: {00}";

            public static LocalizedString y = @"Y: {00}";

        }

        public struct WarpSelection
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString alphabetical = @"Alphabetical";

            public static LocalizedString maplist = @"Map List";

            public static LocalizedString mappreview = @"Map Preview";

            public static LocalizedString mapselectiontitle = @"Map Selection";

            public static LocalizedString okay = @"Ok";

            public static LocalizedString title = @"Warp Tile Selection";

        }

    }

}
