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
            Strings.Commands.SetVar,
            new VariableArgument<string>(Strings.Commands.Arguments.VarGUID, RequiredIfNotHelp, true),
            new VariableArgument<string>(Strings.Commands.Arguments.VarValue, RequiredIfNotHelp, true)
        )
        {
        }

        private VariableArgument<string> ServerVarID => FindArgument<VariableArgument<string>>(0);
        private VariableArgument<string> ServerVarValue => FindArgument<VariableArgument<string>>(1);

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var serverVar = result.Find(ServerVarID);
            var varVal = result.Find(ServerVarValue);

            if (!string.IsNullOrEmpty(serverVar) && !string.IsNullOrEmpty(varVal))
            {
                if (!Guid.TryParse(serverVar, out Guid parsedServerVar))
                {
                    Console.WriteLine(Strings.Commandoutput.variablenotfound.ToString(serverVar));
                    return;
                }

                var variable = GameContext.Queries.ServerVariableById(parsedServerVar);
                if (variable == null)
                {
                    Console.WriteLine(Strings.Commandoutput.variablenotfound.ToString(serverVar));
                    return;
                }

                var oldVal = variable.Value?.Value;

                try
                {
                    switch (variable.Value.Type)
                    {
                        case VariableDataTypes.Boolean:
                            variable.Value.Value = bool.Parse(varVal);
                            break;
                        case VariableDataTypes.Integer:
                        case VariableDataTypes.Number:
                            variable.Value.Value = int.Parse(varVal);
                            break;
                        case VariableDataTypes.String:
                            variable.Value.Value = varVal.ToString();
                            break;
                    }
                }
                catch (Exception exception)
                {
                    // In the event there is a type mismatch, simply log it to the user and reset the server var to what it was before, then return before doing anything else
                    Console.WriteLine(Strings.Commandoutput.variableexception.ToString(serverVar, exception.Message));
                    variable.Value.Value = oldVal;
                    return;
                }

                Player.StartCommonEventsWithTriggerForAll(Enums.CommonEventTrigger.ServerVariableChange, "", serverVar);
                DbInterface.UpdatedServerVariables.AddOrUpdate(variable.Id, variable, (key, oldValue) => variable);
                Console.WriteLine(Strings.Commandoutput.variablechanged.ToString(serverVar, varVal, oldVal));
            }
            else
            {
                Console.WriteLine(Strings.Commandoutput.variablenotfound.ToString(serverVar));
            }
        }
    }
}
