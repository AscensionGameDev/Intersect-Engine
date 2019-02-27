using Intersect.Localization;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{
    internal abstract class TargettedCommand<TTarget> : ServerCommand
    {
        [NotNull]
        private MessageArgument Message => FindArgumentOrThrow<MessageArgument>();

        protected TargettedCommand(
            [NotNull] LocaleCommand command,
            [NotNull] LocaleArgument argument
        ) : base(
            command,
            new MessageArgument(argument, RequiredIfNotHelp, true)
        )
        {
        }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var target = FindTarget(context, result.Find(Message));
            HandleTarget(context, target);
        }

        protected abstract TTarget FindTarget([NotNull] ServerContext context, [CanBeNull] string targetName);

        protected abstract void HandleTarget([NotNull] ServerContext context, [CanBeNull] TTarget target);
    }
}