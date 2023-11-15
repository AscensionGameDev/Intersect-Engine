using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Server.Entities;

namespace Intersect.Server.Core.MapInstancing.Controllers;
public class InstanceController
{
    Guid InstanceId { get; set; }

    public List<Player> Players { get; set; } = new List<Player>();

    public int PlayerCount => Players.Count;

    public List<Guid> PlayerIds => Players.Select(pl => pl.Id).ToList();

    public InstanceController(Guid instanceId, Player creator)
    {
        InstanceId = instanceId;
        AddPlayer(creator);
    }

    public void AddPlayer(Player player)
    {
        if (player == null || !player.Online)
        {
            return;
        }

        if (PlayerIds.Contains(player.Id))
        {
            return;
        }

        Logging.Log.Debug($"{player.Name} has entered instance controller {InstanceId}!");
        Players.Add(player);
    }

    public void RemovePlayer(Guid playerId)
    {
        Logging.Log.Debug($"{playerId} has left instance controller {InstanceId}...");
        Players.RemoveAll(pl => pl.Id == playerId);
    }
}
