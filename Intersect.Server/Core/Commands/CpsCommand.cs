using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.General;
using Intersect.Server.Localization;
using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{
    internal sealed class CpsCommand : ServerCommand
    {
        [NotNull]
        private EnumArgument<string> Operation => FindArgumentOrThrow<EnumArgument<string>>();

        public CpsCommand() : base(
            Strings.Commands.Cps,
            new EnumArgument<string>(
                Strings.Commands.Arguments.CpsOperation,
                RequiredIfNotHelp,
                true,
                Strings.Commands.Arguments.CpsLock.ToString(),
                Strings.Commands.Arguments.CpsStatus.ToString(),
                Strings.Commands.Arguments.CpsUnlock.ToString()
            )
        )
        {
        }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var operation = result.Find(Operation);
            if (operation == Strings.Commands.cpslock)
            {
                Globals.CpsLock = true;
            }
            else if (operation == Strings.Commands.cpsunlock)
            {
                Globals.CpsLock = false;
            }
            //else if (operation == Strings.Commands.cpsstatus)
            //{
            //    Console.WriteLine(Globals.CpsLock
            //        ? Strings.Commandoutput.cpslocked
            //        : Strings.Commandoutput.cpsunlocked);
            //}
            // TODO: Rethink what messages we want to display here. Confirmation of the change is ideal. To reuse code we effectively don't need to really handle status.
            Console.WriteLine(Globals.CpsLock
                ? Strings.Commandoutput.cpslocked
                : Strings.Commandoutput.cpsunlocked);
        }
    }
}