using System.Collections.Generic;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_6.Intersect_Convert_Lib.GameObjects.Events
{
    public class CommandList
    {
        public List<EventCommand> Commands = new List<EventCommand>();

        public CommandList()
        {
        }

        public CommandList(ByteBuffer myBuffer)
        {
            var y = myBuffer.ReadInteger();
            for (var i = 0; i < y; i++)
            {
                Commands.Add(new EventCommand());
                Commands[i].Load(myBuffer);
            }
        }

        public void WriteBytes(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger(Commands.Count);
            foreach (var command in Commands)
            {
                command.Save(myBuffer);
            }
        }
    }
}