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
using System.Diagnostics;

namespace Intersect_Server.Classes
{
    public class EventPageInstance : Entity
    {
        public Client Client = null;
        public bool Local = true;
        public int Trigger;
        public int MovementType;
        public int MovementFreq;
        public int MovementSpeed;
        public EventStruct BaseEvent;
        public EventPage MyPage;
        public EventIndex EventIndex;
        public EventPageInstance(EventStruct myEvent, EventPage myPage, int myIndex, int mapNum, EventIndex eventIndex, Client client) : base(myIndex)
        {
            BaseEvent = myEvent;
            MyPage = myPage;
            CurrentMap = mapNum;
            CurrentX = myEvent.SpawnX;
            CurrentY = myEvent.SpawnY;
            MyName = myEvent.MyName;
            MovementType = MyPage.MovementType;
            MovementFreq = MyPage.MovementFreq;
            MovementSpeed = MyPage.MovementSpeed;
            Trigger = MyPage.Trigger;
            Passable = MyPage.Passable;
            HideName = MyPage.HideName;
            CurrentX = myEvent.SpawnX;
            CurrentY = myEvent.SpawnY;
            CurrentMap = mapNum;
            EventIndex = eventIndex;
            switch (MovementSpeed)
            {
                case 0:
                    Stat[2] = 5;
                    break;
                case 1:
                    Stat[2] = 10;
                    break;
                case 3:
                    Stat[2] = 20;
                    break;
                case 4:
                    Stat[2] = 30;
                    break;
                case 5:
                    Stat[2] = 40;
                    break;

            }
            MySprite = MyPage.Graphic.Filename;
            Face = MyPage.FaceGraphic;
            Client = client;
            if (Client != null)
            {
                Local = true;
                PacketSender.SendEntityDataTo(client, MyIndex, (int) Enums.EntityTypes.Event, Data(), this);
            }
        }
        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(base.Data());
            bf.WriteInteger(MyPage.HideName);
            bf.WriteInteger(MyPage.DisablePreview);
            bf.WriteString(MyPage.Desc);
            return bf.ToArray();
        }
        public void Update(Client client)
        {
            if (MoveTimer >= Environment.TickCount) return;
            if (MovementType != 1) return;
            var i = Globals.Rand.Next(0, 1);
            if (i != 0) return;
            i = Globals.Rand.Next(0, 4);
            if (CanMove(i) > 0) return;
            Move(i, client);
            switch (MovementFreq)
            {
                case 0:
                    MoveTimer = Environment.TickCount + 4000;
                    break;
                case 1:
                    MoveTimer = Environment.TickCount + 2000;
                    break;
                case 2:
                    MoveTimer = Environment.TickCount + 1000;
                    break;
                case 3:
                    MoveTimer = Environment.TickCount + 500;
                    break;
                case 4:
                    MoveTimer = Environment.TickCount + 250;
                    break;
            }
        }

        public bool ShouldDespawn()
        {
            for (int i = 0; i < MyPage.Conditions.Count; i++)
            {
                if (!EventIndex.MeetsConditions(MyPage.Conditions[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class EventIndex
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
        public EventPageInstance PageInstance;

        public Stack<EventStack> CallStack = new Stack<EventStack>();

        public EventIndex(int index, Client client, EventStruct baseEvent)
        {
            MyIndex = index;
            MyClient = client;
            MyPlayer = (Player)Globals.Entities[MyClient.EntityIndex];
            BaseEvent = baseEvent;
        }

        public void Update()
        {
            if (PageInstance != null && !IsGlobal)
            {
                //Check for despawn
                if (PageInstance.ShouldDespawn())
                {
                    PageInstance = null;
                    PacketSender.SendEntityLeaveTo(MyClient, MyIndex, (int) Enums.EntityTypes.LocalEvent);
                }
                else
                {
                    PageInstance.Update(MyClient);
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
                            var newStack = new EventStack {CommandIndex = 0, ListIndex = 0};
                            CallStack.Push(newStack);
                        }
                    }
                }
            }
            else if (!IsGlobal)
            {
                //Try to Spawn a PageInstance.. if we can
                for (int i = 0; i < BaseEvent.MyPages.Count; i++)
                {
                    if (CanSpawnPage(i, BaseEvent))
                    {
                        PageInstance = new EventPageInstance(BaseEvent, BaseEvent.MyPages[i], MyIndex, MapNum, this,MyClient);
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
                case 0:
                    if (MyPlayer.Switches[conditionCommand.Ints[1]] == Convert.ToBoolean(conditionCommand.Ints[2]))
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }


        private void ProcessCommand(EventCommand command)
        {
            CallStack.Peek().WaitingForResponse = 0;
            CallStack.Peek().ResponseType = 0;
            switch (command.Type)
            {
                case EventCommandType.ShowText:
                    PacketSender.SendEventDialog(MyClient, command.Strs[0],command.Strs[1], MyIndex);
                    CallStack.Peek().WaitingForResponse = 1;
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ShowOptions:
                    PacketSender.SendEventDialog(MyClient, command.Strs[0], command.Strs[1], command.Strs[2],
                        command.Strs[3], command.Strs[4], command.Strs[5], MyIndex);
                    CallStack.Peek().WaitingForResponse = 1;
                    CallStack.Peek().ResponseType = 1;
                    break;
                case EventCommandType.SetSwitch:
                    MyPlayer.Switches[command.Ints[0]] = Convert.ToBoolean(command.Ints[1]);
                    CallStack.Peek().CommandIndex++;
                    break;
                case EventCommandType.ConditionalBranch:
                    if (MeetsConditions(command))
                    {
                        var tmpStack = new EventStack
                        {
                            CommandIndex = 0,
                            ListIndex =
                                PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[0]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    else
                    {
                        var tmpStack = new EventStack
                        {
                            CommandIndex = 0,
                            ListIndex =
                                PageInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[1]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
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

    }

    public class EventStack
    {
        public int ListIndex;
        public int CommandIndex;
        public int WaitingForResponse;
        public int ResponseType;
    }
}
