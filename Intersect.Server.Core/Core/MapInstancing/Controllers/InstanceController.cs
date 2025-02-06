using Intersect.Server.Entities;

namespace Intersect.Server.Core.MapInstancing.Controllers;
public class InstanceController
{
    public Guid InstanceId { get; set; }

    public HashSet<Player> Players { get; set; } = new();

    public InstanceController(Guid instanceId, Player creator)
    {
        InstanceId = instanceId;
        AddPlayer(creator);
    }

    public void AddPlayer(Player player)
    {
        if (player == null || !player.IsOnline)
        {
            return;
        }
        Players.Add(player);
    }

    public void RemovePlayer(Guid playerId)
    {
        Players.RemoveWhere(pl => pl.Id == playerId);
    }
}
