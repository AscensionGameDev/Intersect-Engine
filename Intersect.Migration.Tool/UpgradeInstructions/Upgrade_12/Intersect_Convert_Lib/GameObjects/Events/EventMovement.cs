using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Enums;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events
{
    public class EventMovement
    {
        public EventMovementType Type { get; set; } = EventMovementType.None;
        public EventMovementFrequency Frequency { get; set; } = EventMovementFrequency.Normal;
        public EventMovementSpeed Speed { get; set; } = EventMovementSpeed.Normal;
        public EventMoveRoute Route { get; set; } = new EventMoveRoute();
    }
}
