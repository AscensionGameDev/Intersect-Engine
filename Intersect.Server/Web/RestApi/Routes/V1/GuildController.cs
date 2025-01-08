using System.Net;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities;
using Intersect.Server.Localization;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.RestApi.Types;
using Intersect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [Route("api/v1/guilds")]
    [Authorize]
    public sealed partial class GuildController : IntersectController
    {
        [HttpPost]
        public object ListPost([FromBody] PagingInfo pageInfo)
        {
            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.Count = Math.Max(Math.Min(pageInfo.Count, 100), 5);

            var entries = Guild.List(null, null, SortDirection.Ascending, pageInfo.Page * pageInfo.Count, pageInfo.Count, out int entryTotal);

            return new
            {
                total = entryTotal,
                pageInfo.Page,
                count = entries.Count,
                entries
            };
        }

        [HttpGet]
        public DataPage<KeyValuePair<Guild, int>> List(
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

            var values = Guild.List(search?.Length > 2 ? search : null, sortBy, sortDirection, page * pageSize, pageSize, out int total);

            if (limit != pageSize)
            {
                values = values.Take(limit).ToList();
            }

            return new DataPage<KeyValuePair<Guild, int>>
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Count = values.Count,
                Values = values
            };
        }

        [HttpGet("{guildId:guid}")]
        public object GuildGet(Guid guildId)
        {
            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return BadRequest($@"Guild not found: ${guildId}.");
            }

            return guild;
        }

        [HttpPost("{guildId:guid}/name")]
        public object ChangeName(Guid guildId, [FromBody] NameChange change)
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
                return guild;
            }

            return BadRequest($@"Invalid name, or name already taken.");
        }

        [Route("{guildId:guid}")]
        [HttpDelete]
        public object DisbandGuild(Guid guildId)
        {
            var guild = Guild.LoadGuild(guildId);
            if (guild == null)
            {
                return NotFound($@"No guild with id '{guildId}'.");
            }

            Guild.DeleteGuild(guild);

            return guild;
        }

        [HttpGet("{guildId:guid}/members")]
        public DataPage<Player> Members(
            Guid guildId,
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

            var values = Player.List(search?.Length > 2 ? search : null, sortBy, sortDirection, page * pageSize, pageSize, out int total, guildId);

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

        [HttpPost("{guildId:guid}/kick/{lookupKey:LookupKey}")]
        public object Kick(Guid guildId, LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }


            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return BadRequest($@"Guild does not exist.");
            }

            var (client, player) = Player.Fetch(lookupKey);

            //Player not found
            if (player == null)
            {
                return BadRequest($@"Player not found.");
            }

            //Player is not a member of this guild
            if (!guild.IsMember(player.Id))
            {
                return BadRequest($@"{player.Name} is not a member of {guild.Name}.");
            }

            //Cannot kick the owner!
            if (player.GuildRank == 0)
            {
                return BadRequest($@"Cannot kick a guild owner, transfer ownership first.");
            }

            guild.RemoveMember(player.Id, player, default, Database.Logging.Entities.GuildHistory.GuildActivityType.Kicked);

            return player;
        }

        [HttpPost("{guildId:guid}/rank/{lookupKey:LookupKey}")]
        public object SetRank(Guid guildId, LookupKey lookupKey, [FromBody] GuildRank guildRank)
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
                return BadRequest($@"Guild does not exist.");
            }

            var (_, player) = Player.Fetch(lookupKey);

            if (player == null)
            {
                return BadRequest($@"Player not found.");
            }

            if (!guild.IsMember(player.Id))
            {
                return BadRequest($@"{player.Name} is not a member of {guild.Name}.");
            }

            // Cannot kick the owner!
            if (player.GuildRank == 0)
            {
                return BadRequest($@"Cannot change rank of the guild owner, transfer ownership first!");
            }

            guild.SetPlayerRank(player.Id, player, guildRank.Rank, default);

            return player;
        }

        [HttpPost("{guildId:guid}/transfer/{lookupKey:LookupKey}")]
        public object Transfer(Guid guildId, LookupKey lookupKey)
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

            var (_, player) = Player.Fetch(lookupKey);

            if (player == null)
            {
                return BadRequest($@"Player not found.");
            }

            if (!guild.IsMember(player.Id))
            {
                return BadRequest($@"{player.Name} is not a member of {guild.Name}.");
            }

            // Cannot kick the owner!
            if (player.GuildRank == 0)
            {
                return BadRequest($@"Cannot transfer ownership of a guild to ones self.");
            }

            guild.TransferOwnership(player);

            return player;
        }

        [HttpGet("{guildId:guid}/bank")]
        [ProducesResponseType(typeof(NotFoundObjectResult), (int)HttpStatusCode.NotFound, "application/json")]
        [ProducesResponseType(typeof(IEnumerable<GuildBankSlot>), (int)HttpStatusCode.OK, "application/json")]
        public IActionResult ItemsListBank(Guid guildId)
        {
            var guild = Guild.LoadGuild(guildId);
            if (guild == null)
            {
                return NotFound($@"No guild found with id {guildId}");
            }

            return Ok(guild.Bank.Where(slot => !slot.IsEmpty));
        }

        [HttpGet("{guildId:guid}/variables")]
        [ProducesResponseType(typeof(NotFoundObjectResult), (int)HttpStatusCode.NotFound, "application/json")]
        [ProducesResponseType(typeof(IEnumerable<GuildVariable>), (int)HttpStatusCode.OK, "application/json")]
        public IActionResult GuildVariablesList(Guid guildId)
        {
            if (!Guild.TryGet(guildId, out var guild))
            {
                return NotFound($@"No guild found with id {guildId}");
            }

            return Ok(guild.Variables);
        }

        [HttpGet("{guildId:guid}/variables/{variableId:guid}")]
        [ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest, "application/json")]
        [ProducesResponseType(typeof(NotFoundObjectResult), (int)HttpStatusCode.NotFound, "application/json")]
        [ProducesResponseType(typeof(GuildVariable), (int)HttpStatusCode.OK, "application/json")]
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
        [ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest, "application/json")]
        [ProducesResponseType(typeof(NotFoundObjectResult), (int)HttpStatusCode.NotFound, "application/json")]
        [ProducesResponseType(typeof(VariableValueBody), (int)HttpStatusCode.OK, "application/json")]
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
        [ProducesResponseType(typeof(BadRequestObjectResult), (int)HttpStatusCode.BadRequest, "application/json")]
        [ProducesResponseType(typeof(NotFoundObjectResult), (int)HttpStatusCode.NotFound, "application/json")]
        [ProducesResponseType(typeof(GuildVariable), (int)HttpStatusCode.OK, "application/json")]
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
                guild.UpdatedVariables.AddOrUpdate(
                    variableId,
                    variableDescriptor,
                    (_, _) => variableDescriptor
                );
            }

            return Ok(variable);
        }
    }
}
