using System;
using System.Collections.Generic;

namespace IntersectServer
{

	public class Player : Entity
	{
		public bool inGame;
        public Client myClient;
        private bool sentMap = false;
        public List<EventIndex> myEvents = new List<EventIndex>();
        public bool[] switches;
        public int[] variables;

		public Player (int index, Client newClient) : base(index)
		{
            myClient = newClient;
            switches = new bool[Constants.SWITCH_COUNT];
            variables = new int[Constants.VARIABLE_COUNT];
		}

        public override void Die()
        {
            base.Die();
            base.Reset();
            Respawn();
        }

        public override void Warp(int newMap, int newX, int newY)
        {
            Warp(newMap, newX, newY);
        }
        public override void Warp(int newMap, int newX, int newY, int newDir)
        {
            
            currentX = newX;
            currentY = newY;
            if (newMap != currentMap || sentMap == false)
            {
                Console.WriteLine("Sending warp to player.");
                PacketSender.SendEntityLeave(myIndex,0);
                currentMap = newMap;
                PacketSender.SendEntityDataToAll(myIndex, 0, GlobalVariables.entities[myIndex]);
                PacketSender.SendEntityPositionToAll(myIndex,0,GlobalVariables.entities[myIndex]);
                PacketSender.SendMap(myClient,newMap);
                PacketSender.SendEnterMap(myClient,newMap);
                sentMap = true;
                for (int i = 0; i < GlobalVariables.entities.Count; i++)
                {
                    if (i != myIndex)
                    {
                        PacketSender.SendEntityPositionTo(myClient, i,0, GlobalVariables.entities[i]);
                    }
                }
            }
            else
            {
                PacketSender.SendEntityPositionToAll(myIndex,0,GlobalVariables.entities[myIndex]);
                PacketSender.SendEntityVitals(myIndex, isEvent, GlobalVariables.entities[myIndex]);
                PacketSender.SendEntityStats(myIndex, isEvent, GlobalVariables.entities[myIndex]);
            }
            
        }
        private void Respawn()
        {
            Warp(Constants.SPAWN_MAP, Constants.SPAWN_X, Constants.SPAWN_Y, 1);
        }

        public bool MeetsConditions(EventConditions ec, EventInstance ei){
            if (ec.switch1 - 1 > -1)
            {
                if (switches[ec.switch1 - 1] != ec.switch1val)
                {
                    return false;
                }
            }
            if (ec.switch2 - 1 > -1)
            {
                if (switches[ec.switch2 - 1] != ec.switch2val)
                {
                    return false;
                }
            }
            return true;
        }

        public void Update()
        {
            int mapNum;
            bool eventFound;
            int foundEvent;
            EventIndex tmpEvent;
            int pageIndex;
            if (!inGame) { return; }
            //Check to see if we can spawn events, if already spawned.. update them.
            for (int i = 0; i < GlobalVariables.GameMaps[currentMap].surroundingMaps.Count + 1; i++)
            {
                if (i == GlobalVariables.GameMaps[currentMap].surroundingMaps.Count) { mapNum = currentMap; } else { mapNum = GlobalVariables.GameMaps[currentMap].surroundingMaps[i]; }
                if (mapNum > -1)
                {
                    for (int x = 0; x < GlobalVariables.GameMaps[mapNum].Events.Count; x++)
                    {
                        if (CanSpawnEvent(GlobalVariables.GameMaps[mapNum].Events[x],mapNum))
                        {
                            //if event isnt spawned, spawn it
                            foundEvent = EventExists(mapNum, GlobalVariables.GameMaps[mapNum].Events[x].spawnX, GlobalVariables.GameMaps[mapNum].Events[x].spawnY);
                            if (foundEvent == -1)
                            {
                                pageIndex = SpawnEventPage(GlobalVariables.GameMaps[mapNum].Events[x]);
                                tmpEvent = new EventIndex(myEvents.Count,myClient);
                                tmpEvent.isGlobal = false;
                                tmpEvent.mapNum = mapNum;
                                tmpEvent.pageIndex = pageIndex;
                                tmpEvent.spawnX = GlobalVariables.GameMaps[mapNum].Events[x].spawnX;
                                tmpEvent.spawnY = GlobalVariables.GameMaps[mapNum].Events[x].spawnY;
                                myEvents.Add(tmpEvent);
                                tmpEvent.eventInstance = new EventInstance(GlobalVariables.GameMaps[mapNum].Events[x],pageIndex,myEvents.Count - 1,mapNum);
                                tmpEvent.eventInstance.dir = GlobalVariables.GameMaps[mapNum].Events[x].myPages[pageIndex].graphicy;
                                //Send Spawn Event to Player
                                PacketSender.SendEntityData(myClient, myEvents.Count - 1, 1, myEvents[myEvents.Count - 1].eventInstance);
                                PacketSender.SendEntityPositionTo(myClient, myEvents.Count - 1, 1,myEvents[myEvents.Count-1].eventInstance);

                            }
                            else
                            {
                                myEvents[foundEvent].Update();
                            }
                        }
                        else
                        {
                            //if event is up and running, destroy itttt
                            foundEvent = EventExists(mapNum, GlobalVariables.GameMaps[mapNum].Events[x].spawnX, GlobalVariables.GameMaps[mapNum].Events[x].spawnY);
                            if (foundEvent > -1 && foundEvent < myEvents.Count)
                            {
                                myEvents[foundEvent] = null;
                                PacketSender.SendEntityLeaveTo(myClient,foundEvent, 1);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < myEvents.Count; i++)
            {
                if (myEvents[i] != null)
                {
                    eventFound = false;
                    if (myEvents[i].mapNum != currentMap)
                    {
                        for (int x = 0; x < GlobalVariables.GameMaps[currentMap].surroundingMaps.Count; x++)
                        {
                            if (GlobalVariables.GameMaps[currentMap].surroundingMaps[x] == myEvents[i].mapNum)
                            {
                                eventFound = true;
                            }
                        }
                    }
                    else
                    {
                        eventFound = true;
                    }
                    if (eventFound == false)
                    {
                        myEvents[i] = null;
                        PacketSender.SendEntityLeaveTo(myClient, i, 1);
                    }
                }
            }
        }

        private int EventExists(int map, int x, int y)
        {
            for (int i = 0; i < myEvents.Count; i++)
            {
                if (myEvents[i] != null)
                {
                    if (map == myEvents[i].mapNum && x == myEvents[i].spawnX && y == myEvents[i].spawnY)
                    {
                        return i;
                    }
                }
            }
                return -1;
        }

        public void TryActivateEvent(int eventIndex)
        {
            EventStack newStack;
            if (eventIndex > -1 && eventIndex < myEvents.Count)
            {
                if (myEvents[eventIndex] != null)
                {
                    if (myEvents[eventIndex].eventInstance.trigger == 0)
                    {
                        if (isEventOneBlockAway(eventIndex))
                        {
                            if (myEvents[eventIndex].CallStack.Count == 0)
                            {
                                newStack = new EventStack();
                                newStack.commandIndex = 0;
                                newStack.listIndex = 0;
                                myEvents[eventIndex].CallStack.Push(newStack);
                            }
                        }
                    }
                }
            }
        }

        public void RespondToEvent(int eventIndex, int responseID)
        {
            if (eventIndex > -1 && eventIndex < myEvents.Count)
            {
                if (myEvents[eventIndex] != null)
                {
                    if (myEvents[eventIndex].CallStack.Count > 0)
                    {
                        if (myEvents[eventIndex].CallStack.Peek().waitingForResponse == 1)
                        {
                            if (myEvents[eventIndex].CallStack.Peek().responseType == 0)
                            {
                                myEvents[eventIndex].CallStack.Peek().waitingForResponse = 0;
                            }
                            else
                            {
                                EventStack tmpStack = new EventStack();
                                myEvents[eventIndex].CallStack.Peek().waitingForResponse = 0;
                                tmpStack.commandIndex = 0;
                                tmpStack.listIndex = myEvents[eventIndex].eventInstance.baseEvent.myPages[myEvents[eventIndex].pageIndex].commandLists[myEvents[eventIndex].CallStack.Peek().listIndex].commands[myEvents[eventIndex].CallStack.Peek().commandIndex].ints[responseID - 1];
                                myEvents[eventIndex].CallStack.Peek().commandIndex++;
                                myEvents[eventIndex].CallStack.Push(tmpStack);
                            }
                        }
                    }
                }
            }
        }

        bool isEventOneBlockAway(int eventIndex)
        {

            return true;
        }

        private int SpawnEventPage(Event myEvent)
        {
            return 0;
        }

        private bool CanSpawnEvent(Event myEvent, int myMap)
        {
            if (!MeetsConditions(myEvent.myPages[0].myConditions,new EventInstance(myEvent,0,0,myMap))){
                return false;
            }
            return true;
        }
	}
}

