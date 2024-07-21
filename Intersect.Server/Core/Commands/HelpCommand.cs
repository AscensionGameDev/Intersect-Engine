﻿using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{

    internal sealed partial class HelpCommand : ServerCommand
    {

        public HelpCommand(ParserSettings parserSettings) : base(Strings.Commands.Help)
        {
            ParserSettings = parserSettings;
        }

        private ParserSettings ParserSettings { get; }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            Console.WriteLine(@"    " + Strings.Commandoutput.HelpHeader);

            Strings.Commands.CommandList.ForEach(
                command => { Console.WriteLine($@"    {command?.Name,-20} - {command?.Help}"); }
            );

            var helpArgument = Help.HasShortName
                ? ParserSettings.PrefixShort + Help.ShortName.ToString()
                : ParserSettings.PrefixLong + Help.Name;

            Console.WriteLine($@"    {Strings.Commandoutput.HelpFooter.ToString(helpArgument)}");
        }

    }

}
