using System.Net;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Variables;
using Intersect.GameObjects;
using Intersect.Server.Database;
using Intersect.Server.Database.GameData;
using Intersect.Server.Entities;
using Intersect.Server.Web.Http;
using Intersect.Server.Web.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.Controllers.Api.V1;

[Route("api/v1/variables")]
[Authorize]
public sealed partial class VariablesController : IntersectController
{
    [HttpGet]
    [ProducesResponseType(typeof(DataPage<ServerVariableDescriptor>), (int)HttpStatusCode.OK, ContentTypes.Json)]
    public IActionResult ServerVariablesGet([FromQuery] PagingInfo pageInfo)
    {
        pageInfo.Page = Math.Max(pageInfo.Page, 0);
        pageInfo.PageSize = Math.Max(Math.Min(pageInfo.PageSize, 100), 5);
        var entries = GameContext.Queries.ServerVariables(pageInfo.Page, pageInfo.PageSize)?.ToList();

        return Ok(
            new DataPage<ServerVariableDescriptor>
            {
                Total = ServerVariableDescriptor.Lookup.Count,
                Page = pageInfo.Page,
                PageSize = pageInfo.PageSize,
                Count = entries?.Count ?? 0,
                Values = entries,
            }
        );
    }

    [HttpGet("{variableId:guid}")]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
    [ProducesResponseType(typeof(ServerVariableDescriptor), (int)HttpStatusCode.OK, ContentTypes.Json)]
    public IActionResult ServerVariableGet(Guid variableId)
    {
        if (variableId == default)
        {
            return BadRequest($@"Variable id cannot be {variableId}");
        }

        var variable = GameContext.Queries.ServerVariableById(variableId);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (variable == null)
        {
            return NotFound($@"No server variable with id '{variableId}'.");
        }

        return Ok(variable);
    }

    [HttpGet("{variableId:guid}/value")]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
    [ProducesResponseType(typeof(VariableValueBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
    public IActionResult ServerVariableGetValue(Guid variableId)
    {
        if (variableId == default)
        {
            return BadRequest($@"Variable id cannot be {variableId}");
        }

        var variable = GameContext.Queries.ServerVariableById(variableId);

        if (variable == null)
        {
            return NotFound($@"No server variable with id '{variableId}'.");
        }

        return Ok(
            new VariableValueBody
            {
                Value = variable.Value.Value,
            }
        );
    }

    [HttpPost("{variableId:guid}")]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
    [ProducesResponseType(typeof(ServerVariableDescriptor), (int)HttpStatusCode.OK, ContentTypes.Json)]
    public IActionResult ServerVariableSet(Guid variableId, [FromBody] VariableValueBody variableValue)
    {
        if (variableId == default)
        {
            return BadRequest($@"Variable id cannot be {variableId}");
        }

        var variable = GameContext.Queries.ServerVariableById(variableId);

        if (variable == null)
        {
            return NotFound($@"No server variable with id '{variableId}'.");
        }

        var changed = false;
        if (variable.Value != null && variable.Value.Value != variableValue.Value)
        {
            variable.Value.Value = variableValue.Value;
            changed = true;
        }

        // ReSharper disable once InvertIf
        if (changed)
        {
            Player.StartCommonEventsWithTriggerForAll(
                CommonEventTrigger.ServerVariableChange,
                "",
                variableId.ToString()
            );

            _ = DbInterface.UpdatedServerVariables.AddOrUpdate(variable.Id, variable, (_, _) => variable);
        }

        return Ok(variable);
    }
}