using Intersect.Server.Core.Arguments;
using Intersect.Server.Localization;
using JetBrains.Annotations;
using System;

namespace Intersect.Server.Core.Commands
{
    internal sealed class ExitCommand : ServerCommand
    {
        public ExitCommand() : base(
            Strings.Commands.Exit
        )
        {
        }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            context.RequestShutdown();
        }
    }
}