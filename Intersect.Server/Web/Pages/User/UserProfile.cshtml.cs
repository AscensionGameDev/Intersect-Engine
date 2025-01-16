using Intersect.Server.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Intersect.Server.Web.Pages.User;

public partial class UserProfileModel : PageModel
{
    private readonly IOptions<TokenGenerationOptions> _tokenGenerationOptions;

    public UserProfileModel(IOptions<TokenGenerationOptions> tokenGenerationOptions)
    {
        _tokenGenerationOptions = tokenGenerationOptions;
    }

    [FromRoute]
    public string Username { get; set; }

    public Database.PlayerData.User? ViewedUser { get; set; }

    public async void OnGet()
    {
        ViewedUser = Database.PlayerData.User.Find(Username);
    }
}