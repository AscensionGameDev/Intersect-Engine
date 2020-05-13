using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Intersect.GameObjects.Events
{

    public class EventMoveRoute
    {
        public List<MoveRouteAction> Actions { get; set; } = new List<MoveRouteAction>();

        public bool IgnoreIfBlocked { get; set; }

        public bool RepeatRoute { get; set; }

        public Guid Target { get; set; }

        //Temp Values
        [JsonIgnore]
        public bool Complete { get; set; }

        [JsonIgnore]
        public int ActionIndex { get; set; }

        public void CopyFrom(EventMoveRoute route)
        {
            Target = route.Target;
            Complete = false;
            ActionIndex = 0;
            IgnoreIfBlocked = route.IgnoreIfBlocked;
            RepeatRoute = route.RepeatRoute;
            Actions.Clear();
            foreach (var action in route.Actions)
            {
                Actions.Add(action.Copy());
            }
        }

    }

}
