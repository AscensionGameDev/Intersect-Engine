using System;
using System.Collections.Generic;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Conditions;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events.Commands;
using Newtonsoft.Json;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events
{
    public class EventPage
    {
        public enum CommonEventTriggers
        {
            None,
            JoinGame,
            LevelUp,
            OnRespawn,
            Command,
            Autorun,
        }

        public enum EventTriggers
        {
            ActionButton,
            OnTouch,
            Autorun,
            ProjectileHit,
        }

        public Guid AnimationId { get; set; }
        public Dictionary<Guid,List<EventCommand>> CommandLists { get; set; } = new Dictionary<Guid, List<EventCommand>>();
        public ConditionLists ConditionLists { get; set; } = new ConditionLists();
        public string Desc { get; set; } = "";
        public bool DirectionFix { get; set; }
        public bool DisablePreview { get; set; } = true;
        public string FaceGraphic { get; set; } = "";
        public EventGraphic Graphic { get; set; } = new EventGraphic();
        public bool HideName { get; set; }
        public bool InteractionFreeze { get; set; }
        public int Layer { get; set; }
        public int MovementFreq { get; set; }
        public int MovementSpeed { get; set; }
        public int MovementType { get; set; }
        public EventMoveRoute MoveRoute { get; set; } = new EventMoveRoute();
        public bool Passable { get; set; }
        public int Trigger { get; set; }
        public string TriggerCommand { get; set; }
        public Guid TriggerVal { get; set; }
        public bool WalkingAnimation { get; set; } = true;

        public EventPage()
        {
            MovementType = 0;
            MovementSpeed = 2;
            MovementFreq = 2;
            Passable = false;
            Layer = 1;
            Trigger = 0;
            HideName = false;
            CommandLists.Add(Guid.NewGuid(),new List<EventCommand>());
        }

        [JsonConstructor]
        public EventPage(int ignoreThis)
        {
            MovementType = 0;
            MovementSpeed = 2;
            MovementFreq = 2;
            Passable = false;
            Layer = 1;
            Trigger = 0;
            HideName = false;
        }
    }
}