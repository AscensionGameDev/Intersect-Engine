using System.Net;
using Intersect.Server.Collections.Indexing;
using Intersect.Server.General;
using Intersect.Server.Networking;
using Intersect.Server.Web.Http;
using Intersect.Server.Web.Types;
using Intersect.Server.Web.Types.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.Controllers.Api.V1
{
    [Route("api/v1/chat")]
    [Authorize]
    public sealed partial class ChatController : IntersectController
    {
        [HttpPost]
        [ProducesResponseType(typeof(ChatMessageResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult SendGlobal([FromBody] ChatMessage chatMessage)
        {
            PacketSender.SendGlobalMsg(chatMessage.Message, chatMessage.Color ?? CustomColors.Chat.AnnouncementChat, chatMessage.Target);
            return Ok(new ChatMessageResponseBody(true, chatMessage));
        }

        [HttpPost("direct/{lookupKey:LookupKey}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(ChatMessageLookupKeyResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult SendDirect(LookupKey lookupKey, [FromBody] ChatMessage chatMessage)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            var client = Client.Instances.Find(
                lookupClient => string.Equals(
                    lookupKey.Name, lookupClient?.Entity?.Name, StringComparison.OrdinalIgnoreCase
                )
            );

            if (client == null)
            {
                return NotFound($@"No player found for '{lookupKey}'.");
            }

            PacketSender.SendChatMsg(
                client.Entity,
                chatMessage.Message,
                Enums.ChatMessageType.PM,
                chatMessage.Color ?? CustomColors.Chat.PlayerMsg,
                chatMessage.Target
            );

            return Ok(new ChatMessageLookupKeyResponseBody(lookupKey, true, chatMessage));
        }

        [HttpPost("proximity/{mapId:guid}")]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
        [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
        [ProducesResponseType(typeof(ChatMessageMapResponseBody), (int)HttpStatusCode.OK, ContentTypes.Json)]
        public IActionResult SendProximity(Guid mapId, [FromBody] ChatMessage chatMessage)
        {
            if (Guid.Empty == mapId)
            {
                return BadRequest($@"Invalid map id '{mapId}'.");
            }

            bool success = PacketSender.SendProximityMsg(
                chatMessage.Message,
                Enums.ChatMessageType.Local,
                mapId,
                chatMessage.Color ?? CustomColors.Chat.ProximityMsg,
                chatMessage.Target
            );

            if (!success)
            {
                return NotFound($@"No map found for '{mapId}'.");
            }

            return Ok(new ChatMessageMapResponseBody(mapId, true, chatMessage));
        }
    }
}
