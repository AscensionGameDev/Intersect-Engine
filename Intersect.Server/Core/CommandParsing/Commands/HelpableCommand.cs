using Intersect.Core;
using Intersect.Extensions;
using Intersect.Localization;
using Intersect.Server.Core.CommandParsing.Arguments;

using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Commands
{

    public interface IHelpableCommand : ICommand
    {

        [NotNull]
        HelpArgument Help { get; }

    }

    public abstract class HelpableCommand<TContext> : Command<TContext>, IHelpableCommand
        where TContext : IApplicationContext
    {

        protected HelpableCommand([NotNull] LocaleCommand localization, [NotNull] params ICommandArgument[] arguments) :
            base(localization, arguments.Prepend(new HelpArgument()))
        {
        }

        public HelpArgument Help => FindArgumentOrThrow<HelpArgument>();

        public static bool RequiredIfNotHelp(ParserContext context)
        {
            if (!(context.Command is HelpableCommand<TContext> command))
            {
                return false;
            }

            if (context.Parsed == null)
            {
                return false;
            }

            return !(context.Parsed.TryGetValue(command.Help, out var value) && (!value?.IsImplicit ?? true));
        }

        protected override void Handle(TContext context, ParserResult result)
        {
            if (result.Find(Help))
            {
                return;
            }

            HandleValue(context, result);
        }

        protected abstract void HandleValue([NotNull] TContext context, [NotNull] ParserResult result);

    }

}
