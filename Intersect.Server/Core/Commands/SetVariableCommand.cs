using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Localization;
using Intersect.Server.Database;
using Intersect.Enums;
using System;
using Intersect.Server.Database.GameData;
using Intersect.Server.Entities;
using System.Globalization;
using Intersect.GameObjects;

namespace Intersect.Server.Core.Commands
{
    internal sealed partial class SetVariableCommand : ServerCommand
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
            var serverVariableNameOrId = result.Find(ServerVariableId);
            var rawServerVariableValue = result.Find(ServerVariableValue);

            if (string.IsNullOrEmpty(serverVariableNameOrId))
            {
                throw new ArgumentNullException(Strings.Commands.Arguments.VariableId.Name, "No server variable specified.");
            }

            if (string.IsNullOrEmpty(rawServerVariableValue))
            {
                throw new ArgumentNullException(Strings.Commands.Arguments.VariableValue.Name, $"No value specified for server variable '{serverVariableNameOrId}'");
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

            var previousServerVariableValue = variable.Value?.Value;
            string formattedPreviousValue = previousServerVariableValue?.ToString() ?? string.Empty;
            string formattedValue = default;

            switch (variable.Value.Type)
            {
                case VariableDataType.Boolean:
                    variable.Value.Boolean = bool.Parse(rawServerVariableValue);
                    break;

                case VariableDataType.Integer:
                    variable.Value.Integer = int.Parse(rawServerVariableValue, NumberStyles.Integer, CultureInfo.CurrentCulture);
                    break;

                case VariableDataType.Number:
                    variable.Value.Number = double.Parse(rawServerVariableValue, NumberStyles.Float, CultureInfo.CurrentCulture);
                    break;

                case VariableDataType.String:
                    variable.Value.String = rawServerVariableValue.ToString();
                    formattedPreviousValue = $"\"{formattedPreviousValue}\"";
                    formattedValue = $"\"{variable.Value.String}\"";
                    break;
            }

            if (formattedValue == default)
            {
                formattedValue = variable.Value?.ToString();
            }

            Player.StartCommonEventsWithTriggerForAll(CommonEventTrigger.ServerVariableChange, string.Empty, variable.Id.ToString());
            DbInterface.UpdatedServerVariables.AddOrUpdate(variable.Id, variable, (key, oldValue) => variable);
            Console.WriteLine(Strings.Commandoutput.VariableChanged.ToString(variable.Id, variable.Name, formattedValue, formattedPreviousValue));
        }
    }
}
