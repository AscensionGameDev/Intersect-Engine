using System.Collections.Generic;
using Intersect.GameObjects.Conditions;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
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
        public int Task;
        public int Completed;
        public int TaskProgress;
    }

    public class QuestBase : DatabaseObject<QuestBase>
    {
        public string BeforeDesc = "";
        public string EndDesc = "";
        public EventBase EndEvent = new EventBase(-1, -1, 0, 0, true);
        public string InProgressDesc = "";
        public byte LogAfterComplete;
        public byte LogBeforeOffer;

        //Tasks
        public int NextTaskId;

        public byte Quitable;

        public byte Repeatable;

        //Requirements
        public ConditionLists Requirements = new ConditionLists();

        public string StartDesc = "";

        //Events
        public EventBase StartEvent = new EventBase(-1, -1, 0, 0, true);

        public List<QuestTask> Tasks = new List<QuestTask>();

        [JsonConstructor]
        public QuestBase(int index) : base(index)
        {
            Name = "New Quest";
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

        public class QuestTask
        {
            public EventBase CompletionEvent = new EventBase(-1,-1, 0, 0, true);
            public int Data1;
            public int Data2;
            public string Desc = "";
            public int Id;
            public int Objective;

            public QuestTask(int id)
            {
                Id = id;
            }

            public string GetTaskString(LocalizedString[] descriptions)
            {
                var taskString = "";
                switch (Objective)
                {
                    case 0: //Event Driven
                        taskString = descriptions[Objective].ToString(Desc);
                        break;
                    case 1: //Gather Items
                        taskString = descriptions[Objective].ToString(ItemBase.GetName(Data1), Data2, Desc);
                        break;
                    case 2: //Kill Npcs
                        taskString = descriptions[Objective].ToString(NpcBase.GetName(Data1), Data2, Desc);
                        break;
                }
                return taskString;
            }

            public static QuestBase Get(int index)
            {
                return QuestBase.Lookup.Get<QuestBase>(index);
            }
        }

        public class QuestReward
        {
            public int Amount = 0;
            public int ItemNum = 0;
        }
    }
}