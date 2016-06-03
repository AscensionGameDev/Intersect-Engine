/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Collections.Generic;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
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
            if (PageInstance != null)
            {
                //Check for despawn
                if (PageInstance.ShouldDespawn())
                {
                    CurrentX = PageInstance.CurrentX;
                    CurrentY = PageInstance.CurrentY;
                    PageInstance = null;
                    if (HoldingPlayer)
                    {
                        PacketSender.SendReleasePlayer(MyClient, MapNum, MyIndex);
                        HoldingPlayer = false;
                    }
                    if (IsGlobal)
                    {
                        PacketSender.SendEntityLeaveTo(MyClient, BaseEvent.MyIndex, (int)EntityTypes.Event, MapNum);
                    }
                    else
                    {
                        PacketSender.SendEntityLeaveTo(MyClient, BaseEvent.MyIndex, (int)EntityTypes.Event, MapNum);
                    }
                }
                else
                {
                    if (!IsGlobal) PageInstance.Update(); //Process movement and stuff that is client specific
                    if (CallStack.Count > 0)
                    {
                        if (CallStack.Peek().WaitingForResponse == 2 && MyPlayer.InShop == -1)
                            CallStack.Peek().WaitingForResponse = 0;
                        if (CallStack.Peek().WaitingForResponse == 3 && MyPlayer.InBank == false)
                            CallStack.Peek().WaitingForResponse = 0;
                        while (CallStack.Peek().WaitingForResponse == 0)
                        {
                            if (CallStack.Peek().WaitingForRoute > -1)
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
                                        CallStack.Peek().WaitingForRoute = -1;
                                        CallStack.Peek().WaitingForRouteMap = -1;
                                        break;
                                    }
                                }
                                if (CallStack.Peek().WaitingForRoute > -1) break;
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
                                    if (WaitTimer < Environment.TickCount)
                                    {
                                        ProcessCommand(
                                            CallStack.Peek().Page.CommandLists[
                                                CallStack.Peek().ListIndex]
                                                .Commands[CallStack.Peek().CommandIndex]);
                                    }
                                }
                                if (CallStack.Count == 0)
                                {
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
            else
            {
                //Try to Spawn a PageInstance.. if we can
                for (int i = BaseEvent.MyPages.Count-1; i >= 0; i--)
                {
                    if (CanSpawnPage(i, BaseEvent))
                    {
                        if (IsGlobal)
                        {
                            PageInstance = new EventPageInstance(BaseEvent, BaseEvent.MyPages[i], BaseEvent.MyIndex, MapNum, this, MyClient, MapInstance.GetMap(MapNum).GetGlobalEventInstance(BaseEvent).GlobalPageInstance[i]);
                            PageIndex = i;
                        }
                        else
                        {
                            PageInstance = new EventPageInstance(BaseEvent, BaseEvent.MyPages[i], BaseEvent.MyIndex, MapNum, this, MyClient);
                            PageIndex = i;
                        }
                        break;
                    }
                }
            }


        }

        public bool CanSpawnPage(int pageIndex, EventBase eventStruct)
        {
            for (int i = 0; i < eventStruct.MyPages[pageIndex].Conditions.Count; i++)
            {
                if (!MeetsConditions(eventStruct.MyPages[pageIndex].Conditions[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public bool MeetsConditions(EventCommand conditionCommand)
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
                case 2: //Player Switch
                    var servSwitch = false;
                    if (ServerSwitchBase.GetSwitch(conditionCommand.Ints[1]) != null)
                        servSwitch = ServerSwitchBase.GetSwitch(conditionCommand.Ints[1]).Value;
                    if (servSwitch == Convert.ToBoolean(conditionCommand.Ints[2]))
                        return true;
                    break;
                case 3: //Player Variable
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
                case 7: //Level is
                    switch (conditionCommand.Ints[1])
                    {
                        case 0:
                            if (MyPlayer.Level == conditionCommand.Ints[2]) return true;
                            break;
                        case 1:
                            if (MyPlayer.Level >= conditionCommand.Ints[2]) return true;
                            break;
                        case 2:
                            if (MyPlayer.Level <= conditionCommand.Ints[2]) return true;
                            break;
                        case 3:
                            if (MyPlayer.Level > conditionCommand.Ints[2]) return true;
                            break;
                        case 4:
                            if (MyPlayer.Level < conditionCommand.Ints[2]) return true;
                            break;
                        case 5:
                            if (MyPlayer.Level != conditionCommand.Ints[2]) return true;
                            break;
                    }
                    break;
                case 8: //Self Switch
                    if (IsGlobal)
                    {
                        for (int i = 0; i < MapInstance.GetMap(MapNum).GlobalEvents.Count; i++)
                        {
                            if (MapInstance.GetMap(MapNum).GlobalEvents[i] != null &&
                                MapInstance.GetMap(MapNum).GlobalEvents[i].BaseEvent == BaseEvent)
                            {
                                if (MapInstance.GetMap(MapNum).GlobalEvents[i].SelfSwitch[conditionCommand.Ints[1]] == Convert.ToBoolean(conditionCommand.Ints[2]))
                                    return true;
                            }
                        }
                    }
                    else
                    {
                        if (SelfSwitch[conditionCommand.Ints[1]] == Convert.ToBoolean(conditionCommand.Ints[2]))
                            return true;
                    }
                    return false;
                case 9: //Power Is
                    if (MyClient.Power > conditionCommand.Ints[1]) return true;
                    return false;
            }
            return false;
        }

        private void ProcessCommand(EventCommand command)
        {
            bool success = false;
            TileHelper tile;
            int npcNum, animNum, spawnCondition, mapNum = -1, tileX = 0, tileY = 0, direction = (int)Directions.Up;
            Entity targetEntity = null;
            CallStack.Peek().WaitingForResponse = 0;
            CallStack.Peek().ResponseType = 0;
            switch (command.Type)
            {
                case EventCommandType.ShowText:
                    PacketSender.SendEventDialog(MyClient, command.Strs[0], command.Strs[1], MapNum, BaseEvent.MyIndex);
                    CallStack.Peek().WaitingForResponse = 1;
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ShowOptions:
                    PacketSender.SendEventDialog(MyClient, command.Strs[0], command.Strs[1], command.Strs[2],
                        command.Strs[3], command.Strs[4], command.Strs[5], MapNum, BaseEvent.MyIndex);
                    CallStack.Peek().WaitingForResponse = 1;
                    CallStack.Peek().ResponseType = 1;
                    break;
                case EventCommandType.AddChatboxText:
                    switch (command.Ints[0])
                    {
                        case 0: //Player
                            PacketSender.SendPlayerMsg(MyClient, command.Strs[0], Color.FromName(command.Strs[1]));
                            break;
                        case 1: //Local
                            PacketSender.SendProximityMsg(command.Strs[0], MyClient.Entity.CurrentMap, Color.FromName(command.Strs[1]));
                            break;
                        case 2: //Global
                            PacketSender.SendGlobalMsg(command.Strs[0], Color.FromName(command.Strs[1]));
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
                        for (int i = 0; i < MapInstance.GetMap(MapNum).GlobalEvents.Count; i++)
                        {
                            if (MapInstance.GetMap(MapNum).GlobalEvents[i] != null &&
                                MapInstance.GetMap(MapNum).GlobalEvents[i].BaseEvent == BaseEvent)
                            {
                                MapInstance.GetMap(MapNum).GlobalEvents[i].SelfSwitch[command.Ints[0]] = Convert.ToBoolean(command.Ints[1]);
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
                    if (MeetsConditions(command))
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
                        success = MyPlayer.TryGiveItem(new ItemInstance(command.Ints[1], command.Ints[2]));
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
                    PacketSender.SendEntityDataToProximity(MyPlayer.MyIndex, (int)EntityTypes.Player,
                        MyPlayer.Data(), MyPlayer);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ChangeFace:
                    MyPlayer.Face = command.Strs[0];
                    PacketSender.SendEntityDataToProximity(MyPlayer.MyIndex, (int)EntityTypes.Player,
                        MyPlayer.Data(), MyPlayer);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ChangeGender:
                    MyPlayer.Gender = command.Ints[0];
                    PacketSender.SendEntityDataToProximity(MyPlayer.MyIndex, (int)EntityTypes.Player,
                        MyPlayer.Data(), MyPlayer);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.SetAccess:
                    MyPlayer.MyClient.Power = command.Ints[0];
                    PacketSender.SendEntityDataToProximity(MyPlayer.MyIndex, (int)EntityTypes.Player,
                        MyPlayer.Data(), MyPlayer);
                    PacketSender.SendPlayerMsg(MyPlayer.MyClient, "Your access has been updated!", Color.Red);
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
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.WaitForRouteCompletion:
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
                        MapInstance.GetMap(mapNum).SpawnNpc(tileX, tileY, direction, npcNum);
                    }
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
                    WaitTimer = Environment.TickCount +
                                CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[0];
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.OpenBank:
                    MyPlayer.OpenBank();
                    CallStack.Peek().WaitingForResponse = 3;
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.OpenShop:
                    MyPlayer.OpenShop(CallStack.Peek().Page.CommandLists[CallStack.Peek().ListIndex]
                        .Commands[CallStack.Peek().CommandIndex].Ints[0]);
                    CallStack.Peek().WaitingForResponse = 2;
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
        public int WaitingForResponse;
        public int WaitingForRoute = -1;
        public int WaitingForRouteMap;
        public int ResponseType;

        public enum EventResponseType
        {
            Dialog = 1,
        }

        public CommandInstance(EventPage page)
        {
            Page = page;
        }
    }
}
