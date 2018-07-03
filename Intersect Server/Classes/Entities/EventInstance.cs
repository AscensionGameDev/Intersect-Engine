using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Conditions;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.Server.Classes.Localization;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.Database;
using Intersect.Server.Classes.Database.PlayerData.Characters;
using Intersect.Server.Classes.Events;
using Intersect.Server.Classes.General;

using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;
using Intersect.Server.Classes.Spells;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Server.Classes.Entities
{
    using LegacyDatabase = Intersect.Server.Classes.Core.LegacyDatabase;

    public class EventInstance
    {
        public Guid Id;

        public EventBase BaseEvent;

        public Stack<CommandInstance> CallStack = new Stack<CommandInstance>();
        public int CurrentX;
        public int CurrentY;
        public EventPageInstance[] GlobalPageInstance;
        public bool HoldingPlayer;
        public bool IsGlobal;
        public Guid MapId;
        public Client MyClient;
        public Player MyPlayer;
        public bool NpcDeathTriggerd;
        public int PageIndex;
        public EventPageInstance PageInstance;

        //Special conditions
        public bool PlayerHasDied;
        public int SpawnX;
        public int SpawnY;
        public long WaitTimer;

        public bool[] SelfSwitch { get; set; }

        public EventInstance(Guid instanceId, Guid map, Client client, EventBase baseEvent)
        {
            Id = instanceId;
            MyClient = client;
            MapId = map;
            MyPlayer = client.Entity;
            SelfSwitch = new bool[4];
            BaseEvent = baseEvent;
            MapId = map;
            CurrentX = baseEvent.SpawnX;
            CurrentY = baseEvent.SpawnY;
        }

        public EventInstance(Guid instanceId, EventBase baseEvent,Guid map) //Global constructor
        {
            Id = instanceId;
            IsGlobal = true;
            MapId = map;
            BaseEvent = baseEvent;
            SelfSwitch = new bool[4];
            GlobalPageInstance = new EventPageInstance[BaseEvent.Pages.Count];
            CurrentX = baseEvent.SpawnX;
            CurrentY = baseEvent.SpawnY;
            for (int i = 0; i < BaseEvent.Pages.Count; i++)
            {
                GlobalPageInstance[i] = new EventPageInstance(BaseEvent, BaseEvent.Pages[i], MapId, this, null);
            }
        }

        public void Update(long timeMs)
        {
            var sendLeave = false;
            if (PageInstance != null)
            {
                //Check for despawn
                if (PageInstance.ShouldDespawn())
                {
                    CurrentX = PageInstance.X;
                    CurrentY = PageInstance.Y;
                    PageInstance = null;
                    PlayerHasDied = false;
                    if (HoldingPlayer)
                    {
                        PacketSender.SendReleasePlayer(MyClient, Id);
                        HoldingPlayer = false;
                    }
                    sendLeave = true;
                }
                else
                {
                    if (!IsGlobal)
                        PageInstance.Update(CallStack.Count > 0, timeMs); //Process movement and stuff that is client specific

                    //Check to see if we should process event commands
                    if (CallStack.Count > 0)
                    {
                        if (CallStack.Peek().WaitingForResponse == CommandInstance.EventResponse.Shop && MyPlayer.InShop == null) CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                        if (CallStack.Peek().WaitingForResponse == CommandInstance.EventResponse.Crafting && MyPlayer.CraftingTableId == Guid.Empty) CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                        if (CallStack.Peek().WaitingForResponse == CommandInstance.EventResponse.Bank && MyPlayer.InBank == false) CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                        if (CallStack.Peek().WaitingForResponse == CommandInstance.EventResponse.Quest && !MyPlayer.QuestOffers.Contains(CallStack.Peek().ResponseId)) CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                        while (CallStack.Peek().WaitingForResponse == CommandInstance.EventResponse.None)
                        {
                            if (CallStack.Peek().WaitingForRoute != Guid.Empty)
                            {
                                if (CallStack.Peek().WaitingForRoute == MyPlayer.Id)
                                {
                                    if (MyPlayer.MoveRoute == null ||
                                        (MyPlayer.MoveRoute.Complete &&
                                         MyPlayer.MoveTimer < Globals.System.GetTimeMs()))
                                    {
                                        CallStack.Peek().WaitingForRoute = Guid.Empty;
                                        CallStack.Peek().WaitingForRouteMap = Guid.Empty;
                                    }
                                }
                                else
                                {
                                    //Check if the exist exists && if the move route is completed.
                                    foreach (var evt in MyPlayer.EventLookup.Values)
                                    {
                                        if (evt.MapId == CallStack.Peek().WaitingForRouteMap && evt.BaseEvent.Id == CallStack.Peek().WaitingForRoute)
                                        {
                                            if (evt.PageInstance == null) break;
                                            if (!evt.PageInstance.MoveRoute.Complete) break;
                                            CallStack.Peek().WaitingForRoute = Guid.Empty;
                                            CallStack.Peek().WaitingForRouteMap = Guid.Empty;
                                            break;
                                        }
                                    }
                                }
                                if (CallStack.Peek().WaitingForRoute != Guid.Empty) break;
                            }
                            else
                            {
                                if (CallStack.Peek().CommandIndex >=
                                    CallStack.Peek().CommandList.Count)
                                {
                                    CallStack.Pop();
                                }
                                else
                                {
                                    if (WaitTimer < Globals.System.GetTimeMs())
                                    {
                                        CommandProcessing.ProcessCommand(CallStack.Peek().Command,MyPlayer,this);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                if (CallStack.Count == 0)
                                {
                                    PlayerHasDied = false;
                                    NpcDeathTriggerd = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (PageInstance.Trigger == 2)
                        {
                            var newStack = new CommandInstance(PageInstance.MyPage);
                            CallStack.Push(newStack);
                        }
                    }
                }
            }

            if (PageInstance == null)
            {
                //Try to Spawn a PageInstance.. if we can
                for (int i = BaseEvent.Pages.Count - 1; i >= 0; i--)
                {
                    if (Conditions.CanSpawnPage(BaseEvent.Pages[i],MyPlayer,this))
                    {
                        if (IsGlobal)
                        {
                            if (MapInstance.Get(MapId).GetGlobalEventInstance(BaseEvent) != null)
                            {
                                PageInstance = new EventPageInstance(BaseEvent, BaseEvent.Pages[i],BaseEvent.Id,MapId, this, MyClient, MapInstance.Get(MapId).GetGlobalEventInstance(BaseEvent).GlobalPageInstance[i]);
                                sendLeave = false;
                                PageIndex = i;
                            }
                        }
                        else
                        {
                            PageInstance = new EventPageInstance(BaseEvent, BaseEvent.Pages[i], MapId, this, MyClient);
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
                        PacketSender.SendEntityLeaveTo(MyClient, Id, (int) EntityTypes.Event, MapId);
                    }
                    else
                    {
                        PacketSender.SendEntityLeaveTo(MyClient, Id, (int) EntityTypes.Event, MapId);
                    }
                }
            }
        }
    }

    public class CommandInstance
    {
        public enum EventResponse
        {
            None = 0,
            Dialogue,
            Shop,
            Bank,
            Crafting,
            Quest,
        }

        private int commandIndex;
        public int CommandIndex
        {
            get { return commandIndex; }
            set
            {
                commandIndex = value;
                Command = commandIndex >= 0 && commandIndex < CommandList.Count ? CommandList[commandIndex] : null;
            }
        }
        public EventCommand Command;
        public Guid CommandListId;
        public List<EventCommand> CommandList;

        public EventPage Page;
        public Guid ResponseId;
        public EventResponse WaitingForResponse = EventResponse.None;
        public Guid WaitingForRoute;
        public Guid WaitingForRouteMap;

        public CommandInstance(EventPage page, int listIndex = 0)
        {
            Page = page;
            CommandList = page.CommandLists.Values.First();
            CommandIndex = listIndex;
        }

        public CommandInstance(EventPage page, List<EventCommand> commandList, int listIndex = 0)
        {
            Page = page;
            CommandList = commandList;
            CommandIndex = listIndex;
        }

        public CommandInstance(EventPage page, Guid commandListId, int listIndex = 0)
        {
            Page = page;
            CommandList = page.CommandLists[commandListId];
            CommandIndex = listIndex;
        }
    }
}