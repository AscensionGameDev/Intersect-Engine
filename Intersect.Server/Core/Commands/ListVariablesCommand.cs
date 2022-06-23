using System;
using System.Linq;

using Intersect.Enums;
using Intersect.Extensions;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Database.GameData;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{
    internal sealed partial class ListVariablesCommand : ServerCommand
    {
        public ListVariablesCommand() : base(
            Strings.Commands.ListVariables,
            new VariableArgument<int>(Strings.Commands.Arguments.Page, positional: true, defaultValue: 0),
            new VariableArgument<int>(Strings.Commands.Arguments.PageSize, positional: true, defaultValue: 10)
        )
        {
        }

        private VariableArgument<int> Page => FindArgument<VariableArgument<int>>(0);
        private VariableArgument<int> PageSize => FindArgument<VariableArgument<int>>(0);

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var page = Math.Max(0, result.Find(Page));
            var pageSize = Math.Min(Math.Max(10, result.Find(PageSize)), 100);

            var variables = GameContext.Queries.ServerVariables(page, pageSize).ToList();

            if (variables.Count < 1)
            {
                Console.WriteLine(Strings.Commandoutput.VariableListEmpty);
                return;
            }

            string padding = new string(' ', variables.Aggregate(0, (size, variable) => Math.Max(size, variable.Name.Length)));

            foreach (var variable in variables)
            {
                string formattedValue = variable.Value.Value?.ToString();

                if (variable.Value.Type == VariableDataTypes.String)
                {
                    formattedValue = $"\"{variable.Value.String}\"";
                }

                Console.WriteLine(
                    Strings.Commandoutput.VariablePrint.ToString(
                        variable.Id,
                        (padding + variable.Name).Slice(-padding.Length),
                        formattedValue
                    )
                );
            }
        }
    }
}
