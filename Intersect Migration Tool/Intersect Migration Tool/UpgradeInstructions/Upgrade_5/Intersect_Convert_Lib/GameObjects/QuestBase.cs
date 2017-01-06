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

using System.Collections.Generic;
using System.Linq;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects.Events;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects
{
    public enum QuestProgress
    {
        OnAnyTask = 0,
        BeforeTask = 1,
        AfterTask = 2,
        OnTask = 3,
    }

    public struct QuestProgressStruct
    {
        public int task;
        public int completed;
        public int taskProgress;
    }

    public class QuestBase : DatabaseObject
    {
        //General
        public new const string DatabaseTable = "quests";
        public new const GameObject Type = GameObject.Quest;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();
        
        public string Name = "New Quest";
        public string BeforeDesc = "";
        public string StartDesc = "";
        public string InProgressDesc = "";
        public string EndDesc = "";

        public byte Repeatable = 0;
        public byte Quitable = 0;
        public byte LogBeforeOffer = 0;
        public byte LogAfterComplete = 0;

        //Requirements
        //I am cheating here and using event commands as conditional branches instead of having a lot of duplicate code.
        public List<EventCommand> Requirements = new List<EventCommand>();

        //Tasks
        public int NextTaskID = 0;
        public List<QuestTask> Tasks = new List<QuestTask>();

        //Events
        public EventBase StartEvent = new EventBase(-1, 0, 0, true);
        public EventBase EndEvent = new EventBase(-1, 0, 0, true);

        public QuestBase(int id) : base(id)
        {
            
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            BeforeDesc = myBuffer.ReadString();
            StartDesc = myBuffer.ReadString();
            InProgressDesc = myBuffer.ReadString();
            EndDesc = myBuffer.ReadString();

            Repeatable = myBuffer.ReadByte();
            Quitable = myBuffer.ReadByte();
            LogBeforeOffer = myBuffer.ReadByte();
            LogAfterComplete = myBuffer.ReadByte();

            var RequirementCount = myBuffer.ReadInteger();
            Requirements.Clear();
            for (int i = 0; i < RequirementCount; i++)
            {
                var cmd = new EventCommand();
                cmd.Load(myBuffer);
                Requirements.Add(cmd);
            }

            NextTaskID = myBuffer.ReadInteger();
            var MaxTasks = myBuffer.ReadInteger();
            Tasks.Clear();
            for (int i = 0; i < MaxTasks; i++)
            {
                QuestTask task = new QuestTask(myBuffer.ReadInteger());
                task.Objective = myBuffer.ReadInteger();
                task.Desc = myBuffer.ReadString();
                task.Data1 = myBuffer.ReadInteger();
                task.Data2 = myBuffer.ReadInteger();

                var taskCompletionEventLength = myBuffer.ReadInteger();
                task.CompletionEvent.Load(myBuffer.ReadBytes(taskCompletionEventLength));

                Tasks.Add(task);
            }

            var startEventLength = myBuffer.ReadInteger();
            StartEvent.Load(myBuffer.ReadBytes(startEventLength));

            var endEventLength = myBuffer.ReadInteger();
            EndEvent.Load(myBuffer.ReadBytes(endEventLength));

            myBuffer.Dispose();
        }

        public byte[] QuestData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteString(BeforeDesc);
            myBuffer.WriteString(StartDesc);
            myBuffer.WriteString(InProgressDesc);
            myBuffer.WriteString(EndDesc);

            myBuffer.WriteByte(Repeatable);
            myBuffer.WriteByte(Quitable);
            myBuffer.WriteByte(LogBeforeOffer);
            myBuffer.WriteByte(LogAfterComplete);

            myBuffer.WriteInteger(Requirements.Count);
            for (int i = 0; i < Requirements.Count; i++)
            {
                Requirements[i].Save(myBuffer);
            }

            myBuffer.WriteInteger(NextTaskID);
            myBuffer.WriteInteger(Tasks.Count);
            for (int i = 0; i < Tasks.Count; i++)
            {
                myBuffer.WriteInteger(Tasks[i].Id);
                myBuffer.WriteInteger(Tasks[i].Objective);
                myBuffer.WriteString(Tasks[i].Desc);
                myBuffer.WriteInteger(Tasks[i].Data1);
                myBuffer.WriteInteger(Tasks[i].Data2);

                var taskCompleteionData = Tasks[i].CompletionEvent.GetData();
                myBuffer.WriteInteger(taskCompleteionData.Length);
                myBuffer.WriteBytes(taskCompleteionData);
            }

            var startEventData = StartEvent.GetData();
            myBuffer.WriteInteger(startEventData.Length);
            myBuffer.WriteBytes(startEventData);

            var endEventData = EndEvent.GetData();
            myBuffer.WriteInteger(endEventData.Length);
            myBuffer.WriteBytes(endEventData);

            return myBuffer.ToArray();
        }

        public int GetTaskIndex(int taskId)
        {
            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].Id == taskId) return i;
            }
            return -1;
        }

        public QuestTask FindTask(int taskId)
        {
            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].Id == taskId) return Tasks[i];
            }
            return null;
        }

        public static QuestBase GetQuest(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (QuestBase)Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((QuestBase)Objects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return QuestData();
        }

        public override string GetTable()
        {
            return DatabaseTable;
        }

        public override GameObject GetGameObjectType()
        {
            return Type;
        }

        public static DatabaseObject Get(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return Objects[index];
            }
            return null;
        }
        public override void Delete()
        {
            Objects.Remove(GetId());
        }
        public static void ClearObjects()
        {
            Objects.Clear();
        }
        public static void AddObject(int index, DatabaseObject obj)
        {
            Objects.Remove(index);
            Objects.Add(index, obj);
        }
        public static int ObjectCount()
        {
            return Objects.Count;
        }
        public static Dictionary<int, QuestBase> GetObjects()
        {
            Dictionary<int, QuestBase> objects = Objects.ToDictionary(k => k.Key, v => (QuestBase)v.Value);
            return objects;
        }

        public class QuestTask
        {
            public int Id = 0;
            public int Objective = 0;
            public string Desc = "";
            public int Data1 = 0;
            public int Data2 = 0;
            public EventBase CompletionEvent = new EventBase(-1,0,0,true);

            public QuestTask(int id)
            {
                Id = id;
            }

            public string GetTaskString()
            {
                var taskString = "";
                switch (Objective)
                {
                    case 0: //Event Driven
                        taskString = "Event Driven - " + Desc;
                        break;
                    case 1: //Gather Items
                        taskString = "Gather Items [" + ItemBase.GetName(Data1) + " x" + Data2 + "] - " + Desc;
                        break;
                    case 2: //Kill Npcs
                        taskString = "Kill Npc(s) [" + NpcBase.GetName(Data1) + " x" + Data2 + "] - " + Desc;
                        break;
                }
                return taskString;
            }
        }

        public class QuestReward
        {
            public int ItemNum = 0;
            public int Amount = 0;
        }
    }
}
