using Intersect.Server.Database.Logging.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;

namespace Intersect.Server.Database.Logging.Seed
{
    internal partial class SeedTrades : SeedData<TradeHistory>
    {
        public override void Seed(DbSet<TradeHistory> dbSet)
        {
            var chunkSize = 1000;
            for (short i = 0; i < 10; ++i)
            {
                var tradeHistories = Enumerable.Range(0, chunkSize).SelectMany(j =>
                {
                    var ij = i * chunkSize + j;
                    var octets = new byte[]
                    {
                        (byte)((ij >> 24) & 0xff),
                        (byte)((ij >> 16) & 0xff),
                        (byte)((ij >>  8) & 0xff),
                        (byte)((ij >>  0) & 0xff),
                        default,
                        default,
                        default,
                        default
                    };

                    var sourceItems = new Item[]
                            {
                                new Item
                                {
                                    ItemId = Guid.NewGuid(),
                                    Quantity = j
                                }
                            };
                    var targetItems = new Item[]
                            {
                                new Item
                                {
                                    ItemId = Guid.NewGuid(),
                                    Quantity = i
                                }
                            };

                    return new[]
                    {
                        new TradeHistory
                        {
                            TradeId = new Guid(j, default, i, octets),
                            UserId = new Guid(j, default, i, octets),
                            PlayerId = new Guid(j, 0, i, octets),
                            Ip = string.Join(".", octets.Take(4).Select(o => Convert.ToString(o, 10))),
                            TimeStamp = DateTime.UtcNow.AddMilliseconds(ij),
                            TargetId = new Guid(j, 1, i, octets),
                            Items = sourceItems,
                            TargetItems = targetItems
                        },
                        new TradeHistory
                        {
                            TradeId = new Guid(j, default, i, octets),
                            UserId = new Guid(j, 1, i, octets),
                            PlayerId = new Guid(j, 1, i, octets),
                            Ip = string.Join(".", octets.Take(4).Select(o => Convert.ToString(o, 10))),
                            TimeStamp = DateTime.UtcNow.AddMilliseconds(ij),
                            TargetId = new Guid(j, 0, i, octets),
                            Items = targetItems,
                            TargetItems = sourceItems
                        }
                    };
                });
                dbSet.AddRange(tradeHistories);
            }
        }
    }
}
