using System;
using Intersect.Network;
using Intersect.Threading;
using Lidgren.Network;

namespace Intersect.Client.Network
{
    public class ClientNetwork : AbstractNetwork
    {
        public new NetClient Peer => (NetClient)base.Peer;

        public ClientNetwork(NetPeerConfiguration config) : base(config, new NetClient(config))
        {
        }

        public override void Connect()
        {
            Peer.Connect(Config.LocalAddress.ToString(), Config.Port);
        }

        public override void Listen()
        {
            throw new NotImplementedException();
        }

        public override bool Send(IPacket packet)
        {
            var message = Peer.CreateMessage();
            if (!packet.Write(ref message)) throw new Exception();

            var result = Peer.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            switch (result)
            {
                case NetSendResult.Sent:
                case NetSendResult.Queued:
                    return true;

                default:
                    return false;
            }
        }

        public override bool Send(Guid guid, IPacket packet) => Send(packet);

        protected override void RegisterHandlers()
        {
            
        }

        protected override void Poll()
        {
            
        }

        protected override int CalculateNumberOfThreads() => 1;

        protected override IThreadYield CreateThreadYield()
            => new ThreadYieldNet35();
    }
}