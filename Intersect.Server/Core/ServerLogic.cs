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
