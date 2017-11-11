using System;
using System.Collections.Generic;
using Intersect.Localization;

namespace Intersect.GameObjects.Events
{
    public class EventCommand
    {
        public int[] Ints = new int[6];
        public EventMoveRoute Route;
        public string[] Strs = new string[6];
        public EventCommandType Type;

        public EventCommand()
        {
            for (var i = 0; i < 6; i++)
            {
                Strs[i] = "";
                Ints[i] = 0;
            }
        }

        public void Load(ByteBuffer myBuffer)
        {
            Type = (EventCommandType) myBuffer.ReadInteger();
            for (var x = 0; x < 6; x++)
            {
                Strs[x] = myBuffer.ReadString();
                Ints[x] = myBuffer.ReadInteger();
            }
            if (Type == EventCommandType.SetMoveRoute)
            {
                Route = new EventMoveRoute();
                Route.Load(myBuffer);
            }
        }

        public void Save(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger((int) Type);
            for (var x = 0; x < 6; x++)
            {
                myBuffer.WriteString(Strs[x]);
                myBuffer.WriteInteger(Ints[x]);
            }
            if (Type == EventCommandType.SetMoveRoute)
            {
                Route.Save(myBuffer);
            }
        }

        public string GetConditionalDesc()
        {
            if (Type != EventCommandType.ConditionalBranch) return "";
            switch (Ints[0])
            {
                case 0: //Player Switch
                    var pValue = Strings.Get("eventconditiondesc", "false");
                    if (Convert.ToBoolean(Ints[2])) pValue = Strings.Get("eventconditiondesc", "true");
                    return Strings.Get("eventconditiondesc", "playerswitch", PlayerSwitchBase.GetName(Ints[1]), pValue);
                case 1: //Player Variables
                    var pVar = "";
                    switch (Ints[2])
                    {
                        case 0:
                            pVar = Strings.Get("eventconditiondesc", "equal", Ints[3]);
                            break;
                        case 1:
                            pVar = Strings.Get("eventconditiondesc", "greaterequal", Ints[3]);
                            break;
                        case 2:
                            pVar = Strings.Get("eventconditiondesc", "lessthanequal", Ints[3]);
                            break;
                        case 3:
                            pVar = Strings.Get("eventconditiondesc", "greater", Ints[3]);
                            break;
                        case 4:
                            pVar = Strings.Get("eventconditiondesc", "lessthan", Ints[3]);
                            break;
                        case 5:
                            pVar = Strings.Get("eventconditiondesc", "notequal", Ints[3]);
                            break;
                    }
                    return Strings.Get("eventconditiondesc", "playervariable", PlayerVariableBase.GetName(Ints[1]),
                        pVar);
                case 2: //Global Switch
                    var gValue = Strings.Get("eventconditiondesc", "false");
                    if (Convert.ToBoolean(Ints[2])) gValue = Strings.Get("eventconditiondesc", "true");
                    return Strings.Get("eventconditiondesc", "globalswitch", ServerSwitchBase.GetName(Ints[1]), gValue);
                case 3: //Global Variables
                    var gVar = "";
                    switch (Ints[2])
                    {
                        case 0:
                            gVar = Strings.Get("eventconditiondesc", "equal", Ints[3]);
                            break;
                        case 1:
                            gVar = Strings.Get("eventconditiondesc", "greaterequal", Ints[3]);
                            break;
                        case 2:
                            gVar = Strings.Get("eventconditiondesc", "lessthanequal", Ints[3]);
                            break;
                        case 3:
                            gVar = Strings.Get("eventconditiondesc", "greater", Ints[3]);
                            break;
                        case 4:
                            gVar = Strings.Get("eventconditiondesc", "lessthan", Ints[3]);
                            break;
                        case 5:
                            gVar = Strings.Get("eventconditiondesc", "notequal", Ints[3]);
                            break;
                    }
                    return Strings.Get("eventconditiondesc", "globalvariable", ServerVariableBase.GetName(Ints[1]),
                        gVar);
                case 4: //Has Item
                    return Strings.Get("eventconditiondesc", "hasitem", Ints[2], ItemBase.GetName(Ints[1]));
                case 5: //Class Is
                    return Strings.Get("eventconditiondesc", "class", ClassBase.GetName(Ints[1]));
                case 6: //Knows spell
                    return Strings.Get("eventconditiondesc", "knowsspell", SpellBase.GetName(Ints[1]));
                case 7: //Level or Stat is
                    var pLvl = "";
                    switch (Ints[1])
                    {
                        case 0:
                            pLvl = Strings.Get("eventconditiondesc", "equal", Ints[2]);
                            break;
                        case 1:
                            pLvl = Strings.Get("eventconditiondesc", "greaterequal", Ints[2]);
                            break;
                        case 2:
                            pLvl = Strings.Get("eventconditiondesc", "lessthanequal", Ints[2]);
                            break;
                        case 3:
                            pLvl = Strings.Get("eventconditiondesc", "greater", Ints[2]);
                            break;
                        case 4:
                            pLvl = Strings.Get("eventconditiondesc", "lessthan", Ints[2]);
                            break;
                        case 5:
                            pLvl = Strings.Get("eventconditiondesc", "notequal", Ints[2]);
                            break;
                    }
                    var lvlorstat = "";
                    switch (Ints[3])
                    {
                        case 0:
                            lvlorstat = Strings.Get("eventconditiondesc", "level");
                            break;
                        default:
                            lvlorstat = Strings.Get("combat", "stat" + (Ints[3] - 1));
                            break;
                    }
                    return Strings.Get("eventconditiondesc", "levelorstat", lvlorstat, pLvl);
                case 8: //Self Switch
                    var sValue = Strings.Get("eventconditiondesc", "false");
                    if (Convert.ToBoolean(Ints[2])) sValue = Strings.Get("eventconditiondesc", "true");
                    return Strings.Get("eventconditiondesc", "selfswitch",
                        Strings.Get("eventconditiondesc", "selfswitch" + Ints[1]), sValue);
                case 9: //Power is
                    if (Ints[1] == 0)
                    {
                        return Strings.Get("eventconditiondesc", "power",
                            Strings.Get("eventconditiondesc", "modadmin"));
                    }
                    else
                    {
                        return Strings.Get("eventconditiondesc", "power", Strings.Get("eventconditiondesc", "admin"));
                    }
                case 10: //Time is between
                    var timeRanges = new List<string>();
                    var time = new DateTime(2000, 1, 1, 0, 0, 0);
                    for (int i = 0; i < 1440; i += TimeBase.GetTimeBase().RangeInterval)
                    {
                        var addRange = time.ToString("h:mm:ss tt") + " to ";
                        time = time.AddMinutes(TimeBase.GetTimeBase().RangeInterval);
                        addRange += time.ToString("h:mm:ss tt");
                        timeRanges.Add(addRange);
                    }
                    var time1 = "";
                    var time2 = "";
                    if (Ints[1] > -1 && Ints[1] < timeRanges.Count)
                    {
                        time1 = timeRanges[Ints[1]];
                    }
                    else
                    {
                        time1 = Strings.Get("eventconditiondesc", "timeinvalid");
                    }
                    if (Ints[2] > -1 && Ints[2] < timeRanges.Count)
                    {
                        time2 = timeRanges[Ints[2]];
                    }
                    else
                    {
                        time2 = Strings.Get("eventconditiondesc", "timeinvalid");
                    }
                    return Strings.Get("eventconditiondesc", "time", time1, time2);
                case 11: //Can Start Quest...
                    return Strings.Get("eventconditiondesc", "startquest", QuestBase.GetName(Ints[1]));
                case 12: //Quest In Progress...
                    var quest = QuestBase.Lookup.Get<QuestBase>(Ints[1]);
                    if (quest != null)
                    {
                        QuestBase.QuestTask task = null;
                        foreach (var tsk in quest.Tasks)
                        {
                            if (tsk.Id == Ints[3])
                            {
                                task = tsk;
                            }
                        }
                        var taskName = task != null ? task.GetTaskString() : Strings.Get("eventconditiondesc", "tasknotfound");
                        switch (Ints[2])
                        {
                            case 1:
                                return Strings.Get("eventconditiondesc", "questinprogress", QuestBase.GetName(Ints[1]),
                                    Strings.Get("eventconditiondesc", "beforetask", taskName));
                            case 2:
                                return Strings.Get("eventconditiondesc", "questinprogress", QuestBase.GetName(Ints[1]),
                                    Strings.Get("eventconditiondesc", "aftertask", taskName));
                            case 3:
                                return Strings.Get("eventconditiondesc", "questinprogress", QuestBase.GetName(Ints[1]),
                                    Strings.Get("eventconditiondesc", "ontask", taskName));
                            default:
                                return Strings.Get("eventconditiondesc", "questinprogress", QuestBase.GetName(Ints[1]),
                                    Strings.Get("eventconditiondesc", "onanytask"));
                        }
                    }
                    return Strings.Get("eventconditiondesc", "questinprogress", QuestBase.GetName(Ints[1]));
                case 13: //Quest Completed
                    return Strings.Get("eventconditiondesc", "questcompleted", QuestBase.GetName(Ints[1]));
                case 14: //Player death
                    return Strings.Get("eventconditiondesc", "playerdeath");
                case 15: //No NPCs on map
                    return Strings.Get("eventconditiondesc", "nonpcsonmap");
                case 16: //Gender Is
                    return Strings.Get("eventconditiondesc", "gender",
                        (Ints[1] == 0
                            ? Strings.Get("eventconditiondesc", "male")
                            : Strings.Get("eventconditiondesc", "female")));
            }
            return "";
        }
    }
}