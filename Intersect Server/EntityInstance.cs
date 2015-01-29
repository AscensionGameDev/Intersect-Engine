using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntersectServer
{
    public class EventIndex
    {
        public bool isGlobal;
        public int mapNum;
        public int myIndex;
        public Client myClient;
        public Player myPlayer;
        public int spawnX;
        public int spawnY;
        public int pageIndex;
        public EventInstance eventInstance;

        public Stack<EventStack> CallStack = new Stack<EventStack>();

        public EventIndex(int index, Client client)
        {
            myIndex = index;
            myClient = client;
            myPlayer = (Player)GlobalVariables.entities[myClient.entityIndex];
        }

        public void Update()
        {
            if (!isGlobal) { eventInstance.Update(myClient); }
            if (CallStack.Count > 0)
            {
                while (CallStack.Peek().waitingForResponse == 0)
                {
                    if (CallStack.Peek().commandIndex >= eventInstance.baseEvent.myPages[pageIndex].commandLists[CallStack.Peek().listIndex].commands.Count)
                    {
                        CallStack.Pop();
                    }
                    else
                    {
                        ProcessCommand(eventInstance.baseEvent.myPages[pageIndex].commandLists[CallStack.Peek().listIndex].commands[CallStack.Peek().commandIndex]);
                    }
                    if (CallStack.Count == 0) { break; }
                }
            }
            else
            {
                if (eventInstance.trigger == 1)
                {
                    EventStack newStack = new EventStack();
                    newStack.commandIndex = 0;
                    newStack.listIndex = 0;
                    CallStack.Push(newStack);
                }
            }
        }

        private void ProcessCommand(EventCommand ec)
        {
            CallStack.Peek().waitingForResponse = 0;
            CallStack.Peek().responseType = 0;
            switch (ec.type)
            {
                case 0:
                    PacketSender.SendEventDialog(myClient, ec.strs[0], myIndex);
                    CallStack.Peek().waitingForResponse = 1;
                    CallStack.Peek().commandIndex++;
                    break;
                case 1:
                    PacketSender.SendEventDialog(myClient, ec.strs[0], ec.strs[1], ec.strs[2], ec.strs[3], ec.strs[4], myIndex);
                    CallStack.Peek().waitingForResponse = 1;
                    CallStack.Peek().responseType = 1;
                    break;
                case 2:
                    myPlayer.switches[ec.ints[0]-1] = Convert.ToBoolean(ec.ints[1]);
                    CallStack.Peek().commandIndex++;
                    break;
                case 3:
                    break;
                case 4:
                    if (myPlayer.MeetsConditions(ec.myConditions, eventInstance))
                    {
                        EventStack tmpStack = new EventStack();
                        tmpStack.commandIndex = 0;
                        tmpStack.listIndex = eventInstance.baseEvent.myPages[pageIndex].commandLists[CallStack.Peek().listIndex].commands[CallStack.Peek().commandIndex].ints[0];
                        CallStack.Peek().commandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    else
                    {
                        EventStack tmpStack = new EventStack();
                        tmpStack.commandIndex = 0;
                        tmpStack.listIndex = eventInstance.baseEvent.myPages[pageIndex].commandLists[CallStack.Peek().listIndex].commands[CallStack.Peek().commandIndex].ints[1];
                        CallStack.Peek().commandIndex++;
                        CallStack.Push(tmpStack);
                    }
                    break;
                case 5:
                    myPlayer.Warp(eventInstance.baseEvent.myPages[pageIndex].commandLists[CallStack.Peek().listIndex].commands[CallStack.Peek().commandIndex].ints[0],
                        eventInstance.baseEvent.myPages[pageIndex].commandLists[CallStack.Peek().listIndex].commands[CallStack.Peek().commandIndex].ints[1],
                        eventInstance.baseEvent.myPages[pageIndex].commandLists[CallStack.Peek().listIndex].commands[CallStack.Peek().commandIndex].ints[2],
                        eventInstance.baseEvent.myPages[pageIndex].commandLists[CallStack.Peek().listIndex].commands[CallStack.Peek().commandIndex].ints[3]);
                    CallStack.Peek().commandIndex++;
                    break;
            }
        }

    }

    public class EventInstance : Entity
    {
        public int trigger = 0;
        public int movementType;
        public int movementFreq;
        public int movementSpeed;
        public Event baseEvent;
        public EventInstance(Event myEvent, int pageIndex, int myIndex, int mapNum)
            : base(myIndex)
        {
            baseEvent = myEvent;
            currentMap = mapNum;
            isEvent = 1;
            currentX = myEvent.spawnX;
            currentY = myEvent.spawnY;
            myName = myEvent.myName;
            movementType = myEvent.myPages[pageIndex].movementType;
            movementFreq = myEvent.myPages[pageIndex].movementFreq;
            movementSpeed = myEvent.myPages[pageIndex].movementSpeed;
            trigger = myEvent.myPages[pageIndex].trigger;
            passable = myEvent.myPages[pageIndex].passable;
            hideName = myEvent.myPages[pageIndex].hideName;
            base.currentX = myEvent.spawnX;
            base.currentY = myEvent.spawnY;
            base.currentMap = mapNum;
            switch (movementSpeed)
            {
                case 0:
                    base.stat[2] = 5;
                    break;
                case 1:
                    base.stat[2] = 10;
                    break;
                case 3:
                    base.stat[2] = 20;
                    break;
                case 4:
                    base.stat[2] = 30;
                    break;
                case 5:
                    base.stat[2] = 40;
                    break;

            }
            mySprite = myEvent.myPages[pageIndex].graphic;
        }
        public void Update(Client client)
        {
            int i;
            if (base.moveTimer < Environment.TickCount)
            {
                if (movementType == 1)
                {
                    i = GlobalVariables.rand.Next(0, 1);
                    if (i == 0)
                    {
                        i = GlobalVariables.rand.Next(0, 4);
                        if (!base.CanMove(i))
                        {
                            base.Move(i,client);
                            switch (movementFreq)
                            {
                                case 0:
                                    base.moveTimer = Environment.TickCount + 4000;
                                    break;
                                case 1:
                                    base.moveTimer = Environment.TickCount + 2000;
                                    break;
                                case 2:
                                    base.moveTimer = Environment.TickCount + 1000;
                                    break;
                                case 3:
                                    base.moveTimer = Environment.TickCount + 500;
                                    break;
                                case 4:
                                    base.moveTimer = Environment.TickCount + 250;
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

    public class EventStack
    {
        public int listIndex;
        public int commandIndex;
        public int waitingForResponse;
        public int responseType;
    }
}
