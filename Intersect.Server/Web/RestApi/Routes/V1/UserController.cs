﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

using Intersect.Enums;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Notifications;
using Intersect.Server.Web.RestApi.Attributes;
using Intersect.Server.Web.RestApi.Extensions;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.RestApi.Types;
using Intersect.Utilities;

using JetBrains.Annotations;

namespace Intersect.Server.Web.RestApi.Routes.V1
{

    [RoutePrefix("users")]
    [ConfigurableAuthorize(Roles = nameof(ApiRoles.UserQuery))]
    public sealed class UserController : IntersectApiController
    {

        [Route]
        [HttpPost]
        public object ListPost([FromBody] PagingInfo pageInfo)
        {
            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.Count = Math.Max(Math.Min(pageInfo.Count, PAGE_SIZE_MAX), 5);

            var entries = Database.PlayerData.User.List(pageInfo.Page, pageInfo.Count).ToList();

            return new
            {
                total = Database.PlayerData.User.Count(),
                pageInfo.Page,
                count = entries.Count,
                entries
            };
        }

        [Route]
        [HttpGet]
        public DataPage<User> List(
            [FromUri] int page = 0,
            [FromUri] int pageSize = 0,
            [FromUri] int limit = PAGE_SIZE_MAX //,
            //[FromUri] string[] sortBy = null, [FromUri] SortDirection[] sortDirection = null
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, PAGE_SIZE_MAX), PAGE_SIZE_MIN);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            //var sort = Sort.From(sortBy, sortDirection);
            //var values = Database.PlayerData.User.List(page, pageSize, sort).ToList();
            var values = Database.PlayerData.User.List(page, pageSize).ToList();
            if (limit != pageSize)
            {
                values = values.Take(limit).ToList();
            }

            return new DataPage<User>
            {
                Total = Database.PlayerData.User.Count(),
                Page = page,
                PageSize = pageSize,
                Count = values.Count,
                Values = values
            };
        }

        [Route("{userId:guid}")]
        [HttpGet]
        public User UserById(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            return Database.PlayerData.User.Find(userId);
        }

        [Route("{userName}")]
        [HttpGet]
        public object UserByName(string userName)
        {
            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userName}'.");
            }

            return user;
        }

        [Route("register")]
        [HttpPost]
        public object RegisterUser([FromBody] UserInfo user)
        {
            if (string.IsNullOrEmpty(user.Username) ||
                string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(user.Password))
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Username, Email, and Password all must be provided, and not null/empty."
                );
            }

            if (!FieldChecking.IsWellformedEmailAddress(user.Email, Strings.Regex.email))
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, $@"Malformed email address '{user.Email}'."
                );
            }

            if (!FieldChecking.IsValidUsername(user.Username, Strings.Regex.username))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid username '{user.Username}'.");
            }

            if (DbInterface.AccountExists(user.Username))
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, $@"Account already exists with username '{user.Username}'."
                );
            }
            else
            {
                if (DbInterface.EmailInUse(user.Email))
                {
                    return Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest, $@"Account already with email '{user.Email}'."
                    );
                }
                else
                {
                    DbInterface.CreateAccount(null, user.Username, user.Password.ToUpper().Trim(), user.Email);

                    return new
                    {
                        Username = user.Username,
                        Email = user.Email,
                    };
                }
            }
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

            return Database.PlayerData.User.Find(userName)?.Players;
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

            return Database.PlayerData.User.Find(userId)?.Players;
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

            return Database.PlayerData.User.Find(userName)
                ?.Players?.FirstOrDefault(player => string.Equals(player?.Name, playerName, StringComparison.Ordinal));
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

            return Database.PlayerData.User.Find(userId)?.Players?.FirstOrDefault(player => player?.Id == playerId);
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

            return Database.PlayerData.User.Find(userId)?.Players?.Skip(index).FirstOrDefault();
        }

        #region "Change Email"

        [Route("{userName}/manage/email/change")]
        [ConfigurableAuthorize(Roles = nameof(ApiRoles.UserManage))]
        [HttpPost]
        public object UserChangeEmailByName(string userName, [FromBody] AdminChange authorizedChange)
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

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userName}'.");
            }

            if (DbInterface.EmailInUse(email))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, @"Email address already in use.");
            }

            user.Email = email;

            DbInterface.SavePlayerDatabaseAsync();

            return user;
        }

        [Route("{userId:guid}/manage/email/change")]
        [ConfigurableAuthorize(Roles = nameof(ApiRoles.UserManage))]
        [HttpPost]
        public object UserChangeEmailById(Guid userId, [FromBody] AdminChange authorizedChange)
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

            var user = Database.PlayerData.User.Find(userId);

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with id '{userId}'.");
            }

            if (DbInterface.EmailInUse(email))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, @"Email address already in use.");
            }

            user.Email = email;

            DbInterface.SavePlayerDatabaseAsync();

            return user;
        }

        [Route("{userName}/email/change")]
        [HttpPost]
        public object UserChangeEmailByName(string userName, [FromBody] AuthorizedChange authorizedChange)
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

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userName}'.");
            }

            if (!user.IsPasswordValid(authorizedChange.Authorization.ToUpper().Trim()))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, @"Invalid credentials.");
            }

            if (DbInterface.EmailInUse(email))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, @"Email address already in use.");
            }

            user.Email = email;

            DbInterface.SavePlayerDatabaseAsync();

            return user;
        }

        [Route("{userId:guid}/email/change")]
        [HttpPost]
        public object UserChangeEmailById(Guid userId, [FromBody] AuthorizedChange authorizedChange)
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

            var user = Database.PlayerData.User.Find(userId);

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with id '{userId}'.");
            }

            if (!user.IsPasswordValid(authorizedChange.Authorization.ToUpper().Trim()))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, @"Invalid credentials.");
            }

            if (DbInterface.EmailInUse(email))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, @"Email address already in use.");
            }

            user.Email = email;

            DbInterface.SavePlayerDatabaseAsync();

            return user;
        }

        #endregion

        #region "Validate Password"

        [Route("{userName}/password/validate")]
        [HttpPost]
        public object UserValidatePasswordByName(string userName, [FromBody] PasswordValidation data)
        {
            if (string.IsNullOrWhiteSpace(data.Password))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"No password provided.");
            }

            if (!Regex.IsMatch(data.Password.ToUpper().Trim(), "^[0-9A-Fa-f]{64}$", RegexOptions.Compiled))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Did not receive a valid password.");
            }

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userName}'.");
            }

            if (user.IsPasswordValid(data.Password.ToUpper().Trim()))
            {
                return Request.CreateMessageResponse(HttpStatusCode.OK, "Password Correct");
            }

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid credentials.");
        }

        [Route("{userId:guid}/password/validate")]
        [HttpPost]
        public object UserValidatePasswordById(Guid userId, [FromBody] PasswordValidation data)
        {
            if (string.IsNullOrWhiteSpace(data.Password))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"No password provided.");
            }

            if (!Regex.IsMatch(data.Password.ToUpper().Trim(), "^[0-9A-Fa-f]{64}$", RegexOptions.Compiled))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Did not receive a valid password.");
            }

            var user = Database.PlayerData.User.Find(userId);

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userId}'.");
            }

            if (user.IsPasswordValid(data.Password.ToUpper().Trim()))
            {
                return Request.CreateMessageResponse(HttpStatusCode.OK, "Password Correct");
            }

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid credentials.");
        }

        #endregion

        #region "Change Password"

        [Route("{userName}/manage/password/change")]
        [ConfigurableAuthorize(Roles = nameof(ApiRoles.UserManage))]
        [HttpPost]
        public object UserChangePassword(string userName, [FromBody] AdminChange authorizedChange)
        {
            if (!authorizedChange.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid payload");
            }

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userName}'.");
            }

            if (!user.TrySetPassword(authorizedChange.New.ToUpper().Trim()))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, @"Failed to update password.");
            }

            DbInterface.SavePlayerDatabaseAsync();

            return "Password updated.";
        }

        [Route("{userId:guid}/manage/password/change")]
        [ConfigurableAuthorize(Roles = nameof(ApiRoles.UserManage))]
        [HttpPost]
        public object UserChangePassword(Guid userId, [FromBody] AdminChange authorizedChange)
        {
            if (!authorizedChange.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid payload");
            }

            var user = Database.PlayerData.User.Find(userId);

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userId}'.");
            }

            if (!user.TrySetPassword(authorizedChange.New.ToUpper().Trim()))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, @"Failed to update password.");
            }

            DbInterface.SavePlayerDatabaseAsync();

            return Request.CreateMessageResponse(HttpStatusCode.OK, "Password Updated.");
        }

        [Route("{userName}/password/change")]
        [HttpPost]
        public object UserChangePassword(string userName, [FromBody] AuthorizedChange authorizedChange)
        {
            if (!authorizedChange.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid payload");
            }

            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userName}'.");
            }

            if (!user.TryChangePassword(
                authorizedChange.Authorization.ToUpper().Trim(), authorizedChange.New.ToUpper().Trim()
            ))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, @"Invalid credentials.");
            }

            DbInterface.SavePlayerDatabaseAsync();

            return Request.CreateMessageResponse(HttpStatusCode.OK, "Password Updated.");
        }

        [Route("{userId:guid}/password/change")]
        [HttpPost]
        public object UserChangePassword(Guid userId, [FromBody] AuthorizedChange authorizedChange)
        {
            if (!authorizedChange.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid payload");
            }

            var user = Database.PlayerData.User.Find(userId);

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userId}'.");
            }

            if (!user.TryChangePassword(
                authorizedChange.Authorization.ToUpper().Trim(), authorizedChange.New.ToUpper().Trim()
            ))
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, @"Invalid credentials.");
            }

            DbInterface.SavePlayerDatabaseAsync();

            return "Password updated.";
        }

        #endregion

        #region "Request Reset Email Password"

        [Route("{userName}/password/reset")]
        [HttpGet]
        public object UserSendPasswordResetEmailByName(string userName)
        {
            var user = Database.PlayerData.User.Find(userName);

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userName}'.");
            }

            if (Options.Smtp.IsValid())
            {
                var email = new PasswordResetEmail(user);
                email.Send();

                return Request.CreateMessageResponse(HttpStatusCode.OK, "Password reset email sent.");
            }
            else
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    "Could not send password reset email, SMTP settings on the server are not configured!"
                );
            }
        }

        [Route("{userId:guid}/password/reset")]
        [HttpGet]
        public object UserSendPasswordResetEmailById(Guid userId)
        {
            var user = Database.PlayerData.User.Find(userId);

            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with name '{userId}'.");
            }

            if (Options.Smtp.IsValid())
            {
                var email = new PasswordResetEmail(user);
                email.Send();

                return Request.CreateMessageResponse(HttpStatusCode.OK, "Password reset email sent.");
            }
            else
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    "Could not send password reset email, SMTP settings on the server are not configured!"
                );
            }
        }

        #endregion

        #region "Admin Action"

        [Route("{userId:guid}/admin/{act}")]
        [HttpPost]
        public object DoAdminActionOnPlayerById(
            Guid userId,
            string act,
            [FromBody] AdminActionParameters actionParameters
        )
        {
            AdminActions adminAction;
            if (!Enum.TryParse<AdminActions>(act, true, out adminAction))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid action.");
            }

            if (Guid.Empty == userId)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid user id '{userId}'.");
            }

            Tuple<Client, User> fetchResult;
            fetchResult = Database.PlayerData.User.Fetch(userId);

            return DoAdminActionOnUser(
                () => fetchResult,
                () => Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with id '{userId}'."),
                adminAction, actionParameters
            );
        }

        [Route("{userName}/admin/{act}")]
        [HttpPost]
        public object DoAdminActionOnPlayerByName(
            string userName,
            string act,
            [FromBody] AdminActionParameters actionParameters
        )
        {
            AdminActions adminAction;
            if (!Enum.TryParse<AdminActions>(act, true, out adminAction))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid action.");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid user name '{userName}'.");
            }

            Tuple<Client, User> fetchResult;
            fetchResult = Database.PlayerData.User.Fetch(userName);

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
                    if (string.IsNullOrEmpty(Ban.CheckBan(user, "")))
                    {
                        Ban.Add(
                            user.Id, actionParameters.Duration, actionParameters.Reason ?? "",
                            actionParameters.Moderator ?? @"api", actionParameters.Ip ? targetIp : ""
                        );

                        client?.Disconnect();
                        PacketSender.SendGlobalMsg(Strings.Account.banned.ToString(user.Name));

                        return Request.CreateMessageResponse(
                            HttpStatusCode.OK, Strings.Account.banned.ToString(user.Name)
                        );
                    }
                    else
                    {
                        return Request.CreateMessageResponse(
                            HttpStatusCode.BadRequest, Strings.Account.alreadybanned.ToString(user.Name)
                        );
                    }

                case AdminActions.UnBan:
                    Ban.Remove(user.Id);
                    PacketSender.SendGlobalMsg(Strings.Account.unbanned.ToString(user.Name));

                    return Request.CreateMessageResponse(
                        HttpStatusCode.OK, Strings.Account.unbanned.ToString(user.Name)
                    );

                case AdminActions.Mute:
                    if (string.IsNullOrEmpty(Mute.FindMuteReason(user.Id, "")))
                    {
                        Mute.Add(
                            user, actionParameters.Duration, actionParameters.Reason ?? "",
                            actionParameters.Moderator ?? @"api", actionParameters.Ip ? targetIp : ""
                        );

                        PacketSender.SendGlobalMsg(Strings.Account.muted.ToString(user.Name));

                        return Request.CreateMessageResponse(
                            HttpStatusCode.OK, Strings.Account.muted.ToString(user.Name)
                        );
                    }
                    else
                    {
                        return Request.CreateMessageResponse(
                            HttpStatusCode.BadRequest, Strings.Account.alreadymuted.ToString(user.Name)
                        );
                    }

                case AdminActions.UnMute:
                    Mute.Remove(user);
                    PacketSender.SendGlobalMsg(Strings.Account.unmuted.ToString(user.Name));

                    return Request.CreateMessageResponse(
                        HttpStatusCode.OK, Strings.Account.unmuted.ToString(user.Name)
                    );

                case AdminActions.WarpTo:
                    if (player != null)
                    {
                        var mapId = actionParameters.MapId == Guid.Empty ? player.MapId : actionParameters.MapId;
                        player.Warp(mapId, (byte) player.X, (byte) player.Y);

                        return Request.CreateMessageResponse(
                            HttpStatusCode.OK, $@"Warped '{player.Name}' to {mapId} ({player.X}, {player.Y})."
                        );
                    }

                    break;

                case AdminActions.WarpToLoc:
                    if (player != null)
                    {
                        var mapId = actionParameters.MapId == Guid.Empty ? player.MapId : actionParameters.MapId;
                        player.Warp(mapId, actionParameters.X, actionParameters.Y, true);

                        return Request.CreateMessageResponse(
                            HttpStatusCode.OK,
                            $@"Warped '{player.Name}' to {mapId} ({actionParameters.X}, {actionParameters.Y})."
                        );
                    }

                    break;

                case AdminActions.Kick:
                    if (client != null)
                    {
                        client.Disconnect(actionParameters.Reason);
                        PacketSender.SendGlobalMsg(Strings.Player.serverkicked.ToString(player?.Name));

                        return Request.CreateMessageResponse(
                            HttpStatusCode.OK, Strings.Player.serverkicked.ToString(player?.Name)
                        );
                    }

                    break;

                case AdminActions.Kill:
                    if (client != null && client.Entity != null)
                    {
                        client.Entity.Die();
                        PacketSender.SendGlobalMsg(Strings.Player.serverkilled.ToString(player?.Name));

                        return Request.CreateMessageResponse(
                            HttpStatusCode.OK, Strings.Commandoutput.killsuccess.ToString(player?.Name)
                        );
                    }

                    break;

                case AdminActions.WarpMeTo:
                case AdminActions.WarpToMe:
                    return Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest, $@"'{adminAction.ToString()}' not supported by the API."
                    );

                case AdminActions.SetSprite:
                case AdminActions.SetFace:
                case AdminActions.SetAccess:
                default:
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, adminAction.ToString());
            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, Strings.Player.offline);
        }

        #endregion

    }

}
