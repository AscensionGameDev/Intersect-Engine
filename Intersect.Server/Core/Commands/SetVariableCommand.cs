using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Localization;
using Intersect.Server.Database;
using Intersect.Enums;
using System;
using Intersect.Server.Database.GameData;
using Intersect.Server.Entities;

namespace Intersect.Server.Core.Commands
{
    internal sealed class SetVariableCommand : ServerCommand
    {
        public SetVariableCommand() : base(
            Strings.Commands.SetVariable,
            new VariableArgument<string>(Strings.Commands.Arguments.VariableId, RequiredIfNotHelp, true),
            new VariableArgument<string>(Strings.Commands.Arguments.VariableValue, RequiredIfNotHelp, true)
        )
        {
        }

        private VariableArgument<string> ServerVariableId => FindArgument<VariableArgument<string>>(0);
        private VariableArgument<string> ServerVariableValue => FindArgument<VariableArgument<string>>(1);

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var serverVariable = result.Find(ServerVariableId);
            var serverVariableValue = result.Find(ServerVariableValue);

            if (string.IsNullOrEmpty(serverVariable))
            {
                throw new ArgumentNullException("No server variable specified.");
            }

            if (string.IsNullOrEmpty(serverVariableValue))
            {
                throw new ArgumentNullException($"No value specified for server variable with id {ServerVariableId}");
            }

            if (!Guid.TryParse(serverVariable, out Guid parsedServerVar))
            {
                throw new ArgumentException($"{ServerVariableId} is not a valid server variable id");
            }

            var variable = GameContext.Queries.ServerVariableById(parsedServerVar);
            if (variable == default)
            {
                Console.WriteLine(Strings.Commandoutput.VariableNotFound.ToString(serverVariable));
                return;
            }

            var previousServerVariableValue = variable.Value?.Value;

            switch (variable.Value.Type)
            {
                case VariableDataTypes.Boolean:
                    variable.Value.Value = bool.Parse(serverVariableValue);
                    break;
                case VariableDataTypes.Integer:
                case VariableDataTypes.Number:
                    variable.Value.Value = int.Parse(serverVariableValue);
                    break;
                case VariableDataTypes.String:
                    variable.Value.Value = serverVariableValue.ToString();
                    break;
            }

            Player.StartCommonEventsWithTriggerForAll(CommonEventTrigger.ServerVariableChange, string.Empty, serverVariable);
            DbInterface.UpdatedServerVariables.AddOrUpdate(variable.Id, variable, (key, oldValue) => variable);
            Console.WriteLine(Strings.Commandoutput.VariableChanged.ToString(serverVariable, serverVariableValue, previousServerVariableValue));
        }
    }
}
