using System;
using System.Linq;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Network.Packets.Server;
using Intersect.Server.Entities.Pathfinding;
using Intersect.Server.General;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;

namespace Intersect.Server.Entities.Events
{

    public partial class EventPageInstance : Entity
    {

        public EventBase BaseEvent;

        public bool DisablePreview;

        public EventPageInstance GlobalClone;

        private bool mDirectionFix;

        private EventMovementSpeed mMovementSpeed;

        public EventMovementFrequency MovementFreq;

        public EventMovementType MovementType;

        private int mPageNum;

        private Pathfinder mPathFinder;

        private EventRenderLayer mRenderLayer = EventRenderLayer.SameAsPlayer;

        private bool mWalkingAnim;

        public Event MyEventIndex;

        public EventGraphic MyGraphic = new EventGraphic();

        public EventPage MyPage;

        public string Param;

        public Player Player;

        public EventTrigger Trigger;

        public int Speed = 20;

        public EventPageInstance(
            EventBase myEvent,
            EventPage myPage,
            Guid mapId,
            Event eventIndex,
            Player player
        ) : base(Guid.NewGuid())
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
            Player = player;
            SendToPlayer();
        }

        public EventPageInstance(
            EventBase myEvent,
            EventPage myPage,
            Guid instanceId,
            Guid mapId,
            Event eventIndex,
            Player player,
            EventPageInstance globalClone
        ) : base(instanceId)
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
            Player = player;
            SendToPlayer();
        }

        public EventMovementSpeed MovementSpeed
        {
            get => mMovementSpeed;
            set
            {
                mMovementSpeed = value;
                switch (mMovementSpeed)
                {
                    case EventMovementSpeed.Slowest:
                        Speed = 2;

                        break;
                    case EventMovementSpeed.Slower:
                        Speed = 5;

                        break;
                    case EventMovementSpeed.Normal:
                        Speed = 20;

                        break;
                    case EventMovementSpeed.Faster:
                        Speed = 30;

                        break;
                    case EventMovementSpeed.Fastest:
                        Speed = 40;

                        break;
                }
            }
        }

        public void SendToPlayer()
        {
            if (Player != null)
            {
                PacketSender.SendEntityDataTo(Player, this);
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

        public override EntityPacket EntityPacket(EntityPacket packet = null, Player forPlayer = null)
        {
            if (packet == null)
            {
                packet = new EventEntityPacket();
            }

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

            packet = base.EntityPacket(packet, forPlayer);

            var pkt = (EventEntityPacket) packet;
            pkt.HideName = HideName;
            pkt.DirectionFix = mDirectionFix;
            pkt.WalkingAnim = mWalkingAnim;
            pkt.DisablePreview = DisablePreview;
            pkt.Description = MyPage.Description;
            pkt.Graphic = MyGraphic;
            pkt.RenderLayer = (byte) mRenderLayer;

            return pkt;
        }

        public void SetMovementSpeed(EventMovementSpeed speed)
        {
            switch (speed)
            {
                case EventMovementSpeed.Slowest:
                    Speed = 5;

                    break;
                case EventMovementSpeed.Slower:
                    Speed = 10;

                    break;
                case EventMovementSpeed.Normal:
                    Speed = 20;

                    break;
                case EventMovementSpeed.Faster:
                    Speed = 30;

                    break;
                case EventMovementSpeed.Fastest:
                    Speed = 40;

                    break;
            }
        }

        public override int[] GetStatValues()
        {
            var stats = new int[(int)Stats.StatCount];
            stats[(int)Stats.Speed] = Speed;
            return stats;
        }

        public void Update(bool isActive, long timeMs)
        {
            if (MoveTimer >= Globals.Timing.Milliseconds || GlobalClone != null || isActive && MyPage.InteractionFreeze)
            {
                return;
            }

            if (MovementType == EventMovementType.MoveRoute && MoveRoute != null)
            {
                ProcessMoveRoute(Player, timeMs);
            }
            else
            {
                if (MovementType == EventMovementType.Random) //Random
                {
                    if (Randomization.Next(0, 2) != 0)
                    {
                        return;
                    }

                    var dir = (byte)Randomization.Next(0, 4);
                    if (CanMove(dir) == -1)
                    {
                        Move(dir, Player);
                    }
                    MoveTimer = Globals.Timing.Milliseconds + (long)GetMovementTime();
                }
            }
        }

        /// <inheritdoc />
        public override void Move(int moveDir, Player forPlayer, bool doNotUpdate = false, bool correction = false)
        {
            base.Move(moveDir, forPlayer, doNotUpdate, correction);

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

        protected override bool ProcessMoveRoute(Player forPlayer, long timeMs)
        {
            if (!base.ProcessMoveRoute(forPlayer, timeMs))
            {
                var moved = false;
                var shouldSendUpdate = false;
                int lookDir;
                int moveDir;
                if (MoveRoute.ActionIndex < MoveRoute.Actions.Count)
                {
                    switch (MoveRoute.Actions[MoveRoute.ActionIndex].Type)
                    {
                        case MoveRouteEnum.MoveTowardsPlayer:
                            //Pathfinding required.. this will be weird.
                            if (forPlayer != null && GlobalClone == null) //Local Event
                            {
                                if (mPathFinder.GetTarget() == null ||
                                    mPathFinder.GetTarget().TargetMapId != forPlayer.MapId ||
                                    mPathFinder.GetTarget().TargetX != forPlayer.X ||
                                    mPathFinder.GetTarget().TargetY != forPlayer.Y)
                                {
                                    mPathFinder.SetTarget(
                                        new PathfinderTarget(forPlayer.MapId, forPlayer.X, forPlayer.Y, forPlayer.Z)
                                    );
                                }

                                //Todo check if next to or on top of player.. if so don't run pathfinder.
                                if (mPathFinder.Update(timeMs) == PathfinderResult.Success)
                                {
                                    var pathDir = mPathFinder.GetMove();
                                    if (pathDir > -1)
                                    {
                                        if (CanMove(pathDir) == -1)
                                        {
                                            Move((byte) pathDir, forPlayer);
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
                            if (forPlayer != null && GlobalClone == null) //Local Event
                            {
                                moveDir = (byte) GetDirectionTo(forPlayer);
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
                                        Move(moveDir, forPlayer);
                                        moved = true;
                                    }
                                    else
                                    {
                                        //Move Randomly
                                        moveDir = (byte)Randomization.Next(0, 4);
                                        if (CanMove(moveDir) == -1)
                                        {
                                            Move(moveDir, forPlayer);
                                            moved = true;
                                        }
                                    }
                                }
                                else
                                {
                                    //Move Randomly
                                    moveDir = (byte)Randomization.Next(0, 4);
                                    if (CanMove(moveDir) == -1)
                                    {
                                        Move(moveDir, forPlayer);
                                        moved = true;
                                    }
                                }
                            }

                            break;
                        case MoveRouteEnum.FacePlayer:
                            if (forPlayer != null && GlobalClone == null) //Local Event
                            {
                                lookDir = GetDirectionTo(forPlayer);
                                if (lookDir > -1)
                                {
                                    ChangeDir((byte) lookDir);
                                    moved = true;
                                }
                            }

                            break;
                        case MoveRouteEnum.FaceAwayFromPlayer:
                            if (forPlayer != null && GlobalClone == null) //Local Event
                            {
                                lookDir = GetDirectionTo(forPlayer);
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

                                    ChangeDir((byte) lookDir);
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
                            if (MoveRoute.RepeatRoute)
                            {
                                MoveRoute.ActionIndex = 0;
                            }

                            MoveRoute.Complete = true;
                        }
                    }

                    if (shouldSendUpdate)
                    {
                        //Send Update
                        SendToPlayer();
                    }

                    if (MoveTimer < Globals.Timing.Milliseconds)
                    {
                        MoveTimer = Globals.Timing.Milliseconds + (long) GetMovementTime();
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
            if (Player == null && mPageNum != 0)
            {
                return -5;
            }

            switch (moveDir)
            {
                case (int) Directions.Up:
                    if (Y == 0)
                    {
                        return -5;
                    }

                    break;
                case (int) Directions.Down:
                    if (Y == Options.MapHeight - 1)
                    {
                        return -5;
                    }

                    break;
                case (int) Directions.Left:
                    if (X == 0)
                    {
                        return -5;
                    }

                    break;
                case (int) Directions.Right:
                    if (X == Options.MapWidth - 1)
                    {
                        return -5;
                    }

                    break;
            }

            return base.CanMove(moveDir);
        }

        public void TurnTowardsPlayer()
        {
            int lookDir;
            if (Player != null && GlobalClone == null) //Local Event
            {
                lookDir = GetDirectionTo(Player);
                if (lookDir > -1)
                {
                    ChangeDir(lookDir);
                }
            }
        }

        public bool ShouldDespawn(MapInstance map)
        {
            //Should despawn if conditions are not met OR an earlier page can spawn
            if (!Conditions.MeetsConditionLists(MyPage.ConditionLists, MyEventIndex.Player, MyEventIndex))
            {
                return true;
            }

            if (map != null && !map.GetSurroundingMapIds(true).Contains(MyEventIndex.Player.MapId))
            {
                return true;
            }

            for (var i = 0; i < BaseEvent.Pages.Count; i++)
            {
                if (i != mPageNum && Conditions.CanSpawnPage(BaseEvent.Pages[i], MyEventIndex.Player, MyEventIndex))
                {
                    if (i > mPageNum)
                    {
                        return true;
                    }
                }
            }

            if (GlobalClone != null)
            {
                //Removing this line because the global clone MUST be on the same map and its hindering performance.
                //var map = MapInstance.Get(GlobalClone.MapId);
                if (map == null || !map.FindEvent(GlobalClone.BaseEvent, GlobalClone))
                {
                    return true;
                }
            }

            return false;
        }

    }

}
