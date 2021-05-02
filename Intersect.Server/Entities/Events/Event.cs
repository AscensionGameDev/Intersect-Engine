using System;
using System.Collections.Generic;

using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.Logging;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Maps;
using Intersect.Server.Networking;

namespace Intersect.Server.Entities.Events
{

    public partial class Event
    {

        public EventBase BaseEvent;

        public Stack<CommandInstance> CallStack = new Stack<CommandInstance>();

        public bool Global;

        public EventPageInstance[] GlobalPageInstance;

        public bool HoldingPlayer;

        public Guid Id;

        public Guid MapId;

        public MapInstance MapInstance;

        private Dictionary<string, string> mParams = new Dictionary<string, string>();

        public int PageIndex;

        public EventPageInstance PageInstance;

        public Player Player;

        //Special conditions
        public bool PlayerHasDied;

        public int SpawnX;

        public int SpawnY;

        public long WaitTimer;

        public int X;

        public int Y;

        public Event(Guid instanceId, MapInstance map, Player player, EventBase baseEvent)
        {
            Id = instanceId;
            MapId = map?.Id ?? Guid.Empty;
            MapInstance = map;
            Player = player;
            SelfSwitch = new bool[4];
            BaseEvent = baseEvent;
            X = baseEvent.SpawnX;
            Y = baseEvent.SpawnY;
        }

        public Event(Guid instanceId, EventBase baseEvent, MapInstance map) //Global constructor
        {
            Id = instanceId;
            Global = true;
            MapId = map?.Id ?? Guid.Empty;
            MapInstance = map;
            BaseEvent = baseEvent;
            SelfSwitch = new bool[4];
            GlobalPageInstance = new EventPageInstance[BaseEvent.Pages.Count];
            X = (byte) baseEvent.SpawnX;
            Y = baseEvent.SpawnY;
            for (var i = 0; i < BaseEvent.Pages.Count; i++)
            {
                GlobalPageInstance[i] = new EventPageInstance(BaseEvent, BaseEvent.Pages[i], MapId, this, null);
            }
        }

        public bool[] SelfSwitch { get; set; }

        public void Update(long timeMs, MapInstance map)
        {
            var sendLeave = false;
            var originalPageInstance = PageInstance;
            if (PageInstance != null)
            {
                //Check for despawn
                if (PageInstance.ShouldDespawn(map))
                {
                    X = PageInstance.X;
                    Y = PageInstance.Y;
                    if (PageInstance.GlobalClone != null)
                    {
                        Player.GlobalPageInstanceLookup.TryRemove(PageInstance.GlobalClone, out Event val);
                    }
                    PageInstance = null;
                    CallStack.Clear();
                    PlayerHasDied = false;
                    if (HoldingPlayer)
                    {
                        PacketSender.SendReleasePlayer(Player, Id);
                        HoldingPlayer = false;
                    }

                    sendLeave = true;
                }
                else
                {
                    if (!Global)
                    {
                        PageInstance.Update(
                            CallStack.Count > 0, timeMs
                        ); //Process movement and stuff that is client specific
                    }

                    //Check to see if we should process event commands
                    if (CallStack.Count > 0)
                    {
                        var curStack = CallStack.Peek();
                        if (curStack == null)
                        {
                            Log.Error("Curstack variable in event update is null.. not sure how nor how to recover so just gonna let this crash now..");
                        }
                        if (Player == null)
                        {
                            Log.Error("Player variable in event update is null.. not sure how nor how to recover so just gonna let this crash now..");
                        }
                        if (curStack.WaitingForResponse == CommandInstance.EventResponse.Shop && Player.InShop == null)
                        {
                            curStack.WaitingForResponse = CommandInstance.EventResponse.None;
                        }

                        if (curStack.WaitingForResponse == CommandInstance.EventResponse.Crafting &&
                            Player.CraftingTableId == Guid.Empty)
                        {
                            curStack.WaitingForResponse = CommandInstance.EventResponse.None;
                        }

                        if (curStack.WaitingForResponse == CommandInstance.EventResponse.Bank && Player.InBank == false)
                        {
                            curStack.WaitingForResponse = CommandInstance.EventResponse.None;
                        }

                        if (curStack.WaitingForResponse == CommandInstance.EventResponse.Quest &&
                            !Player.QuestOffers.Contains(((StartQuestCommand) curStack.WaitingOnCommand).QuestId))
                        {
                            curStack.WaitingForResponse = CommandInstance.EventResponse.None;
                        }

                        if (curStack.WaitingForResponse == CommandInstance.EventResponse.Timer &&
                            WaitTimer < Globals.Timing.Milliseconds)
                        {
                            curStack.WaitingForResponse = CommandInstance.EventResponse.None;
                        }

                        var commandsExecuted = 0;
                        while (curStack != null && curStack.WaitingForResponse == CommandInstance.EventResponse.None &&
                               !(PageInstance?.ShouldDespawn(map) ?? false) &&
                               commandsExecuted < Options.EventWatchdogKillThreshhold)
                        {
                            if (curStack.WaitingForRoute != Guid.Empty)
                            {
                                if (curStack.WaitingForRoute == Player.Id)
                                {
                                    if (Player.MoveRoute == null ||
                                        Player.MoveRoute.Complete && Player.MoveTimer < Globals.Timing.Milliseconds)
                                    {
                                        curStack.WaitingForRoute = Guid.Empty;
                                        curStack.WaitingForRouteMap = Guid.Empty;
                                    }
                                }
                                else
                                {
                                    //Check if the exist exists && if the move route is completed.
                                    foreach (var evt in Player.EventLookup)
                                    {
                                        if (evt.Value.MapId == curStack.WaitingForRouteMap &&
                                            evt.Value.BaseEvent.Id == curStack.WaitingForRoute)
                                        {
                                            if (evt.Value.PageInstance == null)
                                            {
                                                break;
                                            }

                                            if (!evt.Value.PageInstance.MoveRoute.Complete)
                                            {
                                                break;
                                            }

                                            curStack.WaitingForRoute = Guid.Empty;
                                            curStack.WaitingForRouteMap = Guid.Empty;

                                            break;
                                        }
                                    }
                                }

                                if (curStack.WaitingForRoute != Guid.Empty)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (curStack.CommandIndex >= curStack.CommandList.Count)
                                {
                                    CallStack.Pop();
                                }
                                else
                                {
                                    if (WaitTimer < Globals.Timing.Milliseconds)
                                    {
                                        CommandProcessing.ProcessCommand(curStack.Command, Player, this);
                                        commandsExecuted++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (CallStack.Count == 0)
                                {
                                    PlayerHasDied = false;

                                    break;
                                }
                            }

                            curStack = CallStack.Peek();
                        }

                        if (commandsExecuted >= Options.EventWatchdogKillThreshhold)
                        {
                            CallStack.Clear(); //Killing this event, we're over it.
                            if (this.BaseEvent.MapId == Guid.Empty)
                            {
                                Log.Error(Strings.Events.watchdogkillcommon.ToString(BaseEvent.Name));
                                if (Player.Power.IsModerator)
                                {
                                    PacketSender.SendChatMsg(
                                        Player, Strings.Events.watchdogkillcommon.ToString(BaseEvent.Name), ChatMessageType.Error, Color.Red
                                    );
                                }
                            }
                            else
                            {
                                Log.Error(Strings.Events.watchdogkill.ToString(map.Name, BaseEvent.Name));
                                if (Player.Power.IsModerator)
                                {
                                    PacketSender.SendChatMsg(
                                        Player, Strings.Events.watchdogkill.ToString(map.Name, BaseEvent.Name),
                                        ChatMessageType.Error, Color.Red
                                    );
                                }
                            }
                        }
                    }
                    else
                    {
                        if (PageInstance.Trigger == EventTrigger.Autorun && WaitTimer < Globals.Timing.Milliseconds)
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
                for (var i = BaseEvent.Pages.Count - 1; i >= 0; i--)
                {
                    if (Conditions.CanSpawnPage(BaseEvent.Pages[i], Player, this))
                    {
                        if (Global)
                        {
                            var globalEvent = MapInstance.Get(MapId).GetGlobalEventInstance(BaseEvent);
                            if (globalEvent != null)
                            {
                                PageInstance = new EventPageInstance(
                                    BaseEvent, BaseEvent.Pages[i], BaseEvent.Id, MapId, this, Player,
                                    globalEvent.GlobalPageInstance[i]
                                );

                                if (PageInstance.GlobalClone != null)
                                {
                                    Player.GlobalPageInstanceLookup.AddOrUpdate(globalEvent.GlobalPageInstance[i], this, (key, oldValue) => this);
                                }

                                sendLeave = false;
                                PageIndex = i;
                            }
                        }
                        else
                        {
                            PageInstance = new EventPageInstance(BaseEvent, BaseEvent.Pages[i], MapId, this, Player);
                            sendLeave = false;
                            PageIndex = i;
                        }

                        break;
                    }
                }

                if (sendLeave && originalPageInstance != null)
                {
                    PacketSender.SendEntityLeaveTo(Player, originalPageInstance);
                }
            }
        }

        public Dictionary<string, string> GetParams(Player player)
        {
            var prams = new Dictionary<string, string>();

            foreach (var prm in mParams)
            {
                prams.Add(prm.Key, prm.Value);
            }

            prams.Add("evtName", BaseEvent.Name);

            var map = MapInstance.Get(BaseEvent.MapId);
            if (map != null)
            {
                prams.Add("evtMap", map.Name);
            }

            if (MapId != Guid.Empty)
            {
                if (GlobalPageInstance != null)
                {
                    prams.Add("evtX", GlobalPageInstance[PageIndex].X.ToString());
                    prams.Add("evtY", GlobalPageInstance[PageIndex].Y.ToString());
                }
                else if (PageInstance != null)
                {
                    prams.Add("evtX", PageInstance.X.ToString());
                    prams.Add("evtY", PageInstance.Y.ToString());
                }
            }

            if (player != null)
            {
                //Player Name, Map, X, Y, Z?
                //Player Vitals, Player Stats, Player Sprite?
                //More later.. good start now
                prams.Add("plyrName", player.Name);
                prams.Add("plyrMap", player.Map.Name);
                prams.Add("plyrX", player.X.ToString());
                prams.Add("plyrY", player.Y.ToString());
                prams.Add("plyrZ", player.Z.ToString());
                prams.Add("plyrSprite", player.Sprite);
                prams.Add("plyrFace", player.Face);
                prams.Add("plyrLvl", player.Level.ToString());

                //Vitals
                for (var i = 0; i < player.GetVitals().Length; i++)
                {
                    prams.Add("plyrVit" + i, player.GetVital(i).ToString());
                    prams.Add("plyrMaxVit" + i, player.GetMaxVital(i).ToString());
                }

                //Stats
                var stats = player.GetStatValues();
                for (var i = 0; i < stats.Length; i++)
                {
                    prams.Add("plyrStat" + i, stats[i].ToString());
                }
            }

            return prams;
        }

        public void SetParam(string key, string value)
        {
            key = key.ToLower();
            if (mParams.ContainsKey(key))
            {
                mParams[key] = value;
            }
            else
            {
                mParams.Add(key, value);
            }
        }

        public string GetParam(Player player, string key)
        {
            key = key.ToLower();

            var prams = GetParams(player);

            foreach (var pair in prams)
            {
                if (string.Equals(pair.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return pair.Value;
                }
            }

            return "";
        }

        public string FormatParameters(Player player)
        {
            var prams = GetParams(player);
            var output = "{" + Environment.NewLine;
            foreach (var p in prams)
            {
                output += "\t\t\t\"" + p.Key + "\":\t\t\"" + p.Value + "\"," + Environment.NewLine;
            }

            output += "}";

            return output;
        }

    }

}
