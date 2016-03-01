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
using System.Drawing;

namespace Intersect_Server.Classes
{
    public class EventPageInstance : Entity
    {
        public Client Client = null;
        public int Trigger;
        public int MovementType;
        public int MovementFreq;
        public int MovementSpeed;
        public int MyGraphicType;
        public EventStruct BaseEvent;
        public EventPage MyPage;
        public EventIndex MyEventIndex;
        public EventPageInstance GlobalClone;
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
            MyEventIndex = eventIndex;
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
            MyGraphicType = MyPage.Graphic.Type;
            MySprite = MyPage.Graphic.Filename;
            if (MyGraphicType == 1)
            {
                switch (MyPage.Graphic.Y)
                {
                    case 0:
                        Dir = 1;
                        break;
                    case 1:
                        Dir = 2;
                        break;
                    case 2:
                        Dir = 3;
                        break;
                    case 3:
                        Dir = 0;
                        break;
                }
            }
            if (myPage.Animation > -1)
            {
                Animations.Add(myPage.Animation);
            }
            Face = MyPage.FaceGraphic;
            Client = client;
            SendToClient();
        }
        public EventPageInstance(EventStruct myEvent, EventPage myPage, int myIndex, int mapNum, EventIndex eventIndex, Client client, EventPageInstance globalClone) : base(myIndex)
        {
            BaseEvent = myEvent;
            GlobalClone = globalClone;
            MyPage = myPage;
            CurrentMap = mapNum;
            CurrentX = globalClone.CurrentX;
            CurrentY = globalClone.CurrentY;
            MyName = myEvent.MyName;
            MovementType = globalClone.MovementType;
            MovementFreq = globalClone.MovementFreq;
            MovementSpeed = globalClone.MovementSpeed;
            Trigger = MyPage.Trigger;
            Passable = globalClone.Passable;
            HideName = globalClone.HideName;
            CurrentMap = mapNum;
            MyEventIndex = eventIndex;
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
            MyGraphicType = MyPage.Graphic.Type;
            MySprite = MyPage.Graphic.Filename;
            if (MyGraphicType == 1)
            {
                switch (MyPage.Graphic.Y)
                {
                    case 0:
                        Dir = 1;
                        break;
                    case 1:
                        Dir = 2;
                        break;
                    case 2:
                        Dir = 3;
                        break;
                    case 3:
                        Dir = 0;
                        break;
                }
            }
            if (myPage.Animation > -1)
            {
                Animations.Add(myPage.Animation);
            }
            Face = MyPage.FaceGraphic;
            Client = client;
            SendToClient();
        }

        public void SendToClient()
        {
            if (Client != null)
            {
                PacketSender.SendEntityDataTo(Client, MyIndex, (int)Enums.EntityTypes.Event, Data(), this);
            }
        }
        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(base.Data());
            bf.WriteInteger(MyPage.HideName);
            bf.WriteInteger(MyPage.DisablePreview);
            bf.WriteString(MyPage.Desc);
            bf.WriteInteger(MyPage.Graphic.Type);
            return bf.ToArray();
        }
        public void Update(Client client)
        {
            if (MoveTimer >= Environment.TickCount || GlobalClone != null) return;
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
                if (!MyEventIndex.MeetsConditions(MyPage.Conditions[i]))
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
        public bool[] SelfSwitch { get; set; }
        public EventPageInstance PageInstance;
        public EventPageInstance[] GlobalPageInstance;

        public Stack<EventStack> CallStack = new Stack<EventStack>();

        public EventIndex(int index, Client client, EventStruct baseEvent, int mapNum)
        {
            MyIndex = index;
            MyClient = client;
            MapNum = mapNum;
            MyPlayer = (Player)Globals.Entities[MyClient.EntityIndex];
            SelfSwitch = new bool[4];
            BaseEvent = baseEvent;
        }

        public EventIndex(EventStruct baseEvent,int index,  int mapNum) //Global constructor
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
                            var newStack = new EventStack {CommandIndex = 0, ListIndex = 0};
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
            CallStack.Peek().WaitingForResponse = 0;
            CallStack.Peek().ResponseType = 0;
            switch (command.Type)
            {
                case EventCommandType.ShowText:
                    PacketSender.SendEventDialog(MyClient, command.Strs[0],command.Strs[1],MapNum, BaseEvent.MyIndex);
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
                            PacketSender.SendPlayerMsg(MyClient,command.Strs[0],Color.FromName(command.Strs[1]));
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
                            MyPlayer.Variables[command.Ints[0]] =  command.Ints[2];
                            break;
                        case 1: //Add
                            MyPlayer.Variables[command.Ints[0]] += command.Ints[2];
                            break;
                        case 2: //Subtract
                            MyPlayer.Variables[command.Ints[0]] -= command.Ints[2];
                            break;
                        case 3: //Random
                            MyPlayer.Variables[command.Ints[0]] = Globals.Rand.Next(command.Ints[2],command.Ints[3] + 1);
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
                        var tmpStack = new EventStack
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
                        var tmpStack = new EventStack
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
