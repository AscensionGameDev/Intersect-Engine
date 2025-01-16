using Intersect.Server.Collections.Sorting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Intersect.Server.Web.Pages.Shared.Players;

public partial class PlayerListModel : PageModel
{
    public string? Caption { get; set; }

    public int Count { get; set; } = 10;

    public int Page { get; set; }

    public bool ShowRank { get; set; } = true;

    public SortDirection SortDirection { get; set; } = SortDirection.Descending;
}