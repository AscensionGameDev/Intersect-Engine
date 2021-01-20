using Intersect.Extensions;
using Intersect.Localization;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;

namespace Intersect.Server.Core.Commands
{

    internal abstract class TargettedCommand<TTarget> : ServerCommand
    {

        protected TargettedCommand(
            LocaleCommand command,
            LocaleArgument argument,
            params ICommandArgument[] arguments
        ) : base(command, arguments.Prepend(new VariableArgument<string>(argument, RequiredIfNotHelp, true)))
        {
        }

        protected VariableArgument<string> Target => FindArgumentOrThrow<VariableArgument<string>>();

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var target = FindTarget(context, result, result.Find(Target));
            HandleTarget(context, result, target);
        }

        protected abstract TTarget FindTarget(
            ServerContext context,
            ParserResult result,
            string targetName
        );

        protected abstract void HandleTarget(
            ServerContext context,
            ParserResult result,
            TTarget target
        );

    }

}
