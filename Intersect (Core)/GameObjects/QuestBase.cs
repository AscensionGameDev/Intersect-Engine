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

    public enum QuestProgressState
    {

        OnAnyTask = 0,

        BeforeTask = 1,

        AfterTask = 2,

        OnTask = 3,

    }

    public class QuestProgress
    {

        public bool Completed;

        public Guid TaskId;

        public int TaskProgress;

        public QuestProgress(string data)
        {
            JsonConvert.PopulateObject(data, this);
        }

    }

    public class QuestBase : DatabaseObject<QuestBase>, IFolderable
    {

        [NotMapped] [JsonIgnore]
        public Dictionary<Guid, EventBase>
            AddEvents = new Dictionary<Guid, EventBase>(); //Events that need to be added for the quest, int is task id

        [NotMapped] public List<Guid> RemoveEvents = new List<Guid>(); //Events that need to be removed for the quest

        [NotMapped] public List<QuestTask> Tasks = new List<QuestTask>();

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

        //Basic Quest Properties
        public string StartDescription { get; set; } = "";

        public string BeforeDescription { get; set; } = "";

        public string EndDescription { get; set; } = "";

        public string InProgressDescription { get; set; } = "";

        public bool LogAfterComplete { get; set; }

        public bool LogBeforeOffer { get; set; }

        public bool Quitable { get; set; }

        public bool Repeatable { get; set; }

        //Requirements - Store with json
        [Column("Requirements")]
        [JsonIgnore]
        public string JsonRequirements
        {
            get => Requirements.Data();
            set => Requirements.Load(value);
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
        public Guid EndEventId { get; set; }

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
        public string LocalEventsJson
        {
            get => JsonConvert.SerializeObject(
                AddEvents, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );
            set => JsonConvert.PopulateObject(
                value, AddEvents,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );
        }

        //Editor Only
        [NotMapped]
        [JsonIgnore]
        public Dictionary<Guid, Guid> OriginalTaskEventIds { get; set; } = new Dictionary<Guid, Guid>();

        /// <inheritdoc />
        public string Folder { get; set; } = "";

        /// <summary>
        /// Hides this quest from the quest log if it has not been started and cannot be started due to the requiremetns/conditions
        /// </summary>
        public bool DoNotShowUnlessRequirementsMet { get; set; }

        /// <summary>
        /// Quest category in the quest log when this quest hasn't been started yet
        /// </summary>
        public string UnstartedCategory { get; set; } = "";

        /// <summary>
        /// Quest category in the quest log when this quest is in progress
        /// </summary>
        public string InProgressCategory { get; set; } = "";

        /// <summary>
        /// Quest category in the quest log when this quest has been completed
        /// </summary>
        public string CompletedCategory { get; set; } = "";

        /// <summary>
        /// Order priority of this quest within the quest log
        /// </summary>
        public int OrderValue { get; set; }


        public int GetTaskIndex(Guid taskId)
        {
            for (var i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].Id == taskId)
                {
                    return i;
                }
            }

            return -1;
        }

        public QuestTask FindTask(Guid taskId)
        {
            for (var i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].Id == taskId)
                {
                    return Tasks[i];
                }
            }

            return null;
        }

        public class QuestTask
        {

            [NotMapped] [JsonIgnore] public EventBase EditingEvent;

            public QuestTask(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; set; }

            public Guid CompletionEventId { get; set; }

            [JsonIgnore]
            public EventBase CompletionEvent
            {
                get => EventBase.Get(CompletionEventId);
                set => CompletionEventId = value.Id;
            }

            //# of npcs to kill, # of X item to collect, or for event driven this value should be 1
            public QuestObjective Objective { get; set; } = QuestObjective.EventDriven;

            public Guid TargetId { get; set; }

            public int Quantity { get; set; }

            public string Description { get; set; } = "";

            public string GetTaskString(Dictionary<int, LocalizedString> descriptions)
            {
                var taskString = "";
                switch (Objective)
                {
                    case QuestObjective.EventDriven: //Event Driven
                        taskString = descriptions[(int) Objective].ToString(Description);

                        break;
                    case QuestObjective.GatherItems: //Gather Items
                        taskString = descriptions[(int) Objective]
                            .ToString(ItemBase.GetName(TargetId), Quantity, Description);

                        break;
                    case QuestObjective.KillNpcs: //Kill Npcs
                        taskString = descriptions[(int) Objective]
                            .ToString(NpcBase.GetName(TargetId), Quantity, Description);

                        break;
                }

                return taskString;
            }

        }

    }

}
