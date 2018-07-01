using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        //Basic Quest Properties
        public string StartDesc { get; set; } = "";
        public string BeforeDesc { get; set; } = "";
        public string EndDesc { get; set; } = "";
        public string InProgressDesc { get; set; } = "";
        public byte LogAfterComplete { get; set; }
        public byte LogBeforeOffer { get; set; }
        public byte Quitable { get; set; }
        public byte Repeatable { get; set; }

        //Tasks - To be phased out with guids
        public int NextTaskId { get; set; }

        //Requirements - Store with json
        [Column("Requirements")]
        [JsonIgnore]
        public string JsonRequirements
        {
            get => JsonConvert.SerializeObject(Requirements);
            set => Requirements = JsonConvert.DeserializeObject<ConditionLists>(value);
        }
        [NotMapped]
        public ConditionLists Requirements { get; set; } = new ConditionLists();


        [Column("StartEvent")]
        public Guid StartEventId { get; set; }
        [NotMapped]
        [JsonIgnore]
        public EventBase StartEvent
        {
            get => EventBase.Get(StartEventId);
            set => StartEventId = value.Id;
        }

        [Column("EndEvent")]
        public Guid EndEventId { get;  set; }
        [NotMapped]
        [JsonIgnore]
        public EventBase EndEvent
        {
            get => EventBase.Get(EndEventId);
            set => EndEventId = value.Id;
        }

        [Column("Tasks")]
        [JsonIgnore]
        public string TasksJson
        {
            get => JsonConvert.SerializeObject(Tasks);
            set => Tasks = JsonConvert.DeserializeObject<List<QuestTask>>(value);
        }
        [NotMapped]
        public List<QuestTask> Tasks = new List<QuestTask>();

        [NotMapped]
        public Dictionary<int, EventBase> AddEvents = new Dictionary<int, EventBase>();  //Events that need to be added for the quest, int is task id
        [NotMapped]
        public List<Guid> RemoveEvents = new List<Guid>(); //Events that need to be removed for the quest

        [JsonConstructor]
        public QuestBase(Guid Id) : base(Id)
        {
            Name = "New Quest";
        }

        //Parameterless EF Constructor
        public QuestBase()
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
            public Guid CompletionEventId { get; set; }
            [JsonIgnore]
            public EventBase CompletionEvent
            {
                get => EventBase.Get(CompletionEventId);
                set => CompletionEventId = value.Id;
            }
            [NotMapped]
            [JsonIgnore]
            public EventBase EdittingEvent;
            public int Data1;
            public int Data2;
            public Guid Guid1;
            public Guid Guid2;
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
                        taskString = descriptions[Objective].ToString(ItemBase.GetName(Guid1), Data2, Desc);
                        break;
                    case 2: //Kill Npcs
                        taskString = descriptions[Objective].ToString(NpcBase.GetName(Guid1), Data2, Desc);
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