using System;
using System.Collections.Generic;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects.Events
{
    public class EventCommand
    {
        public EventCommandType Type;
        public string[] Strs = new string[6];
        public int[] Ints = new int[6];
        public EventMoveRoute Route;
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
            Type = (EventCommandType)myBuffer.ReadInteger();
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
            myBuffer.WriteInteger((int)Type);
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
            string output = "";
            switch (Ints[0])
            {
                case 0: //Player Switch
                    return "Player Switch " + PlayerSwitchBase.GetName(Ints[1]) + " is " + Convert.ToBoolean(Ints[2]);
                case 1: //Player Variables
                    output = "Player Variable " + PlayerVariableBase.GetName(Ints[1]);
                    switch (Ints[2])
                    {
                        case 0:
                            output += " is equal to ";
                            break;
                        case 1:
                            output += " is greater than or equal to ";
                            break;
                        case 2:
                            output += " is less than or equal to ";
                            break;
                        case 3:
                            output += " is greater than ";
                            break;
                        case 4:
                            output += " is less than ";
                            break;
                        case 5:
                            output += " does not equal ";
                            break;
                    }
                    output += Ints[3];
                    return output;
                case 2: //Global Switch
                    return "Global Switch " + ServerSwitchBase.GetName(Ints[1]) + " is " + Convert.ToBoolean(Ints[2]);
                case 3: //Global Variables
                    output = "Global Variable " + ServerVariableBase.GetName(Ints[1]);
                    switch (Ints[2])
                    {
                        case 0:
                            output += " is equal to ";
                            break;
                        case 1:
                            output += " is greater than or equal to ";
                            break;
                        case 2:
                            output += " is less than or equal to ";
                            break;
                        case 3:
                            output += " is greater than ";
                            break;
                        case 4:
                            output += " is less than ";
                            break;
                        case 5:
                            output += " does not equal ";
                            break;
                    }
                    output += Ints[3];
                    return output;
                case 4: //Has Item
                    return "Player has at least " + Ints[2] + " of Item " + ItemBase.GetName(Ints[1]);
                case 5: //Class Is
                    return "Player's class is " + ClassBase.GetName(Ints[1]);
                case 6: //Knows spell
                    return "Player knows Spell " + SpellBase.GetName(Ints[1]);
                case 7: //Level is
                    output = "Player's level";
                    switch (Ints[1])
                    {
                        case 0:
                            output += " is equal to ";
                            break;
                        case 1:
                            output += " is greater than or equal to ";
                            break;
                        case 2:
                            output += " is less than or equal to ";
                            break;
                        case 3:
                            output += " is greater than ";
                            break;
                        case 4:
                            output += " is less than ";
                            break;
                        case 5:
                            output += " does not equal ";
                            break;
                    }
                    output += Ints[2];
                    return output;
                case 8: //Self Switch
                    return "Self Switch " + (char)('A' + Ints[1]) + " is " + Convert.ToBoolean(Ints[2]);
                case 9: //Power is
                    output = "Player's Power is";
                    switch (Ints[1])
                    {
                        case 0:
                            output += " Mod or Admin";
                            break;
                        case 1:
                            output += " Admin";
                            break;
                    }
                    return output;
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
                    output = "Time is between ";
                    if (Ints[1] > -1 && Ints[1] < timeRanges.Count)
                    {
                        output += timeRanges[Ints[1]] + " and";
                    }
                    else
                    {
                        output += "invalid and";
                    }
                    if (Ints[2] > -1 && Ints[2] < timeRanges.Count)
                    {
                        output += timeRanges[Ints[2]];
                    }
                    else
                    {
                        output += "invalid";
                    }
                    return output;
                case 11: //Can Start Quest...
                    return "Can Start Quest: " + QuestBase.GetName(Ints[1]) + "";
                case 12: //Quest In Progress...
                    var quest = QuestBase.GetQuest(Ints[1]);
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
                        switch (Ints[2])
                        {
                            case 1:
                                return "Quest In Progress: " + QuestBase.GetName(Ints[1]) + ", Before Task: " + task.GetTaskString();
                            case 2:
                                return "Quest In Progress: " + QuestBase.GetName(Ints[1]) + ", After Task: " + task.GetTaskString();
                            case 3:
                                return "Quest In Progress: " + QuestBase.GetName(Ints[1]) + ", On Task: " + task.GetTaskString();
                            default:
                                return "Quest In Progress: " + QuestBase.GetName(Ints[1]) + ", On Any Task";
                        }
                    }
                    return "Quest In Progress: Deleted Quest";
                case 13: //Quest Completed
                    return "Quest is Completed: " + QuestBase.GetName(Ints[1]);
                case 14: //Player death
                    return "Player death";
                case 15: //No NPCs on map
                    return "No NPCs on the map";
            }
            return "";
        }
    }
}
