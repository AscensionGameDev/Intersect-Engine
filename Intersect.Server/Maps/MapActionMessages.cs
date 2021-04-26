using Intersect.Network.Packets.Server;
using Intersect.Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.Maps
{
    public class MapActionMessages
    {
        public List<ActionMsgPacket> mActionMessages = new List<ActionMsgPacket>();

        public void Add(ActionMsgPacket pkt)
        {
            lock (mActionMessages)
            {
                mActionMessages.Add(pkt);
            }
        }

        public void SendPackets(HashSet<Player> nearbyPlayers)
        {
            if (mActionMessages.Count > 0)
            {
                lock (mActionMessages)
                {
                    var pkt = new ActionMsgPackets()
                    {
                        Packets = mActionMessages.ToArray()
                    };
                    foreach (var plyr in nearbyPlayers)
                    {
                        plyr.SendPacket(pkt);
                    }
                    mActionMessages.Clear();
                }
            }
        }
    }
}
