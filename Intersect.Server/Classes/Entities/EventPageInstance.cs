using System;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.EventProcessing;
using Intersect.Server.General;
using Intersect.Server.Maps;
using Intersect.Server.Misc.Pathfinding;
using Intersect.Server.Networking;

namespace Intersect.Server.Entities
{
    public class EventPageInstance : EntityInstance
    {
        private Pathfinder mPathFinder;
        public EventBase BaseEvent;
        public Client Client;
        private bool mDirectionFix;
        public bool DisablePreview;
        public EventPageInstance GlobalClone;
        public EventInstance MyEventIndex;
        public EventGraphic MyGraphic = new EventGraphic();
        public EventPage MyPage;
        private int mPageNum;
        public string Param;
        private EventRenderLayer mRenderLayer = EventRenderLayer.SameAsPlayer;
        public EventTrigger Trigger;
        private bool mWalkingAnim;

        public EventMovementType MovementType;
        public EventMovementFrequency MovementFreq;
        private EventMovementSpeed mMovementSpeed;
        public EventMovementSpeed MovementSpeed
        {
            get { return mMovementSpeed; }
            set
            {
                mMovementSpeed = value;
                switch (mMovementSpeed)
                {
                    case EventMovementSpeed.Slowest:
                        Stat[(int) Stats.Speed].Stat = 2;
                        break;
                    case EventMovementSpeed.Slower:
                        Stat[(int) Stats.Speed].Stat = 5;
                        break;
                    case EventMovementSpeed.Normal:
                        Stat[(int) Stats.Speed].Stat = 20;
                        break;
                    case EventMovementSpeed.Faster:
                        Stat[(int)Stats.Speed].Stat = 30;
                        break;
                    case EventMovementSpeed.Fastest:
                        Stat[(int)Stats.Speed].Stat = 40;
                        break;
                }
            }
        }

        public EventPageInstance(EventBase myEvent, EventPage myPage, Guid mapId, EventInstance eventIndex, Client client) : base(Guid.NewGuid())
        {
            BaseEvent = myEvent;
            Id = BaseEvent.Id;
            MyPage = myPage;
            MapId = mapId;
            X = eventIndex.X;
            Y = eventIndex.Y;
            Name = myEvent.Name;
            MovementType = MyPage.Movement.Type;
            MovementFreq = MyPage.Movement.Frequency;
            MovementSpeed = MyPage.Movement.Speed;
            DisablePreview = MyPage.DisablePreview;
            Trigger = MyPage.Trigger;
            Passable = MyPage.Passable;
            HideName = MyPage.HideName;
            MyEventIndex = eventIndex;
            MoveRoute = new EventMoveRoute();
            MoveRoute.CopyFrom(MyPage.Movement.Route);
            mPathFinder = new Pathfinder(this);
            SetMovementSpeed(MyPage.Movement.Speed);
            MyGraphic.Type = MyPage.Graphic.Type;
            MyGraphic.Filename = MyPage.Graphic.Filename;
            MyGraphic.X = MyPage.Graphic.X;
            MyGraphic.Y = MyPage.Graphic.Y;
            MyGraphic.Width = MyPage.Graphic.Width;
            MyGraphic.Height = MyPage.Graphic.Height;
            Sprite = MyPage.Graphic.Filename;
            mDirectionFix = MyPage.DirectionFix;
            mWalkingAnim = MyPage.WalkingAnimation;
            mRenderLayer = MyPage.Layer;
            if (MyGraphic.Type == EventGraphicType.Sprite)
            {
                switch (MyGraphic.Y)
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
            if (myPage.AnimationId != Guid.Empty)
            {
                Animations.Add(myPage.AnimationId);
            }
            Face = MyPage.FaceGraphic;
            mPageNum = BaseEvent.Pages.IndexOf(MyPage);
            Client = client;
            SendToClient();
        }

        public EventPageInstance(EventBase myEvent, EventPage myPage, Guid instanceId, Guid mapId, EventInstance eventIndex, Client client, EventPageInstance globalClone) : base(instanceId)
        {
            BaseEvent = myEvent;
            Id = BaseEvent.Id;
            GlobalClone = globalClone;
            MyPage = myPage;
            MapId = mapId;
            X = globalClone.X;
            Y = globalClone.Y;
            Name = myEvent.Name;
            MovementType = globalClone.MovementType;
            MovementFreq = globalClone.MovementFreq;
            MovementSpeed = globalClone.MovementSpeed;
            DisablePreview = globalClone.DisablePreview;
            Trigger = MyPage.Trigger;
            Passable = globalClone.Passable;
            HideName = globalClone.HideName;
            MyEventIndex = eventIndex;
            MoveRoute = globalClone.MoveRoute;
            mPathFinder = new Pathfinder(this);
            SetMovementSpeed(MyPage.Movement.Speed);
            MyGraphic.Type = globalClone.MyGraphic.Type;
            MyGraphic.Filename = globalClone.MyGraphic.Filename;
            MyGraphic.X = globalClone.MyGraphic.X;
            MyGraphic.Y = globalClone.MyGraphic.Y;
            MyGraphic.Width = globalClone.MyGraphic.Width;
            MyGraphic.Height = globalClone.MyGraphic.Height;
            Sprite = MyPage.Graphic.Filename;
            mDirectionFix = MyPage.DirectionFix;
            mWalkingAnim = MyPage.WalkingAnimation;
            mRenderLayer = MyPage.Layer;
            if (globalClone.MyGraphic.Type == EventGraphicType.Sprite)
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
            if (myPage.AnimationId != Guid.Empty)
            {
                Animations.Add(myPage.AnimationId);
            }
            Face = MyPage.FaceGraphic;
            mPageNum = BaseEvent.Pages.IndexOf(MyPage);
            Client = client;
            SendToClient();
        }

        public void SendToClient()
        {
            if (Client != null)
            {
                PacketSender.SendEntityDataTo(Client, this);
            }
            else
            {
                PacketSender.SendEntityDataToProximity(this, null);
            }
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Event;
        }

        public override EntityPacket EntityPacket(EntityPacket packet = null)
        {
            if (packet == null) packet = new EventEntityPacket();
            
            if (GlobalClone != null)
            {
                Sprite = GlobalClone.Sprite;
                Face = GlobalClone.Face;
                Level = GlobalClone.Level;
                X = GlobalClone.X;
                Y = GlobalClone.Y;
                Z = GlobalClone.Z;
                Dir = GlobalClone.Dir;
                Passable = GlobalClone.Passable;
                HideName = GlobalClone.HideName;
            }

            packet = base.EntityPacket(packet);

            var pkt = (EventEntityPacket)packet;
            pkt.HideName = HideName;
            pkt.DirectionFix = mDirectionFix;
            pkt.WalkingAnim = mWalkingAnim;
            pkt.DisablePreview = DisablePreview;
            pkt.Description = MyPage.Description;
            pkt.Graphic = MyGraphic;
            pkt.RenderLayer = (byte) mRenderLayer;
            return pkt;
        }

        //Stats
        public override void SendStatUpdate(int index)
        {
            //do nothing
        }

        public void SetMovementSpeed(EventMovementSpeed speed)
        {
            switch (speed)
            {
                case EventMovementSpeed.Slowest:
                    Stat[(int)Stats.Speed].Stat = 5;
                    break;
                case EventMovementSpeed.Slower:
                    Stat[(int)Stats.Speed].Stat = 10;
                    break;
                case EventMovementSpeed.Normal:
                    Stat[(int)Stats.Speed].Stat = 20;
                    break;
                case EventMovementSpeed.Faster:
                    Stat[(int)Stats.Speed].Stat = 30;
                    break;
                case EventMovementSpeed.Fastest:
                    Stat[(int)Stats.Speed].Stat = 40;
                    break;
            }
        }

        public void Update(bool isActive, long timeMs)
        {
            if (MoveTimer >= Globals.Timing.TimeMs || GlobalClone != null ||  (isActive && MyPage.InteractionFreeze)) return;
            if (MovementType == EventMovementType.MoveRoute && MoveRoute != null)
            {
                ProcessMoveRoute(Client, timeMs);
            }
            else
            {
                if (MovementType == EventMovementType.Random) //Random
                {
                    if (Globals.Rand.Next(0, 2) != 0) return;
                    var dir = (byte)Globals.Rand.Next(0, 4);
                    if (CanMove(dir) == -1)
                    {
                        Move(dir, Client);
                        MoveTimer = Globals.Timing.TimeMs + (long) GetMovementTime();
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void Move(byte moveDir, Client client, bool dontUpdate = false, bool correction = false)
        {
            base.Move(moveDir, client, dontUpdate, correction);

            if (this.Trigger == EventTrigger.PlayerCollide && Passable)
            {
                var players = Map.GetPlayersOnMap();
                foreach (var player in players)
                {
                    if (player.X == X && player.Y == Y && player.Z == Z)
                    {
                        player.HandleEventCollision(this.MyEventIndex, mPageNum);
                    }
                }
            }
        }

        protected override bool ProcessMoveRoute(Client client, long timeMs)
        {
            if (!base.ProcessMoveRoute(client, timeMs))
            {
                var moved = false;
                var shouldSendUpdate = false;
                sbyte lookDir = 0;
                byte moveDir = 0;
                if (MoveRoute.ActionIndex < MoveRoute.Actions.Count)
                {
                    switch (MoveRoute.Actions[MoveRoute.ActionIndex].Type)
                    {
                        case MoveRouteEnum.MoveTowardsPlayer:
                            //Pathfinding required.. this will be weird.
                            if (client != null && GlobalClone == null) //Local Event
                            {
                                if (mPathFinder.GetTarget() == null ||
                                    mPathFinder.GetTarget().TargetMapId != client.Entity.MapId ||
                                    mPathFinder.GetTarget().TargetX != client.Entity.X ||
                                    mPathFinder.GetTarget().TargetY != client.Entity.Y)
                                {
                                    mPathFinder.SetTarget(new PathfinderTarget(client.Entity.MapId,
                                        client.Entity.X, client.Entity.Y, client.Entity.Z));
                                }
                                //Todo check if next to or on top of player.. if so don't run pathfinder.
                                if (mPathFinder.Update(timeMs) == PathfinderResult.Success)
                                {
                                    var pathDir = mPathFinder.GetMove();
                                    if (pathDir > -1)
                                    {
                                        if (CanMove(pathDir) == -1)
                                        {
                                            Move((byte)pathDir, client);
                                            moved = true;
                                        }
                                        else
                                        {
                                            mPathFinder.PathFailed(timeMs);
                                        }
                                    }
                                }
                            }
                            break;
                        case MoveRouteEnum.MoveAwayFromPlayer:
                            //This won't be anything special.
                            if (client != null && GlobalClone == null) //Local Event
                            {
                                moveDir = (byte)GetDirectionTo(client.Entity);
                                if (moveDir > -1)
                                {
                                    switch (moveDir)
                                    {
                                        case (int) Directions.Up:
                                            moveDir = (int) Directions.Down;
                                            break;
                                        case (int) Directions.Down:
                                            moveDir = (int) Directions.Up;
                                            break;
                                        case (int) Directions.Left:
                                            moveDir = (int) Directions.Right;
                                            break;
                                        case (int) Directions.Right:
                                            moveDir = (int) Directions.Left;
                                            break;
                                    }
                                    if (CanMove(moveDir) == -1)
                                    {
                                        Move(moveDir, client);
                                        moved = true;
                                    }
                                    else
                                    {
                                        //Move Randomly
                                        moveDir = (byte)Globals.Rand.Next(0, 4);
                                        if (CanMove(moveDir) == -1)
                                        {
                                            Move(moveDir, client);
                                            moved = true;
                                        }
                                    }
                                }
                                else
                                {
                                    //Move Randomly
                                    moveDir = (byte)Globals.Rand.Next(0, 4);
                                    if (CanMove(moveDir) == -1)
                                    {
                                        Move(moveDir, client);
                                        moved = true;
                                    }
                                }
                            }
                            break;
                        case MoveRouteEnum.FacePlayer:
                            if (client != null && GlobalClone == null) //Local Event
                            {
                                lookDir = GetDirectionTo(client.Entity);
                                if (lookDir > -1)
                                {
                                    ChangeDir((byte)lookDir);
                                    moved = true;
                                }
                            }
                            break;
                        case MoveRouteEnum.FaceAwayFromPlayer:
                            if (client != null && GlobalClone == null) //Local Event
                            {
                                lookDir = GetDirectionTo(client.Entity);
                                if (lookDir > -1)
                                {
                                    switch (lookDir)
                                    {
                                        case (int) Directions.Up:
                                            lookDir = (int) Directions.Down;
                                            break;
                                        case (int) Directions.Down:
                                            lookDir = (int) Directions.Up;
                                            break;
                                        case (int) Directions.Left:
                                            lookDir = (int) Directions.Right;
                                            break;
                                        case (int) Directions.Right:
                                            lookDir = (int) Directions.Left;
                                            break;
                                    }
                                    ChangeDir((byte)lookDir);
                                    moved = true;
                                }
                            }
                            break;
                        case MoveRouteEnum.SetSpeedSlowest:
                            SetMovementSpeed(EventMovementSpeed.Slowest);
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetSpeedSlower:
                            SetMovementSpeed(EventMovementSpeed.Slower);
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetSpeedNormal:
                            SetMovementSpeed(EventMovementSpeed.Normal);
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetSpeedFaster:
                            SetMovementSpeed(EventMovementSpeed.Faster);
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetSpeedFastest:
                            SetMovementSpeed(EventMovementSpeed.Fastest);
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetFreqLowest:
                            MovementFreq = EventMovementFrequency.Lowest;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetFreqLower:
                            MovementFreq = EventMovementFrequency.Lower;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetFreqNormal:
                            MovementFreq = EventMovementFrequency.Normal;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetFreqHigher:
                            MovementFreq = EventMovementFrequency.Higher;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetFreqHighest:
                            MovementFreq = EventMovementFrequency.Highest;
                            moved = true;
                            break;
                        case MoveRouteEnum.WalkingAnimOn:
                            mWalkingAnim = true;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.WalkingAnimOff:
                            mWalkingAnim = false;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.DirectionFixOn:
                            mDirectionFix = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.DirectionFixOff:
                            mDirectionFix = false;
                            moved = true;
                            break;
                        case MoveRouteEnum.WalkthroughOn:
                            Passable = true;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.WalkthroughOff:
                            Passable = false;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.ShowName:
                            HideName = false;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.HideName:
                            HideName = true;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetLevelBelow:
                            mRenderLayer = EventRenderLayer.BelowPlayer;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetLevelNormal:
                            mRenderLayer = EventRenderLayer.SameAsPlayer;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetLevelAbove:
                            mRenderLayer = EventRenderLayer.AbovePlayer;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetGraphic:
                            MyGraphic.Type = MoveRoute.Actions[MoveRoute.ActionIndex].Graphic.Type;
                            MyGraphic.Filename = MoveRoute.Actions[MoveRoute.ActionIndex].Graphic.Filename;
                            MyGraphic.X = MoveRoute.Actions[MoveRoute.ActionIndex].Graphic.X;
                            MyGraphic.Y = MoveRoute.Actions[MoveRoute.ActionIndex].Graphic.Y;
                            MyGraphic.Width = MoveRoute.Actions[MoveRoute.ActionIndex].Graphic.Width;
                            MyGraphic.Height = MoveRoute.Actions[MoveRoute.ActionIndex].Graphic.Height;
                            if (MyGraphic.Type == EventGraphicType.Sprite)
                            {
                                switch (MoveRoute.Actions[MoveRoute.ActionIndex].Graphic.Y)
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
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetAnimation:
                            Animations.Clear();
                            var anim = AnimationBase.Get(MoveRoute.Actions[MoveRoute.ActionIndex].AnimationId);
                            if (anim != null)
                            {
                                Animations.Add(MoveRoute.Actions[MoveRoute.ActionIndex].AnimationId);
                            }
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        default:
                            //Gonna end up returning false because command not found
                            return false;
                    }
                    if (moved || MoveRoute.IgnoreIfBlocked)
                    {
                        MoveRoute.ActionIndex++;
                        if (MoveRoute.ActionIndex >= MoveRoute.Actions.Count)
                        {
                            if (MoveRoute.RepeatRoute) MoveRoute.ActionIndex = 0;
                            MoveRoute.Complete = true;
                        }
                    }
                    if (shouldSendUpdate)
                    {
                        //Send Update
                        SendToClient();
                    }
                    if (MoveTimer < Globals.Timing.TimeMs)
                    {
                        MoveTimer = Globals.Timing.TimeMs + (long) GetMovementTime();
                    }
                }
            }
            return true;
        }

        public override float GetMovementTime()
        {
            switch (MovementFreq)
            {
                case EventMovementFrequency.Lowest:
                    return 4000;
                case EventMovementFrequency.Lower:
                    return 2000;
                case EventMovementFrequency.Normal:
                    return 1000;
                case EventMovementFrequency.Higher:
                    return 500;
                case EventMovementFrequency.Highest:
                    return 250;
                default:
                    return 1000;
            }
        }

        public override int CanMove(int moveDir)
        {
            if (Client == null && mPageNum != 0) return -5;
            switch (moveDir)
            {
                case (int) Directions.Up:
                    if (Y == 0) return -5;
                    break;
                case (int) Directions.Down:
                    if (Y == Options.MapHeight - 1) return -5;
                    break;
                case (int) Directions.Left:
                    if (X == 0) return -5;
                    break;
                case (int) Directions.Right:
                    if (X == Options.MapWidth - 1) return -5;
                    break;
            }
            return base.CanMove(moveDir);
        }

        public void TurnTowardsPlayer()
        {
            sbyte lookDir = -1;
            if (Client != null && GlobalClone == null) //Local Event
            {
                lookDir = GetDirectionTo(Client.Entity);
                if (lookDir > -1)
                {
                    ChangeDir((byte)lookDir);
                }
            }
        }

        public bool ShouldDespawn()
        {
            //Should despawn if conditions are not met OR an earlier page can spawn
            if (!Conditions.MeetsConditionLists(MyPage.ConditionLists, MyEventIndex.MyPlayer, MyEventIndex))
                return true;
            if (Map != null && !Map.GetSurroundingMaps(true).Contains(MyEventIndex.MyPlayer.Map))
                return true;
            for (int i = 0; i < BaseEvent.Pages.Count; i++)
            {
                if (Conditions.CanSpawnPage(BaseEvent.Pages[i],MyEventIndex.MyPlayer,MyEventIndex))
                {
                    if (i > mPageNum) return true;
                }
            }
            if (GlobalClone != null)
            {
                var map = MapInstance.Get(GlobalClone.MapId);
                if (map == null || !map.FindEvent(GlobalClone.BaseEvent, GlobalClone)) return true;
            }
            return false;
        }
    }
}