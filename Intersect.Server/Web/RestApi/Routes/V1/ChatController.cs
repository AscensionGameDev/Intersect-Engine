using Intersect.Server.General;
using Intersect.Server.Networking;
using Intersect.Server.Web.RestApi.Attributes;
using Intersect.Server.Web.RestApi.Types;
using JetBrains.Annotations;
using System;
using System.Web.Http;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [RoutePrefix("chat")]
    [ConfigurableAuthorize]
    public sealed class ChatController : ApiController
    {

        [Route]
        [Route("global")]
        [HttpPost]
        public object SendGlobal([FromBody] ChatMessage chatMessage)
        {
            chatMessage.Color = chatMessage.Color ?? CustomColors.AnnouncementChat;

            try
            {
                PacketSender.SendGlobalMsg(chatMessage.Message, chatMessage.Color, chatMessage.Target);

                return new
                {
                    success = true,
                    chatMessage
                };
            }
            catch (Exception exception)
            {
                return new
                {
                    success = false,
                    chatMessage,
                    exception
                };
            }
        }

        [Route("proximity/{mapId:guid}")]
        [HttpPost]
        public object SendProximity(Guid mapId, [FromBody] ChatMessage chatMessage)
        {
            chatMessage.Color = chatMessage.Color ?? CustomColors.ProximityMsg;

            if (Guid.Empty == mapId)
            {
                return new
                {
                    success = false,
                    mapId,
                    chatMessage,
                    exception = new ArgumentException($@"Invalid map id '{mapId}'.", nameof(mapId))
                };
            }

            try
            {
                return new
                {
                    success = PacketSender.SendProximityMsg(chatMessage.Message, mapId, chatMessage.Color, chatMessage.Target),
                    mapId,
                    chatMessage,
                    exception = new ArgumentOutOfRangeException(nameof(mapId), $@"Map with id '{mapId}' not found.")
                };
            }
            catch (Exception exception)
            {
                return new
                {
                    success = false,
                    chatMessage,
                    exception
                };
            }
        }

        [Route("direct/{name}")]
        [HttpPost]
        public object SendDirect(string name, [FromBody] ChatMessage chatMessage)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new
                {
                    success = false,
                    name,
                    chatMessage,
                    exception = new ArgumentException($@"Invalid name '{name}'.", nameof(name))
                };
            }

            var client = Globals.Clients.Find(lookupClient => string.Equals(name, lookupClient?.Entity?.Name, StringComparison.OrdinalIgnoreCase));
            if (client == null)
            {
                return new
                {
                    success = false,
                    name,
                    chatMessage,
                    exception = new ArgumentException($@"No player found named '{name}'.", nameof(name))
                };
            }

            return SendDirect(client, chatMessage);
        }

        [Route("direct/{id:guid}")]
        [HttpPost]
        public object SendDirect(Guid id, [FromBody] ChatMessage chatMessage)
        {
            if (Guid.Empty == id)
            {
                return new
                {
                    success = false,
                    id,
                    chatMessage,
                    exception = new ArgumentException($@"Invalid id '{id}'.", nameof(id))
                };
            }

            var client = Globals.Clients.Find(lookupClient => id == lookupClient?.Entity?.Id);
            if (client == null)
            {
                return new
                {
                    success = false,
                    id,
                    chatMessage,
                    exception = new ArgumentException($@"No player found with id '{id}'.", nameof(id))
                };
            }

            return SendDirect(client, chatMessage);
        }

        private static object SendDirect([NotNull] Client client, ChatMessage chatMessage)
        {
            chatMessage.Color = chatMessage.Color ?? CustomColors.PlayerMsg;

            try
            {
                PacketSender.SendChatMsg(client, chatMessage.Message, chatMessage.Color, chatMessage.Target);

                return new
                {
                    success = true,
                    chatMessage
                };
            }
            catch (Exception exception)
            {
                return new
                {
                    success = false,
                    chatMessage,
                    exception
                };
            }
        }

        // TODO: "party" message endpoint?

    }
}