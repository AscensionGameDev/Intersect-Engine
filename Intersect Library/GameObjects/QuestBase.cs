using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
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
        public Guid TaskId;
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
        public string LocalEventsJson
        {
            get => JsonConvert.SerializeObject(AddEvents, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, DefaultValueHandling = DefaultValueHandling.Ignore, ObjectCreationHandling = ObjectCreationHandling.Replace });
            set => JsonConvert.PopulateObject(value, AddEvents, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, DefaultValueHandling = DefaultValueHandling.Ignore, ObjectCreationHandling = ObjectCreationHandling.Replace });
        }
        [NotMapped]
        [JsonIgnore]
        public Dictionary<Guid, EventBase> AddEvents = new Dictionary<Guid, EventBase>();  //Events that need to be added for the quest, int is task id
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

        public int GetTaskIndex(Guid taskId)
        {
            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].Id == taskId) return i;
            }
            return -1;
        }

        public QuestTask FindTask(Guid taskId)
        {
            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].Id == taskId) return Tasks[i];
            }
            return null;
        }

        public class QuestTask
        {
            public Guid Id { get; protected set; }

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
            
            //# of npcs to kill, # of X item to collect, or for event driven this value should be 1
            public QuestObjective Objective { get; set; } = QuestObjective.EventDriven;
            public Guid TargetId { get; set; }
            public int Quantity { get; set; }
            public string Description { get; set; } = "";

            public QuestTask(Guid id)
            {
                Id = id;
            }

            public string GetTaskString(LocalizedString[] descriptions)
            {
                var taskString = "";
                switch (Objective)
                {
                    case QuestObjective.EventDriven: //Event Driven
                        taskString = descriptions[(int)Objective].ToString(Description);
                        break;
                    case QuestObjective.GatherItems: //Gather Items
                        taskString = descriptions[(int)Objective].ToString(ItemBase.GetName(TargetId), Quantity, Description);
                        break;
                    case QuestObjective.KillNpcs: //Kill Npcs
                        taskString = descriptions[(int)Objective].ToString(NpcBase.GetName(TargetId), Quantity, Description);
                        break;
                }
                return taskString;
            }
        }
    }
}