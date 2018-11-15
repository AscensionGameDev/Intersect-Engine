using System;
using System.Collections.Generic;
using Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects.Events
{
    public class EventCommand
    {
        public int[] Ints = new int[6];
        public EventMoveRoute Route;
        public string[] Strs = new string[6];
        public EventCommandType Type;

        public EventCommand()
        {
            for (var i = 0; i < 6; i++)
            {
                Strs[i] = "";
                Ints[i] = 0;
            }
        }

        public void Load(Upgrade_10.Intersect_Convert_Lib.ByteBuffer myBuffer)
        {
            Type = (EventCommandType) myBuffer.ReadInteger();
            for (var x = 0; x < 6; x++)
            {
                Strs[x] = myBuffer.ReadString();
                Ints[x] = myBuffer.ReadInteger();
            }
            if (Type == EventCommandType.SetMoveRoute)
            {
                Route = new EventMoveRoute();
                Route.Load(myBuffer);
            }
        }

        public void Save(Upgrade_10.Intersect_Convert_Lib.ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger((int) Type);
            for (var x = 0; x < 6; x++)
            {
                myBuffer.WriteString(Strs[x]);
                myBuffer.WriteInteger(Ints[x]);
            }
            if (Type == EventCommandType.SetMoveRoute)
            {
                Route.Save(myBuffer);
            }
        }
    }
}