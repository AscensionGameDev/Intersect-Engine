using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Localization;
using Intersect.Server.Networking.Helpers;

namespace Intersect.Server.Core.Commands
{

    internal sealed partial class NetDebugCommand : ServerCommand
    {

        public NetDebugCommand() : base(Strings.Commands.NetDebug)
        {
        }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            NetDebug.GenerateDebugFile();
        }

    }

}
