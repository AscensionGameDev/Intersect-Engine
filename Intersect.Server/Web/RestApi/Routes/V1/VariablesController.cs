﻿using Intersect.GameObjects;
using Intersect.Server.Database;
using Intersect.Server.Database.GameData;
using Intersect.Server.Entities;
using Intersect.Server.Web.RestApi.Payloads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [Route("api/v1/variables")]
    [Authorize]
    public sealed partial class VariablesController : IntersectController
    {
        [HttpGet("global")]
        public object GlobalVariablesGet([FromQuery] PagingInfo pageInfo)
        {
            pageInfo.Page = Math.Max(pageInfo.Page, 0);
            pageInfo.Count = Math.Max(Math.Min(pageInfo.Count, 100), 5);

            var entries = GameContext.Queries.ServerVariables(pageInfo.Page, pageInfo.Count)?.ToList();

            return new
            {
                total = ServerVariableBase.Lookup.Count(),
                pageInfo.Page,
                count = entries?.Count ?? 0,
                entries
            };
        }

        [HttpGet("global/{guid:guid}")]
        public object GlobalVariableGet(Guid guid)
        {
            if (Guid.Empty == guid)
            {
                return BadRequest(@"Invalid global variable id.");
            }

            var variable = GameContext.Queries.ServerVariableById(guid);

            if (variable == null)
            {
                return NotFound($@"No global variable with id '{guid}'.");
            }

            return variable;
        }

        [HttpGet("global/{guid:guid}/value")]
        public object GlobalVariableGetValue(Guid guid)
        {
            if (Guid.Empty == guid)
            {
                return BadRequest(@"Invalid global variable id.");
            }

            var variable = GameContext.Queries.ServerVariableById(guid);

            if (variable == null)
            {
                return NotFound($@"No global variable with id '{guid}'.");
            }

            return new
            {
                value = variable?.Value.Value,
            };
        }

        [HttpPost("global/{guid:guid}")]
        public object GlobalVariableSet(Guid guid, [FromBody] VariableValue value)
        {
            if (Guid.Empty == guid)
            {
                return BadRequest(@"Invalid global variable id.");
            }

            var variable = GameContext.Queries.ServerVariableById(guid);

            if (variable == null)
            {
                return NotFound($@"No global variable with id '{guid}'.");
            }

            var changed = true;
            if (variable.Value?.Value == value.Value)
            {
                changed = false;
            }
            variable.Value.Value = value.Value;

            if (changed)
            {
                Player.StartCommonEventsWithTriggerForAll(Enums.CommonEventTrigger.ServerVariableChange, "", guid.ToString());
            }
            DbInterface.UpdatedServerVariables.AddOrUpdate(variable.Id, variable, (key, oldValue) => variable);

            return variable;
        }
    }
}
