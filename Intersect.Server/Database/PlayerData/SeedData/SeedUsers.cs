using System;
using System.Security.Cryptography;
using System.Text;

using Intersect.Enums;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;

using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.PlayerData.SeedData
{

    public class SeedUsers : SeedData<User>
    {

        private static string GenerateSalt(RandomNumberGenerator rng)
        {
            var buffer = new byte[32];
            rng.GetBytes(buffer);

            return BitConverter.ToString(buffer).Replace("-", "");
        }

        private static string HashPassword(string salt, string password)
        {
            using (var algorithm = new SHA256Managed())
            {
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
                var salted = $@"{BitConverter.ToString(hash).Replace("-", "")}{salt}";
                var saltedHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(salted));

                return BitConverter.ToString(saltedHash).Replace("-", "");
            }
        }

        public override void Seed(DbSet<User> dbSet)
        {
            var emptyBytes = new byte[] {0, 0, 0, 0, 0, 0, 0, 1};
            using (var rng = new RNGCryptoServiceProvider())
            {
                for (var n = 0; n < 300; ++n)
                {
                    var salt = GenerateSalt(rng);

                    var userRights = UserRights.None;
                    if (n < 10)
                    {
                        userRights = new UserRights
                        {
                            Editor = true,
                            Ban = true,
                            Kick = true,
                            Mute = true,
                            Api = true,
                            ApiRoles = new ApiRoles
                            {
                                UserManage = true,
                                UserQuery = true
                            }
                        };
                    }
                    else if (n < 20)
                    {
                        userRights = new UserRights
                        {
                            Editor = true,
                            Ban = true,
                            Kick = true,
                            Mute = true,
                            Api = true,
                            ApiRoles = new ApiRoles
                            {
                                UserManage = true,
                                UserQuery = true
                            }
                        };
                    }
                    else if (n < 100)
                    {
                        userRights = UserRights.Admin;
                    }
                    else if (n < 200)
                    {
                        userRights = UserRights.Moderation;
                    }

                    var id = new Guid(n, 0, 0, emptyBytes);
                    var user = new User
                    {
                        Name = n == 0 ? "test" : $@"test{n:D3}",
                        Email = $@"test{n:D3}@test.test",
                        Salt = salt,
                        Password = HashPassword(salt, "test"),
                        Power = userRights
                    };

                    typeof(User).GetProperty("Id")?.SetValue(user, id);

                    dbSet.Add(user);

                    var player = new Player
                    {
                        Id = new Guid(n, 0, 0, emptyBytes),
                        Name = n == 0 ? "test" : $@"test{n:D3}",
                        ClassId = Guid.Empty,
                        Gender = n % 2 == 0 ? Gender.Male : Gender.Female,
                        Level = 1,
                        Exp = 0,
                        StatPoints = 0,

                        Sprite = "1.png",
                        Face = null
                    };

                    for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                    {
                        player.Equipment[i] = -1;
                    }

                    player.SetVital(Vitals.Health, 10);
                    player.SetVital(Vitals.Mana, 10);

                    for (var i = 0; i < (int) Stats.StatCount; i++)
                    {
                        player.Stat[i].BaseStat = 0;
                    }

                    user.Players?.Add(player);
                    player.ValidateLists();
                }
            }
        }

    }

    public static class RandomNumberGeneratorExtensions
    {

        public static int NextInt(
            this RandomNumberGenerator rng,
            int min = int.MinValue,
            int max = int.MaxValue
        )
        {
            var buffer = new byte[4];
            rng.GetBytes(buffer);

            return Math.Max(Math.Min(BitConverter.ToInt32(buffer, 0), max), min);
        }

        public static uint NextUInt(this RandomNumberGenerator rng)
        {
            var buffer = new byte[4];
            rng.GetBytes(buffer);

            return BitConverter.ToUInt32(buffer, 0);
        }

    }

}
