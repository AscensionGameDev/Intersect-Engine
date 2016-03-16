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
using Intersect_Server.Classes.Entities;
using Intersect_Server.Classes.Misc;

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
        public Entities.EventInstance MyEventIndex;
        public EventPageInstance GlobalClone;
        public EventMoveRoute MoveRoute = null;
        private Pathfinder _pathFinder;
        private int WalkingAnim = 0;
        private int DirectionFix = 0;
        private int RenderLevel = 1;
        public EventPageInstance(EventStruct myEvent, EventPage myPage, int myIndex, int mapNum, Entities.EventInstance eventIndex, Client client) : base(myIndex)
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
            MoveRoute = MyPage.MoveRoute;
            _pathFinder = new Pathfinder(this);
            SetMovementSpeed(MyPage.MovementSpeed);
            MyGraphicType = MyPage.Graphic.Type;
            MySprite = MyPage.Graphic.Filename;
            DirectionFix = MyPage.DirectionFix;
            WalkingAnim = MyPage.WalkingAnimation;
            RenderLevel = MyPage.Layer;
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
        public EventPageInstance(EventStruct myEvent, EventPage myPage, int myIndex, int mapNum, Entities.EventInstance eventIndex, Client client, EventPageInstance globalClone) : base(myIndex)
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
            MoveRoute = globalClone.MoveRoute;
            _pathFinder = new Pathfinder(this);
            SetMovementSpeed(MyPage.MovementSpeed);
            MyGraphicType = MyPage.Graphic.Type;
            MySprite = MyPage.Graphic.Filename;
            DirectionFix = MyPage.DirectionFix;
            WalkingAnim = MyPage.WalkingAnimation;
            RenderLevel = MyPage.Layer;
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

        public void SetMovementSpeed(int speed)
        {
            switch (speed)
            {
                case 0:
                    Stat[2] = 5;
                    break;
                case 1:
                    Stat[2] = 10;
                    break;
                case 2:
                    Stat[2] = 20;
                    break;
                case 3:
                    Stat[2] = 30;
                    break;
                case 4:
                    Stat[2] = 40;
                    break;

            }
        }
        public void Update()
        {
            if (MoveTimer >= Environment.TickCount || GlobalClone != null) return;
            bool moved = false;
            if (MovementType ==2 && MoveRoute != null)
            {
                moved = ProcessMoveRoute();
            }
            else
            {
                if (MovementType == 1) //Random
                {
                    if (Globals.Rand.Next(0, 2) != 0) return;
                    var dir = Globals.Rand.Next(0, 4);
                    if (CanMove(dir) > 0)
                    {
                        Move(dir, Client);
                        moved = true;
                    }
                }
            }

            if (moved)
            {
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

        private bool ProcessMoveRoute()
        {
            var moved = false;
            int lookDir = 0, moveDir = 0;
            if (MoveRoute.ActionIndex < MoveRoute.Actions.Count)
            {
                switch (MoveRoute.Actions[MoveRoute.ActionIndex].Type)
                {
                    case MoveRouteEnum.MoveUp:
                        if (CanMove((int) Enums.Directions.Up) == 0)
                        {
                            Move((int)Enums.Directions.Up,Client);
                            moved = true;
                        }
                        break;
                    case MoveRouteEnum.MoveDown:
                        if (CanMove((int)Enums.Directions.Down) == 0)
                        {
                            Move((int)Enums.Directions.Down, Client);
                            moved = true;
                        }
                        break;
                    case MoveRouteEnum.MoveLeft:
                        if (CanMove((int)Enums.Directions.Left) == 0)
                        {
                            Move((int)Enums.Directions.Left, Client);
                            moved = true;
                        }
                        break;
                    case MoveRouteEnum.MoveRight:
                        if (CanMove((int)Enums.Directions.Right) == 0)
                        {
                            Move((int)Enums.Directions.Right, Client);
                            moved = true;
                        }
                        break;
                    case MoveRouteEnum.MoveRandomly:
                        var dir = Globals.Rand.Next(0, 4);
                        if (CanMove(dir) == 0)
                        {
                            Move(dir, Client);
                            moved = true;
                        }
                        break;
                    case MoveRouteEnum.MoveTowardsPlayer:
                        //Pathfinding required.. this will be weird.
                        if (Client != null && GlobalClone == null) //Local Event
                        {
                            if (_pathFinder.GetTarget() == null || _pathFinder.GetTarget().TargetMap != Client.Entity.CurrentMap || _pathFinder.GetTarget().TargetX != Client.Entity.CurrentX || _pathFinder.GetTarget().TargetY != Client.Entity.CurrentY)
                            {
                                _pathFinder.SetTarget(new PathfinderTarget(Client.Entity.CurrentMap,
                                    Client.Entity.CurrentX, Client.Entity.CurrentY));
                            }
                            else
                            {
                                if (_pathFinder.GetMove() > -1)
                                {
                                    if (CanMove(_pathFinder.GetMove()) == 0)
                                    {
                                        Move(_pathFinder.GetMove(), Client);
                                        _pathFinder.RemoveMove();
                                        moved = true;
                                    }
                                }
                            }
                        }
                        break;
                    case MoveRouteEnum.MoveAwayFromPlayer:
                        //This won't be anything special.
                        if (Client != null && GlobalClone == null) //Local Event
                        {
                            moveDir = GetDirectionTo(Client.Entity);
                            if (moveDir > -1)
                            {
                                switch (moveDir)
                                {
                                    case (int) Enums.Directions.Up:
                                        moveDir = (int) Enums.Directions.Down;
                                        break;
                                    case (int) Enums.Directions.Down:
                                        moveDir = (int) Enums.Directions.Up;
                                        break;
                                    case (int) Enums.Directions.Left:
                                        moveDir = (int) Enums.Directions.Right;
                                        break;
                                    case (int) Enums.Directions.Right:
                                        moveDir = (int) Enums.Directions.Left;
                                        break;
                                }
                                if (CanMove(moveDir) == 0)
                                {
                                    Move(moveDir, Client);
                                    moved = true;
                                }
                                else
                                {
                                    //Move Randomly
                                    moveDir = Globals.Rand.Next(0, 4);
                                    if (CanMove(moveDir) == 0)
                                    {
                                        Move(moveDir, Client);
                                        moved = true;
                                    }
                                }
                            }
                            else
                            {
                                //Move Randomly
                                moveDir = Globals.Rand.Next(0, 4);
                                if (CanMove(moveDir) == 0)
                                {
                                    Move(moveDir, Client);
                                    moved = true;
                                }
                            }
                        }
                        break;
                    case MoveRouteEnum.StepForward:
                        if (CanMove(Dir) > 0)
                        {
                            Move(Dir, Client);
                            moved = true;
                        }
                        break;
                    case MoveRouteEnum.StepBack:
                        switch (Dir)
                        {
                            case (int)Enums.Directions.Up:
                                moveDir = (int) Enums.Directions.Down;
                                break;
                            case (int)Enums.Directions.Down:
                                moveDir = (int)Enums.Directions.Up;
                                break;
                            case (int)Enums.Directions.Left:
                                moveDir = (int)Enums.Directions.Right;
                                break;
                            case (int)Enums.Directions.Right:
                                moveDir = (int)Enums.Directions.Left;
                                break;
                        }
                        if (CanMove(moveDir) > 0)
                        {
                            Move(moveDir, Client);
                            moved = true;
                        }
                        break;
                    case MoveRouteEnum.FaceUp:
                        ChangeDir((int)Enums.Directions.Up);
                        moved = true;
                        break;
                    case MoveRouteEnum.FaceDown:
                        ChangeDir((int)Enums.Directions.Down);
                        moved = true;
                        break;
                    case MoveRouteEnum.FaceLeft:
                        ChangeDir((int)Enums.Directions.Left);
                        moved = true;
                        break;
                    case MoveRouteEnum.FaceRight:
                        ChangeDir((int)Enums.Directions.Right);
                        moved = true;
                        break;
                    case MoveRouteEnum.Turn90Clockwise:
                        switch (Dir)
                        {
                            case (int)Enums.Directions.Up:
                                lookDir = (int)Enums.Directions.Right;
                                break;
                            case (int)Enums.Directions.Down:
                                lookDir = (int)Enums.Directions.Left;
                                break;
                            case (int)Enums.Directions.Left:
                                lookDir = (int)Enums.Directions.Down;
                                break;
                            case (int)Enums.Directions.Right:
                                lookDir = (int)Enums.Directions.Up;
                                break;
                        }
                        ChangeDir(lookDir);
                        moved = true;
                        break;
                    case MoveRouteEnum.Turn90CounterClockwise:
                        switch (Dir)
                        {
                            case (int)Enums.Directions.Up:
                                lookDir = (int)Enums.Directions.Left;
                                break;
                            case (int)Enums.Directions.Down:
                                lookDir = (int)Enums.Directions.Right;
                                break;
                            case (int)Enums.Directions.Left:
                                lookDir = (int)Enums.Directions.Up;
                                break;
                            case (int)Enums.Directions.Right:
                                lookDir = (int)Enums.Directions.Down;
                                break;
                        }
                        ChangeDir(lookDir);
                        moved = true;
                        break;
                    case MoveRouteEnum.Turn180:
                        switch (Dir)
                        {
                            case (int)Enums.Directions.Up:
                                lookDir = (int)Enums.Directions.Down;
                                break;
                            case (int)Enums.Directions.Down:
                                lookDir = (int)Enums.Directions.Up;
                                break;
                            case (int)Enums.Directions.Left:
                                lookDir = (int)Enums.Directions.Right;
                                break;
                            case (int)Enums.Directions.Right:
                                lookDir = (int)Enums.Directions.Left;
                                break;
                        }
                        ChangeDir(lookDir);
                        moved = true;
                        break;
                    case MoveRouteEnum.TurnRandomly:
                        ChangeDir(Globals.Rand.Next(0,4));
                        moved = true;
                        break;
                    case MoveRouteEnum.FacePlayer:
                        if (Client != null && GlobalClone == null) //Local Event
                        {
                            lookDir = GetDirectionTo(Client.Entity);
                            if (lookDir > -1)
                            {
                                ChangeDir(lookDir);
                                moved = true;
                            }
                        }
                        break;
                    case MoveRouteEnum.FaceAwayFromPlayer:
                        if (Client != null && GlobalClone == null) //Local Event
                        {
                            lookDir = GetDirectionTo(Client.Entity);
                            if (lookDir > -1)
                            {
                                switch (lookDir)
                                {
                                    case (int)Enums.Directions.Up:
                                        lookDir = (int)Enums.Directions.Down;
                                        break;
                                    case (int)Enums.Directions.Down:
                                        lookDir = (int)Enums.Directions.Up;
                                        break;
                                    case (int)Enums.Directions.Left:
                                        lookDir = (int)Enums.Directions.Right;
                                        break;
                                    case (int)Enums.Directions.Right:
                                        lookDir = (int)Enums.Directions.Left;
                                        break;
                                }
                                ChangeDir(lookDir);
                                moved = true;
                            }
                        }
                        break;
                    case MoveRouteEnum.SetSpeedSlowest:
                        SetMovementSpeed(0);
                        break;
                    case MoveRouteEnum.SetSpeedSlower:
                        SetMovementSpeed(1);
                        break;
                    case MoveRouteEnum.SetSpeedNormal:
                        SetMovementSpeed(2);
                        break;
                    case MoveRouteEnum.SetSpeedFaster:
                        SetMovementSpeed(3);
                        break;
                    case MoveRouteEnum.SetSpeedFastest:
                        SetMovementSpeed(4);
                        break;
                    case MoveRouteEnum.SetFreqLowest:
                        MovementFreq = 0;
                        break;
                    case MoveRouteEnum.SetFreqLower:
                        MovementFreq = 1;
                        break;
                    case MoveRouteEnum.SetFreqNormal:
                        MovementFreq = 2;
                        break;
                    case MoveRouteEnum.SetFreqHigher:
                        MovementFreq = 3;
                        break;
                    case MoveRouteEnum.SetFreqHighest:
                        MovementFreq = 4;
                        break;
                    case MoveRouteEnum.WalkingAnimOn:
                        WalkingAnim = 1;
                        break;
                    case MoveRouteEnum.WalkingAnimOff:
                        WalkingAnim = 0;
                        break;
                    case MoveRouteEnum.DirectionFixOn:
                        DirectionFix = 1;
                        break;
                    case MoveRouteEnum.DirectionFixOff:
                        DirectionFix = 0;
                        break;
                    case MoveRouteEnum.WalkthroughOn:
                        Passable = 1;
                        break;
                    case MoveRouteEnum.WalkthroughOff:
                        Passable = 0;
                        break;
                    case MoveRouteEnum.ShowName:
                        HideName = 0;
                        break;
                    case MoveRouteEnum.HideName:
                        HideName = 1;
                        break;
                    case MoveRouteEnum.SetLevelBelow:
                        RenderLevel = 0;
                        break;
                    case MoveRouteEnum.SetLevelNormal:
                        RenderLevel = 1;
                        break;
                    case MoveRouteEnum.SetLevelAbove:
                        RenderLevel = 2;
                        break;
                    case MoveRouteEnum.Wait100:
                        break;
                    case MoveRouteEnum.Wait500:
                        break;
                    case MoveRouteEnum.Wait1000:
                        break;
                    case MoveRouteEnum.SetGraphic:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (moved || MoveRoute.IgnoreIfBlocked)
                {
                    MoveRoute.ActionIndex++;
                    if (MoveRoute.ActionIndex >= MoveRoute.Actions.Count && MoveRoute.RepeatRoute)
                    {
                        MoveRoute.ActionIndex = 0;
                    }
                }
            }
            return moved;
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
}
