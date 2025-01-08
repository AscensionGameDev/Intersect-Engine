using System.Net;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.RestApi.Types;
using Intersect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    public partial struct AdminActionParameters
    {
        public string Moderator { get; set; }

        public int Duration { get; set; }

        public bool Ip { get; set; }

        public string Reason { get; set; }

        public byte X { get; set; }

        public byte Y { get; set; }

        public Guid MapId { get; set; }
    }

    [Route("api/v1/players")]
    [Authorize]
    public sealed partial class PlayerController : IntersectController
    {

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

        [HttpGet]
        public DataPage<Player> List(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] int limit = PAGE_SIZE_MAX,
            [FromQuery] string sortBy = null,
            [FromQuery] SortDirection sortDirection = SortDirection.Ascending,
            [FromQuery] string search = null
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

        [HttpGet("rank")]
        public DataPage<Player> Rank(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] int limit = PAGE_SIZE_MAX,
            [FromQuery] SortDirection sortDirection = SortDirection.Descending
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

        [HttpPost("online")]
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

        [HttpGet("online")]
        public DataPage<Player> Online(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] int limit = PAGE_SIZE_MAX,
            [FromQuery] string sortBy = null,
            [FromQuery] SortDirection sortDirection = SortDirection.Ascending,
            [FromQuery] string search = null
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, 100), 5);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            Sort.From(sortBy, sortDirection);
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

        [HttpGet("online/count")]
        public object OnlineCount()
        {
            return new
            {
                onlineCount = Globals.OnlineList?.Count ?? 0,
            };
        }

        [HttpGet("{lookupKey:LookupKey}")]
        public IActionResult LookupPlayer(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            var (_, player) = Player.Fetch(lookupKey, loadRelationships: true, loadBags: true);
            if (player != null)
            {
                return Ok(player);
            }

            return NotFound(lookupKey.HasId ? $@"No player with id '{lookupKey.Id}'." : $@"No player with name '{lookupKey.Name}'.");
        }

        [HttpDelete("{lookupKey:LookupKey}")]
        public object DeletePlayer(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            var (client, player) = Player.Fetch(lookupKey);
            if (player != null)
            {
                if (Player.FindOnline(player.Id) != null)
                {
                    return InternalServerError("Failed to delete player because they are online!");
                }

                var user = Database.PlayerData.User.FindById(player.UserId);
                if (user == null)
                {
                    return BadRequest($@"Failed to load user for {player.Name}!");
                }

                var matchingPlayer = user.Players.FirstOrDefault(p => p.Id == player.Id);
                if (matchingPlayer != default)
                {
                    if (!user.TryDeleteCharacter(matchingPlayer))
                    {
                        if (client.User == user)
                        {
                            client.LogAndDisconnect(player.Id, nameof(Database.PlayerData.User.TryDeleteCharacter));
                        }
                    }
                }

                return player;
            }

            return NotFound(lookupKey.HasId ? $@"No player with id '{lookupKey.Id}'." : $@"No player with name '{lookupKey.Name}'.");
        }

        [HttpPost("{lookupKey:LookupKey}/name")]
        public object ChangeName(LookupKey lookupKey, [FromBody] NameChange change)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (!FieldChecking.IsValidUsername(change.Name, Strings.Regex.Username))
            {
                return BadRequest($@"Invalid name.");
            }

            if (Player.PlayerExists(change.Name))
            {
                return BadRequest($@"Name already taken.");
            }

            var (_, player) = Player.Fetch(lookupKey);
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

            return NotFound(lookupKey.HasId ? $@"No player with id '{lookupKey.Id}'." : $@"No player with name '{lookupKey.Name}'.");
        }

        [HttpGet("{lookupKey:LookupKey}/variables")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest, "application/json")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound, "application/json")]
        [ProducesResponseType(typeof(IEnumerable<PlayerVariable>), (int)HttpStatusCode.OK, "application/json")]
        public IActionResult PlayerVariablesList(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Player.TryFetch(lookupKey, out var fetchResult))
            {
                return NotFound(
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            var (_, player) = fetchResult;

            return Ok(player.Variables);
        }

        [HttpGet("{lookupKey:LookupKey}/variables/{variableId:guid}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest, "application/json")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound, "application/json")]
        [ProducesResponseType(typeof(PlayerVariable), (int)HttpStatusCode.OK, "application/json")]
        public IActionResult PlayerVariableGet(LookupKey lookupKey, Guid variableId)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Player.TryFetch(lookupKey, out var fetchResult))
            {
                return NotFound(
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            var (_, player) = fetchResult;

            if (!PlayerVariableBase.TryGet(variableId, out var variableDescriptor))
            {
                return NotFound($@"Variable not found for id {variableId}");
            }

            var variable = player.GetVariable(variableDescriptor.Id, true);
            return Ok(variable);
        }

        [HttpGet("{lookupKey:LookupKey}/variables/{variableId:guid}/value")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest, "application/json")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound, "application/json")]
        [ProducesResponseType(typeof(VariableValueBody), (int)HttpStatusCode.OK, "application/json")]
        public IActionResult PlayerVariableValueGet(LookupKey lookupKey, Guid variableId)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Player.TryFetch(lookupKey, out var fetchResult))
            {
                return NotFound(
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            var (_, player) = fetchResult;

            if (!PlayerVariableBase.TryGet(variableId, out var variableDescriptor))
            {
                return NotFound($@"Variable not found for id {variableId}");
            }

            var variable = player.GetVariable(variableDescriptor.Id, true);
            return Ok(new VariableValueBody
            {
                Value = variable.Value.Value,
            });
        }

        [HttpPost("{lookupKey:LookupKey}/variables/{variableId:guid}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest, "application/json")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound, "application/json")]
        [ProducesResponseType(typeof(PlayerVariable), (int)HttpStatusCode.OK, "application/json")]
        public IActionResult PlayerVariableSet(LookupKey lookupKey, Guid variableId, [FromBody] VariableValueBody valueBody)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Player.TryFetch(lookupKey, out var fetchResult))
            {
                return NotFound(
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            var (_, player) = fetchResult;

            if (!PlayerVariableBase.TryGet(variableId, out var variableDescriptor))
            {
                return NotFound($@"Variable not found for id {variableId}");
            }

            var variable = player.GetVariable(variableDescriptor.Id, true);

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
                player.StartCommonEventsWithTrigger(CommonEventTrigger.PlayerVariableChange, string.Empty, variableId.ToString());
                using var context = DbInterface.CreatePlayerContext(false);
                context.Update(player);
                context.SaveChanges();
            }

            return Ok(variable);
        }

        [HttpPost("{lookupKey:LookupKey}/class")]
        public object PlayerClassSet(LookupKey lookupKey, [FromBody] ClassChange change)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (change.ClassId == Guid.Empty || ClassBase.Get(change.ClassId) == null)
            {
                return BadRequest($@"Invalid class id ${change.ClassId}.");
            }

            var (_, player) = Player.Fetch(lookupKey);
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

            return NotFound(lookupKey.HasId ? $@"No player with id '{lookupKey.Id}'." : $@"No player with name '{lookupKey.Name}'.");
        }

        [HttpPost("{lookupKey:LookupKey}/level")]
        public object PlayerLevelSet(LookupKey lookupKey, [FromBody] LevelChange change)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            var (_, player) = Player.Fetch(lookupKey);
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

            return NotFound(lookupKey.HasId ? $@"No player with id '{lookupKey.Id}'." : $@"No player with name '{lookupKey.Name}'.");
        }

        public struct ItemListResponse
        {
            public IEnumerable<BankSlot> Bank { get; init; }

            public IEnumerable<InventorySlot> Inventory { get; init; }
        }

        [HttpGet("{lookupKey:LookupKey}/items")]
        public IActionResult ItemsList(LookupKey lookupKey)
        {
            var (_, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return NotFound(
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            return Ok(
                new ItemListResponse
                {
                    Bank = player.Bank.Where(slot => !slot.IsEmpty),
                    Inventory = player.Items.Where(slot => !slot.IsEmpty),
                }
            );
        }

        [HttpGet("{lookupKey:LookupKey}/items/bank")]
        public IActionResult ItemsListBank(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            var (_, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return NotFound(
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            return Ok(player.Bank.Where(slot => !slot.IsEmpty));
        }

        [HttpGet("bag/{bagId:guid}")]
        public IActionResult GetPlayerBag(Guid bagId)
        {
            if (bagId == Guid.Empty)
            {
                return BadRequest(@"Invalid bag id.");
            }

            if (Bag.TryGetBag(bagId, out var bag))
            {
                return Ok(bag);
            }

            return NotFound(@"Bag does not exist.");
        }

        [HttpGet("{lookupKey:LookupKey}/items/inventory")]
        public IActionResult ItemsListInventory(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            var (_, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return NotFound(
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            return Ok(player.Items.Where(slot => !slot.IsEmpty));
        }

        [HttpPost("{lookupKey:LookupKey}/items/give")]
        public object ItemsGive(LookupKey lookupKey, [FromBody] ItemInfo itemInfo)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (ItemBase.Get(itemInfo.ItemId) == null)
            {
                return BadRequest(@"Invalid item id.");
            }

            if (itemInfo.Quantity < 1)
            {
                return BadRequest("Cannot give 0, or a negative amount of an item.");
            }

            var (_, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return NotFound(
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            if (!player.TryGiveItem(itemInfo.ItemId, itemInfo.Quantity, ItemHandling.Normal, itemInfo.BankOverflow, -1, true))
            {
                return InternalServerError($@"Failed to give player {itemInfo.Quantity} of '{itemInfo.ItemId}'.");
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

        [HttpPost("{lookupKey:LookupKey}/items/take")]
        public object ItemsTake(LookupKey lookupKey, [FromBody] ItemInfo itemInfo)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (itemInfo.Quantity < 1)
            {
                return BadRequest("Cannot take 0, or a negative amount of an item.");
            }

            var (_, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return NotFound(
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

            return InternalServerError($@"Failed to take {itemInfo.Quantity} of '{itemInfo.ItemId}' from player.");
        }

        [HttpGet("{lookupKey:LookupKey}/spells")]
        public IActionResult SpellsList(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            var (_, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return NotFound(
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                );
            }

            return Ok(player.Spells.Where(s => !s.IsEmpty));
        }

        [HttpPost("{lookupKey:LookupKey}/spells/teach")]
        public object SpellsTeach(LookupKey lookupKey, [FromBody] SpellInfo spell)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (SpellBase.Get(spell.SpellId) == null)
            {
                return BadRequest(@"Invalid spell id.");
            }

            var (_, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return NotFound(
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

            return InternalServerError($@"Failed to teach player spell with id '{spell.SpellId}'. They might already know it!");
        }

        [HttpPost("{lookupKey:LookupKey}/spells/forget")]
        public object SpellsTake(LookupKey lookupKey, [FromBody] SpellInfo spell)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (SpellBase.Get(spell.SpellId) == null)
            {
                return BadRequest(@"Invalid spell id.");
            }

            var (_, player) = Player.Fetch(lookupKey);
            if (player == null)
            {
                return NotFound(
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

            return InternalServerError($@"Failed to remove player spell with id '{spell.SpellId}'.");
        }

        [HttpPost("{lookupKey:LookupKey}/admin/{adminAction:AdminAction}")]
        public object DoAdminActionOnPlayerByLookupKey(
            LookupKey lookupKey,
            AdminAction adminAction,
            [FromBody] AdminActionParameters actionParameters
        )
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            Tuple<Client, Player> fetchResult;
            fetchResult = Player.Fetch(lookupKey);

            return DoAdminActionOnPlayer(
                () => fetchResult,
                () => NotFound(
                    lookupKey.HasId
                        ? $@"No player with id '{lookupKey.Id}'."
                        : $@"No player with name '{lookupKey.Name}'."
                ), adminAction, actionParameters
            );
        }

        private object DoAdminActionOnPlayer(
            Func<Tuple<Client, Player>> fetch,
            Func<IActionResult> onError,
            AdminAction adminAction,
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
            var targetIp = client?.Ip ?? string.Empty;

            var actionPerformer = IntersectUser;
            if (actionPerformer == default)
            {
                return onError();
            }

            switch (adminAction)
            {
                case AdminAction.Ban:
                    if (actionPerformer.Power.CompareTo(player.Power) < 0) // Authority Comparison.
                    {
                        // Inform to whoever performed the action that they are
                        // not allowed to do this due to the lack of authority over their target.
                        return BadRequest(Strings.Account.NotAllowed.ToString(player.Name));
                    }
                    else if (Ban.Find(userId) != null) // If the target is already banned.
                    {
                        return BadRequest(Strings.Account.AlreadyBanned.ToString(player.Name));
                    }

                    // If target is online, not yet banned and the banner has the authority to ban.
                    else
                    {
                        // Add ban
                        Ban.Add(
                            userId, actionParameters.Duration, actionParameters.Reason ?? string.Empty,
                            actionParameters.Moderator ?? @"api", actionParameters.Ip ? targetIp : string.Empty
                        );

                        // Disconnect the banned player.
                        client?.Disconnect();

                        // Sends a global chat message to every user online about the banned player.
                        PacketSender.SendGlobalMsg(Strings.Account.Banned.ToString(player.Name));

                        //  Inform to the API about the successful ban.
                        return Ok(Strings.Account.Banned.ToString(player.Name));
                    }

                case AdminAction.UnBan:
                    Ban.Remove(userId);
                    PacketSender.SendGlobalMsg(Strings.Account.UnbanSuccess.ToString(player.Name));

                    return Ok(Strings.Account.UnbanSuccess.ToString(player.Name));

                case AdminAction.Mute:
                    if (actionPerformer.Power.CompareTo(player.Power) < 0) // Authority Comparison.
                    {
                        // Inform to whoever performed the action that they are
                        // not allowed to do this due to the lack of authority over their target.
                        return BadRequest(Strings.Account.NotAllowed.ToString(player.Name));
                    }
                    else if (Mute.Find(userId) != null) // If the target is already muted.
                    {
                        return BadRequest(Strings.Account.AlreadyMuted.ToString(player.Name));
                    }

                    // If target is online, not yet muted and the action performer has the authority to mute.
                    else
                    {
                        if (user == null)
                        {
                            Mute.Add(
                                userId, actionParameters.Duration, actionParameters.Reason ?? string.Empty,
                                actionParameters.Moderator ?? @"api", actionParameters.Ip ? targetIp : string.Empty
                            );
                        }
                        else
                        {
                            Mute.Add(
                                user, actionParameters.Duration, actionParameters.Reason ?? string.Empty,
                                actionParameters.Moderator ?? @"api", actionParameters.Ip ? targetIp : string.Empty
                            );
                        }

                        PacketSender.SendGlobalMsg(Strings.Account.Muted.ToString(player.Name));

                        return Ok(Strings.Account.Muted.ToString(player.Name));
                    }

                case AdminAction.UnMute:
                    if (user == null)
                    {
                        Mute.Remove(userId);
                    }
                    else
                    {
                        Mute.Remove(user);
                    }

                    PacketSender.SendGlobalMsg(Strings.Account.UnmuteSuccess.ToString(player.Name));

                    return Ok(Strings.Account.UnmuteSuccess.ToString(player.Name));

                case AdminAction.WarpTo:
                    if (client?.Entity != null)
                    {
                        var mapId = actionParameters.MapId == Guid.Empty ? client.Entity.MapId : actionParameters.MapId;
                        client.Entity.Warp(mapId, (byte) client.Entity.X, (byte) client.Entity.Y);

                        return Ok($@"Warped '{player.Name}' to {mapId} ({client.Entity.X}, {client.Entity.Y}).");
                    }

                    break;

                case AdminAction.WarpToLoc:
                    if (client?.Entity != null)
                    {
                        var mapId = actionParameters.MapId == Guid.Empty ? client.Entity.MapId : actionParameters.MapId;
                        client.Entity.Warp(mapId, actionParameters.X, actionParameters.Y, true);

                        return Ok($@"Warped '{player.Name}' to {mapId} ({actionParameters.X}, {actionParameters.Y}).");
                    }

                    break;

                case AdminAction.Kick:
                    if (client != null)
                    {
                        if (actionPerformer.Power.CompareTo(player.Power) < 0) // Authority Comparison.
                        {
                            // Inform to whoever performed the action that they are
                            // not allowed to do this due to the lack of authority over their target.
                            return BadRequest(Strings.Account.NotAllowed.ToString(player.Name));
                        }
                        else
                        {
                            client.Disconnect(actionParameters.Reason);
                            PacketSender.SendGlobalMsg(Strings.Player.ServerKicked.ToString(player.Name));

                            return Ok(Strings.Player.ServerKicked.ToString(player.Name));
                        }
                    }

                    break;

                case AdminAction.Kill:
                    if (client != null && client.Entity != null)
                    {
                        if (actionPerformer.Power.CompareTo(player.Power) < 0) // Authority Comparison.
                        {
                            // Inform to whoever performed the action that they are
                            // not allowed to do this due to the lack of authority over their target.
                            return BadRequest(Strings.Account.NotAllowed.ToString(player.Name));
                        }
                        else
                        {
                            lock (client.Entity.EntityLock)
                            {
                                client.Entity.Die();
                            }

                            PacketSender.SendGlobalMsg(Strings.Player.ServerKilled.ToString(player.Name));

                            return Ok(Strings.Commandoutput.KillSuccess.ToString(player.Name));
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

            return NotFound(Strings.Player.Offline);
        }
    }
}
