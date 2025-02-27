using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Conditions;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.Serialization;
using Intersect.Localization;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects;

public enum QuestProgressState
{
    OnAnyTask = 0,

    BeforeTask = 1,

    AfterTask = 2,

    OnTask = 3,
}

public partial class QuestProgress
{
    public bool Completed;

    public Guid TaskId;

    public int TaskProgress;

    public QuestProgress(string data)
    {
        JsonConvert.PopulateObject(data, this);
    }
}

public partial class QuestBase : DatabaseObject<QuestBase>, IFolderable
{
    [NotMapped]
    [JsonIgnore]
    //Events that need to be added for the quest, int is task id
    public Dictionary<Guid, EventDescriptor> AddEvents { get; set; } = [];

    [NotMapped]
    //Events that need to be removed for the quest
    public List<Guid> RemoveEvents { get; set; } = [];

    [NotMapped]
    public List<QuestTask> Tasks { get; set; } = [];

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
    public string StartDescription { get; set; } = string.Empty;

    public string BeforeDescription { get; set; } = string.Empty;

    public string EndDescription { get; set; } = string.Empty;

    public string InProgressDescription { get; set; } = string.Empty;

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
    public ConditionLists Requirements { get; set; } = new();

    [Column("StartEvent")]
    public Guid StartEventId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public EventDescriptor StartEvent
    {
        get => EventDescriptor.Get(StartEventId);
        set => StartEventId = value.Id;
    }

    [Column("EndEvent")]
    public Guid EndEventId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public EventDescriptor EndEvent
    {
        get => EventDescriptor.Get(EndEventId);
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
                SerializationBinder = new IntersectTypeSerializationBinder(),
                TypeNameHandling = TypeNameHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            }
        );
        set => JsonConvert.PopulateObject(
            value, AddEvents,
            new JsonSerializerSettings()
            {
                SerializationBinder = new IntersectTypeSerializationBinder(),
                TypeNameHandling = TypeNameHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            }
        );
    }

    //Editor Only
    [NotMapped]
    [JsonIgnore]
    public Dictionary<Guid, Guid> OriginalTaskEventIds { get; set; } = new();

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;

    /// <summary>
    /// Hides this quest from the quest log if it has not been started and cannot be started due to the requiremetns/conditions
    /// </summary>
    public bool DoNotShowUnlessRequirementsMet { get; set; }

    /// <summary>
    /// Quest category in the quest log when this quest hasn't been started yet
    /// </summary>
    public string UnstartedCategory { get; set; } = string.Empty;

    /// <summary>
    /// Quest category in the quest log when this quest is in progress
    /// </summary>
    public string InProgressCategory { get; set; } = string.Empty;

    /// <summary>
    /// Quest category in the quest log when this quest has been completed
    /// </summary>
    public string CompletedCategory { get; set; } = string.Empty;

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

    public partial class QuestTask
    {
        [NotMapped]
        [JsonIgnore]
        public EventDescriptor EditingEvent;

        public QuestTask(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

        public Guid CompletionEventId { get; set; }

        [JsonIgnore]
        public EventDescriptor CompletionEvent
        {
            get => EventDescriptor.Get(CompletionEventId);
            set => CompletionEventId = value.Id;
        }

        //# of npcs to kill, # of X item to collect, or for event driven this value should be 1
        public QuestObjective Objective { get; set; } = QuestObjective.EventDriven;

        public Guid TargetId { get; set; }

        public int Quantity { get; set; }

        public string Description { get; set; } = string.Empty;

        public string GetTaskString(Dictionary<int, LocalizedString> descriptions)
        {
            var taskString = string.Empty;
            switch (Objective)
            {
                case QuestObjective.EventDriven: //Event Driven
                    taskString = descriptions[(int)Objective].ToString(Description);

                    break;
                case QuestObjective.GatherItems: //Gather Items
                    taskString = descriptions[(int)Objective].ToString(
                        ItemDescriptor.GetName(TargetId),
                        Quantity,
                        Description
                    );

                    break;
                case QuestObjective.KillNpcs: //Kill Npcs
                    taskString = descriptions[(int)Objective].ToString(
                        NpcBase.GetName(TargetId),
                        Quantity,
                        Description
                    );

                    break;
            }

            return taskString;
        }
    }
}
