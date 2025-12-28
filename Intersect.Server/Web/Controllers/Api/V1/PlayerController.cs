using System.Net;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.PlayerClass;
using Intersect.Framework.Core.GameObjects.Variables;
using Intersect.GameObjects;
using Intersect.Server.Collections.Indexing;
using Intersect.Server.Collections.Sorting;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Web.Http;
using Intersect.Server.Web.Types;
using Intersect.Server.Web.Types.Player;
using Intersect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.Controllers.Api.V1
{
    [Route("api/v1/players")]
    [Authorize]
    public sealed partial class PlayerController : IntersectController
    {
        #region Lookup

        [HttpGet]
        [ProducesResponseType(typeof(DataPage<Player>), (int)HttpStatusCode.OK, ContentTypes.Json)]
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
            limit = Math.Max(limit, 1);
            pageSize = Math.Clamp(pageSize, PagingInfo.MinPageSize, PagingInfo.MaxPageSize);

            var take = Math.Min(limit, pageSize);
            var values = Player.List(
                search?.Length > 2 ? search : null,
                sortBy,
                sortDirection,
                page * pageSize,
                take,
                out var total
            );

            return Ok(
                new DataPage<Player>(
                    Total: total,
                    Page: page,
                    PageSize: pageSize,
                    Count: values.Count,
                    Values: values
                )
            );
        }

        [HttpGet("rank")]
        [ProducesResponseType(typeof(DataPage<Player>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult Rank(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] int limit = PagingInfo.MaxPageSize,
            [FromQuery] SortDirection sortDirection = SortDirection.Descending
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, PagingInfo.MaxPageSize), PagingInfo.MinPageSize);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            var values = Player.Rank(page, pageSize, sortDirection).ToList();
            if (limit != pageSize)
            {
                values = values.Take(limit).ToList();
            }

            return Ok(new DataPage<Player>
            {
                Total = Player.Count(),
                Page = page,
                PageSize = pageSize,
                Count = values.Count,
                Values = values,
                Sort = [new Sort
                {
                    By = [
                        nameof(Player.Level),
                        nameof(Player.Exp),
                    ],
                    Direction = sortDirection,
                }],
            });
        }

        [HttpGet("online")]
        [ProducesResponseType(typeof(DataPage<Player>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult Online(
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

            _ = Sort.From(sortBy, sortDirection);
            var connectedPlayers = Player.OnlinePlayers;
            IEnumerable<Player> enumerable = connectedPlayers;

            if (!string.IsNullOrWhiteSpace(search))
            {
                enumerable = enumerable.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }

            var total = enumerable.Count();

            enumerable = (sortBy?.ToLower() ?? "") switch
            {
                "level" => sortDirection == SortDirection.Ascending
                    ? enumerable.OrderBy(u => u.Level).ThenBy(u => u.Exp)
                    : enumerable.OrderByDescending(u => u.Level).ThenByDescending(u => u.Exp),
                "onlinetime" => sortDirection == SortDirection.Ascending
                    ? enumerable.OrderBy(u => u.OnlineTime)
                    : enumerable.OrderByDescending(u => u.OnlineTime),
                _ => sortDirection == SortDirection.Ascending
                    ? enumerable.OrderBy(u => u.Name.ToUpper())
                    : enumerable.OrderByDescending(u => u.Name.ToUpper()),
            };

            var values = enumerable.Skip(page * pageSize).ToList() ?? new List<Player>();

            if (limit != pageSize)
            {
                values = values.Take(limit).ToList();
            }

            return Ok(new DataPage<Player>(
                Total: total,
                Page: page,
                PageSize: pageSize,
                Count: values.Count,
                Values: values
            ));
        }

        [HttpGet("online/count")]
        [ProducesResponseType(typeof(OnlineCountResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult OnlineCount() => Ok(new OnlineCountResponseBody(Player.OnlinePlayers.Count));

        #endregion

        #region Player Basic Operations

        [HttpGet("{lookupKey:LookupKey}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(Player), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult LookupPlayer(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (!Player.TryFetch(lookupKey, out var player, loadRelationships: true, loadBags: true))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            return Ok(player);
        }

        [HttpDelete("{lookupKey:LookupKey}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
        [ProducesResponseType(typeof(Player), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult DeletePlayer(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (!Player.TryFetch(lookupKey, out var client, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

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
            if (matchingPlayer != default && !user.TryDeleteCharacter(matchingPlayer) && client.User == user)
            {
                _ = client.LogAndDisconnect(player.Id, nameof(Database.PlayerData.User.TryDeleteCharacter));
            }

            return Ok(player);
        }

        [HttpPost("{lookupKey:LookupKey}/name")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(Player), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult ChangeName(LookupKey lookupKey, [FromBody] NameChangePayload change)
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

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            player.Name = change.Name;
            if (player.IsOnline)
            {
                PacketSender.SendEntityDataToProximity(player);
            }

            using (var context = DbInterface.CreatePlayerContext(false))
            {
                _ = context.Update(player);
                _ = context.SaveChanges();
            }

            return Ok(player);
        }

        #endregion

        #region Player Properties Change

        [HttpPost("{lookupKey:LookupKey}/class")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(Player), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult PlayerClassSet(LookupKey lookupKey, [FromBody] IdPayload change)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (!ClassDescriptor.TryGet(change.Id, out var _))
            {
                return BadRequest($@"Invalid class id {change.Id}.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            player.ClassId = change.Id;
            player.RecalculateStatsAndPoints();
            player.UnequipInvalidItems();
            if (player.IsOnline)
            {
                PacketSender.SendEntityDataToProximity(player);
            }

            using (var context = DbInterface.CreatePlayerContext(false))
            {
                _ = context.Update(player);
                _ = context.SaveChanges();
            }

            return Ok(player);
        }

        [HttpPost("{lookupKey:LookupKey}/level")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(Player), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult PlayerLevelSet(LookupKey lookupKey, [FromBody] LevelChangeRequestBody change)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            player.SetLevel(change.Level, true);
            if (player.IsOnline)
            {
                PacketSender.SendEntityDataToProximity(player);
            }

            using (var context = DbInterface.CreatePlayerContext(false))
            {
                _ = context.Update(player);
                _ = context.SaveChanges();
            }

            return Ok(player);
        }

        #endregion

        #region Player Items Management

        [HttpGet("{lookupKey:LookupKey}/items")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(ItemListResponse), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult ItemsList(LookupKey lookupKey)
        {
            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            return Ok(new ItemListResponse(player.Bank.Where(slot => !slot.IsEmpty), player.Items.Where(slot => !slot.IsEmpty)));
        }

        [HttpGet("{lookupKey:LookupKey}/items/bank")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(IEnumerable<BankSlot>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult ItemsListBank(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            return Ok(player.Bank.Where(slot => !slot.IsEmpty));
        }

        [HttpGet("bag/{bagId:guid}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(Bag), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult GetPlayerBag(Guid bagId)
        {
            if (bagId == Guid.Empty)
            {
                return BadRequest(@"Invalid bag id.");
            }

            if (!Bag.TryGetBag(bagId, out var bag))
            {
                return NotFound(@"Bag does not exist.");
            }

            return Ok(bag);
        }

        [HttpGet("{lookupKey:LookupKey}/items/inventory")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(IEnumerable<InventorySlot>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult ItemsListInventory(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            return Ok(player.Items.Where(slot => !slot.IsEmpty));
        }

        [HttpPost("{lookupKey:LookupKey}/items/give")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
        [ProducesResponseType(typeof(ItemsGiveResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult ItemsGive(LookupKey lookupKey, [FromBody] ItemInfoRequestBody itemInfo)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (!ItemDescriptor.TryGet(itemInfo.ItemId, out var descriptor))
            {
                return NotFound($"No item found with ID '{itemInfo.ItemId}'");
            }

            if (itemInfo.Quantity < 1)
            {
                return BadRequest("Cannot give 0, or a negative amount of an item.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            if (!player.TryGiveItem(itemInfo.ItemId, itemInfo.Quantity, ItemHandling.Normal, itemInfo.BankOverflow, -1, true))
            {
                return InternalServerError($@"Failed to give player {itemInfo.Quantity} of '{descriptor.Name}'.");
            }

            using (var context = DbInterface.CreatePlayerContext(false))
            {
                _ = context.Update(player);
                _ = context.SaveChanges();
            }

            var quantityBank = player.CountItems(itemInfo.ItemId, false, true);
            var quantityInventory = player.CountItems(itemInfo.ItemId, true, false);

            return Ok(
                new ItemsGiveResponseBody(
                    itemInfo.ItemId,
                    new ItemsGiveQuantityData(quantityBank + quantityInventory, quantityBank, quantityInventory)
                )
            );
        }

        [HttpPost("{lookupKey:LookupKey}/items/take")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
        [ProducesResponseType(typeof(ItemsTakeResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult ItemsTake(LookupKey lookupKey, [FromBody] ItemInfoRequestBody itemInfo)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (itemInfo.Quantity < 1)
            {
                return BadRequest("Cannot take 0, or a negative amount of an item.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            if (!ItemDescriptor.TryGet(itemInfo.ItemId, out var descriptor))
            {
                return NotFound($"No item found with ID '{itemInfo.ItemId}'");
            }

            if (!player.TryTakeItem(itemInfo.ItemId, itemInfo.Quantity))
            {
                return InternalServerError($@"Failed to take {itemInfo.Quantity} of '{descriptor.Name}' from player.");
            }

            using (var context = DbInterface.CreatePlayerContext(false))
            {
                _ = context.Update(player);
                _ = context.SaveChanges();
            }

            return Ok(new ItemsTakeResponseBody(itemInfo.ItemId, itemInfo.Quantity));
        }

        #endregion

        #region Player Spells Management

        [HttpGet("{lookupKey:LookupKey}/spells")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(IEnumerable<SpellSlot>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult SpellsList(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            return Ok(player.Spells.Where(s => !s.IsEmpty));
        }

        [HttpPost("{lookupKey:LookupKey}/spells/teach")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
        [ProducesResponseType(typeof(IdPayload), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult SpellsTeach(LookupKey lookupKey, [FromBody] IdPayload spell)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (SpellDescriptor.Get(spell.Id) == null)
            {
                return BadRequest(@"Invalid spell id.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            if (!player.TryTeachSpell(new Spell(spell.Id), true))
            {
                return InternalServerError($@"Failed to teach player spell with id '{spell.Id}'. They might already know it!");
            }

            using (var context = DbInterface.CreatePlayerContext(false))
            {
                _ = context.Update(player);
                _ = context.SaveChanges();
            }

            return Ok(spell);
        }

        [HttpPost("{lookupKey:LookupKey}/spells/forget")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
        [ProducesResponseType(typeof(IdPayload), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult SpellsTake(LookupKey lookupKey, [FromBody] IdPayload spell)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (SpellDescriptor.Get(spell.Id) == null)
            {
                return BadRequest(@"Invalid spell id.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            if (!player.TryForgetSpell(new Spell(spell.Id), true))
            {
                return InternalServerError($@"Failed to remove player spell with id '{spell.Id}'.");
            }

            using (var context = DbInterface.CreatePlayerContext(false))
            {
                _ = context.Update(player);
                _ = context.SaveChanges();
            }

            return Ok(spell);
        }

        #endregion

        #region Player Variables

        [HttpGet("{lookupKey:LookupKey}/variables")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(IEnumerable<PlayerVariable>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult PlayerVariablesList(LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            return Ok(player.Variables);
        }

        [HttpGet("{lookupKey:LookupKey}/variables/{variableId:guid}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(PlayerVariable), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult PlayerVariableGet(LookupKey lookupKey, Guid variableId)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            if (!PlayerVariableDescriptor.TryGet(variableId, out var variableDescriptor))
            {
                return NotFound($@"Variable not found for id {variableId}");
            }

            var variable = player.GetVariable(variableDescriptor.Id, true);
            return Ok(variable);
        }

        [HttpGet("{lookupKey:LookupKey}/variables/{variableId:guid}/value")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(VariableValueBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult PlayerVariableValueGet(LookupKey lookupKey, Guid variableId)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            if (!PlayerVariableDescriptor.TryGet(variableId, out var variableDescriptor))
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
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(PlayerVariable), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult PlayerVariableSet(LookupKey lookupKey, Guid variableId, [FromBody] VariableValueBody valueBody)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            if (!PlayerVariableDescriptor.TryGet(variableId, out var variableDescriptor))
            {
                return NotFound($@"Variable not found for id {variableId}");
            }

            var variable = player.GetVariable(variableDescriptor.Id, true);

            if (variable?.Value == null)
            {
                return InternalServerError("Variable value storage is missing.");
            }

            if (!VariableValueHelper.TryConvertValue(variableDescriptor.DataType, valueBody.Value, out object convertedValue, out string error))
            {
                return BadRequest(error);
            }

            if (!VariableValueHelper.Equals(variableDescriptor.DataType, variable.Value, convertedValue))
            {
                variable.Value.Value = convertedValue;
                player.StartCommonEventsWithTrigger(CommonEventTrigger.PlayerVariableChange, string.Empty, variableId.ToString());
                using var context = DbInterface.CreatePlayerContext(false);
                _ = context.Update(player);
                _ = context.SaveChanges();
            }

            return Ok(variable);
        }

        #endregion

        #region Admin Actions

        [HttpPost("{lookupKey:LookupKey}/admin/{adminAction:AdminAction}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotImplemented, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult DoAdminActionOnPlayerByLookupKey(
            LookupKey lookupKey,
            AdminAction adminAction,
            [FromBody] AdminActionParameters actionParameters
        )
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid id." : @"Invalid name.");
            }

            if (!Player.TryFetch(lookupKey, out var client, out var player))
            {
                return NotFound($@"No player found for lookup key '{lookupKey}'");
            }

            return DoAdminActionOnPlayer(
                client,
                player,
                () => NotFound($@"No player found for lookup key '{lookupKey}'"),
                adminAction,
                actionParameters
            );
        }

        private IActionResult DoAdminActionOnPlayer(
            Client client,
            Player player,
            Func<IActionResult> onError,
            AdminAction adminAction,
            AdminActionParameters actionParameters
        )
        {
            if (player == default)
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
                        _ = Ban.Add(
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
                    _ = Ban.Remove(userId);
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
                            _ = Mute.Add(
                                userId, actionParameters.Duration, actionParameters.Reason ?? string.Empty,
                                actionParameters.Moderator ?? @"api", actionParameters.Ip ? targetIp : string.Empty
                            );
                        }
                        else
                        {
                            _ = Mute.Add(
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
                        _ = Mute.Remove(userId);
                    }
                    else
                    {
                        _ = Mute.Remove(user);
                    }

                    PacketSender.SendGlobalMsg(Strings.Account.UnmuteSuccess.ToString(player.Name));

                    return Ok(Strings.Account.UnmuteSuccess.ToString(player.Name));

                case AdminAction.WarpTo:
                    if (client?.Entity != null)
                    {
                        var mapId = actionParameters.MapId == Guid.Empty ? client.Entity.MapId : actionParameters.MapId;
                        client.Entity.Warp(mapId, (byte)client.Entity.X, (byte)client.Entity.Y);

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

        #endregion

        #region Obsolete

        [HttpPost]
        [Obsolete("The appropriate verb for retrieving a list of records is GET not POST")]
        public IActionResult ListPost([FromBody] PagingInfo pageInfo)
        {
            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.PageSize = Math.Max(Math.Min(pageInfo.PageSize, 100), 5);

            var values = Player.List(
                null,
                null,
                SortDirection.Ascending,
                pageInfo.Page * pageInfo.PageSize,
                pageInfo.PageSize,
                out var entryTotal
            );

            return Ok(
                new DataPage<Player>
                {
                    Total = entryTotal,
                    Page = pageInfo.Page,
                    PageSize = pageInfo.PageSize,
                    Count = values.Count,
                    Values = values,
                }
            );
        }

        [HttpPost("online")]
        [Obsolete("The appropriate verb for retrieving a list of records is GET not POST")]
        public object OnlinePost([FromBody] PagingInfo pageInfo)
        {
            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.PageSize = Math.Max(Math.Min(pageInfo.PageSize, 100), 5);

            var connectedPlayers = Player.OnlinePlayers;
            var entries = connectedPlayers.Skip(pageInfo.Page * pageInfo.PageSize).Take(pageInfo.PageSize).ToList();

            return new DataPage<Player>
            {
                Total = connectedPlayers.Count,
                Page = pageInfo.Page,
                PageSize = pageInfo.PageSize,
                Count = entries?.Count ?? 0,
                Values = entries,
            };
        }

        #endregion
    }
}
