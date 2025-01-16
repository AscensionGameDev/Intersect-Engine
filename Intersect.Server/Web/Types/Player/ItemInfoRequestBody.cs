namespace Intersect.Server.Web.Types.Player;

public partial struct ItemInfoRequestBody
{
    public Guid ItemId { get; set; }

    public int Quantity { get; set; }

    public bool BankOverflow { get; set; }
}
