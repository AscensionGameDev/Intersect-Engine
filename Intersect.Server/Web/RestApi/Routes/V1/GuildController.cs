using System.Net;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Collections.Indexing;
using Intersect.Server.Collections.Sorting;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities;
using Intersect.Server.Localization;
using Intersect.Server.Web.Http;
using Intersect.Server.Web.RestApi.Types;
using Intersect.Server.Web.RestApi.Types.Guild;
using Intersect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [Route("api/v1/guilds")]
    [Authorize]
    public sealed partial class GuildController : IntersectController
    {
        #region Lookup

        [HttpGet]
        [ProducesResponseType(typeof(DataPage<KeyValuePair<Guild, int>>), (int)HttpStatusCode.OK, ContentTypes.Json)]
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

            var values = Guild.List(search?.Length > 2 ? search : null, sortBy, sortDirection, page * pageSize, pageSize, out int total);

            if (limit != pageSize)
            {
                values = values.Take(limit).ToList();
            }

            return Ok(new DataPage<KeyValuePair<Guild, int>>(
                Total: total,
                Page: page,
                PageSize: pageSize,
                Count: values.Count,
                Values: values
            ));
        }

        [HttpGet("{guildId:guid}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(Guild), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult GuildGet(Guid guildId)
        {
            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return BadRequest($@"Guild not found: ${guildId}.");
            }

            return Ok(guild);
        }

        #endregion

        #region Basic Operations

        [HttpPost("{guildId:guid}/name")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(Guild), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult ChangeName(Guid guildId, [FromBody] NameChangePayload change)
        {
            if (!FieldChecking.IsValidGuildName(change.Name, Strings.Regex.GuildName))
            {
                return BadRequest($@"Invalid guild name.");
            }

            var guild = Guild.LoadGuild(guildId);
            if (guild == null)
            {
                return NotFound($@"No guild with id '{guildId}'.");
            }

            if (guild.Rename(change.Name))
            {
                return Ok(guild);
            }

            return BadRequest($@"Invalid name, or name already taken.");
        }

        [Route("{guildId:guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(Guild), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult DisbandGuild(Guid guildId)
        {
            var guild = Guild.LoadGuild(guildId);
            if (guild == null)
            {
                return NotFound($@"No guild with id '{guildId}'.");
            }

            Guild.DeleteGuild(guild);
            return Ok(guild);
        }

        [HttpGet("{guildId:guid}/members")]
        [ProducesResponseType(typeof(DataPage<Player>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult Members(
            Guid guildId,
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

            var values = Player.List(search?.Length > 2 ? search : null, sortBy, sortDirection, page * pageSize, pageSize, out int total, guildId);

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

        [HttpPost("{guildId:guid}/kick/{lookupKey:LookupKey}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(Player), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult Kick(Guid guildId, LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            var guild = Guild.LoadGuild(guildId);
            if (guild == null)
            {
                return NotFound($@"Guild does not exist with id {guildId}.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"Player not found with lookup key {lookupKey}.");
            }

            if (!guild.IsMember(player.Id))
            {
                return BadRequest($@"{player.Name} is not a member of {guild.Name}.");
            }

            if (player.GuildRank == 0)
            {
                return BadRequest($@"Cannot kick a guild owner, transfer ownership first.");
            }

            guild.TryRemoveMember(player.Id, player, default, Database.Logging.Entities.GuildHistory.GuildActivityType.Kicked);
            return Ok(player);
        }

        [HttpPost("{guildId:guid}/rank/{lookupKey:LookupKey}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(Player), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult SetRank(Guid guildId, LookupKey lookupKey, [FromBody] GuildRankPayload guildRank)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            if (guildRank.Rank <= 0 || guildRank.Rank >= Options.Instance.Guild.Ranks.Length)
            {
                return BadRequest($@"Invalid guild rank, should be > 0 and < {Options.Instance.Guild.Ranks.Length}.");
            }

            var guild = Guild.LoadGuild(guildId);
            if (guild == null)
            {
                return NotFound($@"Guild does not exist with id {guildId}.");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return NotFound($@"Player not found with lookup key {lookupKey}.");
            }

            if (!guild.IsMember(player.Id))
            {
                return BadRequest($@"{player.Name} is not a member of {guild.Name}.");
            }

            if (player.GuildRank == 0)
            {
                return BadRequest($@"Cannot change rank of the guild owner, transfer ownership first!");
            }

            guild.SetPlayerRank(player.Id, player, guildRank.Rank, default);
            return Ok(player);
        }

        [HttpPost("{guildId:guid}/transfer/{lookupKey:LookupKey}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(Player), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult Transfer(Guid guildId, LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            var guild = Guild.LoadGuild(guildId);
            if (guild == null)
            {
                return NotFound($@"No guild found with id {guildId}");
            }

            if (!Player.TryFetch(lookupKey, out var player))
            {
                return BadRequest($@"Player not found with lookup key {lookupKey}.");
            }

            if (!guild.IsMember(player.Id))
            {
                return BadRequest($@"{player.Name} is not a member of {guild.Name}.");
            }

            if (player.GuildRank == 0)
            {
                return BadRequest($@"Cannot transfer ownership of a guild to ones self.");
            }

            if (!guild.TransferOwnership(player))
            {
                return BadRequest($@"Failed to transfer ownership.");
            }

            return Ok(player);
        }

        [HttpGet("{guildId:guid}/bank")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(IEnumerable<GuildBankSlot>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult ItemsListBank(Guid guildId)
        {
            var guild = Guild.LoadGuild(guildId);
            if (guild == null)
            {
                return NotFound($@"No guild found with id {guildId}");
            }

            return Ok(guild.Bank.Where(slot => !slot.IsEmpty));
        }

        #endregion

        #region Variables

        [HttpGet("{guildId:guid}/variables")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(IEnumerable<GuildVariable>), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult GuildVariablesList(Guid guildId)
        {
            if (!Guild.TryGet(guildId, out var guild))
            {
                return NotFound($@"No guild found with id {guildId}");
            }

            return Ok(guild.Variables);
        }

        [HttpGet("{guildId:guid}/variables/{variableId:guid}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(GuildVariable), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult GuildVariableGet(Guid guildId, Guid variableId)
        {
            if (!Guild.TryGet(guildId, out var guild))
            {
                return NotFound($@"No guild found with id {guildId}");
            }

            if (variableId == Guid.Empty)
            {
                return BadRequest($@"Variable id cannot be {variableId}");
            }

            if (!GuildVariableBase.TryGet(variableId, out var variableDescriptor))
            {
                return NotFound($@"Variable not found for id {variableId}");
            }

            var variable = guild.GetVariable(variableDescriptor.Id, true);
            return Ok(variable);
        }

        [HttpGet("{guildId:guid}/variables/{variableId:guid}/value")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(VariableValueBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult GuildVariableGetValue(Guid guildId, Guid variableId)
        {
            if (!Guild.TryGet(guildId, out var guild))
            {
                return NotFound($@"No guild found with id {guildId}");
            }

            if (variableId == Guid.Empty)
            {
                return BadRequest($@"Variable id cannot be {variableId}");
            }

            if (!GuildVariableBase.TryGet(variableId, out var variableDescriptor))
            {
                return NotFound($@"Variable not found for id {variableId}");
            }

            var variable = guild.GetVariable(variableDescriptor.Id, true);
            return Ok(new VariableValueBody
            {
                Value = variable.Value.Value,
            });
        }

        [HttpPost("{guildId:guid}/variables/{variableId:guid}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(GuildVariable), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult GuildVariableSet(Guid guildId, Guid variableId, [FromBody] VariableValueBody valueBody)
        {
            if (!Guild.TryGet(guildId, out var guild))
            {
                return NotFound($@"No guild found with id {guildId}");
            }

            if (variableId == Guid.Empty)
            {
                return BadRequest($@"Variable id cannot be {variableId}");
            }

            if (!GuildVariableBase.TryGet(variableId, out var variableDescriptor))
            {
                return NotFound($@"Variable not found for id {variableId}");
            }

            var variable = guild.GetVariable(variableDescriptor.Id, true);

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
                guild.StartCommonEventsWithTriggerForAll(CommonEventTrigger.GuildVariableChange, string.Empty, variableId.ToString());
                _ = guild.UpdatedVariables.AddOrUpdate(
                    variableId,
                    variableDescriptor,
                    (_, _) => variableDescriptor
                );
            }

            return Ok(variable);
        }

        #endregion

        #region Obsolete

        [HttpPost]
        [Obsolete("The appropriate verb for retrieving a list of records is GET not POST")]
        public IActionResult ListPost([FromBody] PagingInfo pageInfo)
        {
            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.PageSize = Math.Max(Math.Min(pageInfo.PageSize, 100), 5);

            var entries = Guild.List(null, null, SortDirection.Ascending, pageInfo.Page * pageInfo.PageSize, pageInfo.PageSize, out int entryTotal);

            return Ok(new DataPage<KeyValuePair<Guild, int>>(
                Total: entryTotal,
                Page: pageInfo.Page,
                PageSize: pageInfo.PageSize,
                Count: entries.Count,
                Values: entries
            ));
        }

        #endregion
    }
}
