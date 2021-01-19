using System;

using Intersect.Core.ExperimentalFeatures;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Core.ExperimentalFeatures;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{

    internal class ExperimentsCommand : TargettedCommand<IExperimentalFlag>
    {

        public ExperimentsCommand() : base(
            Strings.Commands.Experiments, Strings.Commands.Arguments.TargetExperimentalFeature,
            new VariableArgument<bool>(Strings.Commands.Arguments.EnablementBoolean, positional: true)
        )
        {
        }

        private VariableArgument<bool> Enablement => FindArgumentOrThrow<VariableArgument<bool>>();

        protected override IExperimentalFlag FindTarget(ServerContext context, ParserResult result, string targetName)
        {
            if (Guid.TryParse(targetName, out var flagId) && Experiments.Instance.TryGet(flagId, out var flag))
            {
                return flag;
            }

            if (!string.IsNullOrWhiteSpace(targetName) && Experiments.Instance.TryGet(targetName, out flag))
            {
                return flag;
            }

            Console.WriteLine($@"    {Strings.Commands.ExperimentalFlagNotFound.ToString(targetName)}");

            return default(IExperimentalFlag);
        }

        protected override void HandleTarget(ServerContext context, ParserResult result, IExperimentalFlag target)
        {
            if (target == default(IExperimentalFlag))
            {
                return;
            }

            if (result.TryFind(Enablement, out var enablement, allowImplicit: false))
            {
                if (!Experiments.Instance.TrySet(target, enablement))
                {
                    throw new Exception(@"Unknown error occurred.");
                }
            }
            else
            {
                enablement = target.Enabled;
            }

            var statusString = enablement ? Strings.General.EnabledLowerCase : Strings.General.DisabledLowerCase;
            var enabledString = Strings.Commandoutput.ExperimentalFeatureEnablement.ToString(target.Name, statusString);

            Console.WriteLine($@"    {enabledString}");
        }

    }

}
