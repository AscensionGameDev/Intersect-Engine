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

using Intersect.GameObjects;
using Intersect.Server.Database;
using Intersect.Server.Database.GameData;

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

        [Route("{lookupKey:LookupKey}/items")]
        [HttpGet]
        public object ItemsList(LookupKey lookupKey)
        {
            return new
            {
                inventory = ItemsListInventory(lookupKey),
                bank = ItemsListBank(lookupKey)
            };
        }

        [Route("{lookupKey:LookupKey}/items/bank")]
        [HttpGet]
        public object ItemsListBank(LookupKey lookupKey)
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

                return player.Bank;
            }
        }

        [Route("{lookupKey:LookupKey}/items/inventory")]
        [HttpGet]
        public object ItemsListInventory(LookupKey lookupKey)
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

                return player.Items;
            }
        }

        [Route("{lookupKey:LookupKey}/items")]
        [HttpPost]
        public object ItemsGive(LookupKey lookupKey, [FromBody] Item item, bool bankOverflow = false)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (null == item)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid item payload.");
            }

            if (Guid.Empty == item.ItemId)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid item id.");
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

                if (!player.TryGiveItem(item, bankOverflow, true))
                {
                    return Request.CreateErrorResponse(
                        HttpStatusCode.InternalServerError, $@"Failed to give player {item.Quantity} of '{item.ItemId}'."
                    );
                }

                var quantityBank = player.CountItems(item.ItemId, false, true);
                var quantityInventory = player.CountItems(item.ItemId, true, false);
                return new
                {
                    id = item.ItemId,
                    quantity = new
                    {
                        total = quantityBank + quantityInventory,
                        bank = quantityBank,
                        inventory = quantityInventory
                    }
                };

            }
        }

        [Route("{lookupKey:LookupKey}/items/{slotIndex:int}")]
        [HttpDelete]
        public object ItemsTake(LookupKey lookupKey, int slotIndex, int quantity = -1)
        {
            // TODO(source): Makes sense to do this, but it wasn't requested yet
            throw new NotImplementedException("TODO(source)");
        }

        // I've actually commented out this variation of the ItemsTake endpoint
        // because it breaks the DELETE /players/<id|name>/items/<slot> format.
        // This may get scrapped before implementation.
        //[Route("items/inventory/{slotId:guid}")]
        //[HttpDelete]
        //public object ItemsTake(Guid slotId, int quantity = -1)
        //{
        //    // TODO(source): Makes sense to do this, but it wasn't requested yet
        //    throw new NotImplementedException("TODO(source)");
        //}

        // TODO: spells give/take/list

        [Route("{lookupKey:LookupKey}/spells")]
        [HttpGet]
        public object SpellsList(LookupKey lookupKey)
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

                return player.Spells;
            }
        }

        [Route("{lookupKey:LookupKey}/spells")]
        [HttpPost]
        public object SpellsTeach(LookupKey lookupKey, [FromBody] Spell spell)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (null == spell)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid spell payload.");
            }

            if (Guid.Empty == spell.SpellId)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid spell id.");
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

                if (player.TryTeachSpell(spell, true))
                {
                    return new
                    {
                        spell
                    };
                }

                return Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError, $@"Failed to teach player spell with id '{spell.SpellId}'."
                );
            }
        }

        [Route("{lookupKey:LookupKey}/spells")]
        [HttpDelete]
        public object SpellsTake(LookupKey lookupKey, [FromBody] Spell spell)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (null == spell)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid spell payload.");
            }

            if (Guid.Empty == spell.SpellId)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid spell id.");
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

                if (player.TryForgetSpell(spell, true))
                {
                    return new
                    {
                        spell
                    };
                }

                return Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError, $@"Failed to teach player spell with id '{spell.SpellId}'."
                );
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
