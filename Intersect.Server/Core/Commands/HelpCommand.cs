using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Localization;

using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{

    internal sealed class HelpCommand : ServerCommand
    {

        public HelpCommand([NotNull] ParserSettings parserSettings) : base(Strings.Commands.Help)
        {
            ParserSettings = parserSettings;
        }

        [NotNull]
        private ParserSettings ParserSettings { get; }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            Console.WriteLine(@"    " + Strings.Commandoutput.helpheader);

            Strings.Commands.CommandList.ForEach(
                command => { Console.WriteLine($@"    {command?.Name,-20} - {command?.Help}"); }
            );

            var helpArgument = Help.HasShortName
                ? ParserSettings.PrefixShort + Help.ShortName.ToString()
                : ParserSettings.PrefixLong + Help.Name;

            Console.WriteLine($@"    {Strings.Commandoutput.helpfooter.ToString(helpArgument)}");
        }

    }

}
