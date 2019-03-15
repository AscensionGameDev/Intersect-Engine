using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Entities;
using Intersect.Server.Web.RestApi.Attributes;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [RoutePrefix("users")]
    [Authorize]
    public sealed class UserController : ApiController
    {
        [Route]
        [HttpGet]
        public object List()
        {
            // TODO: Implement user listing with pagination
            return new
            {
            };
        }

        [Route("{userName}")]
        [HttpGet]
        public User UserByName(string userName)
        {
            return Database.PlayerData.User.FindByName(userName);
        }

        [Route("{userId:guid}")]
        [HttpGet]
        public User UserById(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            return Database.PlayerData.User.FindById(userId);
        }

        [Route("{userName}/players")]
        [HttpGet]
        [ConfigurableAuthorize, OverrideAuthorization]
        public List<Player> PlayersByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return null;
            }

            return Database.PlayerData.User
                .FindByName(userName)
                .Players;
        }

        [Route("{userId:guid}/players")]
        [HttpGet]
        [ConfigurableAuthorize, OverrideAuthorization]
        public List<Player> PlayersByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            return Database.PlayerData.User
                .FindById(userId)
                .Players;
        }

        [Route("{userName}/players/{playerName}")]
        [HttpGet]
        [ConfigurableAuthorize, OverrideAuthorization]
        public Player PlayerByNameForUserByName(string userName, string playerName)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(playerName))
            {
                return null;
            }

            return Database.PlayerData.User
                .FindByName(userName)
                .Players
                .FirstOrDefault(player => string.Equals(player?.Name, playerName, StringComparison.Ordinal));
        }

        [Route("{userId:guid}/players/{playerId:guid}")]
        [HttpGet]
        [ConfigurableAuthorize, OverrideAuthorization]
        public Player PlayerByIdForUserById(Guid userId, Guid playerId)
        {
            if (userId == Guid.Empty || playerId == Guid.Empty)
            {
                return null;
            }

            return Database.PlayerData.User
                .FindById(userId)?
                .Players
                .FirstOrDefault(player => player?.Id == playerId);
        }
    }
}