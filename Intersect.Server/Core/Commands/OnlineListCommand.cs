using System.Linq;

using Intersect.Server.Core.CommandParsing;
using Intersect.Server.General;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{

    internal sealed class OnlineListCommand : ServerCommand
    {

        public OnlineListCommand() : base(Strings.Commands.OnlineList)
        {
        }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var bufferWidth = (Console.BufferWidth == 0 ? 100 : Console.BufferWidth) - 1;
            var columnScale = bufferWidth / 66.0;
            var columnWidths = new[] {10, 28, 28}.Select(width => (int) (width * columnScale)).ToArray();
            columnWidths[0] += bufferWidth - columnWidths.Sum();
            columnWidths[1] -= 2;
            columnWidths[2] -= 2;
            var formatLine = string.Join("| ", columnWidths.Select((width, column) => $"{{{column},{-width}}}"));

            Console.WriteLine(
                formatLine, Strings.Commandoutput.listid, Strings.Commandoutput.listaccount,
                Strings.Commandoutput.listcharacter
            );

            Console.WriteLine(new string('-', bufferWidth));

            for (var i = 0; i < Globals.Clients.Count; i++)
            {
                if (Globals.Clients[i] == null)
                {
                    continue;
                }

                var name = Globals.Clients[i].Entity != null ? Globals.Clients[i].Entity.Name : "";
                Console.WriteLine(formatLine, "#" + i, Globals.Clients[i].Name, name);
            }
        }

    }

}
