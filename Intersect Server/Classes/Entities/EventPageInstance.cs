using Intersect;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Maps;
using Intersect_Server.Classes.Misc;
using Intersect_Server.Classes.Misc.Pathfinding;
using Intersect_Server.Classes.Networking;

namespace Intersect_Server.Classes.Entities
{
    public class EventPageInstance : Entity
    {
        private Pathfinder _pathFinder;
        public EventBase BaseEvent;
        public Client Client;
        private int DirectionFix;
        public int DisablePreview;
        public EventPageInstance GlobalClone;
        public int MovementFreq;
        public int MovementSpeed;
        public int MovementType;
        public EventInstance MyEventIndex;
        public EventGraphic MyGraphic = new EventGraphic();
        public EventPage MyPage;
        private int PageNum;
        private int RenderLevel = 1;
        public int Trigger;
        private int WalkingAnim;
        public string Param;

        public EventPageInstance(EventBase myEvent, EventPage myPage, int myIndex, int mapNum, EventInstance eventIndex,
            Client client) : base(myIndex)
        {
            BaseEvent = myEvent;
            MyPage = myPage;
            CurrentMap = mapNum;
            CurrentX = eventIndex.CurrentX;
            CurrentY = eventIndex.CurrentY;
            MyName = myEvent.Name;
            MovementType = MyPage.MovementType;
            MovementFreq = MyPage.MovementFreq;
            MovementSpeed = MyPage.MovementSpeed;
            DisablePreview = MyPage.DisablePreview;
            Trigger = MyPage.Trigger;
            Passable = MyPage.Passable;
            HideName = MyPage.HideName;
            MyEventIndex = eventIndex;
            MoveRoute = new EventMoveRoute();
            MoveRoute.CopyFrom(MyPage.MoveRoute);
            _pathFinder = new Pathfinder(this);
            SetMovementSpeed(MyPage.MovementSpeed);
            MyGraphic.Type = MyPage.Graphic.Type;
            MyGraphic.Filename = MyPage.Graphic.Filename;
            MyGraphic.X = MyPage.Graphic.X;
            MyGraphic.Y = MyPage.Graphic.Y;
            MyGraphic.Width = MyPage.Graphic.Width;
            MyGraphic.Height = MyPage.Graphic.Height;
            MySprite = MyPage.Graphic.Filename;
            DirectionFix = MyPage.DirectionFix;
            WalkingAnim = MyPage.WalkingAnimation;
            RenderLevel = MyPage.Layer;
            if (MyGraphic.Type == 1)
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
            if (myPage.Animation > -1)
            {
                Animations.Add(myPage.Animation);
            }
            Face = MyPage.FaceGraphic;
            PageNum = BaseEvent.MyPages.IndexOf(MyPage);
            Client = client;
            SendToClient();
        }

        public EventPageInstance(EventBase myEvent, EventPage myPage, int myIndex, int mapNum, EventInstance eventIndex,
            Client client, EventPageInstance globalClone) : base(myIndex)
        {
            BaseEvent = myEvent;
            GlobalClone = globalClone;
            MyPage = myPage;
            CurrentMap = mapNum;
            CurrentX = globalClone.CurrentX;
            CurrentY = globalClone.CurrentY;
            MyName = myEvent.Name;
            MovementType = globalClone.MovementType;
            MovementFreq = globalClone.MovementFreq;
            MovementSpeed = globalClone.MovementSpeed;
            DisablePreview = globalClone.DisablePreview;
            Trigger = MyPage.Trigger;
            Passable = globalClone.Passable;
            HideName = globalClone.HideName;
            CurrentMap = mapNum;
            MyEventIndex = eventIndex;
            MoveRoute = globalClone.MoveRoute;
            _pathFinder = new Pathfinder(this);
            SetMovementSpeed(MyPage.MovementSpeed);
            MyGraphic.Type = globalClone.MyGraphic.Type;
            MyGraphic.Filename = globalClone.MyGraphic.Filename;
            MyGraphic.X = globalClone.MyGraphic.X;
            MyGraphic.Y = globalClone.MyGraphic.Y;
            MyGraphic.Width = globalClone.MyGraphic.Width;
            MyGraphic.Height = globalClone.MyGraphic.Height;
            MySprite = MyPage.Graphic.Filename;
            DirectionFix = MyPage.DirectionFix;
            WalkingAnim = MyPage.WalkingAnimation;
            RenderLevel = MyPage.Layer;
            if (globalClone.MyGraphic.Type == 1)
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
            PageNum = BaseEvent.MyPages.IndexOf(MyPage);
            Client = client;
            SendToClient();
        }

        public void SendToClient()
        {
            if (Client != null)
            {
                PacketSender.SendEntityDataTo(Client, this);
            }
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Event;
        }

        public override byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(base.Data());
            bf.WriteInteger(HideName);
            bf.WriteInteger(DirectionFix);
            bf.WriteInteger(WalkingAnim);
            bf.WriteInteger(DisablePreview);
            bf.WriteString(MyPage.Desc);
            bf.WriteInteger(MyGraphic.Type);
            bf.WriteString(MyGraphic.Filename);
            bf.WriteInteger(MyGraphic.X);
            bf.WriteInteger(MyGraphic.Y);
            bf.WriteInteger(MyGraphic.Width);
            bf.WriteInteger(MyGraphic.Height);
            bf.WriteInteger(RenderLevel);
            return bf.ToArray();
        }

        //Stats
        public override void SendStatUpdate(int index)
        {
            //do nothing
        }

        public void SetMovementSpeed(int speed)
        {
            switch (speed)
            {
                case 0:
                    Stat[2].Stat = 5;
                    break;
                case 1:
                    Stat[2].Stat = 10;
                    break;
                case 2:
                    Stat[2].Stat = 20;
                    break;
                case 3:
                    Stat[2].Stat = 30;
                    break;
                case 4:
                    Stat[2].Stat = 40;
                    break;
            }
        }

        public void Update(bool isActive)
        {
            if (MoveTimer >= Globals.System.GetTimeMs() || GlobalClone != null ||
                (isActive && MyPage.InteractionFreeze == 1)) return;
            if (MovementType == 2 && MoveRoute != null)
            {
                ProcessMoveRoute(Client);
            }
            else
            {
                if (MovementType == 1) //Random
                {
                    if (Globals.Rand.Next(0, 2) != 0) return;
                    var dir = Globals.Rand.Next(0, 4);
                    if (CanMove(dir) == -1)
                    {
                        Move(dir, Client);
                        MoveTimer = Globals.System.GetTimeMs() + (long) GetMovementTime();
                    }
                }
            }
        }

        protected override bool ProcessMoveRoute(Client client)
        {
            if (!base.ProcessMoveRoute(client))
            {
                var moved = false;
                var shouldSendUpdate = false;
                int lookDir = 0, moveDir = 0;
                if (MoveRoute.ActionIndex < MoveRoute.Actions.Count)
                {
                    switch (MoveRoute.Actions[MoveRoute.ActionIndex].Type)
                    {
                        case MoveRouteEnum.MoveTowardsPlayer:
                            //Pathfinding required.. this will be weird.
                            if (client != null && GlobalClone == null) //Local Event
                            {
                                if (_pathFinder.GetTarget() == null ||
                                    _pathFinder.GetTarget().TargetMap != client.Entity.CurrentMap ||
                                    _pathFinder.GetTarget().TargetX != client.Entity.CurrentX ||
                                    _pathFinder.GetTarget().TargetY != client.Entity.CurrentY)
                                {
                                    _pathFinder.SetTarget(new PathfinderTarget(client.Entity.CurrentMap,
                                        client.Entity.CurrentX, client.Entity.CurrentY));
                                }
                                else
                                {
                                    if (_pathFinder.GetMove() > -1)
                                    {
                                        if (CanMove(_pathFinder.GetMove()) == -1)
                                        {
                                            Move(_pathFinder.GetMove(), client);
                                            _pathFinder.RemoveMove();
                                            moved = true;
                                        }
                                    }
                                }
                            }
                            break;
                        case MoveRouteEnum.MoveAwayFromPlayer:
                            //This won't be anything special.
                            if (client != null && GlobalClone == null) //Local Event
                            {
                                moveDir = GetDirectionTo(client.Entity);
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
                                        moveDir = Globals.Rand.Next(0, 4);
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
                                    moveDir = Globals.Rand.Next(0, 4);
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
                                    ChangeDir(lookDir);
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
                                    ChangeDir(lookDir);
                                    moved = true;
                                }
                            }
                            break;
                        case MoveRouteEnum.SetSpeedSlowest:
                            SetMovementSpeed(0);
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetSpeedSlower:
                            SetMovementSpeed(1);
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetSpeedNormal:
                            SetMovementSpeed(2);
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetSpeedFaster:
                            SetMovementSpeed(3);
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetSpeedFastest:
                            SetMovementSpeed(4);
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetFreqLowest:
                            MovementFreq = 0;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetFreqLower:
                            MovementFreq = 1;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetFreqNormal:
                            MovementFreq = 2;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetFreqHigher:
                            MovementFreq = 3;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetFreqHighest:
                            MovementFreq = 4;
                            moved = true;
                            break;
                        case MoveRouteEnum.WalkingAnimOn:
                            WalkingAnim = 1;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.WalkingAnimOff:
                            WalkingAnim = 0;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.DirectionFixOn:
                            DirectionFix = 1;
                            moved = true;
                            break;
                        case MoveRouteEnum.DirectionFixOff:
                            DirectionFix = 0;
                            moved = true;
                            break;
                        case MoveRouteEnum.WalkthroughOn:
                            Passable = 1;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.WalkthroughOff:
                            Passable = 0;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.ShowName:
                            HideName = 0;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.HideName:
                            HideName = 1;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetLevelBelow:
                            RenderLevel = 0;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetLevelNormal:
                            RenderLevel = 1;
                            shouldSendUpdate = true;
                            moved = true;
                            break;
                        case MoveRouteEnum.SetLevelAbove:
                            RenderLevel = 2;
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
                            if (MyGraphic.Type == 1)
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
                            var anim = AnimationBase.Lookup.Get(MoveRoute.Actions[MoveRoute.ActionIndex].AnimationIndex);
                            if (anim != null)
                            {
                                Animations.Add(MoveRoute.Actions[MoveRoute.ActionIndex].AnimationIndex);
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
                        SpawnTime = Globals.System.GetTimeMs();
                        SendToClient();
                    }
                    if (MoveTimer < Globals.System.GetTimeMs())
                    {
                        MoveTimer = Globals.System.GetTimeMs() + (long) GetMovementTime();
                    }
                }
            }
            return true;
        }

        public override float GetMovementTime()
        {
            switch (MovementFreq)
            {
                case 0:
                    return 4000;
                case 1:
                    return 2000;
                case 2:
                    return 1000;
                case 3:
                    return 500;
                case 4:
                    return 250;
                default:
                    return 1000;
            }
        }

        public override int CanMove(int moveDir)
        {
            switch (moveDir)
            {
                case (int) Directions.Up:
                    if (CurrentY == 0) return -5;
                    break;
                case (int) Directions.Down:
                    if (CurrentY == Options.MapHeight - 1) return -5;
                    break;
                case (int) Directions.Left:
                    if (CurrentX == 0) return -5;
                    break;
                case (int) Directions.Right:
                    if (CurrentX == Options.MapWidth - 1) return -5;
                    break;
            }
            return base.CanMove(moveDir);
        }

        public void TurnTowardsPlayer()
        {
            int lookDir = -1;
            if (Client != null && GlobalClone == null) //Local Event
            {
                lookDir = GetDirectionTo(Client.Entity);
                if (lookDir > -1)
                {
                    ChangeDir(lookDir);
                }
            }
        }

        public bool ShouldDespawn()
        {
            //Should despawn if conditions are not met OR an earlier page can spawn
            if (!EventInstance.MeetsConditionLists(MyPage.ConditionLists, MyEventIndex.MyPlayer, MyEventIndex))
                return true;
            for (int i = 0; i < BaseEvent.MyPages.Count; i++)
            {
                if (MyEventIndex.CanSpawnPage(i, BaseEvent))
                {
                    if (i > PageNum) return true;
                }
            }
            if (GlobalClone != null)
            {
                var map = MapInstance.GetMap(GlobalClone.CurrentMap);
                if (map == null || !map.FindEvent(GlobalClone.BaseEvent, GlobalClone)) return true;
            }
            return false;
        }
    }
}