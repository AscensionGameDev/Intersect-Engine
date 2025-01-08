using Intersect.Enums;
using Intersect.GameObjects.Conditions;
using Intersect.GameObjects.Events.Commands;

using Newtonsoft.Json;

namespace Intersect.GameObjects.Events;

public partial class EventPage
{
    public EventPage()
    {
        CommandLists.Add(Guid.NewGuid(), []);
    }

    [JsonConstructor]
    public EventPage(int ignoreThis)
    {
    }

    public Guid AnimationId { get; set; }

    public Dictionary<Guid, List<EventCommand>> CommandLists { get; set; } = new();

    public ConditionLists ConditionLists { get; set; } = new();

    public string Description { get; set; } = string.Empty;

    public bool DirectionFix { get; set; }

    public bool DisablePreview { get; set; } = true;

    public string FaceGraphic { get; set; } = string.Empty;

    public EventGraphic Graphic { get; set; } = new();

    public bool HideName { get; set; }

    public bool InteractionFreeze { get; set; }

    public EventRenderLayer Layer { get; set; } = EventRenderLayer.SameAsPlayer;

    public EventMovement Movement { get; set; } = new();

    public bool Passable { get; set; }

    public EventTrigger Trigger { get; set; } = EventTrigger.ActionButton;

    public CommonEventTrigger CommonTrigger { get; set; } = CommonEventTrigger.None;

    public string TriggerCommand { get; set; }

    public Guid TriggerId { get; set; }

    public Guid TriggerVal { get; set; }

    public bool WalkingAnimation { get; set; } = true;

    public bool IgnoreNpcAvoids { get; set; }
}
