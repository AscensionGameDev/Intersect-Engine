using Intersect.Network.Packets.Server;
using Intersect.Server.Entities;
using System.Collections.Generic;

namespace Intersect.Server.Maps
{
    public class MapAnimations
    {
        public List<PlayAnimationPacket> mAnimations = new List<PlayAnimationPacket>();

        public void Add(PlayAnimationPacket pkt)
        {
            lock (mAnimations)
            {
                mAnimations.Add(pkt);
            }
        }

        public void SendPackets(HashSet<Player> nearbyPlayers)
        {
            if (mAnimations.Count > 0)
            {
                lock (mAnimations)
                {
                    var pkt = new PlayAnimationPackets()
                    {
                        Packets = mAnimations.ToArray()
                    };
                    foreach (var plyr in nearbyPlayers)
                    {
                        plyr.SendPacket(pkt, Network.TransmissionMode.Any);
                    }
                    mAnimations.Clear();
                }
            }
        }
    }
}
