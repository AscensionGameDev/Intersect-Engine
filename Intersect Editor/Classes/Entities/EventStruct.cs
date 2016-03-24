/*
    Intersect Game Engine (Editor)
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

namespace Intersect_Editor.Classes
{
    public class EventStruct
    {
        public string MyName { get; set; }
        public int MyIndex { get; set; }
        public int SpawnX { get; set; }
        public int SpawnY { get; set; }
        public byte IsGlobal { get; set; }
        public int Deleted { get; set; }
        public bool CommonEvent { get; set; }
        public List<EventPage> MyPages { get; set; }

        public EventStruct(int index, int x, int y, bool isCommon = false, byte isGlobal = 0)
        {
            MyName = "New Event";
            MyIndex = index;
            SpawnX = x;
            SpawnY = y;
            IsGlobal = isGlobal;
            CommonEvent = isCommon;
            MyPages = new List<EventPage>();
            MyPages.Add(new EventPage());
        }

        public EventStruct(int index, EventStruct copy)
        {
            MyName = "New Event";
            MyPages = new List<EventPage>();
            ByteBuffer myBuffer = new ByteBuffer();
            MyIndex = index;
            myBuffer.WriteBytes(copy.EventData());
            Deleted = myBuffer.ReadInteger();
            if (Deleted != 1)
            {
                MyName = myBuffer.ReadString();
                SpawnX = myBuffer.ReadInteger();
                SpawnY = myBuffer.ReadInteger();
                IsGlobal = myBuffer.ReadByte();
                int pageCount = myBuffer.ReadInteger();
                CommonEvent = copy.CommonEvent;
                for (var i = 0; i < pageCount; i++)
                {
                    MyPages.Add(new EventPage(myBuffer));
                }
            }
        }
        public EventStruct(int index, ByteBuffer myBuffer, bool isCommon = false)
        {
            MyName = "New Event";
            MyPages = new List<EventPage>();
            MyIndex = index;
            Deleted = myBuffer.ReadInteger();
            if (Deleted != 1)
            {
                MyName = myBuffer.ReadString();
                SpawnX = myBuffer.ReadInteger();
                SpawnY = myBuffer.ReadInteger();
                IsGlobal = myBuffer.ReadByte();
                int pageCount = myBuffer.ReadInteger();
                CommonEvent = isCommon;
                for (var i = 0; i < pageCount; i++)
                {
                    MyPages.Add(new EventPage(myBuffer));
                }
            }
        }
        public byte[] EventData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(Deleted);
            if (Deleted == 0)
            {
                myBuffer.WriteString(MyName);
                myBuffer.WriteInteger(SpawnX);
                myBuffer.WriteInteger(SpawnY);
                myBuffer.WriteByte(IsGlobal);
                myBuffer.WriteInteger(MyPages.Count);
                for (var i = 0; i < MyPages.Count; i++)
                {
                    MyPages[i].WriteBytes(myBuffer);
                }
            }
            return myBuffer.ToArray();
        }
    }

    public class EventPage
    {
        public string Desc = "";
        public int MovementType;
        public int MovementSpeed;
        public int MovementFreq;
        public EventMoveRoute MoveRoute = new EventMoveRoute();
        public int Passable;
        public int Layer;
        public int Trigger;
        public string FaceGraphic = "";
        public EventGraphic Graphic = new EventGraphic();
        public int HideName;
        public int DisablePreview = 1;
        public int DirectionFix;
        public int WalkingAnimation = 1;
        public int Animation = -1;
        public List<CommandList> CommandLists = new List<CommandList>();
        public List<EventCommand> Conditions = new List<EventCommand>(); 

        public EventPage()
        {
            MovementType = 0;
            MovementSpeed = 2;
            MovementFreq = 2;
            Passable = 0;
            Layer = 1;
            Trigger = 0;
            HideName = 0;
            CommandLists.Add(new CommandList());
        }

        public EventPage(ByteBuffer curBuffer)
        {
            Desc = curBuffer.ReadString();
            MovementType = curBuffer.ReadInteger();
            if (MovementType == 2) MoveRoute.Load(curBuffer);
            MovementSpeed = curBuffer.ReadInteger();
            MovementFreq = curBuffer.ReadInteger();
            Passable = curBuffer.ReadInteger();
            Layer = curBuffer.ReadInteger();
            Trigger = curBuffer.ReadInteger();
            FaceGraphic = curBuffer.ReadString();
            Graphic.Load(curBuffer);
            HideName = curBuffer.ReadInteger();
            DisablePreview = curBuffer.ReadInteger();
            DirectionFix = curBuffer.ReadInteger();
            WalkingAnimation = curBuffer.ReadInteger();
            Animation = curBuffer.ReadInteger();
            var x = curBuffer.ReadInteger();
            for (var i = 0; i < x; i++)
            {
                CommandLists.Add(new CommandList(curBuffer));
            }
            x = curBuffer.ReadInteger();
            for (var i = 0; i < x; i++)
            {
                Conditions.Add(new EventCommand());
                Conditions[i].Load(curBuffer);
            }
        }

        public void WriteBytes(ByteBuffer myBuffer)
        {
            myBuffer.WriteString(Desc);
            myBuffer.WriteInteger(MovementType);
            if (MovementType == 2) MoveRoute.Save(myBuffer);
            myBuffer.WriteInteger(MovementSpeed);
            myBuffer.WriteInteger(MovementFreq);
            myBuffer.WriteInteger(Passable);
            myBuffer.WriteInteger(Layer);
            myBuffer.WriteInteger(Trigger);
            myBuffer.WriteString(FaceGraphic);
            Graphic.Save(myBuffer);
            myBuffer.WriteInteger(HideName);
            myBuffer.WriteInteger(DisablePreview);
            myBuffer.WriteInteger(DirectionFix);
            myBuffer.WriteInteger(WalkingAnimation);
            myBuffer.WriteInteger(Animation);
            myBuffer.WriteInteger(CommandLists.Count);
            foreach (var commandList in CommandLists)
            {
                commandList.WriteBytes(myBuffer);
            }
            myBuffer.WriteInteger(Conditions.Count);
            foreach (var condition in Conditions)
            {
                condition.Save(myBuffer);
            }
        }
    }

    public class EventMoveRoute
    {
        public int Target = -1;
        public bool RepeatRoute = false;
        public bool IgnoreIfBlocked = false;
        public List<MoveRouteAction> Actions = new List<MoveRouteAction>();

        public EventMoveRoute()
        {
        }

        public void Load(ByteBuffer myBuffer)
        {
            Target = myBuffer.ReadInteger();
            if (myBuffer.ReadByte() == 1)
            {
                IgnoreIfBlocked = true;
            }
            else
            {
                IgnoreIfBlocked = false;
            }
            if (myBuffer.ReadByte() == 1)
            {
                RepeatRoute = true;
            }
            else
            {
                RepeatRoute = false;
            }
            int actionCount = myBuffer.ReadInteger();
            for (int i = 0; i < actionCount; i++)
            {
                Actions.Add(new MoveRouteAction());
                Actions[Actions.Count - 1].Load(myBuffer);
            }
        }

        public void Save(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger(Target);
            if (IgnoreIfBlocked)
            {
                myBuffer.WriteByte(1);
            }
            else
            {
                myBuffer.WriteByte(0);
            }
            if (RepeatRoute)
            {
                myBuffer.WriteByte(1);
            }
            else
            {
                myBuffer.WriteByte(0);
            }
            myBuffer.WriteInteger(Actions.Count);
            foreach (MoveRouteAction action in Actions)
            {
                action.Save(myBuffer);
            }
        }

        public void CopyFrom(EventMoveRoute route)
        {
            Target = route.Target;
            IgnoreIfBlocked = route.IgnoreIfBlocked;
            RepeatRoute = route.RepeatRoute;
            Actions.Clear();
            foreach (MoveRouteAction action in route.Actions)
            {
                Actions.Add(action.Copy());
            }
        }
    }

    public enum MoveRouteEnum
    {
        MoveUp = 1,
        MoveDown,
        MoveLeft,
        MoveRight,
        MoveRandomly,
        MoveTowardsPlayer,
        MoveAwayFromPlayer,
        StepForward,
        StepBack,
        FaceUp,
        FaceDown,
        FaceLeft,
        FaceRight,
        Turn90Clockwise,
        Turn90CounterClockwise,
        Turn180,
        TurnRandomly,
        FacePlayer,
        FaceAwayFromPlayer,
        SetSpeedSlowest,
        SetSpeedSlower,
        SetSpeedNormal,
        SetSpeedFaster,
        SetSpeedFastest,
        SetFreqLowest,
        SetFreqLower,
        SetFreqNormal,
        SetFreqHigher,
        SetFreqHighest,
        WalkingAnimOn,
        WalkingAnimOff,
        DirectionFixOn,
        DirectionFixOff,
        WalkthroughOn,
        WalkthroughOff,
        ShowName,
        HideName,
        SetLevelBelow,
        SetLevelNormal,
        SetLevelAbove,
        Wait100,
        Wait500,
        Wait1000,
        SetGraphic,
        SetAnimation,
    }

    public class MoveRouteAction
    {
        public MoveRouteEnum Type;
        public EventGraphic Graphic = null;
        public int AnimationIndex = -1;


        public void Save(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger((int)Type);
            if (Type == MoveRouteEnum.SetGraphic)
            {
                Graphic.Save(myBuffer);
            }
            else if (Type == MoveRouteEnum.SetAnimation)
            {
                myBuffer.WriteInteger(AnimationIndex);
            }
        }

        public void Load(ByteBuffer myBuffer)
        {
            Type = (MoveRouteEnum)myBuffer.ReadInteger();
            if (Type == MoveRouteEnum.SetGraphic)
            {
                Graphic = new EventGraphic();
                Graphic.Load(myBuffer);
            }
            else if (Type == MoveRouteEnum.SetAnimation)
            {
                AnimationIndex = myBuffer.ReadInteger();
            }
        }

        public MoveRouteAction Copy()
        {
            MoveRouteAction copy = new MoveRouteAction();
            copy.Type = Type;
            if (Type == MoveRouteEnum.SetGraphic)
            {
                copy.Graphic = new EventGraphic();
                copy.Graphic.CopyFrom(Graphic);
            }
            else if (Type == MoveRouteEnum.SetAnimation)
            {
                copy.AnimationIndex = AnimationIndex;
            }
            return copy;
        }
    }

    public class CommandList
    {
        public List<EventCommand> Commands = new List<EventCommand>();

        public CommandList()
        {

        }

        public CommandList(ByteBuffer myBuffer)
        {
            var y = myBuffer.ReadInteger();
            for (var i = 0; i < y; i++)
            {
                Commands.Add(new EventCommand());
                Commands[i].Load(myBuffer);

            }
        }

        public void WriteBytes(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger(Commands.Count);
            foreach (var command in Commands)
            {
                command.Save(myBuffer);
            }
        }
    }

    public enum EventCommandType
    {
        Null = 0,

        //Dialog
        ShowText,
        ShowOptions,
        AddChatboxText,
        //Logic Flow
        SetSwitch,
        SetVariable,
        SetSelfSwitch,
        ConditionalBranch,
        ExitEventProcess,
        Label,
        GoToLabel,
        StartCommonEvent,
        //Player Control
        RestoreHp,
        RestoreMp,
        LevelUp,
        GiveExperience,
        ChangeLevel,
        ChangeSpells,
        ChangeItems,
        ChangeSprite,
        ChangeFace,
        ChangeGender,
        SetAccess,
        //Movement,
        WarpPlayer,
        SetMoveRoute,
        WaitForRouteCompletion,
        HoldPlayer,
        ReleasePlayer,
        SpawnNpc,
        //Special Effects
        PlayAnimation,
        PlayBgm,
        FadeoutBgm,
        PlaySound,
        StopSounds,
        //Etc
        Wait,
        //Shop and Bank
        OpenBank,
        OpenShop
    }

    public class EventCommand
    {
        public EventCommandType Type;
        public string[] Strs = new string[6];
        public int[] Ints = new int[6];
        public EventMoveRoute Route;
        public EventCommand()
        {
            for (var i = 0; i < 6; i++)
            {
                Strs[i] = "";
                Ints[i] = 0;
            }
        }

        public void Load(ByteBuffer myBuffer)
        {
            Type = (EventCommandType)myBuffer.ReadInteger();
            for (var x = 0; x < 6; x++)
            {
                Strs[x] = myBuffer.ReadString();
                Ints[x] = myBuffer.ReadInteger();
            }
            if (Type == EventCommandType.SetMoveRoute)
            {
                Route = new EventMoveRoute();
                Route.Load(myBuffer);
            }
        }

        public void Save(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger((int)Type);
            for (var x = 0; x < 6; x++)
            {
                myBuffer.WriteString(Strs[x]);
                myBuffer.WriteInteger(Ints[x]);
            }
            if (Type == EventCommandType.SetMoveRoute)
            {
                Route.Save(myBuffer);
            }
        }
    }

    public class EventGraphic
    {
        public string Filename;
        public int Type;
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public EventGraphic()
        {
            Type = 0;
            Filename = "";
            X = -1;
            Y = -1;
            Width = -1;
            Height = -1;
        }

        public void Load(ByteBuffer curBuffer)
        {
            Type = curBuffer.ReadInteger();
            Filename = curBuffer.ReadString();
            X = curBuffer.ReadInteger();
            Y = curBuffer.ReadInteger();
            Width = curBuffer.ReadInteger();
            Height = curBuffer.ReadInteger();
        }

        public void Save(ByteBuffer curBuffer)
        {
            curBuffer.WriteInteger(Type);
            curBuffer.WriteString(Filename);
            curBuffer.WriteInteger(X);
            curBuffer.WriteInteger(Y);
            curBuffer.WriteInteger(Width);
            curBuffer.WriteInteger(Height);
        }

        public void CopyFrom(EventGraphic toCopy)
        {
            Type = toCopy.Type;
            Filename = toCopy.Filename;
            X = toCopy.X;
            Y = toCopy.Y;
            Width = toCopy.Width;
            Height = toCopy.Height;
        }

    }
}
