using System.Collections.Generic;
using Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects.Conditions;
using Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects.Events;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects
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

    public class QuestBase : DatabaseObject<QuestBase>
    {
        public string BeforeDesc = "";
        public string EndDesc = "";
        public EventBase EndEvent = new EventBase(-1, 0, 0, true);
        public string InProgressDesc = "";
        public byte LogAfterComplete;
        public byte LogBeforeOffer;

        //Tasks
        public int NextTaskID;
        public byte Quitable;

        public byte Repeatable;

        //Requirements
        public ConditionLists Requirements = new ConditionLists();
        public string StartDesc = "";

        //Events
        public EventBase StartEvent = new EventBase(-1, 0, 0, true);
        public List<QuestTask> Tasks = new List<QuestTask>();

        public QuestBase(int id) : base(id)
        {
            Name = "New Quest";
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

            Requirements.Load(myBuffer);

            NextTaskID = myBuffer.ReadInteger();
            var MaxTasks = myBuffer.ReadInteger();
            Tasks.Clear();
            for (int i = 0; i < MaxTasks; i++)
            {
                QuestTask task = new QuestTask(myBuffer.ReadInteger())
                {
                    Objective = myBuffer.ReadInteger(),
                    Desc = myBuffer.ReadString(),
                    Data1 = myBuffer.ReadInteger(),
                    Data2 = myBuffer.ReadInteger()
                };

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

            Requirements.Save(myBuffer);

            myBuffer.WriteInteger(NextTaskID);
            myBuffer.WriteInteger(Tasks.Count);
            for (int i = 0; i < Tasks.Count; i++)
            {
                myBuffer.WriteInteger(Tasks[i].Id);
                myBuffer.WriteInteger(Tasks[i].Objective);
                myBuffer.WriteString(Tasks[i].Desc);
                myBuffer.WriteInteger(Tasks[i].Data1);
                myBuffer.WriteInteger(Tasks[i].Data2);

                var taskCompleteionData = Tasks[i].CompletionEvent.BinaryData;
                myBuffer.WriteInteger(taskCompleteionData.Length);
                myBuffer.WriteBytes(taskCompleteionData);
            }

            var startEventData = StartEvent.BinaryData;
            myBuffer.WriteInteger(startEventData.Length);
            myBuffer.WriteBytes(startEventData);

            var endEventData = EndEvent.BinaryData;
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

        public override byte[] BinaryData => QuestData();

        public class QuestTask
        {
            public EventBase CompletionEvent = new EventBase(-1, 0, 0, true);
            public int Data1;
            public int Data2;
            public string Desc = "";
            public int Id;
            public int Objective;

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
            public int Amount = 0;
            public int ItemNum = 0;
        }
    }
}