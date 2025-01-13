namespace Intersect.Server.Web.RestApi.Types.Player;

public partial struct ItemInfo
{
    public Guid ItemId { get; set; }

    public int Quantity { get; set; }

    public bool BankOverflow { get; set; }
}
