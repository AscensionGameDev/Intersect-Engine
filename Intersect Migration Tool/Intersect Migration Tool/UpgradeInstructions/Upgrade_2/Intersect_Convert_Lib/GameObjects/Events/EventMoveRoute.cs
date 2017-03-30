using System.Collections.Generic;
using Intersect;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_2.Intersect_Convert_Lib.GameObjects.Events
{
    public class EventMoveRoute
    {
        public int ActionIndex = 0;
        public List<MoveRouteAction> Actions = new List<MoveRouteAction>();
        public bool Complete = false;
        public bool IgnoreIfBlocked = false;
        public bool RepeatRoute = false;
        public int Target = -1;

        public EventMoveRoute()
        {
        }

        public void Load(ByteBuffer myBuffer)
        {
            Target = myBuffer.ReadInteger();
            if (myBuffer.ReadByte() == 1)
            {
                IgnoreIfBlocked = true;
            }
            else
            {
                IgnoreIfBlocked = false;
            }
            if (myBuffer.ReadByte() == 1)
            {
                RepeatRoute = true;
            }
            else
            {
                RepeatRoute = false;
            }
            int actionCount = myBuffer.ReadInteger();
            for (int i = 0; i < actionCount; i++)
            {
                Actions.Add(new MoveRouteAction());
                Actions[Actions.Count - 1].Load(myBuffer);
            }
        }

        public void Save(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger(Target);
            if (IgnoreIfBlocked)
            {
                myBuffer.WriteByte(1);
            }
            else
            {
                myBuffer.WriteByte(0);
            }
            if (RepeatRoute)
            {
                myBuffer.WriteByte(1);
            }
            else
            {
                myBuffer.WriteByte(0);
            }
            myBuffer.WriteInteger(Actions.Count);
            foreach (MoveRouteAction action in Actions)
            {
                action.Save(myBuffer);
            }
        }

        public void CopyFrom(EventMoveRoute route)
        {
            Target = route.Target;
            Complete = false;
            ActionIndex = 0;
            IgnoreIfBlocked = route.IgnoreIfBlocked;
            RepeatRoute = route.RepeatRoute;
            Actions.Clear();
            foreach (MoveRouteAction action in route.Actions)
            {
                Actions.Add(action.Copy());
            }
        }
    }
}