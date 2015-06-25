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

namespace Intersect_Server.Classes
{
    public class EventStruct
    {
        public string MyName = "New Event";
        public int SpawnX;
        public int SpawnY;
        public int Deleted;
        public int PageCount = 1;
        public List<EventPage> MyPages = new List<EventPage>();

        public EventStruct(int x, int y)
        {
            SpawnX = x;
            SpawnY = y;
            MyPages.Add(new EventPage());
        }
        public EventStruct(ByteBuffer myBuffer)
        {
            MyName = myBuffer.ReadString();
            SpawnX = myBuffer.ReadInteger();
            SpawnY = myBuffer.ReadInteger();
            Deleted = myBuffer.ReadInteger();
            PageCount = myBuffer.ReadInteger();
            for (var i = 0; i < PageCount; i++)
            {
                MyPages.Add(new EventPage(myBuffer));
            }
        }
        public byte[] EventData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(MyName);
            myBuffer.WriteInteger(SpawnX);
            myBuffer.WriteInteger(SpawnY);
            myBuffer.WriteInteger(Deleted);
            myBuffer.WriteInteger(PageCount);
            for (var i = 0; i < PageCount; i++)
            {
                MyPages[i].WriteBytes(myBuffer);
            }
            return myBuffer.ToArray();
        }
    }

    public class EventPage
    {
        public EventConditions MyConditions;
        public string Desc = "";
        public int MovementType;
        public int MovementSpeed;
        public int MovementFreq;
        public int Passable;
        public int Layer;
        public int Trigger;
        public int GraphicType;
        public string Graphic = "";
        public string FaceGraphic = "";
        public int Graphicx;
        public int Graphicy;
        public int HideName;
        public int DisablePreview;
        public List<CommandList> CommandLists = new List<CommandList>();

        public EventPage()
        {
            MyConditions = new EventConditions();
            MovementType = 0;
            MovementSpeed = 2;
            MovementFreq = 2;
            Passable = 0;
            Layer = 1;
            Trigger = 0;
            GraphicType = 0;
            Graphic = "";
            Graphicx = -1;
            Graphicy = -1;
            HideName = 0;
            CommandLists.Add(new CommandList());
        }

        public EventPage(ByteBuffer curBuffer)
        {
            MyConditions = new EventConditions();
            MyConditions.Load(curBuffer);
            Desc = curBuffer.ReadString();
            MovementType = curBuffer.ReadInteger();
            MovementSpeed = curBuffer.ReadInteger();
            MovementFreq = curBuffer.ReadInteger();
            Passable = curBuffer.ReadInteger();
            Layer = curBuffer.ReadInteger();
            Trigger = curBuffer.ReadInteger();
            GraphicType = curBuffer.ReadInteger();
            Graphic = curBuffer.ReadString();
            FaceGraphic = curBuffer.ReadString();
            Graphicx = curBuffer.ReadInteger();
            Graphicy = curBuffer.ReadInteger();
            HideName = curBuffer.ReadInteger();
            DisablePreview = curBuffer.ReadInteger();
            var x = curBuffer.ReadInteger();
            for (var i = 0; i < x; i++)
            {
                CommandLists.Add(new CommandList(curBuffer));
            }

        }

        public void WriteBytes(ByteBuffer myBuffer)
        {
            MyConditions.WriteBytes(myBuffer);
            myBuffer.WriteString(Desc);
            myBuffer.WriteInteger(MovementType);
            myBuffer.WriteInteger(MovementSpeed);
            myBuffer.WriteInteger(MovementFreq);
            myBuffer.WriteInteger(Passable);
            myBuffer.WriteInteger(Layer);
            myBuffer.WriteInteger(Trigger);
            myBuffer.WriteInteger(GraphicType);
            myBuffer.WriteString(Graphic);
            myBuffer.WriteString(FaceGraphic);
            myBuffer.WriteInteger(Graphicx);
            myBuffer.WriteInteger(Graphicy);
            myBuffer.WriteInteger(HideName);
            myBuffer.WriteInteger(DisablePreview);
            myBuffer.WriteInteger(CommandLists.Count);
            foreach (var t in CommandLists)
            {
                t.WriteBytes(myBuffer);
            }
        }

    }

    public class EventConditions
    {
        public int Switch1;
        public bool Switch1Val;
        public int Switch2;
        public bool Switch2Val;

        public void WriteBytes(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger(Switch1);
            myBuffer.WriteInteger(Convert.ToInt32(Switch1Val));
            myBuffer.WriteInteger(Switch2);
            myBuffer.WriteInteger(Convert.ToInt32(Switch1Val));
        }

        public void Load(ByteBuffer myBuffer)
        {
            Switch1 = myBuffer.ReadInteger();
            Switch1Val = Convert.ToBoolean(myBuffer.ReadInteger());
            Switch2 = myBuffer.ReadInteger();
            Switch2Val = Convert.ToBoolean(myBuffer.ReadInteger());
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
                Commands[i].Type = myBuffer.ReadInteger();
                if (Commands[i].Type != 4)
                {
                    for (var x = 0; x < 6; x++)
                    {
                        Commands[i].Strs[x] = myBuffer.ReadString();
                        Commands[i].Ints[x] = myBuffer.ReadInteger();
                    }
                }
                else
                {
                    Commands[i].MyConditions.Load(myBuffer);
                    for (var x = 0; x < 6; x++)
                    {
                        Commands[i].Strs[x] = myBuffer.ReadString();
                        Commands[i].Ints[x] = myBuffer.ReadInteger();
                    }
                }
            }
        }

        public void WriteBytes(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger(Commands.Count);
            for (var i = 0; i < Commands.Count; i++)
            {
                myBuffer.WriteInteger(Commands[i].Type);
                if (Commands[i].Type != 4)
                {
                    for (var x = 0; x < 6; x++)
                    {
                        myBuffer.WriteString(Commands[i].Strs[x]);
                        myBuffer.WriteInteger(Commands[i].Ints[x]);
                    }
                }
                else
                {
                    Commands[i].MyConditions.WriteBytes(myBuffer);
                    for (var x = 0; x < 6; x++)
                    {
                        myBuffer.WriteString(Commands[i].Strs[x]);
                        myBuffer.WriteInteger(Commands[i].Ints[x]);
                    }
                }
            }
        }
    }

    public class EventCommand
    {
        public int Type;
        public EventConditions MyConditions = new EventConditions();
        public string[] Strs = new string[6];
        public int[] Ints = new int[6];
        public EventCommand()
        {
            for (var i = 0; i < 6; i++)
            {
                Strs[i] = "";
                Ints[i] = 0;
            }
        }
    }
}
