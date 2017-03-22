
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Conditions;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.Localization;
using Intersect_Server.Classes.Core;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Items;
using Intersect_Server.Classes.Maps;
using Intersect_Server.Classes.Networking;
using Intersect_Server.Classes.Spells;

namespace Intersect_Server.Classes.Entities
{
    public class EventInstance
    {
        public EventBase BaseEvent;
        public bool IsGlobal;
        public int MapNum;
        public int MyIndex;
        public Client MyClient;
        public Player MyPlayer;
        public int SpawnX;
        public int SpawnY;
        public int PageIndex;
        public bool[] SelfSwitch { get; set; }
        public bool HoldingPlayer = false;
        public long WaitTimer = 0;
        public EventPageInstance PageInstance;
        public EventPageInstance[] GlobalPageInstance;
        public int CurrentX;
        public int CurrentY;

        //Special conditions
        public bool PlayerHasDied = false;
        public bool NPCDeathTriggerd = false;

        public Stack<CommandInstance> CallStack = new Stack<CommandInstance>();

        public EventInstance(int index, Client client, EventBase baseEvent, int mapNum)
        {
            MyIndex = index;
            MyClient = client;
            MapNum = mapNum;
            MyPlayer = (Player)Globals.Entities[MyClient.EntityIndex];
            SelfSwitch = new bool[4];
            BaseEvent = baseEvent;
            CurrentX = baseEvent.SpawnX;
            CurrentY = baseEvent.SpawnY;
        }

        public EventInstance(EventBase baseEvent, int index, int mapNum) //Global constructor
        {
            IsGlobal = true;
            MapNum = mapNum;
            BaseEvent = baseEvent;
            MyIndex = index;
            SelfSwitch = new bool[4];
            GlobalPageInstance = new EventPageInstance[BaseEvent.MyPages.Count];
            CurrentX = baseEvent.SpawnX;
            CurrentY = baseEvent.SpawnY;
            for (int i = 0; i < BaseEvent.MyPages.Count; i++)
            {
                GlobalPageInstance[i] = new EventPageInstance(BaseEvent, BaseEvent.MyPages[i], index, MapNum, this, null);
            }
        }

        public void Update()
        {
            var sendLeave = false;
            if (PageInstance != null)
            {
                //Check for despawn
                if (PageInstance.ShouldDespawn())
                {
                    CurrentX = PageInstance.CurrentX;
                    CurrentY = PageInstance.CurrentY;
                    PageInstance = null;
                    PlayerHasDied = false;
                    if (HoldingPlayer)
                    {
                        PacketSender.SendReleasePlayer(MyClient, MapNum, MyIndex);
                        HoldingPlayer = false;
                    }
                    sendLeave = true;
                }
                else
                {
                    if (!IsGlobal) PageInstance.Update(CallStack.Count > 0); //Process movement and stuff that is client specific
                    if (CallStack.Count > 0)
                    {
                        if (CallStack.Peek().WaitingForResponse == CommandInstance.EventResponse.Shop && MyPlayer.InShop == -1)
                            CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                        if (CallStack.Peek().WaitingForResponse == CommandInstance.EventResponse.Crafting && MyPlayer.InCraft == -1)
                            CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                        if (CallStack.Peek().WaitingForResponse == CommandInstance.EventResponse.Bank && MyPlayer.InBank == false)
                            CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                        if (CallStack.Peek().WaitingForResponse == CommandInstance.EventResponse.Quest && !MyPlayer.QuestOffers.Contains(CallStack.Peek().ResponseIndex))
                            CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                        while (CallStack.Peek().WaitingForResponse == CommandInstance.EventResponse.None)
                        {
                            if (CallStack.Peek().WaitingForRoute > -2)
                            {
                                if (CallStack.Peek().WaitingForRoute == -1)
                                {
                                    if (MyPlayer.MoveRoute == null || (MyPlayer.MoveRoute.Complete && MyPlayer.MoveTimer < Globals.System.GetTimeMs()))
                                    {
                                        CallStack.Peek().WaitingForRoute = -2;
                                        CallStack.Peek().WaitingForRouteMap = -1;
                                    }
                                }
                                else
                                {
                                    //Check if the exist exists && if the move route is completed.
                                    for (var i = 0; i < MyPlayer.MyEvents.Count; i++)
                                    {
                                        if (MyPlayer.MyEvents[i] == null) continue;
                                        if (MyPlayer.MyEvents[i].MapNum == CallStack.Peek().WaitingForRouteMap &&
                                            MyPlayer.MyEvents[i].BaseEvent.MyIndex == CallStack.Peek().WaitingForRoute)
                                        {
                                            if (MyPlayer.MyEvents[i].PageInstance == null) break;
                                            if (!MyPlayer.MyEvents[i].PageInstance.MoveRoute.Complete) break;
                                            CallStack.Peek().WaitingForRoute = -2;
                                            CallStack.Peek().WaitingForRouteMap = -1;
                                            break;
                                        }
                                    }
                                }
                                if (CallStack.Peek().WaitingForRoute > -2) break;
                            }
                            else
                            {
                                if (CallStack.Peek().CommandIndex >=
                                    CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                        .Commands.Count)
                                {
                                    CallStack.Pop();
                                }
                                else
                                {
                                    if (WaitTimer < Globals.System.GetTimeMs())
                                    {
                                        ProcessCommand(
                                            CallStack.Peek().Page.CommandLists[
                                                CallStack.Peek().ListIndex]
                                                .Commands[CallStack.Peek().CommandIndex]);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                if (CallStack.Count == 0)
                                {
                                    PlayerHasDied = false;
                                    NPCDeathTriggerd = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (PageInstance.Trigger == 2)
                        {
                            var newStack = new CommandInstance(PageInstance.MyPage) { CommandIndex = 0, ListIndex = 0 };
                            CallStack.Push(newStack);
                        }
                    }
                }
            }

            if (PageInstance == null)
            {
                //Try to Spawn a PageInstance.. if we can
                for (int i = BaseEvent.MyPages.Count - 1; i >= 0; i--)
                {
                    if (CanSpawnPage(i, BaseEvent))
                    {
                        if (IsGlobal)
                        {
                            if (MapInstance.GetMap(MapNum).GetGlobalEventInstance(BaseEvent) != null)
                            {
                                PageInstance = new EventPageInstance(BaseEvent, BaseEvent.MyPages[i], BaseEvent.MyIndex, MapNum, this, MyClient, MapInstance.GetMap(MapNum).GetGlobalEventInstance(BaseEvent).GlobalPageInstance[i]);
                                sendLeave = false;
                                PageIndex = i;
                            }
                        }
                        else
                        {
                            PageInstance = new EventPageInstance(BaseEvent, BaseEvent.MyPages[i], BaseEvent.MyIndex, MapNum, this, MyClient);
                            sendLeave = false;
                            PageIndex = i;
                        }
                        break;
                    }
                }

                if (sendLeave)
                {
                    if (IsGlobal)
                    {
                        PacketSender.SendEntityLeaveTo(MyClient, BaseEvent.MyIndex, (int)EntityTypes.Event, MapNum);
                    }
                    else
                    {
                        PacketSender.SendEntityLeaveTo(MyClient, BaseEvent.MyIndex, (int)EntityTypes.Event, MapNum);
                    }
                }
            }


        }

        public bool CanSpawnPage(int pageIndex, EventBase eventStruct)
        {
            return EventInstance.MeetsConditionLists(eventStruct.MyPages[pageIndex].ConditionLists, MyPlayer, this);
        }

        public static bool MeetsConditionLists(ConditionLists lists, Player MyPlayer, EventInstance EventInstance, bool SingleList = true)
        {
            //If no condition lists then this passes
            if (lists.Lists.Count == 0)
                return true;

            for (int i = 0; i < lists.Lists.Count; i++)
            {
                if (MeetsConditionList(lists.Lists[i], MyPlayer, EventInstance)) //Checks to see if all conditions in this list are met
                {
                    //If all conditions are met.. and we only need a single list to pass then return true
                    if (SingleList)
                        return true;

                    continue;
                }

                //If not.. and we need all lists to pass then return false
                if (!SingleList)
                    return false;
            }
            //There were condition lists. If single list was true then we failed every single list and should return false.
            //If single list was false (meaning we needed to pass all lists) then we've made it.. return true.
            return !SingleList;
        }

        public static bool MeetsConditionList(ConditionList list, Player MyPlayer, EventInstance EventInstance)
        {
            for (int i = 0; i < list.Conditions.Count; i++)
            {
                if (!MeetsCondition(list.Conditions[i], MyPlayer, EventInstance)) return false;
            }
            return true;
        }

        public static bool MeetsCondition(EventCommand conditionCommand, Player MyPlayer, EventInstance EventInstance)
        {
            //For instance use PageInstance
            switch (conditionCommand.Ints[0])
            {
                case 0: //Player Switch
                    var switchVal = false;
                    if (MyPlayer.Switches.ContainsKey(conditionCommand.Ints[1]))
                        switchVal = MyPlayer.Switches[conditionCommand.Ints[1]];
                    if (switchVal == Convert.ToBoolean(conditionCommand.Ints[2]))
                    {
                        return true;
                    }
                    break;
                case 1: //Player Variable
                    var varVal = 0;
                    if (MyPlayer.Variables.ContainsKey(conditionCommand.Ints[1]))
                        varVal = MyPlayer.Variables[conditionCommand.Ints[1]];
                    switch (conditionCommand.Ints[2]) //Comparator
                    {
                        case 0: //Equal to
                            if (varVal == conditionCommand.Ints[3])
                                return true;
                            break;
                        case 1: //Greater than or equal to
                            if (varVal >= conditionCommand.Ints[3])
                                return true;
                            break;
                        case 2: //Less than or equal to
                            if (varVal <= conditionCommand.Ints[3])
                                return true;
                            break;
                        case 3: //Greater than
                            if (varVal > conditionCommand.Ints[3])
                                return true;
                            break;
                        case 4: //Less than
                            if (varVal < conditionCommand.Ints[3])
                                return true;
                            break;
                        case 5: //Does not equal
                            if (varVal != conditionCommand.Ints[3])
                                return true;
                            break;
                    }
                    break;
                case 2: //Global Switch
                    var servSwitch = false;
                    if (ServerSwitchBase.GetSwitch(conditionCommand.Ints[1]) != null)
                        servSwitch = ServerSwitchBase.GetSwitch(conditionCommand.Ints[1]).Value;
                    if (servSwitch == Convert.ToBoolean(conditionCommand.Ints[2]))
                        return true;
                    break;
                case 3: //Global Variable
                    var servVar = 0;
                    if (ServerVariableBase.GetVariable(conditionCommand.Ints[1]) != null)
                        servVar = ServerVariableBase.GetVariable(conditionCommand.Ints[1]).Value;
                    switch (conditionCommand.Ints[2]) //Comparator
                    {
                        case 0: //Equal to
                            if (servVar == conditionCommand.Ints[3])
                                return true;
                            break;
                        case 1: //Greater than or equal to
                            if (servVar >= conditionCommand.Ints[3])
                                return true;
                            break;
                        case 2: //Less than or equal to
                            if (servVar <= conditionCommand.Ints[3])
                                return true;
                            break;
                        case 3: //Greater than
                            if (servVar > conditionCommand.Ints[3])
                                return true;
                            break;
                        case 4: //Less than
                            if (servVar < conditionCommand.Ints[3])
                                return true;
                            break;
                        case 5: //Does not equal
                            if (servVar != conditionCommand.Ints[3])
                                return true;
                            break;
                    }
                    break;
                case 4: //Has Item
                    if (MyPlayer.FindItem(conditionCommand.Ints[1], conditionCommand.Ints[2]) > -1)
                    {
                        return true;
                    }
                    break;
                case 5: //Class Is
                    if (MyPlayer.Class == conditionCommand.Ints[1])
                    {
                        return true;
                    }
                    break;
                case 6: //Knows spell
                    if (MyPlayer.KnowsSpell(conditionCommand.Ints[1]))
                    {
                        return true;
                    }
                    break;
                case 7: //Level or Stat is
                    var lvlStat = 0;
                    if (conditionCommand.Ints[3] == 0)
                    {
                        lvlStat = MyPlayer.Level;
                    }
                    else
                    {
                        lvlStat = MyPlayer.Stat[conditionCommand.Ints[3] - 1].Stat;
                    }
                    switch (conditionCommand.Ints[1])
                    {
                        case 0:
                            if (lvlStat == conditionCommand.Ints[2]) return true;
                            break;
                        case 1:
                            if (lvlStat >= conditionCommand.Ints[2]) return true;
                            break;
                        case 2:
                            if (lvlStat <= conditionCommand.Ints[2]) return true;
                            break;
                        case 3:
                            if (lvlStat > conditionCommand.Ints[2]) return true;
                            break;
                        case 4:
                            if (lvlStat < conditionCommand.Ints[2]) return true;
                            break;
                        case 5:
                            if (lvlStat != conditionCommand.Ints[2]) return true;
                            break;
                    }
                    break;
                case 8: //Self Switch
                    if (EventInstance != null)
                    {
                        if (EventInstance.IsGlobal)
                        {
                            var evts = MapInstance.GetMap(EventInstance.MapNum).GlobalEventInstances.Values.ToList();
                            for (int i = 0; i < evts.Count; i++)
                            {
                                if (evts[i] != null && evts[i].BaseEvent == EventInstance.BaseEvent)
                                {
                                    if (evts[i].SelfSwitch[conditionCommand.Ints[1]] ==
                                        Convert.ToBoolean(conditionCommand.Ints[2]))
                                        return true;
                                }
                            }
                        }
                        else
                        {
                            if (EventInstance.SelfSwitch[conditionCommand.Ints[1]] ==
                                Convert.ToBoolean(conditionCommand.Ints[2]))
                                return true;
                        }
                    }
                    return false;
                case 9: //Power Is
                    if (MyPlayer.MyClient.Power > conditionCommand.Ints[1]) return true;
                    return false;
                case 10: //Time is between
                    if (conditionCommand.Ints[1] > -1 && conditionCommand.Ints[2] > -1 &&
                        conditionCommand.Ints[1] < 1440/TimeBase.GetTimeBase().RangeInterval &&
                        conditionCommand.Ints[2] < 1440/TimeBase.GetTimeBase().RangeInterval)
                    {
                        return (ServerTime.GetTimeRange() >= conditionCommand.Ints[1] &&
                               ServerTime.GetTimeRange() <= conditionCommand.Ints[2]);
                    }
                    else
                    {
                        return true;
                    }
                case 11: //Can Start Quest
                    var startQuest = QuestBase.GetQuest(conditionCommand.Ints[1]);
                    if (startQuest != null)
                    {
                        return MyPlayer.CanStartQuest(startQuest);
                    }
                    break;
                case 12: //Quest In Progress
                    var questInProgress = QuestBase.GetQuest(conditionCommand.Ints[1]);
                    if (questInProgress != null)
                    {
                        return MyPlayer.QuestInProgress(questInProgress,(QuestProgress)conditionCommand.Ints[2],conditionCommand.Ints[3]);
                    }
                    break;
                case 13: //Quest Completed
                    var questCompleted = QuestBase.GetQuest(conditionCommand.Ints[1]);
                    if (questCompleted != null)
                    {
                        return MyPlayer.QuestCompleted(questCompleted);
                    }
                    break;
                case 14: //Player death
                    if (EventInstance != null)
                    {
                        return EventInstance.PlayerHasDied;
                    }
                    return false;
                case 15: //no NPCs on the map (used for boss fights)
                    if (EventInstance != null)
                    {
                        if (EventInstance.NPCDeathTriggerd == true) return false; //Only call it once
                        MapInstance m = MapInstance.GetMap(EventInstance.MapNum);
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
                    break;
                case 16: //Gender is
                    return MyPlayer.Gender == conditionCommand.Ints[1];
            }
            return false;
        }

        private string ParseEventText(string input)
        {
            if (MyClient != null && MyClient.Entity != null)
            {
                input = input.Replace(Strings.Get("events","playernamecommand"), MyClient.Entity.MyName);
                input = input.Replace(Strings.Get("events", "eventnamecommand"), PageInstance.MyName);
                if (input.Contains(Strings.Get("events", "onlinelistcommand")) || input.Contains(Strings.Get("events", "onlinecountcommand")))
                {
                    var onlineList = Globals.GetOnlineList();
                    input = input.Replace(Strings.Get("events", "onlinecountcommand"), onlineList.Count.ToString());
                    var sb = new StringBuilder();
                    for (int i = 0; i < onlineList.Count; i++)
                    {
                        sb.Append(onlineList[i].MyName + (i != onlineList.Count - 1 ? ", " : ""));
                    }
                    input = input.Replace(Strings.Get("events", "onlinelistcommand"), sb.ToString());
                }
            }
            return input;
        }

        private void ProcessCommand(EventCommand command)
        {
            bool success = false;
            TileHelper tile;
            int npcNum, animNum, spawnCondition, mapNum = -1, tileX = 0, tileY = 0, direction = (int)Directions.Up;
            Entity targetEntity = null;
            CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
            CallStack.Peek().ResponseIndex = 0;
            switch (command.Type)
            {
                case EventCommandType.ShowText:
                    PacketSender.SendEventDialog(MyClient, ParseEventText(command.Strs[0]), command.Strs[1], MapNum, BaseEvent.MyIndex);
                    CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Dialogue;
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ShowOptions:
                    PacketSender.SendEventDialog(MyClient, ParseEventText(command.Strs[0]), ParseEventText(command.Strs[1]), ParseEventText(command.Strs[2]),
                         ParseEventText(command.Strs[3]), ParseEventText(command.Strs[4]), command.Strs[5], MapNum, BaseEvent.MyIndex);
                    CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Dialogue;
                    CallStack.Peek().ResponseIndex = 1;
                    break;
                case EventCommandType.AddChatboxText:
                    switch (command.Ints[0])
                    {
                        case 0: //Player
                            PacketSender.SendPlayerMsg(MyClient, ParseEventText(command.Strs[0]), Color.FromName(command.Strs[1]));
                            break;
                        case 1: //Local
                            PacketSender.SendProximityMsg(ParseEventText(command.Strs[0]), MyClient.Entity.CurrentMap, Color.FromName(command.Strs[1]));
                            break;
                        case 2: //Global
                            PacketSender.SendGlobalMsg(ParseEventText(command.Strs[0]), Color.FromName(command.Strs[1]));
                            break;
                    }
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.SetSwitch:
                    if (command.Ints[0] == (int) SwitchVariableTypes.PlayerSwitch)
                    {
                        if (!MyPlayer.Switches.ContainsKey(command.Ints[1]))
                        {
                            MyPlayer.Switches.Add(command.Ints[1], false);
                        }
                        MyPlayer.Switches[command.Ints[1]] = Convert.ToBoolean(command.Ints[2]);
                    }
                    else if(command.Ints[0] == (int)SwitchVariableTypes.ServerSwitch)
                    {
                        var serverSwitch = ServerSwitchBase.GetSwitch(command.Ints[1]);
                        if (serverSwitch != null)
                        {
                            serverSwitch.Value = Convert.ToBoolean(command.Ints[2]);
                           Database.SaveGameObject(serverSwitch);
                        }
                    }
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.SetVariable:
                    if (command.Ints[0] == (int) SwitchVariableTypes.PlayerVariable)
                    {
                        if (!MyPlayer.Variables.ContainsKey(command.Ints[1]))
                        {
                           MyPlayer.Variables.Add(command.Ints[1],0); 
                        }
                        switch (command.Ints[2])
                        {
                            case 0: //Set
                                MyPlayer.Variables[command.Ints[1]] = command.Ints[3];
                                break;
                            case 1: //Add
                                MyPlayer.Variables[command.Ints[1]] += command.Ints[3];
                                break;
                            case 2: //Subtract
                                MyPlayer.Variables[command.Ints[1]] -= command.Ints[3];
                                break;
                            case 3: //Random
                                MyPlayer.Variables[command.Ints[1]] = Globals.Rand.Next(command.Ints[3], command.Ints[4] + 1);
                                break;
                        }
                    }
                    else if (command.Ints[0] == (int) SwitchVariableTypes.ServerVariable)
                    {
                        var serverVarible = ServerVariableBase.GetVariable(command.Ints[1]);
                        if (serverVarible != null)
                        {
                            switch (command.Ints[2])
                            {
                                case 0: //Set
                                    serverVarible.Value = command.Ints[3];
                                    break;
                                case 1: //Add
                                    serverVarible.Value += command.Ints[3];
                                    break;
                                case 2: //Subtract
                                    serverVarible.Value -= command.Ints[3];
                                    break;
                                case 3: //Random
                                    serverVarible.Value = Globals.Rand.Next(command.Ints[3],command.Ints[4] + 1);
                                    break;
                            }
                        }
                        Database.SaveGameObject(serverVarible);
                    }
                    
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.SetSelfSwitch:
                    if (IsGlobal)
                    {
                        var evts = MapInstance.GetMap(MapNum).GlobalEventInstances.Values.ToList();
                        for (int i = 0; i < evts.Count; i++)
                        {
                            if (evts[i] != null && evts[i].BaseEvent == BaseEvent)
                            {
                                evts[i].SelfSwitch[command.Ints[0]] = Convert.ToBoolean(command.Ints[1]);
                            }
                        }
                    }
                    else
                    {
                        SelfSwitch[command.Ints[0]] = Convert.ToBoolean(command.Ints[1]);
                    }
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ConditionalBranch:
                    if (MeetsCondition(command, MyPlayer, this))
                    {
                        var tmpStack = new CommandInstance(CallStack.Peek().Page)
                        {
                            CommandIndex = 0,
                            ListIndex =
                                CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[4]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    else
                    {
                        var tmpStack = new CommandInstance(CallStack.Peek().Page)
                        {
                            CommandIndex = 0,
                            ListIndex =
                                CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[5]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    break;
                case EventCommandType.ExitEventProcess:
                    CallStack.Clear();
                    return;
                case EventCommandType.Label:
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.GoToLabel:
                    //Recursively search through commands for the label, and create a brand new call stack based on where that label is located.
                    Stack<CommandInstance> newCallStack = LoadLabelCallstack(command.Strs[0], CallStack.Peek().Page);
                    if (newCallStack != null)
                    {
                        CallStack = newCallStack;
                    }
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.StartCommonEvent:
                    CallStack.Peek().CommandIndex++;
                    var commonEvent = EventBase.GetEvent(command.Ints[0]);
                    if (commonEvent != null)
                    {
                        for (int i = 0; i < commonEvent.MyPages.Count; i++)
                        {
                            if (CanSpawnPage(i, commonEvent))
                            {
                                var commonEventStack =
                                    new CommandInstance(commonEvent.MyPages[i])
                                    {
                                        CommandIndex = 0,
                                        ListIndex = 0,
                                    };

                                CallStack.Push(commonEventStack);
                            }
                        }
                    }

                    break;
                case EventCommandType.RestoreHp:
                    MyPlayer.RestoreVital(Vitals.Health);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.RestoreMp:
                    MyPlayer.RestoreVital(Vitals.Mana);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.LevelUp:
                    MyPlayer.LevelUp();
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.GiveExperience:
                    MyPlayer.GiveExperience(command.Ints[0]);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ChangeLevel:
                    MyPlayer.SetLevel(command.Ints[0], true);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ChangeSpells:
                    //0 is add, 1 is remove
                    success = false;
                    if (command.Ints[0] == 0)//Try to add a spell
                    {
                        success = MyPlayer.TryTeachSpell(new SpellInstance(command.Ints[1]));
                    }
                    else
                    {
                        if (MyPlayer.FindSpell(command.Ints[1]) > -1)
                        {
                            MyPlayer.ForgetSpell(MyPlayer.FindSpell(command.Ints[1]));
                            success = true;
                        }
                    }
                    if (success)
                    {
                        var tmpStack = new CommandInstance(CallStack.Peek().Page)
                        {
                            CommandIndex = 0,
                            ListIndex =
                                CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[4]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    else
                    {
                        var tmpStack = new CommandInstance(CallStack.Peek().Page)
                        {
                            CommandIndex = 0,
                            ListIndex =
                                CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[5]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    break;
                case EventCommandType.ChangeItems:
                    //0 is give, 1 is take
                    success = false;
                    if (command.Ints[0] == 0)//Try to give item
                    {
                        success = MyPlayer.TryGiveItem(new ItemInstance(command.Ints[1], command.Ints[2],-1));
                    }
                    else
                    {
                        int itemIndex = MyPlayer.FindItem(command.Ints[1], command.Ints[2]);
                        if (itemIndex > -1)
                        {
                            success = MyPlayer.TakeItem(itemIndex, command.Ints[2]);
                        }
                    }
                    if (success)
                    {
                        var tmpStack = new CommandInstance(CallStack.Peek().Page)
                        {
                            CommandIndex = 0,
                            ListIndex =
                                CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[4]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    else
                    {
                        var tmpStack = new CommandInstance(CallStack.Peek().Page)
                        {
                            CommandIndex = 0,
                            ListIndex =
                                CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[5]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    break;
                case EventCommandType.ChangeSprite:
                    MyPlayer.MySprite = command.Strs[0];
                    PacketSender.SendEntityDataToProximity(MyPlayer);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ChangeFace:
                    MyPlayer.Face = command.Strs[0];
                    PacketSender.SendEntityDataToProximity(MyPlayer);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ChangeGender:
                    MyPlayer.Gender = command.Ints[0];
                    PacketSender.SendEntityDataToProximity(MyPlayer);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.SetAccess:
                    MyPlayer.MyClient.Power = command.Ints[0];
                    PacketSender.SendEntityDataToProximity(MyPlayer);
                    PacketSender.SendPlayerMsg(MyPlayer.MyClient, Strings.Get("player","powerchanged"), Color.Red);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.WarpPlayer:
                    MyPlayer.Warp(CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[0],
                        CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[1],
                        CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[2],
                        CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[3]);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.SetMoveRoute:
                    if (CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[
                                CallStack.Peek().CommandIndex].Route.Target == -1)
                    {
                        MyClient.Entity.MoveRoute = new EventMoveRoute();
                        MyClient.Entity.MoveRouteSetter = PageInstance;
                        MyClient.Entity.MoveRoute.CopyFrom(CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                            .Commands[CallStack.Peek().CommandIndex].Route);
                        PacketSender.SendMoveRouteToggle(MyClient, true);
                    }
                    else
                    {
                        for (var i = 0; i < MyPlayer.MyEvents.Count; i++)
                        {
                            if (MyPlayer.MyEvents[i] == null) continue;
                            if (MyPlayer.MyEvents[i].BaseEvent.MyIndex ==
                                CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[
                                    CallStack.Peek().CommandIndex].Route.Target)
                            {
                                if (MyPlayer.MyEvents[i].PageInstance != null)
                                {
                                    MyPlayer.MyEvents[i].PageInstance.MoveRoute.CopyFrom(
                                        CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                            .Commands[
                                                CallStack.Peek().CommandIndex].Route);
                                    MyPlayer.MyEvents[i].PageInstance.MovementType = 2;
                                }
                            }
                        }
                    }
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.WaitForRouteCompletion:
                    if (CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[
                                CallStack.Peek().CommandIndex].Ints[0] == -1)
                    {
                        CallStack.Peek().WaitingForRoute = -1;
                        CallStack.Peek().WaitingForRouteMap = -1;
                    }
                    else
                    {
                        for (var i = 0; i < MyPlayer.MyEvents.Count; i++)
                        {
                            if (MyPlayer.MyEvents[i] == null) continue;
                            if (MyPlayer.MyEvents[i].BaseEvent.MyIndex ==
                                CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[
                                    CallStack.Peek().CommandIndex].Ints[0])
                            {
                                CallStack.Peek().WaitingForRoute = MyPlayer.MyEvents[i].BaseEvent.MyIndex;
                                CallStack.Peek().WaitingForRouteMap = MyPlayer.MyEvents[i].MapNum;
                                break;
                            }
                        }
                    }
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.SpawnNpc:
                    npcNum = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[0];
                    spawnCondition = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[1];
                    mapNum = -1;
                    tileX = 0;
                    tileY = 0;
                    direction = (int)Directions.Up;
                    targetEntity = null;
                    switch (spawnCondition)
                    {
                        case 0: //Tile Spawn
                            mapNum = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[2];
                            tileX = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[3];
                            tileY = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[4];
                            direction = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[5];
                            break;
                        case 1: //Entity Spawn
                            if (CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                .Commands[
                                    CallStack.Peek().CommandIndex].Ints[2] == -1)
                            {
                                targetEntity = MyPlayer;
                            }
                            else
                            {
                                for (var i = 0; i < MyPlayer.MyEvents.Count; i++)
                                {
                                    if (MyPlayer.MyEvents[i] == null) continue;
                                    if (MyPlayer.MyEvents[i].BaseEvent.MyIndex ==
                                        CallStack.Peek().Page.CommandLists[
                                            CallStack.Peek().ListIndex].Commands[
                                                CallStack.Peek().CommandIndex].Ints[2])
                                    {
                                        targetEntity = MyPlayer.MyEvents[i].PageInstance;
                                        break;
                                    }
                                }
                            }
                            if (targetEntity != null)
                            {
                                int xDiff = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[3];
                                int yDiff = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[4];
                                if (
                                    CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                        .Commands[CallStack.Peek().CommandIndex].Ints[5] == 1)
                                {
                                    int tmp = 0;
                                    switch (targetEntity.Dir)
                                    {
                                        case (int)Directions.Down:
                                            yDiff *= -1;
                                            xDiff *= -1;
                                            break;
                                        case (int)Directions.Left:
                                            tmp = yDiff;
                                            yDiff = xDiff;
                                            xDiff = tmp;
                                            break;
                                        case (int)Directions.Right:
                                            tmp = yDiff;
                                            yDiff = xDiff;
                                            xDiff = -tmp;
                                            break;
                                    }
                                    direction = targetEntity.Dir;
                                }
                                mapNum = targetEntity.CurrentMap;
                                tileX = targetEntity.CurrentX + xDiff;
                                tileY = targetEntity.CurrentY + yDiff;
                            }
                            break;
                    }
                    tile = new TileHelper(mapNum, tileX, tileY);
                    if (tile.TryFix())
                    {
                        var npc = MapInstance.GetMap(mapNum).SpawnNpc(tileX, tileY, direction, npcNum, true);
                        MyPlayer.SpawnedNpcs.Add((Npc)npc);
                    }
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.DespawnNpc:
                    var entities = MyPlayer.SpawnedNpcs.ToArray();
                    for (int i = 0; i < entities.Length; i++)
                    {
                        if (entities[i] != null  && entities[i].GetType() == typeof(Npc))
                        {
                            if (((Npc)entities[i]).Despawnable == true)
                            {
                                ((Npc)entities[i]).Die(false);
                            }
                        }
                    }
                    MyPlayer.SpawnedNpcs.Clear();
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.PlayAnimation:
                    //Playing an animations requires a target type/target or just a tile.
                    //We need an animation number and whether or not it should rotate (and the direction I guess)
                    animNum = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[0];
                    spawnCondition = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[1];
                    mapNum = -1;
                    tileX = 0;
                    tileY = 0;
                    direction = (int)Directions.Up;
                    targetEntity = null;
                    switch (spawnCondition)
                    {
                        case 0: //Tile Spawn
                            mapNum = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[2];
                            tileX = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[3];
                            tileY = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[4];
                            direction = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[5];
                            break;
                        case 1: //Entity Spawn
                            if (CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                .Commands[
                                    CallStack.Peek().CommandIndex].Ints[2] == -1)
                            {
                                targetEntity = MyPlayer;
                            }
                            else
                            {
                                for (var i = 0; i < MyPlayer.MyEvents.Count; i++)
                                {
                                    if (MyPlayer.MyEvents[i] == null) continue;
                                    if (MyPlayer.MyEvents[i].BaseEvent.MyIndex ==
                                        CallStack.Peek().Page.CommandLists[
                                            CallStack.Peek().ListIndex].Commands[
                                                CallStack.Peek().CommandIndex].Ints[2])
                                    {
                                        targetEntity = MyPlayer.MyEvents[i].PageInstance;
                                        break;
                                    }
                                }
                            }
                            if (targetEntity != null)
                            {
                                int xDiff = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[3];
                                int yDiff = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[4];
                                if (CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                        .Commands[CallStack.Peek().CommandIndex].Ints[5] == 2 || CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                        .Commands[CallStack.Peek().CommandIndex].Ints[5] == 3)
                                    direction = targetEntity.Dir;
                                if (xDiff == 0 && yDiff == 0)
                                {
                                    if (CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                        .Commands[CallStack.Peek().CommandIndex].Ints[5] == 2 || CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                        .Commands[CallStack.Peek().CommandIndex].Ints[5] == 3)
                                        direction = -1;
                                    //Send Animation on Npc
                                    if (targetEntity.GetType() == typeof(Player))
                                    {
                                        PacketSender.SendAnimationToProximity(animNum, 1, targetEntity.MyIndex, MyClient.Entity.CurrentMap, 0, 0, direction); //Target Type 1 will be global entity
                                    }
                                    else
                                    {
                                        PacketSender.SendAnimationToProximity(animNum, 2, targetEntity.MyIndex, targetEntity.CurrentMap, targetEntity.MyIndex, 0, direction);
                                    }
                                    CallStack.Peek().CommandIndex++;
                                    return;
                                }
                                else
                                {
                                    //Determine the tile data
                                    if (CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                        .Commands[CallStack.Peek().CommandIndex].Ints[5] == 1 || CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                        .Commands[CallStack.Peek().CommandIndex].Ints[5] == 3)
                                    {
                                        int tmp = 0;
                                        switch (targetEntity.Dir)
                                        {
                                            case (int)Directions.Down:
                                                yDiff *= -1;
                                                xDiff *= -1;
                                                break;
                                            case (int)Directions.Left:
                                                tmp = yDiff;
                                                yDiff = xDiff;
                                                xDiff = tmp;
                                                break;
                                            case (int)Directions.Right:
                                                tmp = yDiff;
                                                yDiff = xDiff;
                                                xDiff = -tmp;
                                                break;
                                        }
                                    }
                                    mapNum = targetEntity.CurrentMap;
                                    tileX = targetEntity.CurrentX + xDiff;
                                    tileY = targetEntity.CurrentY + yDiff;
                                }
                            }
                            break;
                    }
                    tile = new TileHelper(mapNum, tileX, tileY);
                    if (tile.TryFix())
                    {
                        PacketSender.SendAnimationToProximity(animNum, -1, -1, tile.GetMap(), tile.GetX(), tile.GetY(), direction);
                    }
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.HoldPlayer:
                    HoldingPlayer = true;
                    PacketSender.SendHoldPlayer(MyClient,MapNum,MyIndex);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ReleasePlayer:
                    HoldingPlayer = false;
                    PacketSender.SendReleasePlayer(MyClient, MapNum, MyIndex);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.PlayBgm:
                    PacketSender.SendPlayMusic(MyClient, CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Strs[0]);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.FadeoutBgm:
                    PacketSender.SendFadeMusic(MyClient);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.PlaySound:
                    PacketSender.SendPlaySound(MyClient, CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Strs[0]);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.StopSounds:
                    PacketSender.SendStopSounds(MyClient);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.Wait:
                    WaitTimer = Globals.System.GetTimeMs() +
                                CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[0];
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.OpenBank:
                    MyPlayer.OpenBank();
                    CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Bank;
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.OpenShop:
                    MyPlayer.OpenShop(CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                        .Commands[CallStack.Peek().CommandIndex].Ints[0]);
                    CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Shop;
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.OpenCraftingBench:
                    MyPlayer.OpenCraftingBench(CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                        .Commands[CallStack.Peek().CommandIndex].Ints[0]);
                    CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Crafting;
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.SetClass:
                    if (ClassBase.GetClass(CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                        .Commands[CallStack.Peek().CommandIndex].Ints[0]) != null)
                    {
                        MyPlayer.Class = CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                            .Commands[CallStack.Peek().CommandIndex].Ints[0];
                    }
                    PacketSender.SendEntityDataToProximity(MyPlayer);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.StartQuest:
                    success = false;
                    var quest = QuestBase.GetQuest(CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[0]);
                    if (quest != null)
                    {
                        if (MyPlayer.CanStartQuest(quest))
                        {
                            var offer = Convert.ToBoolean(CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[1]);
                            if (offer)
                            {
                                MyPlayer.OfferQuest(quest);
                                CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Quest;
                                CallStack.Peek().ResponseIndex = quest.Id;
                                break;
                            }
                            else
                            {
                                MyPlayer.StartQuest(quest);
                                success = true;
                            }
                        }
                    }
                    if (success)
                    {
                        var tmpStack = new CommandInstance(CallStack.Peek().Page)
                        {
                            CommandIndex = 0,
                            ListIndex =
                                CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[4]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    else
                    {
                        var tmpStack = new CommandInstance(CallStack.Peek().Page)
                        {
                            CommandIndex = 0,
                            ListIndex =
                                CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[5]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    break;
                case EventCommandType.CompleteQuestTask:
                    MyPlayer.CompleteQuestTask(
                        CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                            .Commands[CallStack.Peek().CommandIndex].Ints[0],
                            CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                .Commands[CallStack.Peek().CommandIndex].Ints[1]);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.EndQuest:
                    CallStack.Peek().CommandIndex++;
                    break;
            }
        }

        private Stack<CommandInstance> LoadLabelCallstack(string label, EventPage currentPage)
        {
            Stack<CommandInstance> newStack = new Stack<CommandInstance>();
            newStack.Push(new CommandInstance(currentPage) { CommandIndex = 0, ListIndex = 0 }); //Start from the top
            if (FindLabelResursive(newStack, label))
            {
                return newStack;
            }
            return null;
        }

        private bool FindLabelResursive(Stack<CommandInstance> stack, string label)
        {
            if (stack.Peek().ListIndex < CallStack.Peek().Page.CommandLists.Count)
            {
                while (stack.Peek().CommandIndex < CallStack.Peek().Page.CommandLists[stack.Peek().ListIndex].Commands.Count)
                {
                    EventCommand command =
                        CallStack.Peek().Page.CommandLists[stack.Peek().ListIndex].Commands[stack.Peek().CommandIndex];
                    switch (command.Type)
                    {
                        case EventCommandType.ShowOptions:
                            for (int i = 0; i < 4; i++)
                            {
                                var tmpStack = new CommandInstance(CallStack.Peek().Page);
                                tmpStack.CommandIndex = 0;
                                tmpStack.ListIndex =
                                    CallStack.Peek().Page.CommandLists[stack.Peek().ListIndex].Commands[stack.Peek().CommandIndex].Ints[i];
                                stack.Peek().CommandIndex++;
                                stack.Push(tmpStack);
                                if (FindLabelResursive(stack, label)) return true;
                                stack.Peek().CommandIndex--;
                            }
                            break;
                        case EventCommandType.ConditionalBranch:
                        case EventCommandType.ChangeSpells:
                        case EventCommandType.ChangeItems:
                            for (int i = 4; i <= 5; i++)
                            {
                                var tmpStack = new CommandInstance(CallStack.Peek().Page);
                                tmpStack.CommandIndex = 0;
                                tmpStack.ListIndex =
                                    CallStack.Peek().Page.CommandLists[stack.Peek().ListIndex].Commands[stack.Peek().CommandIndex].Ints[i];
                                stack.Peek().CommandIndex++;
                                stack.Push(tmpStack);
                                if (FindLabelResursive(stack, label)) return true;
                                stack.Peek().CommandIndex--;
                            }
                            break;
                        case EventCommandType.Label:
                            //See if we found the label!
                            if (
                                CallStack.Peek().Page.CommandLists[stack.Peek().ListIndex].Commands[
                                    stack.Peek().CommandIndex].Strs[0] == label)
                            {
                                return true;
                            }
                            break;

                    }
                    stack.Peek().CommandIndex++;
                }
                stack.Pop(); //We made it through a list
            }
            return false;
        }

    }

    public class CommandInstance
    {
        public EventPage Page;
        public int ListIndex;
        public int CommandIndex;
        public EventResponse WaitingForResponse = EventResponse.None;
        public int ResponseIndex = -1;
        public int WaitingForRoute = -2;
        public int WaitingForRouteMap;

        public enum EventResponse
        {
            None = 0,
            Dialogue,
            Shop,
            Bank,
            Crafting,
            Quest,
        }

        public CommandInstance(EventPage page)
        {
            Page = page;
        }
    }
}
