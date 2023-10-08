﻿using Intersect.Enums;
using Intersect.Server.Database;
using Intersect.Server.Database.Logging.Entities;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.RestApi.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [Route("api/v1/logs")]
    [Authorize]
    public sealed partial class LogsController : IntersectController
    {

        [HttpGet("chat")]
        public DataPage<ChatHistory> ListChat(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] int limit = PAGE_SIZE_MAX,
            [FromQuery] int messageType = -1,
            [FromQuery] Guid userId = default,
            [FromQuery] Guid playerId = default,
            [FromQuery] Guid guildId = default,
            [FromQuery] string search = null,
            [FromQuery] SortDirection sortDirection = SortDirection.Ascending
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

                if (guildId != Guid.Empty)
                {
                    messages = messages.Where(m => m.MessageType == ChatMessageType.Guild && m.TargetId == guildId);
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

        [HttpGet("pm")]
        public DataPage<ChatHistory> ListPMs(
            [FromQuery] Guid player1Id,
            [FromQuery] Guid player2Id,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] int limit = PAGE_SIZE_MAX,
            [FromQuery] string search = null,
            [FromQuery] SortDirection sortDirection = SortDirection.Ascending
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
            var targetIds = messages.Where(m => m.TargetId != Guid.Empty && m.MessageType == ChatMessageType.PM).GroupBy(m => m.TargetId).Select(m => m.First().TargetId).ToList();

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

        [HttpGet("user/{userId:guid}/ip")]
        public DataPage<IpAddress> ListIpHistory(
            Guid userId,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] int limit = PAGE_SIZE_MAX
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


        [HttpGet("user/{userId:guid}/activity")]
        public DataPage<UserActivityHistory> ListUserActivity(
            Guid userId,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] int limit = PAGE_SIZE_MAX,
            [FromQuery] SortDirection sortDirection = SortDirection.Ascending
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

        [HttpGet("player/{playerId:guid}/activity")]
        public DataPage<UserActivityHistory> ListPlayerActivity(
            Guid playerId,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] int limit = PAGE_SIZE_MAX,
            [FromQuery] SortDirection sortDirection = SortDirection.Ascending
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

        [HttpGet("trade")]
        public DataPage<TradeHistory> ListTrades(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] int limit = PAGE_SIZE_MAX,
            [FromQuery] Guid userId = default,
            [FromQuery] Guid playerId = default,
            [FromQuery] SortDirection sortDirection = SortDirection.Ascending
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, 100), 5);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            var start = DateTime.UtcNow;

            using (var context = DbInterface.CreateLoggingContext())
            {
                var trades = context.TradeHistory.AsQueryable();

                if (userId != Guid.Empty)
                {
                    trades = trades.Where(m => m.UserId == userId);
                }

                if (playerId != Guid.Empty)
                {
                    trades = trades.Where(m => m.PlayerId == playerId);
                }

                if (sortDirection == SortDirection.Ascending)
                {
                    trades = trades.OrderBy(m => m.TimeStamp);
                }
                else
                {
                    trades = trades.OrderByDescending(m => m.TimeStamp);
                }

                var doGroupBy = userId == default && playerId == default;
                var pageScalar = doGroupBy ? 2 : 1;
                var skip = page * pageSize * pageScalar;
                var take = limit * pageScalar;

                trades = trades.Skip(skip).Take(take);

                if (doGroupBy)
                {
                    trades = trades.GroupBy(t => t.TradeId, (key, group) => group.OrderBy(gt => gt.PlayerId)).Select(t => t.First());
                }

                var values = trades.ToList();

                PopulateTradeNames(values);

                var delta = DateTime.UtcNow - start;

                return new DataPage<TradeHistory>
                {
                    Total = trades.Count(),
                    Page = page,
                    PageSize = pageSize,
                    Count = values.Count,
                    Sort = new[] { Sort.From(nameof(TradeHistory.TimeStamp), sortDirection) },
                    Values = values,
#if DEBUG
                    Extra = delta
#endif
                };
            }
        }

        private void PopulateTradeNames(List<TradeHistory> trades)
        {
            var userIds = trades.Where(m => m.UserId != Guid.Empty).GroupBy(m => m.UserId).Select(m => m.First().UserId).ToList();
            var playerIds = trades.Where(m => m.PlayerId != Guid.Empty).GroupBy(m => m.PlayerId).Select(m => m.First().PlayerId).ToList();
            var targetIds = trades.Where(m => m.TargetId != Guid.Empty).GroupBy(m => m.TargetId).Select(m => m.First().TargetId).ToList();

            var playerSet = new HashSet<Guid>(playerIds);
            playerSet.UnionWith(targetIds);

            using (var db = DbInterface.CreatePlayerContext(true))
            {
                var users = db.Users.Where(u => userIds.Contains(u.Id)).Select(u => new KeyValuePair<Guid, string>(u.Id, u.Name)).ToDictionary(p => p.Key, p => p.Value);
                var players = db.Players.Where(p => playerSet.Contains(p.Id)).Select(p => new KeyValuePair<Guid, string>(p.Id, p.Name)).ToDictionary(p => p.Key, p => p.Value);

                foreach (var msg in trades)
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


        [HttpGet("guild/{guildId:guid}/activity")]
        public DataPage<GuildHistory> ListGuildActivity(
            Guid guildId,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 0,
            [FromQuery] int limit = PAGE_SIZE_MAX,
            [FromQuery] SortDirection sortDirection = SortDirection.Ascending
        )
        {
            page = Math.Max(page, 0);
            pageSize = Math.Max(Math.Min(pageSize, 100), 5);
            limit = Math.Max(Math.Min(limit, pageSize), 1);

            using (var context = DbInterface.LoggingContext)
            {
                var guildActivity = context.GuildHistory.AsQueryable();

                if (guildId != Guid.Empty)
                {
                    guildActivity = guildActivity.Where(m => m.GuildId == guildId);
                }

                if (sortDirection == SortDirection.Ascending)
                {
                    guildActivity = guildActivity.OrderBy(m => m.TimeStamp);
                }
                else
                {
                    guildActivity = guildActivity.OrderByDescending(m => m.TimeStamp);
                }

                var values = guildActivity.Skip(page * pageSize).Take(pageSize).ToList();

                PopulateGuildActivityNames(values);

                if (limit != pageSize)
                {
                    values = values.Take(limit).ToList();
                }

                return new DataPage<GuildHistory>
                {
                    Total = guildActivity.Count(),
                    Page = page,
                    PageSize = pageSize,
                    Count = values.Count,
                    Values = values
                };
            }
        }

        private void PopulateGuildActivityNames(List<GuildHistory> guildActivity)
        {
            var userIds = guildActivity.Where(m => m.UserId != Guid.Empty).GroupBy(m => m.UserId).Select(m => m.First().UserId).ToList();
            var playerIds = guildActivity.Where(m => m.PlayerId != Guid.Empty).GroupBy(m => m.PlayerId).Select(m => m.First().PlayerId).ToList();
            var targetIds = guildActivity.Where(m => m.InitiatorId != Guid.Empty).GroupBy(m => m.InitiatorId).Select(m => m.First().InitiatorId).ToList();

            var playerSet = new HashSet<Guid>(playerIds);
            playerSet.UnionWith(targetIds);

            using (var db = DbInterface.CreatePlayerContext(true))
            {
                var users = db.Users.Where(u => userIds.Contains(u.Id)).Select(u => new KeyValuePair<Guid, string>(u.Id, u.Name)).ToDictionary(p => p.Key, p => p.Value);
                var players = db.Players.Where(p => playerSet.Contains(p.Id)).Select(p => new KeyValuePair<Guid, string>(p.Id, p.Name)).ToDictionary(p => p.Key, p => p.Value);

                foreach (var msg in guildActivity)
                {
                    if (users.ContainsKey(msg.UserId))
                    {
                        msg.Username = users[msg.UserId];
                    }

                    if (players.ContainsKey(msg.PlayerId))
                    {
                        msg.PlayerName = players[msg.PlayerId];
                    }

                    if (players.ContainsKey(msg.InitiatorId))
                    {
                        msg.InitiatorName = players[msg.InitiatorId];
                    }
                }
            }

        }
    }
}