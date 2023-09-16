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
            if (!FieldChecking.IsValidGuildName(change.Name, Strings.Regex.guildname))
            {
                return BadRequest($@"Invalid guild name.");
            }

            var guild = Guild.LoadGuild(guildId);
            if (guild != null)
            {
                if (guild.Rename(change.Name))
                {
                    return guild;
                }
                else
                {
                    return BadRequest($@"Invalid name, or name already taken.");
                }
            }

            return NotFound($@"No guild with id '{guildId}'.");
        }

        [Route("{guildId:guid}")]
        [HttpDelete]
        public object DisbandGuild(Guid guildId)
        {
            var guild = Guild.LoadGuild(guildId);
            if (guild != null)
            {
                Guild.DeleteGuild(guild);

                return guild;
            }

            return NotFound($@"No guild with id '{guildId}'.");
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

        [HttpGet("{guildId:guid}/bank")]
        public object ItemsListBank(Guid guildId)
        {
            var guild = Guild.LoadGuild(guildId);
            if (guild == null)
            {
                return NotFound($@"No guild with id '{guildId}'.");
            }

            return guild.Bank;
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
                return BadRequest($@"Cannot transfer ownership of a guild to ones self.");
            }

            guild.TransferOwnership(player);

            return player;
        }

        [HttpGet("{guildId:guid}/variables")]
        public object GuildVariablesGet(Guid guildId)
        {
            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return BadRequest($@"Guild does not exist.");
            }

            return guild.Variables;
        }

        [HttpGet("{guildId:guid}/variables/{variableId:guid}")]
        public object GuildVariableGet(Guid guildId, Guid variableId)
        {
            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return BadRequest($@"Guild does not exist.");
            }

            if (variableId == Guid.Empty || GuildVariableBase.Get(variableId) == null)
            {
                return BadRequest($@"Invalid variable id ${variableId}.");
            }

            return guild.GetVariable(variableId, true);
        }

        [HttpGet("{guildId:guid}/variables/{variableId:guid}/value")]
        public object GuildVariableGetValue(Guid guildId, Guid variableId)
        {
            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return BadRequest($@"Guild does not exist.");
            }

            if (variableId == Guid.Empty || GuildVariableBase.Get(variableId) == null)
            {
                return BadRequest($@"Invalid variable id ${variableId}.");
            }

            return new
            {
                value = guild.GetVariable(variableId, true).Value.Value,
            };
        }

        [HttpPost("{guildId:guid}/variables/{variableId:guid}")]
        public object GuildVariableSet(Guid guildId, Guid variableId, [FromBody] VariableValue value)
        {
            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return BadRequest($@"Guild does not exist.");
            }

            if (variableId == Guid.Empty || GuildVariableBase.Get(variableId) == null)
            {
                return BadRequest($@"Invalid variable id ${variableId}.");
            }

            var variable = guild.GetVariable(variableId, true);

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
                guild.StartCommonEventsWithTriggerForAll(Enums.CommonEventTrigger.GuildVariableChange, "", variableId.ToString());
                guild.UpdatedVariables.AddOrUpdate(variableId, GuildVariableBase.Get(variableId), (key, oldValue) => GuildVariableBase.Get(variableId));
            }

            return variable;
        }
    }
}
