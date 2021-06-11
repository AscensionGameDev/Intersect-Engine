using Intersect.GameObjects;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Web.RestApi.Attributes;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.RestApi.Types;
using Intersect.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [RoutePrefix("guilds")]
    [ConfigurableAuthorize]
    public sealed class GuildController : IntersectApiController
    {

        [Route]
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

        [Route]
        [HttpGet]
        public DataPage<KeyValuePair<Guild, int>> List(
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

        [Route("{guildId:guid}")]
        [HttpGet]
        public object GuildGet(Guid guildId)
        {
            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Guild not found: ${guildId}.");
            }

            return guild;
        }

        [Route("{guildId:guid}/name")]
        [HttpPost]
        public object ChangeName(Guid guildId, [FromBody] NameChange change)
        {
            if (!FieldChecking.IsValidGuildName(change.Name, Strings.Regex.guildname))
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Invalid guild name."
                );
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
                    return Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        $@"Invalid name, or name already taken."
                    );
                }
            }

            return Request.CreateErrorResponse(
                HttpStatusCode.NotFound,
                $@"No guild with id '{guildId}'."
            );
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

            return Request.CreateErrorResponse(
                HttpStatusCode.NotFound,
                $@"No guild with id '{guildId}'."
            );
        }

        [Route("{guildId:guid}/members")]
        [HttpGet]
        public DataPage<Player> Members(
            Guid guildId,
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

        [Route("{guildId:guid}/bank")]
        [HttpGet]
        public object ItemsListBank(Guid guildId)
        {
            var guild = Guild.LoadGuild(guildId);
            if (guild == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    $@"No guild with id '{guildId}'."
                );
            }

            return guild.Bank;
        }

        [Route("{guildId:guid}/kick/{lookupKey:LookupKey}")]
        [HttpPost]
        public object Kick(Guid guildId, LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }


            var guild = Guild.LoadGuild(guildId);
            
            if (guild == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Guild does not exist."
                );
            }

            var (client, player) = Player.Fetch(lookupKey);

            //Player not found
            if (player == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Player not found."
                );
            }

            //Player is not a member of this guild
            if (!guild.IsMember(player.Id))
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"{player.Name} is not a member of {guild.Name}."
                );
            }
            
            //Cannot kick the owner!
            if (player.GuildRank == 0)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Cannot kick a guild owner, transfer ownership first."
                );
            }

            guild.RemoveMember(player, null, Database.Logging.Entities.GuildHistory.GuildActivityType.Kicked);

            return player;
        }

        [Route("{guildId:guid}/rank/{lookupKey:LookupKey}")]
        [HttpPost]
        public object SetRank(Guid guildId, LookupKey lookupKey, [FromBody] GuildRank guildRank)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }

            if (guildRank.Rank <= 0 || guildRank.Rank >= Options.Instance.Guild.Ranks.Length)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, $@"Invalid guild rank, should be > 0 and < {Options.Instance.Guild.Ranks.Length}."
                );
            }


            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Guild does not exist."
                );
            }

            var (client, player) = Player.Fetch(lookupKey);

            //Player not found
            if (player == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Player not found."
                );
            }

            //Player is not a member of this guild
            if (!guild.IsMember(player.Id))
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"{player.Name} is not a member of {guild.Name}."
                );
            }

            //Cannot kick the owner!
            if (player.GuildRank == 0)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Cannot change rank of the guild owner, transfer ownership first!"
                );
            }

            guild.SetPlayerRank(player, guildRank.Rank, null);

            return player;
        }

        [Route("{guildId:guid}/transfer/{lookupKey:LookupKey}")]
        [HttpPost]
        public object Transfer(Guid guildId, LookupKey lookupKey)
        {
            if (lookupKey.IsInvalid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name."
                );
            }


            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Guild does not exist."
                );
            }

            var (client, player) = Player.Fetch(lookupKey);

            //Player not found
            if (player == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Player not found."
                );
            }

            //Player is not a member of this guild
            if (!guild.IsMember(player.Id))
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"{player.Name} is not a member of {guild.Name}."
                );
            }

            //Cannot kick the owner!
            if (player.GuildRank == 0)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Cannot transfer ownership of a guild to ones self."
                );
            }

            guild.TransferOwnership(player);

            return player;
        }

        [Route("{guildId:guid}/variables")]
        [HttpGet]
        public object GuildVariablesGet(Guid guildId)
        {
            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Guild does not exist."
                );
            }

            return guild.Variables;
        }

        [Route("{guildId:guid}/variables/{variableId:guid}")]
        [HttpGet]
        public object GuildVariableGet(Guid guildId, Guid variableId)
        {
            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Guild does not exist."
                );
            }

            if (variableId == Guid.Empty || GuildVariableBase.Get(variableId) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid variable id ${variableId}.");
            }

            return guild.GetVariable(variableId, true);
        }

        [Route("{guildId:guid}/variables/{variableId:guid}/value")]
        [HttpGet]
        public object GuildVariableGetValue(Guid guildId, Guid variableId)
        {
            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Guild does not exist."
                );
            }

            if (variableId == Guid.Empty || GuildVariableBase.Get(variableId) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid variable id ${variableId}.");
            }

            return new
            {
                value = guild.GetVariable(variableId, true).Value.Value,
            };
        }

        [Route("{guildId:guid}/variables/{variableId:guid}")]
        [HttpPost]
        public object GuildVariableSet(Guid guildId, Guid variableId, [FromBody] VariableValue value)
        {
            var guild = Guild.LoadGuild(guildId);

            if (guild == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    $@"Guild does not exist."
                );
            }

            if (variableId == Guid.Empty || GuildVariableBase.Get(variableId) == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $@"Invalid variable id ${variableId}.");
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
