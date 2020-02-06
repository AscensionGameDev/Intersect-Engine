using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Threading;

namespace Intersect.Server.Core
{
    internal sealed class ServerLogic : Threaded
    {
        public ServerLogic() : base("ServerLogic")
        {

        }
        protected override void ThreadStart()
        {
            ServerLoop.RunServerLoop();
        }
    }
}
