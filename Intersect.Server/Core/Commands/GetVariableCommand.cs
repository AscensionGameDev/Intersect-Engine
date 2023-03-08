using System;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Database.GameData;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{
    internal sealed partial class GetVariableCommand : ServerCommand
    {
        public GetVariableCommand() : base(
            Strings.Commands.GetVariable,
            new VariableArgument<string>(Strings.Commands.Arguments.VariableId, RequiredIfNotHelp, true)
        )
        {
        }

        private VariableArgument<string> ServerVariableId => FindArgument<VariableArgument<string>>(0);

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var serverVariableNameOrId = result.Find(ServerVariableId);

            if (string.IsNullOrEmpty(serverVariableNameOrId))
            {
                throw new ArgumentNullException(Strings.Commands.Arguments.VariableId.Name, "No server variable specified.");
            }

            ServerVariableBase variable;
            if (Guid.TryParse(serverVariableNameOrId, out Guid serverVariableId))
            {
                variable = GameContext.Queries.ServerVariableById(serverVariableId);
            }
            else
            {
                variable = GameContext.Queries.ServerVariableByName(serverVariableNameOrId);
            }

            if (variable == default)
            {
                Console.WriteLine(Strings.Commandoutput.VariableNotFound.ToString(serverVariableNameOrId));
                return;
            }

            string formattedValue = variable.Value.Value?.ToString();

            if (variable.Value.Type == VariableDataType.String)
            {
                formattedValue = $"\"{variable.Value.String}\"";
            }

            Console.WriteLine(Strings.Commandoutput.VariablePrint.ToString(variable.Id, variable.Name, formattedValue));
        }
    }
}
