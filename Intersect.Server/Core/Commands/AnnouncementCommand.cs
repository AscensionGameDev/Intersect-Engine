using Intersect.Core;
using Intersect.Server.Core.Arguments;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{
    internal sealed class AnnouncementCommand : ServerCommand
    {
        [NotNull]
        private MessageArgument Message => FindArgumentOrThrow<MessageArgument>();

        public AnnouncementCommand() : base(
            Strings.Commands.Announcement,
            new MessageArgument(
                Strings.Commands.Arguments.AnnouncementsMessage,
                RequiredIfNotHelp,
                true
            )
        )
        {
        }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            PacketSender.SendGlobalMsg(result.Find(Message));
        }
    }
}