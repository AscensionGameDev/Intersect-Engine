using System;

using Intersect.Core.ExperimentalFeatures;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Core.ExperimentalFeatures;
using Intersect.Server.Localization;

using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{
    internal class ExperimentsCommand : TargettedCommand<ExperimentalFlag>
    {
        [NotNull]
        private VariableArgument<bool> Enablement => FindArgumentOrThrow<VariableArgument<bool>>();

        public ExperimentsCommand() : base(
            Strings.Commands.Experiments, Strings.Commands.Arguments.TargetExperimentalFeature,
            new VariableArgument<bool>(Strings.Commands.Arguments.EnablementBoolean, positional: true)
        )
        {
        }

        protected override ExperimentalFlag FindTarget(ServerContext context, ParserResult result, string targetName)
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
            return default(ExperimentalFlag);
        }

        protected override void HandleTarget(ServerContext context, ParserResult result, ExperimentalFlag target)
        {
            if (target == default(ExperimentalFlag))
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
            var enabledString =
                Strings.Commandoutput.ExperimentalFeatureEnablement.ToString(target.Name, statusString);

            Console.WriteLine($@"    {enabledString}");
        }
    }
}
