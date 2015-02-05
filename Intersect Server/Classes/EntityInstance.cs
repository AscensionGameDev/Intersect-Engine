using System;
using System.Collections.Generic;

namespace Intersect_Server.Classes
{
    public class EventIndex
    {
        public bool IsGlobal;
        public int MapNum;
        public int MyIndex;
        public Client MyClient;
        public Player MyPlayer;
        public int SpawnX;
        public int SpawnY;
        public int PageIndex;
        public EventInstance EventInstance;

        public Stack<EventStack> CallStack = new Stack<EventStack>();

        public EventIndex(int index, Client client)
        {
            MyIndex = index;
            MyClient = client;
            MyPlayer = (Player)Globals.Entities[MyClient.entityIndex];
        }

        public void Update()
        {
            if (!IsGlobal) { EventInstance.Update(MyClient); }
            if (CallStack.Count > 0)
            {
                while (CallStack.Peek().WaitingForResponse == 0)
                {
                    if (CallStack.Peek().CommandIndex >= EventInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex].Commands.Count)
                    {
                        CallStack.Pop();
                    }
                    else
                    {
                        ProcessCommand(EventInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex]);
                    }
                    if (CallStack.Count == 0) { break; }
                }
            }
            else
            {
                if (EventInstance.Trigger != 1) return;
                var newStack = new EventStack {CommandIndex = 0, ListIndex = 0};
                CallStack.Push(newStack);
            }
        }

        private void ProcessCommand(EventCommand ec)
        {
            CallStack.Peek().WaitingForResponse = 0;
            CallStack.Peek().ResponseType = 0;
            switch (ec.Type)
            {
                case 0:
                    PacketSender.SendEventDialog(MyClient, ec.Strs[0], MyIndex);
                    CallStack.Peek().WaitingForResponse = 1;
                    CallStack.Peek().CommandIndex++;
                    break;
                case 1:
                    PacketSender.SendEventDialog(MyClient, ec.Strs[0], ec.Strs[1], ec.Strs[2], ec.Strs[3], ec.Strs[4], MyIndex);
                    CallStack.Peek().WaitingForResponse = 1;
                    CallStack.Peek().ResponseType = 1;
                    break;
                case 2:
                    MyPlayer.Switches[ec.Ints[0]-1] = Convert.ToBoolean(ec.Ints[1]);
                    CallStack.Peek().CommandIndex++;
                    break;
                case 3:
                    break;
                case 4:
                    if (MyPlayer.MeetsConditions(ec.MyConditions, EventInstance))
                    {
                        var tmpStack = new EventStack
                        {
                            CommandIndex = 0,
                            ListIndex =
                                EventInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex]
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
                                EventInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex]
                                    .Commands[CallStack.Peek().CommandIndex].Ints[1]
                        };
                        CallStack.Peek().CommandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    break;
                case 5:
                    MyPlayer.Warp(EventInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[0],
                        EventInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[1],
                        EventInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[2],
                        EventInstance.BaseEvent.MyPages[PageIndex].CommandLists[CallStack.Peek().ListIndex].Commands[CallStack.Peek().CommandIndex].Ints[3]);
                    CallStack.Peek().CommandIndex++;
                    break;
            }
        }

    }

    public class EventInstance : Entity
    {
        public int Trigger;
        public int MovementType;
        public int MovementFreq;
        public int MovementSpeed;
        public Event BaseEvent;
        public EventInstance(Event myEvent, int pageIndex, int myIndex, int mapNum)
            : base(myIndex)
        {
            BaseEvent = myEvent;
            CurrentMap = mapNum;
            IsEvent = 1;
            CurrentX = myEvent.SpawnX;
            CurrentY = myEvent.SpawnY;
            MyName = myEvent.MyName;
            MovementType = myEvent.MyPages[pageIndex].MovementType;
            MovementFreq = myEvent.MyPages[pageIndex].MovementFreq;
            MovementSpeed = myEvent.MyPages[pageIndex].MovementSpeed;
            Trigger = myEvent.MyPages[pageIndex].Trigger;
            Passable = myEvent.MyPages[pageIndex].Passable;
            HideName = myEvent.MyPages[pageIndex].HideName;
            CurrentX = myEvent.SpawnX;
            CurrentY = myEvent.SpawnY;
            CurrentMap = mapNum;
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
            MySprite = myEvent.MyPages[pageIndex].Graphic;
        }
        public void Update(Client client)
        {
            if (MoveTimer >= Environment.TickCount) return;
            if (MovementType != 1) return;
            var i = Globals.Rand.Next(0, 1);
            if (i != 0) return;
            i = Globals.Rand.Next(0, 4);
            if (CanMove(i)) return;
            Move(i,client);
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
    }

    public class EventStack
    {
        public int ListIndex;
        public int CommandIndex;
        public int WaitingForResponse;
        public int ResponseType;
    }
}
