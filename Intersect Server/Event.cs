using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntersectServer
{
    public class Event
    {
        public string myName = "New Event";
        public int spawnX;
        public int spawnY;
        public int deleted;
        public int pageCount = 1;
        public List<EventPage> myPages = new List<EventPage>();

        public Event(int x, int y)
        {
            spawnX = x;
            spawnY = y;
            myPages.Add(new EventPage());
        }
        public Event(ByteBuffer myBuffer)
        {
            myName = myBuffer.ReadString();
            spawnX = myBuffer.ReadInteger();
            spawnY = myBuffer.ReadInteger();
            deleted = myBuffer.ReadInteger();
            pageCount = myBuffer.ReadInteger();
            for (int i = 0; i < pageCount; i++)
            {
                myPages.Add(new EventPage(myBuffer));
            }
        }
        public byte[] EventData()
        {
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteString(myName);
            myBuffer.WriteInteger(spawnX);
            myBuffer.WriteInteger(spawnY);
            myBuffer.WriteInteger(deleted);
            myBuffer.WriteInteger(pageCount);
            for (int i = 0; i < pageCount; i++)
            {
                myPages[i].WriteBytes(myBuffer);
            }
            return myBuffer.ToArray();
        }
    }

    public class EventPage
    {
        public EventConditions myConditions;
        public int movementType;
        public int movementSpeed;
        public int movementFreq;
        public int passable;
        public int layer;
        public int trigger;
        public int graphicType;
        public string graphic;
        public int graphicx;
        public int graphicy;
        public int hideName;
        public List<CommandList> commandLists = new List<CommandList>();

        public EventPage()
        {
            myConditions = new EventConditions();
            movementType = 0;
            movementSpeed = 2;
            movementFreq = 2;
            passable = 0;
            layer = 1;
            trigger = 0;
            graphicType = 0;
            graphic = "";
            graphicx = -1;
            graphicy = -1;
            hideName = 0;
            commandLists.Add(new CommandList());
        }

        public EventPage(ByteBuffer curBuffer)
        {
            int x = 0;
            myConditions = new EventConditions();
            myConditions.Load(curBuffer);
            movementType = curBuffer.ReadInteger();
            movementSpeed = curBuffer.ReadInteger();
            movementFreq = curBuffer.ReadInteger();
            passable = curBuffer.ReadInteger();
            layer = curBuffer.ReadInteger();
            trigger = curBuffer.ReadInteger();
            graphicType = curBuffer.ReadInteger();
            graphic = curBuffer.ReadString();
            graphicx = curBuffer.ReadInteger();
            graphicy = curBuffer.ReadInteger();
            hideName = curBuffer.ReadInteger();
            x = curBuffer.ReadInteger();
            for (int i = 0; i < x; i++)
            {
                commandLists.Add(new CommandList(curBuffer));
            }

        }

        public void WriteBytes(ByteBuffer myBuffer)
        {
            myConditions.WriteBytes(myBuffer);
            myBuffer.WriteInteger(movementType);
            myBuffer.WriteInteger(movementSpeed);
            myBuffer.WriteInteger(movementFreq);
            myBuffer.WriteInteger(passable);
            myBuffer.WriteInteger(layer);
            myBuffer.WriteInteger(trigger);
            myBuffer.WriteInteger(graphicType);
            myBuffer.WriteString(graphic);
            myBuffer.WriteInteger(graphicx);
            myBuffer.WriteInteger(graphicy);
            myBuffer.WriteInteger(hideName);
            myBuffer.WriteInteger(commandLists.Count);
            for (int i = 0; i < commandLists.Count; i++)
            {
                commandLists[i].WriteBytes(myBuffer);
            }
        }

    }

    public class EventConditions
    {
        public int switch1 = 0;
        public bool switch1val = false;
        public int switch2 = 0;
        public bool switch2val = false;

        public void WriteBytes(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger(switch1);
            myBuffer.WriteInteger(Convert.ToInt32(switch1val));
            myBuffer.WriteInteger(switch2);
            myBuffer.WriteInteger(Convert.ToInt32(switch1val));
        }

        public void Load(ByteBuffer myBuffer)
        {
            switch1 = myBuffer.ReadInteger();
            switch1val = Convert.ToBoolean(myBuffer.ReadInteger());
            switch2 = myBuffer.ReadInteger();
            switch2val = Convert.ToBoolean(myBuffer.ReadInteger());
        }
    }

    public class CommandList
    {
        public List<EventCommand> commands = new List<EventCommand>();

        public CommandList()
        {

        }

        public CommandList(ByteBuffer myBuffer)
        {
            int y = myBuffer.ReadInteger();
            for (int i = 0; i < y; i++)
            {
                commands.Add(new EventCommand());
                commands[i].type = myBuffer.ReadInteger();
                if (commands[i].type != 4)
                {
                    for (int x = 0; x < 6; x++)
                    {
                        commands[i].strs[x] = myBuffer.ReadString();
                        commands[i].ints[x] = myBuffer.ReadInteger();
                    }
                }
                else
                {
                    commands[i].myConditions.Load(myBuffer);
                    for (int x = 0; x < 6; x++)
                    {
                        commands[i].strs[x] = myBuffer.ReadString();
                        commands[i].ints[x] = myBuffer.ReadInteger();
                    }
                }
            }
        }

        public void WriteBytes(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger(commands.Count);
            for (int i = 0; i < commands.Count; i++)
            {
                myBuffer.WriteInteger(commands[i].type);
                if (commands[i].type != 4)
                {
                    for (int x = 0; x < 6; x++)
                    {
                        myBuffer.WriteString(commands[i].strs[x]);
                        myBuffer.WriteInteger(commands[i].ints[x]);
                    }
                }
                else
                {
                    commands[i].myConditions.WriteBytes(myBuffer);
                    for (int x = 0; x < 6; x++)
                    {
                        myBuffer.WriteString(commands[i].strs[x]);
                        myBuffer.WriteInteger(commands[i].ints[x]);
                    }
                }
            }
        }
    }

    public class EventCommand
    {
        public int type;
        public EventConditions myConditions = new EventConditions();
        public string[] strs = new string[6];
        public int[] ints = new int[6];
        public EventCommand()
        {
            for (int i = 0; i < 6; i++)
            {
                strs[i] = "";
                ints[i] = 0;
            }
        }
    }
}
