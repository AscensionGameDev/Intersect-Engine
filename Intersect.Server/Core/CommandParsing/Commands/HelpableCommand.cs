using Intersect.Core;
using Intersect.Extensions;
using Intersect.Localization;
using Intersect.Server.Core.CommandParsing.Arguments;
using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Commands
{
    public abstract class HelpableCommand<TContext> : Command<TContext>
        where TContext : IApplicationContext
    {
        [NotNull]
        protected HelpArgument Help => FindArgumentOrThrow<HelpArgument>();

        protected HelpableCommand(
            [NotNull] LocaleCommand localization,
            [NotNull] params ICommandArgument[] arguments
            ) : base(
            localization,
            arguments.Prepend(new HelpArgument())
        )
        {
        }

        protected static bool RequiredIfNotHelp(ParserContext context)
        {
            if (context.Command is HelpableCommand<TContext> command)
            {
                return !context.Parsed?.ContainsKey(command.Help) ?? false;
            }

            return false;
        }

        protected override void Handle(TContext context, ParserResult result)
        {
            if (result.Find(Help))
            {
                Console.WriteLine(
                    $@"    {Localization.Usage.ToString(result.Parsed.Find(Help as ICommandArgument)?.ArgumentName)}");
                Console.WriteLine($@"    {Localization.Description}");
                return;
            }

            HandleValue(context, result);
        }

        protected abstract void HandleValue([NotNull] TContext context, [NotNull] ParserResult result);
    }
}