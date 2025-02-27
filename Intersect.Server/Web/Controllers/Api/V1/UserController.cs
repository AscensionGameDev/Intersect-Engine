using System.Net;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Variables;
using Intersect.Security;
using Intersect.Server.Collections.Indexing;
using Intersect.Server.Collections.Sorting;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Notifications;
using Intersect.Server.Web.Http;
using Intersect.Server.Web.Types;
using Intersect.Server.Web.Types.User;
using Intersect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.Controllers.Api.V1
{
    [Route("api/v1/users")]
    [Authorize(Roles = nameof(ApiRoles.UserQuery))]
    public sealed partial class UserController : IntersectController
    {
        #region "Basic CRUD Operations"

        [HttpGet]
        [ProducesResponseType(typeof(DataPage<User>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult List(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] int limit = PagingInfo.MaxPageSize,
            [FromQuery] string sortBy = null,
            [FromQuery] SortDirection sortDirection = SortDirection.Ascending,
            [FromQuery] string search = null
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, PagingInfo.MaxPageSize), PagingInfo.MinPageSize);
            limit = Math.Max(Math.Min(limit, pageSize), 1);
            var values = Database.PlayerData.User.List(search?.Length > 2 ? search : null, sortBy, sortDirection, page * pageSize, pageSize, out var total);

            if (limit != pageSize)
            {
                values = values.Take(limit).ToList();
            }

            return Ok(new DataPage<User>(
                Total: total,
                Page: page,
                PageSize: pageSize,
                Count: values.Count,
                Values: values
            ));
        }

        [HttpGet("{lookupKey:LookupKey}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult GetUser(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid user id." : @"Invalid username.");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                return NotFound($@"No user found for lookup key '{lookupKey}'.");
            }

            return Ok(user);
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(RegisterResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult RegisterUser([FromBody] UserInfoRequestBody user)
        {
            if (string.IsNullOrEmpty(user.Username) ||
                string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Username, Email, and Password all must be provided, and not null/empty.");
            }

            if (!FieldChecking.IsWellformedEmailAddress(user.Email, Strings.Regex.Email))
            {
                return BadRequest($@"Malformed email address '{user.Email}'.");
            }

            if (!FieldChecking.IsValidUsername(user.Username, Strings.Regex.Username))
            {
                return BadRequest($@"Invalid username '{user.Username}'.");
            }

            var cleanedPassword = user.Password?.Trim();
            if (!PasswordUtils.IsValidClientPasswordHash(cleanedPassword))
            {
                return BadRequest(@"Did not receive a valid password.");
            }

            if (Database.PlayerData.User.UserExists(user.Username))
            {
                return BadRequest($@"Account already exists with username '{user.Username}'.");
            }

            if (Database.PlayerData.User.UserExists(user.Email))
            {
                return BadRequest($@"Account already with email '{user.Email}'.");
            }

            DbInterface.CreateAccount(null, user.Username, cleanedPassword, user.Email);
            return Ok(new RegisterResponseBody(user.Username, user.Email));
        }

        [HttpDelete("{lookupKey:LookupKey}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult DeleteUser(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid user id." : @"Invalid username.");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                return NotFound($@"No user found for lookup key '{lookupKey}'.");
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
                var client = Client.LookupByConnectionId.Values.FirstOrDefault(c => c.User.Id == user.Id);
                _ = client?.LogAndDisconnect(default, nameof(DeleteUser));
            }

            return Ok(user);
        }

        #endregion

        #region "Player CRUD Operations"

        [HttpGet("{lookupKey:LookupKey}/players")]
        [Authorize]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
        [ProducesResponseType(typeof(IEnumerable<Player>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult GetPlayers(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid user id." : @"Invalid username.");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                return NotFound($@"No user found for lookup key '{lookupKey}'.");
            }

            var players = user.Players;
            if (players == default)
            {
                return InternalServerError("Unknown error occurred loading players for user.");
            }

            return Ok(players);
        }

        [HttpGet("{lookupKey:LookupKey}/players/{playerName}")]
        [Authorize]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(Player), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult GetPlayer(LookupKey lookupKey, string playerName)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid user id." : @"Invalid username.");
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                return BadRequest("Invalid player name.");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                return NotFound($@"No user found for lookup key '{lookupKey}'.");
            }

            var player = user.Players?.FirstOrDefault(p => string.Equals(p?.Name, playerName, StringComparison.Ordinal));
            if (player == default)
            {
                return NotFound($@"No player exists with name '{playerName}' for user with lookup key '{lookupKey}'");
            }

            return Ok(player);
        }

        [HttpGet("{lookupKey:LookupKey}/players/{index:int}")]
        [Authorize]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
        [ProducesResponseType(typeof(Player), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult GetPlayerByIndex(LookupKey lookupKey, int index)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid user id." : @"Invalid username.");
            }

            if (index < 0)
            {
                return BadRequest($@"Invalid player index {index}.");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                return NotFound($@"No user found for lookup key '{lookupKey}'.");
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
                return NotFound($@"No player found for user with lookup key {lookupKey} with index {index}.");
            }

            return Ok(player);
        }

        #endregion

        #region "Account Management"

        [HttpPost("{lookupKey:LookupKey}/name")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult ChangeUsername(LookupKey lookupKey, [FromBody] NameChangePayload change)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid user id." : @"Invalid username.");
            }

            if (!FieldChecking.IsValidUsername(change.Name, Strings.Regex.Username))
            {
                return BadRequest($@"Invalid name.");
            }

            if (Database.PlayerData.User.UserExists(change.Name))
            {
                return BadRequest($@"Name already taken.");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                return NotFound($@"No user found for lookup key '{lookupKey}'.");
            }

            user.Name = change.Name;
            _ = user.Save();
            return Ok(user);
        }

        [HttpPost("{lookupKey:LookupKey}/manage/email/change")]
        [Authorize(Roles = nameof(ApiRoles.UserManage))]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.Conflict, ContentTypes.Json)]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult ChangeEmail(LookupKey lookupKey, [FromBody] AdminChangeRequestBody authorizedChange)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid user id." : @"Invalid username.");
            }

            var email = authorizedChange.New;
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest($@"Malformed email address '{email}'.");
            }

            if (!FieldChecking.IsWellformedEmailAddress(email, Strings.Regex.Email))
            {
                return BadRequest($@"Malformed email address '{email}'.");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                return NotFound($@"No user found for lookup key '{lookupKey}'.");
            }

            if (Database.PlayerData.User.UserExists(email))
            {
                return Conflict(@"Email address already in use.");
            }

            user.Email = email;
            _ = user.Save();
            return Ok(user);
        }

        [HttpPost("{lookupKey:LookupKey}/email/change")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.Conflict, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.Forbidden, ContentTypes.Json)]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult UserChangeEmail(LookupKey lookupKey, [FromBody] AuthorizedChangeRequestBody authorizedChange)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid user id." : @"Invalid username.");
            }

            var email = authorizedChange.New;
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest($@"Malformed email address '{email}'.");
            }

            if (!FieldChecking.IsWellformedEmailAddress(email, Strings.Regex.Email))
            {
                return BadRequest($@"Malformed email address '{email}'.");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                return NotFound($@"No user found for lookup key '{lookupKey}'.");
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
            _ = user.Save();
            return Ok(user);
        }

        [HttpPost("{lookupKey:LookupKey}/password/validate")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult ValidatePassword(LookupKey lookupKey, [FromBody] PasswordValidationRequestBody data)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid user id." : @"Invalid username.");
            }

            var cleanedPassword = data.Password?.Trim();
            if (string.IsNullOrWhiteSpace(cleanedPassword))
            {
                return BadRequest(@"No password provided.");
            }

            if (!PasswordUtils.IsValidClientPasswordHash(cleanedPassword))
            {
                return BadRequest(@"Did not receive a valid password.");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                return NotFound($@"No user found for lookup key '{lookupKey}'.");
            }

            if (!user.IsPasswordValid(cleanedPassword))
            {
                return BadRequest(@"Invalid credentials.");
            }

            return Ok("Password Correct");
        }

        [HttpPost("{lookupKey:LookupKey}/manage/password/change")]
        [Authorize(Roles = nameof(ApiRoles.UserManage))]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.Forbidden, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult ChangePassword(LookupKey lookupKey, [FromBody] AdminChangeRequestBody authorizedChange)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid user id." : @"Invalid username.");
            }

            if (string.IsNullOrEmpty(authorizedChange.New))
            {
                return BadRequest(@"Invalid payload");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                return NotFound($@"No user found for lookup key '{lookupKey}'.");
            }

            if (!user.TrySetPassword(authorizedChange.New?.ToUpperInvariant()?.Trim()))
            {
                return Forbidden(@"Failed to update password.");
            }

            _ = user.Save();
            return Ok("Password updated.");
        }

        [HttpPost("{lookupKey:LookupKey}/password/change")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.Forbidden, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult UserChangePassword(LookupKey lookupKey, [FromBody] AuthorizedChangeRequestBody authorizedChange)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid user id." : @"Invalid username.");
            }

            if (string.IsNullOrWhiteSpace(authorizedChange.Authorization) || string.IsNullOrWhiteSpace(authorizedChange.New))
            {
                return BadRequest(@"Invalid payload");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                return NotFound($@"No user found for lookup key '{lookupKey}'.");
            }

            var oldPassword = authorizedChange.Authorization?.ToUpperInvariant()?.Trim();
            var newPassword = authorizedChange.New?.ToUpperInvariant()?.Trim();
            if (!user.TryChangePassword(oldPassword, newPassword))
            {
                return Forbidden(@"Invalid credentials.");
            }

            _ = user.Save();
            return Ok("Password Updated.");
        }

        [HttpGet("{lookupKey:LookupKey}/password/reset")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult UserSendPasswordResetEmail(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid user id." : @"Invalid username.");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
            {
                return NotFound($@"No user found for lookup key '{lookupKey}'.");
            }

            if (!Options.Instance.SmtpSettings.IsValid())
            {
                return NotFound("Could not send password reset email, SMTP settings on the server are not configured!");
            }

            var email = new PasswordResetEmail(user);
            if (!email.Send())
            {
                return InternalServerError("Failed to send reset email.");
            }

            return Ok("Password reset email sent.");
        }

        #endregion

        #region User Variables

        [HttpGet("{lookupKey:LookupKey}/variables")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(IEnumerable<UserVariable>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult UserVariablesList(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Database.PlayerData.User.TryFind(lookupKey, out var user))
            {
                return NotFound($@"No user found for {lookupKey}");
            }

            return Ok(user.Variables);
        }

        [HttpGet("{lookupKey:LookupKey}/variables/{variableId:guid}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(UserVariable), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult UserVariableGet(LookupKey lookupKey, Guid variableId)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Database.PlayerData.User.TryFind(lookupKey, out var user))
            {
                return NotFound($@"No user found for {lookupKey}");
            }

            if (variableId == Guid.Empty)
            {
                return BadRequest($@"Variable id cannot be {variableId}");
            }

            if (!UserVariableDescriptor.TryGet(variableId, out var variableDescriptor))
            {
                return NotFound($@"Variable not found for id {variableId}");
            }

            var variable = user.GetVariable(variableDescriptor.Id, true);
            return Ok(variable);
        }

        [HttpGet("{lookupKey:LookupKey}/variables/{variableId:guid}/value")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(VariableValueBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult UserVariableValueGet(LookupKey lookupKey, Guid variableId)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Database.PlayerData.User.TryFind(lookupKey, out var user))
            {
                return NotFound($@"No user found for {lookupKey}");
            }

            if (variableId == Guid.Empty)
            {
                return BadRequest($@"Variable id cannot be {variableId}");
            }

            if (!UserVariableDescriptor.TryGet(variableId, out var variableDescriptor))
            {
                return NotFound($@"Variable not found for id {variableId}");
            }

            var variable = user.GetVariable(variableDescriptor.Id, true);
            return Ok(new VariableValueBody
            {
                Value = variable.Value.Value,
            });
        }

        [HttpPost("{lookupKey:LookupKey}/variables/{variableId:guid}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(UserVariable), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult UserVariableSet(LookupKey lookupKey, Guid variableId, [FromBody] VariableValueBody valueBody)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Database.PlayerData.User.TryFind(lookupKey, out var user))
            {
                return NotFound($@"No user found for {lookupKey}");
            }

            if (variableId == Guid.Empty)
            {
                return BadRequest($@"Variable id cannot be {variableId}");
            }

            if (!UserVariableDescriptor.TryGet(variableId, out var variableDescriptor))
            {
                return NotFound($@"Variable not found for id {variableId}");
            }

            var variable = user.GetVariable(variableDescriptor.Id, true);

            var changed = false;
            if (variable?.Value != null)
            {
                if (variable.Value.Value != valueBody.Value)
                {
                    variable.Value.Value = valueBody.Value;
                    changed = true;
                }
            }

            // ReSharper disable once InvertIf
            if (changed)
            {
                user.StartCommonEventsWithTriggerForAll(CommonEventTrigger.UserVariableChange, string.Empty, variableId.ToString());
                _ = user.UpdatedVariables.AddOrUpdate(
                    variableId,
                    variableDescriptor,
                    (_, _) => variableDescriptor
                );
            }

            return Ok(variable);
        }

        #endregion Variables

        #region "Admin Action"

        [HttpPost("{lookupKey:LookupKey}/admin/{adminAction:AdminAction}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotImplemented, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult DoAdminActionOnUserByLookupKey(
            LookupKey lookupKey,
            AdminAction adminAction,
            [FromBody] AdminActionParameters actionParameters
        )
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Database.PlayerData.User.TryFetch(lookupKey, out var user, out var client))
            {
                return NotFound($"No user found for lookup key '{lookupKey}'");
            }

            return DoAdminActionOnUser(
                client,
                user,
                () => NotFound($@"No user found for lookup key '{lookupKey}'."),
                adminAction,
                actionParameters
            );
        }

        private IActionResult DoAdminActionOnUser(
            Client client,
            User user,
            Func<IActionResult> onError,
            AdminAction adminAction,
            AdminActionParameters actionParameters
        )
        {
            if (user == default)
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
                        return BadRequest(Strings.Account.AlreadyBanned.ToString(user.Name));
                    }

                    // If target is online, not yet banned and the banner has the authority to ban.
                    else
                    {
                        // Add ban
                        _ = Ban.Add(
                            user.Id, actionParameters.Duration, actionParameters.Reason ?? string.Empty,
                            actionPerformer.Name, actionParameters.Ip ? targetIp : string.Empty
                        );

                        // Disconnect the banned player.
                        client?.Disconnect();

                        // Sends a global chat message to every user online about the banned player.
                        PacketSender.SendGlobalMsg(Strings.Account.Banned.ToString(user.Name));

                        //  Inform to the API about the successful ban.
                        return Ok(Strings.Account.Banned.ToString(user.Name));
                    }

                case AdminAction.UnBan:
                    _ = Ban.Remove(user.Id, false);
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
                        return BadRequest(Strings.Account.AlreadyMuted.ToString(user.Name));
                    }

                    // If target is online, not yet muted and the action performer has the authority to mute.
                    else
                    {
                        _ = Mute.Add(
                            user, actionParameters.Duration, actionParameters.Reason ?? string.Empty,
                            actionPerformer.Name, actionParameters.Ip ? targetIp : string.Empty
                        );

                        PacketSender.SendGlobalMsg(Strings.Account.Muted.ToString(user.Name));

                        return Ok(Strings.Account.Muted.ToString(user.Name));
                    }

                case AdminAction.UnMute:
                    _ = Mute.Remove(user);
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
                        player.Warp(mapId, (byte)player.X, (byte)player.Y);

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
                            PacketSender.SendGlobalMsg(Strings.Player.ServerKicked.ToString(player?.Name));

                            return Ok(Strings.Player.ServerKicked.ToString(player?.Name));
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

                            PacketSender.SendGlobalMsg(Strings.Player.ServerKilled.ToString(player?.Name));

                            return Ok(Strings.Commandoutput.KillSuccess.ToString(player?.Name));
                        }
                    }

                    break;

                case AdminAction.WarpMeTo:
                case AdminAction.WarpToMe:
                    return BadRequest($@"'{adminAction}' not supported by the API.");

                case AdminAction.SetSprite:
                case AdminAction.SetFace:
                case AdminAction.SetAccess:
                default:
                    return NotImplemented(adminAction.ToString());
            }

            return NotFound(Strings.Player.Offline);
        }

        #endregion

        #region Obsolete

        [HttpPost]
        [Obsolete("The appropriate verb for retrieving a list of records is GET not POST")]
        public IActionResult ListPost([FromBody] PagingInfo pageInfo)
        {
            var page = Math.Max(pageInfo.Page, 0);
            var pageSize = Math.Max(Math.Min(pageInfo.PageSize, PagingInfo.MaxPageSize), PagingInfo.MinPageSize);

            var values = Database.PlayerData.User.List(null, null, SortDirection.Ascending, pageInfo.Page * pageInfo.PageSize, pageInfo.PageSize, out var total);

            return Ok(new DataPage<User>(
                Total: total,
                Page: page,
                PageSize: pageSize,
                Count: values.Count,
                Values: values
            ));
        }

        #endregion
    }
}
