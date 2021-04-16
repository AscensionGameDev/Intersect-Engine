using Intersect.Enums;
using Intersect.Server.Database;
using Intersect.Server.Database.Logging.Entities;
using Intersect.Server.Entities;
using Intersect.Server.Web.RestApi;
using Intersect.Server.Web.RestApi.Attributes;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.RestApi.Types;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Intersect.Server.Web.RestApi.Routes.V1
{

    [RoutePrefix("logs")]
    [ConfigurableAuthorize]
    public sealed class LogsController : IntersectApiController
    {

        [Route("chat")]
        [HttpGet]
        public DataPage<ChatHistory> ListChat(
            [FromUri] int page = 0,
            [FromUri] int pageSize = 0,
            [FromUri] int limit = PAGE_SIZE_MAX,
            [FromUri] int messageType = -1,
            [FromUri] Guid userId = default(Guid),
            [FromUri] Guid playerId = default(Guid),
            [FromUri] string search = null,
            [FromUri] SortDirection sortDirection = SortDirection.Ascending
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, 100), 5);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            using (var context = DbInterface.LoggingContext)
            {
                var messages = context.ChatHistory.AsQueryable();

                if (messageType > -1)
                {
                    messages = messages.Where(m => m.MessageType == (ChatMessageType)messageType);
                }

                if (userId != Guid.Empty)
                {
                    messages = messages.Where(m => m.UserId == userId);
                }

                if (playerId != Guid.Empty)
                {
                    if (messageType == (int)ChatMessageType.PM)
                    {
                        messages = messages.Where(m => m.PlayerId == playerId || m.TargetId == playerId);
                    }
                    else
                    {
                        messages = messages.Where(m => m.PlayerId == playerId);
                    }
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    messages = messages.Where(m => EF.Functions.Like(m.MessageText, $"%{search}%"));
                }

                if (sortDirection == SortDirection.Ascending)
                {
                    messages = messages.OrderBy(m => m.TimeStamp);
                }
                else
                {
                    messages = messages.OrderByDescending(m => m.TimeStamp);
                }

                var values = messages.Skip(page * pageSize).Take(pageSize).ToList();

                PopulateMessageNames(values);

                if (limit != pageSize)
                {
                    values = values.Take(limit).ToList();
                }

                return new DataPage<ChatHistory>
                {
                    Total = messages.Count(),
                    Page = page,
                    PageSize = pageSize,
                    Count = values.Count,
                    Values = values
                };
            }
        }

        [Route("pm")]
        [HttpGet]
        public DataPage<ChatHistory> ListPMs(
            [FromUri] Guid player1Id,
            [FromUri] Guid player2Id,
            [FromUri] int page = 0,
            [FromUri] int pageSize = 0,
            [FromUri] int limit = PAGE_SIZE_MAX,
            [FromUri] string search = null,
            [FromUri] SortDirection sortDirection = SortDirection.Ascending
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, 100), 5);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            using (var context = DbInterface.LoggingContext)
            {
                var messages = context.ChatHistory.AsQueryable();

                messages = messages.Where(m => m.MessageType == ChatMessageType.PM);

                messages = messages.Where(m => (m.PlayerId == player1Id && m.TargetId == player2Id) || (m.PlayerId == player2Id && m.TargetId == player1Id));

                if (!string.IsNullOrWhiteSpace(search))
                {
                    messages = messages.Where(m => EF.Functions.Like(m.MessageText, $"%{search}%"));
                }

                if (sortDirection == SortDirection.Ascending)
                {
                    messages = messages.OrderBy(m => m.TimeStamp);
                }
                else
                {
                    messages = messages.OrderByDescending(m => m.TimeStamp);
                }

                var values = messages.Skip(page * pageSize).Take(pageSize).ToList();

                PopulateMessageNames(values);

                if (limit != pageSize)
                {
                    values = values.Take(limit).ToList();
                }

                return new DataPage<ChatHistory>
                {
                    Total = messages.Count(),
                    Page = page,
                    PageSize = pageSize,
                    Count = values.Count,
                    Values = values
                };
            }
        }

        private void PopulateMessageNames(List<ChatHistory> messages)
        {
            var userIds = messages.Where(m => m.UserId != Guid.Empty).GroupBy(m => m.UserId).Select(m => m.First().UserId).ToList();
            var playerIds = messages.Where(m => m.PlayerId != Guid.Empty).GroupBy(m => m.PlayerId).Select(m => m.First().PlayerId).ToList();
            var targetIds = messages.Where(m => m.TargetId != Guid.Empty).GroupBy(m => m.TargetId).Select(m => m.First().PlayerId).ToList();

            var playerSet = new HashSet<Guid>(playerIds);
            playerSet.UnionWith(targetIds);

            using (var db = DbInterface.CreatePlayerContext(true))
            {
                var users = db.Users.Where(u => userIds.Contains(u.Id)).Select(u => new KeyValuePair<Guid, string>(u.Id, u.Name)).ToDictionary(p => p.Key, p => p.Value);
                var players = db.Players.Where(p => playerSet.Contains(p.Id)).Select(p => new KeyValuePair<Guid, string>(p.Id, p.Name)).ToDictionary(p => p.Key, p => p.Value);

                foreach (var msg in messages)
                {
                    if (users.ContainsKey(msg.UserId))
                    {
                        msg.Username = users[msg.UserId];
                    }

                    if (players.ContainsKey(msg.PlayerId))
                    {
                        msg.PlayerName = players[msg.PlayerId];
                    }

                    if (players.ContainsKey(msg.TargetId))
                    {
                        msg.TargetName = players[msg.TargetId];
                    }
                }
            }

        }

        [Route("user/{userId:guid}/ip")]
        [HttpGet]
        public DataPage<IpAddress> ListIpHistory(
            Guid userId,
            [FromUri] int page = 0,
            [FromUri] int pageSize = 0,
            [FromUri] int limit = PAGE_SIZE_MAX
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, 100), 5);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            using (var context = DbInterface.LoggingContext)
            {
                var history = context.UserActivityHistory.AsQueryable();
                var ipAddresses = history.Where(m => m.UserId == userId && m.UserId != Guid.Empty && !string.IsNullOrWhiteSpace(m.Ip)).OrderByDescending(m => m.TimeStamp).GroupBy(m => m.Ip).Select(m => new IpAddress { Ip = m.First().Ip, LastUsed = m.First().TimeStamp });
                var addresses = ipAddresses.Skip(page * pageSize).Take(pageSize).ToList();

                //Foreach IP Address Find Other Users
                using (var ctx = DbInterface.CreatePlayerContext(true))
                {
                    foreach (var addr in addresses)
                    {
                        var otherUsers = context.UserActivityHistory.Where(m => m.Ip == addr.Ip && m.UserId != userId && !string.IsNullOrWhiteSpace(m.Ip)).GroupBy(m => m.UserId).Select(m => m.First().UserId).ToList();
                        foreach (var usr in otherUsers)
                        {
                            var name = ctx.Users.Where(u => u.Id == usr).Select(u => u.Name).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                addr.OtherUsers.Add(usr, name);
                            }
                        }
                    }
                }

                return new DataPage<IpAddress>()
                {
                    Total = ipAddresses.Count(),
                    Page = page,
                    PageSize = pageSize,
                    Count = addresses.Count,
                    Values = addresses
                };

            }
        }


        [Route("user/{userId:guid}/activity")]
        [HttpGet]
        public DataPage<UserActivityHistory> ListUserActivity(
            Guid userId,
            [FromUri] int page = 0,
            [FromUri] int pageSize = 0,
            [FromUri] int limit = PAGE_SIZE_MAX,
            [FromUri] SortDirection sortDirection = SortDirection.Ascending
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, 100), 5);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            using (var context = DbInterface.LoggingContext)
            {
                var activity = context.UserActivityHistory.Where(m => m.UserId == userId);

                if (sortDirection == SortDirection.Ascending)
                {
                    activity = activity.OrderBy(m => m.TimeStamp);
                }
                else
                {
                    activity = activity.OrderByDescending(m => m.TimeStamp);
                }

                var values = activity.Skip(page * pageSize).Take(pageSize).ToList();

                return new DataPage<UserActivityHistory>()
                {
                    Total = activity.Count(),
                    Page = page,
                    PageSize = pageSize,
                    Count = values.Count,
                    Values = values
                };

            }

        }

        [Route("player/{playerId:guid}/activity")]
        [HttpGet]
        public DataPage<UserActivityHistory> ListPlayerActivity(
            Guid playerId,
            [FromUri] int page = 0,
            [FromUri] int pageSize = 0,
            [FromUri] int limit = PAGE_SIZE_MAX,
            [FromUri] SortDirection sortDirection = SortDirection.Ascending
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, 100), 5);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            using (var context = DbInterface.LoggingContext)
            {
                var activity = context.UserActivityHistory.Where(m => m.PlayerId == playerId);

                if (sortDirection == SortDirection.Ascending)
                {
                    activity = activity.OrderBy(m => m.TimeStamp);
                }
                else
                {
                    activity = activity.OrderByDescending(m => m.TimeStamp);
                }

                var values = activity.Skip(page * pageSize).Take(pageSize).ToList();

                return new DataPage<UserActivityHistory>()
                {
                    Total = activity.Count(),
                    Page = page,
                    PageSize = pageSize,
                    Count = values.Count,
                    Values = values
                };

            }

        }

    }
}