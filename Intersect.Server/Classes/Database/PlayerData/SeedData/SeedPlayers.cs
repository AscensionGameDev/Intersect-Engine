using Intersect.Server.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;

using Intersect.Enums;

namespace Intersect.Server.Database.PlayerData.SeedData
{
    public class SeedPlayers : SeedData<Player>
    {
        public override void Seed(DbSet<Player> dbSet)
        {
            var emptyBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            using (var rng = new RNGCryptoServiceProvider())
            {
                for (var n = 0; n < 1000; ++n)
                {
                    var userId = new Guid(n, 0, 0, emptyBytes);
                    var player = new Player
                    {
                        Id = userId,
                        Name = n == 0 ? "test" : $@"test{n:D3}",
                        ClassId = Guid.Empty,
                        Gender = n % 2 == 0 ? Gender.Male : Gender.Female,
                        Level = 1,
                        Exp = 0,
                        StatPoints = 0,

                        Sprite = "1.png",
                        Face = null
                    };

                    //client.Characters.Add(player);
                    player.ValidateLists();
                    for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                    {
                        player.Equipment[i] = -1;
                    }

                    player.SetVital(Vitals.Health, 10);
                    player.SetVital(Vitals.Mana, 10);

                    for (var i = 0; i < (int)Stats.StatCount; i++)
                    {
                        player.Stat[i].Stat = 0;
                    }

                    typeof(Player).GetProperty("UserId")?.SetValue(player, userId);

                    dbSet.Add(player);
                }
            }
        }
    }
}
