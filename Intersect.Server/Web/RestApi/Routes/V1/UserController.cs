using System.Text.RegularExpressions;

using Intersect.Enums;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Notifications;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.RestApi.Types;
using Intersect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.RestApi.Routes.V1
{

    [Route("api/v1/users")]
    [Authorize(Roles = nameof(ApiRoles.UserQuery))]
    public sealed partial class UserController : IntersectController
    {

        [HttpPost]
        public DataPage<User> ListPost([FromBody] PagingInfo pageInfo)
        {
            var page = Math.Max(pageInfo.Page, 0);
            var pageSize = Math.Max(Math.Min(pageInfo.Count, PAGE_SIZE_MAX), PAGE_SIZE_MIN);

            var values = Database.PlayerData.User.List(null, null, SortDirection.Ascending, pageInfo.Page * pageInfo.Count, pageInfo.Count, out var total);

            return new DataPage<User>
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Count = values.Count,
                Values = values
            };
        }

        [HttpGet]
        public DataPage<User> List(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] int limit = PAGE_SIZE_MAX,
            [FromQuery] string sortBy = null,
            [FromQuery] SortDirection sortDirection = SortDirection.Ascending,
            [FromQuery] string search = null
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, PAGE_SIZE_MAX), PAGE_SIZE_MIN);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            var values = Database.PlayerData.User.List(search?.Length > 2 ? search : null, sortBy, sortDirection, page * pageSize, pageSize, out var total);

            if (limit != pageSize)
            {
                values = values.Take(limit).ToList();
            }

            return new DataPage<User>
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Count = values.Count,
                Values = values
            };
        }

        [HttpGet("{userId:guid}")]
        public object UserById(Guid userId)
        {
            if (Guid.Empty == userId)
            {
                return BadRequest($@"Invalid user id '{userId}'.");
            }

            var user = Database.PlayerData.User.FindById(userId);

            if (user == null)
            {
                return NotFound($@"No user with id {userId}.");
            }

            return user;
        }

        [HttpGet("{userName}")]
        public object UserByName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("Invalid user name.");
            }

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return NotFound($@"No user with name '{userName}'.");
            }

            return user;
        }

        [HttpPost("register")]
        public object RegisterUser([FromBody] UserInfo user)
        {
            if (string.IsNullOrEmpty(user.Username) ||
                string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Username, Email, and Password all must be provided, and not null/empty.");
            }

            if (!FieldChecking.IsWellformedEmailAddress(user.Email, Strings.Regex.email))
            {
                return BadRequest($@"Malformed email address '{user.Email}'.");
            }

            if (!FieldChecking.IsValidUsername(user.Username, Strings.Regex.username))
            {
                return BadRequest($@"Invalid username '{user.Username}'.");
            }

            if (Database.PlayerData.User.UserExists(user.Username))
            {
                return BadRequest($@"Account already exists with username '{user.Username}'.");
            }
            else
            {
                if (Database.PlayerData.User.UserExists(user.Email))
                {
                    return BadRequest($@"Account already with email '{user.Email}'.");
                }
                else
                {
                    DbInterface.CreateAccount(null, user.Username, user.Password?.ToUpperInvariant()?.Trim(), user.Email);

                    return new
                    {
                        Username = user.Username,
                        Email = user.Email,
                    };
                }
            }
        }

        [HttpDelete("{username}")]
        public object DeleteUserByUsername(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("Invalid user name.");
            }

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return NotFound($@"No user with name '{userName}'.");
            }

            foreach (var plyr in user.Players)
            {
                if (Player.FindOnline(plyr.Id) != null)
                {
                    return BadRequest($@"Cannot delete a user is currently online.");
                }
            }

            if (!user.TryDelete())
            {
                var client = Globals.ClientLookup.Values.FirstOrDefault(c => c.User.Id == user.Id);
                client?.LogAndDisconnect(default, nameof(DeleteUserByUsername));
            }

            return user;
        }

        [HttpDelete("{userId:guid}")]
        public object DeleteUserById(Guid userId)
        {
            if (Guid.Empty == userId)
            {
                return BadRequest($@"Invalid user id '{userId}'.");
            }

            var user = Database.PlayerData.User.FindById(userId);

            if (user == null)
            {
                return NotFound($@"No user with id '{userId}'.");
            }

            foreach (var plyr in user.Players)
            {
                if (Player.FindOnline(plyr.Id) != null)
                {
                    return BadRequest($@"Cannot delete a user is currently online.");
                }
            }

            if (!user.TryDelete())
            {
                var client = Globals.ClientLookup.Values.FirstOrDefault(c => c.User.Id == user.Id);
                client?.LogAndDisconnect(default, nameof(DeleteUserById));
            }

            return user;
        }

        [HttpPost("{username}/name")]
        public object ChangeNameByUsername(string userName, [FromBody] NameChange change)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("Invalid user name.");
            }

            if (!FieldChecking.IsValidUsername(change.Name, Strings.Regex.username))
            {
                return BadRequest($@"Invalid name.");
            }

            if (Database.PlayerData.User.UserExists(change.Name))
            {
                return BadRequest($@"Name already taken.");
            }

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return NotFound($@"No user with name '{userName}'.");
            }

            user.Name = change.Name;
            user.Save();

            return user;
        }

        [HttpPost("{userId:guid}/name")]
        public object ChangeNameById(Guid userId, [FromBody] NameChange change)
        {
            if (Guid.Empty == userId)
            {
                return BadRequest($@"Invalid user id '{userId}'.");
            }

            if (!FieldChecking.IsValidUsername(change.Name, Strings.Regex.username))
            {
                return BadRequest($@"Invalid name.");
            }

            if (Database.PlayerData.User.UserExists(change.Name))
            {
                return BadRequest($@"Name already taken.");
            }

            var user = Database.PlayerData.User.FindById(userId);

            if (user == null)
            {
                return NotFound($@"No user with id {userId}.");
            }

            user.Name = change.Name;
            user.Save();

            return user;
        }

        [HttpGet("{userName}/players")]
        [Authorize]
        public object PlayersByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("Invalid user name.");
            }

            var user = Database.PlayerData.User.Find(userName);
            if (user == default)
            {
                return NotFound($@"No user with name '{userName}'.");
            }

            var players = user.Players;
            if (players == default)
            {
                return InternalServerError("Unknown error occurred loading players for user.");
            }

            return players;
        }

        [HttpGet("{userId:guid}/players")]
        [Authorize]
        public object PlayersByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("Invalid user id.");
            }

            var user = Database.PlayerData.User.FindById(userId);
            if (user == default)
            {
                return NotFound($@"No user with id {userId}.");
            }

            var players = user.Players;
            if (players == default)
            {
                return InternalServerError("Unknown error occurred loading players for user.");
            }

            return players;
        }

        [HttpGet("{userName}/players/{playerName}")]
        [Authorize]
        public object PlayerByNameForUserByName(string userName, string playerName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("Invalid user name.");
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                return BadRequest("Invalid player name.");
            }

            var user = Database.PlayerData.User.Find(userName);
            if (user == default)
            {
                return NotFound($@"No user with name '{userName}'.");
            }

            var player = user.Players?.FirstOrDefault(p => string.Equals(p?.Name, playerName, StringComparison.Ordinal));
            if (player == default)
            {
                return NotFound($@"No player exists for '{userName}' with name '{playerName}'.");
            }

            return player;
        }

        [HttpGet("{userId:guid}/players/{playerId:guid}")]
        [Authorize]
        public object PlayerByIdForUserById(Guid userId, Guid playerId)
        {
            if (Guid.Empty == userId)
            {
                return BadRequest($@"Invalid user id '{userId}'.");
            }

            if (Guid.Empty == playerId)
            {
                return BadRequest($@"Invalid player id '{playerId}'.");
            }

            var user = Database.PlayerData.User.FindById(userId);

            if (user == default)
            {
                return NotFound($@"No user with id '{userId}'.");
            }

            var players = user.Players;
            if (players == default)
            {
                return InternalServerError("Unknown error occurred loading players for user.");
            }

            var player = players.FirstOrDefault(p => p?.Id == playerId);

            if (player == default)
            {
                return NotFound($@"No player found on user {userId} with id {playerId}.");
            }

            return player;
        }

        [HttpGet("{userId:guid}/players/{index:int}")]
        [Authorize]
        public object PlayerByIndexForUserById(Guid userId, int index)
        {
            if (Guid.Empty == userId)
            {
                return BadRequest($@"Invalid user id '{userId}'.");
            }

            if (index < 0)
            {
                return BadRequest($@"Invalid player index {index}.");
            }

            var user = Database.PlayerData.User.FindById(userId);

            if (user == null)
            {
                return NotFound($@"No user with id '{userId}'.");
            }

            var players = user.Players;
            if (players == default)
            {
                return InternalServerError("Unknown error occurred loading players for user.");
            }

            if (index >= players.Count)
            {
                return BadRequest($@"The user only has {players.Count} players, {index} is out of bounds.");
            }

            var player = players.Skip(index).FirstOrDefault();

            if (player == default)
            {
                return NotFound($@"No player found on user {userId} with index {index}.");
            }

            return player;
        }

        #region "Change Email"

        [HttpPost("{userName}/manage/email/change")]
        [Authorize(Roles = nameof(ApiRoles.UserManage))]
        public object UserChangeEmailByName(string userName, [FromBody] AdminChange authorizedChange)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("Invalid user name.");
            }

            var email = authorizedChange.New;

            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest($@"Malformed email address '{email}'.");
            }

            if (!FieldChecking.IsWellformedEmailAddress(email, Strings.Regex.email))
            {
                return BadRequest($@"Malformed email address '{email}'.");
            }

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return NotFound($@"No user with name '{userName}'.");
            }

            if (Database.PlayerData.User.UserExists(email))
            {
                return Conflict(@"Email address already in use.");
            }

            user.Email = email;
            user.Save();

            return user;
        }

        [HttpPost("{userId:guid}/manage/email/change")]
        [Authorize(Roles = nameof(ApiRoles.UserManage))]
        public object UserChangeEmailById(Guid userId, [FromBody] AdminChange authorizedChange)
        {
            if (Guid.Empty == userId)
            {
                return BadRequest($@"Invalid user id '{userId}'.");
            }

            var email = authorizedChange.New;

            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest($@"Malformed email address '{email}'.");
            }

            if (!FieldChecking.IsWellformedEmailAddress(email, Strings.Regex.email))
            {
                return BadRequest($@"Malformed email address '{email}'.");
            }

            var user = Database.PlayerData.User.FindById(userId);

            if (user == null)
            {
                return NotFound($@"No user with id '{userId}'.");
            }

            if (Database.PlayerData.User.UserExists(email))
            {
                return Conflict(@"Email address already in use.");
            }

            user.Email = email;
            user.Save();

            return user;
        }

        [HttpPost("{userName}/email/change")]
        public object UserChangeEmailByName(string userName, [FromBody] AuthorizedChange authorizedChange)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("Invalid user name.");
            }

            var email = authorizedChange.New;

            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest($@"Malformed email address '{email}'.");
            }

            if (!FieldChecking.IsWellformedEmailAddress(email, Strings.Regex.email))
            {
                return BadRequest($@"Malformed email address '{email}'.");
            }

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return NotFound($@"No user with name '{userName}'.");
            }

            if (!user.IsPasswordValid(authorizedChange.Authorization?.ToUpperInvariant()?.Trim()))
            {
                return Forbidden(@"Invalid credentials.");
            }

            if (Database.PlayerData.User.UserExists(email))
            {
                return Conflict(@"Email address already in use.");
            }

            user.Email = email;
            user.Save();

            return user;
        }

        [HttpPost("{userId:guid}/email/change")]
        public object UserChangeEmailById(Guid userId, [FromBody] AuthorizedChange authorizedChange)
        {
            if (Guid.Empty == userId)
            {
                return BadRequest($@"Invalid user id '{userId}'.");
            }

            var email = authorizedChange.New;

            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest($@"Malformed email address '{email}'.");
            }

            if (!FieldChecking.IsWellformedEmailAddress(email, Strings.Regex.email))
            {
                return BadRequest($@"Malformed email address '{email}'.");
            }

            var user = Database.PlayerData.User.FindById(userId);

            if (user == null)
            {
                return NotFound($@"No user with id '{userId}'.");
            }

            if (!user.IsPasswordValid(authorizedChange.Authorization?.ToUpperInvariant()?.Trim()))
            {
                return Forbidden(@"Invalid credentials.");
            }

            if (Database.PlayerData.User.UserExists(email))
            {
                return Conflict(@"Email address already in use.");
            }

            user.Email = email;
            user.Save();

            return user;
        }

        #endregion

        #region "Validate Password"

        [HttpPost("{userName}/password/validate")]
        public object UserValidatePasswordByName(string userName, [FromBody] PasswordValidation data)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("Invalid user name.");
            }

            if (string.IsNullOrWhiteSpace(data.Password))
            {
                return BadRequest(@"No password provided.");
            }

            if (!Regex.IsMatch(data.Password?.ToUpperInvariant()?.Trim(), "^[0-9A-Fa-f]{64}$", RegexOptions.Compiled))
            {
                return BadRequest(@"Did not receive a valid password.");
            }

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return NotFound($@"No user with name '{userName}'.");
            }

            if (user.IsPasswordValid(data.Password?.ToUpperInvariant()?.Trim()))
            {
                return Ok("Password Correct");
            }

            return BadRequest(@"Invalid credentials.");
        }

        [HttpPost("{userId:guid}/password/validate")]
        public object UserValidatePasswordById(Guid userId, [FromBody] PasswordValidation data)
        {
            if (Guid.Empty == userId)
            {
                return BadRequest($@"Invalid user id '{userId}'.");
            }

            if (string.IsNullOrWhiteSpace(data.Password))
            {
                return BadRequest(@"No password provided.");
            }

            if (!Regex.IsMatch(data.Password?.ToUpperInvariant()?.Trim(), "^[0-9A-Fa-f]{64}$", RegexOptions.Compiled))
            {
                return BadRequest(@"Did not receive a valid password.");
            }

            var user = Database.PlayerData.User.FindById(userId);

            if (user == null)
            {
                return NotFound($@"No user with name '{userId}'.");
            }

            if (user.IsPasswordValid(data.Password?.ToUpperInvariant()?.Trim()))
            {
                return Ok("Password Correct");
            }

            return BadRequest(@"Invalid credentials.");
        }

        #endregion

        #region "Change Password"

        [HttpPost("{userName}/manage/password/change")]
        [Authorize(Roles = nameof(ApiRoles.UserManage))]
        public object UserChangePassword(string userName, [FromBody] AdminChange authorizedChange)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("Invalid user name.");
            }

            if (!authorizedChange.IsValid)
            {
                return BadRequest(@"Invalid payload");
            }

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return NotFound($@"No user with name '{userName}'.");
            }

            if (!user.TrySetPassword(authorizedChange.New?.ToUpperInvariant()?.Trim()))
            {
                return Forbidden(@"Failed to update password.");
            }

            user.Save();

            return Ok("Password updated.");
        }

        [HttpPost("{userId:guid}/manage/password/change")]
        [Authorize(Roles = nameof(ApiRoles.UserManage))]
        public object UserChangePassword(Guid userId, [FromBody] AdminChange authorizedChange)
        {
            if (Guid.Empty == userId)
            {
                return BadRequest($@"Invalid user id '{userId}'.");
            }

            if (!authorizedChange.IsValid)
            {
                return BadRequest(@"Invalid payload");
            }

            var user = Database.PlayerData.User.FindById(userId);

            if (user == null)
            {
                return NotFound($@"No user with name '{userId}'.");
            }

            if (!user.TrySetPassword(authorizedChange.New?.ToUpperInvariant()?.Trim()))
            {
                return Forbidden(@"Failed to update password.");
            }

            user.Save();

            return Ok("Password Updated.");
        }

        [HttpPost("{userName}/password/change")]
        public object UserChangePassword(string userName, [FromBody] AuthorizedChange authorizedChange)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("Invalid user name.");
            }

            if (!authorizedChange.IsValid)
            {
                return BadRequest(@"Invalid payload");
            }

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return NotFound($@"No user with name '{userName}'.");
            }

            if (!user.TryChangePassword(
                authorizedChange.Authorization?.ToUpperInvariant()?.Trim(), authorizedChange.New?.ToUpperInvariant()?.Trim()
            ))
            {
                return Forbidden(@"Invalid credentials.");
            }

            user.Save();

            return Ok("Password Updated.");
        }

        [HttpPost("{userId:guid}/password/change")]
        public object UserChangePassword(Guid userId, [FromBody] AuthorizedChange authorizedChange)
        {
            if (Guid.Empty == userId)
            {
                return BadRequest($@"Invalid user id '{userId}'.");
            }

            if (!authorizedChange.IsValid)
            {
                return BadRequest(@"Invalid payload");
            }

            var user = Database.PlayerData.User.FindById(userId);

            if (user == null)
            {
                return NotFound($@"No user with name '{userId}'.");
            }

            if (!user.TryChangePassword(
                authorizedChange.Authorization?.ToUpperInvariant()?.Trim(), authorizedChange.New?.ToUpperInvariant()?.Trim()
            ))
            {
                return Forbidden(@"Invalid credentials.");
            }

            user.Save();

            return "Password updated.";
        }

        #endregion

        #region "Request Reset Email Password"

        [HttpGet("{userName}/password/reset")]
        public object UserSendPasswordResetEmailByName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("Invalid user name.");
            }

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return NotFound($@"No user with name '{userName}'.");
            }

            if (Options.Smtp.IsValid())
            {
                var email = new PasswordResetEmail(user);
                if (email.Send())
                {
                    return Ok("Password reset email sent.");
                }

                return InternalServerError("Failed to send reset email.");
            }
            else
            {
                return NotFound("Could not send password reset email, SMTP settings on the server are not configured!");
            }
        }

        [HttpGet("{userId:guid}/password/reset")]
        public object UserSendPasswordResetEmailById(Guid userId)
        {
            var user = Database.PlayerData.User.FindById(userId);

            if (user == null)
            {
                return NotFound($@"No user with name '{userId}'.");
            }

            if (Options.Smtp.IsValid())
            {
                var email = new PasswordResetEmail(user);
                if (email.Send())
                {
                    return Ok("Password reset email sent.");
                }

                return InternalServerError("Failed to send reset email.");
            }
            else
            {
                return NotFound("Could not send password reset email, SMTP settings on the server are not configured!");
            }
        }

        #endregion

        #region "Admin Action"

        [HttpPost("{userId:guid}/admin/{act}")]
        public object DoAdminActionOnPlayerById(
            Guid userId,
            string act,
            [FromBody] AdminActionParameters actionParameters
        )
        {
            if (!Enum.TryParse<AdminAction>(act, true, out var adminAction))
            {
                return BadRequest(@"Invalid action.");
            }

            if (Guid.Empty == userId)
            {
                return BadRequest($@"Invalid user id '{userId}'.");
            }

            Tuple<Client, User> fetchResult;
            fetchResult = Database.PlayerData.User.Fetch(userId);

            return DoAdminActionOnUser(
                () => fetchResult,
                () => NotFound($@"No user with id '{userId}'."),
                adminAction, actionParameters
            );
        }

        [HttpPost("{userName}/admin/{act}")]
        public object DoAdminActionOnPlayerByName(
            string userName,
            string act,
            [FromBody] AdminActionParameters actionParameters
        )
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("Invalid user name.");
            }

            if (!Enum.TryParse<AdminAction>(act, true, out var adminAction))
            {
                return BadRequest(@"Invalid action.");
            }

            Tuple<Client, User> fetchResult;
            fetchResult = Database.PlayerData.User.Fetch(userName);

            return DoAdminActionOnUser(
                () => fetchResult,
                () => NotFound($@"No user with name '{userName}'."),
                adminAction, actionParameters
            );
        }

        private IActionResult DoAdminActionOnUser(
            Func<Tuple<Client, User>> fetch,
            Func<IActionResult> onError,
            AdminAction adminAction,
            AdminActionParameters actionParameters
        )
        {
            var (client, user) = fetch();

            if (user == null)
            {
                return onError();
            }

            var actionPerformer = IntersectUser;
            if (actionPerformer == default)
            {
                return onError();
            }

            var player = client?.Entity;
            var targetIp = client?.Ip ?? string.Empty;
            switch (adminAction)
            {
                case AdminAction.Ban:
                    if (actionPerformer.Power.CompareTo(user.Power) < 0) // Authority Comparison.
                    {
                        // Inform to whoever performed the action that they are
                        // not allowed to do this due to the lack of authority over their target.
                        return BadRequest(Strings.Account.NotAllowed.ToString(user.Name));
                    }
                    else if (Ban.Find(user.Id) != null) // If the target is already banned.
                    {
                        return BadRequest(Strings.Account.alreadybanned.ToString(user.Name));
                    }

                    // If target is online, not yet banned and the banner has the authority to ban.
                    else
                    {
                        // Add ban
                        Ban.Add(
                            user.Id, actionParameters.Duration, actionParameters.Reason ?? string.Empty,
                            actionPerformer.Name, actionParameters.Ip ? targetIp : string.Empty
                        );

                        // Disconnect the banned player.
                        client?.Disconnect();

                        // Sends a global chat message to every user online about the banned player.
                        PacketSender.SendGlobalMsg(Strings.Account.banned.ToString(user.Name));

                        //  Inform to the API about the successful ban.
                        return Ok(Strings.Account.banned.ToString(user.Name));
                    }

                case AdminAction.UnBan:
                    Ban.Remove(user.Id, false);
                    PacketSender.SendGlobalMsg(Strings.Account.UnbanSuccess.ToString(user.Name));

                    return Ok(Strings.Account.UnbanSuccess.ToString(user.Name));

                case AdminAction.Mute:
                    if (actionPerformer.Power.CompareTo(user.Power) < 0) // Authority Comparison.
                    {
                        // Inform to whoever performed the action that they are
                        // not allowed to do this due to the lack of authority over their target.
                        return BadRequest(Strings.Account.NotAllowed.ToString(user.Name));
                    }
                    else if (Mute.Find(user) != null) // If the target is already muted.
                    {
                        return BadRequest(Strings.Account.alreadymuted.ToString(user.Name));
                    }

                    // If target is online, not yet muted and the action performer has the authority to mute.
                    else
                    {
                        Mute.Add(
                            user, actionParameters.Duration, actionParameters.Reason ?? string.Empty,
                            actionPerformer.Name, actionParameters.Ip ? targetIp : string.Empty
                        );

                        PacketSender.SendGlobalMsg(Strings.Account.muted.ToString(user.Name));

                        return Ok(Strings.Account.muted.ToString(user.Name));
                    }

                case AdminAction.UnMute:
                    Mute.Remove(user);
                    PacketSender.SendGlobalMsg(Strings.Account.UnmuteSuccess.ToString(user.Name));

                    return Ok(Strings.Account.UnmuteSuccess.ToString(user.Name));

                case AdminAction.WarpTo:
                    if (player != null)
                    {
                        if (actionParameters.MapId == Guid.Empty)
                        {
                            return BadRequest(@"Expected a map ID.");
                        }

                        var mapId = actionParameters.MapId == Guid.Empty ? player.MapId : actionParameters.MapId;
                        player.Warp(mapId, (byte) player.X, (byte) player.Y);

                        return Ok($@"Warped '{player.Name}' to {mapId} ({player.X}, {player.Y}).");
                    }

                    break;

                case AdminAction.WarpToLoc:
                    if (player != null)
                    {
                        var mapId = actionParameters.MapId == Guid.Empty ? player.MapId : actionParameters.MapId;
                        player.Warp(mapId, actionParameters.X, actionParameters.Y, true);

                        return Ok($@"Warped '{player.Name}' to {mapId} ({actionParameters.X}, {actionParameters.Y}).");
                    }

                    break;

                case AdminAction.Kick:
                    if (client != null)
                    {
                        if (actionPerformer.Power.CompareTo(player?.Power) < 0) // Authority Comparison.
                        {
                            // Inform to whoever performed the action that they are
                            // not allowed to do this due to the lack of authority over their target.
                            return BadRequest(Strings.Account.NotAllowed.ToString(player?.Name));
                        }
                        else
                        {
                            client.Disconnect(actionParameters.Reason);
                            PacketSender.SendGlobalMsg(Strings.Player.serverkicked.ToString(player?.Name));

                            return Ok(Strings.Player.serverkicked.ToString(player?.Name));
                        }
                    }

                    break;

                case AdminAction.Kill:
                    if (client != null && client.Entity != null)
                    {
                        if (actionPerformer.Power.CompareTo(player?.Power) < 0) // Authority Comparison.
                        {
                            // Inform to whoever performed the action that they are
                            // not allowed to do this due to the lack of authority over their target.
                            return BadRequest(Strings.Account.NotAllowed.ToString(player?.Name));
                        }
                        else
                        {
                            lock (client.Entity.EntityLock)
                            {
                                client.Entity.Die();
                            }

                            PacketSender.SendGlobalMsg(Strings.Player.serverkilled.ToString(player?.Name));

                            return Ok(Strings.Commandoutput.killsuccess.ToString(player?.Name));
                        }
                    }

                    break;

                case AdminAction.WarpMeTo:
                case AdminAction.WarpToMe:
                    return BadRequest($@"'{adminAction.ToString()}' not supported by the API.");

                case AdminAction.SetSprite:
                case AdminAction.SetFace:
                case AdminAction.SetAccess:
                default:
                    return NotImplemented(adminAction.ToString());
            }

            return NotFound(Strings.Player.offline);
        }

        #endregion

    }

}
