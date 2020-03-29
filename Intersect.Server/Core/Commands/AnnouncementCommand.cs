using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{

    internal sealed class AnnouncementCommand : ServerCommand
    {

        public AnnouncementCommand() : base(
            Strings.Commands.Announcement,
            new VariableArgument<string>(Strings.Commands.Arguments.AnnouncementMessage, RequiredIfNotHelp, true)
        )
        {
        }

        [NotNull]
        private VariableArgument<string> Message => FindArgumentOrThrow<VariableArgument<string>>();

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            PacketSender.SendGlobalMsg(result.Find(Message));
        }

    }

}
