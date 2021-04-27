using Intersect.Network.Packets.Server;
using Intersect.Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.Maps
{
    public class MapEntityMovements
    {
        private Dictionary<Guid, List<EntityMovePacket>> mMovements = new Dictionary<Guid, List<EntityMovePacket>>();

        public void Add(Entity en, bool correction, Player forPlayer = null)
        {
            lock (mMovements)
            {
                var id = forPlayer?.Id ?? Guid.Empty;
                if (!mMovements.ContainsKey(id))
                {
                    mMovements.Add(id, new List<EntityMovePacket>());
                }
                mMovements[id].Add(new EntityMovePacket(en.Id, en.GetEntityType(), en.MapId, (byte)en.X, (byte)en.Y, (byte)en.Dir, correction));
            }
        }

        public void SendPackets(HashSet<Player> nearbyPlayers)
        {
            if (mMovements.Count > 0)
            {
                lock (mMovements)
                {
                    var globalMovements = mMovements.ContainsKey(Guid.Empty) ? mMovements[Guid.Empty].ToArray() : null;
                    foreach (var player in nearbyPlayers)
                    {
                        var localMovements = mMovements.ContainsKey(player.Id) ? mMovements[player.Id].ToArray() : null;
                        if (globalMovements != null || localMovements != null)
                        {
                            var pkt = new EntityMovementPackets()
                            {
                                GlobalMovements = globalMovements,
                                LocalMovements = localMovements
                            };
                            player.SendPacket(pkt);
                        }
                    }
                    mMovements.Clear();
                }
            }
        }
    }
}
