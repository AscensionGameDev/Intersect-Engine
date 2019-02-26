using System.Linq;
using Intersect.Core;
using Intersect.Localization;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Localization;
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
            [CanBeNull] params ICommandArgument[] arguments
            ) : base(
            localization,
            new [] { new HelpArgument() }.Concat(arguments ?? new ICommandArgument[0]).ToArray()
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
                Console.WriteLine(@"    " + Localization.Usage.ToString(Strings.Commands.commandinfo));
                Console.WriteLine(@"    " + Localization.Description);
                return;
            }

            HandleValue(context, result);
        }

        protected abstract void HandleValue([NotNull] TContext context, [NotNull] ParserResult result);
    }
}