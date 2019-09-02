using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

using Intersect.Enums;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Web.RestApi.Attributes;
using Intersect.Server.Web.RestApi.Extensions;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Utilities;
using JetBrains.Annotations;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [RoutePrefix("users")]
    [ConfigurableAuthorize(Roles = nameof(UserRights.PersonalInformation))]
    public sealed class UserController : ApiController
    {
        [Route]
        [HttpGet]
        public object List(int page = 0, int count = 10)
        {
            page = Math.Max(page, 0);
            count = Math.Max(Math.Min(count, 100), 5);

            using (var context = PlayerContext.Temporary)
            {
                var entries = Database.PlayerData.User.List(page, count, context).ToList();
                return new
                {
                    total = context.Users.Count(),
                    page,
                    count = entries.Count,
                    entries
                };
            }
        }

        [Route("{userId:guid}")]
        [HttpGet]
        public User UserById(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            using (var context = PlayerContext.Temporary)
            {
                return Database.PlayerData.User.Find(userId, context);
            }
        }

        [Route("userId:guid/AdminActions/{adminAction:AdminActions}")]
        [HttpPost]
        public object DoAdminActionOnPlayerById(
            Guid userId,
            AdminActions adminAction,
            [FromBody] AdminActionParameters actionParameters
        )
        {
            if (Guid.Empty == userId)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid user id '{userId}'.");
            }

            Tuple<Client, User> fetchResult;
            using (var context = PlayerContext.Temporary)
            {
                fetchResult = Database.PlayerData.User.Fetch(userId, context);
            }

            return DoAdminActionOnUser(
                () => fetchResult,
                () => Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with id '{userId}'."),
                adminAction, actionParameters
            );
        }

        [Route("{userName}")]
        [HttpGet]
        public object UserByName(string userName)
        {
            using (var context = PlayerContext.Temporary)
            {
                var user = Database.PlayerData.User.Find(userName, context);

                if (user == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userName}'.");
                }

                return user;
            }
        }

        [Route("{userName}/email/change")]
        [HttpGet]
        public object UserChangeEmail(string userName, [FromBody] AuthorizedChange authorizedChange)
        {
            var email = authorizedChange.New;

            if (string.IsNullOrWhiteSpace(email))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Malformed email address '{email}'.");
            }

            if (!FieldChecking.IsWellformedEmailAddress(email, Strings.Regex.email))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Malformed email address '{email}'.");
            }

            using (var context = PlayerContext.Temporary)
            {
                var user = Database.PlayerData.User.Find(userName, context);

                if (user == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userName}'.");
                }

                if (!user.IsPasswordValid(authorizedChange.Authorization))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Forbidden, @"Invalid credentials.");
                }

                if (LegacyDatabase.EmailInUse(email))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Conflict, @"Email address already in use.");
                }

                user.Email = email;

                LegacyDatabase.SavePlayerDatabaseAsync();
                return user;
            }
        }

        [Route("{userName}/password/validate")]
        [HttpPost]
        public object UserValidatePassword(string userName, [FromBody] string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"No password provided.");
            }

            if (!Regex.IsMatch(password, "^[0-9A-Fa-f]{64}$", RegexOptions.Compiled))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Did not receive a valid password.");
            }

            using (var context = PlayerContext.Temporary)
            {
                var user = Database.PlayerData.User.Find(userName, context);

                if (user == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userName}'.");
                }

                if (user.IsPasswordValid(password))
                {
                    return true;
                }

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid credentials.");
            }
        }

        [Route("{userName}/password/change")]
        [HttpPost]
        public object UserChangePassword(string userName, [FromBody] AuthorizedChange authorizedChange)
        {
            if (!authorizedChange.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid payload");
            }

            using (var context = PlayerContext.Temporary)
            {
                var user = Database.PlayerData.User.Find(userName, context);

                if (user == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userName}'.");
                }

                if (!user.TryChangePassword(authorizedChange.Authorization, authorizedChange.New))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Forbidden, @"Invalid credentials.");
                }

                LegacyDatabase.SavePlayerDatabaseAsync();
                return true;

            }
        }

        [Route("{userName}/AdminActions/{adminAction:AdminActions}")]
        [HttpPost]
        public object DoAdminActionOnPlayerByName(
            string userName,
            AdminActions adminAction,
            [FromBody] AdminActionParameters actionParameters
        )
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid user name '{userName}'.");
            }

            Tuple<Client, User> fetchResult;
            using (var context = PlayerContext.Temporary)
            {
                fetchResult = Database.PlayerData.User.Fetch(userName, context);
            }

            return DoAdminActionOnUser(
                () => fetchResult,
                () => Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userName}'."),
                adminAction, actionParameters
            );
        }

        private object DoAdminActionOnUser(
            [NotNull] Func<Tuple<Client, User>> fetch,
            [NotNull] Func<HttpResponseMessage> onError,
            AdminActions adminAction,
            AdminActionParameters actionParameters
        )
        {
            var (client, user) = fetch();

            if (user == null)
            {
                return onError();
            }

            var player = client?.Entity;
            var targetIp = client?.GetIp() ?? "";

            switch (adminAction)
            {
                case AdminActions.Ban:
                    Ban.Add(
                        user.Id,
                        actionParameters.Duration,
                        actionParameters.Reason ?? "",
                        actionParameters.Moderator ?? @"api",
                        actionParameters.Ip ? targetIp : ""
                    );
                    client?.Disconnect();
                    PacketSender.SendGlobalMsg(Strings.Account.banned.ToString(user.Name));
                    return Request.CreateMessageResponse(HttpStatusCode.OK, Strings.Account.banned.ToString(user.Name));

                case AdminActions.UnBan:
                    Ban.Remove(user.Id);
                    PacketSender.SendGlobalMsg(Strings.Account.unbanned.ToString(user.Name));
                    return Request.CreateMessageResponse(HttpStatusCode.OK, Strings.Account.unbanned.ToString(user.Name));

                case AdminActions.Mute:
                    Mute.Add(
                        user,
                        actionParameters.Duration,
                        actionParameters.Reason ?? "",
                        actionParameters.Moderator ?? @"api",
                        actionParameters.Ip ? targetIp : ""
                    );
                    PacketSender.SendGlobalMsg(Strings.Account.muted.ToString(user.Name));
                    return Request.CreateMessageResponse(HttpStatusCode.OK, Strings.Account.muted.ToString(user.Name));

                case AdminActions.UnMute:
                    Mute.Remove(user);
                    PacketSender.SendGlobalMsg(Strings.Account.unmuted.ToString(user.Name));
                    return Request.CreateMessageResponse(HttpStatusCode.OK, Strings.Account.unmuted.ToString(user.Name));

                case AdminActions.WarpTo:
                    if (player != null)
                    {
                        var mapId = actionParameters.MapId == Guid.Empty ? player.MapId : actionParameters.MapId;
                        player.Warp(mapId, (byte)player.X, (byte)player.Y);
                        return Request.CreateMessageResponse(HttpStatusCode.OK, $@"Warped '{player.Name}' to {mapId} ({player.X}, {player.Y}).");
                    }
                    break;

                case AdminActions.WarpToLoc:
                    if (player != null)
                    {
                        var mapId = actionParameters.MapId == Guid.Empty ? player.MapId : actionParameters.MapId;
                        player.Warp(mapId, actionParameters.X, actionParameters.Y, true);
                        return Request.CreateMessageResponse(HttpStatusCode.OK, $@"Warped '{player.Name}' to {mapId} ({actionParameters.X}, {actionParameters.Y}).");
                    }
                    break;

                case AdminActions.Kick:
                    if (client != null)
                    {
                        client.Disconnect(actionParameters.Reason);
                        PacketSender.SendGlobalMsg(Strings.Player.serverkicked.ToString(player?.Name));
                        return Request.CreateMessageResponse(HttpStatusCode.OK, Strings.Player.serverkicked.ToString(player?.Name));
                    }
                    break;

                case AdminActions.Kill:
                    if (client != null)
                    {
                        client.Disconnect(actionParameters.Reason);
                        PacketSender.SendGlobalMsg(Strings.Player.serverkilled.ToString(player?.Name));
                        return Request.CreateMessageResponse(HttpStatusCode.OK, Strings.Commandoutput.killsuccess.ToString(player?.Name));
                    }
                    break;

                case AdminActions.WarpMeTo:
                case AdminActions.WarpToMe:
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"'{adminAction.ToString()}' not supported by the API.");

                case AdminActions.SetSprite:
                case AdminActions.SetFace:
                case AdminActions.SetAccess:
                default:
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, adminAction.ToString());
            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, Strings.Player.offline);
        }

        [Route("{userName}/players")]
        [HttpGet]
        [ConfigurableAuthorize, OverrideAuthorization]
        public List<Player> PlayersByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return null;
            }

            using (var context = PlayerContext.Temporary)
            {
                return Database.PlayerData.User
                    .Find(userName, context)?
                    .Players;
            }
        }

        [Route("{userId:guid}/players")]
        [HttpGet]
        [ConfigurableAuthorize, OverrideAuthorization]
        public List<Player> PlayersByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            using (var context = PlayerContext.Temporary)
            {
                return Database.PlayerData.User
                    .Find(userId, context)?
                    .Players;
            }
        }

        [Route("{userName}/players/{playerName}")]
        [HttpGet]
        [ConfigurableAuthorize, OverrideAuthorization]
        public Player PlayerByNameForUserByName(string userName, string playerName)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(playerName))
            {
                return null;
            }

            using (var context = PlayerContext.Temporary)
            {
                return Database.PlayerData.User
                    .Find(userName, context)?
                    .Players?
                    .FirstOrDefault(player => string.Equals(player?.Name, playerName, StringComparison.Ordinal));
            }
        }

        [Route("{userId:guid}/players/{playerId:guid}")]
        [HttpGet]
        [ConfigurableAuthorize, OverrideAuthorization]
        public Player PlayerByIdForUserById(Guid userId, Guid playerId)
        {
            if (userId == Guid.Empty || playerId == Guid.Empty)
            {
                return null;
            }

            using (var context = PlayerContext.Temporary)
            {
                return Database.PlayerData.User
                    .Find(userId, context)?
                    .Players?
                    .FirstOrDefault(player => player?.Id == playerId);
            }
        }

        [Route("{userId:guid}/players/{index:int}")]
        [HttpGet]
        [ConfigurableAuthorize, OverrideAuthorization]
        public Player PlayerByIndexForUserById(Guid userId, int index)
        {
            if (userId == Guid.Empty || index < 0)
            {
                return null;
            }

            using (var context = PlayerContext.Temporary)
            {
                return Database.PlayerData.User
                    .Find(userId, context)?
                    .Players?
                    .Skip(index)
                    .FirstOrDefault();
            }
        }
    }
}