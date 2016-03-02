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
using System.Drawing;

namespace Intersect_Server.Classes.Entities
{
    public class EventInstance
    {
        public EventStruct BaseEvent;
        public bool IsGlobal;
        public int MapNum;
        public int MyIndex;
        public Client MyClient;
        public Player MyPlayer;
        public int SpawnX;
        public int SpawnY;
        public int PageIndex;
        public bool[] SelfSwitch { get; set; }
        public EventPageInstance PageInstance;
        public EventPageInstance[] GlobalPageInstance;

        public Stack<CommandInstance> CallStack = new Stack<CommandInstance>();

        public EventInstance(int index, Client client, EventStruct baseEvent, int mapNum)
        {
            MyIndex = index;
            MyClient = client;
            MapNum = mapNum;
            MyPlayer = (Player)Globals.Entities[MyClient.EntityIndex];
            SelfSwitch = new bool[4];
            BaseEvent = baseEvent;
        }

        public EventInstance(EventStruct baseEvent, int index, int mapNum) //Global constructor
        {
            IsGlobal = true;
            MapNum = mapNum;
            BaseEvent = baseEvent;
            MyIndex = index;
            SelfSwitch = new bool[4];
            GlobalPageInstance = new EventPageInstance[BaseEvent.MyPages.Count];
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
                    PageInstance = null;
                    if (IsGlobal)
                    {
                        PacketSender.SendEntityLeaveTo(MyClient, BaseEvent.MyIndex, (int)Enums.EntityTypes.Event, MapNum);
                    }
                    else
                    {
                        PacketSender.SendEntityLeaveTo(MyClient, BaseEvent.MyIndex, (int)Enums.EntityTypes.Event, MapNum);
                    }
                }
                else
                {
                    if (!IsGlobal) PageInstance.Update(MyClient); //Process movement and stuff that is client specific
                    if (CallStack.Count > 0)
                    {
                        while (CallStack.Peek().WaitingForResponse == 0)
                        {
                            if (CallStack.Peek().CommandIndex >=
                                PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex]
                                    .Commands.Count)
                            {
                                CallStack.Pop();
                            }
                            else
                            {
                                ProcessCommand(
                                    PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex]
                                        .Commands[CallStack.Peek().CommandIndex]);
                            }
                            if (CallStack.Count == 0)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (PageInstance.Trigger == 2)
                        {
                            var newStack = new CommandInstance { CommandIndex = 0, ListIndex = 0 };
                            CallStack.Push(newStack);
                        }
                    }
                }
            }
            else
            {
                //Try to Spawn a PageInstance.. if we can
                for (int i = 0; i < BaseEvent.MyPages.Count; i++)
                {
                    if (CanSpawnPage(i, BaseEvent))
                    {
                        if (IsGlobal)
                        {
                            PageInstance = new EventPageInstance(BaseEvent, BaseEvent.MyPages[i], BaseEvent.MyIndex, MapNum, this, MyClient, BaseEvent.GlobalInstance.GlobalPageInstance[i]);
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

        private bool CanSpawnPage(int pageIndex, EventStruct eventStruct)
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
                    if (MyPlayer.Switches[conditionCommand.Ints[1]] == Convert.ToBoolean(conditionCommand.Ints[2]))
                    {
                        return true;
                    }
                    break;
                case 1: //Player Variable
                    switch (conditionCommand.Ints[2])//Comparator
                    {
                        case 0: //Equal to
                            if (MyPlayer.Variables[conditionCommand.Ints[1]] == conditionCommand.Ints[3]) return true;
                            break;
                        case 1: //Greater than or equal to
                            if (MyPlayer.Variables[conditionCommand.Ints[1]] >= conditionCommand.Ints[3]) return true;
                            break;
                        case 2: //Less than or equal to
                            if (MyPlayer.Variables[conditionCommand.Ints[1]] <= conditionCommand.Ints[3]) return true;
                            break;
                        case 3: //Greater than
                            if (MyPlayer.Variables[conditionCommand.Ints[1]] > conditionCommand.Ints[3]) return true;
                            break;
                        case 4: //Less than
                            if (MyPlayer.Variables[conditionCommand.Ints[1]] < conditionCommand.Ints[3]) return true;
                            break;
                        case 5: //Does not equal
                            if (MyPlayer.Variables[conditionCommand.Ints[1]] != conditionCommand.Ints[3]) return true;
                            break;
                    }
                    break;
                case 6: //Self Switch
                    if (IsGlobal)
                    {
                        for (int i = 0; i < Globals.GameMaps[MapNum].GlobalEvents.Count; i++)
                        {
                            if (Globals.GameMaps[MapNum].GlobalEvents[i] != null &&
                                Globals.GameMaps[MapNum].GlobalEvents[i].BaseEvent == BaseEvent)
                            {
                                if (Globals.GameMaps[MapNum].GlobalEvents[i].SelfSwitch[conditionCommand.Ints[1]] == Convert.ToBoolean(conditionCommand.Ints[2]))
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
            }
            return false;
        }


        private void ProcessCommand(EventCommand command)
        {
            bool success = false;
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
                    MyPlayer.Switches[command.Ints[0]] = Convert.ToBoolean(command.Ints[1]);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.SetVariable:
                    switch (command.Ints[1])
                    {
                        case 0: //Set
                            MyPlayer.Variables[command.Ints[0]] = command.Ints[2];
                            break;
                        case 1: //Add
                            MyPlayer.Variables[command.Ints[0]] += command.Ints[2];
                            break;
                        case 2: //Subtract
                            MyPlayer.Variables[command.Ints[0]] -= command.Ints[2];
                            break;
                        case 3: //Random
                            MyPlayer.Variables[command.Ints[0]] = Globals.Rand.Next(command.Ints[2], command.Ints[3] + 1);
                            break;
                    }
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.SetSelfSwitch:
                    if (IsGlobal)
                    {
                        for (int i = 0; i < Globals.GameMaps[MapNum].GlobalEvents.Count; i++)
                        {
                            if (Globals.GameMaps[MapNum].GlobalEvents[i] != null &&
                                Globals.GameMaps[MapNum].GlobalEvents[i].BaseEvent == BaseEvent)
                            {
                                Globals.GameMaps[MapNum].GlobalEvents[i].SelfSwitch[command.Ints[0]] = Convert.ToBoolean(command.Ints[1]);
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
                        var tmpStack = new CommandInstance
                        {
                            CommandIndex = 0,
                            ListIndex =
                                PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[4]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    else
                    {
                        var tmpStack = new CommandInstance
                        {
                            CommandIndex = 0,
                            ListIndex =
                                PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex]
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
                    Stack<CommandInstance> newCallStack = LoadLabelCallstack(command.Strs[0]);
                    if (newCallStack != null)
                    {
                        CallStack = newCallStack;
                    }
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.RestoreHp:
                    MyPlayer.RestoreVital(Enums.Vitals.Health);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.RestoreMp:
                    MyPlayer.RestoreVital(Enums.Vitals.Mana);
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
                    MyPlayer.SetLevel(command.Ints[0],true);
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
                        var tmpStack = new CommandInstance
                        {
                            CommandIndex = 0,
                            ListIndex =
                                PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[4]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    else
                    {
                        var tmpStack = new CommandInstance
                        {
                            CommandIndex = 0,
                            ListIndex =
                                PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex]
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
                        if ( itemIndex > -1)
                        {
                            success = MyPlayer.TakeItem(itemIndex,command.Ints[2]);
                        }
                    }
                    if (success)
                    {
                        var tmpStack = new CommandInstance
                        {
                            CommandIndex = 0,
                            ListIndex =
                                PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[4]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    else
                    {
                        var tmpStack = new CommandInstance
                        {
                            CommandIndex = 0,
                            ListIndex =
                                PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[5]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    break;
                case EventCommandType.ChangeSprite:
                    MyPlayer.MySprite = command.Strs[0];
                    PacketSender.SendEntityDataToProximity(MyPlayer.MyIndex, (int) Enums.EntityTypes.Player,
                        MyPlayer.Data(), MyPlayer);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ChangeFace:
                    MyPlayer.Face = command.Strs[0];
                    PacketSender.SendEntityDataToProximity(MyPlayer.MyIndex, (int)Enums.EntityTypes.Player,
                        MyPlayer.Data(), MyPlayer);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ChangeGender:
                    MyPlayer.Gender = command.Ints[0];
                    PacketSender.SendEntityDataToProximity(MyPlayer.MyIndex, (int)Enums.EntityTypes.Player,
                        MyPlayer.Data(), MyPlayer);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.SetAccess:
                    MyPlayer.MyClient.Power = command.Ints[0];
                    PacketSender.SendEntityDataToProximity(MyPlayer.MyIndex, (int)Enums.EntityTypes.Player,
                        MyPlayer.Data(), MyPlayer);
                    PacketSender.SendPlayerMsg(MyPlayer.MyClient, "Your access has been updated!", Color.Red);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.WarpPlayer:
                    MyPlayer.Warp(PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[0],
                        PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[1],
                        PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[2],
                        PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[3]);
                    CallStack.Peek().CommandIndex++;
                    break;

            }
        }

        private Stack<CommandInstance> LoadLabelCallstack(string label)
        {
            Stack<CommandInstance> newStack = new Stack<CommandInstance>();
            newStack.Push(new CommandInstance {CommandIndex = 0, ListIndex = 0}); //Start from the top
            if (FindLabelResursive(newStack, label))
            {
                return newStack;
            }
            return null;
        }

        private bool FindLabelResursive(Stack<CommandInstance> stack, string label)
        {
            if (stack.Peek().ListIndex < PageInstance.BaseEvent.MyPages[PageIndex].CommandLists.Count)
            {
                while (stack.Peek().CommandIndex < PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[stack.Peek().ListIndex].Commands.Count)
                {
                    EventCommand command =
                        PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[stack.Peek().ListIndex].Commands[stack.Peek().CommandIndex];
                    switch (command.Type)
                    {
                        case EventCommandType.ShowOptions:
                            for (int i = 0; i < 4; i++)
                            {
                                var tmpStack = new CommandInstance();
                                tmpStack.CommandIndex = 0;
                                tmpStack.ListIndex =
                                    PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[stack.Peek().ListIndex].Commands[stack.Peek().CommandIndex].Ints[i];
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
                                var tmpStack = new CommandInstance();
                                tmpStack.CommandIndex = 0;
                                tmpStack.ListIndex =
                                    PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[stack.Peek().ListIndex].Commands[stack.Peek().CommandIndex].Ints[i];
                                stack.Peek().CommandIndex++;
                                stack.Push(tmpStack);
                                if (FindLabelResursive(stack, label)) return true;
                                stack.Peek().CommandIndex--;
                            }
                            break;
                        case EventCommandType.Label:
                            //See if we found the label!
                            if (
                                PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[stack.Peek().ListIndex].Commands[
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
        public int ListIndex;
        public int CommandIndex;
        public int WaitingForResponse;
        public int ResponseType;
    }
}
