using Intersect.Enums;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Web.RestApi.Attributes;
using Intersect.Server.Web.RestApi.Extensions;
using Intersect.Server.Web.RestApi.Types;
using JetBrains.Annotations;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Intersect.Server.Web.RestApi.Routes.V1
{

    public struct AdminActionParameters
    {

        public string Moderator { get; set; }

        public int Duration { get; set; }

        public bool Ip { get; set; }

        public string Reason { get; set; }

        public byte X { get; set; }

        public byte Y { get; set; }

        public Guid MapId { get; set; }

    }

    [RoutePrefix("players")]
    [ConfigurableAuthorize]
    public sealed class PlayerController : ApiController
    {

        [Route]
        [HttpGet]
        public object List(int page = 0, int count = 10)
        {
            page = Math.Max(page, 0);
            count = Math.Max(Math.Min(count, 100), 5);

            using (var context = PlayerContext.Temporary)
            {
                var entries = Player.List(page, count, context).ToList();
                return new
                {
                    total = context?.Players.Count() ?? 0,
                    page,
                    count = entries.Count,
                    entries
                };
            }
        }

        [Route("online")]
        [HttpGet]
        public object Online(int page = 0, int count = 10)
        {
            page = Math.Max(page, 0);
            count = Math.Max(Math.Min(count, 100), 5);

            var entries = Globals.OnlineList?.Skip(page * count).Take(count).ToList();
            return new
            {
                total = Globals.OnlineList?.Count ?? 0,
                page,
                count = entries?.Count ?? 0,
                entries
            };
        }

        [Route("online/count")]
        [HttpGet]
        public int OnlineCount()
        {
            return Globals.OnlineList?.Count ?? 0;
        }

        [Route("{lookupKey:LookupKey}")]
        [HttpGet]
        public object LookupPlayer(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            using (var context = PlayerContext.Temporary)
            {
                var (client, player) = Player.Fetch(lookupKey, context);
                if (player != null)
                {
                    return player;
                }
            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, lookupKey.HasId ? $@"No player with id '{lookupKey.Id}'." : $@"No player with name '{lookupKey.Name}'.");
        }

        [Route("{lookupKey:LookupKey}/variables")]
        [HttpGet]
        public object PlayerVariableGet(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            using (var context = PlayerContext.Temporary)
            {
                var (client, player) = Player.Fetch(lookupKey, context);
                if (player == null)
                {
                    return Request.CreateErrorResponse(
                        HttpStatusCode.NotFound,
                        lookupKey.HasId
                            ? $@"No player with id '{lookupKey.Id}'."
                            : $@"No player with name '{lookupKey.Name}'."
                    );
                }

                return player.Variables;
            }
        }

        [Route("{lookupKey:LookupKey}/variables/{index:int}")]
        [HttpGet]
        public object PlayerVariableGet(LookupKey lookupKey, int index)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (index < 0)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid variable index ${index}.");
            }

            using (var context = PlayerContext.Temporary)
            {
                var (client, player) = Player.Fetch(lookupKey, context);
                if (player == null)
                {
                    return Request.CreateErrorResponse(
                        HttpStatusCode.NotFound,
                        lookupKey.HasId
                            ? $@"No player with id '{lookupKey.Id}'."
                            : $@"No player with name '{lookupKey.Name}'."
                    );
                }

                if (index >= player.Variables.Count)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Variable index {index} out of bounds ({player.Variables.Count} variables).");
                }

                return player.Variables[index];
            }
        }

        [Route("{lookupKey:LookupKey}/variables/{index:int}/value")]
        [HttpGet]
        public object PlayerVariableGetValue(LookupKey lookupKey, int index)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (index < 0)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid variable index ${index}.");
            }

            using (var context = PlayerContext.Temporary)
            {
                var (client, player) = Player.Fetch(lookupKey, context);
                if (player == null)
                {
                    return Request.CreateErrorResponse(
                        HttpStatusCode.NotFound,
                        lookupKey.HasId
                            ? $@"No player with id '{lookupKey.Id}'."
                            : $@"No player with name '{lookupKey.Name}'."
                    );
                }

                if (index >= player.Variables.Count)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Variable index {index} out of bounds ({player.Variables.Count} variables).");
                }

                return player.Variables[index]?.Value.Value;
            }
        }

        [Route("{lookupKey:LookupKey}/variable/{index:int}")]
        [HttpPost]
        public object PlayerVariableSet(LookupKey lookupKey, int index, [FromBody] dynamic value)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (index < 0)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid variable index ${index}.");
            }

            using (var context = PlayerContext.Temporary)
            {
                var (client, player) = Player.Fetch(lookupKey, context);
                if (player == null)
                {
                    return Request.CreateErrorResponse(
                        HttpStatusCode.NotFound,
                        lookupKey.HasId
                            ? $@"No player with id '{lookupKey.Id}'."
                            : $@"No player with name '{lookupKey.Name}'."
                    );
                }

                if (index >= player.Variables.Count)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Variable index ${index} out of bounds (${player.Variables.Count} variables).");
                }

                var variable = player.Variables[index];

                if (variable?.Value != null)
                {
                    variable.Value.Value = value;
                }

                return player.Variables[index];
            }
        }

        [Route("{lookupKey:LookupKey}/AdminActions/{adminAction:AdminActions}")]
        [HttpPost]
        public object DoAdminActionOnPlayerByName(
            LookupKey lookupKey,
            AdminActions adminAction,
            [FromBody] AdminActionParameters actionParameters
        )
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            Tuple<Client, Player> fetchResult;
            using (var context = PlayerContext.Temporary)
            {
                fetchResult = Player.Fetch(lookupKey, context);
            }

            return DoAdminActionOnPlayer(
                () => fetchResult,
                () => Request.CreateErrorResponse(HttpStatusCode.NotFound, lookupKey.HasId ? $@"No player with id '{lookupKey.Id}'." : $@"No player with name '{lookupKey.Name}'."),
                adminAction, actionParameters
            );
        }

        private object DoAdminActionOnPlayer(
            [NotNull] Func<Tuple<Client, Player>> fetch,
            [NotNull] Func<HttpResponseMessage> onError,
            AdminActions adminAction,
            AdminActionParameters actionParameters
        )
        {
            var (client, player) = fetch();

            if (player == null)
            {
                return onError();
            }

            var user = client?.User;
            var userId = user?.Id ?? player.UserId;
            var targetIp = client?.GetIp() ?? "";

            switch (adminAction)
            {
                case AdminActions.Ban:
                    Ban.Add(
                        userId,
                        actionParameters.Duration,
                        actionParameters.Reason ?? "",
                        actionParameters.Moderator ?? @"api",
                        actionParameters.Ip ? targetIp : ""
                    );
                    client?.Disconnect();
                    PacketSender.SendGlobalMsg(Strings.Account.banned.ToString(player.Name));
                    return Request.CreateMessageResponse(HttpStatusCode.OK, Strings.Account.banned.ToString(player.Name));

                case AdminActions.UnBan:
                    Ban.Remove(userId);
                    PacketSender.SendGlobalMsg(Strings.Account.unbanned.ToString(player.Name));
                    return Request.CreateMessageResponse(HttpStatusCode.OK, Strings.Account.unbanned.ToString(player.Name));

                case AdminActions.Mute:
                    if (user == null)
                    {
                        Mute.Add(
                            userId,
                            actionParameters.Duration,
                            actionParameters.Reason ?? "",
                            actionParameters.Moderator ?? @"api",
                            actionParameters.Ip ? targetIp : ""
                        );
                    }
                    else
                    {
                        Mute.Add(
                            user,
                            actionParameters.Duration,
                            actionParameters.Reason ?? "",
                            actionParameters.Moderator ?? @"api",
                            actionParameters.Ip ? targetIp : ""
                        );
                    }
                    PacketSender.SendGlobalMsg(Strings.Account.muted.ToString(player.Name));
                    return Request.CreateMessageResponse(HttpStatusCode.OK, Strings.Account.muted.ToString(player.Name));

                case AdminActions.UnMute:
                    if (user == null)
                    {
                        Mute.Remove(userId);
                    }
                    else
                    {
                        Mute.Remove(user);
                    }
                    PacketSender.SendGlobalMsg(Strings.Account.unmuted.ToString(player.Name));
                    return Request.CreateMessageResponse(HttpStatusCode.OK, Strings.Account.unmuted.ToString(player.Name));

                case AdminActions.WarpTo:
                    if (client?.Entity != null)
                    {
                        var mapId = actionParameters.MapId == Guid.Empty ? client.Entity.MapId : actionParameters.MapId;
                        client.Entity.Warp(mapId, client.Entity.X, client.Entity.Y);
                        return Request.CreateMessageResponse(HttpStatusCode.OK, $@"Warped '{player.Name}' to {mapId} ({client.Entity.X}, {client.Entity.Y}).");
                    }
                    break;

                case AdminActions.WarpToLoc:
                    if (client?.Entity != null)
                    {
                        var mapId = actionParameters.MapId == Guid.Empty ? client.Entity.MapId : actionParameters.MapId;
                        client.Entity.Warp(mapId, actionParameters.X, actionParameters.Y, true);
                        return Request.CreateMessageResponse(HttpStatusCode.OK, $@"Warped '{player.Name}' to {mapId} ({actionParameters.X}, {actionParameters.Y}).");
                    }
                    break;

                case AdminActions.Kick:
                    if (client != null)
                    {
                        client.Disconnect(actionParameters.Reason);
                        PacketSender.SendGlobalMsg(Strings.Player.serverkicked.ToString(player.Name));
                        return Request.CreateMessageResponse(HttpStatusCode.OK, Strings.Player.serverkicked.ToString(player.Name));
                    }
                    break;

                case AdminActions.Kill:
                    if (client != null)
                    {
                        client.Disconnect(actionParameters.Reason);
                        PacketSender.SendGlobalMsg(Strings.Player.serverkilled.ToString(player.Name));
                        return Request.CreateMessageResponse(HttpStatusCode.OK, Strings.Commandoutput.killsuccess.ToString(player.Name));
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

    }

}
