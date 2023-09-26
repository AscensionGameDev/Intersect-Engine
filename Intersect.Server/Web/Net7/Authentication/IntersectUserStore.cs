using System.Diagnostics;
using System.Security.Claims;
using Intersect.Server.Database.PlayerData;
using Microsoft.AspNetCore.Identity;

namespace Intersect.Server.Web.Authentication;

public class IntersectRoleStore : IRoleStore<UserRole>
{
    public void Dispose()
    {
    }

    public Task<IdentityResult> CreateAsync(UserRole role, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(UserRole role, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<IdentityResult> DeleteAsync(UserRole role, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<string> GetRoleIdAsync(UserRole role, CancellationToken cancellationToken)
    {
        Debugger.Break();
        return Task.FromResult(role.Id.ToString());
    }

    public Task<string> GetRoleNameAsync(UserRole role, CancellationToken cancellationToken)
    {
        Debugger.Break();
        return Task.FromResult(role.Name);
    }

    public Task SetRoleNameAsync(UserRole role, string roleName, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<string> GetNormalizedRoleNameAsync(UserRole role, CancellationToken cancellationToken)
    {
        Debugger.Break();
        return Task.FromResult(role.Name);
    }

    public Task SetNormalizedRoleNameAsync(UserRole role, string normalizedName, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<UserRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<UserRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }
}

public class IntersectUserStore :
    IUserLoginStore<User>,
    IUserClaimStore<User>,
    IUserPasswordStore<User>,
    IUserRoleStore<User>,
    // IUserSecurityStampStore<User>,
    IUserEmailStore<User>,
    // IUserLockoutStore<User>,
    // IUserPhoneNumberStore<User>,
    IQueryableUserStore<User>,
    // IUserTwoFactorStore<User>,
    IUserAuthenticationTokenStore<User>,
    // IUserAuthenticatorKeyStore<User>,
    // IUserTwoFactorRecoveryCodeStore<User>,
    IProtectedUserStore<User>
{
    public IQueryable<User> Users { get; }

    public void Dispose()
    {
    }

    public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.Id.ToString());

    public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.Name);

    public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.Name);

    public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return Task.FromResult(Guid.TryParse(userId, out var id) ? User.FindById(id) : default);
    }

    public Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return Task.FromResult(User.Find(normalizedUserName));
    }

    public Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult<IList<Claim>>(user.Claims);
    }

    public Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        Debugger.Break();
        user.Claims.AddRange(claims);
        return Task.CompletedTask;
    }

    public Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        Debugger.Break();
        user.Claims.Remove(claim);
        user.Claims.Add(newClaim);
        return Task.CompletedTask;
    }

    public Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        Debugger.Break();
        foreach (var claim in claims)
        {
            user.Claims.Remove(claim);
        }
        return Task.CompletedTask;
    }

    public Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.Password);

    public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken) => Task.FromResult(true);

    public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
    {
        // TODO
        return Task.FromResult(true);
    }

    public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return Task.FromResult(User.FindFromNameOrEmail(normalizedEmail));
    }

    public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.Email);

    public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task SetTokenAsync(
        User user,
        string loginProvider,
        string name,
        string value,
        CancellationToken cancellationToken
    )
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task RemoveTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task<string> GetTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        user.Claims.Add(new Claim(ClaimTypes.Role, roleName));
        return Task.CompletedTask;
    }

    public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        var claims = user.Claims.Where(claim => claim.Type == ClaimTypes.Role && claim.Value == roleName).ToArray();
        foreach (var claim in claims)
        {
            user.Claims.Remove(claim);
        }

        return Task.CompletedTask;
    }

    public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult<IList<string>>(user.Claims.Where(claim => claim.Type == ClaimTypes.Role).Select(claim => claim.Value).Distinct().ToList());
    }

    public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Claims.Any(claim => claim.Type == ClaimTypes.Role && claim.Value == roleName));
    }

    public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }
}