using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.General;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{

    internal sealed class CpsCommand : ServerCommand
    {

        public CpsCommand() : base(
            Strings.Commands.Cps
        )
        {
        }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            //else if (operation == Strings.Commands.Arguments.CpsStatus)
            //{
            //    Console.WriteLine(Globals.CpsLock
            //        ? Strings.Commandoutput.cpslocked
            //        : Strings.Commandoutput.cpsunlocked);
            //}
            // TODO: Rethink what messages we want to display here. Confirmation of the change is ideal. To reuse code we effectively don't need to really handle status.
            Console.WriteLine(Globals.Cps);
        }

    }

}
