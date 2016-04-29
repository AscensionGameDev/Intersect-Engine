using System.Collections.Generic;

namespace Intersect_Library.GameObjects.Events
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
