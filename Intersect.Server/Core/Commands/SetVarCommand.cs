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
    class SetVarCommand : ServerCommand
    {
        public SetVarCommand() : base(
            Strings.Commands.SetVariable,
            new VariableArgument<string>(Strings.Commands.Arguments.VariableId, RequiredIfNotHelp, true),
            new VariableArgument<string>(Strings.Commands.Arguments.VariableValue, RequiredIfNotHelp, true)
        )
        {
        }

        private VariableArgument<string> ServerVariableID => FindArgument<VariableArgument<string>>(0);
        private VariableArgument<string> ServerVariableValue => FindArgument<VariableArgument<string>>(1);

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var serverVariable = result.Find(ServerVariableID);
            var serverVariableValue = result.Find(ServerVariableValue);

            if (string.IsNullOrEmpty(serverVariable) || string.IsNullOrEmpty(serverVariableValue))
            {
                Console.WriteLine(Strings.Commandoutput.VariableNotFound.ToString(serverVariable));
                return;
            }

            if (!Guid.TryParse(serverVariable, out Guid parsedServerVar))
            {
                Console.WriteLine(Strings.Commandoutput.VariableNotFound.ToString(serverVariable));
                return;
            }

            var variable = GameContext.Queries.ServerVariableById(parsedServerVar);
            if (variable == default)
            {
                Console.WriteLine(Strings.Commandoutput.VariableNotFound.ToString(serverVariable));
                return;
            }

            var oldVal = variable.Value?.Value;

            try
            {
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

                Player.StartCommonEventsWithTriggerForAll(Enums.CommonEventTrigger.ServerVariableChange, string.Empty, serverVariable);
                DbInterface.UpdatedServerVariables.AddOrUpdate(variable.Id, variable, (key, oldValue) => variable);
                Console.WriteLine(Strings.Commandoutput.VariableChanged.ToString(serverVariable, serverVariableValue, oldVal));
            }
            catch (Exception exception)
            {
                Logging.Log.Error(exception.Message);
            }
        }
    }
}
