using Intersect.Server.General;
using Intersect.Server.Networking;
using Intersect.Server.Web.RestApi.Payloads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [Route("api/v1/chat")]
    [Authorize]
    public sealed partial class ChatController : IntersectController
    {
        [HttpPost]
        [HttpPost("global")]
        public object SendGlobal([FromBody] ChatMessage chatMessage)
        {
            try
            {
                PacketSender.SendGlobalMsg(
                    chatMessage.Message, chatMessage.Color ?? CustomColors.Chat.AnnouncementChat, chatMessage.Target
                );

                return new
                {
                    success = true,
                    chatMessage
                };
            }
            catch (Exception exception)
            {
                return InternalServerError(exception.Message);
            }
        }

        [HttpPost("direct/{lookupKey:LookupKey}")]
        public object SendDirect(LookupKey lookupKey, [FromBody] ChatMessage chatMessage)
        {
            if (lookupKey.IsInvalid)
            {
                return BadRequest(lookupKey.IsIdInvalid ? @"Invalid player id." : @"Invalid player name.");
            }

            var client = Globals.Clients.Find(
                lookupClient => string.Equals(
                    lookupKey.Name, lookupClient?.Entity?.Name, StringComparison.OrdinalIgnoreCase
                )
            );

            if (client == null)
            {
                return NotFound($@"No player found for '{lookupKey}'.");
            }

            try
            {
                PacketSender.SendChatMsg(
                    client.Entity, chatMessage.Message, Enums.ChatMessageType.PM, chatMessage.Color ?? CustomColors.Chat.PlayerMsg,
                    chatMessage.Target
                );

                return new
                {
                    success = true,
                    lookupKey,
                    chatMessage
                };
            }
            catch (Exception exception)
            {
                return InternalServerError(exception.Message);
            }
        }

        [HttpPost("proximity/{mapId:guid}")]
        public object SendProximity(Guid mapId, [FromBody] ChatMessage chatMessage)
        {
            if (Guid.Empty == mapId)
            {
                return BadRequest($@"Invalid map id '{mapId}'.");
            }

            try
            {
                if (PacketSender.SendProximityMsg(
                    chatMessage.Message, Enums.ChatMessageType.Local, mapId, chatMessage.Color ?? CustomColors.Chat.ProximityMsg, chatMessage.Target
                ))
                {
                    return new
                    {
                        success = true,
                        mapId,
                        chatMessage
                    };
                }

                return NotFound($@"No map found for '{mapId}'.");
            }
            catch (Exception exception)
            {
                return InternalServerError(exception.Message);
            }
        }

        // TODO: "party" message endpoint?
    }
}
