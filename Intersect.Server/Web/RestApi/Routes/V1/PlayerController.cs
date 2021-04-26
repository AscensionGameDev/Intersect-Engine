using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities;
using Intersect.Server.Extensions;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Web.RestApi.Attributes;
using Intersect.Server.Web.RestApi.Extensions;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.RestApi.Types;
using Intersect.Utilities;

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
    public sealed class PlayerController : IntersectApiController
    {

        [Route]
        [HttpPost]
        public object ListPost([FromBody] PagingInfo pageInfo)
        {
            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.Count = Math.Max(Math.Min(pageInfo.Count, 100), 5);

            int entryTotal = 0;
            var entries = Player.List(null, null, SortDirection.Ascending, pageInfo.Page * pageInfo.Count, pageInfo.Count, out entryTotal);

            return new
            {
                total = entryTotal,
                pageInfo.Page,
                count = entries.Count,
                entries
            };
        }

        [Route]
        [HttpGet]
        public DataPage<Player> List(
            [FromUri] int page = 0,
            [FromUri] int pageSize = 0,
            [FromUri] int limit = PAGE_SIZE_MAX,
            [FromUri] string sortBy = null,
            [FromUri] SortDirection sortDirection = SortDirection.Ascending,
            [FromUri] string search = null
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, 100), 5);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            int total = 0;
            var values = Player.List(search?.Length > 2 ? search : null, sortBy, sortDirection, page * pageSize, pageSize, out total);

            if (limit != pageSize)
            {
                values = values.Take(limit).ToList();
            }

            return new DataPage<Player>
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Count = values.Count,
                Values = values
            };
        }

        [Route("rank")]
        [HttpGet]
        public DataPage<Player> Rank(
            [FromUri] int page = 0,
            [FromUri] int pageSize = 0,
            [FromUri] int limit = PAGE_SIZE_MAX,
            [FromUri] SortDirection sortDirection = SortDirection.Descending
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, 100), 5);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            var values = Player.Rank(page, pageSize, sortDirection).ToList();
            if (limit != pageSize)
            {
                values = values.Take(limit).ToList();
            }

            return new DataPage<Player>
            {
                Total = Player.Count(),
                Page = page,
                PageSize = pageSize,
                Count = values.Count,
                Values = values,
                Extra = new
                {
                    sortDirection
                }
            };
        }

        [Route("online")]
        [HttpPost]
        public object OnlinePost([FromBody] PagingInfo pageInfo)
        {
            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.Count = Math.Max(Math.Min(pageInfo.Count, 100), 5);

            var entries = Globals.OnlineList?.Skip(pageInfo.Page * pageInfo.Count).Take(pageInfo.Count).ToList();

            return new
            {
                total = Globals.OnlineList?.Count ?? 0,
                pageInfo.Page,
                count = entries?.Count ?? 0,
                entries
            };
        }

        [Route("online")]
        [HttpGet]
        public DataPage<Player> Online(
            [FromUri] int page = 0,
            [FromUri] int pageSize = 0,
            [FromUri] int limit = PAGE_SIZE_MAX,
            [FromUri] string sortBy = null,
            [FromUri] SortDirection sortDirection = SortDirection.Ascending,
            [FromUri] string search = null
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, 100), 5);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            var sort = Sort.From(sortBy, sortDirection);
            IEnumerable<Player> enumerable = Globals.OnlineList ?? new List<Player>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                enumerable = enumerable.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }

            var total = enumerable.Count();

            switch (sortBy?.ToLower() ?? "")
            {
                case "level":
                    enumerable = sortDirection == SortDirection.Ascending ? enumerable.OrderBy(u => u.Level).ThenBy(u => u.Exp) : enumerable.OrderByDescending(u => u.Level).ThenByDescending(u => u.Exp);
                    break;
                case "onlinetime":
                    enumerable = sortDirection == SortDirection.Ascending ? enumerable.OrderBy(u => u.OnlineTime) : enumerable.OrderByDescending(u => u.OnlineTime);
                    break;
                case "name":
                default:
                    enumerable = sortDirection == SortDirection.Ascending ? enumerable.OrderBy(u => u.Name.ToUpper()) : enumerable.OrderByDescending(u => u.Name.ToUpper());
                    break;
            }

            var values = enumerable.Skip(page * pageSize).ToList() ?? new List<Player>();

            if (limit != pageSize)
            {
                values = values.Take(limit).ToList();
            }

            return new DataPage<Player>
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Count = values.Count,
                Values = values
            };
        }

        [Route("online/count")]
        [HttpGet]
        public object OnlineCount()
        {
            return new
            {
                onlineCount = Globals.OnlineList?.Count ?? 0,
            };
        }

        [Route("{lookupKey:LookupKey}")]
        [HttpGet]
        public object LookupPlayer(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            var (client, player) = Player.Fetch(lookupKey);
            if (player != null)
            {
                return player;
            }

            return Request.CreateErrorResponse(
                HttpStatusCode.NotFound,
                lookupKey.HasId ? $@"No player with id '{lookupKey.Id}'." : $@"No player with name '{lookupKey.Name}'."
            );
        }

        [Route("{lookupKey:LookupKey}")]
        [HttpDelete]
        public object DeletePlayer(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            var (client, player) = Player.Fetch(lookupKey);
            if (player != null)
            {
                if (Player.FindOnline(player.Id) != null)
                {
                    return Request.CreateErrorResponse(
                        HttpStatusCode.InternalServerError,
                        "Failed to delete player because they are online!"
                    );
                }

                var user = Database.PlayerData.User.Find(player.UserId);
                if (user == null)
                {
                    return Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        $@"Failed to load user for {player.Name}!"
                    );
                }

                user.DeleteCharacter(user.Players.FirstOrDefault(p => p.Id == player.Id));

                return player;
            }

            return Request.CreateErrorResponse(
                HttpStatusCode.NotFound,
                lookupKey.HasId ? $@"No player with id '{lookupKey.Id}'." : $@"No player with name '{lookupKey.Name}'."
            );
        }

        [Route("{lookupKey:LookupKey}/name")]
        [HttpPost]
        public object ChangeName(LookupKey lookupKey, [FromBody] NameChange change)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            if (!FieldChecking.IsValidUsername(change.Name, Strings.Regex.username))
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Invalid name."
                );
            }

            if (Player.PlayerExists(change.Name))
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Name already taken."
                );
            }

            var (client, player) = Player.Fetch(lookupKey);
            if (player != null)
            {
                player.Name = change.Name;
                if (player.Online)
                {
                    PacketSender.SendEntityDataToProximity(player);
                }

                using (var context = DbInterface.CreatePlayerContext(false))
                {
                    context.Update(player);
                    context.SaveChanges();
                }

                return player;
            }

            return Request.CreateErrorResponse(
                HttpStatusCode.NotFound,
                lookupKey.HasId ? $@"No player with id '{lookupKey.Id}'." : $@"No player with name '{lookupKey.Name}'."
            );
        }

        [Route("{lookupKey:LookupKey}/variables")]
        [HttpGet]
        public object PlayerVariableGet(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            var (client, player) = Player.Fetch(lookupKey);
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

        [Route("{lookupKey:LookupKey}/variables/{variableId:guid}")]
        [HttpGet]
        public object PlayerVariableGet(LookupKey lookupKey, Guid variableId)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            if (variableId == Guid.Empty || PlayerVariableBase.Get(variableId) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid variable id ${variableId}.");
            }

            var (client, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            return player.GetVariable(variableId, true);
        }

        [Route("{lookupKey:LookupKey}/variables/{variableId:guid}/value")]
        [HttpGet]
        public object PlayerVariableGetValue(LookupKey lookupKey, Guid variableId)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            if (variableId == Guid.Empty || PlayerVariableBase.Get(variableId) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid variable id ${variableId}.");
            }

            var (client, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            return new
            {
                value = player.GetVariable(variableId, true).Value.Value,
            };
        }

        [Route("{lookupKey:LookupKey}/variables/{variableId:guid}")]
        [HttpPost]
        public object PlayerVariableSet(LookupKey lookupKey, Guid variableId, [FromBody] VariableValue value)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            if (variableId == Guid.Empty || PlayerVariableBase.Get(variableId) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid variable id ${variableId}.");
            }

            var (client, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            var variable = player.GetVariable(variableId, true);

            var changed = true;
            if (variable?.Value != null)
            {
                if (variable?.Value?.Value != value.Value)
                {
                    changed = false;
                }
                variable.Value.Value = value.Value;
            }

            if (changed)
            {
                var plyr = Player.FindOnline(player.Id);
                if (plyr != null)
                {
                    player.StartCommonEventsWithTrigger(CommonEventTrigger.PlayerVariableChange, "", variableId.ToString());
                }
            }

            using (var context = DbInterface.CreatePlayerContext(false))
            {
                context.Update(player);
                context.SaveChanges();
            }

            return variable;
        }

        [Route("{lookupKey:LookupKey}/class")]
        [HttpPost]
        public object PlayerClassSet(LookupKey lookupKey, [FromBody] ClassChange change)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            if (change.ClassId == Guid.Empty || ClassBase.Get(change.ClassId) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid class id ${change.ClassId}.");
            }

            var (client, player) = Player.Fetch(lookupKey);
            if (player != null)
            {
                player.ClassId = change.ClassId;
                player.RecalculateStatsAndPoints();
                player.UnequipInvalidItems();
                if (player.Online)
                {
                    PacketSender.SendEntityDataToProximity(player);
                }

                using (var context = DbInterface.CreatePlayerContext(false))
                {
                    context.Update(player);
                    context.SaveChanges();
                }

                return player;
            }

            return Request.CreateErrorResponse(
                HttpStatusCode.NotFound,
                lookupKey.HasId ? $@"No player with id '{lookupKey.Id}'." : $@"No player with name '{lookupKey.Name}'."
            );
        }

        [Route("{lookupKey:LookupKey}/level")]
        [HttpPost]
        public object PlayerLevelSet(LookupKey lookupKey, [FromBody] LevelChange change)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            var (client, player) = Player.Fetch(lookupKey);
            if (player != null)
            {
                player.SetLevel(change.Level, true);
                if (player.Online)
                {
                    PacketSender.SendEntityDataToProximity(player);
                }

                using (var context = DbInterface.CreatePlayerContext(false))
                {
                    context.Update(player);
                    context.SaveChanges();
                }

                return player;
            }

            return Request.CreateErrorResponse(
                HttpStatusCode.NotFound,
                lookupKey.HasId ? $@"No player with id '{lookupKey.Id}'." : $@"No player with name '{lookupKey.Name}'."
            );
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
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            var (client, player) = Player.Fetch(lookupKey);
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

        [Route("bag/{bagId:guid}")]
        [HttpGet]
        public object ItemsListBag(Guid bagId)
        {
            if (bagId == Guid.Empty)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid bag id.");
            }

            var bag = Bag.GetBag(bagId);

            if (bag == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, @"Bag does not exist.");
            }
            else
            {
                return bag;
            }
        }

        [Route("{lookupKey:LookupKey}/items/inventory")]
        [HttpGet]
        public object ItemsListInventory(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            var (client, player) = Player.Fetch(lookupKey);
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

        [Route("{lookupKey:LookupKey}/items/give")]
        [HttpPost]
        public object ItemsGive(LookupKey lookupKey, [FromBody] ItemInfo itemInfo)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            if (ItemBase.Get(itemInfo.ItemId) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid item id.");
            }

            if (itemInfo.Quantity < 1)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, "Cannot give 0, or a negative amount of an item."
                );
            }

            var (client, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            if (!player.TryGiveItem(itemInfo.ItemId, itemInfo.Quantity, ItemHandling.Normal, itemInfo.BankOverflow, true))
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError,
                    $@"Failed to give player {itemInfo.Quantity} of '{itemInfo.ItemId}'."
                );
            }

            using (var context = DbInterface.CreatePlayerContext(false))
            {
                context.Update(player);
                context.SaveChanges();
            }

            var quantityBank = player.CountItems(itemInfo.ItemId, false, true);
            var quantityInventory = player.CountItems(itemInfo.ItemId, true, false);

            return new
            {
                id = itemInfo.ItemId,
                quantity = new
                {
                    total = quantityBank + quantityInventory,
                    bank = quantityBank,
                    inventory = quantityInventory
                }
            };
        }

        [Route("{lookupKey:LookupKey}/items/take")]
        [HttpPost]
        public object ItemsTake(LookupKey lookupKey, [FromBody] ItemInfo itemInfo)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            if (itemInfo.Quantity < 1)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, "Cannot take 0, or a negative amount of an item."
                );
            }

            var (client, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            if (player.TryTakeItem(itemInfo.ItemId, itemInfo.Quantity))
            {
                using (var context = DbInterface.CreatePlayerContext(false))
                {
                    context.Update(player);
                    context.SaveChanges();
                }

                return new
                {
                    itemInfo.ItemId,
                    itemInfo.Quantity
                };
            }

            return Request.CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                $@"Failed to take {itemInfo.Quantity} of '{itemInfo.ItemId}' from player."
            );
        }

        [Route("{lookupKey:LookupKey}/spells")]
        [HttpGet]
        public object SpellsList(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            var (client, player) = Player.Fetch(lookupKey);
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

        [Route("{lookupKey:LookupKey}/spells/teach")]
        [HttpPost]
        public object SpellsTeach(LookupKey lookupKey, [FromBody] SpellInfo spell)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            if (SpellBase.Get(spell.SpellId) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid spell id.");
            }

            var (client, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            if (player.TryTeachSpell(new Spell(spell.SpellId), true))
            {
                using (var context = DbInterface.CreatePlayerContext(false))
                {
                    context.Update(player);
                    context.SaveChanges();
                }

                return spell;
            }

            return Request.CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                $@"Failed to teach player spell with id '{spell.SpellId}'. They might already know it!"
            );
        }

        [Route("{lookupKey:LookupKey}/spells/forget")]
        [HttpPost]
        public object SpellsTake(LookupKey lookupKey, [FromBody] SpellInfo spell)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            if (SpellBase.Get(spell.SpellId) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid spell id.");
            }

            var (client, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            if (player.TryForgetSpell(new Spell(spell.SpellId), true))
            {
                using (var context = DbInterface.CreatePlayerContext(false))
                {
                    context.Update(player);
                    context.SaveChanges();
                }

                return spell;
            }

            return Request.CreateErrorResponse(
                HttpStatusCode.InternalServerError, $@"Failed to remove player spell with id '{spell.SpellId}'."
            );
        }

        [Route("{lookupKey:LookupKey}/admin/{act}")]
        [HttpPost]
        public object DoAdminActionOnPlayerByName(
            LookupKey lookupKey,
            string act,
            [FromBody] AdminActionParameters actionParameters
        )
        {
            if (!Enum.TryParse<AdminActions>(act, true, out var adminAction))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, @"Invalid action.");
            }

            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            Tuple<Client, Player> fetchResult;
            fetchResult = Player.Fetch(lookupKey);

            return DoAdminActionOnPlayer(
                () => fetchResult,
                () => Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                ), adminAction, actionParameters
            );
        }

        private object DoAdminActionOnPlayer(
            Func<Tuple<Client, Player>> fetch,
            Func<HttpResponseMessage> onError,
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
                    if (string.IsNullOrEmpty(Ban.CheckBan(user, "")))
                    {
                        Ban.Add(
                            userId, actionParameters.Duration, actionParameters.Reason ?? "",
                            actionParameters.Moderator ?? @"api", actionParameters.Ip ? targetIp : ""
                        );

                        client?.Disconnect();
                        PacketSender.SendGlobalMsg(Strings.Account.banned.ToString(player.Name));

                        return Request.CreateMessageResponse(
                            HttpStatusCode.OK, Strings.Account.banned.ToString(player.Name)
                        );
                    }
                    else
                    {
                        return Request.CreateMessageResponse(
                            HttpStatusCode.BadRequest, Strings.Account.alreadybanned.ToString(player.Name)
                        );
                    }

                case AdminActions.UnBan:
                    Ban.Remove(userId);
                    PacketSender.SendGlobalMsg(Strings.Account.unbanned.ToString(player.Name));

                    return Request.CreateMessageResponse(
                        HttpStatusCode.OK, Strings.Account.unbanned.ToString(player.Name)
                    );

                case AdminActions.Mute:
                    if (string.IsNullOrEmpty(Mute.FindMuteReason(userId, "")))
                    {
                        if (user == null)
                        {
                            Mute.Add(
                                userId, actionParameters.Duration, actionParameters.Reason ?? "",
                                actionParameters.Moderator ?? @"api", actionParameters.Ip ? targetIp : ""
                            );
                        }
                        else
                        {
                            Mute.Add(
                                user, actionParameters.Duration, actionParameters.Reason ?? "",
                                actionParameters.Moderator ?? @"api", actionParameters.Ip ? targetIp : ""
                            );
                        }

                        PacketSender.SendGlobalMsg(Strings.Account.muted.ToString(player.Name));

                        return Request.CreateMessageResponse(
                            HttpStatusCode.OK, Strings.Account.muted.ToString(player.Name)
                        );
                    }
                    else
                    {
                        return Request.CreateMessageResponse(
                            HttpStatusCode.BadRequest, Strings.Account.alreadymuted.ToString(player.Name)
                        );
                    }

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

                    return Request.CreateMessageResponse(
                        HttpStatusCode.OK, Strings.Account.unmuted.ToString(player.Name)
                    );

                case AdminActions.WarpTo:
                    if (client?.Entity != null)
                    {
                        var mapId = actionParameters.MapId == Guid.Empty ? client.Entity.MapId : actionParameters.MapId;
                        client.Entity.Warp(mapId, (byte) client.Entity.X, (byte) client.Entity.Y);

                        return Request.CreateMessageResponse(
                            HttpStatusCode.OK,
                            $@"Warped '{player.Name}' to {mapId} ({client.Entity.X}, {client.Entity.Y})."
                        );
                    }

                    break;

                case AdminActions.WarpToLoc:
                    if (client?.Entity != null)
                    {
                        var mapId = actionParameters.MapId == Guid.Empty ? client.Entity.MapId : actionParameters.MapId;
                        client.Entity.Warp(mapId, actionParameters.X, actionParameters.Y, true);

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
                        PacketSender.SendGlobalMsg(Strings.Player.serverkicked.ToString(player.Name));

                        return Request.CreateMessageResponse(
                            HttpStatusCode.OK, Strings.Player.serverkicked.ToString(player.Name)
                        );
                    }

                    break;

                case AdminActions.Kill:
                    if (client != null && client.Entity != null)
                    {
                        lock (client.Entity.EntityLock)
                        {
                            client.Entity.Die();
                        }
                        
                        PacketSender.SendGlobalMsg(Strings.Player.serverkilled.ToString(player.Name));

                        return Request.CreateMessageResponse(
                            HttpStatusCode.OK, Strings.Commandoutput.killsuccess.ToString(player.Name)
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

    }

}
