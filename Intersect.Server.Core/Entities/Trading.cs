using Intersect.Server.Database;

namespace Intersect.Server.Entities;


public partial struct Trading : IDisposable
{

    private readonly Player mPlayer;

    public bool Actively => Counterparty != null;

    public Player Counterparty;

    public bool Accepted;

    public Item[] Offer;

    public Player Requester;

    public Dictionary<Player, long> Requests;

    public Trading(Player player)
    {
        mPlayer = player;

        Accepted = false;
        Counterparty = null;
        Offer = new Item[Options.Instance.Player.MaxInventory];
        Requester = null;
        Requests = new Dictionary<Player, long>();
    }

    public void Dispose()
    {
        Offer = Array.Empty<Item>();
        Requester = null;
        Requests.Clear();
    }

}
