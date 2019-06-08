using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.Web.RestApi.Attributes;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [RoutePrefix("users")]
    [ConfigurableAuthorize(Roles = nameof(UserRights.PersonalInformation))]
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
            using (var context = PlayerContext.Temporary)
            {
                return Database.PlayerData.User.FindByName(userName, context);
            }
        }

        [Route("{userId:guid}")]
        [HttpGet]
        public User UserById(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            using (var context = PlayerContext.Temporary)
            {
                return Database.PlayerData.User.FindById(userId, context);
            }
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

            using (var context = PlayerContext.Temporary)
            {
                return Database.PlayerData.User
                    .FindByName(userName, context)?
                    .Players;
            }
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

            using (var context = PlayerContext.Temporary)
            {
                return Database.PlayerData.User
                    .FindById(userId, context)?
                    .Players;
            }
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

            using (var context = PlayerContext.Temporary)
            {
                return Database.PlayerData.User
                    .FindByName(userName, context)?
                    .Players?
                    .FirstOrDefault(player => string.Equals(player?.Name, playerName, StringComparison.Ordinal));
            }
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

            using (var context = PlayerContext.Temporary)
            {
                return Database.PlayerData.User
                    .FindById(userId, context)?
                    .Players?
                    .FirstOrDefault(player => player?.Id == playerId);
            }
        }
    }
}