using Intersect.Server.Database.Logging.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;

namespace Intersect.Server.Database.Logging.Seed
{
    internal class SeedTrades : SeedData<TradeHistory>
    {
        public override void Seed(DbSet<TradeHistory> dbSet)
        {
            var chunkSize = 1000;
            for (short i = 0; i < 10; ++i)
            {
                var tradeHistories = Enumerable.Range(0, chunkSize).Select(j =>
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
                    return new TradeHistory
                    {
                        TradeId = new Guid(j, default, i, octets),
                        UserId = new Guid(j, default, i, octets),
                        PlayerId = new Guid(j, default, i, octets),
                        Ip = string.Join(".", octets.Take(4).Select(o => Convert.ToString(o, 10))),
                        TimeStamp = DateTime.UtcNow.AddMilliseconds(ij),
                        TargetId = new Guid(j, default, i, octets),
                        Items = new Item[]
                        {
                            new Item
                            {
                                ItemId = Guid.NewGuid(),
                                Quantity = j
                            }
                        },
                        TargetItems = new Item[]
                        {
                            new Item
                            {
                                ItemId = Guid.NewGuid(),
                                Quantity = i
                            }
                        }
                    };
                });
                dbSet.AddRange(tradeHistories);
            }
        }
    }
}
