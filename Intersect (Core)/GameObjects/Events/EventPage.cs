using System;
using System.Collections.Generic;

using Intersect.Enums;
using Intersect.GameObjects.Conditions;
using Intersect.GameObjects.Events.Commands;

using Newtonsoft.Json;

namespace Intersect.GameObjects.Events
{

    public class EventPage
    {

        public EventPage()
        {
            CommandLists.Add(Guid.NewGuid(), new List<EventCommand>());
        }

        [JsonConstructor]
        public EventPage(int ignoreThis)
        {
        }

        public Guid AnimationId { get; set; }

        public Dictionary<Guid, List<EventCommand>> CommandLists { get; set; } =
            new Dictionary<Guid, List<EventCommand>>();

        public ConditionLists ConditionLists { get; set; } = new ConditionLists();

        public string Description { get; set; } = "";

        public bool DirectionFix { get; set; }

        public bool DisablePreview { get; set; } = true;

        public string FaceGraphic { get; set; } = "";

        public EventGraphic Graphic { get; set; } = new EventGraphic();

        public bool HideName { get; set; }

        public bool InteractionFreeze { get; set; }

        public EventRenderLayer Layer { get; set; } = EventRenderLayer.SameAsPlayer;

        public EventMovement Movement { get; set; } = new EventMovement();

        public bool Passable { get; set; }

        public EventTrigger Trigger { get; set; } = EventTrigger.ActionButton;

        public CommonEventTrigger CommonTrigger { get; set; } = CommonEventTrigger.None;

        public string TriggerCommand { get; set; }

        public Guid TriggerId { get; set; }

        public Guid TriggerVal { get; set; }

        public bool WalkingAnimation { get; set; } = true;

    }

}
