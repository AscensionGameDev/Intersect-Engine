using Intersect.Server.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Intersect.Server.Web.Pages.Shared.Players;

public partial class PlayerCardModel : PageModel
{
    public Player? Player { get; set; }

    public long? Rank { get; set; }

    public long? RankScale { get; set; }
}