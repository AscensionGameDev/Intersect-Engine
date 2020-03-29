using Intersect.Extensions;
using Intersect.Localization;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;

using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{

    internal abstract class TargettedCommand<TTarget> : ServerCommand
    {

        protected TargettedCommand(
            [NotNull] LocaleCommand command,
            [NotNull] LocaleArgument argument,
            [NotNull] params ICommandArgument[] arguments
        ) : base(command, arguments.Prepend(new VariableArgument<string>(argument, RequiredIfNotHelp, true)))
        {
        }

        [NotNull]
        protected VariableArgument<string> Target => FindArgumentOrThrow<VariableArgument<string>>();

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var target = FindTarget(context, result, result.Find(Target));
            HandleTarget(context, result, target);
        }

        protected abstract TTarget FindTarget(
            [NotNull] ServerContext context,
            [NotNull] ParserResult result,
            [CanBeNull] string targetName
        );

        protected abstract void HandleTarget(
            [NotNull] ServerContext context,
            [NotNull] ParserResult result,
            [CanBeNull] TTarget target
        );

    }

}
