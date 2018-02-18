using System.Collections.Generic;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_7.Intersect_Convert_Lib.GameObjects.Events
{
    public class CommandList
    {
        public List<EventCommand> Commands = new List<EventCommand>();

        public CommandList()
        {
        }

        public CommandList(Upgrade_10.Intersect_Convert_Lib.ByteBuffer myBuffer)
        {
            var y = myBuffer.ReadInteger();
            for (var i = 0; i < y; i++)
            {
                Commands.Add(new EventCommand());
                Commands[i].Load(myBuffer);
            }
        }

        public void WriteBytes(Upgrade_10.Intersect_Convert_Lib.ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger(Commands.Count);
            foreach (var command in Commands)
            {
                command.Save(myBuffer);
            }
        }
    }
}