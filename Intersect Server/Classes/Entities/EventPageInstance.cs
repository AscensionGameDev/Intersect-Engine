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
}
